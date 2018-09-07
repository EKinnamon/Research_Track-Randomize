using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;

using AutoFixture;

using AutoMapper;

using EKSurvey.Core.Models.DataTransfer;
using EKSurvey.Core.Models.Entities;
using EKSurvey.Core.Models.Enums;
using EKSurvey.Core.Models.Profiles;
using EKSurvey.Core.Services;

using FakeItEasy;

using MoreLinq;

namespace EKSurvey.Tests.Core.Services.Contexts
{
    public class SurveyManagerTestContext : ServiceBaseTestContext<SurveyManager>
    {
        private const string SurveyFakeDataPath = "TestData/Surveys.json";

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
        public Fake<DbSet<Page>> PageSet { get; set; }
        public Fake<DbSet<Test>> TestSet { get; set; }
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
            PageSet = Fixture.Freeze<Fake<DbSet<Page>>>();
            TestSet = Fixture.Freeze<Fake<DbSet<Test>>>();
            TestResponseSet = Fixture.Freeze<Fake<DbSet<TestResponse>>>();
            TestSectionMarkerSet = Fixture.Freeze<Fake<DbSet<TestSectionMarker>>>();

            Fixture.Inject(DbContext.FakedObject);
            Fixture.Inject(Rng.FakedObject);
        }

        public void PrepareTestEntities(bool includeCompleted = false)
        {
            UserId = Guid.NewGuid().ToString();

            var sectionId = 0;
            var pageId = 0;

            IEnumerable<Page> PageBuilder(Section section)
            {
                var pageTypes = new List<Func<Page>>
                    {
                        () => Fixture
                            .Build<FreeTextQuestion>()
                            .With(q => q.Id, ++pageId)
                            .Without(q => q.Section)
                            .Without(q => q.TestResponses)
                            .Create(),
                        () => Fixture
                            .Build<RangeQuestion>()
                            .With(q => q.Id, ++pageId)
                            .Without(q => q.Section)
                            .Without(q => q.TestResponses)
                            .Create(),
                        () => Fixture
                            .Build<StaticTextPage>()
                            .With(q => q.Id, ++pageId)
                            .Without(q => q.Section)
                            .Without(q => q.TestResponses)
                            .Create(),
                        () => Fixture
                            .Build<TrueFalseQuestion>()
                            .With(q => q.Id, ++pageId)
                            .Without(q => q.Section)
                            .Without(q => q.TestResponses)
                            .Create()
                    };

                var order = 0;

                while (true)
                {
                    var page = pageTypes[Fixture.Create<int>() % pageTypes.Count].Invoke();
                    page.SectionId = section.Id;
                    page.Order = ++order;
                    yield return page;
                }
            }

            IEnumerable<Section> SectionBuilder(Survey survey)
            {
                var i = 0;
                while (true)
                {
                    var makeCluster = Fixture.Create<int>() % 5 == 0;

                    if (!makeCluster)
                    {
                        var section = Fixture.Build<Section>()
                            .With(s => s.Id, ++sectionId)
                            .With(s => s.Survey, survey)
                            .With(s => s.SurveyId, survey.Id)
                            .With(s => s.SelectorType, SelectorType.Random)
                            .With(s => s.Order, i++)
                            .Without(s => s.Pages)
                            .Without(s => s.TestSectionMarkers)
                            .Create();

                        section.Pages = new HashSet<Page>(PageBuilder(section).Take(Fixture.Create<int>() % 15 + 2));

                        yield return section;
                    }
                    else
                    {
                        var order = i;
                        var cluster = Enumerable.Range(1, Fixture.Create<int>() % 5 + 1)
                            .Select(j =>
                            {
                                var section = Fixture.Build<Section>()
                                    .With(s => s.Id, ++sectionId)
                                    .With(s => s.Survey, survey)
                                    .With(s => s.SurveyId, survey.Id)
                                    .With(s => s.SelectorType, SelectorType.Random)
                                    .With(s => s.Order, order)
                                    .Without(s => s.Pages)
                                    .Without(s => s.TestSectionMarkers)
                                    .Create();

                                section.Pages = new HashSet<Page>(PageBuilder(section).Take(Fixture.Create<int>() % 15 + 2));

                                return section;
                            });

                        foreach (var section in cluster)
                            yield return section;
                    }
                }
            }

            IEnumerable<TestSectionMarker> TestSectionMarkerBuilder(Test test)
            {
                var sectionGroupCount = test.Survey.Sections.Select(s => s.Order).Distinct().Count();
                var sectionCount = Fixture.Create<int>() % sectionGroupCount + 1;
                var sections = test.Completed.HasValue
                    ? test.Survey.Sections.GroupBy(s => s.Order)
                    : test.Survey.Sections.GroupBy(s => s.Order).Take(sectionCount);

                var lastKey = sections.Last().Key;

                foreach (var sectionGroup in sections.OrderBy(s => s.Key))
                {
                    var finishSection = Fixture.Create<bool>();
                    var section = sectionGroup.Count() == 1
                        ? sectionGroup.Single()
                        : sectionGroup.Shuffle().First();

                    var userSection = Fixture
                        .Build<TestSectionMarker>()
                        .With(t => t.TestId, test.Id)
                        .With(t => t.Test, test)
                        .With(t => t.SectionId, section.Id)
                        .With(t => t.Section, section)
                        .With(t => t.Completed, sectionGroup.Key == lastKey && finishSection ? Fixture.Create<DateTime>() : (DateTime?) null)
                        .Create();

                    yield return userSection;
                }
            }

            IEnumerable<TestResponse> TestResponseBuilder(Test test)
            {
                IEnumerable<Page> pages;
                int? selectedPageId = null;

                if (test.Completed.HasValue)
                {
                    pages = test.Survey.Sections.SelectMany(s => s.Pages);
                }
                else
                {
                    var sections = test.Survey.Sections.GroupBy(s => s.Order);
                    var unansweredSections = sections.TakeUntil(sg => sg.Any(i => i.TestSectionMarkers.Any(tsm => tsm.Completed.HasValue)));

                    //var sections = test
                    //    .Survey
                    //    .Sections
                    //    .OrderBy(s => s.Order)
                    //    .TakeUntil(s => s.Id != sectionId);

                    //var section = test.Survey.Sections.Single(s => s.Id == sectionId);

                    //selectedPageId = section
                    //    .Pages
                    //    .Shuffle()
                    //    .First()
                    //    .Id;

                    //pages = sections.OrderBy(s => s.Order)
                    //    .SelectMany(s => s.Pages)
                    //    .OrderBy(p => p.Order)
                    //    .TakeWhile(p => p.Id != selectedPageId);
                }

                foreach (var page in pages)
                {
                    var response = Fixture
                        .Build<TestResponse>()
                        .With(r => r.Page, page)
                        .With(r => r.PageId, page.Id)
                        .With(r => r.Test, test)
                        .With(r => r.TestId, test.Id)
                        .With(r => r.Modified)
                        .With(r => r.Modified, Fixture.Create<bool>() ? Fixture.Create<DateTime>() : (DateTime?) null)
                        .With(r => r.Responded, selectedPageId == page.Id ? Fixture.Create<DateTime>() : (DateTime?) null)
                        .Create();

                    yield return response;
                }
            }

            IEnumerable<Test> TestBuilder(Survey survey)
            {
                var i = survey.Id ^ Fixture.Create<int>();
                if (i % 2 == 0)
                    yield break;

                var test = Fixture.Build<Test>()
                    .With(t => t.UserId, UserId)
                    .With(t => t.SurveyId, survey.Id)
                    .With(t => t.Modified, i % 10 == 0 ? Fixture.Create<DateTime>() : (DateTime?)null)
                    .With(t => t.Completed, i % 4 == 0 ? Fixture.Create<DateTime>() : (DateTime?)null)
                    .With(t => t.Survey, survey)
                    .Without(t => t.TestResponses)
                    .Without(t => t.TestSectionMarkers)
                    .Create();

                test.TestSectionMarkers = new HashSet<TestSectionMarker>(TestSectionMarkerBuilder(test));
                survey.Sections = new HashSet<Section>(survey.Sections.Select(s =>
                    {
                        s.TestSectionMarkers =
                            new HashSet<TestSectionMarker>(test.TestSectionMarkers.Where(tsm => tsm.SectionId == s.Id));
                        return s;
                    }));

                test.TestResponses = new HashSet<TestResponse>(TestResponseBuilder(test));

                yield return test;
            }

            Surveys = Enumerable.Range(1, 20).Select(i =>
            {
                var survey = Fixture
                    .Build<Survey>()
                    .With(s => s.Id, i)
                    .With(s => s.Deleted, i % 4 == 0 ? Fixture.Create<DateTime>() : (DateTime?) null)
                    .With(s => s.Modified, i % 2 == 0 ? Fixture.Create<DateTime>() : (DateTime?) null)
                    .Without(s => s.Sections)
                    .Without(s => s.Tests)
                    .Create();

                survey.Sections = new HashSet<Section>(SectionBuilder(survey).Take(Fixture.Create<int>() % 15 + 2));

                survey.Tests = new HashSet<Test>(TestBuilder(survey));

                return survey;
            }).ToList();

            Survey = Surveys
                .Where(s => s.IsActive && !s.Deleted.HasValue)
                .Shuffle()
                .First();

            UserSurveys = Surveys
                .Where(s => s.IsActive && !s.Deleted.HasValue)
                .Select(s =>
                {
                    var test = includeCompleted
                        ? s.Tests.SingleOrDefault(t => t.UserId.Equals(UserId, StringComparison.CurrentCulture))
                        : s.Tests.SingleOrDefault(t => t.UserId.Equals(UserId, StringComparison.CurrentCulture) && !t.Completed.HasValue);

                    var userSurvey = new UserSurvey
                    {
                        UserId = UserId,
                        Id = s.Id,
                        Name = s.Name,
                        Description = s.Description,
                        Version = s.Version,
                        IsActive = s.IsActive,
                        Created = s.Created,
                        Modified = s.Modified,
                        Started = test?.Started,
                        Completed = test?.Completed
                    };

                    return userSurvey;
                }).ToList();

            UserSections = Survey
                .Sections
                .GroupBy(s => s.Order)
                .Select(s =>
                {
                    IUserSection section;
                    if (s.Count() == 1)
                    {
                        var test = s.Single().Survey.Tests.SingleOrDefault(t =>
                            t.UserId.Equals(UserId, StringComparison.OrdinalIgnoreCase));
                        section = new UserSection();
                    }
                    else
                    {
                        section = new UserSectionGroup();
                    }

                    return section;
                }).ToList();
        }

        public IList<Survey> Surveys { get; set; }
        public IList<Section> Sections => Surveys.SelectMany(s => s.Sections).ToList();
        public IList<Page> Pages => Sections.SelectMany(s => s.Pages).ToList();
        public IList<Test> Tests => Surveys.SelectMany(s => s.Tests).ToList();
        public IList<TestSectionMarker> TestSectionMarkers => Tests.SelectMany(t => t.TestSectionMarkers).ToList();
        //public IList<TestResponse> TestResponses => Tests.SelectMany(t => t.TestResponses).ToList();
        //public IList<string> UserIds => Tests.Select(t => t.UserId).ToList();


        public IList<UserSurvey> UserSurveys { get; set; }
        public IList<IUserSection> UserSections { get; set; }

        public string UserId { get; set; }
        public Survey Survey { get; set; }
        public Section Section { get; set; }
        public Page Page { get; set; }
        public Test Test { get; set; }

        public void PrepareServiceHelperCalls()
        {
            SurveySet.FakedObject.SetupData(Surveys, i => Surveys.SingleOrDefault(s => i.OfType<int>().Any(si => s.Id == si)));
            SectionSet.FakedObject.SetupData(Sections, i => Sections.SingleOrDefault(s => i.OfType<int>().Any(si => s.Id == si)));
            PageSet.FakedObject.SetupData(Pages, i => Pages.SingleOrDefault(p => i.OfType<int>().Any(pi => p.Id == pi)));
            TestSet.FakedObject.SetupData(Tests, i => Tests.SingleOrDefault(t => i.OfType<Guid>().Any(ti => t.Id == ti)));
            TestSectionMarkerSet.FakedObject.SetupData(TestSectionMarkers, i => TestSectionMarkers.SingleOrDefault(t => i.OfType<Guid>().Any(tsi => t.Id == tsi)));
            //TestResponseSet.FakedObject.SetupData(TestResponses, i => TestResponses.SingleOrDefault(t => i.OfType<Guid>().Any(tri => t.Id == tri)));

            A.CallTo(() => DbContext.FakedObject.Set<Survey>()).Returns(SurveySet.FakedObject);
            A.CallTo(() => DbContext.FakedObject.Set<Section>()).Returns(SectionSet.FakedObject);
            A.CallTo(() => DbContext.FakedObject.Set<Page>()).Returns(PageSet.FakedObject);
            A.CallTo(() => DbContext.FakedObject.Set<Test>()).Returns(TestSet.FakedObject);
            A.CallTo(() => DbContext.FakedObject.Set<TestSectionMarker>()).Returns(TestSectionMarkerSet.FakedObject);
            A.CallTo(() => DbContext.FakedObject.Set<TestResponse>()).Returns(TestResponseSet.FakedObject);
        }

        //private static IEnumerable<Survey> GenerateSurveyFixtureData(int count = 20)
        //{
        //    var sectionIdBuilder = 0;
        //    var pageIdBuilder = 0;
        //    var pageTypes = new List<Func<Page>>
        //    {
        //        () => Fixture
        //            .Build<FreeTextQuestion>()
        //            .With(q => q.Id, ++pageIdBuilder)
        //            .Without(q => q.Section)
        //            .Without(q => q.TestResponses)
        //            .Create(),
        //        () => Fixture
        //            .Build<RangeQuestion>()
        //            .With(q => q.Id, ++pageIdBuilder)
        //            .Without(q => q.Section)
        //            .Without(q => q.TestResponses)
        //            .Create(),
        //        () => Fixture
        //            .Build<StaticTextPage>()
        //            .With(q => q.Id, ++pageIdBuilder)
        //            .Without(q => q.Section)
        //            .Without(q => q.TestResponses)
        //            .Create(),
        //        () => Fixture
        //            .Build<TrueFalseQuestion>()
        //            .With(q => q.Id, ++pageIdBuilder)
        //            .Without(q => q.Section)
        //            .Without(q => q.TestResponses)
        //            .Create()
        //    };

        //    IEnumerable<Page> PageBuilder(Section section) => Enumerable
        //        .Range(0, Fixture.Create<int>() % 6 + 1)
        //        .Select(pi =>
        //        {
        //            var page = pageTypes[Fixture.Create<int>() % pageTypes.Count].Invoke();
        //            page.SectionId = section.Id;
        //            page.Order = pi;
        //            return page;
        //        });

        //    IEnumerable<Section> SectionBuilder(Survey survey) => Enumerable
        //        .Range(0, Fixture.Create<int>() % 5 + 2)
        //        .Select(i =>
        //        {
        //            var section = Fixture.Build<Section>()
        //                .With(s => s.Id, ++sectionIdBuilder)
        //                .With(s => s.SurveyId, survey.Id)
        //                .With(s => s.SelectorType, SelectorType.Random)
        //                .With(s => s.Order, i)
        //                .Without(s => s.Survey)
        //                .Without(s => s.Pages)
        //                .Without(s => s.TestSectionMarkers)
        //                .Create();

        //            ((HashSet<Page>)section.Pages).UnionWith(PageBuilder(section));

        //            return section;
        //        });

        //    IEnumerable<TestSectionMarker> TestSectionMarkerBuilder(Test test)
        //    {
        //        IEnumerable<Section> sections;
        //        int? sectionId = null;
        //        if (test.Completed.HasValue)
        //        {
        //            sections = test.Survey.Sections;
        //        }
        //        else
        //        {
        //            var sectionCount = test.Survey.Sections.Count - 2;

        //            sections = test.Survey
        //                .Sections
        //                .OrderBy(s => s.Order)
        //                .Take(Fixture.Create<int>() % sectionCount + 1)
        //                .ToList();

        //            sectionId = sections.Last().Id;
        //        }

        //        foreach (var section in sections)
        //        {
        //            yield return Fixture
        //                .Build<TestSectionMarker>()
        //                .With(t => t.SectionId, section.Id)
        //                .With(t => t.TestId, test.Id)
        //                .With(t => t.Completed, section.Id == sectionId ? (DateTime?) null : Fixture.Create<DateTime>())
        //                .Without(t => t.Section)
        //                .Without(t => t.Test)
        //                .Create();
        //        }
        //    }

        //    IEnumerable<TestResponse> TestResponseBuilder(Test test)
        //    {
        //        IEnumerable<Page> pages;
        //        int? pageId = null;


        //        if (test.Completed.HasValue)
        //        {
        //            pages = test.Survey.Sections.SelectMany(s => s.Pages);
        //        }
        //        else
        //        {
        //            var sectionId = test.TestSectionMarkers
        //                .First(tsm => !tsm.Completed.HasValue)
        //                .SectionId;

        //            var sections = test.Survey
        //                .Sections
        //                .OrderBy(s => s.Order)
        //                .TakeUntil(s => s.Id != sectionId);

        //            var section = test.Survey.Sections.Single(s => s.Id == sectionId);

        //            pageId = section.Pages.Shuffle()
        //                .First()
        //                .Id;

        //            pages = sections.OrderBy(s => s.Order)
        //                .SelectMany(s => s.Pages)
        //                .OrderBy(p => p.Order)
        //                .TakeWhile(p => p.Id != pageId);
        //        }

        //        foreach (var page in pages)
        //        {
        //            var response = Fixture
        //                .Build<TestResponse>()
        //                .With(r => r.PageId, page.Id)
        //                .With(r => r.TestId, test.Id)
        //                .With(r => r.Modified, Fixture.Create<bool>() ? Fixture.Create<DateTime>() : (DateTime?) null)
        //                .With(r => r.Responded, pageId == page.Id ? Fixture.Create<DateTime>() : (DateTime?) null)
        //                .Without(r => r.Test)
        //                .Without(r => r.Page)
        //                .Create();

        //            yield return response;
        //        }

        //    }

        //    IEnumerable<Test> TestBuilder(Survey survey) => Enumerable
        //        .Range(0, Fixture.Create<int>() % 20 + 1)
        //        .Select(i =>
        //        {
        //            var test = Fixture.Build<Test>()
        //                .With(t => t.Survey, survey)
        //                .With(t => t.SurveyId, survey.Id)
        //                .With(t => t.Completed, i % 4 == 0 ? Fixture.Create<DateTime>() : (DateTime?) null)
        //                .With(t => t.Modified, i %10 == 0 ? Fixture.Create<DateTime>() : (DateTime?) null)
        //                .Without(t => t.TestResponses)
        //                .Without(t => t.TestSectionMarkers)
        //                .Create();

        //            ((HashSet<TestSectionMarker>)test.TestSectionMarkers).UnionWith(TestSectionMarkerBuilder(test));
        //            ((HashSet<TestResponse>)test.TestResponses).UnionWith(TestResponseBuilder(test));

        //            return test;
        //        });

        //    var surveys = Enumerable.Range(1, count).Select(i =>
        //    {
        //        var survey = Fixture
        //            .Build<Survey>()
        //            .With(s => s.Id, i)
        //            .With(s => s.Deleted, i % 4 == 0 ? Fixture.Create<DateTime>() : (DateTime?) null)
        //            .With(s => s.Modified, i % 2 == 0 ? Fixture.Create<DateTime>() : (DateTime?) null)
        //            .Without(s => s.Sections)
        //            .Without(s => s.Tests)
        //            .Create();

        //        ((HashSet<Section>)survey.Sections).UnionWith(SectionBuilder(survey));
        //        ((HashSet<Test>)survey.Tests).UnionWith(TestBuilder(survey));

        //        return survey;
        //    }).ToList();

        //    return surveys;
        //}

        private static void SurveyJoiner(IEnumerable<Survey> surveys)
        {
            foreach (var survey in surveys)
            {
                survey.Sections =
                    new HashSet<Section>(survey.Sections.Select(s =>
                    {
                        s.Survey = survey;
                        s.Pages = 
                            new HashSet<Page>(s.Pages.Select(p =>
                            {
                                p.Section = s;
                                p.TestResponses =
                                    new HashSet<TestResponse>(
                                        from t in survey.Tests
                                        from tr in t.TestResponses
                                        where tr.PageId == p.Id
                                        select tr);

                                return p;
                            }));

                        s.TestSectionMarkers =
                            new HashSet<TestSectionMarker>(
                                from t in survey.Tests
                                from tsm in t.TestSectionMarkers
                                where tsm.SectionId == s.Id
                                select tsm);

                        return s;
                    }));

                survey.Tests =
                    new HashSet<Test>(survey.Tests.Select(t =>
                    {
                        t.Survey = survey;
                        t.TestSectionMarkers =
                            new HashSet<TestSectionMarker>(t.TestSectionMarkers.Select(tsm =>
                                {
                                    tsm.Test = t;
                                    tsm.Section = survey.Sections.Single(s => s.Id == tsm.SectionId);

                                    return tsm;
                                }));

                        t.TestResponses =
                            new HashSet<TestResponse>(t.TestResponses.Select(tr =>
                            {
                                tr.Test = t;
                                tr.Page = survey.Sections.SelectMany(s => s.Pages).Single(p => p.Id == tr.PageId);
                                return tr;
                            }));

                        return t; 
                    }));

            }
        }

    }
}
