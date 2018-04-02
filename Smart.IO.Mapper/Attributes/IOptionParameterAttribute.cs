namespace Smart.IO.Mapper.Attributes
{
    public interface IOptionParameterAttribute
    {
        string Key { get; }

        object Value { get; }
    }
}
