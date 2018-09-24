using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using FakeItEasy;
using FakeItEasy.Creation;

namespace EKSurvey.Tests.FakeOptionsBuilders
{
    // ReSharper disable once UnusedMember.Global 
    public class DbSetSurveyFakeOptionsBuilder : IFakeOptionsBuilder
    {
        public bool CanBuildOptionsForFakeOfType(Type type)
        {
            if (!type.IsGenericType)
                return false;

            var genericType = type.GetGenericTypeDefinition();
            return genericType == typeof(IDbSet<>) || genericType == typeof(DbSet<>);
        }

        public void BuildOptions(Type typeOfFake, IFakeOptions options)
        {
            var iqueryableInterfaceType = typeof(IQueryable<>);
            var idbAsyncEnumerableInterfaceType = typeof(IDbAsyncEnumerable<>);

            var genericClass = typeOfFake.GetGenericArguments().First();

            var iqueryableTypeName = $"{iqueryableInterfaceType.FullName}[[{genericClass.AssemblyQualifiedName}]],{iqueryableInterfaceType.Assembly.FullName}";
            var idbAsyncEnumerableTypeName = $"{idbAsyncEnumerableInterfaceType.FullName}[[{genericClass.AssemblyQualifiedName}]],{idbAsyncEnumerableInterfaceType.Assembly.FullName}";

            var iqueryableType = Type.GetType(iqueryableTypeName, true, true);
            var idbAsyncEnumerableType = Type.GetType(idbAsyncEnumerableTypeName, true, true);

            options.Implements(iqueryableType);
            options.Implements(idbAsyncEnumerableType);
        }

        public Priority Priority => Priority.Default;
    }
}
