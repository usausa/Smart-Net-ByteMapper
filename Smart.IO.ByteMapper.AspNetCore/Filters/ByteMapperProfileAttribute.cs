namespace Smart.IO.ByteMapper.AspNetCore.Filters;

using System;

using Microsoft.AspNetCore.Mvc.Filters;

#pragma warning disable CA1813
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ByteMapperProfileAttribute : Attribute, IResourceFilter
{
    public Type ProfileType { get; }

    public ByteMapperProfileAttribute(Type profileType)
    {
        ProfileType = profileType;
    }

    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        context.HttpContext.Items[ByteMapperConst.ProfileKey] = ProfileType;
    }

    public void OnResourceExecuted(ResourceExecutedContext context)
    {
    }
}
#pragma warning restore CA1813

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ByteMapperProfileAttribute<TProfile> : ByteMapperProfileAttribute
    where TProfile : class
{
    public ByteMapperProfileAttribute()
        : base(typeof(TProfile))
    {
    }
}
