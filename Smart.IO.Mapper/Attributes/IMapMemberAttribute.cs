namespace Smart.IO.Mapper.Attributes
{
    using Smart.IO.Mapper.Builders;

    public interface IMapMemberAttribute
    {
        int Offset { get; }

        IMapConverterBuilder GetConverterBuilder();
    }
}
