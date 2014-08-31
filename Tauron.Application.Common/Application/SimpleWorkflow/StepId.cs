using Tauron.JetBrains.Annotations;

namespace Tauron.Application.SimpleWorkflow
{
    public struct StepId
    {
        public static readonly StepId Null = new StepId();

        public static readonly StepId Invalid = new StepId("Invalid");
        public static readonly StepId None = new StepId("None");
        public static readonly StepId Finish = new StepId("Finish");
        public static readonly StepId Loop = new StepId("Loop");
        public static readonly StepId LoopEnd = new StepId("LoopEnd");

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public StepId([NotNull] string name) : this()
        {
            Name = name;
        }

        [NotNull]
        public string Name { get; private set; }

        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(obj, null)) return false;
            if (!(obj is StepId)) return false;

            return ((StepId) obj).Name == Name;
        }

        public override string ToString()
        {
            return Name ?? string.Empty;
        }
    }
}