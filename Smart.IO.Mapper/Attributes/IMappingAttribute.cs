namespace Smart.IO.Mapper.Attributes
{
    public interface IMappingAttribute
    {
        string[] Profiles { get; }

        int Offset { get; }
    }
}
