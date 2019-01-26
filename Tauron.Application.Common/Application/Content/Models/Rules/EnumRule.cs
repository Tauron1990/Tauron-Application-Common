using System;
using System.Globalization;

namespace Tauron.Application.Models.Rules
{
    public class EnumRule : ModelRule
    {
        public Type EnumType { get; }

        public string ResourceName { get; set; }

        public EnumRule(Type enumType)
            : base(nameof(EnumRule)) =>
            EnumType = enumType;


        public override ValidatorResult IsValidValue(object value, ValidatorContext context)
        {
            if (EnumType == null)
                return CreateResult(RuleMessages.EnumDataTypeCannotBeNull);
            if (!EnumType.IsEnum)
                return CreateResult(RuleMessages.EnumDataTypeNeedsToBeAnEnum);
            if (value == null)
                return CreateResult();

            var text = value as string;

            if (text != null && string.IsNullOrEmpty(text))
                return CreateResult();

            var type = value.GetType();
            if (type.IsEnum && EnumType != type)
                return CreateResult(FindResource(ResourceName));
            if (!type.IsValueType && type != typeof(string))
                return CreateResult(FindResource(ResourceName));
            if (type == typeof(bool) || type == typeof(float) || type == typeof(double) || type == typeof(decimal) || type == typeof(char))
                return CreateResult(FindResource(ResourceName));

            object obj;
            if (type.IsEnum)
                obj = value;
            else
            {
                try
                {
                    obj = ((text == null) ? Enum.ToObject(EnumType, value) : Enum.Parse(EnumType, text, ignoreCase: false));
                }
                catch (ArgumentException)
                {
                    return CreateResult(FindResource(ResourceName));
                }
            }

            if (!IsEnumTypeInFlagsMode(EnumType)) return Enum.IsDefined(EnumType, obj) ? CreateResult() : CreateResult(FindResource(ResourceName));

            string underlyingTypeValueString = GetUnderlyingTypeValueString(EnumType, obj);
            string value2 = obj.ToString();
            return !underlyingTypeValueString.Equals(value2) ? CreateResult() : CreateResult(FindResource(ResourceName));
        }

        private static bool IsEnumTypeInFlagsMode(Type enumType) 
            => enumType.GetCustomAttributes(typeof(FlagsAttribute), inherit: false).Length != 0;

        private static string GetUnderlyingTypeValueString(Type enumType, object enumValue) 
            => Convert.ChangeType(enumValue, Enum.GetUnderlyingType(enumType), CultureInfo.InvariantCulture).ToString();
    }
}