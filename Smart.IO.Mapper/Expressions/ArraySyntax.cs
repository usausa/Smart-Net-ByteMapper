namespace Smart.IO.Mapper.Expressions
{
    public interface IArraySyntax
    {
        IArraySyntax Count(int count);

        // TODO Property
    }

    public sealed class ArrayMapBuilder : IPropertyMapFactory, IArraySyntax, IPropertyMapSyntax
    {
        public IArraySyntax Count(int count)
        {
            // TODO
            return this;
        }
    }

    public static class ArrayMapExtensions
    {
        // TODO
    }
}
