using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Ioc
{
    public interface IContextHolder
    {
        ObjectContext Context { get; set; }
    }
}