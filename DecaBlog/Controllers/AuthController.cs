using DecaBlog.Models.DTO;
using DecaBlog.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Linq;
using DecaBlog.Commons.Helpers;
using Microsoft.AspNetCore.Identity;
using DecaBlog.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace DecaBlog.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IMailService _mailService;
        private readonly UserManager<User> _userManager;
        private readonly IJwtService _jwtService;

        public AuthController(IAuthService authService, IUserService userService, IMailService mailService, IJwtService jwtService, UserManager<User> userManager)
        {
            _authService = authService;
            _userService = userService;
            _mailService = mailService;
            _userManager = userManager;
            _jwtService = jwtService;
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseHelper.BuildResponse<string>(false, "Email is Invalid", ModelState, ""));
            var response = await _authService.ForgotPassword(email);
            if (response == null)
                return BadRequest(ResponseHelper.BuildResponse<string>(false, "Email does not exist", ModelState, ""));
            var origin = HttpContext.Request.Headers["Origin"][0];
            var uriBuilder = new UriBuilder(origin + "/forgotpassword");
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["email"] = email;
            query["token"] = response;
            uriBuilder.Query = query.ToString();
            var urlString = uriBuilder.ToString();
            var placeholders = new Dictionary<string, string>
            {
                ["{firstmsg}"] = "To Reset your Password, Please click the link below.",
                ["{Link}"] = urlString,
            };
            var mail = new EmailMessage
            {
                Subject = "Forgot Password",
                Template = "ForgotPassword",
                PlainTextMessage = "To Reset your Password, Please click the link below.",
                ToEmail = new List<string> { email },
                PlaceHolders = placeholders
            };
            await _mailService.SendMailAsync(mail);
            return Ok(ResponseHelper.BuildResponse<string>(true, "Successfully sent mail", null, response));
        }

        [HttpPatch("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseHelper.BuildResponse<string>(false, "Password reset not successful", ModelState, null));
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("Not found", "User not found");
                return NotFound(ResponseHelper.BuildResponse<string>(false, $"User with email: {model.Email} not found", ModelState, null));
            }
            if (!(await _userManager.IsEmailConfirmedAsync(user)))
            {
                ModelState.AddModelError("Password reset failed", "Email not confirmed");
                return BadRequest(ResponseHelper.BuildResponse<string>(false, "Email is not confirmed", ModelState, null));
            }
            var response = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!response.Succeeded)
            {
                ModelState.AddModelError("Unsuccessful", "Password reset failed");
                return BadRequest(ResponseHelper.BuildResponse<string>(false, "Password reset not successful", ModelState, null));
            }
            return Ok(ResponseHelper.BuildResponse<string>(true, "Password reset successful", ResponseHelper.NoErrors, null));
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            //find the user by his email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("Invalid", "Invalid Credentials");
                return BadRequest(ResponseHelper.BuildResponse<LoginResponseDto>(false, "User does not exist", ModelState, null));
            }
            // check if user's email is confirmed
            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                var result = await _authService.Login(model);
                if (result == null)
                {
                    ModelState.AddModelError("Failed", "Login Failed");
                    return BadRequest(ResponseHelper.BuildResponse<LoginResponseDto>(false, "Email or Password incorrect, please check and try again.", ModelState, null));
                }
                return Ok(ResponseHelper.BuildResponse<LoginResponseDto>(true, "Login is sucessful!", null, result));
            }
            ModelState.AddModelError("Invalid", "Please confirm your email.");
            return BadRequest(ResponseHelper.BuildResponse<LoginResponseDto>(false, "Email not confirmed", ModelState, null));
        }

        [HttpPut("change-password")]
        [Authorize(Roles = "Admin, Editor, Decadev")]
        public async Task<IActionResult> ChangePassword(string Id, [FromBody] PasswordToUpdateDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseHelper.BuildResponse<string>(false, "Change password failed", ModelState, null));
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                ModelState.AddModelError("NotFound", "User not found");
                return NotFound(ResponseHelper.BuildResponse<string>(false, "Change password failed", ModelState, null));
            }
            var result = await _authService.ChangePassword(Id, model);
            if (!result)
                return BadRequest(ResponseHelper.BuildResponse<string>(true, "Failed to change password", ResponseHelper.NoErrors, null));
            return Ok(ResponseHelper.BuildResponse<string>(true, "Your Password has been changed successfully", ResponseHelper.NoErrors, null));
        }

        [HttpGet("google-signin")]
        public IActionResult GoogleSignIn()
        {
            return new ChallengeResult(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse") // Where google responds back 
            });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            //Check authentication response as mentioned on startup file as options.DefaultSignInScheme = "External"
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //string[] userInfo = { authenticateResult.Principal.FindFirst(ClaimTypes.Name).Value, authenticateResult.Principal.FindFirst(ClaimTypes.Email).Value };
            if (!authenticateResult.Succeeded)
            {
                ModelState.AddModelError("Invalid", "Credentials provided by the user is invalid");
                return BadRequest(ResponseHelper.BuildResponse<object>(false, "Invalid credentials", ModelState, null));
            }
            //Check if the redirection has been done via google or any other links
            if (authenticateResult.Principal.Identities.ToList()[0].AuthenticationType.ToLower() == "google")
            {
                //check if principal value exists or not 
                if (authenticateResult.Principal != null)
                {
                    //get google account id for any operation to be carried out on the basis of the id
                    var googleAccountId = authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var googleAccountEmail = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
                    var user = await _userManager.FindByEmailAsync(googleAccountEmail);
                    if (user == null)
                    {
                        ModelState.AddModelError("Not Found", $"User with email {authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value} was not found");
                        return NotFound(ResponseHelper.BuildResponse<object>(false, "User not found", ModelState, null));
                    }
                    // check if user's email is confirmed
                    if (await _userManager.IsEmailConfirmedAsync(user))
                    {
                        var userRoles = await _userManager.GetRolesAsync(user);
                        var token = _jwtService.GenerateJWTToken(user, userRoles.ToList());
                        var loginInfo = new LoginResponseDto()
                        {
                            UserId = user.Id,
                            Token = token,
                            FullName = user.FirstName + " " + user.LastName,
                        };
                        foreach (var role in userRoles)
                            loginInfo.Role.Add(role);
                        //claim value initialization as mentioned on the startup file with o.DefaultScheme = "Application"
                        var claimsIdentity = new ClaimsIdentity("Application");
                        var res = new ResponseDto<LoginResponseDto>();
                        res.Status = true;
                        res.Data = loginInfo;
                        res.Message = "User Logged in successfully";
                        return Ok(res);
                    }
                    ModelState.AddModelError("Invalid", "Please confirm your email.");
                    return BadRequest(ResponseHelper.BuildResponse<LoginResponseDto>(false, "Email not confirmed", ModelState, null));
                }
            }
            ModelState.AddModelError("Invalid", "User must first confirm email before attempting to login");
            return BadRequest(ResponseHelper.BuildResponse<object>(false, "Email not confirmed", ModelState, null));
        }
    }
}
