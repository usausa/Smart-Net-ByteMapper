namespace Smart.IO.ByteMapper;

using Smart.IO.ByteMapper.Helpers;

internal readonly struct TypeProfile : IEquatable<TypeProfile>
{
    private readonly Type type;

    private readonly string profile;

    public Type Type => type;

    public string Profile => profile;

    public TypeProfile(Type type, string profile)
    {
        this.type = type;
        this.profile = profile;
    }

    public bool Equals(TypeProfile other) => type == other.type && profile == other.profile;

    public override bool Equals(object obj) => obj is TypeProfile other && type == other.type && profile == other.profile;

    public override int GetHashCode()
    {
        var hash = type.GetHashCode();
        hash ^= StringHash.CalcNameHash(profile);
        return hash;
    }
}
