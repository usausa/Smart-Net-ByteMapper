﻿namespace Smart.IO.Mapper.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class MapAttribute : Attribute
    {
        public int Size { get; }

        public byte[] Delimiter { get; set; }

        public MapAttribute(int size)
        {
            Size = size;
        }
    }
}
