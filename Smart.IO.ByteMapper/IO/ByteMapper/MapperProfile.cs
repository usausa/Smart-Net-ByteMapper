namespace Smart.IO.ByteMapper
{
    using System;
    using System.Collections.Generic;

    public class MapperProfile : IMapperProfile
    {
        private readonly List<IMappingFactory> factories = new();

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Ignore")]
        IEnumerable<IMappingFactory> IMapperProfile.ResolveMappingFactories() => factories;
    }
}
