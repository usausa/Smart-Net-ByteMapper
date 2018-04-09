namespace Smart.IO.Mapper.Expressions
{
    using System;
    using System.Linq.Expressions;

    public interface ITypeConfigSyntax<T>
    {
        // Type default

        ITypeConfigSyntax<T> TypeDefault(string key, object value);

        // Type setting

        ITypeConfigSyntax<T> AutoFiller(bool value);

        ITypeConfigSyntax<T> UseDelimitter(bool value);

        // Mapper

        ITypeConfigSyntax<T> Map(ITypeMapExpression expression);

        ITypeConfigSyntax<T> Map(int offset, ITypeMapExpression expression);

        // ForMember

        ITypeConfigSyntax<T> ForMember(string name, Action<IMemberConfigSyntax> config);

        ITypeConfigSyntax<T> ForMember(string name, int offset, Action<IMemberConfigSyntax> config);

        ITypeConfigSyntax<T> ForMember(Expression<Func<T, object>> expr, Action<IMemberConfigSyntax> config);

        ITypeConfigSyntax<T> ForMember(Expression<Func<T, object>> expr, int offset, Action<IMemberConfigSyntax> config);
    }
}
