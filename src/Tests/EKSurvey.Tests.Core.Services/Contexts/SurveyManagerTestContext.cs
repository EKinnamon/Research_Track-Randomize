using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using AutoFixture;

using AutoMapper;
using EKSurvey.Core.Models.Entities;
using EKSurvey.Core.Models.Profiles;
using EKSurvey.Core.Services;
using EKSurvey.Data;
using FakeItEasy;
using LazyEntityGraph.AutoFixture;
using LazyEntityGraph.EntityFramework;

namespace EKSurvey.Tests.Core.Services.Contexts
{
    public class SurveyManagerTestContext : ServiceBaseTestContext<SurveyManager>
    {
        public SurveyManagerTestContext()
        {
            Mapper = new MapperConfiguration(GenerateTestConfiguration).CreateMapper();

            Fixture.Register(() => Mapper);
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var lazyEntityGraphCustomization =
                new LazyEntityGraphCustomization(ModelMetadataGenerator.LoadFromCodeFirstContext(str => new SurveyDbContext(), true));
            Fixture.Customize(lazyEntityGraphCustomization);
        }

        public DbContext DbContext { get; set; }
        public Random Rng { get; set; }
        public DbSet<Survey> SurveySet { get; set; }
        public DbSet<Test> TestSet { get; set; }

        private static void GenerateTestConfiguration(IMapperConfigurationExpression config)
        {
            config.AddProfile<DtoModelProfile>();
        }

        public void PrepareServiceConfiguration()
        {
            DbContext = Fixture.Freeze<Fake<DbContext>>().FakedObject;
            Rng = Fixture.Freeze<Fake<Random>>().FakedObject;
            SurveySet = A.Fake<DbSet<Survey>>(o => o.Implements(typeof(IQueryable<Survey>)).Implements(typeof(IDbAsyncEnumerable<Survey>)));
            TestSet = Fixture.Freeze<Fake<DbSet<Test>>>().FakedObject;
        }

        public IList<Survey> Surveys { get; set; } = new FixtureData<Survey>("TestData/surveys.json").ToList();
        public IList<Test> Tests => Surveys.SelectMany(s => s.Tests).ToList();

        public void PrepareServiceHelperCalls()
        {
            SurveySet.SetupData(Surveys);
            A.CallTo(() => DbContext.Set<Survey>()).Returns(SurveySet);
        }
    }
}
