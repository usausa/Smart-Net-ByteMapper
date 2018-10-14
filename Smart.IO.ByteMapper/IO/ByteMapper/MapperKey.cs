namespace Smart.IO.ByteMapper
{
    using System;

    public struct MapperKey : IEquatable<MapperKey>
    {
        public Type Type { get; }

        public string Profile { get; }

        public MapperKey(Type type, string profile)
        {
            Type = type;
            Profile = profile;
        }

        public bool Equals(MapperKey other)
        {
            return Type == other.Type && Profile == other.Profile;
        }

        public override bool Equals(object obj)
        {
            return obj is MapperKey other && Type == other.Type && Profile == other.Profile;
        }

        public override int GetHashCode()
        {
            var hash = Type.GetHashCode();
            hash = hash ^ Profile.GetHashCode();
            return hash;
        }
    }
}