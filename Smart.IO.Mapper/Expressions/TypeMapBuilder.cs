namespace Smart.IO.Mapper.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Mappers;

    internal class MapBuilder<T> : ITypeConfigSyntax<T>, IMapping
    {
        private readonly Dictionary<string, object> parameters = new Dictionary<string, object>();

        // TODO lastOffset max

        // TODO list1
        // TODO list2

        private bool useFiller = true;

        private bool useDelimitter = true;

        public Type Type { get; }

        public string Profile { get; set; } // TODO

        public int Size { get; set; } // TODO

        public MapBuilder(Type type)
        {
            Type = type;
        }

        // Type default

        public ITypeConfigSyntax<T> AddTypeDefault(string key, object value)
        {
            parameters[key] = value;
            return this;
        }

        // Type setting

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

        // Mapper

        public ITypeConfigSyntax<T> AddMapper(ITypeMapFactory factory)
        {
            throw new NotImplementedException();
        }

        public ITypeConfigSyntax<T> AddMapper(int offset, ITypeMapFactory factory)
        {
            throw new NotImplementedException();
        }

        // ForMember

        public ITypeConfigSyntax<T> ForMember(string name, Action<IMemberMapConfigSyntax> config)
        {
            throw new NotImplementedException();
        }

        public ITypeConfigSyntax<T> ForMember(string name, int offset, Action<IMemberMapConfigSyntax> config)
        {
            throw new NotImplementedException();
        }

        public ITypeConfigSyntax<T> ForMember(Expression<Func<T, object>> expr, Action<IMemberMapConfigSyntax> config)
        {
            throw new NotImplementedException();
        }

        public ITypeConfigSyntax<T> ForMember(Expression<Func<T, object>> expr, int offset, Action<IMemberMapConfigSyntax> config)
        {
            throw new NotImplementedException();
        }

        public IMapper[] CreateMappers(IComponentContainer components, IDictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        // TODO Entry
        // TODO Entry
    }
}
