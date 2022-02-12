using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using DecaBlog.Data.Repositories.Interfaces;
using DecaBlog.Helpers;
using DecaBlog.Models;
using DecaBlog.Models.DTO;
using DecaBlog.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace DecaBlog.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userMgr;
        private readonly IInviteeRepository _inviteeRepository;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;
        const int PHOTO_MAX_ALLOWABLE_SIZE = 3000000;
        private readonly ISquadRepository _squadRepository;
        private readonly IStackRepository _stackRepository;
        private readonly IJwtService _jwtService;
        private readonly IMailService _mailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(UserManager<User> userManager,
            IInviteeRepository inviteeRepository, IMapper mapper, ICloudinaryService cloud, ISquadRepository squadRepository,
            IStackRepository stackRepository, IJwtService jwtService, IMailService mailService, IHttpContextAccessor httpContextAccessor)
        {
            _userMgr = userManager;
            _inviteeRepository = inviteeRepository;
            _mapper = mapper;
            _cloudinaryService = cloud;
            _squadRepository = squadRepository;
            _stackRepository = stackRepository;
            _mailService = mailService;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<bool> ActivateUser(string userid)
        {
            var res = new ResponseDto<string>();
            var user = await _userMgr.FindByIdAsync(userid);
            if (user == null)
                return false;
            user.IsActive = true;
            var result = await _userMgr.UpdateAsync(user);
            if (!result.Succeeded)
                return false;
            return true;
        }

        public async Task<PaginatedListDto<UserMinInfoToReturnDto>> GetUsers(int pageNumber, int perPage)
        {
            var usersToReturn = new List<UserMinInfoToReturnDto>();
            var users = _userMgr.Users
                .Include(x => x.UserStacks).ThenInclude(u => u.Stack)
                .Include(x => x.UserSquads).ThenInclude(u => u.Squad);
            if (users != null)
            {
                var pagedList = PagedList<User>.Paginate(users, pageNumber, perPage);
                foreach (var user in pagedList.Data)
                {
                    var role = await _userMgr.GetRolesAsync(user);
                    var userToReturn = _mapper.Map<UserMinInfoToReturnDto>(user);
                    userToReturn.Role = role.ToList();
                    usersToReturn.Add(userToReturn);
                }
                var result = new PaginatedListDto<UserMinInfoToReturnDto>
                {
                    MetaData = pagedList.MetaData,
                    Data = usersToReturn
                };
                return result;
            }
            return new PaginatedListDto<UserMinInfoToReturnDto> { MetaData = null };
        }

        public async Task<bool> ConfirmEmail(EmailConfirmationDto model)
        {
            var user = await _userMgr.FindByEmailAsync(model.Email);
            if (user == null)
                return false;
            var response = await _userMgr.ConfirmEmailAsync(user, model.Token);
            if (!response.Succeeded)
                return false;
            user.IsActive = true;
            await _userMgr.UpdateAsync(user);
            return true;
        }

        public async Task<(bool, bool)> AddSupportingEmail(string userId, string email)
        {
            /*
             The double boolean tuple is used so we can understand the type of response returned to the calling controller
             (false,false) = already added a supporting email, (true, false) = Not a decadev, (false, true) = User manager update failed
             (true, true) udated done successfully
             */
            var user = await _userMgr.Users.Where(x => x.Id == userId).Include(x => x.SupportingEmails).FirstOrDefaultAsync();
            if (user.SupportingEmails != null)
                return (false, false);
            var resp = Regex.Match(user.Email, "^\\w+([-+.']\\w+)*@?(decagon.dev)$");
            if (!resp.Success)
                return (true, false);
            user.SupportingEmails = new SupportingEmail { Email = email };
            var response = await _userMgr.UpdateAsync(user);
            if (!response.Succeeded)
                return (false, true);
            return (true, true);
        }

        public async Task<(bool, AddressToReturnDto)> UpdateAddress(AddressToUpdateDto model, string userId)
        {
            var getUserwithAddress = await _userMgr.Users.Where(x => x.Id == userId).Include(x => x.Address).FirstOrDefaultAsync();
            var roleAdmin = await _userMgr.IsInRoleAsync(getUserwithAddress, "Admin");
            if (!roleAdmin)
            {
                var identity = (System.Security.Claims.ClaimsPrincipal)System.Threading.Thread.CurrentPrincipal;
                var principal = System.Threading.Thread.CurrentPrincipal as System.Security.Claims.ClaimsPrincipal;
                var currentuserId = identity.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                if (!userId.Equals(currentuserId))
                    return (false, null);
            }
            var address = _mapper.Map<Address>(model);
            address.UserId = userId;
            try
            {
                getUserwithAddress.Address = address;
                var res = await _userMgr.UpdateAsync(getUserwithAddress);
                if (res.Succeeded)
                {
                    var updatedAddress = _mapper.Map<AddressToReturnDto>(address);
                    return (true, updatedAddress);
                }
            }
            catch (Exception)
            {
                //log error
                return (true, null);
            }
            return (true, null);
        }

        public async Task<ResponseDto<string>> AddInvitee(RegisterInvitedUserDto model, string inviteToken)
        {
            var invitee = await _inviteeRepository.GetInviteeByEmail(model.Email);
            if (invitee != null)
                return new ResponseDto<string> { Status = false, Message = "Link has expired" };
            invitee = new Invitee();
            _mapper.Map(model, invitee);
            invitee.InviterToken = inviteToken;
            var res = await _inviteeRepository.AddInvitee(invitee);
            if (!res)
                return new ResponseDto<string> { Status = false, Message = "Unable to register, please try again." };
            return new ResponseDto<string>
            {
                Status = true,
                Message = "Your registration was successful. Your profile is waiting for an admin approval. You will get an email when approved.",
                Errors = null,
                Data = invitee.InviteeId
            };
        }

        public async Task<UserInfoToReturnDto> UpdateUser(string id, UserToUpdateDto model)
        {
            try
            {
                var user = await _userMgr.FindByIdAsync(id);
                if (user != null)
                {
                    Mapper.Map(model, user, typeof(UserToUpdateDto), typeof(User));
                    var response = await _userMgr.UpdateAsync(user);
                    var result = _mapper.Map<UserInfoToReturnDto>(user);
                    return result;
                }
                return null;
            }
            catch (Exception)
            {
                //log error
                return null;
            }
        }

        public async Task<bool> DeactivateUser(string UserId)
        {
            var user = await _userMgr.FindByIdAsync(UserId);
            if (user == null)
                return false;
            user.IsActive = false;
            await _userMgr.UpdateAsync(user);
            return true;
        }

        public async Task<PhotoToReturnDto> UpdateUserPhoto(PhotoToUploadDto model, string userId)
        {
            var fileFormats = new string[] { ".png", ".jpg", ".jpeg" };
            var isCorrectFormat = false;
            foreach (var f in fileFormats)
            {
                if (model.Photo.FileName.EndsWith(f))
                {
                    isCorrectFormat = true;
                    break;
                }
            }
            if (!isCorrectFormat)
                throw new InvalidOperationException("File is not an image");
            if (model.Photo.Length > PHOTO_MAX_ALLOWABLE_SIZE)
                throw new InvalidOperationException("File is too large. Max size is 3mb");
            var user = await _userMgr.FindByIdAsync(userId);
            if (user == null)
                return null;
            var uploadResult = await _cloudinaryService.UploadPhoto(model.Photo);
            if (uploadResult == null)
                return null;
            await _cloudinaryService.DeletePhoto(user.PhotoPublicId);
            user.PhotoPublicId = uploadResult.PublicId;
            user.PhotoUrl = uploadResult.Url;
            await _userMgr.UpdateAsync(user);
            return new PhotoToReturnDto
            {
                Id = user.Id,
                PublicId = uploadResult.PublicId,
                PhotoUrl = uploadResult.Url
            };
        }

        public async Task<bool> RemoveProfilePhoto(string userId)
        {
            var user = await _userMgr.FindByIdAsync(userId);
            if (user == null)
                return false;
            var deleteResult = await _cloudinaryService.DeletePhoto(user.PhotoPublicId);
            if (!deleteResult)
                return false;
            user.PhotoPublicId = null;
            user.PhotoUrl = null;
            await _userMgr.UpdateAsync(user);
            return true;
        }

        public async Task<RegisterResponseDto> AddDecadev(UserToRegisterDto model)
        {
            if (await _stackRepository.GetStackById(model.StackId) == null)
                return new RegisterResponseDto { Tag = "StackNullError", Key = "Stack", Message = "Invalid Stack" };
            if (await _squadRepository.GetSquad(model.SquadId) == null)
                return new RegisterResponseDto { Tag = "SquadNullError", Key = "Squad", Message = "Invalid Squad" };
            var user = _mapper.Map<User>(model);
            user.InvitationToken = _jwtService.GenerateInvitationToken(user.Id, user.Email);
            var addUserResult = await _userMgr.CreateAsync(user, model.Password);
            if (!addUserResult.Succeeded)
                return new RegisterResponseDto { Tag = "CreateUserError", ErrorResult = addUserResult, Message = "Registration failed" };
            user.UserStacks.Add(new UserStack { StackId = model.StackId });
            user.UserSquads.Add(new UserSquad { SquadId = model.SquadId });
            var addUserToRoleResult = await _userMgr.AddToRoleAsync(user, "Decadev");
            if (!addUserToRoleResult.Succeeded)
                return new RegisterResponseDto { Tag = "AddToRoleError", ErrorResult = addUserToRoleResult, Message = "Registration failed!" };
            var token = await _userMgr.GenerateEmailConfirmationTokenAsync(user);
            var userToReturn = _mapper.Map<UserToReturnDto>(user);
            return new RegisterResponseDto { Tag = "Success", ErrorResult = null, User = userToReturn, Token = token };
        }

        public async Task<PaginatedListDto<InviteesMinInfoToReturnDto>> GetInvitees(int pageNumber, int perPage)
        {
            var users = _inviteeRepository.GetInvitees();
            if (users != null)
            {
                var InviteeToReturn = new List<InviteesMinInfoToReturnDto>();
                var pagedList = PagedList<Invitee>.Paginate(users, pageNumber, perPage);
                foreach (var user in pagedList.Data)
                {
                    InviteeToReturn.Add(new InviteesMinInfoToReturnDto
                    {
                        InviteeId = user.InviteeId,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Stack = (await _stackRepository.GetStackById(user.StackId))?.Name,
                        Squad = (await _squadRepository.GetSquad(user.SquadId))?.Name
                    });
                }
                var result = new PaginatedListDto<InviteesMinInfoToReturnDto>
                {
                    MetaData = pagedList.MetaData,
                    Data = InviteeToReturn
                };
                return result;
            }
            return new PaginatedListDto<InviteesMinInfoToReturnDto> { MetaData = null };
        }

        public async Task<(bool, string)> InviteUser(string email, string inviterId)
        {
            var invitedUser = await _userMgr.FindByEmailAsync(email);
            if (invitedUser != null)
                return (false, $"{email} is already a user");
            Invitee invitee = await _inviteeRepository.GetInviteeByEmail(email);
            if (invitee != null)
                return (false, $"{email} has already been invited");
            var inviter = await _userMgr.FindByIdAsync(inviterId);
            if (inviter == null)
                return (false, "No current user");
            var baseUrl = _httpContextAccessor.HttpContext.Request.Headers["Origin"][0];
            var urlQueryStrings = new Dictionary<string, string>
            {
                ["email"] = email,
                ["inviteToken"] = inviter.InvitationToken
            };
            var placeHolders = new Dictionary<string, string>
            {
                ["{inviterName}"] = $"{inviter.FirstName} {inviter.LastName}",
                ["{link}"] = QueryHelpers.AddQueryString(baseUrl + "/invite", urlQueryStrings)
            };
            var contact = new EmailMessage
            {
                ToEmail = new List<string>() { email },
                Template = "InviteUser",
                PlaceHolders = placeHolders,
                Subject = "You are Invited",
                PlainTextMessage = "This is a forum for developers"
            };
            await _mailService.SendMailAsync(contact);
            return (true, "Invitation has been sent");
        }

        public async Task<(bool, string)> ApproveInvitee(string inviteId)
        {
            var invitee = await _inviteeRepository.GetInviteeById(inviteId);
            if (invitee == null) return (false, "invitee does not exit");
            var stack = await _stackRepository.GetStackById(invitee.StackId);
            if (stack == null) return (false, "Invalid stack selected");
            var squad = await _squadRepository.GetSquad(invitee.SquadId);
            if (squad == null) return (false, "Invalid squad selected");
            var user = new User
            {
                FirstName = invitee.FirstName,
                LastName = invitee.LastName,
                Email = invitee.Email,
                UserName = invitee.Email,
                InvitationToken = invitee.InviterToken
            };
            var response = await _userMgr.CreateAsync(user, invitee.Password);
            if (!response.Succeeded)
                return (false, "Failed to create invitee");
            var createdUser = await _userMgr.FindByEmailAsync(user.Email);
            if (createdUser == null) return (false, "Failed to create invitee");
            createdUser.IsActive = true;
            var activateEmailToken = await _userMgr.GenerateEmailConfirmationTokenAsync(createdUser);
            var emailConfirm = await _userMgr.ConfirmEmailAsync(user, activateEmailToken);
            if (!emailConfirm.Succeeded) return (false, "Failed to create invitee");
            user.UserSquads.Add(new UserSquad { Squad = squad });
            user.UserStacks.Add(new UserStack { Stack = stack });
            var updateResult = await _userMgr.UpdateAsync(user);
            if (!updateResult.Succeeded) return (false, "could not add squad or stack for this user");
            return (true, "invitation approved and confirmed");
        }
    }
}
