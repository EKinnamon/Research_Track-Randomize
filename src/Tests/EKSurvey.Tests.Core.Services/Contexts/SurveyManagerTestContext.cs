using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        //public Fake<DbSet<Test>> TestSet { get; set; }

        private static void GenerateTestConfiguration(IMapperConfigurationExpression config)
        {
            config.AddProfile<DtoModelProfile>();
        }

        public void PrepareServiceConfiguration()
        {
            DbContext = Fixture.Freeze<Fake<DbContext>>();
            Rng = Fixture.Freeze<Fake<Random>>();
            SurveySet = Fixture.Freeze<Fake<DbSet<Survey>>>();
            //TestSet = Fixture.Freeze<Fake<DbSet<Test>>>().FakedObject;
        }

        public IList<Survey> Surveys { get; set; } = new FixtureData<Survey>("TestData/surveys.json").ToList();
        public IList<Test> Tests => Surveys.SelectMany(s => s.Tests).ToList();

        public void PrepareServiceHelperCalls()
        {
            SurveySet.SetupData(Surveys);
            DbContext.CallsTo(ctx => ctx.Set<Survey>()).Returns(SurveySet.FakedObject);
        }
    }
}
