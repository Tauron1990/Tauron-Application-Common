namespace Tauron.Application.Common.BaseLayer.BusinessLayer
{
    public class ObjectReturn : Return
    {
        public override bool Error { get; } = false;

        public object Result { get; }

        public ObjectReturn(object result) => Result = result;
    }
}