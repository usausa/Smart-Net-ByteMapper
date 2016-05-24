namespace Smart.IO.Mapper
{
    using System;
    using System.Text;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// ByteMapperTest の概要の説明
    /// </summary>
    [TestClass]
    public class ByteMapperTest
    {
        private static readonly Encoding SjisEncoding = Encoding.GetEncoding("Shift_JIS");

        // ------------------------------------------------------------
        // Basic
        // ------------------------------------------------------------

        private class BasicEntity
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }

        private class BasicProfile : IMapperProfile
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Ignore")]
            public void Configure(IMapperConfigurationExpresion config)
            {
                config
                    .DefaultEncording(SjisEncoding)
                    .DefaultPadding(0x20)
                    .DefaultNullIfEmpty(true)
                    .DefaultDelimitter(new byte[] { 0x0D, 0x0A })
                    .DefaultPadding(typeof(int), Padding.Left);

                config.CreateMap<BasicEntity>(17)
                    .ForMember(_ => _.Id, 5)
                    .ForMember(_ => _.Name, 10);
            }
        }

        [TestMethod]
        public void TestBasic()
        {
            var builder = new MapperConfigBuilder(new BasicProfile());
            var mapper = new ByteMapper(builder.Build());

            var entity = new BasicEntity { Id = 1, Name = "うさうさ" };
            var bytes = mapper.ToByte(entity);

            CollectionAssert.AreEqual(SjisEncoding.GetBytes("    1うさうさ  \r\n"), bytes);

            var entity2 = mapper.FromByte<BasicEntity>(bytes);

            Assert.AreEqual(entity.Id, entity2.Id);
            Assert.AreEqual(entity.Name, entity2.Name);
        }

        [TestMethod]
        public void TestBasicNull()
        {
            var builder = new MapperConfigBuilder(new BasicProfile());
            var mapper = new ByteMapper(builder.Build());

            var entity = mapper.FromByte<BasicEntity>(SjisEncoding.GetBytes("               \r\n"));

            Assert.AreEqual(0, entity.Id);
            Assert.AreEqual(null, entity.Name);
        }

        // ------------------------------------------------------------
        // Nullable
        // ------------------------------------------------------------

        private class NullableEntity
        {
            public int? Value { get; set; }
        }

        private class NullableProfile : IMapperProfile
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Ignore")]
            public void Configure(IMapperConfigurationExpresion config)
            {
                config
                    .DefaultEncording(SjisEncoding)
                    .DefaultPadding(0x20)
                    .DefaultPadding(typeof(int), Padding.Left);

                config.CreateMap<NullableEntity>(5)
                    .ForMember(_ => _.Value, 5);
            }
        }

        [TestMethod]
        public void TestNullableNull()
        {
            var builder = new MapperConfigBuilder(new NullableProfile());
            var mapper = new ByteMapper(builder.Build());

            var entity = new NullableEntity { Value = null };
            var bytes = mapper.ToByte(entity);

            CollectionAssert.AreEqual(SjisEncoding.GetBytes("     "), bytes);

            var entity2 = mapper.FromByte<NullableEntity>(bytes);

            Assert.AreEqual(entity.Value, entity2.Value);
        }

        [TestMethod]
        public void TestNullableHasValue()
        {
            var builder = new MapperConfigBuilder(new NullableProfile());
            var mapper = new ByteMapper(builder.Build());

            var entity = new NullableEntity { Value = 123 };
            var bytes = mapper.ToByte(entity);

            CollectionAssert.AreEqual(SjisEncoding.GetBytes("  123"), bytes);

            var entity2 = mapper.FromByte<NullableEntity>(bytes);

            Assert.AreEqual(entity.Value, entity2.Value);
        }

        // ------------------------------------------------------------
        // Format
        // ------------------------------------------------------------

        private class FormatEntity
        {
            public float Value { get; set; }
        }

        private class FormatProfile : IMapperProfile
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Ignore")]
            public void Configure(IMapperConfigurationExpresion config)
            {
                config
                    .DefaultEncording(SjisEncoding)
                    .DefaultPadding(0x20)
                    .DefaultPadding(typeof(float), Padding.Left);

                config.CreateMap<FormatEntity>(5)
                    .ForMember(_ => _.Value, 5, c => c.Formatter("F2"));
            }
        }

        [TestMethod]
        public void TestFormat()
        {
            var builder = new MapperConfigBuilder(new FormatProfile());
            var mapper = new ByteMapper(builder.Build());

            var entity = new FormatEntity { Value = 1.23f };
            var bytes = mapper.ToByte(entity);

            CollectionAssert.AreEqual(SjisEncoding.GetBytes(" 1.23"), bytes);

            var entity2 = mapper.FromByte<FormatEntity>(bytes);

            Assert.AreEqual(entity.Value, entity2.Value);
        }

        // ------------------------------------------------------------
        // DateTime
        // ------------------------------------------------------------

        private class DateTimeEntity
        {
            public DateTime Value1 { get; set; }

            public DateTime? Value2 { get; set; }
        }

        private class DateTimeProfile : IMapperProfile
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Ignore")]
            public void Configure(IMapperConfigurationExpresion config)
            {
                config
                    .DefaultEncording(SjisEncoding)
                    .DefaultPadding(0x20);

                config.CreateMap<DateTimeEntity>(28)
                    .ForMember(_ => _.Value1, 14, c => c.DateTime("yyyyMMddHHmmss"))
                    .ForMember(_ => _.Value2, 14, c => c.DateTime("yyyyMMddHHmmss"));
            }
        }

        [TestMethod]
        public void TestDateTime()
        {
            var builder = new MapperConfigBuilder(new DateTimeProfile());
            var mapper = new ByteMapper(builder.Build());

            var entity = new DateTimeEntity { Value1 = new DateTime(2000, 12, 31, 23, 59, 59), Value2 = null };
            var bytes = mapper.ToByte(entity);

            CollectionAssert.AreEqual(SjisEncoding.GetBytes("20001231235959              "), bytes);

            var entity2 = mapper.FromByte<DateTimeEntity>(bytes);

            Assert.AreEqual(entity.Value1, entity2.Value1);
            Assert.AreEqual(entity.Value2, entity2.Value2);
        }

        // ------------------------------------------------------------
        // Enum
        // ------------------------------------------------------------

        // TODO Add Enum support

        // TODO Add NumberStyle parse support
    }
}
