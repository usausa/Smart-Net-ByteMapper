namespace Smart.IO.MapperOld
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITypeConfigurationExpression<T>
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="length"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ITypeConfigurationExpression<T> Filler(int length, byte value);

        /// <summary>
        ///
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ITypeConfigurationExpression<T> Filler(int offset, int length, byte value);

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        ITypeConfigurationExpression<T> Constant(byte[] value);

        /// <summary>
        ///
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ITypeConfigurationExpression<T> Constant(int offset, byte[] value);

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="length"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        ITypeConfigurationExpression<T> ForMember(string name, int length, Action<IMemberConfigurationExpression> config);

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        ITypeConfigurationExpression<T> ForMember(string name, int offset, int length, Action<IMemberConfigurationExpression> config);

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        ITypeConfigurationExpression<T> ForMember(string name, int length);

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        ITypeConfigurationExpression<T> ForMember(string name, int offset, int length);

        /// <summary>
        ///
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="length"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        ITypeConfigurationExpression<T> ForMember(Expression<Func<T, object>> expr, int length, Action<IMemberConfigurationExpression> config);

        /// <summary>
        ///
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        ITypeConfigurationExpression<T> ForMember(Expression<Func<T, object>> expr, int offset, int length, Action<IMemberConfigurationExpression> config);

        /// <summary>
        ///
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        ITypeConfigurationExpression<T> ForMember(Expression<Func<T, object>> expr, int length);

        /// <summary>
        ///
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        ITypeConfigurationExpression<T> ForMember(Expression<Func<T, object>> expr, int offset, int length);
    }
}
