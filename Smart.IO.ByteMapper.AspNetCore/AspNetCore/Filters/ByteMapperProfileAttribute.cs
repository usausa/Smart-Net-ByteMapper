namespace Smart.AspNetCore.Filters
{
    using System;

    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ByteMapperProfileAttribute : Attribute, IAuthorizationFilter
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
}
