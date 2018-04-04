namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Helpers;
    using Smart.IO.Mapper.Mappers;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public abstract class AbstractMapTypeAttribute : Attribute, IMapTypeAttribute
    {
        public int Offset { get; }

        protected AbstractMapTypeAttribute(int offset)
        {
            Offset = offset;
        }

        public abstract int CalcSize(Type type);

        public abstract IMapper CreateMapper(IComponentContainer components, IMappingParameter parameters, Type type);
    }
}
