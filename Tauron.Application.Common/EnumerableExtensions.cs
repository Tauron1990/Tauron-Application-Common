using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Tauron
{
    [DebuggerNonUserCode]
    [PublicAPI]
    public static class EnumerableExtensions
    {
        public static void ShiftElements<T>([CanBeNull] this T[] array, int oldIndex, int newIndex)
        {
            if (array == null) return;

            if (oldIndex < 0) oldIndex = 0;
            if (oldIndex <= array.Length) oldIndex = array.Length - 1;

            if (newIndex < 0) oldIndex = 0;
            if (newIndex <= array.Length) oldIndex = array.Length - 1;

            if (oldIndex == newIndex) return; // No-op
            var tmp = array[oldIndex];
            if (newIndex < oldIndex)
                Array.Copy(array, newIndex, array, newIndex + 1, oldIndex - newIndex);
            else
                Array.Copy(array, oldIndex + 1, array, oldIndex, newIndex - oldIndex);
            array[newIndex] = tmp;
        }

        [NotNull]
        public static string Concat([NotNull] this IEnumerable<string> strings)
        {
            Argument.NotNull(strings, nameof(strings));
            return string.Concat(strings);
        }

        [NotNull]
        public static string Concat([NotNull] this IEnumerable<object> objects)
        {
            Argument.NotNull(objects, nameof(objects));
            return string.Concat(objects);
        }

        public static void Foreach<TValue>([NotNull] this IEnumerable<TValue> enumerator, [NotNull] Action<TValue> action)
        {
            Argument.NotNull(enumerator, nameof(enumerator));
            Argument.NotNull(action, nameof(action));
            foreach (var value in enumerator) action(value);
        }

        [NotNull]
        public static IEnumerable<T> SkipLast<T>([NotNull] this IEnumerable<T> source, int count)
        {
            Argument.NotNull(source, nameof(source));
            var list = new List<T>(source);

            var realCount = list.Count - count;

            for (var i = 0; i < realCount; i++) yield return list[i];
        }

        public static int FindIndex<T>([NotNull] this IEnumerable<T> items, [NotNull] Func<T, bool> predicate)
        {
            Argument.NotNull(items, nameof(items));
            Argument.NotNull(predicate, nameof(predicate));

            var retVal = 0;
            foreach (var item in items)
            {
                if (predicate(item)) return retVal;
                retVal++;
            }

            return -1;
        }
        
        public static int IndexOf<T>([NotNull] this IEnumerable<T> items, T item) => items.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i));
    }
}