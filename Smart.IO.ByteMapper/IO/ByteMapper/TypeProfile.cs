namespace Smart.IO.ByteMapper
{
    using System;

    internal struct TypeProfile : IEquatable<TypeProfile>
    {
        public Type Type { get; }

        public string Profile { get; }

        public TypeProfile(Type type, string profile)
        {
            Type = type;
            Profile = profile;
        }

        public bool Equals(TypeProfile other) => Type == other.Type && Profile == other.Profile;

        public override bool Equals(object obj) => obj is TypeProfile other && Type == other.Type && Profile == other.Profile;

        public override int GetHashCode()
        {
            var hash = Type.GetHashCode();
            hash = hash ^ Profile.GetHashCode(StringComparison.Ordinal);
            return hash;
        }
    }
}