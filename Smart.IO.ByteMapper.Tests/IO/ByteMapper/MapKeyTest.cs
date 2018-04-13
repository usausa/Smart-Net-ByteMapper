namespace Smart.IO.ByteMapper
{
    using Xunit;

    public class MapKeyTest
    {
        [Fact]
        public void MapKeyCompare()
        {
            var key1 = new MapKey(typeof(object), string.Empty);
            var key1B = key1;

            // Compare to null
            Assert.False(key1.Equals(null));

            // Compar to self
            Assert.True(key1.Equals(key1B));

            // Compar to same
            Assert.True(key1.Equals(new MapKey(typeof(object), string.Empty)));

            // Compar to different type
            Assert.False(key1.Equals(new MapKey(typeof(string), string.Empty)));

            // Compar to different name
            Assert.False(key1.Equals(new MapKey(typeof(object), "x")));

            // Compar to another type
            Assert.False(key1.Equals(new object()));
        }
    }
}
