using System;

namespace Tauron.Application.CQRS.Common
{
    public static class Guard
    {
        public static TType CheckNull<TType>(TType? toCkeck)
            where TType : class
        {
            if(toCkeck == null)
                throw new ArgumentNullException(nameof(toCkeck));

            return toCkeck;
        }
    }
}