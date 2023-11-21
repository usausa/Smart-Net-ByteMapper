namespace Smart.AspNetCore.Filters;

using Microsoft.AspNetCore.Mvc.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ByteMapperProfileAttribute : Attribute, IAuthorizationFilter
{
    private readonly string profile;

    public ByteMapperProfileAttribute(string profile)
    {
        this.profile = profile;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Ignore")]
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        context.HttpContext.Items[Const.ProfileKey] = profile;
    }
}
