namespace Smart.IO.Mapper.Expressions
{
    using System.Text;

    using Smart.IO.Mapper.Builders;

    public interface IMapTextSyntax
    {
        IMapTextSyntax Encoding(Encoding value);

        IMapTextSyntax Trim(bool value);

        IMapTextSyntax Padding(Padding value);

        IMapTextSyntax Filler(byte value);
    }

    internal sealed class MapTextExpression : IMemberMapExpression, IMapTextSyntax
    {
        private readonly TextConverterBuilder builder = new TextConverterBuilder();

        public MapTextExpression(int length)
        {
            builder.Length = length;
        }

        public IMapTextSyntax Encoding(Encoding value)
        {
            builder.Encoding = value;
            return this;
        }

        public IMapTextSyntax Trim(bool value)
        {
            builder.Trim = value;
            return this;
        }

        public IMapTextSyntax Padding(Padding value)
        {
            builder.Padding = value;
            return this;
        }

        public IMapTextSyntax Filler(byte value)
        {
            builder.Filler = value;
            return this;
        }

        IMapConverterBuilder IMemberMapExpression.GetMapConverterBuilder()
        {
            return builder;
        }
    }
}
