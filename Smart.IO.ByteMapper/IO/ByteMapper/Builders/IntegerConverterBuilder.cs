﻿namespace Smart.IO.ByteMapper.Builders
{
    using System;

    using Smart.IO.ByteMapper.Converters;

    public sealed class IntegerConverterBuilder : AbstractMapConverterBuilder<IntegerConverterBuilder>
    {
        public int Length { get; set; }

        public Padding? Padding { get; set; }

        public bool? ZeroFill { get; set; }

        public byte? Filler { get; set; }

        static IntegerConverterBuilder()
        {
            AddEntry(typeof(int), (b, t) => b.Length, (b, t, c) => b.CreateInt32Converter(t, c));
            AddEntry(typeof(int?), (b, t) => b.Length, (b, t, c) => b.CreateInt32Converter(t, c));
            AddEntry(typeof(long), (b, t) => b.Length, (b, t, c) => b.CreateInt64Converter(t, c));
            AddEntry(typeof(long?), (b, t) => b.Length, (b, t, c) => b.CreateInt64Converter(t, c));
            AddEntry(typeof(short), (b, t) => b.Length, (b, t, c) => b.CreateInt16Converter(t, c));
            AddEntry(typeof(short?), (b, t) => b.Length, (b, t, c) => b.CreateInt16Converter(t, c));
        }

        private IMapConverter CreateInt32Converter(Type type, IBuilderContext context)
        {
            return new Int32Converter(
                Length,
                Padding ?? context.GetParameter<Padding>(Parameter.NumberPadding),
                ZeroFill ?? context.GetParameter<bool>(Parameter.ZeroFill),
                Filler ?? context.GetParameter<byte>(Parameter.NumberFiller),
                type);
        }

        private IMapConverter CreateInt64Converter(Type type, IBuilderContext context)
        {
            return new Int64Converter(
                Length,
                Padding ?? context.GetParameter<Padding>(Parameter.NumberPadding),
                ZeroFill ?? context.GetParameter<bool>(Parameter.ZeroFill),
                Filler ?? context.GetParameter<byte>(Parameter.NumberFiller),
                type);
        }

        private IMapConverter CreateInt16Converter(Type type, IBuilderContext context)
        {
            return new Int16Converter(
                Length,
                Padding ?? context.GetParameter<Padding>(Parameter.NumberPadding),
                ZeroFill ?? context.GetParameter<bool>(Parameter.ZeroFill),
                Filler ?? context.GetParameter<byte>(Parameter.NumberFiller),
                type);
        }
    }
}
