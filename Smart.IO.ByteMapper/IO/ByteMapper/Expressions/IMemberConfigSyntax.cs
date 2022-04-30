namespace Smart.IO.ByteMapper.Expressions;

public interface IMemberConfigSyntax : IMemberMapConfigSyntax
{
    IMapArraySyntax Array(int length, Action<IMemberMapConfigSyntax> config);
}
