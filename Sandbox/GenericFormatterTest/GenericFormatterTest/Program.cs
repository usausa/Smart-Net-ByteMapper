﻿namespace GenericFormatterTest
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

        // TODO GeericInstanceを作るヘルパー、判定ルーチン
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
        private readonly ITypeMapper<T> typeMapper;

        private readonly Func<object> factory;

        public SingleInputReader(MapperFactory mapperFactory, Func<object> factory)
        {
            typeMapper = mapperFactory.Create<T>();
            this.factory = factory;
        }

        public object Read(Stream stream, long? length)
        {
            var buffer = new byte[typeMapper.Size];
            if (stream.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                return default;
            }

            var target = (T)factory();
            typeMapper.FromByte(buffer, 0, target);
            return target;
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

        public object Read(Stream stream, long? length)
        {
            if (length.HasValue)
            {
                var array = new T[length.Value / typeMapper.Size];

                var index = 0;
                var buffer = new byte[typeMapper.Size];
                while (stream.Read(buffer, 0, buffer.Length) == buffer.Length)
                {
                    var target = (T)factory();
                    typeMapper.FromByte(buffer, 0, target);
                    array[index] = target;
                    index++;
                }

                return array;
            }
            else
            {
                var list = new List<T>();

                var buffer = new byte[typeMapper.Size];
                while (stream.Read(buffer, 0, buffer.Length) == buffer.Length)
                {
                    var target = (T)factory();
                    typeMapper.FromByte(buffer, 0, target);
                    list.Add(target);
                }

                return list.ToArray();
            }
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

        public object Read(Stream stream, long? length)
        {
            var list = length.HasValue ? new List<T>((int)(length.Value / typeMapper.Size)) : new List<T>();

            var buffer = new byte[typeMapper.Size];
            while (stream.Read(buffer, 0, buffer.Length) == buffer.Length)
            {
                var target = (T)factory();
                typeMapper.FromByte(buffer, 0, target);
                list.Add(target);
            }

            return list;
        }
    }
}
