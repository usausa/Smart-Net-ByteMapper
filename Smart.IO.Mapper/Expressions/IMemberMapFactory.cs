namespace Smart.IO.Mapper.Expressions
{
    using System;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Converters;
    using Smart.IO.Mapper.Helpers;

    public interface IMemberMapFactory
    {
        int CalcSize(Type type);

        IByteConverter CreateConverter(IComponentContainer components, IMappingParameter parameters, Type type);
    }
}
