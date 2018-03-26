namespace SjisTest
{
    public static class SjisHelper
    {
        public static int ParseInt(byte[] bytes, int index, int count)
        {
            var end = index + count;

            // No check, No minus
            var value = 0;
            for (var i = index; i < end; i++)
            {
                value *= 10;
                value += bytes[i] - '0';
            }

            return value;
        }
    }
}
