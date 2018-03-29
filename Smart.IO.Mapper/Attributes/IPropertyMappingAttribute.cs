namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.IO.Mapper.Converters;

    public interface IPropertyMappingAttribute
    {
        int Offset { get; }

        int CalcSize(Type type);

        IByteConverter CreateConverter(IMappingCreateContext context, Type type);
    }
}
