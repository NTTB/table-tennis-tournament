using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace T3.Web;

public class TypedGuidBinder<TOut> : IModelBinder where TOut : TypedGuid, new()
{
    static TypedGuidBinder()
    {
        System.Diagnostics.Debug.Assert(typeof(TOut).IsSealed);
    }

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        // Looks up the value of the argument by name and sets it in the model state
        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        if (Guid.TryParse(valueProviderResult.FirstValue, out var guid))
        {
            var typedGuid = new TOut { Value = guid };
            bindingContext.Model = typedGuid;
            bindingContext.Result = ModelBindingResult.Success(typedGuid);
        }
        else
        {
            bindingContext.Result = ModelBindingResult.Failed();
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid Guid");
        }

        return Task.CompletedTask;
    }
}