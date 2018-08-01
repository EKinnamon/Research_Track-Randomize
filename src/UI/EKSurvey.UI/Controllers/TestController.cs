using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

using AutoMapper;
using EKSurvey.Core.Models.ViewModels.Test;
using EKSurvey.Core.Services;

using Elmah;
using Microsoft.AspNet.Identity;
using Exception = System.Exception;

namespace EKSurvey.UI.Controllers
{
    public class TestController : Controller
    {
        private readonly ISurveyManager _surveyManager;
        private readonly ITestManager _testManager;
        private readonly IMapper _mapper;

        public TestController(ISurveyManager surveyManager, ITestManager testManager, IMapper mapper)
        {
            _surveyManager = surveyManager ?? throw new ArgumentNullException(nameof(surveyManager));
            _testManager = testManager ?? throw new ArgumentNullException(nameof(testManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Survey");
            }

            try
            {
                var test = await _testManager.CreateAsync(viewModel.SurveyId, viewModel.UserId);
                return RedirectToAction("Respond", new { id = viewModel.SurveyId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ErrorSignal.FromCurrentContext().Raise(ex);
                TempData["Errors"] = ModelState;
            }

            return RedirectToAction("Index", "Survey");
        }

        [HttpGet]
        public async Task<ActionResult> Respond(int id, int? pageId)
        {
            var userSection = await _surveyManager.GetCurrentUserSectionAsync(User.Identity.GetUserId(), id);
            var userPages = (await _surveyManager.GetUserPagesAsync(User.Identity.GetUserId(), userSection.Id)).ToList();
            var userTest = await _testManager.GetAsync(User.Identity.GetUserId(), id);
            var userPage = pageId.HasValue
                ? userPages.Single(p => p.Page.Id == pageId.Value)
                : await _surveyManager.GetCurrentUserPageAsync(User.Identity.GetUserId(), id);

            if (userPage == null)
            {
                return RedirectToAction("SectionReview", "Test", new { id });
            }

            var viewModel = _mapper.Map<ResponseViewModel>(userPage);

            var pageIndex = userPages.FindIndex(up => up.Page.Id == userPage.Page.Id);
            viewModel.TestId = userTest.Id;
            viewModel.PriorPageId = pageIndex != 0 
                ? userPages[pageIndex - 1].Page.Id 
                : (int?) null;

            return View((viewModel.Page.GetType().BaseType ?? viewModel.Page.GetType()).Name, viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Respond(ResponseViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var userSection = await _surveyManager.GetCurrentUserSectionAsync(User.Identity.GetUserId(), viewModel.SurveyId);
                var userPages = (await _surveyManager.GetUserPagesAsync(User.Identity.GetUserId(), userSection.Id)).ToList();
                var userPage = userPages.Single(p => p.Page.Id == viewModel.PageId);

                return View(userPage.Page.GetType().BaseType?.Name, viewModel);
            }

            try
            {
                var response = await _testManager.RespondAsync(User.Identity.GetUserId(), viewModel.SurveyId, viewModel.Response, viewModel.PageId);
                return RedirectToAction("Respond", new { id = viewModel.SurveyId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(viewModel.Page.GetType().BaseType?.Name, viewModel);
        }

        [HttpGet]
        public async Task<ActionResult> SectionReview(int id)
        {
            var sections = await _surveyManager.GetUserSectionsAsync(User.Identity.GetUserId(), id);
            var userSection = await _surveyManager.GetCurrentUserSectionAsync(User.Identity.GetUserId(), id);
            var sectionResponses = await _surveyManager.GetSectionResponsesAsync(User.Identity.GetUserId(), userSection.Id);
            var viewModel = _mapper.Map<SectionReviewViewModel>(userSection);
            _mapper.Map(sectionResponses, viewModel);
            viewModel.IsLastSection = sections.Last().Id == userSection.Id;
            return View(viewModel);
        }
    }
}