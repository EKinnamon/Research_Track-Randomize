using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AutoMapper;
using EKSurvey.Core.Models.DataTransfer;
using EKSurvey.Core.Models.Entities;
using Page = EKSurvey.Core.Models.Entities.Page;

namespace EKSurvey.Core.Models.Profiles
{
    public class DtoModelProfile : Profile
    {
        public DtoModelProfile()
        {
            CreateMap<Test, UserSurvey>()
                // UserId, Started, Completed mapped
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Survey.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Survey.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Survey.Description))
                .ForMember(dest => dest.Version, opt => opt.MapFrom(src => src.Survey.Version))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Survey.IsActive))
                .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Survey.Created));

            CreateMap<Survey, UserSurvey>()
                // Id, Name, Version, IsActive, Created, Modified mapped
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Started, opt => opt.Ignore())
                .ForMember(dest => dest.Completed, opt => opt.Ignore());

            CreateMap<Section, UserSection>()
                // Id, SurveyId, Name, Order mapped.
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.TestId, opt => opt.Ignore())
                .ForMember(dest => dest.Started, opt => opt.Ignore())
                .ForMember(dest => dest.Modified, opt => opt.Ignore())
                .ForMember(dest => dest.Completed, opt => opt.Ignore())
                .AfterMap((src, dest, ctx) =>
                {
                    var dbContext = ctx.Items["dbContext"] as DbContext ?? throw new AutoMapperMappingException("DbContext must be supplied for this mapping."); 
                    var userId = ctx.Items["userId"].ToString();
                    dest.UserId = userId;
                    var userTest = dbContext.Set<Test>().SingleOrDefault(t => t.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase) && t.SurveyId == src.SurveyId);
                    var sectionResponses = dbContext.Set<TestResponse>().Where(tr => tr.Page.SectionId == src.Id);

                    if (userTest == null)
                        return;

                    dest.TestId = userTest.Id;

                    dest.Started = src.TestSectionMarkers
                        .SingleOrDefault(tsm => tsm.TestId == userTest.Id && tsm.SectionId == src.Id)?.Started;

                    if (sectionResponses.Any())
                    {
                        dest.Modified = sectionResponses
                            .ToList()
                            .Select(sr => sr.Modified.GetValueOrDefault(sr.Created))
                            .Max();
                    }

                    dest.Completed = src.TestSectionMarkers
                        .SingleOrDefault(tsm => tsm.TestId == userTest.Id && tsm.SectionId == src.Id)?.Completed;
                });

            CreateMap<IEnumerable<Section>, UserSectionGroup>()
                .ForMember(dest => dest.SelectorType, opt => opt.Ignore())
                .AfterMap((src, dest, ctx) =>
                {
                    var dbContext = ctx.Items["dbContext"] as DbContext ?? throw new AutoMapperMappingException("DbContext must be supplied for this mapping.");
                    var userId = ctx.Items["userId"].ToString();

                    var userSections = ctx.Mapper.Map<IEnumerable<UserSection>>(src, opt =>
                    {
                        opt.Items.Add("dbContext", dbContext);
                        opt.Items.Add("userId", userId);
                    });

                    dest.SelectorType = src.First().SelectorType;
                    dest.AddRange(userSections);
                });


            CreateMap<IPage, UserPage>()
                .ForMember(dest => dest.Page, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.SurveyId, opt => opt.Ignore())
                .ForMember(dest => dest.Response, opt => opt.Ignore())
                .ForMember(dest => dest.Responded, opt => opt.Ignore())
                .ForMember(dest => dest.Modified, opt => opt.Ignore())
                .AfterMap((src, dest, ctx) =>
                {
                    var dbContext = ctx.Items["dbContext"] as DbContext ?? throw new AutoMapperMappingException("DbContext must be supplied for this mapping.");
                    var userId = ctx.Items["userId"].ToString();
                    dest.UserId = userId;

                    var page = src as Page ?? throw new AutoMapperMappingException("Invalid page type being mapped.");

                    dest.SurveyId = page.Section.SurveyId;

                    var test = dbContext.Set<Test>().SingleOrDefault(t => t.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase) && t.SurveyId == page.Section.SurveyId);
                    if (test == null)
                        return;

                    var userResponse = page.TestResponses.SingleOrDefault(tr => tr.TestId == test.Id);
                    dest.Response = userResponse?.Response;
                    dest.Responded = userResponse?.Responded;
                    dest.Modified = userResponse?.Modified;
                });

            CreateMap<TestResponse, UserResponse>()
                // PageId, TestId, Response mapped
                .ForMember(dest => dest.SurveyId, opt => opt.MapFrom(src => src.Page.Section.SurveyId))
                .ForMember(dest => dest.SectionId, opt => opt.MapFrom(src => src.Page.SectionId))
                .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.Page.Order))
                .ForMember(dest => dest.IsQuestion, opt => opt.MapFrom(src => src.Page is IQuestion))
                .ForMember(dest => dest.QuestionType, opt => opt.MapFrom(src => src.Page is IQuestion ? src.Page.GetType() : null))
                // ReSharper disable once MergeCastWithTypeCheck -- won't work in a lambda expression in this scenario.
                .ForMember(dest => dest.Range, opt => opt.MapFrom(src => src.Page is RangeQuestion ? ((RangeQuestion) src.Page).Range : (int?) null))
                .ForMember(dest => dest.IsHtml, opt => opt.MapFrom(src => src.Page.IsHtml))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Page.Text))
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.IsLikert, opt => opt.Ignore())
                .AfterMap((src, dest, ctx) =>
                {
                    dest.UserId = ctx.Items["userId"].ToString();

                    if (!(src.Page is RangeQuestion question))
                        return;

                    dest.IsLikert = question.IsLikert;
                    dest.Range = question.Range;
                });
        }
    }
}