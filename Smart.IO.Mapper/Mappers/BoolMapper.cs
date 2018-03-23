namespace Smart.IO.Mapper.Mappers
{
    using System;

    public sealed class BoolMapper : IMemberMapper
    {
        private readonly int offset;

        private readonly Func<object, object> getter;

        private readonly Action<object, object> setter;

        private readonly byte trueValue;

        private readonly byte falseValue;

        public int Length => 1;

        public bool CanRead => getter != null;

        public bool CanWrite => setter != null;

        public BoolMapper(
            int offset,
            Func<object, object> getter,
            Action<object, object> setter,
            byte trueValue,
            byte falseValue)
        {
            this.offset = offset;
            this.getter = getter;
            this.setter = setter;
            this.trueValue = trueValue;
            this.falseValue = falseValue;
        }

        public void Read(byte[] buffer, int index, object target)
        {
            setter(target, buffer[index + offset] == trueValue);
        }

        public void Write(byte[] buffer, int index, object target)
        {
            buffer[index + offset] = (bool)getter(target) ? trueValue : falseValue;
        }
    }

    public sealed class NullableBoolMapper : IMemberMapper
    {
        private readonly int offset;

        private readonly Func<object, object> getter;

        private readonly Action<object, object> setter;

        private readonly byte trueValue;

        private readonly byte falseValue;

        private readonly byte nullValue;

        public int Length => 1;

        public bool CanRead => getter != null;

        public bool CanWrite => setter != null;

        public NullableBoolMapper(
            int offset,
            Func<object, object> getter,
            Action<object, object> setter,
            byte trueValue,
            byte falseValue,
            byte nullValue)
        {
            this.offset = offset;
            this.getter = getter;
            this.setter = setter;
            this.trueValue = trueValue;
            this.falseValue = falseValue;
            this.nullValue = nullValue;
        }

        public void Read(byte[] buffer, int index, object target)
        {
            var b = buffer[index + offset];
            setter(target, b == trueValue ? true : b == nullValue ? (bool?)null : false);
        }

        public void Write(byte[] buffer, int index, object target)
        {
            var value = (bool?)getter(target);
            buffer[index + offset] = value.HasValue ? (value.Value ? trueValue : falseValue) : nullValue;
        }
    }
}
