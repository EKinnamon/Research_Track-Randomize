using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AutoMapper;
using EKSurvey.Core.Models.DataTransfer;
using EKSurvey.Core.Models.Entities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EKSurvey.Core.Services
{
    public class DtoBuilder : IDtoBuilder
    {
        private readonly IdentityUser _user;
        private readonly DbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly AfterMapperActionCollection _afterActions;

        public DtoBuilder(IdentityUser user, DbContext dbContext, IMapper mapper)
        {
            _user = user;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

            _afterActions = new AfterMapperActionCollection
            {
                new AfterMapperAction<Survey,UserSurvey>
                {
                    Action = (survey, userSurvey) =>
                    {
                        var src = (Survey) survey;
                        var dest = (UserSurvey) userSurvey;

                        dest.UserId = user.Id;

                        var userTest = src.Tests.FirstOrDefault(t =>
                            t.UserId.Equals(_user.Id, StringComparison.OrdinalIgnoreCase));

                        dest.Started = userTest?.Started;
                        dest.Completed = userTest?.Completed;
                    }
                },
                new AfterMapperAction<Section,UserSection>
                {
                    Action = (section, userSection) =>
                    {
                        var src = (Section) section;
                        var dest = (UserSection) userSection;

                        dest.UserId = user.Id;
                        var userTest = _dbContext.Set<Test>().SingleOrDefault(t => t.UserId.Equals(_user.Id, StringComparison.OrdinalIgnoreCase) && t.SurveyId == src.SurveyId);
                        var sectionResponses = _dbContext.Set<TestResponse>().Where(tr => tr.Page.SectionId == src.Id);

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

                    }
                },
                new AfterMapperAction<IEnumerable<Section>, UserSectionGroup>
                {
                    Action = (sections, sectionGroup) =>
                    {
                        var src = (IEnumerable<Section>) sections;
                        var dest = (UserSectionGroup) sectionGroup;

                        var userSections = Build<IEnumerable<Section>, IEnumerable<UserSection>>(src);

                        dest.SelectorType = src.First().SelectorType;
                        dest.AddRange(userSections);
                    }
                },
                new AfterMapperAction<IPage, UserPage>
                {
                    Action = (page, userPage) =>
                    {
                        var src = (Page) page;
                        var dest = (UserPage) userPage;

                        dest.UserId = _user.Id;
                        dest.SurveyId = src.Section.SurveyId;

                        var test = _dbContext
                            .Set<Test>()
                            .SingleOrDefault(t => t.UserId.Equals(_user.Id, StringComparison.OrdinalIgnoreCase) && 
                                                  t.SurveyId == src.Section.SurveyId);
                        if (test == null)
                            return;

                        var userResponse = src.TestResponses.SingleOrDefault(tr => tr.TestId == test.Id);
                        dest.Response = userResponse?.Response;
                        dest.Responded = userResponse?.Responded;
                        dest.Modified = userResponse?.Modified;
                    }
                },
                new AfterMapperAction<TestResponse, UserResponse>
                {
                    Action = (testResponse, userResponse) =>
                    {
                        var src = (TestResponse) testResponse;
                        var dest = (UserResponse) userResponse;

                        dest.UserId = _user.Id;
                        if (!(src.Page is RangeQuestion question))
                            return;

                        dest.IsLikert = question.IsLikert;
                        dest.Range = question.Range;
                    }
                }
            };
        }

        public TDto Build<T, TDto>(T entity)
        {
            return _afterActions.Exists<T,TDto>()
                ? _mapper.Map<TDto>(entity) 
                : _mapper.Map<TDto>(entity, opt => opt.AfterMap(AfterMapper(typeof(T), typeof(TDto))));
        }

        private Action<object, object> AfterMapper(Type srcType, Type destType)
        {
            return _afterActions[srcType, destType].Action;
        }
    }
}
