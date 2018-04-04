namespace Smart.IO.Mapper.Mock
{
    using Smart.IO.Mapper.Helpers;

    public class MockMappingParametercs : IMappingParameter
    {
        public T GetParameter<T>(string key)
        {
            return default;
        }
    }
}
