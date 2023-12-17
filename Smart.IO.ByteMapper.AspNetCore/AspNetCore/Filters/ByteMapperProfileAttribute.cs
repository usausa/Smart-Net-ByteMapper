namespace Smart.AspNetCore.Filters;

using Microsoft.AspNetCore.Mvc.Filters;

#pragma warning disable CA1062
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ByteMapperProfileAttribute : Attribute, IAuthorizationFilter
{
    private readonly string profile;

    public ByteMapperProfileAttribute(string profile)
    {
        this.profile = profile;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        context.HttpContext.Items[Const.ProfileKey] = profile;
    }
}
