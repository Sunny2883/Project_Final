using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Promact.CustomerSuccess.Platform.Controllers;
[Route("api")]
[ApiController]
public class HomeController : AbpController
{
    public ActionResult Index()
    {
        return Redirect("~/swagger");
    }
    [HttpGet("helth-check")]
    public IActionResult GetHealthDownload()
    {
        return Ok("OK");
    }
}


