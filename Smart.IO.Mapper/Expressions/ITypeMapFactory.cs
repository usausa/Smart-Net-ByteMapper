namespace Smart.IO.Mapper.Expressions
{
    using System;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Helpers;
    using Smart.IO.Mapper.Mappers;

    public interface ITypeMapFactory
    {
        int CalcSize(Type type);

        IMapper CreateMapper(IComponentContainer components, IMappingParameter parameters, Type type);
    }
}
