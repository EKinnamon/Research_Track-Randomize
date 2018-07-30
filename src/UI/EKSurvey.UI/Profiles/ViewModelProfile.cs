using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EKSurvey.Core.Models.DataTransfer;
using EKSurvey.Core.Models.Identity;
using EKSurvey.Core.Models.ViewModels.Account;
using EKSurvey.Core.Models.ViewModels.Survey;
using EKSurvey.Core.Models.ViewModels.Test;

namespace EKSurvey.UI.Profiles
{
    public class ViewModelProfile : Profile
    {
        public ViewModelProfile()
        {
            CreateMap<RegisterViewModel, ApplicationUser>()
                // Email, SecondaryEmail, PhoneNumber mapped.
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

            CreateMap<ICollection<UserSurvey>, IndexViewModel>()
                .ForMember(dest => dest.AvailableSurveys, opt => opt.MapFrom(src => new HashSet<UserSurvey>(src.Where(i => !i.Completed.HasValue).OrderBy(i => i.Modified.GetValueOrDefault(i.Created)).Skip(1))))
                .ForMember(dest => dest.CompletedSurveys, opt => opt.MapFrom(src => new HashSet<UserSurvey>(src.Where(i => i.Completed.HasValue))))
                .ForMember(dest => dest.NextSurvey, opt => opt.MapFrom(src => src.OrderBy(i => i.Started.GetValueOrDefault(DateTime.UtcNow)).ThenBy(i => i.Modified.GetValueOrDefault(i.Created)).FirstOrDefault()));

            CreateMap<UserPage, ResponseViewModel>()
                // UserId, SurveyId, Page mapped.
                .ForMember(dest => dest.PriorPageId, opt => opt.Ignore())
                .ForMember(dest => dest.TestId, opt => opt.Ignore())
                .ForMember(dest => dest.PageId, opt => opt.MapFrom(src => src.Page.Id));
        }
    }
}