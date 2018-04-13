namespace Smart.IO.ByteMapper.Builders
{
    using System;
    using System.Collections.Generic;

    using Smart.IO.ByteMapper.Converters;

    public abstract class AbstractMapConverterBuilder<TBuilder> : IMapConverterBuilder
        where TBuilder : AbstractMapConverterBuilder<TBuilder>
    {
        private static readonly Dictionary<Type, Entry> Entries = new Dictionary<Type, Entry>();

        protected static void AddEntry(Type type, Func<TBuilder, Type, int> calcSize, Func<TBuilder, Type, IBuilderContext, IMapConverter> factory)
        {
            Entries.Add(type, new Entry(calcSize, factory));
        }

        protected static void AddEntry(Type type, int size, Func<TBuilder, Type, IBuilderContext, IMapConverter> factory)
        {
            Entries.Add(type, new Entry((b, t) => size, factory));
        }

        public bool Match(Type type)
        {
            return Entries.ContainsKey(type);
        }

        public int CalcSize(Type type)
        {
            return Entries[type].CalcSize((TBuilder)this, type);
        }

        public IMapConverter CreateConverter(IBuilderContext context, Type type)
        {
            return Entries[type].Factory((TBuilder)this, type, context);
        }

        private class Entry
        {
            public Func<TBuilder, Type, int> CalcSize { get; }

            public Func<TBuilder, Type, IBuilderContext, IMapConverter> Factory { get; }

            public Entry(Func<TBuilder, Type, int> calcSize, Func<TBuilder, Type, IBuilderContext, IMapConverter> factory)
            {
                CalcSize = calcSize;
                Factory = factory;
            }
        }
    }
}
