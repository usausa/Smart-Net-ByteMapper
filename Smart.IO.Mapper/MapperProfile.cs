namespace Smart.IO.Mapper
{
    using System;
    using System.Collections.Generic;

    public class MapperProfile : IMapperProfile
    {
        private readonly List<IMapping> mappings = new List<IMapping>();

        public string Name { get; }

        public MapperProfile()
        {
        }

        public MapperProfile(string name)
        {
            Name = name;
        }

        public MapperProfile AddMapping(IMapping mapping)
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

        IEnumerable<IMapping> IMapperProfile.ResolveMappings()
        {
            return mappings;
        }
    }
}
