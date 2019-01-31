using System.ComponentModel.DataAnnotations;
using System.Resources;

namespace Tauron.Application.Models.Rules
{
    internal static class RuleMessages
    {
        private static ResourceManager _resourceManager;

        private static ResourceManager ResourceManager => _resourceManager ??
                                                         (_resourceManager =
                                                             new ResourceManager(
                                                                 "System.ComponentModel.DataAnnotations.Resources.DataAnnotationsResources",
                                                                 typeof(Validator).Assembly));

        internal static string RequireRuleError => ResourceManager.GetString("RequiredAttribute_ValidationError");

        internal static string CompareMustMatch => ResourceManager.GetString("CompareAttribute_MustMatch");

        internal static string CompareUnknownProperty => ResourceManager.GetString("CompareAttribute_UnknownProperty");

        internal static string CreditCardInvalid => ResourceManager.GetString("CreditCardAttribute_Invalid");

        internal static string EmailAddressInvalid => ResourceManager.GetString("EmailAddressAttribute_Invalid");

        internal static string EnumDataTypeCannotBeNull => ResourceManager.GetString("EnumDataTypeAttribute_TypeCannotBeNull");

        internal static string EnumDataTypeNeedsToBeAnEnum => ResourceManager.GetString("EnumDataTypeAttribute_TypeNeedsToBeAnEnum");

        internal static string FileExtensionsInvalid => ResourceManager.GetString("FileExtensionsAttribute_Invalid");

        internal static string PhoneInvalid => ResourceManager.GetString("PhoneAttribute_Invalid");

        internal static string UrlInvalid => ResourceManager.GetString("UrlAttribute_Invalid");

        internal static string InvalidMaxLength => ResourceManager.GetString("MaxLengthAttribute_InvalidMaxLength");

        internal static string MaxLengthValidationError => ResourceManager.GetString("MaxLengthAttribute_ValidationError");

        internal static string InvalidMinLength => ResourceManager.GetString("MinLengthAttribute_InvalidMinLength");

        internal static string MinLengthValidationError => ResourceManager.GetString("MinLengthAttribute_ValidationError");

        internal static string RangeValidationError => ResourceManager.GetString("RangeAttribute_ValidationError");

        internal static string RangeMinGreaterThanMax => ResourceManager.GetString("RangeAttribute_MinGreaterThanMax");

        internal static string RangeMustSetMinAndMax => ResourceManager.GetString("RangeAttribute_Must_Set_Min_And_Max");

        internal static string RangeMustSetOperandType => ResourceManager.GetString("RangeAttribute_Must_Set_Operand_Type");

        internal static string RangeArbitraryTypeNotIComparable => ResourceManager.GetString("RangeAttribute_ArbitraryTypeNotIComparable");

        internal static string RegexValidationError => ResourceManager.GetString("RegexAttribute_ValidationError");

        internal static string RegularExpressionEmptyPattern => ResourceManager.GetString("RegularExpressionAttribute_Empty_Pattern");

        internal static string StringLengthValidationError => ResourceManager.GetString("StringLengthAttribute_ValidationError");

        internal static string StringLengthInvalidMaxLength => ResourceManager.GetString("StringLengthAttribute_InvalidMaxLength");

        internal static string StringLengthValidationErrorIncludingMinimum => ResourceManager.GetString("StringLengthAttribute_ValidationErrorIncludingMinimum");
    }
}