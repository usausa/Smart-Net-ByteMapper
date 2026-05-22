namespace Smart.IO.ByteMapper.AspNetCore.Filters;

using System;

using Microsoft.AspNetCore.Mvc.Filters;

// Sets the ByteMapper profile for the current request by writing the profile Type to HttpContext.Items.
// Can be applied to controllers or individual actions.
#pragma warning disable CA1813
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ByteMapperProfileAttribute : Attribute, IResourceFilter
{
    public Type ProfileType { get; }

    public ByteMapperProfileAttribute(Type profileType)
    {
        ArgumentNullException.ThrowIfNull(profileType);
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

// Generic form of ByteMapperProfileAttribute (C# 11+ generic attribute).
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ByteMapperProfileAttribute<TProfile> : ByteMapperProfileAttribute
    where TProfile : class
{
    public ByteMapperProfileAttribute()
        : base(typeof(TProfile))
    {
    }
}
