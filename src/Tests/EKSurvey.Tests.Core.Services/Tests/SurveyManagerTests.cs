﻿using System;
using System.Linq;
using EKSurvey.Core.Services;
using EKSurvey.Tests.Core.Services.Contexts;
using FluentAssertions;
using Xunit;

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
            surveys.Select(i => i.IsActive).Should().AllBeEquivalentTo(true);
        }

    }
}
