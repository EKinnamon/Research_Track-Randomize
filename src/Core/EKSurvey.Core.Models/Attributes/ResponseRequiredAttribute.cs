using System;
using System.ComponentModel.DataAnnotations;
using EKSurvey.Core.Models.Entities;
using EKSurvey.Core.Models.ViewModels.Test;

namespace EKSurvey.Core.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class ResponseRequiredAttribute : ValidationAttribute
    {
        private readonly string _pagePropertyName;

        public ResponseRequiredAttribute(string pagePropertyName)
        {
            _pagePropertyName = pagePropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var valueString = value as string;

            if (!(validationContext.ObjectInstance is ResponseViewModel responseViewModel))
                return null;

            if (responseViewModel.PageType.IsAssignableFrom(typeof(IQuestion)) && string.IsNullOrWhiteSpace(valueString))
            {
                return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
            }

            return null;
        }
    }
}