using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EKSurvey.Core.Models.DataTransfer;
using EKSurvey.Core.Models.Identity;
using EKSurvey.Core.Models.ViewModels.Account;
using EKSurvey.Core.Models.ViewModels.Survey;

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
                .ForMember(dest => dest.AvailableSurveys, opt => opt.MapFrom(src => new HashSet<UserSurvey>(src.Where(i => !i.Completed.HasValue))))
                .ForMember(dest => dest.CompletedSurveys, opt => opt.MapFrom(src => new HashSet<UserSurvey>(src.Where(i => i.Completed.HasValue))))
                .ForMember(dest => dest.NextSurvey, opt => opt.MapFrom(src => src.OrderBy(i => i.Modified.GetValueOrDefault(i.Created)).FirstOrDefault()));
        }
    }
}