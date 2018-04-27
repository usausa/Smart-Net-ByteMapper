namespace GenericFormatterTest
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Smart.IO.ByteMapper;

    public static class Program
    {
        public static void Main(string[] args)
        {
        }

        private static IInputReader CreateReader(Type type)
        {
            // TODO GeericInstanceを作るヘルパー、判定ルーチン
            return null;
        }
    }

    // MEMO
    // IEでは駄目、何回か処理される
    // 本物はasyncで実装か
    // TODO Outputの方も？

    public interface IInputReader
    {
        object Read(Stream stream, long? length);
    }

    public sealed class SingleInputReader<T> : IInputReader
    {
        private readonly ITypeMapper<T> mapper;

        private readonly Func<object> factory;

        public SingleInputReader(MapperFactory mapperFactory, Func<object> factory)
        {
            mapper = mapperFactory.Create<T>();
            this.factory = factory;
        }

        public object Read(Stream stream, long? length)
        {
            var buffer = new byte[mapper.Size];
            if (stream.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                return default;
            }

            var target = (T)factory();
            mapper.FromByte(buffer, 0, target);
            return target;
        }
    }

    public sealed class ArrayInputReader<T> : IInputReader
    {
        private readonly ITypeMapper<T> mapper;

        private readonly Func<object> factory;

        public ArrayInputReader(MapperFactory mapperFactory, Func<object> factory)
        {
            mapper = mapperFactory.Create<T>();
            this.factory = factory;
        }

        public object Read(Stream stream, long? length)
        {
            if (length.HasValue)
            {
                var array = new T[length.Value / mapper.Size];

                var index = 0;
                var buffer = new byte[mapper.Size];
                while (stream.Read(buffer, 0, buffer.Length) == buffer.Length)
                {
                    var target = (T)factory();
                    mapper.FromByte(buffer, 0, target);
                    array[index] = target;
                    index++;
                }

                return array;
            }
            else
            {
                var list = new List<T>();

                var buffer = new byte[mapper.Size];
                while (stream.Read(buffer, 0, buffer.Length) == buffer.Length)
                {
                    var target = (T)factory();
                    mapper.FromByte(buffer, 0, target);
                    list.Add(target);
                }

                return list.ToArray();
            }
        }
    }

    public sealed class ListInputReader<T> : IInputReader
    {
        private readonly ITypeMapper<T> mapper;

        private readonly Func<object> factory;

        public ListInputReader(MapperFactory mapperFactory, Func<object> factory)
        {
            mapper = mapperFactory.Create<T>();
            this.factory = factory;
        }

        public object Read(Stream stream, long? length)
        {
            var list = length.HasValue ? new List<T>((int)(length.Value / mapper.Size)) : new List<T>();

            var buffer = new byte[mapper.Size];
            while (stream.Read(buffer, 0, buffer.Length) == buffer.Length)
            {
                var target = (T)factory();
                mapper.FromByte(buffer, 0, target);
                list.Add(target);
            }

            return list;
        }
    }
}
