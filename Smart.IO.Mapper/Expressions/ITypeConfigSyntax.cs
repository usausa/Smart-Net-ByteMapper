namespace Smart.IO.Mapper.Expressions
{
    using System;
    using System.Linq.Expressions;

    public interface ITypeConfigSyntax<T>
    {
        // Type default

        ITypeConfigSyntax<T> AddTypeDefault(string key, object value);

        // Type setting

        ITypeConfigSyntax<T> UseFiller(bool value);

        ITypeConfigSyntax<T> UseDelimitter(bool value);

        // Mapper

        ITypeConfigSyntax<T> AddMapper(ITypeMapFactory factory);

        ITypeConfigSyntax<T> AddMapper(int offset, ITypeMapFactory factory);

        // ForMember

        ITypeConfigSyntax<T> ForMember(string name, Action<IMemberMapConfigSyntax> config);

        ITypeConfigSyntax<T> ForMember(string name, int offset, Action<IMemberMapConfigSyntax> config);

        ITypeConfigSyntax<T> ForMember(Expression<Func<T, object>> expr, Action<IMemberMapConfigSyntax> config);

        ITypeConfigSyntax<T> ForMember(Expression<Func<T, object>> expr, int offset, Action<IMemberMapConfigSyntax> config);
    }
}
