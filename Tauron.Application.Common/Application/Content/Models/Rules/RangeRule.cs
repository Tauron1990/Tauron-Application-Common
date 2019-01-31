using System;
using System.ComponentModel;
using System.Globalization;
using JetBrains.Annotations;

namespace Tauron.Application.Models.Rules
{
    [PublicAPI]
    public class RangeRule : ModelRule
    {
        public object Minimum { get; private set;}
        
        public object Maximum { get; private set; }
        
        public Type OperandType { get; }

        private Func<object, object> Conversion
        {
            get;
            set;
        }

        public RangeRule(string minimum, string maximum)
            : this(minimum, maximum, typeof(string))
        { }

        public RangeRule(double minimum, double maximum)
            : this(minimum, maximum, typeof(double))
        { }

        public RangeRule(int minimum, int maximum)
            : this(minimum, maximum, typeof(int))
        { }

        public RangeRule(object minimum, object maximum, Type operandType)
            : base(nameof(RangeRule))
        {
            Minimum = minimum;
            Maximum = maximum;
            OperandType = operandType;
        }

        private void Initialize(IComparable minimum, IComparable maximum, Func<object, object> conversion)
        {
            if (minimum.CompareTo(maximum) > 0)
                throw new InvalidOperationException(string.Format(RuleMessages.RangeMinGreaterThanMax, maximum, minimum));

            Minimum = minimum;
            Maximum = maximum;
            Conversion = conversion;
        }

        public override ValidatorResult IsValidValue(object value, ValidatorContext context)
        {
            try
            {
                SetupConversion();
                switch (value) {
                    case null:
                        return CreateResult();
                    case string text when string.IsNullOrEmpty(text):
                        return CreateResult();
                }

                var obj = Conversion(value);
                
                var comparable = (IComparable)Minimum;
                var comparable2 = (IComparable)Maximum;
                if (comparable.CompareTo(obj) <= 0)
                    return comparable2.CompareTo(obj) >= 0 ? CreateResult() : CreateResult(RuleMessages.RangeValidationError, context.DisplayName, Minimum, Maximum);
                return CreateResult(RuleMessages.RangeValidationError, context.DisplayName, Minimum, Maximum);
            }
            catch (Exception e)
            {
                return CreateResult(e.Message);
            }
        }

        private void SetupConversion()
        {
            if (this.Conversion != null)
                return;

            var minimum = Minimum;
            var maximum = Maximum;
            if (minimum == null || maximum == null)
                throw new InvalidOperationException(RuleMessages.RangeMustSetMinAndMax);

            var type2 = minimum.GetType();
            if (type2 == typeof(int))
            {
                Initialize((int)minimum, (int)maximum, v => Convert.ToInt32(v, CultureInfo.InvariantCulture));
                return;
            }

            if (type2 == typeof(double))
            {
                Initialize((double)minimum, (double)maximum, v => Convert.ToDouble(v, CultureInfo.InvariantCulture));
                return;
            }

            Type type = OperandType;
            if (type == null)
                throw new InvalidOperationException(RuleMessages.RangeMustSetOperandType);

            Type typeFromHandle = typeof(IComparable);
            if (!typeFromHandle.IsAssignableFrom(type))
                throw new InvalidOperationException(string.Format(RuleMessages.RangeArbitraryTypeNotIComparable, type.FullName, typeFromHandle.FullName));

            TypeConverter converter = TypeDescriptor.GetConverter(type);
            IComparable minimum2 = (IComparable)converter.ConvertFromString((string)minimum);
            IComparable maximum2 = (IComparable)converter.ConvertFromString((string)maximum);

            object Conversion(object value)
            {
                if (value == null || !(value.GetType() == type))
                    return converter.ConvertFrom(value);

                return value;
            }

            Initialize(minimum2, maximum2, Conversion);
        }
    }
}