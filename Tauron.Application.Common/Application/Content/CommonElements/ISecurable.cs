using System.Security.Principal;

namespace Tauron.Application
{
    public interface ISecurable
    {
        bool IsUserInRole(IIdentity identity, string roles);
    }
}