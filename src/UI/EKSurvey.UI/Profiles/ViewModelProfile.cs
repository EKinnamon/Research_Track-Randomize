using AutoMapper;
using EKSurvey.Core.Models.Identity;
using EKSurvey.Core.Models.ViewModels.Account;

namespace EKSurvey.UI.Profiles
{
    public class ViewModelProfile : Profile
    {
        public ViewModelProfile()
        {
            CreateMap<RegisterViewModel, ApplicationUser>()
                // Email, SecondaryEmail, PhoneNumber mapped.
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
        }
    }
}