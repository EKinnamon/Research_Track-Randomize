using System;
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
        public async Task<ActionResult> Respond(int id)
        {
            var page = await _surveyManager.GetCurrentUserPageAsync(User.Identity.GetUserId(), id);
            var viewModel = _mapper.Map<ResponseViewModel>(page);
            return View(viewModel);
        }
    }
}