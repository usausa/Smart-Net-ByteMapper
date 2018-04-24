namespace Smart.IO.ByteMapper.Expressions
{
    using System;

    using Smart.Functional;

    using Xunit;

    public class TypeConfigExpressionTest
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
                .CreateMapByExpression<MapNullFillerObject>(2, c => c.NullFiller(0xFF))
                .CreateMapByExpression<DefaultNullFillerObject>(2, c => { })
                .ToMapperFactory();
            var mapMapper = mapperFactory.Create<MapNullFillerObject>();
            var defaultMapper = mapperFactory.Create<DefaultNullFillerObject>();

            // Write
            Assert.Equal(new byte[] { 0xFF, 0xFF }, mapMapper.ToByte(null));

            Assert.Equal(new byte[] { 0xCC, 0xCC }, defaultMapper.ToByte(null));
        }

        internal class MapNullFillerObject
        {
        }

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
                .CreateMapByExpression<TypeFillerObject>(2, c => c.AutoFiller(true).TypeFiller(0xFF))
                .CreateMapByExpression<DefaultFillerObject>(2, c => c.AutoFiller(true))
                .CreateMapByExpression<NoFillerObject>(2, c => c.AutoFiller(false))
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

        internal class TypeFillerObject
        {
        }

        internal class DefaultFillerObject
        {
        }

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
                .CreateMapByExpression<TypeDelimitterObject>(2, c => c.AutoFiller(false).UseDelimitter(0xFF))
                .CreateMapByExpression<DefaultDelimitterObject>(2, c => c.AutoFiller(false).UseDelimitter(true))
                .CreateMapByExpression<NoDelimitterObject>(2, c => c.AutoFiller(false).UseDelimitter(null))
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

        internal class TypeDelimitterObject
        {
        }

        internal class DefaultDelimitterObject
        {
        }

        internal class NoDelimitterObject
        {
        }

        //--------------------------------------------------------------------------------
        // Fix
        //--------------------------------------------------------------------------------

        [Fact]
        public void CoverageFix()
        {
            // Map
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new MapperFactoryConfig().CreateMapByExpression<DummyObject>(0, c => c.Constant(-1, Array.Empty<byte>())));

            // ForMember
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new MapperFactoryConfig().CreateMapByExpression<DummyObject>(0, c => c.ForMember("x", -1, null)));
            Assert.Throws<ArgumentNullException>(() =>
                new MapperFactoryConfig().CreateMapByExpression<DummyObject>(0, c => c.ForMember("x", null)));
            Assert.Throws<ArgumentException>(() =>
                new MapperFactoryConfig().CreateMapByExpression<DummyObject>(0, c => c.ForMember("x", m => { })));
            Assert.Throws<InvalidOperationException>(() =>
                new MapperFactoryConfig().CreateMapByExpression<DummyObject>(0, c => c.ForMember(x => x.IntValue, m => { })));
            Assert.Throws<ByteMapperException>(() =>
                new MapperFactoryConfig().CreateMapByExpression<DummyObject>(0, c => c.ForMember(x => x.IntValue, m => m.Boolean())));

            var mapperFactory = new MapperFactoryConfig()
                .DefaultDelimiter(null)
                .Also(config =>
                {
                    config.CreateMapByExpression<DummyObject>(5, c => c
                        .ForMember(nameof(DummyObject.IntValue), 0, m => m.Binary())
                        .ForMember(nameof(DummyObject.BoolValue), m => m.Boolean()));
                })
                .ToMapperFactory();
            Assert.NotNull(mapperFactory.Create<DummyObject>());
        }

        internal class DummyObject
        {
            public int IntValue { get; set; }

            public bool BoolValue { get; set; }
        }
    }
}
