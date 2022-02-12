using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DecaBlog.Commons.Helpers;
using DecaBlog.Models.DTO;
using DecaBlog.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DecaBlog.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SquadController : ControllerBase
    {
        private readonly ISquadService _squadService;
        private readonly IMapper _mapper;

        public SquadController(ISquadService squadService, IMapper mapper)
        {
            _squadService = squadService;
            _mapper = mapper;
        }

        [HttpPost("add-squad")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddSquad(SquadToAddDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseHelper.BuildResponse<SquadMinInfoToReturnDto>(false, "Something went wrong", ModelState, null));
            var result = await _squadService.AddSquad(model);
            if (result == null)
            {
                ModelState.AddModelError("Add Squad", "Unable to add squad");
                return NotFound(ResponseHelper.BuildResponse<SquadMinInfoToReturnDto>(false, "Unable to add Squad", ModelState, null));
            }
            return Ok(ResponseHelper.BuildResponse(true, "Squad successfully added", ResponseHelper.NoErrors, result));
        }

        [HttpGet("get-by-id/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetSquad(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                ModelState.AddModelError("InvalidId", "Invalid squad id");
                return BadRequest(ResponseHelper.BuildResponse<object>(false, "Enter a valid squad Id", ModelState, null));
            }
            var squad = await _squadService.GetSquad(id);
            if (squad == null)
            {
                ModelState.AddModelError("NotFound", "Squad does not exist");
                return NotFound(ResponseHelper.BuildResponse<object>(false, "No squad found!", ModelState, null));
            }
            var squadToReturn = _mapper.Map<SquadMinInfoToReturnDto>(squad);
            return Ok(ResponseHelper.BuildResponse<SquadMinInfoToReturnDto>(true, "Success", ResponseHelper.NoErrors, squadToReturn));
        }

        [HttpGet("get-all-squads")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllSquads()
        {
            var squads = await _squadService.GetAllSquads();
            var listOfSquadToReturn = new List<SquadMinInfoToReturnDto>();
            if (squads != null)
            {
                foreach (var squad in squads)
                {
                    var mapped = _mapper.Map<SquadMinInfoToReturnDto>(squad);
                    listOfSquadToReturn.Add(mapped);
                }
                return Ok(ResponseHelper.BuildResponse<List<SquadMinInfoToReturnDto>>(true, "List of users", ResponseHelper.NoErrors, listOfSquadToReturn));
            }
            ModelState.AddModelError("Notfound", "There was no record for squads found!");
            return NotFound(ResponseHelper.BuildResponse<IEnumerable<SquadMinInfoToReturnDto>>(false, "No squad found!", ModelState, null));
        }
    }
}
