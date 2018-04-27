namespace Smart.AspNetCore.Filters
{
    using System;

    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ByteMapperProfileAttribute : ResultFilterAttribute
    {
        private readonly string profile;

        public ByteMapperProfileAttribute(string profile)
        {
            this.profile = profile;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Items[Consts.ProfileKey] = profile;
        }
    }
}
