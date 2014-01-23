using System;
using System.Globalization;
using System.Linq;
using Microsoft.Build.Construction;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Build
{
    [PublicAPI]
    public enum ErrorReport
    {
        None,
        Prompt,
        Send,
        Queue
    }

    [PublicAPI]
    public enum Platform
    {
        Unknown,
        AnyCpu,
        X86,
        X64
    }

    [PublicAPI]
    public enum DebugType
    {
        None,
        Full,
        Pdbonly
    }

    [PublicAPI]
    public class SimpleProjectProfile
    {
        public const string PlatformTargetProperty = "PlatformTarget";
        public const string DebugSymbolsProperty = "DebugSymbols";
        public const string DebugTypeProperty = "DebugType";
        public const string OptimizeProperty = "Optimize";
        public const string OutputPathProperty = "OutputPath";
        public const string DefineConstantsProperty = "DefineConstants";
        public const string ErrorReportProperty = "ErrorReport";
        public const string WarningLevelProperty = "WarningLevel";

        private readonly ProjectPropertyGroupElement _groupElement;
        private readonly Platform _platform;
        private readonly bool _isDebug;

        [NotNull]
        public string Name { get; private set; }

        [NotNull]
        public AlternateProject Project { get; private set; }

        public bool DebugSymbols
        {
            get
            {
                return
                    Convert.ToBoolean(GetPropertyValue(DebugSymbolsProperty,
                        DebugSymbolsDefaultValue));
            }
            set
            {
                _groupElement.SetProperty(DebugSymbolsProperty, value.ToString());
            }
        }

        internal bool DebugSymbolsDefaultValue
        {
            get { return _isDebug; }
        }

        public DebugType DebugType
        {
            get
            {
                return
                    (DebugType)
                        Enum.Parse(typeof (DebugType),
                            GetPropertyValue(DebugTypeProperty, DebugTypeDefaultValue), true);
            }
            set
            {
                _groupElement.SetProperty(DebugTypeProperty, value.ToString());
            }
        }

        internal DebugType DebugTypeDefaultValue
        {
            get { return _isDebug ? DebugType.Full : DebugType.Pdbonly; }
        }

        public Platform Platform
        {
            get { return _platform; }
        }

        public bool Optimize
        {
            get
            {
                return bool.Parse(GetPropertyValue(OptimizeProperty, OptimizeDefaultValue));
            }
            set
            {
                _groupElement.SetProperty(OptimizeProperty, value.ToString());
            }
        }

        internal bool OptimizeDefaultValue
        {
            get { return !_isDebug; }
        }

        [NotNull]
        public string OutputPath
        {
            get
            {
                return GetPropertyValue(OutputPathProperty, OutputPathDefaultValue);
            }
            set { _groupElement.SetProperty(OutputPathProperty, value); }
        }

        [NotNull]
        internal string OutputPathDefaultValue
        {
            get { return _isDebug ? @"bin\Debug\" : @"bin\Release\"; }
        }

        [NotNull]
        public string DefineConstants
        {
            get
            {
                return GetPropertyValue(DefineConstantsProperty, DefineConstantsDefaultValue);
            }
            set
            {
                _groupElement.AddProperty(DefineConstantsProperty, value);
            }
        }

        [NotNull]
        internal string DefineConstantsDefaultValue
        {
            get { return _isDebug ? "DEBUG;TRACE;" : "TRACE"; }
        }

        public ErrorReport ErrorReport
        {
            get
            {
                return
                    (ErrorReport)
                        Enum.Parse(typeof (ErrorReport), GetPropertyValue(ErrorReportProperty, ErrorReportDefaultValue), true);
            }
            set
            {
                _groupElement.SetProperty(ErrorReportProperty, value.ToString().ToLowerInvariant());
            }
        }

        public ErrorReport ErrorReportDefaultValue
        {
            get
            {
                return ErrorReport.Prompt;
            }
        }

        public int WarningLevel
        {
            get { return int.Parse(GetPropertyValue(WarningLevelProperty, WarningLevelDefaultValue)); }
            set { _groupElement.SetProperty(WarningLevelProperty, value.ToString(CultureInfo.InvariantCulture)); }
        }

        public int WarningLevelDefaultValue { get { return 4; } }

        internal SimpleProjectProfile([NotNull] AlternateProject project, [NotNull] string name,
            [NotNull] ProjectPropertyGroupElement groupElement)
        {
            _groupElement = groupElement;
            Project = project;
            Name = name;

            string[] platform = name.Split(new[] {'|'}, 2, StringSplitOptions.RemoveEmptyEntries);
            _isDebug = platform[0].ToLowerInvariant() == "debug";
            _platform = (Platform)Enum.Parse(typeof(Platform), platform[1], true);

            groupElement.SetProperty(PlatformTargetProperty, platform[1]);
        }

        [NotNull]
        private string GetPropertyValue([NotNull] string name, [NotNull] object defaultValue)
        {
            var prop = _groupElement.Properties.FirstOrDefault(inprop => inprop.Name == name);
            if (prop != null) return prop.Value;

            string realvalue = defaultValue.ToString();

            _groupElement.SetProperty(name, realvalue);
            return realvalue;
        }
    }
}
