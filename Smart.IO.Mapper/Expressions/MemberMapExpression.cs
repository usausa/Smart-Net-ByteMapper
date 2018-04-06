namespace Smart.IO.Mapper.Expressions
{
    using System;
    using Smart.ComponentModel;
    using Smart.IO.Mapper.Converters;
    using Smart.IO.Mapper.Helpers;

    internal class MemberMapExpression : IMemberConfigSyntax
    {
        public IMemberMapFactory Factory { get; private set; }

        public void SetMemberMapFactory(IMemberMapFactory factory)
        {
            Factory = factory;
        }

        public IMemberMapConfigSyntax Array(int count)
        {
            // TODO thisを返す
            throw new NotImplementedException();
        }
    }

    internal class MemberMapArrayBuilder : IMemberMapConfigSyntax
    {
        public void SetMemberMapFactory(IMemberMapFactory factory)
        {
            throw new NotImplementedException();
        }
    }
}
