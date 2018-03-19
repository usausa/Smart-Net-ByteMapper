namespace Smart.IO.Mapper
{
    using System;

    public class MapperEntry
    {
        public Type TargetType { get; }

        public string Profile { get; }

        public IMemberMapperFactory[] Factories { get; }

        public MapperEntry(Type targetType, string profile, IMemberMapperFactory[] factories)
        {
            TargetType = targetType;
            Profile = profile;
            Factories = factories;
        }
    }
}
