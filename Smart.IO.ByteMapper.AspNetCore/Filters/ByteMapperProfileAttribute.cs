namespace Smart.IO.ByteMapper.AspNetCore.Filters;

using System;

using Microsoft.AspNetCore.Mvc.Filters;

/// <summary>
/// Sets the ByteMapper profile for the current request by writing the profile
/// <see cref="Type"/> to <see cref="Microsoft.AspNetCore.Http.HttpContext.Items"/>.
/// Can be applied to controllers or individual actions.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
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

    public void OnResourceExecuted(ResourceExecutedContext context) { }
}

/// <summary>
/// Generic form of <see cref="ByteMapperProfileAttribute"/> (C# 11+ generic attribute).
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class ByteMapperProfileAttribute<TProfile> : ByteMapperProfileAttribute
    where TProfile : class
{
    public ByteMapperProfileAttribute() : base(typeof(TProfile)) { }
}
