using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Integration.API.Controllers
{
    [Authorize(Roles = "Basic,Premium")]
    public class BaseController : ControllerBase
	{
		
	}
}

