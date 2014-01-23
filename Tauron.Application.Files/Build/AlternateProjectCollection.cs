using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Build.Evaluation;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Build
{
    [PublicAPI]
    public class AlternateProjectCollection
    {
        private List<AlternateProject> _projects;
        
        [NotNull]
        public ProjectCollection ProjectCollection { get; private set; }

        [NotNull]
        public IEnumerable<AlternateProject> Projects { get
        {
            return new ReadOnlyCollection<AlternateProject>(_projects);
        } }

        public AlternateProjectCollection() 
            : this(ProjectCollection.GlobalProjectCollection)
        {
        }

        public AlternateProjectCollection([NotNull] ProjectCollection collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");

            ProjectCollection = collection;

            _projects = new List<AlternateProject>();

            foreach (var loadedProject in collection.LoadedProjects)
            {
                _projects.Add(new AlternateProject(loadedProject, this));
            }
        }

        internal void Add([NotNull] AlternateProject project)
        {
            _projects.Add(project);
        }
    }
}