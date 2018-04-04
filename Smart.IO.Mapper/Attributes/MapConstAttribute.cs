namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Helpers;
    using Smart.IO.Mapper.Mappers;

    public sealed class MapConstAttribute : AbstractTypeMappingAttribute
    {
        private readonly byte[] content;

        public MapConstAttribute(int offset, byte[] content)
            : base(offset)
        {
            this.content = content;
        }

        public override int CalcSize(Type type)
        {
            return content.Length;
        }

        public override IMapper CreateMapper(IComponentContainer components, IMappingParameter parameters, Type type)
        {
            return new ConstMapper(Offset, content);
        }
    }
}
