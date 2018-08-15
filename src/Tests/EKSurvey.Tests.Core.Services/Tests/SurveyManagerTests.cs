using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using EKSurvey.Core.Models.DataTransfer;
using EKSurvey.Core.Models.Entities;
using EKSurvey.Core.Services;
using EKSurvey.Tests.Core.Services.Contexts;

using FluentAssertions;

using Xunit;

using JoinNeeds = EKSurvey.Tests.Core.Services.Contexts.SurveyManagerTestContext.JoinNeeds;

namespace EKSurvey.Tests.Core.Services.Tests
{
    public class SurveyManagerTests
    {
        private readonly SurveyManagerTestContext _context = new SurveyManagerTestContext();

        public SurveyManagerTests()
        {
            _context.Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [Fact]
        public void Constructor_with_valid_arguments_instantiates_service()
        {
            _context.PrepareServiceConfiguration();
            var service = new SurveyManager(_context.DbContext.FakedObject, _context.Rng.FakedObject, _context.Mapper);

            service.Should().NotBeNull();
            service.Should().BeOfType<SurveyManager>();
        }

        [Fact]
        public void Constructor_with_null_dbcontext_throws_exception()
        {
            _context.PrepareServiceConfiguration();

            var exception = Assert.Throws<ArgumentNullException>(() =>
                new SurveyManager(null, 
                    _context.Rng.FakedObject, 
                    _context.Mapper));

            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Constructor_with_null_random_throws_exception()
        {
            _context.PrepareServiceConfiguration();
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new SurveyManager(_context.DbContext.FakedObject, 
                    null, 
                    _context.Mapper));

            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Constructor_with_null_mapper_throws_exception()
        {
            _context.PrepareServiceConfiguration();
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new SurveyManager(_context.DbContext.FakedObject,
                    _context.Rng.FakedObject,
                    null));

            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void GetActiveSurveys_will_return_all_active_surveys()
        {
            _context.PrepareServiceConfiguration();
            _context.PrepareServiceHelperCalls();
            _context.PrepareService();

            var surveys = _context.Service.GetActiveSurveys();

            surveys.Should().NotBeNull();
            surveys.Should().NotContainNulls();
            surveys.Should().BeAssignableTo<IQueryable<Survey>>();
            surveys.Select(i => i.IsActive).Should().AllBeEquivalentTo(true);
            surveys.Select(i => i.Deleted.HasValue).Should().AllBeEquivalentTo(false);
        }

        [Fact]
        public async Task GetActiveSurveysAsync_will_return_all_active_surveys()
        {
            _context.PrepareDataHeirarchy(JoinNeeds.SurveyTests);
            _context.PrepareServiceConfiguration(needsHeirarchy: true);
            _context.PrepareServiceHelperCalls();
            _context.PrepareService();

            var surveys = await _context.Service.GetActiveSurveysAsync();

            surveys.Should().NotBeNull();
            surveys.Should().NotContainNulls();
            surveys.Should().BeAssignableTo<IQueryable<Survey>>();
            surveys.Select(i => i.IsActive).Should().AllBeEquivalentTo(true);
            surveys.Select(i => i.Deleted.HasValue).Should().AllBeEquivalentTo(false);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void GetUserSurveys_will_return_all_surveys_for_user(bool includeCompleted)
        {
            _context.PrepareServiceConfiguration(includeCompleted);
            _context.PrepareServiceHelperCalls();
            _context.PrepareService();

            var userSurveys = _context.Service.GetUserSurveys(_context.UserId, includeCompleted);

            userSurveys.Should().NotBeNull();
            userSurveys.Should().NotContainNulls();
            userSurveys.Should().BeAssignableTo<ICollection<UserSurvey>>();
            userSurveys.Select(i => i.UserId).Distinct().First().Should().BeEquivalentTo(_context.UserId);

            if (!includeCompleted)
            {
                userSurveys.Select(s => s.Completed.HasValue).Should().AllBeEquivalentTo(false);
            }

        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetUserSurveysAsync_will_return_all_surveys_for_user(bool includeCompleted)
        {
            _context.PrepareServiceConfiguration(includeCompleted);
            _context.PrepareServiceHelperCalls();
            _context.PrepareService();

            var userSurveys = await _context.Service.GetUserSurveysAsync(_context.UserId, includeCompleted);

            userSurveys.Should().NotBeNull();
            userSurveys.Should().NotContainNulls();
            userSurveys.Should().BeAssignableTo<ICollection<UserSurvey>>();
            userSurveys.Select(i => i.UserId).Distinct().Single().Should().BeEquivalentTo(_context.UserId);

            if (!includeCompleted)
            {
                userSurveys.Select(s => s.Completed.HasValue).Should().AllBeEquivalentTo(false);
            }

        }

        [Fact]
        public void GetUserSections_will_return_all_sections_for_survey_and_user()
        {
            _context.PrepareServiceConfiguration();
            _context.PrepareServiceHelperCalls();
            _context.PrepareService();

            var userSections = _context.Service.GetUserSections(_context.UserId, _context.SurveyId);
        }
    }
}
