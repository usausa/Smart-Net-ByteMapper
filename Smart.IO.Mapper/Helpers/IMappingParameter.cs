namespace Smart.IO.Mapper.Helpers
{
    public interface IMappingParameter
    {
        T GetParameter<T>(string key);
    }
}
