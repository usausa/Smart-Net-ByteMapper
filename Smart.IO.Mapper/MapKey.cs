namespace Smart.IO.Mapper
{
    using System;

    internal sealed class MapKey
    {
        public Type Type { get; }

        public string Name { get; }

        public MapKey(Type type, string name)
        {
            Type = type;
            Name = name;
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
                return Type == other.Type && Name == other.Name;
            }

            return false;
        }

        public override int GetHashCode()
        {
            var hash = Type.GetHashCode();
            hash = hash ^ Name.GetHashCode();
            return hash;
        }
    }
}