using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ServiceManager.Core
{
    public static class Extensions
    {
        public static bool TryGetTypedValue<TType>(this Dictionary<string, object> dic, string key, [MaybeNullWhen(false)] out TType value)
        {
            if (dic.TryGetValue(key, out var rawContent) && rawContent is TType typedContent)
            {
                value = typedContent;
                return true;
            }

            value = default!;
            return false;
        }
    }
}