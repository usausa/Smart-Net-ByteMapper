namespace Smart.IO.Mapper.Expressions
{
    using System;
    using System.Collections.Generic;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Mappers;

    internal class MapBuilder<T> : ITypeConfigSyntax<T>, IMapping
    {
        // TODO lastOffset max

        private bool useFiller = true;

        private bool useDelimitter = true;

        public Type Type { get; }

        public string Profile { get; set; } // TODO

        public int Size { get; set; } // TODO

        public MapBuilder(Type type)
        {
            Type = type;
        }

        public ITypeConfigSyntax<T> UseFiller(bool value)
        {
            useFiller = value;
            return this;
        }

        public ITypeConfigSyntax<T> UseDelimitter(bool value)
        {
            useDelimitter = value;
            return this;
        }

        public ITypeConfigSyntax<T> UseDelimitter(params byte[] delimitter)
        {
            throw new NotImplementedException();
        }

        public ITypeConfigSyntax<T> AddMapper(ITypeMapFactory factory)
        {
            throw new NotImplementedException();
        }

        public ITypeConfigSyntax<T> AddMapper(int offset, ITypeMapFactory factory)
        {
            throw new NotImplementedException();
        }

        public IMapper[] CreateMappers(IComponentContainer components, IDictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }
    }
}
