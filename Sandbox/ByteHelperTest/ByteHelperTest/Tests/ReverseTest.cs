namespace ByteHelperTest.Tests
{
    using Xunit;

    public class ReverseTest
    {
        [Fact]
        public void Reverse()
        {
            var buffer = new byte[] { 1, 2, 3 };
            ByteHelper.Reverse(buffer, 0, buffer.Length);

            Assert.Equal(new byte[] { 3, 2, 1 }, buffer);

            buffer = new byte[] { 1, 2, 3, 4 };
            ByteHelper.Reverse(buffer, 0, buffer.Length);

            Assert.Equal(new byte[] { 4, 3, 2, 1 }, buffer);
        }

        [Fact]
        public void ReverseUnsafe()
        {
            var buffer = new byte[] { 1, 2, 3 };
            ByteHelper.ReverseUnsafe(buffer, 0, buffer.Length);

            Assert.Equal(new byte[] { 3, 2, 1 }, buffer);

            buffer = new byte[] { 1, 2, 3, 4 };
            ByteHelper.ReverseUnsafe(buffer, 0, buffer.Length);

            Assert.Equal(new byte[] { 4, 3, 2, 1 }, buffer);
        }
    }
}
