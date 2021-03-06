﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron
{
    [PublicAPI]
    public static class CollectionExtensions
    {
        public static TValue AddIfNotExis<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dic,
            [NotNull] TKey key, [NotNull] Func<TValue> creator)
        {
            Argument.NotNull(dic, nameof(dic));
            Argument.NotNull(key, nameof(key));
            Argument.NotNull(creator, nameof(creator));
            TValue temp;

            if (dic.ContainsKey(key))
                temp = dic[key];
            else
            {
                temp = creator();
                dic[key] = temp;
            }

            return temp;
        }

        [CanBeNull]
        public static TValue TryGetAndCast<TValue>([NotNull] this IDictionary<string, object> dic, [NotNull] string key)
            where TValue : class
        {
            Argument.NotNull(dic, nameof(dic));
            Argument.NotNull(key, nameof(key));

            if (dic.TryGetValue(key, out var obj)) return obj as TValue;

            return null;
        }

        public static TValue TryGetOrDefault<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dic, TKey key) 
            => dic.TryGetValue(key, out var value) ? value : default;
    }
}