using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DecaBlog.Models;
using DecaBlog.Models.DTO;
using DecaBlog.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DecaBlog.Commons.Helpers;
using System.Web;

namespace DecaBlog.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IMailService _mailService;

        public UserController(IUserService userService, IMapper mapper, UserManager<User> userManager,
            IMailService mailService)
        {
            _userService = userService;
            _mapper = mapper;
            _userManager = userManager;
            _mailService = mailService;
        }

        [HttpGet("get-users")]
        public async Task<IActionResult> GetUsers([FromQuery] int pageNumber, [FromQuery] int perPage)
        {
            var result = await _userService.GetUsers(pageNumber, perPage);
            if (result.MetaData == null)
            {
                ModelState.AddModelError("NotFound", "No record of users found");
                var response = ResponseHelper.BuildResponse<List<UserMinInfoToReturnDto>>(false, "No results found!", ModelState, null);
                return NotFound(response);
            }
            return Ok(ResponseHelper.BuildResponse(true, "List of Users", ResponseHelper.NoErrors, result));
        }

        [HttpPatch("activate-user/{userid}")]
        public async Task<IActionResult> ActivateUser(string userid)
        {
            if (string.IsNullOrWhiteSpace(userid))
            {
                ModelState.AddModelError("userId", "Invalid userId");
                var response = ResponseHelper.BuildResponse<object>(false, "User not Activated", ModelState, null);
                return BadRequest(response);
            }
            var rep = await _userService.ActivateUser(userid);
            if (!rep)
            {
                ModelState.AddModelError("", "Unable to activate user");
                var response = ResponseHelper.BuildResponse<object>(false, "Unable to activate user", ModelState, null);
                return BadRequest(response);
            }
            var reply = ResponseHelper.BuildResponse<object>(true, "User has been activated", ResponseHelper.NoErrors, null);
            return Ok(reply);
        }

        [HttpPut("add-invitee")]
        public async Task<ActionResult<User>> AddInvitee([FromBody] RegisterInvitedUserDto model, [FromHeader] string inviteToken)
        {
            if (!ModelState.IsValid)
            {
                var response = ResponseHelper.BuildResponse<object>(false, "Please fill in the fields correctly", ModelState, null);
                return BadRequest(response);
            }
            var registerResult = await _userService.AddInvitee(model, inviteToken);
            if (!registerResult.Status)
                return BadRequest(registerResult);
            return Ok(registerResult);
        }

        [HttpPost("add-supporting-email/{userId}")]
        [Authorize(Roles = "Decadev")]
        public async Task<IActionResult> AddSupportingEmail([FromBody] AddSupportingEmailDto model, [FromRoute] string userId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseHelper.BuildResponse<string>(false, "Add supporting email failed", ModelState, null));
            //Validate input
            if (string.IsNullOrEmpty(userId))
                return BadRequest(ResponseHelper.BuildResponse<string>(false, "Please provide a valid Id", ResponseHelper.NoErrors, null));
            //Check if user exist
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ModelState.AddModelError("Not found", "User not found");
                return NotFound(ResponseHelper.BuildResponse<string>(false, $"User with id: {userId} not found", ResponseHelper.NoErrors, null));
            }
            var result = await _userService.AddSupportingEmail(userId, model.Email);
            if (!result.Item1 && !result.Item2)
                return BadRequest(ResponseHelper.BuildResponse<string>(false, "Cannot update existing supporting email", ResponseHelper.NoErrors, null));
            if (!result.Item1 && result.Item2)
                return BadRequest(ResponseHelper.BuildResponse<string>(false, "Supporting email update failed", ResponseHelper.NoErrors, null));
            if (result.Item1 && !result.Item2)
                return BadRequest(ResponseHelper.BuildResponse<string>(false, "User is not a decadev", ResponseHelper.NoErrors, null));
            var placeholders = new Dictionary<string, string>()
            {
                ["{Title}"] = "Congratulations!!",
                ["{body}"] = "Supporting email has been updated.",
            };
            var mail = new EmailMessage
            {
                ToEmail = new List<string> { model.Email },
                Subject = "Supporting Email Confirmation",
                PlainTextMessage = "Congratulations!! Supporting email has been updated",
                PlaceHolders = placeholders,
                Template = "SupportingEmail"
            };
            _mailService.SendMailAsync(mail).Wait();
            return Ok(ResponseHelper.BuildResponse<string>(true, "Support email added successfully", ResponseHelper.NoErrors, null));
        }

        [HttpPut("update-address/{userId}")]
        [Authorize(Roles = "Decadev,Editor,Admin")]
        public async Task<IActionResult> UpdateAddress([FromBody] AddressToUpdateDto model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ModelState.AddModelError("Unsuccessfull", "User not found");
                return NotFound(ResponseHelper.BuildResponse<object>(false, $"User with id:{userId} not found", ModelState, null));
            }
            var response = await _userService.UpdateAddress(model, userId);
            if (!response.Item1)
            {
                ModelState.AddModelError("Unauthorized", "User Address not updated");
                return Unauthorized(ResponseHelper.BuildResponse<object>(false, "You're not authorized to update this user's address", ModelState, null));
            }
            if (response.Item1 && response.Item2 == null)
            {
                ModelState.AddModelError("Unsuccessfull", "User Address not updated");
                return BadRequest(ResponseHelper.BuildResponse<object>(false, "User Address was not updated successfully", ModelState, null));
            }
            return Ok(ResponseHelper.BuildResponse<object>(true, "User Address updated successfully", null, response.Item2));
        }

        //<base_url>/api/v1/user/update
        [HttpPut("update-user/{userId}")]
        [Authorize(Roles = "Decadev,Editor,Admin")]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UserToUpdateDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseHelper.BuildResponse<object>(false, "Invalid entry", ModelState, model));
            var response = await _userService.UpdateUser(userId, model);
            if (response == null)
            {
                ModelState.AddModelError("Invalid", "User does not exist");
                return BadRequest(ResponseHelper.BuildResponse<object>(false, "Invalid user", ModelState, null));
            }
            return Ok(ResponseHelper.BuildResponse<UserInfoToReturnDto>(status: true, message: "Profile successfully updated", errs: ResponseHelper.NoErrors, data: response));
        }

        [HttpPatch("deactivate/{UserId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateUser(string UserId)
        {
            if (string.IsNullOrWhiteSpace(UserId))
            {
                ModelState.AddModelError("error", $"user does not exist");
                return BadRequest(ResponseHelper.BuildResponse<object>(false, "User does not exist", ModelState, null));
            }
            var stat = await _userService.DeactivateUser(UserId);
            if (stat == false)
            {
                ModelState.AddModelError("user", $"User does not exist");
                return BadRequest(ResponseHelper.BuildResponse<object>(false, "User does not exist", ModelState, null));
            }
            return Ok(ResponseHelper.BuildResponse<object>(true, "User deactivated", ResponseHelper.NoErrors, null));
        }

        [HttpGet("get-user/{userId}")]
        [Authorize(Roles = "Decadev,Editor,Admin")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var user = await _userManager.Users
                .Include(x => x.Articles)
                .Include(x => x.Address)
                .Include(x => x.UserStacks).ThenInclude(x => x.Stack)
                .Include(x => x.UserSquads).ThenInclude(x => x.Squad)
                .FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                ModelState.AddModelError("User", "User not found");
                var res = ResponseHelper.BuildResponse<object>(false, "The user does not exist", ModelState, "");
                return NotFound(res);
            }
            var userRoles = _userManager.GetRolesAsync(user).Result.ToList();
            var userToreturn = _mapper.Map<UserFullInfoDto>(user);
            userToreturn.Role = userRoles;
            return Ok(userToreturn);
        }

        [HttpPatch("editor/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MakeUserAnEditor(string userId)
        {
            //check if user exist
            ResponseDto<object> res;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ModelState.AddModelError("User", "The user does not exist");
                res = ResponseHelper.BuildResponse<object>(false, "The user does not exist", ModelState, "");
                return NotFound(res);
            }
            // check if user has role already
            var userRole = await _userManager.GetRolesAsync(user);
            if (await _userManager.IsInRoleAsync(user, "Editor"))
            {
                ModelState.AddModelError("error", "The User Already has this role");
                res = ResponseHelper.BuildResponse<object>(false, "The User Already has this role", ModelState, "");
                return BadRequest(res);
            }
            //add new role to user
            var response = await _userManager.AddToRoleAsync(user, "Editor");
            if (!response.Succeeded)
            {
                ModelState.AddModelError("Error", "Role was not added successfully");
                var errmsg = ResponseHelper.BuildResponse<object>(false, "Role was not added successfully", ModelState, "");
                return BadRequest(errmsg);
            }
            res = ResponseHelper.BuildResponse<object>(true, "Role Assigned", ResponseHelper.NoErrors, null);
            return Ok(res);
        }

        [HttpPatch("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(EmailConfirmationDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseHelper.BuildResponse<string>(false, "Something went wrong! Please try again.", ModelState, null));
            var response = await _userService.ConfirmEmail(model);
            if (!response)
            {
                ModelState.AddModelError("Confirm Email", "Email confirmation failed. Please try again.");
                return BadRequest(ResponseHelper.BuildResponse<string>(response, "Email confirmation failed", ModelState, null));
            }
            return Ok(ResponseHelper.BuildResponse<string>(response, "Email successfully confirmed", ResponseHelper.NoErrors, null));
        }

        [HttpPost("change-photo/{userId}")]
        [Authorize(Roles = "Decadev,Editor,Admin")]
        public async Task<IActionResult> UpdateUserPhoto([FromForm] PhotoToUploadDto model, [FromRoute] string userId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseHelper.BuildResponse<object>(false, "Please upload a photo", null, null));
            PhotoToReturnDto uploadResult = null;
            try
            {
                uploadResult = await _userService.UpdateUserPhoto(model, userId);
            }
            catch (Exception e)
            {
                return BadRequest(ResponseHelper.BuildResponse<object>(false, e.Message, null, null));
            }
            if (uploadResult == null)
            {
                ModelState.AddModelError("Error", "Photo Could not be Uploaded");
                var responseObj = ResponseHelper.BuildResponse<object>(false, "Error uploading photo", ModelState, null);
                return BadRequest(responseObj);
            }
            var res = ResponseHelper.BuildResponse<PhotoToReturnDto>(true, "Photo updated", ResponseHelper.NoErrors, uploadResult);
            return Ok(res);
        }

        [HttpDelete("remove-photo/{userId}")]
        [Authorize(Roles = "Decadev,Editor,Admin")]
        public async Task<IActionResult> RemoveProfilePhoto([FromRoute] string userId)
        {
            var deleteResult = await _userService.RemoveProfilePhoto(userId);
            if (!deleteResult)
            {
                ModelState.AddModelError("Error", "Unable to delete photo");
                var res = ResponseHelper.BuildResponse<object>(false, "Photo could not be deleted", ModelState, null);
                return BadRequest(res);
            }
            var response = ResponseHelper.BuildResponse<object>(true, "Photo Removed", ResponseHelper.NoErrors, null);
            return Ok(response);
        }

        [HttpPost("add-decadev")]
        public async Task<IActionResult> AddDecadev([FromBody] UserToRegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseHelper.BuildResponse<object>(false, "Registration failed", ModelState, null));
            var result = await _userService.AddDecadev(model);
            if (result.Tag == "StackNullError" || result.Tag == "SquadNullError")
            {
                ModelState.AddModelError(result.Key, result.Message);
                return BadRequest(ResponseHelper.BuildResponse<object>(false, "Registration", ModelState, null));
            }
            if (result.Tag == "CreateUserError" || result.Tag == "AddToRoleError")
            {
                foreach (var err in result.ErrorResult.Errors)
                    ModelState.AddModelError(err.Code, err.Description);
                return BadRequest(ResponseHelper.BuildResponse<object>(false, result.Message, ModelState, null));
            }
            var origin = HttpContext.Request.Headers["Origin"][0];
            var uriBuilder = new UriBuilder(origin + "/confirmemail");
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["email"] = result.User.Email;
            query["token"] = result.Token;
            uriBuilder.Query = query.ToString();
            var urlString = uriBuilder.ToString();
            var firstName = result.User.FirstName;
            var placeholders = new Dictionary<string, string>
            {
                ["{Message}"] = "Thank you for signing up. To get started, you need to confirm your email. Please click on the button below to continue.",
                ["{Link}"] = urlString,
                ["{FirstName}"] = char.ToUpper(firstName[0]) + firstName.Substring(1)
            };
            var mail = new EmailMessage
            {
                Subject = "Email Confirmation",
                Template = "ConfirmEmail",
                PlainTextMessage = "Thank you for signing up. To get started, you need to confirm your email. Please click the link below to continue.",
                ToEmail = new List<string> { result.User.Email },
                PlaceHolders = placeholders
            };
            await _mailService.SendMailAsync(mail);
            return Ok(ResponseHelper.BuildResponse(true, "Registered successfully!", ResponseHelper.NoErrors, result.User.Id));
        }

        [HttpGet("get-invitees")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetInvitees(int pageNumber, int perPage) =>
            Ok(ResponseHelper.BuildResponse(true, "success", ResponseHelper.NoErrors, await _userService.GetInvitees(pageNumber, perPage)));

        [HttpPost("invite-user")]
        [Authorize(Roles = "Decadev, Editor, Admin")]
        public async Task<IActionResult> InviteUser(string email)
        {
            string inviterId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (inviterId == null)
                return BadRequest(ResponseHelper.BuildResponse<object>(false, "Can't find current user", ResponseHelper.NoErrors, null));
            var inviteResult = await _userService.InviteUser(email, inviterId);
            if (inviteResult.Item1 == false)
                return BadRequest(ResponseHelper.BuildResponse<object>(false, $"{inviteResult.Item2}", ResponseHelper.NoErrors, null));
            return Ok(ResponseHelper.BuildResponse(true, $"{inviteResult.Item2}", ResponseHelper.NoErrors, false));
        }

        [HttpPatch("approve-invitee/{inviteeId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveInvitee(string inviteeId)
        {
            if (string.IsNullOrWhiteSpace(inviteeId))
            {
                ModelState.AddModelError("Error", "please provide a valid inviteeId");
                var res = ResponseHelper.BuildResponse<string>(false, "invalid invite id", ModelState, null);
                return BadRequest(res);
            }
            var approveInvite = await _userService.ApproveInvitee(inviteeId);
            if (!approveInvite.Item1)
            {
                ModelState.AddModelError("Error", approveInvite.Item2);
                var res = ResponseHelper.BuildResponse<string>(false, "failed to activate invited guest", ModelState, null);
                return BadRequest(res);
            }
            var response = ResponseHelper.BuildResponse<string>(true, "invite approved", ResponseHelper.NoErrors, null);
            return Ok(response);
        }
    }
}
