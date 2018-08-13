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
        public Fake<DbSet<Section>> SectionSet { get; set; }
        public Fake<DbSet<TestResponse>> TestResponseSet { get; set; }

        private static void GenerateTestConfiguration(IMapperConfigurationExpression config)
        {
            config.AddProfile<DtoModelProfile>();
        }

        public void PrepareServiceConfiguration(bool includeCompleted = false)
        {
            DbContext = Fixture.Freeze<Fake<DbContext>>();
            Rng = Fixture.Freeze<Fake<Random>>();
            SurveySet = Fixture.Freeze<Fake<DbSet<Survey>>>();
            SectionSet = Fixture.Freeze<Fake<DbSet<Section>>>();
            TestResponseSet = Fixture.Freeze<Fake<DbSet<TestResponse>>>();

            Fixture.Inject(DbContext.FakedObject);
            Fixture.Inject(Rng.FakedObject);

            if (!includeCompleted)
            {
                var completedSurveys = Surveys.Where(s => s.Tests.Any(t => t.Completed.HasValue)).ToList();
                var completedTests = completedSurveys.SelectMany(s => s.Tests).Where(t => t.Completed.HasValue).ToList();
                var userIds = completedTests.Select(t => t.UserId).ToList();
                UserId = UserIds[Fixture.Create<int>() % userIds.Count];
                var surveyIds = completedSurveys.Select(s => s.Id).ToList();
                SurveyId = surveyIds[Fixture.Create<int>() % surveyIds.Count];
            }
            else
            {
                UserId = UserIds[Fixture.Create<int>() % UserIds.Count];
            }
        }

        public IList<Survey> Surveys { get; set; } = new FixtureData<Survey>("TestData/surveys.json").ToList();
        public IList<Test> Tests => Surveys.SelectMany(s => s.Tests).ToList();
        public IList<string> UserIds => Tests.Select(t => t.UserId).ToList();
        public IList<Section> Sections => Surveys.SelectMany(s => s.Sections).ToList();

        public string UserId { get; set; }
        public int SurveyId { get; set; }

        public void PrepareServiceHelperCalls()
        {
            var surveysQueryable = Surveys.AsQueryable();
            A.CallTo(() => ((IQueryable<Survey>)SurveySet.FakedObject).GetEnumerator()).Returns(surveysQueryable.GetEnumerator());
            A.CallTo(() => ((IQueryable<Survey>)SurveySet.FakedObject).Provider).Returns(surveysQueryable.Provider);
            A.CallTo(() => ((IQueryable<Survey>)SurveySet.FakedObject).Expression).Returns(surveysQueryable.Expression);
            A.CallTo(() => ((IQueryable<Survey>)SurveySet.FakedObject).ElementType).Returns(surveysQueryable.ElementType);
            A.CallTo(() => SurveySet.FakedObject.Include(A<string>._)).Returns(SurveySet.FakedObject);

            A.CallTo(() => DbContext.FakedObject.Set<Survey>()).Returns(SurveySet.FakedObject);

            var sectionsQueryable = Sections.AsQueryable();
            A.CallTo(() => ((IQueryable<Section>)SectionSet.FakedObject).GetEnumerator()).Returns(sectionsQueryable.GetEnumerator());
            A.CallTo(() => ((IQueryable<Section>)SectionSet.FakedObject).Provider).Returns(sectionsQueryable.Provider);
            A.CallTo(() => ((IQueryable<Section>)SectionSet.FakedObject).Expression).Returns(sectionsQueryable.Expression);
            A.CallTo(() => ((IQueryable<Section>)SectionSet.FakedObject).ElementType).Returns(sectionsQueryable.ElementType);

            A.CallTo(() => DbContext.FakedObject.Set<Section>()).Returns(SectionSet.FakedObject);
        }
    }
}
