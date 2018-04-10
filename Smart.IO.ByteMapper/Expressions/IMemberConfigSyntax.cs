namespace Smart.IO.ByteMapper.Expressions
{
    using System;

    public interface IMemberConfigSyntax : IMemberMapConfigSyntax
    {
        void Array(int length, Action<IMemberMapConfigSyntax> config);
    }
}
