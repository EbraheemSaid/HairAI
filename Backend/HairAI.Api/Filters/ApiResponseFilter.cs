using HairAI.Api.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HairAI.Api.Filters;

public class ApiResponseFilter : ActionFilterAttribute
{
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is ObjectResult objectResult)
        {
            // If the result is already an ApiResponse, don't wrap it
            if (objectResult.Value?.GetType().IsGenericType == true && 
                objectResult.Value.GetType().GetGenericTypeDefinition() == typeof(ApiResponse<>))
            {
                return;
            }

            // Wrap the result in an ApiResponse
            var apiResponse = typeof(ApiResponse<>)
                .MakeGenericType(objectResult.Value?.GetType() ?? typeof(object))
                .GetMethod("Ok")
                ?.Invoke(null, new object[] { objectResult.Value ?? new object(), "Success" });

            context.Result = new ObjectResult(apiResponse)
            {
                StatusCode = objectResult.StatusCode
            };
        }

        base.OnActionExecuted(context);
    }
}