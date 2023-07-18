using FluentValidation.Results;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.Extensions
{
    public static class ValidationResultExtensions
    {
        public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState)
        {
            foreach (ValidationFailure error in result.Errors)
            {
                modelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
        }
    }
}