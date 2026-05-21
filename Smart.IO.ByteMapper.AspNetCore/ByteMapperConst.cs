namespace Smart.IO.ByteMapper.AspNetCore;

internal static class ByteMapperConst
{
    /// <summary>
    /// Key used to store the active ByteMapper profile name in
    /// <see cref="Microsoft.AspNetCore.Http.HttpContext.Items"/>.
    /// Set by <see cref="Filters.ByteMapperProfileAttribute"/>.
    /// </summary>
    internal const string ProfileKey = "__ByteMapperProfile";
}
