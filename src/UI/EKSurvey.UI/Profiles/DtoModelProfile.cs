using System;
using System.Data.Entity;
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
                    var dbContext = ctx.Items["dbContext"] as DbContext;
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
        }
    }
}