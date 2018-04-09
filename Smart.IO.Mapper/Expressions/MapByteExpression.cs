//namespace Smart.IO.Mapper.Expressions
//{
//    using System;

//    using Smart.ComponentModel;
//    using Smart.IO.Mapper.Converters;
//    using Smart.IO.Mapper.Helpers;

//    internal sealed class MapByteExpression : IMemberMapFactory
//    {
//        private static readonly IMapConverter MapConverter = new ByteConverter();

//        public int CalcSize(Type type)
//        {
//            return 1;
//        }

//        public IMapConverter CreateConverter(IComponentContainer components, IMappingParameter parameters, Type type)
//        {
//            return MapConverter;
//        }
//    }
//}
