namespace Tauron.Application.Common.BaseLayer.BusinessLayer
{
    public class VoidReturn : Return
    {
        public override bool Error { get; } = false;
    }
}