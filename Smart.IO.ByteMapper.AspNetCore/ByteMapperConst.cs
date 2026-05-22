namespace Smart.IO.ByteMapper.AspNetCore;

internal static class ByteMapperConst
{
    // Key used to store the active ByteMapper profile Type in HttpContext.Items.
    // Set by ByteMapperProfileAttribute.
    internal const string ProfileKey = "__ByteMapperProfile";
}
