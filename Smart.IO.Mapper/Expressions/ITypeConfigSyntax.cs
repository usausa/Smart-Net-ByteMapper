namespace Smart.IO.Mapper.Expressions
{
    public interface ITypeConfigSyntax<T>
    {
        // TODO 汎用データ、いくつかはTypedで！、ここはExtensionで？

        ITypeConfigSyntax<T> UseFiller(bool value);

        ITypeConfigSyntax<T> UseDelimitter(bool value);

        ITypeConfigSyntax<T> UseDelimitter(params byte[] delimitter);

        ITypeConfigSyntax<T> AddMapper(ITypeMapFactory factory);

        ITypeConfigSyntax<T> AddMapper(int offset, ITypeMapFactory factory);

        // TODO ForMember
    }
}
