using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tauron.Application.Models
{
    public class DisplayNameHelper
    {
        private class CacheKey : IEquatable<CacheKey>
        {
            public string TypeName { get; }

            public string PropertyName { get; }

            public CacheKey(string typeName, string propertyName)
            {
                TypeName = typeName;
                PropertyName = propertyName;
            }

            public bool Equals(CacheKey other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return string.Equals(TypeName, other.TypeName) && string.Equals(PropertyName, other.PropertyName);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == GetType() && Equals((CacheKey) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((TypeName != null ? TypeName.GetHashCode() : 0) * 397) ^ (PropertyName != null ? PropertyName.GetHashCode() : 0);
                }
            }

            public static bool operator ==(CacheKey left, CacheKey right) => Equals(left, right);

            public static bool operator !=(CacheKey left, CacheKey right) => !Equals(left, right);
        }

        private static readonly Dictionary<CacheKey, string> Cache = new Dictionary<CacheKey, string>();

        public static string GetDisplayName(string type, string name, Func<PropertyInfo> getter)
        {
            var key = new CacheKey(type, name);
            if (Cache.TryGetValue(key, out var text)) return text;

            var info = getter();
            if (info == null)
            {
                Cache[key] = null;
                return null;
            }

            var resourceName =info.GetAllCustomAttributes<DisplayNameAttribate>().FirstOrDefault()?.PropertyResourceName;
            var resourceResult = ResourceManagerProvider.FindResource(resourceName ?? string.Empty, null);

            Cache[key] = resourceResult;

            return resourceResult;
        }
    }
}