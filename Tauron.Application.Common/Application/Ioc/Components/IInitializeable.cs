namespace Tauron.Application.Ioc.Components
{
    public interface IInitializeable
    {
        void Initialize(ComponentRegistry components);
    }
}