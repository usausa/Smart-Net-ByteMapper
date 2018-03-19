namespace Smart.IO.Mapper.Factories
{
    using Smart.IO.Mapper.Mappers;

    public sealed class BoolMapperFactory : IMemberMapperFactory
    {
        // TODO nullable

        public IMemberMapper Create(IMapperCreateContext context)
        {
            // TODO context

            return new BoolMapper(
                0,
                null,
                null,
                0x00,
                0x00);
        }
    }
}
