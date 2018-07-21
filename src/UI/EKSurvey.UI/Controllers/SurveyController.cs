using System.Web.Mvc;

namespace EKSurvey.UI.Controllers
{
    [Authorize]
    public class SurveyController : Controller
    {
        // GET: Survey
        public ActionResult Index()
        {
            return View();
        }
    }
}