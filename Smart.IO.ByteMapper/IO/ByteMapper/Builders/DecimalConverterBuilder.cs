namespace Smart.IO.ByteMapper.Builders
{
    using System;

    using Smart.IO.ByteMapper.Converters;
    using Smart.IO.ByteMapper.Helpers;

    public sealed class DecimalConverterBuilder : AbstractMapConverterBuilder<DecimalConverterBuilder>
    {
        public int Length { get; set; }

        public byte Scale { get; set; }

        public bool? UseGrouping { get; set; }

        public int GroupingSize { get; set; } = 3;

        public Padding? Padding { get; set; }

        public bool? ZeroFill { get; set; }

        public byte? Filler { get; set; }

        static DecimalConverterBuilder()
        {
            AddEntry(typeof(decimal), (b, t) => b.Length, (b, t, c) => b.CreateDecimalConverter(t, c));
            AddEntry(typeof(decimal?), (b, t) => b.Length, (b, t, c) => b.CreateDecimalConverter(t, c));
        }

        private IMapConverter CreateDecimalConverter(Type type, IBuilderContext context)
        {
            var groupingSize = UseGrouping ?? context.GetParameter<bool>(Parameter.UseGrouping) ? GroupingSize : 0;
            if (!BytesHelper.IsDecimalLimited64Applicable(Length, Scale, groupingSize))
            {
                throw new InvalidOperationException($"Parameter is invalid. length=[{Length}], scale=[{Scale}], groupingSize=[{groupingSize}]");
            }

            return new DecimalConverter(
                Length,
                Scale,
                groupingSize,
                Padding ?? context.GetParameter<Padding>(Parameter.NumberPadding),
                ZeroFill ?? context.GetParameter<bool>(Parameter.ZeroFill),
                Filler ?? context.GetParameter<byte>(Parameter.NumberFiller),
                type);
        }
    }
}
