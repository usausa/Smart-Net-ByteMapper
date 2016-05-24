namespace Smart.IO.Mapper.Configuration
{
    using Smart.IO.Mapper.Mappers;

    /// <summary>
    ///
    /// </summary>
    internal class MemberConfigurationExpression : IMemberConfigurationExpression
    {
        private readonly MemberMapper memberMapper;

        /// <summary>
        ///
        /// </summary>
        /// <param name="memberMapper"></param>
        public MemberConfigurationExpression(MemberMapper memberMapper)
        {
            this.memberMapper = memberMapper;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="filler"></param>
        /// <returns></returns>
        public IMemberConfigurationExpression Padding(byte filler)
        {
            memberMapper.PaddingByte = filler;
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public IMemberConfigurationExpression Padding(Padding direction)
        {
            memberMapper.Padding = direction;
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="filler"></param>
        /// <returns></returns>
        public IMemberConfigurationExpression Padding(Padding direction, byte filler)
        {
            memberMapper.Padding = direction;
            memberMapper.PaddingByte = filler;
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public IMemberConfigurationExpression Trim(bool value)
        {
            memberMapper.Trim = value;
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public IMemberConfigurationExpression NullIfEmpty(bool value)
        {
            memberMapper.NullIfEmpty = value;
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public IMemberConfigurationExpression Formatter(IFormatter value)
        {
            memberMapper.Formatter = value;
            return this;
        }
    }
}
