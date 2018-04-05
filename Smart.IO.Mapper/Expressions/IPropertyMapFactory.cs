namespace Smart.IO.Mapper.Expressions
{
    using System;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Converters;
    using Smart.IO.Mapper.Helpers;

    public interface IPropertyMapFactory
    {
        int CalcSize(Type type);

        IByteConverter CreateConverter(IComponentContainer components, IMappingParameter parameters, Type type);
    }
}
