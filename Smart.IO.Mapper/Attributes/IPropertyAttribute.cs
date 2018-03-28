namespace Smart.IO.Mapper.Attributes
{
    public interface IPropertyAttribute
    {
        int Offset { get; }

        string[] Profiles { get; }

        // TODO Length?

        // TODO Interface? IsConstraint, CreateFactory
    }
}
