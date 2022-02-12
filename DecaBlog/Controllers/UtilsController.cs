using DecaBlog.Commons.Helpers;
using DecaBlog.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DecaBlog.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UtilsController : ControllerBase
    {
        private readonly IUtilsService _utilsService;
        public UtilsController(IUtilsService utilsService)
        {
            _utilsService = utilsService;
        }

        [HttpGet("stack-and-squad")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetstacksAndSquad()
        {
            var StackResAndSquad = await _utilsService.GetAllSquadsAndStack();
            if (StackResAndSquad == (null, null))
                return BadRequest(ResponseHelper.BuildResponse<object>(true, "Failed to get stacks and sqauds", ResponseHelper.NoErrors, null));
            return Ok(ResponseHelper.BuildResponse<object>(true, "Stacks and Squad", ResponseHelper.NoErrors, new { stack = StackResAndSquad.Item1, squad = StackResAndSquad.Item2 }));
        }
    }
}
