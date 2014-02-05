using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Views.Core
{
    public static class NamingHelper
    {
        [NotNull]
        public static IEnumerable<string> CreatePossibilyNames([NotNull] string baseName)
        {
            Contract.Requires<ArgumentNullException>(baseName != null, "baseName");
            Contract.Ensures(Contract.Result<IEnumerable<string>>() != null);

            yield return baseName;
            yield return baseName + "View";
            yield return baseName + "ViewModel";

            if (!baseName.EndsWith("Model")) yield break;
            baseName = baseName.Remove(baseName.Length - 5);

            yield return baseName;
            yield return baseName + "View";
            yield return baseName + "ViewModel";
        }
    }
}