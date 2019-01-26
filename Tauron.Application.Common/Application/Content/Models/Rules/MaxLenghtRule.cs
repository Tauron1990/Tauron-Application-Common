using System;
using System.Collections;

namespace Tauron.Application.Models.Rules
{
    public class MaxLenghtRule : ModelRule
    {
        public int Length
        {
            get;
            private set;
        }

        public MaxLenghtRule(int length)
            : base(nameof(MaxLenghtRule)) =>
            Length = length;

        public override ValidatorResult IsValidValue(object value, ValidatorContext context)
        {
            if (Length == 0 || Length < -1)
                return CreateResult(RuleMessages.InvalidMaxLength);
            if (value == null)
                return CreateResult();

            int num;

            switch (value)
            {
                case string text:
                    num = text.Length;
                    break;
                case Array array:
                    num = array.Length;
                    break;
                case IEnumerable enumerable:
                    num = Count(enumerable);
                    break;
                default:
                    num = 0;
                    break;
            }

            return -1 != Length && num <= Length ? CreateResult() : CreateResult(RuleMessages.InvalidMaxLength);
        }

        public static void DynamicUsing(object resource, Action action)
        {
            try
            {
                action();
            }
            finally
            {
                if (resource is IDisposable d)
                    d.Dispose();
            }
        }

        public static int Count(IEnumerable source)
        {
            if (source is ICollection col)
                return col.Count;

            int c = 0;
            var e = source.GetEnumerator();
            DynamicUsing(e, () =>
            {
                while (e.MoveNext())
                    c++;
            });

            return c;
        }
    }
}