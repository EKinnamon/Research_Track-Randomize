using System;
using System.Data.Entity;
using System.Linq;
using AutoMapper;
using EKSurvey.Core.Models.DataTransfer;
using EKSurvey.Core.Models.Entities;
using Page = EKSurvey.Core.Models.Entities.Page;

namespace EKSurvey.UI.Profiles
{
    public class DtoModelProfile : Profile
    {
        public DtoModelProfile()
        {
            CreateMap<Survey, UserSurvey>()
                // Id, Name, Version, IsActive, Created, Modified mapped
                .AfterMap((src, dest, ctx) =>
                {
                    var userId = ctx.Items["userId"].ToString();
                    dest.UserId = userId;

                    var userTest = src.Tests.FirstOrDefault(t => t.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase));

                    dest.Started = userTest?.Started;

                    dest.Completed = userTest?.Completed;
                });

            CreateMap<Section, UserSection>()
                // Id, SurveyId, Name, Order mapped.
                .AfterMap((src, dest, ctx) =>
                {
                    var dbContext = ctx.Items["dbContext"] as DbContext ?? throw new AutoMapperMappingException("DbContext must be supplied for this mapping."); 
                    var userId = ctx.Items["userId"].ToString();
                    dest.UserId = userId;
                    var userTest = dbContext.Set<Test>().Find(userId, src.SurveyId);
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

            CreateMap<IPage, UserPage>()
                .ForMember(dest => dest.Page, opt => opt.MapFrom(src => src))
                .AfterMap((src, dest, ctx) =>
                {
                    var dbContext = ctx.Items["dbContext"] as DbContext ?? throw new AutoMapperMappingException("DbContext must be supplied for this mapping.");
                    var userId = ctx.Items["userId"].ToString();
                    dest.UserId = userId;

                    var page = src as Page ?? throw new AutoMapperMappingException("Invalid page type being mapped.");

                    dest.SurveyId = page.Section.SurveyId;

                    var test = dbContext.Set<Test>().Find(userId, page.Section.SurveyId);
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
                .ForMember(dest => dest.IsHtml, opt => opt.MapFrom(src => src.Page.IsHtml))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Page.Text))
                .AfterMap((src, dest, ctx) =>
                {
                    dest.UserId = ctx.Items["userId"].ToString();
                });
        }
    }
}