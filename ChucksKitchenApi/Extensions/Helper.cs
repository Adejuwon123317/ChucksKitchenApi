using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ChucksKitchenApi.Extensions
{
    public static class Extensions
    {
        public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelstate)
        {
            foreach (var error in result.Errors)
            {
                modelstate.AddModelError(error.PropertyName, error.ErrorMessage);
            }
        }
    }
}
