namespace GenericFormatterTest
{
    using System;
    using System.IO;

    using Smart.IO.ByteMapper;

    public static class Program
    {
        public static void Main(string[] args)
        {
        }
    }

    // MEMO
    // IEでは駄目、何回か処理される

    // TODO 本物はasyncで実装か

    // TODO GeericInstanceを作るヘルパー

    public interface IInputReader
    {
        bool Read(Stream stream, long? length, out object value);
    }

    public sealed class SingleInputReader<T> : IInputReader
    {
        private readonly ITypeMapper<T> typeMapper;

        private readonly Func<object> factory;

        public SingleInputReader(MapperFactory mapperFactory, Func<object> factory)
        {
            typeMapper = mapperFactory.Create<T>();
            this.factory = factory;
        }

        public bool Read(Stream stream, long? length, out object value)
        {
            // TODO 失敗の条件？
            throw new NotImplementedException();
        }
    }

    public sealed class ArrayInputReader<T> : IInputReader
    {
        private readonly ITypeMapper<T> typeMapper;

        private readonly Func<object> factory;

        public ArrayInputReader(MapperFactory mapperFactory, Func<object> factory)
        {
            typeMapper = mapperFactory.Create<T>();
            this.factory = factory;
        }

        public bool Read(Stream stream, long? length, out object value)
        {
            // TODO contentLength
            throw new NotImplementedException();
        }
    }

    public sealed class ListInputReader<T> : IInputReader
    {
        private readonly ITypeMapper<T> typeMapper;

        private readonly Func<object> factory;

        public ListInputReader(MapperFactory mapperFactory, Func<object> factory)
        {
            typeMapper = mapperFactory.Create<T>();
            this.factory = factory;
        }

        public bool Read(Stream stream, long? length, out object value)
        {
            throw new NotImplementedException();
        }
    }
}
