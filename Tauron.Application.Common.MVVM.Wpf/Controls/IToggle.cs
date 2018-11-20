using System;

namespace Tauron.Application.Controls
{
    public interface IToggle
    {
        event Action<IToggle, bool> Switched;

        void SetHeader(object header);

        void Switch();
    }
}