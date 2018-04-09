namespace Smart.IO.Mapper.Expressions
{
    using System;

    public interface IMemberConfigSyntax : IMemberMapConfigSyntax
    {
        void Array(int length, Action<IMemberMapConfigSyntax> config);
    }
}
