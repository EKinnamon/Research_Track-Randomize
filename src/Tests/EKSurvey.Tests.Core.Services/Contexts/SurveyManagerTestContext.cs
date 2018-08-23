using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AutoFixture;

using AutoMapper;

using EKSurvey.Core.Models.Entities;
using EKSurvey.Core.Models.Enums;
using EKSurvey.Core.Models.Profiles;
using EKSurvey.Core.Services;
using EKSurvey.Tests.Extensions;

using FakeItEasy;
using MoreLinq;

namespace EKSurvey.Tests.Core.Services.Contexts
{
    public class SurveyManagerTestContext : ServiceBaseTestContext<SurveyManager>
    {
        private const string SurveyFakeDataPath = "TestData/Surveys.json";
        private const string TestResponsesFakeDataPath = "TestData/TestResponses.json";
        private const string TestSectionMarkersFakeDataPath = "TestData/TestSectionMarkers.json";

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
        public Fake<DbSet<TestSectionMarker>> TestSectionMarkerSet { get; set; }

        private static void GenerateTestConfiguration(IMapperConfigurationExpression config)
        {
            config.AddProfile<DtoModelProfile>();
        }

        public void PrepareServiceConfiguration()
        {
            DbContext = Fixture.Freeze<Fake<DbContext>>();
            Rng = Fixture.Freeze<Fake<Random>>();
            SurveySet = Fixture.Freeze<Fake<DbSet<Survey>>>();
            SectionSet = Fixture.Freeze<Fake<DbSet<Section>>>();
            TestResponseSet = Fixture.Freeze<Fake<DbSet<TestResponse>>>();

            Fixture.Inject(DbContext.FakedObject);
            Fixture.Inject(Rng.FakedObject);
        }

        public void PrepareTestEntities(bool includeCompleted = false)
        {
            if (includeCompleted)
            {
                UserId = UserIds.Shuffle().First();
                SurveyId = Surveys.Shuffle().First().Id;
            }
            else
            {
                var completedTest = Tests.Where(t => !t.Completed.HasValue).Shuffle().First();
                UserId = completedTest.UserId;
                SurveyId = completedTest.SurveyId;
            }
        }

        public IList<Survey> Surveys { get; set; } = (FixtureData<Survey>.Load(SurveyFakeDataPath).Callback(SurveyJoiner) ?? GenerateSurveyFixtureData(4).CacheAs(SurveyFakeDataPath)).ToList();

        public IList<string> UserIds => Tests.Select(t => t.UserId).ToList();
        public IList<Section> Sections => Surveys.SelectMany(s => s.Sections).ToList();
        public IList<Page> Pages => Sections.SelectMany(s => s.Pages).ToList();
        public IList<Test> Tests => Surveys.SelectMany(s => s.Tests).ToList();

        public IList<TestResponse> TestResponses => Tests.SelectMany(t => t.TestResponses).ToList();
        public IList<TestSectionMarker> TestSectionMarkers => Tests.SelectMany(t => t.TestSectionMarkers).ToList();

        public string UserId { get; set; }
        public int SurveyId { get; set; }

        public void PrepareServiceHelperCalls()
        {
            var surveysQueryable = Surveys.AsQueryable();
            SetupDbSetCalls(SurveySet.FakedObject, surveysQueryable);
            A.CallTo(() => SurveySet.FakedObject.Include(A<string>._)).Returns(SurveySet.FakedObject);
            A.CallTo(() => DbContext.FakedObject.Set<Survey>()).Returns(SurveySet.FakedObject);

            var sectionsQueryable = Sections.AsQueryable();
            SetupDbSetCalls(SectionSet.FakedObject, sectionsQueryable);
            A.CallTo(() => DbContext.FakedObject.Set<Section>()).Returns(SectionSet.FakedObject);

            var testResponseQueryable = TestResponses.AsQueryable();
            SetupDbSetCalls(TestResponseSet.FakedObject, testResponseQueryable);
            A.CallTo(() => DbContext.FakedObject.Set<TestResponse>()).Returns(TestResponseSet.FakedObject);
        }

        private static void SetupDbSetCalls<T>(IQueryable<T> fake, IQueryable<T> fakeData) where T : class
        {
            A.CallTo(() => fake.GetEnumerator()).Returns(fakeData.GetEnumerator());
            A.CallTo(() => fake.Provider).Returns(fakeData.Provider);
            A.CallTo(() => fake.Expression).Returns(fakeData.Expression);
            A.CallTo(() => fake.ElementType).Returns(fakeData.ElementType);
        }

        private static IEnumerable<Survey> GenerateSurveyFixtureData(int count = 20)
        {
            var pageTypes = new List<Func<Page>>
            {
                () => Fixture
                    .Build<FreeTextQuestion>()
                    .Without(p => p.Section)
                    .Without(p => p.TestResponses)
                    .Create(),
                () => Fixture
                    .Build<RangeQuestion>()
                    .Without(p => p.Section)
                    .Without(p => p.TestResponses)
                    .Create(),
                () => Fixture
                    .Build<StaticTextPage>()
                    .Without(p => p.Section)
                    .Without(p => p.TestResponses)
                    .Create(),
                () => Fixture
                    .Build<TrueFalseQuestion>()
                    .Without(p => p.Section)
                    .Without(p => p.TestResponses)
                    .Create()
            };

            IEnumerable<Page> PageBuilder(Section section) => Enumerable
                .Range(0, Fixture.Create<int>() % 6 + 1)
                .Select(pi =>
                {
                    var page = pageTypes[Fixture.Create<int>() % pageTypes.Count].Invoke();
                    page.Section = section;
                    page.SectionId = section.Id;
                    page.Order = pi;
                    return page;
                });

            IEnumerable<Section> SectionBuilder(Survey survey) => Enumerable
                .Range(0, Fixture.Create<int>() % 5 + 2)
                .Select(i =>
                {
                    var section = Fixture.Build<Section>()
                        .With(s => s.SurveyId, survey.Id)
                        .With(s => s.SelectorType, SelectorType.Random)
                        .With(s => s.Order, i)
                        .Without(s => s.Survey)
                        .Without(s => s.Pages)
                        .Without(s => s.TestSectionMarkers)
                        .Create();

                    ((HashSet<Page>)section.Pages).UnionWith(PageBuilder(section));

                    return section;
                });

            IEnumerable<TestSectionMarker> TestSectionMarkerBuilder(Test test)
            {
                IEnumerable<Section> sections;
                int? sectionId = null;
                if (test.Completed.HasValue)
                {
                    sections = test.Survey.Sections;
                }
                else
                {
                    var sectionCount = test.Survey.Sections.Count - 1;

                    sections = test.Survey
                        .Sections
                        .OrderBy(s => s.Order)
                        .Take(Fixture.Create<int>() % sectionCount + 1)
                        .ToList();

                    sectionId = sections.Last().Id;
                }

                foreach (var section in sections)
                {
                    yield return Fixture
                        .Build<TestSectionMarker>()
                        .With(t => t.SectionId, section.Id)
                        .With(t => t.TestId, test.Id)
                        .With(t => t.Completed, section.Id == sectionId ? (DateTime?) null : Fixture.Create<DateTime>())
                        .Without(t => t.Test)
                        .Without(t => t.Section)
                        .Create();
                }
            }

            IEnumerable<TestResponse> TestResponseBuilder(Test test)
            {
                IEnumerable<Page> pages;
                int? pageId = null;


                if (test.Completed.HasValue)
                {
                    pages = test.Survey.Sections.SelectMany(s => s.Pages);
                }
                else
                {
                    var sectionId = test.TestSectionMarkers
                        .First(tsm => !tsm.Completed.HasValue)
                        .SectionId;

                    var sections = test.Survey
                        .Sections
                        .OrderBy(s => s.Order)
                        .TakeUntil(s => s.Id != sectionId);

                    var section = test.Survey.Sections.Single(s => s.Id == sectionId);

                    pageId = section.Pages.Shuffle()
                        .First()
                        .Id;

                    pages = sections.OrderBy(s => s.Order)
                        .SelectMany(s => s.Pages)
                        .OrderBy(p => p.Order)
                        .TakeWhile(p => p.Id != pageId);
                }

                foreach (var page in pages)
                {
                    var response = Fixture
                        .Build<TestResponse>()
                        .With(r => r.PageId, page.Id)
                        .With(r => r.Modified, Fixture.Create<bool>() ? Fixture.Create<DateTime>() : (DateTime?) null)
                        .With(r => r.Responded, pageId == page.Id ? Fixture.Create<DateTime>() : (DateTime?) null)
                        .Without(r => r.Test)
                        .Without(r => r.Page)
                        .Create();

                    yield return response;
                }

            }

            IEnumerable<Test> TestBuilder(Survey survey) => Enumerable
                .Range(0, Fixture.Create<int>() % 20 + 1)
                .Select(i =>
                {
                    var test = Fixture.Build<Test>()
                        .With(t => t.Survey, survey)
                        .With(t => t.SurveyId, survey.Id)
                        .With(t => t.Completed, i % 4 == 0 ? Fixture.Create<DateTime>() : (DateTime?) null)
                        .With(t => t.Modified, i %10 == 0 ? Fixture.Create<DateTime>() : (DateTime?) null)
                        .Without(t => t.TestResponses)
                        .Without(t => t.TestSectionMarkers)
                        .Create();

                    ((HashSet<TestSectionMarker>)test.TestSectionMarkers).UnionWith(TestSectionMarkerBuilder(test));
                    ((HashSet<TestResponse>)test.TestResponses).UnionWith(TestResponseBuilder(test));

                    return test;
                });

            var surveys = Enumerable.Range(1, count).Select(i =>
            {
                var survey = Fixture
                    .Build<Survey>()
                    .With(s => s.Id, i)
                    .With(s => s.Deleted, i % 4 == 0 ? Fixture.Create<DateTime>() : (DateTime?) null)
                    .With(s => s.Modified, i % 2 == 0 ? Fixture.Create<DateTime>() : (DateTime?) null)
                    .Without(s => s.Sections)
                    .Without(s => s.Tests)
                    .Create();

                ((HashSet<Section>)survey.Sections).UnionWith(SectionBuilder(survey));
                ((HashSet<Test>)survey.Tests).UnionWith(TestBuilder(survey));

                //survey.Sections = new HashSet<Section>(survey.Sections.Select(s =>
                //{
                //    s.TestSectionMarkers =
                //        new HashSet<TestSectionMarker>
                //        (from t in survey.Tests
                //            from tsm in t.TestSectionMarkers
                //            where tsm.SectionId == s.Id
                //            select tsm);


                //    s.Pages = new HashSet<Page>(s.Pages.Select(p =>
                //    {
                //        p.TestResponses =
                //            new HashSet<TestResponse>
                //            (from t in survey.Tests
                //                from tr in t.TestResponses
                //                where tr.PageId == p.Id
                //                select tr);

                //        return p; 
                //    }));

                //    return s;
                //}));

                return survey;
            }).ToList();

            return surveys;
        }

        private static void SurveyJoiner(IEnumerable<Survey> surveys)
        {
            foreach (var survey in surveys)
            {
                survey.Sections = new HashSet<Section>(survey.Sections.Select(s =>
                {
                    s.TestSectionMarkers =
                        new HashSet<TestSectionMarker>
                        (from t in survey.Tests
                         from tsm in t.TestSectionMarkers
                         where tsm.SectionId == s.Id
                         select tsm);


                    s.Pages = new HashSet<Page>(s.Pages.Select(p =>
                    {
                        p.TestResponses =
                            new HashSet<TestResponse>
                            (from t in survey.Tests
                             from tr in t.TestResponses
                             where tr.PageId == p.Id
                             select tr);

                        return p;
                    }));

                    return s;
                }));

            }
        }
    }
}
