using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.Kernel;
using EKSurvey.Core.Models.Entities;
using EKSurvey.Tests.Extensions;

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
            var seededRequest = request as SeededRequest;
            if (seededRequest == null)
                return new NoSpecimen();

            var surveyType = seededRequest.Request as Type;
            if (surveyType == null || surveyType != typeof(Survey))
                return new NoSpecimen();

            var survey = new Survey
            {
                Sections = context.CreateMany<Section>(context.Create<int>() % 9 + 2).ToList(),
                Tests = context.CreateMany<Test>(context.Create<int>() % 1001).ToList()
            };

            var sectionOrder = 0;
            foreach (var section in survey.Sections)
            {
                section.Order = ++sectionOrder;
                section.Pages = GeneratePages(section, context, context.Create<int>() % 20 + 1);
                section.TestSectionMarkers = null;
            }

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
