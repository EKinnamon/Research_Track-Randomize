using System;

namespace EKSurvey.Core.Services
{
    internal class AfterMapperAction<T, TDto> : AfterMapperAction
    {
        public AfterMapperAction() : base(typeof(T), typeof(TDto)) { }
    }

    internal class AfterMapperAction
    {
        public Type SrcType { get; }

        public Type DestType { get; }

        public Action<object,object> Action { get; set; }

        public AfterMapperAction(Type srcType, Type destType)
        {
            SrcType = srcType;
            DestType = destType;
        }
    }
}