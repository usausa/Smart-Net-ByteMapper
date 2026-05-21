namespace Smart.IO.ByteMapper.AspNetCore.Filters;

using System;

using Microsoft.AspNetCore.Mvc.Filters;

/// <summary>
/// Sets the ByteMapper profile for the current request by writing to
/// <see cref="Microsoft.AspNetCore.Http.HttpContext.Items"/>.
/// Can be applied to controllers or individual actions.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class ByteMapperProfileAttribute : Attribute, IResourceFilter
{
    public string Profile { get; }

    public ByteMapperProfileAttribute(string profile)
    {
        Profile = profile;
    }

    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        context.HttpContext.Items[ByteMapperConst.ProfileKey] = Profile;
    }

    public void OnResourceExecuted(ResourceExecutedContext context) { }
}
