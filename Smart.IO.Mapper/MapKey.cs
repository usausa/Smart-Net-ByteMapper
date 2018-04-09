namespace Smart.IO.Mapper
{
    using System;

    internal sealed class MapKey
    {
        public Type Type { get; }

        public string Profile { get; }

        public MapKey(Type type, string profile)
        {
            Type = type;
            Profile = profile;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is MapKey other)
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