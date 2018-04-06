namespace Smart.IO.Mapper.Expressions
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Converters;
    using Smart.IO.Mapper.Helpers;

    public interface IMapNumberSyntax
    {
        IMapDateTimeSyntax Encoding(Encoding value);

        IMapDateTimeSyntax Trim(bool value);

        IMapDateTimeSyntax Padding(Padding value);

        IMapDateTimeSyntax Filler(byte value);

        IMapDateTimeSyntax Style(NumberStyles value);

        IMapDateTimeSyntax Provider(IFormatProvider value);
    }

    internal sealed class MapNumberExpression : IMemberMapFactory, IMapNumberSyntax
    {
        public IMapDateTimeSyntax Encoding(Encoding value)
        {
            throw new NotImplementedException();
        }

        public IMapDateTimeSyntax Trim(bool value)
        {
            throw new NotImplementedException();
        }

        public IMapDateTimeSyntax Padding(Padding value)
        {
            throw new NotImplementedException();
        }

        public IMapDateTimeSyntax Filler(byte value)
        {
            throw new NotImplementedException();
        }

        public IMapDateTimeSyntax Style(NumberStyles value)
        {
            throw new NotImplementedException();
        }

        public IMapDateTimeSyntax Provider(IFormatProvider value)
        {
            throw new NotImplementedException();
        }

        public int CalcSize(Type type)
        {
            throw new NotImplementedException();
        }

        public IMapConverter CreateConverter(IComponentContainer components, IMappingParameter parameters, Type type)
        {
            throw new NotImplementedException();
        }

        // TODO
    }
}
