using System;
using System.Linq;
using AutoMapper;
using EKSurvey.Core.Models.DataTransfer;
using EKSurvey.Core.Models.Entities;

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

                    var userTests =
                        from ut in src.Tests
                        where ut.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase)
                        select ut;
                    dest.Completed = userTests.FirstOrDefault(t => t.Completed.HasValue)?.Completed;
                });
        }
    }
}