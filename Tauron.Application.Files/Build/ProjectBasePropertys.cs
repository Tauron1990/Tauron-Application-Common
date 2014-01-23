using System;
using System.Linq;
using Microsoft.Build.Construction;
using Tauron.Application.Files.Build.Conditions;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Build
{
    [PublicAPI]
    public sealed class ProjectBasePropertys
    {
        public const string ConfigurationProperty = "Configuration";
        public const string PlatformProperty = "Platform";
        public const string ProjectGuidProperty = "ProjectGuid";
        public const string OutputTypeProperty = "OutputType";
        public const string AssemblyNameProperty = "AssemblyName";
        public const string TargetFrameworkVersionProperty = "TargetFrameworkVersion";
        public const string WarningLevelProperty = "WarningLevel";
        public const string SolutionDirProperty = "SolutionDir";

        private readonly ProjectRootElement _element;
        private ProjectPropertyGroupElement _rootGroup;

        internal ProjectBasePropertys([NotNull] ProjectRootElement element)
        {
            _element = element;
        }

        [CanBeNull]
        public ProjectPropertyElement SearchProperty([NotNull] string name)
        {
            return _element.Properties.FirstOrDefault(prop => prop.Name == name);
        }

        public void SetProperty([NotNull] string name, [CanBeNull] string value)
        {
            if (value == null)
            {
                var temp = SearchProperty(name);
                if (temp != null)
                    _rootGroup.RemoveChild(temp);
            }
            else
                _rootGroup.SetProperty(name, value);
        }

        [NotNull]
        public string Configuration
        {
            get
            {
                var temp = SearchProperty(ConfigurationProperty);
                return temp == null ? string.Empty : temp.Value;
            }
            set
            {
                SetProperty(ConfigurationProperty, value);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "No security relevance")]
        public Platform Platform
        {
            get
            {
                var temp = SearchProperty(PlatformProperty);
                Platform plt;
                if (temp != null && Enum.TryParse(temp.Value, true, out plt))
                    return plt;
                return 0;
            }
            set
            {
                SetProperty(PlatformProperty, value == 0 ? null : value.ToString().ToLowerInvariant());
            }
        }

        public Guid ProjectGuid
        {
            get
            {
                var temp = SearchProperty(ProjectGuidProperty);
                if (temp == null) return Guid.Empty;

                Guid guid;
                return Guid.TryParse(temp.Value, out guid) ? guid : Guid.Empty;
            }
            set
            {
                SetProperty(ProjectGuidProperty, value.ToString());
            }
        }

        [CanBeNull]
        public ProjectPropertyElement OutputType
        {
            get
            {
                return SearchProperty(OutputTypeProperty);
            }
        }

        [CanBeNull]
        public ProjectPropertyElement AssemblyName
        {
            get
            {
                return SearchProperty(AssemblyNameProperty);
            }
        }

        [CanBeNull]
        public ProjectPropertyElement TargetFrameworkVersion
        {
            get
            {
                return SearchProperty("TargetFrameworkVersion");
            }
        }

        [CanBeNull]
        public ProjectPropertyElement WarningLevel
        {
            get
            {
                return SearchProperty(WarningLevelProperty);
            }
        }

        [CanBeNull]
        public ProjectPropertyElement SolutionDir
        {
            get
            {
                return SearchProperty(SolutionDirProperty);
            }
        }

        internal void InitializePropertys()
        {
            var rootGroup = _element.AddPropertyGroup();

            rootGroup.AddProperty(ConfigurationProperty, "Debug").Condition =
                CreateEmptyCheckCondidition(ConfigurationProperty);

            rootGroup.AddProperty(PlatformProperty, "AnyCpu").Condition =
                CreateEmptyCheckCondidition(PlatformProperty);

            rootGroup.AddProperty(ProjectGuidProperty, Guid.NewGuid().ToString("B"));
            rootGroup.AddProperty(OutputTypeProperty, OutputTypes.Library);
            rootGroup.AddProperty(AssemblyNameProperty, "Unkown");
            rootGroup.AddProperty(TargetFrameworkVersionProperty, TargetFrameworkVersions.V45);
            rootGroup.AddProperty(WarningLevelProperty, "4");

            rootGroup.AddProperty(SolutionDirProperty, "..\\").Condition =
                new ConditionBuilder
                {
                    Condition = new OrOperator
                    {
                        Left = new EqualOperator
                        {
                            Left = new PropertyReference {PropertyName = SolutionDirProperty},
                            Right = new StaticReference {Value = string.Empty}
                        },
                        Right = new EqualOperator
                        {
                            Left = new PropertyReference {PropertyName = SolutionDirProperty},
                            Right = new StaticReference {Value = "*Undefined*"}
                        }

                    }
                }.ToString();

            _rootGroup = rootGroup;
        }

        [NotNull]
        public ProjectPropertyElement CreatePropertyInRoot([NotNull] string name, [NotNull] string value)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (value == null) throw new ArgumentNullException("value");

            return _rootGroup == null ? _element.AddProperty(name, value) : _rootGroup.AddProperty(name, value);
        }

        [NotNull]
        private static string CreateEmptyCheckCondidition([NotNull] string propName)
        {
            return new ConditionBuilder
            {
                Condition = new EqualOperator
                {
                    Left = new PropertyReference { PropertyName = propName },
                    Right = new StaticReference { Value = string.Empty }
                }
            }.ToString();
        }
    }
}
