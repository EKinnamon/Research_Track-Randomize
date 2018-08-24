using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EKSurvey.Core.Services
{
    internal class AfterMapperActionCollection : ICollection<AfterMapperAction>
    {
        private readonly ICollection<AfterMapperAction> _actions = new HashSet<AfterMapperAction>();

        public AfterMapperAction this[Type srcType, Type destType]
        {
            get => _actions.SingleOrDefault(a => a.SrcType == srcType && a.DestType == destType);
            set
            { 
                if (value.SrcType == srcType && value.DestType == destType)
                    _actions.Add(value);

                throw new IndexOutOfRangeException("Value does not match index.");
            }
        }

        public bool Exists<T, TDto>() => this[typeof(T), typeof(TDto)] != null;

        public IEnumerator<AfterMapperAction> GetEnumerator() => _actions.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public void Add(AfterMapperAction item)
        {
            _actions.Add(item);
        }
        public void Clear()
        {
            _actions.Clear();
        }
        public bool Contains(AfterMapperAction item)
        {
            return _actions.Contains(item);
        }
        public void CopyTo(AfterMapperAction[] array, int arrayIndex)
        {
            _actions.CopyTo(array, arrayIndex);
        }
        public bool Remove(AfterMapperAction item)
        {
            return _actions.Remove(item);
        }
        public int Count => _actions.Count;
        public bool IsReadOnly => false;
    }
}