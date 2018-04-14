namespace Smart.IO.ByteMapper.Attributes
{
    using System;

    using Xunit;

    public class MapAttributeTest
    {
        //--------------------------------------------------------------------------------
        // NullFiller
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapUseNullFiller()
        {
            var mapperFactory = new MapperFactoryConfig()
                .DefaultDelimiter(null)
                .DefaultFiller(0xCC)
                .CreateMapByAttribute<MapNullFillerObject>()
                .CreateMapByAttribute<DefaultNullFillerObject>()
                .ToMapperFactory();
            var mapMapper = mapperFactory.Create<MapNullFillerObject>();
            var defaultMapper = mapperFactory.Create<DefaultNullFillerObject>();

            // Write
            Assert.Equal(new byte[] { 0xFF, 0xFF }, mapMapper.ToByte(null));

            Assert.Equal(new byte[] { 0xCC, 0xCC }, defaultMapper.ToByte(null));
        }

        [Map(2, NullFiller = 0xFF)]
        internal class MapNullFillerObject
        {
        }

        [Map(2)]
        internal class DefaultNullFillerObject
        {
        }

        //--------------------------------------------------------------------------------
        // AutoFiller
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapUseAutoFiller()
        {
            var mapperFactory = new MapperFactoryConfig()
                .DefaultDelimiter(null)
                .DefaultFiller(0xCC)
                .CreateMapByAttribute<TypeFillerObject>()
                .CreateMapByAttribute<DefaultFillerObject>()
                .CreateMapByAttribute<NoFillerObject>()
                .ToMapperFactory();
            var typeMapper = mapperFactory.Create<TypeFillerObject>();
            var defaultMapper = mapperFactory.Create<DefaultFillerObject>();
            var noMapper = mapperFactory.Create<NoFillerObject>();

            // Write
            Assert.Equal(new byte[] { 0xFF, 0xFF }, typeMapper.ToByte(new TypeFillerObject()));

            Assert.Equal(new byte[] { 0xCC, 0xCC }, defaultMapper.ToByte(new DefaultFillerObject()));

            var buffer = new byte[noMapper.Size].Fill(0x11);
            noMapper.ToByte(buffer, 0, new NoFillerObject());
            Assert.Equal(new byte[] { 0x11, 0x11 }, buffer);
        }

        [Map(2, AutoFiller = true)]
        [TypeFiller(0xFF)]
        internal class TypeFillerObject
        {
        }

        [Map(2, AutoFiller = true)]
        internal class DefaultFillerObject
        {
        }

        [Map(2, AutoFiller = false)]
        internal class NoFillerObject
        {
        }

        //--------------------------------------------------------------------------------
        // Delimitter
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapUseDelimitter()
        {
            var mapperFactory = new MapperFactoryConfig()
                .DefaultFiller(0x00)
                .DefaultDelimiter(0xCC)
                .CreateMapByAttribute<TypeDelimitterObject>()
                .CreateMapByAttribute<DefaultDelimitterObject>()
                .CreateMapByAttribute<NoDelimitterObject>()
                .ToMapperFactory();
            var typeMapper = mapperFactory.Create<TypeDelimitterObject>();
            var defaultMapper = mapperFactory.Create<DefaultDelimitterObject>();
            var noMapper = mapperFactory.Create<NoDelimitterObject>();

            // Write
            Assert.Equal(new byte[] { 0x00, 0xFF }, typeMapper.ToByte(new TypeDelimitterObject()));

            Assert.Equal(new byte[] { 0x00, 0xCC }, defaultMapper.ToByte(new DefaultDelimitterObject()));

            var buffer = new byte[noMapper.Size].Fill(0x11);
            noMapper.ToByte(buffer, 0, new NoDelimitterObject());
            Assert.Equal(new byte[] { 0x11, 0x11 }, buffer);
        }

        [Map(2, AutoFiller = false, UseDelimitter = true)]
        [TypeDelimiter(0xFF)]
        internal class TypeDelimitterObject
        {
        }

        [Map(2, AutoFiller = false, UseDelimitter = true)]
        internal class DefaultDelimitterObject
        {
        }

        [Map(2, AutoFiller = false, UseDelimitter = false)]
        internal class NoDelimitterObject
        {
        }

        //--------------------------------------------------------------------------------
        // Fix
        //--------------------------------------------------------------------------------

        [Fact]
        public void CoverageFix()
        {
            var attribute = new MapAttribute(0);

            Assert.False(attribute.HasNullFiller);
            Assert.Equal(0, attribute.NullFiller);

            Assert.Throws<ArgumentOutOfRangeException>(() => new MapAttribute(-1));
        }
    }
}
