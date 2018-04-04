namespace Smart.IO.Mapper.Attributes
{
    using System;
    using Smart.ComponentModel;

    using Smart.IO.Mapper.Helpers;
    using Smart.IO.Mapper.Mappers;

    public interface ITypeMappingAttribute
    {
        int Offset { get; }

        int CalcSize(Type type);

        IMapper CreateMapper(IComponentContainer components, IMappingParameter parameters, Type type);
    }
}
