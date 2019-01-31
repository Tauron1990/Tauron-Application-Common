using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace Tauron.Application.Models.Rules
{
    [PublicAPI]
    public class FileExtensionsRule : ModelRule
    {
        public FileExtensionsRule()
            : base(nameof(FileExtensionsRule))
        {
            
        }

        private string _extensions;
        
        public string Extensions
        {
            get => !string.IsNullOrWhiteSpace(_extensions) ? _extensions : "png,jpg,jpeg,gif";
            set => _extensions = value;
        }

        private string ExtensionsFormatted => ExtensionsParsed.Aggregate((left, right) => left + ", " + right);

        private string ExtensionsNormalized => Extensions.Replace(" ", "").Replace(".", "").ToLowerInvariant();

        private IEnumerable<string> ExtensionsParsed => from e in ExtensionsNormalized.Split(',')
                                                        select "." + e;
        
        public override ValidatorResult IsValidValue(object value, ValidatorContext context)
        {
            switch (value)
            {
                case null:
                case string text when ValidateExtension(text):
                    return CreateResult();
                default:
                    return CreateResult(RuleMessages.FileExtensionsInvalid, value, ExtensionsFormatted);
            }
        }

        private bool ValidateExtension(string fileName)
        {
            try
            {
                return ExtensionsParsed.Contains(Path.GetExtension(fileName)?.ToLowerInvariant());
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}