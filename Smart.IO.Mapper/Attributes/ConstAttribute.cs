namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.IO.Mapper.Mappings;

    public sealed class ConstAttribute : AbstractTypeMappingAttribute
    {
        public byte[] Content { get; set; }

        public ConstAttribute(int offset)
            : base(offset)
        {
        }

        public override int CalcSize(Type type)
        {
            return Content.Length;
        }

        public override IMapping CreateMapping(IMappingCreateContext context, Type type)
        {
            return new ConstMapping(Offset, Content);
        }
    }
}
