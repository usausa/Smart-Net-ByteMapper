namespace Smart.IO.Mapper.Attributes
{
    using System;
    using Smart.ComponentModel;

    using Smart.IO.Mapper.Converters;
    using Smart.IO.Mapper.Helpers;

    public interface IMapMemberAttribute
    {
        int Offset { get; }

        int CalcSize(Type type);

        IMapConverter CreateConverter(IComponentContainer components, IMappingParameter parameters, Type type);
    }
}
