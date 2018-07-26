using System;
using System.Threading.Tasks;
using System.Web.Mvc;

using AutoMapper;

using EKSurvey.Core.Models.ViewModels.Survey;
using EKSurvey.Core.Services;

using Microsoft.AspNet.Identity;

namespace EKSurvey.UI.Controllers
{
    [Authorize]
    public class SurveyController : Controller
    {
        private readonly ISurveyManager _surveyManager;
        private readonly IMapper _mapper;

        public SurveyController(ISurveyManager surveyManager, IMapper mapper)
        {
            _surveyManager = surveyManager ?? throw new ArgumentNullException(nameof(surveyManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); 
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var surveys = await _surveyManager.GetUserSurveysAsync(User.Identity.GetUserId());
            var viewModel = _mapper.Map<IndexViewModel>(surveys);

            return View(viewModel);
        }

    }
}