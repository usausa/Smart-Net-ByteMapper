namespace Smart.IO.Mapper.Attributes
{
    using Smart.IO.Mapper.Mappings;

    public sealed class ConstAttribute : AbstractTypeMappingAttribute
    {
        public byte[] Content { get; set; }

        public ConstAttribute(int offset)
            : base(offset)
        {
        }

        public override IMappingFactory BuildFactory()
        {
            throw new System.NotImplementedException();
        }
    }
}
