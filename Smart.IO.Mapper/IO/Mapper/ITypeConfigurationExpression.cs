namespace Smart.IO.Mapper
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITypeExpression<T>
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="length"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ITypeExpression<T> Filler(int length, byte value);

        /// <summary>
        ///
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ITypeExpression<T> Filler(int offset, int length, byte value);

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        ITypeExpression<T> Constant(byte[] value);

        /// <summary>
        ///
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ITypeExpression<T> Constant(int offset, byte[] value);

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="length"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        ITypeExpression<T> ForMember(string name, int length, Action<IMemberConfigurationExpression> config);

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        ITypeExpression<T> ForMember(string name, int offset, int length, Action<IMemberConfigurationExpression> config);

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        ITypeExpression<T> ForMember(string name, int length);

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        ITypeExpression<T> ForMember(string name, int offset, int length);

        /// <summary>
        ///
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="length"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        ITypeExpression<T> ForMember(Expression<Func<T, object>> expr, int length, Action<IMemberConfigurationExpression> config);

        /// <summary>
        ///
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        ITypeExpression<T> ForMember(Expression<Func<T, object>> expr, int offset, int length, Action<IMemberConfigurationExpression> config);

        /// <summary>
        ///
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        ITypeExpression<T> ForMember(Expression<Func<T, object>> expr, int length);

        /// <summary>
        ///
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        ITypeExpression<T> ForMember(Expression<Func<T, object>> expr, int offset, int length);
    }
}
