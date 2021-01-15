namespace Smart.IO.ByteMapper.Expressions
{
    using System;

    public interface IMemberConfigSyntax : IMemberMapConfigSyntax
    {
        IMapArraySyntax Array(int length, Action<IMemberMapConfigSyntax> config);
    }
}
