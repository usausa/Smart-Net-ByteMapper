namespace Smart.IO.ByteMapper.Builders;

using Smart.IO.ByteMapper.Converters;

public abstract class AbstractMapConverterBuilder<TBuilder> : IMapConverterBuilder
    where TBuilder : AbstractMapConverterBuilder<TBuilder>
{
    private static readonly Dictionary<Type, Entry> Entries = [];

    protected static void AddEntry(Type type, Func<TBuilder, Type, int> calcSize, Func<TBuilder, Type, IBuilderContext, IMapConverter> factory)
    {
        Entries.Add(type, new Entry(calcSize, factory));
    }

    protected static void AddEntry(Type type, int size, Func<TBuilder, Type, IBuilderContext, IMapConverter> factory)
    {
        Entries.Add(type, new Entry((_, _) => size, factory));
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
    private sealed class Entry
    {
        public readonly Func<TBuilder, Type, int> CalcSize;

        public readonly Func<TBuilder, Type, IBuilderContext, IMapConverter> Factory;

        public Entry(Func<TBuilder, Type, int> calcSize, Func<TBuilder, Type, IBuilderContext, IMapConverter> factory)
        {
            CalcSize = calcSize;
            Factory = factory;
        }
    }
}
