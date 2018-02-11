namespace Tauron.Application.Common.Updater.Provider
{
    public sealed class ReleaseFile
    {
        public ReleaseFile(string name, string location)
        {
            Name = name;
            Location = location;
        }

        public string Location { get; }

        public string Name { get; }
    }
}