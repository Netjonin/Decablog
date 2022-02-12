using DecaBlog.Models.DTO;
using DecaBlog.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using DecaBlog.Commons.Helpers;

namespace DecaBlog.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class StackController : ControllerBase
    {
        private readonly IStackService _stackService;
        public StackController(IStackService stackService)
        {
            _stackService = stackService;
        }

        [HttpPost("add-stack")]
        [Authorize(Roles = "Admin")]
        public IActionResult AddStack(StackToAddDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseHelper.BuildResponse<string>(false, "Failed to add stack", ModelState, null));
            var exists = _stackService.GetStackByName(model.Name);
            if (exists != null)
            {
                ModelState.AddModelError("Failed", "Stack already exist!");
                var response = ResponseHelper.BuildResponse<StackMinInfoToReturnDto>(false, "Failed to add stack", ModelState, null);
                return BadRequest(response);
            }
            var result = _stackService.AddStack(model);
            if (result == null)
            {
                ModelState.AddModelError("Failed", "Stack could not be added!");
                var response = ResponseHelper.BuildResponse<StackMinInfoToReturnDto>(false, "Failed to add stack", ModelState, null);
                return BadRequest(response);
            }
            return Ok(ResponseHelper.BuildResponse(true, "Stack added!", ResponseHelper.NoErrors, result));
        }

        [HttpGet("get-all-stacks")]
        [Authorize(Roles = "Admin, Editor, Decadev")]
        public async Task<IActionResult> GetAllStacks()
        {
            var data = await _stackService.GetAllStacks();
            if (data == null)
            {
                ModelState.AddModelError("Stacks", "No record found");
                return NotFound(ResponseHelper.BuildResponse<IEnumerable<StackMinInfoToReturnDto>>(false, "No record found!", ModelState, null));
            }
            return Ok(ResponseHelper.BuildResponse(true, "Stacks", ResponseHelper.NoErrors, data));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("get-by-id/{stackId}")]
        public async Task<IActionResult> GetStackById(string stackId)
        {
            var stackToReturn = await _stackService.GetStackById(stackId);
            if (stackToReturn == null)
                return BadRequest(ResponseHelper.BuildResponse<StackMinInfoToReturnDto>(false, $"Stack with provided id:{stackId} not found", ResponseHelper.NoErrors, null));
            return Ok(ResponseHelper.BuildResponse<StackMinInfoToReturnDto>(true, $"Stack returned successfully", ResponseHelper.NoErrors, stackToReturn));
        }
    }
}
