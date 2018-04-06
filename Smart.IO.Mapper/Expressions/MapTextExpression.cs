namespace Smart.IO.Mapper.Expressions
{
    using System;
    using System.Text;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Converters;
    using Smart.IO.Mapper.Helpers;

    public interface IMapTextSyntax
    {
        IMapDateTimeSyntax Encoding(Encoding value);

        IMapDateTimeSyntax Trim(bool value);

        IMapDateTimeSyntax Padding(Padding value);

        IMapDateTimeSyntax Filler(byte value);
    }

    internal sealed class MapTextExpression : IMemberMapFactory, IMapTextSyntax
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

        public int CalcSize(Type type)
        {
            throw new NotImplementedException();
        }

        public IByteConverter CreateConverter(IComponentContainer components, IMappingParameter parameters, Type type)
        {
            throw new NotImplementedException();
        }
    }
}
