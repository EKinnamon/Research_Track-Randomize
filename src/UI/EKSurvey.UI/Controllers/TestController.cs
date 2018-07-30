using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

using AutoMapper;
using EKSurvey.Core.Models.ViewModels.Test;
using EKSurvey.Core.Services;

using Elmah;
using Microsoft.AspNet.Identity;

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

            var userPage = pageId.HasValue
                ? userPages.Single(p => p.Page.Id == pageId.Value)
                : await _surveyManager.GetCurrentUserPageAsync(User.Identity.GetUserId(), id);

            var viewModel = _mapper.Map<ResponseViewModel>(userPage);

            var pageIndex = userPages.FindIndex(up => up.Page.Id == userPage.Page.Id);
            viewModel.PriorPageId = pageIndex != 0 
                ? userPages[pageIndex - 1].Page.Id 
                : (int?) null;

            var pageType = userPage.Page.GetType().BaseType;

            return View(pageType.Name, viewModel);
        }
    }
}