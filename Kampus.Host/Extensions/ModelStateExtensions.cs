using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Kampus.Host.Extensions
{
    public static class ModelStateExtensions
    {
        public static bool IsValidField(this ModelStateDictionary modelState, string key)
        {
            return modelState.GetFieldValidationState(key) != ModelValidationState.Invalid;
        }
    }
}
