using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using AutoFixture;

using AutoMapper;
using EKSurvey.Core.Models.Entities;
using EKSurvey.Core.Models.Profiles;
using EKSurvey.Core.Services;
using FakeItEasy;

namespace EKSurvey.Tests.Core.Services.Contexts
{
    public class SurveyManagerTestContext : ServiceBaseTestContext<SurveyManager>
    {
        public SurveyManagerTestContext()
        {
            Mapper = new MapperConfiguration(GenerateTestConfiguration).CreateMapper();

            Fixture.Register(() => Mapper);
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        public Fake<DbContext> DbContext { get; set; }
        public Fake<Random> Rng { get; set; }
        public Fake<DbSet<Survey>> SurveySet { get; set; }

        private static void GenerateTestConfiguration(IMapperConfigurationExpression config)
        {
            config.AddProfile<DtoModelProfile>();
        }

        public void PrepareServiceConfiguration()
        {
            DbContext = Fixture.Freeze<Fake<DbContext>>();
            Rng = Fixture.Freeze<Fake<Random>>();
            SurveySet = A.Fake<DbSet<Survey>>(o =>
                o.Implements(typeof(IQueryable<Survey>)).Implements(typeof(IDbAsyncEnumerable<Survey>)));


        }

        public IQueryable<Survey> Surveys { get; set; } = new FixtureData<Survey>("TestData/surveys.json").AsQueryable();

        public void PrepareServiceHelperCalls()
        {
            A.CallTo(() => ((IQueryable<Survey>) SurveySet.FakedObject).GetEnumerator()).Returns(Surveys.GetEnumerator());
            A.CallTo(() => ((IQueryable<Survey>) SurveySet.FakedObject).Provider).Returns(Surveys.Provider);
            A.CallTo(() => ((IQueryable<Survey>) SurveySet.FakedObject).Expression).Returns(Surveys.Expression);
            A.CallTo(() => ((IQueryable<Survey>) SurveySet.FakedObject).ElementType).Returns(Surveys.ElementType);

            A.CallTo(() => DbContext.FakedObject.Set<Survey>()).Returns(SurveySet.FakedObject);
        }
    }
}
