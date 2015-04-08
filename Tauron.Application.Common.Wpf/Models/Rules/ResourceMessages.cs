using System.Resources;

namespace Tauron.Application.Models.Rules
{
    internal static class ResourceMessages
    {
        private static ResourceManager _resourceManager;

        public static ResourceManager ResourceManager
        {
            get
            {
                return _resourceManager ??
                       (_resourceManager =
                           new ResourceManager(
                               "System.ComponentModel.DataAnnotations.Resources.DataAnnotationsResources",
                               typeof (System.ComponentModel.DataAnnotations.Validator).Assembly));
            }
        }
        internal static string RequireRuleError
        {
            get
            {
// ReSharper disable once ResourceItemNotResolved
                return ResourceManager.GetString("RequiredAttribute_ValidationError");
            }
        }
    }
}