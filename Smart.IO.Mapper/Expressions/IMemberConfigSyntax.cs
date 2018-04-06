namespace Smart.IO.Mapper.Expressions
{
    using System;

    public interface IMemberConfigSyntax : IMemberMapConfigSyntax
    {
        IMemberMapConfigSyntax Array(int count);
    }
}
