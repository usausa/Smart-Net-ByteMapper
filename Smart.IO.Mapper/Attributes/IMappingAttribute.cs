namespace Smart.IO.Mapper.Attributes
{
    using Smart.IO.Mapper.Mappings;

    public interface IMappingAttribute
    {
        string[] Profiles { get; }

        int Offset { get; }

        // TODO Length?

        IMappingFactory BuildFactory();
    }
}
