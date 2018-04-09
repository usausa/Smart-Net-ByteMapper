namespace Smart.IO.Mapper.Builders
{
    using System;

    using Smart.IO.Mapper.Mappers;

    public class FillerTypeMapperBuilder : ITypeMapperBuilder
    {
        public int Offset { get; set; }

        public int Length { get; set; }

        public byte? Filler { get; set; }

        public int CalcSize(Type type)
        {
            return Length;
        }

        public IMapper CreateMapper(IBuilderContext context, Type type)
        {
            return new FillMapper(
                Offset,
                Length,
                Filler ?? context.GetParameter<byte>(Parameter.Filler));
        }
    }
}
