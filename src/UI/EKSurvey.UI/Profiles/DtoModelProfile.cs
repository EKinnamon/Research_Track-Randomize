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

                    dest.TestId = src.TestSectionMarkers
                        .First(tsm => tsm.Test.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase))
                        .TestId;

                    dest.Started = src.TestSectionMarkers
                        .Where(tsm => tsm.Test.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase) && !tsm.Completed.HasValue)
                        .Max(tsm => tsm.Started);

                    dest.Modified = src.TestResponses
                        .Where(r => r.Test.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase) && r.SectionId == src.Id)
                        .Max(r => r.Modified.GetValueOrDefault(r.Created));

                    dest.Completed = src.TestSectionMarkers
                        .Where(tsm => tsm.Test.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase) && tsm.Completed.HasValue)
                        .Max(tsm => tsm.Completed);
                });
        }
    }
}