﻿using System;
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
        public Fake<DbSet<Test>> TestSet { get; set; }

        private static void GenerateTestConfiguration(IMapperConfigurationExpression config)
        {
            config.AddProfile<DtoModelProfile>();
        }

        public void PrepareServiceConfiguration()
        {
            DbContext = Fixture.Freeze<Fake<DbContext>>();
            Rng = Fixture.Freeze<Fake<Random>>();
            SurveySet = Fixture.Freeze<Fake<DbSet<Survey>>>();
            TestSet = Fixture.Freeze<Fake<DbSet<Test>>>();

            Fixture.Inject(DbContext.FakedObject);
            Fixture.Inject(Rng.FakedObject);
        }

        public IList<Survey> Surveys { get; set; } = new FixtureData<Survey>("TestData/surveys.json").ToList();
        public IList<Test> Tests => Surveys.SelectMany(s => s.Tests).ToList();

        public void PrepareServiceHelperCalls()
        {
            var surveysQueryable = Surveys.AsQueryable();
            A.CallTo(() => ((IQueryable<Survey>)SurveySet.FakedObject).GetEnumerator()).Returns(surveysQueryable.GetEnumerator());
            A.CallTo(() => ((IQueryable<Survey>)SurveySet.FakedObject).Provider).Returns(surveysQueryable.Provider);
            A.CallTo(() => ((IQueryable<Survey>)SurveySet.FakedObject).Expression).Returns(surveysQueryable.Expression);
            A.CallTo(() => ((IQueryable<Survey>)SurveySet.FakedObject).ElementType).Returns(surveysQueryable.ElementType);
            A.CallTo(() => SurveySet.FakedObject.Include(A<string>._)).Returns(SurveySet.FakedObject);

            A.CallTo(() => DbContext.FakedObject.Set<Survey>()).Returns(SurveySet.FakedObject);

            var testsQueryable = Tests.AsQueryable();
            A.CallTo(() => ((IQueryable<Test>)TestSet.FakedObject).GetEnumerator()).Returns(testsQueryable.GetEnumerator());
            A.CallTo(() => ((IQueryable<Test>)TestSet.FakedObject).Provider).Returns(testsQueryable.Provider);
            A.CallTo(() => ((IQueryable<Test>)TestSet.FakedObject).Expression).Returns(testsQueryable.Expression);
            A.CallTo(() => ((IQueryable<Test>)TestSet.FakedObject).ElementType).Returns(testsQueryable.ElementType);

            A.CallTo(() => DbContext.FakedObject.Set<Test>()).Returns(TestSet.FakedObject);
        }
    }
}
