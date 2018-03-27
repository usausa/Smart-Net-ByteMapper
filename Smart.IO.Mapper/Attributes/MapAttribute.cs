﻿namespace Smart.IO.Mapper.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class MapAttribute : Attribute
    {
        public int Size { get; }

        public bool FillDefault { get; set; } = true;

        public MapAttribute(int size)
        {
            Size = size;
        }
    }
}
