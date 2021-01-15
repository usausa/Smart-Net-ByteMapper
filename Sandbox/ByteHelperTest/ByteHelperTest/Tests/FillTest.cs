namespace ByteHelperTest.Tests
{
    using Xunit;

    public class FillTest
    {
        [Fact]
        public void Fill()
        {
            var buffer = new byte[8].FillMemoryCopy(1, 6, 0x01);

            Assert.Equal(new byte[] { 0, 1, 1, 1, 1, 1, 1, 0 }, buffer);
        }
    }
}
