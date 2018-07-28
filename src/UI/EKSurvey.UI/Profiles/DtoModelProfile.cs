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

                    var userTest = src.Tests.FirstOrDefault(t => t.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase));

                    dest.Started = userTest?.Started;

                    dest.Completed = userTest?.Completed;
                });

            CreateMap<Section, UserSection>()
                // Id, SurveyId, Name, Order
                .AfterMap((src, dest, ctx) =>
                {
                    var userId = ctx.Items["userId"].ToString();
                    dest.UserId = userId;

                    var userTest = src.Survey.Tests.FirstOrDefault(t => t.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase));

                    dest.Started = userTest?.TestResponses
                        .Where(r => r.SectionId == src.Id)
                        .Min(r => r.Created);

                    dest.Modified = userTest?.TestResponses
                        .Where(r => r.SectionId == src.Id)
                        .Max(r => r.Modified.GetValueOrDefault(r.Created));

                    //var sectionCompleted = userTest.TestResponses.
                });
        }
    }
}