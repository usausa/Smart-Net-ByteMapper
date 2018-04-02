namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.IO.Mapper.Mappings;

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

        public override IMapping CreateMapping(IMappingCreateContext context, Type type)
        {
            return new ConstMapping(Offset, content);
        }
    }
}
