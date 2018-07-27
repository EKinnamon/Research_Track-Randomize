using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using EKSurvey.Core.Models.ViewModels.Test;
using EKSurvey.Core.Services;

namespace EKSurvey.UI.Controllers
{
    public class TestController : Controller
    {
        private readonly ITestManager _testManager;
        private readonly IMapper _mapper;

        public TestController(ITestManager testManager, IMapper mapper)
        {
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
                var responseViewModel = _mapper.Map<ResponseViewModel>(test);
                return RedirectToAction("Respond", responseViewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                TempData["Errors"] = ModelState;
            }

            return RedirectToAction("Index", "Survey");
        }

        [HttpGet]
        public async Task<ActionResult> Respond(ResponseViewModel viewModel)
        {
            throw new NotImplementedException();
        }
    }
}