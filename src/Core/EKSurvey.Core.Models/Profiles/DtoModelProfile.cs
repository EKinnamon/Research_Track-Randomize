using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EKSurvey.Core.Models.DataTransfer;
using EKSurvey.Core.Models.Entities;

namespace EKSurvey.Core.Models.Profiles
{
    public class DtoModelProfile : Profile
    {
        public DtoModelProfile()
        {
            CreateMap<Test, UserSurvey>()
                // UserId, Started, Completed mapped
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Survey.Id))
                .ForMember(dest => dest.TestId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Survey.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Survey.Description))
                .ForMember(dest => dest.Version, opt => opt.MapFrom(src => src.Survey.Version))
                .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Survey.Created))
                .ForMember(dest => dest.Modified, opt => opt.MapFrom(src => src.Survey.Modified));

            CreateMap<Survey, UserSurvey>()
                // Id, Name, Version, IsActive, Created, Modified mapped
                .ForMember(dest => dest.TestId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Started, opt => opt.Ignore())
                .ForMember(dest => dest.Completed, opt => opt.Ignore());

            CreateMap<TestSectionMarker, UserSection>()
                // TestId, Started, Completed mapped
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Test.UserId))
                .ForMember(dest => dest.SurveyId, opt => opt.MapFrom(src => src.Test.SurveyId))
                .ForMember(dest => dest.TestSectionMarkerId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.Section.Order))
                .ForMember(dest => dest.Modified, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.SectionId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Section.Name)).AfterMap((src, dest, ctx) =>
                {
                    var testResponses = src.Test.TestResponses;
                    if (testResponses == null || !testResponses.Any())
                        dest.Modified = null;
                    else
                    {
                        dest.Modified = testResponses
                            .Where(tr => tr.Page.SectionId == src.SectionId)
                            .Select(tsm => tsm.Modified.GetValueOrDefault(tsm.Created))
                            .Max();
                    }
                });

            CreateMap<Section, UserSection>()
                // Id, SurveyId, Name, Order mapped.
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.TestId, opt => opt.Ignore())
                .ForMember(dest => dest.TestSectionMarkerId, opt => opt.Ignore())
                .ForMember(dest => dest.Started, opt => opt.Ignore())
                .ForMember(dest => dest.Modified, opt => opt.Ignore())
                .ForMember(dest => dest.Completed, opt => opt.Ignore());

            CreateMap<IEnumerable<Section>, UserSectionGroup>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SelectorType, opt => opt.Ignore())
                .AfterMap((src, dest, ctx) =>
                {
                    var userSections = ctx.Mapper.Map<IEnumerable<UserSection>>(src);
                    dest.SelectorType = src.First().SelectorType;
                    dest.AddRange(userSections);
                });

            CreateMap<TestResponse, UserPage>()
                .ForMember(dest => dest.Page, opt => opt.MapFrom(src => src.Page))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Test.UserId))
                .ForMember(dest => dest.TestResponseId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SurveyId, opt => opt.MapFrom(src => src.Test.SurveyId))
                .ForMember(dest => dest.Response, opt => opt.MapFrom(src => src.Response))
                .ForMember(dest => dest.Responded, opt => opt.MapFrom(src => src.Responded))
                .ForMember(dest => dest.Modified, opt => opt.MapFrom(src => src.Modified));

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
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Test.UserId))
                .ForMember(dest => dest.IsLikert, opt => opt.Ignore())
                .AfterMap((src, dest, ctx) =>
                {
                    if (!(src.Page is RangeQuestion question))
                        return;

                    dest.IsLikert = question.IsLikert;
                    dest.Range = question.Range;
                });
        }
    }
}