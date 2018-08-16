using System;
using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using AutoMapper;
using EKSurvey.Data;
using LazyEntityGraph.AutoFixture;
using LazyEntityGraph.EntityFramework;

namespace EKSurvey.Tests
{
    public abstract class BaseTestContext : IDisposable
    {
        protected static IFixture Fixture { get; set; }
        public IMapper Mapper { get; set; }

        static BaseTestContext()
        {
            Fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var lazyEntityGraphCustomization =
                new LazyEntityGraphCustomization(ModelMetadataGenerator.LoadFromCodeFirstContext(str => new SurveyDbContext(), true));
            Fixture.Customize(lazyEntityGraphCustomization);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Fixture = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
