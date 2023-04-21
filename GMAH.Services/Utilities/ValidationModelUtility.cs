using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GMAH.Services.Utilities
{
    /// <summary>
    /// Read more: https://www.geekinsta.com/manually-validate-with-data-annotations/
    /// </summary>
    public class ValidationModelUtility
    {
        public static ValidationModelResult Validate(object value)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(value, null, null);

            Validator.TryValidateObject(value, context, results, true);

            if (results.Count != 0)
            {
                return new ValidationModelResult
                {
                    IsValidate = false,
                    ErrorMessage = results.First().ErrorMessage,
                };
            }

            return new ValidationModelResult
            {
                IsValidate = true,
            };
        }

        public class ValidationModelResult
        {
            public string ErrorMessage { get; set; }
            public bool IsValidate { get; set; }
        }
    }
}
