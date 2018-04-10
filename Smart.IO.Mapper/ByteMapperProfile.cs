namespace Smart.IO.Mapper
{
    using System;
    using System.Collections.Generic;

    public class ByteMapperProfile : IByteMapperProfile
    {
        private readonly List<IMapping> mappings = new List<IMapping>();

        public string Name { get; }

        public ByteMapperProfile()
        {
        }

        public ByteMapperProfile(string name)
        {
            Name = name;
        }

        public ByteMapperProfile AddMapping(IMapping mapping)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            mappings.Add(mapping);
            return this;
        }

        //--------------------------------------------------------------------------------
        // IByteMapperProfile
        //--------------------------------------------------------------------------------

        IEnumerable<IMapping> IByteMapperProfile.ResolveMappings()
        {
            return mappings;
        }
    }
}
