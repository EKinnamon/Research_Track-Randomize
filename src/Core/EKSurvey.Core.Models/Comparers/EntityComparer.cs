using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EKSurvey.Core.Models.Comparers
{
    public class EntityComparer<T> : IEqualityComparer<T>
    {
        private readonly PropertyInfo[] _properties;

        public static EntityComparer<T> Id => new EntityComparer<T>("Id");

        protected EntityComparer(params string[] comparePropertyNames)
        {
            var type = typeof(T);
            _properties = comparePropertyNames.Select(n => type.GetProperty(n, BindingFlags.DeclaredOnly |
                                                                               BindingFlags.Instance |
                                                                               BindingFlags.GetProperty |
                                                                               BindingFlags.Public |
                                                                               BindingFlags.IgnoreCase))
                .ToArray();
        }

        public bool Equals(T x, T y) => _properties.All(p => p.GetValue(x).Equals(p.GetValue(y)));

        public int GetHashCode(T obj) => _properties
            .Select(property => property.GetValue(obj))
            .Aggregate(-892461325, (current, val) => current * -1521134295 + val.GetHashCode());
    }
}
