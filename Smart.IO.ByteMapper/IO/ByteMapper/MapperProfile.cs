namespace Smart.IO.ByteMapper
{
    using System;
    using System.Collections.Generic;

    public class MapperProfile : IMapperProfile
    {
        private readonly List<IMappingFactory> factories = new List<IMappingFactory>();

        public string Name { get; }

        public MapperProfile()
        {
        }

        public MapperProfile(string name)
        {
            Name = name;
        }

        public MapperProfile AddMappingFactory(IMappingFactory factory)
        {
            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            factories.Add(factory);
            return this;
        }

        //--------------------------------------------------------------------------------
        // IByteMapperProfile
        //--------------------------------------------------------------------------------

        IEnumerable<IMappingFactory> IMapperProfile.ResolveMappingFactories()
        {
            return factories;
        }
    }
}
