using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.Kernel;
using EKSurvey.Core.Models.Entities;
using EKSurvey.Tests.Extensions;
using MoreLinq;

namespace EKSurvey.Tests.SpecimenBuilders
{
    public class SurveySpecimenBuilder : ISpecimenBuilder
    {
        private readonly Dictionary<Type, Func<ISpecimenContext, Page>> _pageDictionary = new Dictionary<Type, Func<ISpecimenContext, Page>>
        {
            {typeof(FreeTextQuestion), ctx => ctx.Create<FreeTextQuestion>() },
            {typeof(RangeQuestion), ctx => ctx.Create<RangeQuestion>() },
            {typeof(StaticTextPage), ctx => ctx.Create<StaticTextPage>() },
            {typeof(TrueFalseQuestion), ctx => ctx.Create<TrueFalseQuestion>() }
        };

        public object Create(object request, ISpecimenContext context)
        {
            if (!(request is SeededRequest seededRequest))
                return new NoSpecimen();

            if(!(seededRequest.Request is Type surveyType) || surveyType != typeof(Survey))
                return new NoSpecimen();

            var survey = new Survey
            {
                Id = context.Create<int>(),
                Name = context.Create<string>(),
                Description = context.Create<string>(),
                Version = context.Create<string>(),
                IsActive = context.Create<bool>(),
                Created = context.Create<DateTime>(),
                Modified = context.Create<int>() % 50 == 0 ? context.Create<DateTime>() : (DateTime?)null,
                Deleted = context.Create<int>() % 100 == 0 ? context.Create<DateTime>() : (DateTime?)null,
                Sections = context.CreateMany<Section>(context.Create<int>() % 9 + 2).ToList(),
                Tests = context.CreateMany<Test>(context.Create<int>() % 1001).ToList()
            };

            //var sectionOrder = 0;
            //foreach (var section in survey.Sections)
            //{
            //    section.Survey = survey;
            //    section.SurveyId = survey.Id;
            //    section.Order = ++sectionOrder;
            //    section.Pages = GeneratePages(section, context, context.Create<int>() % 20 + 1);
            //    section.TestSectionMarkers = survey.Tests.SelectMany(t => t.TestSectionMarkers).Where(tsm => tsm.SectionId == section.Id).ToList();
            //}

            return survey;
        }

        private ICollection<Page> GeneratePages(Section section, ISpecimenContext context, int count)
        {
            var pages = Enumerable.Range(0, count).Select(i =>
            {
                var page = _pageDictionary[_pageDictionary.Keys.Shuffle().First()].Invoke(context);
                page.Section = section;
                page.SectionId = section.Id;
                page.Order = i;
                page.TestResponses = null;
                return page;
            });

            return pages.ToList();
        }
    }
}
