namespace Smart.IO.Mapper
{
    public interface ITypeMapper
    {
        // TODO サイズ、

        T FromByte<T>(byte[] buffer)
            where T : new();

        // TODO Factroy?
        // TODO Stream read ?
        // TODO Writer, bytes & stream ?
    }
}
