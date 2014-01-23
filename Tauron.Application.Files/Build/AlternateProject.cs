using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Tauron.Application.Files.Build.Conditions;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Build
{
    [PublicAPI]
    public static class ProjectType
    {
        public const string CSharpProject = @"$(MSBuildToolsPath)\Microsoft.CSharp.targets";
        public const string VBProject = @"$(MSBuildToolsPath)\Microsoft.VisualBasic.targets";

        public const string CommonProps = @"$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props";

        public const string CommonPropsCondition =
            @"Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')";
    }

    [PublicAPI]
    public class AlternateProject
    {
        public const string DefaultReleaseProfile = "Release|AnyCpu";
        public const string DefaultDebugProfile = "Debug|AnyCpu";
        public const string ProjectGuidProperty = "ProjectGuid";

        private Project _project;
        private List<SimpleProjectProfile> _projectProfiles;
        private ProjectImportElement _projectTypeImport;

        private readonly AlternateProjectCollection _projectCollection;
        [NotNull]
        public ProjectRootElement RootElement { get; private set; }

        [NotNull]
        public ProjectBasePropertys BasePropertys { get; private set; }

        [NotNull]
        public ReferenceManager ReferenceManager { get; private set; }

        [NotNull]
        public ProjectCompilation ProjectCompilation { get; private set; }

        public Guid ProjectGuid
        {
            get
            {
                return BasePropertys.ProjectGuid;
            }
            set
            {
                BasePropertys.ProjectGuid = value;
            }
        }

        [NotNull]
        public string FilePath { get { return _project == null ? string.Empty : _project.FullPath; } }

        [NotNull]
        public IEnumerable<SimpleProjectProfile> ProjectProfiles
        {
            get
            {
                return new ReadOnlyCollection<SimpleProjectProfile>(_projectProfiles);
            }
        }

        public AlternateProject([NotNull] string filePath, [NotNull] AlternateProjectCollection collection)
            : this(new Project(filePath), collection)
        {
        }

        public AlternateProject([NotNull] Project project, [NotNull] AlternateProjectCollection projectCollection)
        {
            if (project == null) throw new ArgumentNullException("project");
            if (projectCollection == null) throw new ArgumentNullException("projectCollection");

            _project = project;
            _projectCollection = projectCollection;

            InitializeRoot(_project.Xml);
        }

        public AlternateProject([NotNull] AlternateProjectCollection projectCollection)
        {
            if (projectCollection == null) throw new ArgumentNullException("projectCollection");
            _projectCollection = projectCollection;

            InitializeRoot(ProjectRootElement.Create(projectCollection.ProjectCollection));
        }

        private void InitializeRoot([NotNull] ProjectRootElement element)
        {
            const string profileConditionFlag = "'$(Configuration)|$(Platform)' ==";

            RootElement = element;
            BasePropertys = new ProjectBasePropertys(element);
            ReferenceManager = new ReferenceManager(RootElement);
            ProjectCompilation = new ProjectCompilation(RootElement, ReferenceManager.UsedItemGroupElements);

            _projectProfiles = new List<SimpleProjectProfile>();

            foreach (
                var propertyGroup in
                    element.PropertyGroups.Where(
                        propertyGroup => propertyGroup.Condition.Trim().StartsWith(profileConditionFlag, StringComparison.Ordinal)))
            {
                _projectProfiles.Add(new SimpleProjectProfile(this,
                    propertyGroup.Condition.Replace(profileConditionFlag, string.Empty).Trim(' ', '\t', '\''), propertyGroup));
            }

            _projectCollection.Add(this);
        }

        [NotNull]
        public Project CreateProject()
        {
            return _project ?? (_project = new Project(RootElement));
        }

        public void InitializeBasePropertys()
        {
            RootElement.DefaultTargets = "Build";
            RootElement.AddImport(ProjectType.CommonProps).Condition = ProjectType.CommonPropsCondition;

            BasePropertys.InitializePropertys();   
        }

        public void InitializeDefaultProfiles()
        {
            CreateAndInitProfile(DefaultDebugProfile);
            CreateAndInitProfile(DefaultReleaseProfile);
        }

        public void SetProjectType([NotNull] string type)
        {
            if (_projectTypeImport == null)
            {
                _projectTypeImport = RootElement.Imports.OrderBy(imp => imp.ProjectLocation.Line).ElementAtOrDefault(1);
                if (_projectTypeImport != null) return;
                _projectTypeImport = RootElement.AddImport(type);
                return;
            }

            _projectTypeImport.Project = type;
        }

        private void CreateAndInitProfile([NotNull] string name)
        {
            SimpleProjectProfile profile = CreateProfile(name);

            profile.DebugSymbols = profile.DebugSymbolsDefaultValue;
            profile.DebugType = profile.DebugTypeDefaultValue;
            profile.DefineConstants = profile.DefineConstantsDefaultValue;
            profile.ErrorReport = profile.ErrorReportDefaultValue;
            profile.Optimize = profile.OptimizeDefaultValue;
            profile.OutputPath = profile.OutputPathDefaultValue;
            profile.WarningLevel = profile.WarningLevelDefaultValue;
        }

        [NotNull]
        public SimpleProjectProfile CreateProfile([NotNull] string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            name = name.Trim();

            SimpleProjectProfile profile = _projectProfiles.Find(projectProfile => projectProfile.Name == name);
            if (profile != null) return profile;

            ProjectPropertyGroupElement profileRoot = RootElement.AddPropertyGroup();

            profileRoot.Condition = new ConditionBuilder
            {
                Condition = new EqualOperator
                {
                    Left = new MultiPropertyReference(true, "Configuration", "Platform"),
                    Right = new StaticReference {Value = name}
                }
            }.ToString();

            profile = new SimpleProjectProfile(this, name, profileRoot);
            _projectProfiles.Add(profile);

            return profile;
        }

        public void Save([NotNull] string fileLocation)
        {
            if (fileLocation == null) throw new ArgumentNullException("fileLocation");

            CreateProject().Save(fileLocation);
        }
    }
}