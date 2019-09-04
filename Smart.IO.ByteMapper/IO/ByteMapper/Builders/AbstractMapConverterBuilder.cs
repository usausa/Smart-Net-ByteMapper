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
            Entries.Add(type, new Entry { CalcSize = calcSize, Factory = factory });
        }

        protected static void AddEntry(Type type, int size, Func<TBuilder, Type, IBuilderContext, IMapConverter> factory)
        {
            Entries.Add(type, new Entry { CalcSize = (b, t) => size, Factory = factory });
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Performance")]
        private class Entry
        {
            public Func<TBuilder, Type, int> CalcSize;

            public Func<TBuilder, Type, IBuilderContext, IMapConverter> Factory;
        }
    }
}
