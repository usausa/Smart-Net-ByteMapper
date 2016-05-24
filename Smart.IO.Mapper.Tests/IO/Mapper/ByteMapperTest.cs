namespace Smart.IO.Mapper
{
    using System.Text;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// ByteMapperTest の概要の説明
    /// </summary>
    [TestClass]
    public class ByteMapperTest
    {
        private static readonly Encoding SjisEncoding = Encoding.GetEncoding("Shift_JIS");

        private class BasicProfile : IMapperProfile
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Ignore")]
            public void Configure(IMapperConfigurationExpresion config)
            {
                config
                    .DefaultEncording(SjisEncoding)
                    .DefaultPadding(0x20)
                    .DefaultDelimitter(new byte[] { 0x0D, 0x0A })
                    .DefaultPadding(typeof(int), Padding.Left);

                config.CreateMap<Entity>(17)
                    .ForMember(_ => _.Id, 5)
                    .ForMember(_ => _.Name, 10);
            }
        }

        /// <summary>
        ///
        /// </summary>
        [TestMethod]
        public void TestBasic()
        {
            var builder = new MapperConfigBuilder(new BasicProfile());
            var mapper = new ByteMapper(builder.Build());

            var entity = new Entity { Id = 1, Name = "うさうさ" };
            var bytes = mapper.ToByte(entity);
            CollectionAssert.AreEqual(SjisEncoding.GetBytes("    1うさうさ  \r\n"), bytes);

            var entity2 = mapper.FromByte<Entity>(bytes);
            Assert.AreEqual(entity.Id, entity2.Id);
            Assert.AreEqual(entity.Name, entity2.Name);
        }
    }
}
