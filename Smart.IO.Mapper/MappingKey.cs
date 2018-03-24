namespace Smart.IO.Mapper
{
    using System;

    public class MappingKey
    {
        public Type Type { get; }

        public string Profile { get; }

        public MappingKey(Type type, string profile)
        {
            Type = type;
            Profile = profile;
        }

        public override bool Equals(object obj)
        {
            if (obj is MappingKey other)
            {
                return Type == other.Type && Profile == other.Profile;
            }

            return false;
        }

        public override int GetHashCode()
        {
            var hash = Type.GetHashCode();
            hash = hash ^ Profile.GetHashCode();
            return hash;
        }
    }
}