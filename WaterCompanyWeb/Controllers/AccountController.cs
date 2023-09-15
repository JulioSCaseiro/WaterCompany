using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WaterCompanyWeb.Data;
using WaterCompanyWeb.Data.Entities;
using WaterCompanyWeb.Helpers;
using WaterCompanyWeb.Models;
using static System.Net.Mime.MediaTypeNames;

namespace WaterCompanyWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IClientRepository _clientRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly UserManager<User> _userManager;
        private readonly IImageHelper _imageHelper;

        public AccountController(IUserHelper userHelper,
            IMailHelper mailHelper,
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager,
            IClientRepository clientRepository,
            IStaffRepository staffRepository,
            IConverterHelper converterHelper,
            UserManager<User> userManager,
            IImageHelper imageHelper
            )
        {
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _configuration = configuration;
            _roleManager = roleManager;
            _clientRepository = clientRepository;
            _staffRepository = staffRepository;
            _converterHelper = converterHelper;
            _userManager = userManager;
            _imageHelper = imageHelper;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user == null)
                {
                    this.ModelState.AddModelError(string.Empty, "Failed to login");
                }
                else
                if (user != null && await _userManager.IsInRoleAsync(user, "Admin") ||
                                    await _userManager.IsInRoleAsync(user, "Staff") ||
                                    await _userManager.IsInRoleAsync(user, "Client"))
                {
                    var result = await _userHelper.LoginAsync(model);
                    if (result.Succeeded)
                    {
                        if (this.Request.Query.Keys.Contains("ReturnUrl"))
                        {
                            return Redirect(this.Request.Query["ReturnUrl"].First());
                        }
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        this.ModelState.AddModelError(string.Empty, "Failed to login");
                    }
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [RoleAuthorization("Admin")]
        public IActionResult Register()
        {
            var list = _roleManager.Roles.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "Please select a role from below",
                Value = "0"
            });

            var model = new RegisterNewUserViewModel
            {
                Roles = list
            };
            return View(model);
        }

        [RoleAuthorization("Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterNewUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user == null)
                {
                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Username,
                        UserName = model.Username,
                        PhoneNumber = model.PhoneNumber,
                        Address = model.Address,
                        ZIPCode = model.ZIP,
                        NIF = model.NIF
                    };

                    var result = await _userHelper.AddUserAsync(user, model.Password);
                    if (result != IdentityResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "The user couldn't be created.");
                        return View(model);
                    }

                    var role = await _roleManager.FindByIdAsync(model.AccountRole);
                    if (role.Name == "Client")
                    {
                        Client client = new Client
                        {
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Email = model.Email,
                            Address = model.Address,
                            PhoneNumber = model.PhoneNumber,
                            ZIPCode = model.ZIP,
                            NIF = model.NIF,
                            User = user
                        };
                        await _userHelper.AddUserToRoleAsync(user, "Client");
                        await _clientRepository.CreateAsync(client);
                        await _userHelper.UpdateUserAsync(user);
                        //Verify if the user is in the role "Client"
                        var isInRole = await _userHelper.IsUserInRoleAsync(user, "Client");
                        if (!isInRole)
                        {
                            await _userHelper.AddUserToRoleAsync(user, "Client");
                        }
                    }

                    else if (role.Name == "Staff")
                    {
                        Staff staff = new Staff
                        {
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Email = model.Email,
                            Address = model.Address,
                            PhoneNumber = model.PhoneNumber,
                            ZIPCode = model.ZIP,
                            NIF = model.NIF,
                            User = user
                        };

                        await _userHelper.AddUserToRoleAsync(user, "Staff");
                        await _staffRepository.CreateAsync(staff);
                        await _userHelper.UpdateUserAsync(user);
                        var isInRole = await _userHelper.IsUserInRoleAsync(user, "Staff");
                        if (!isInRole)
                        {
                            await _userHelper.AddUserToRoleAsync(user, "Staff");
                        }
                    }
                    else
                    {
                            ModelState.AddModelError(string.Empty, "You cannot create an admin account.");
                            return View(model);
                    }
                }

                string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                string tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                Response response = _mailHelper.SendEmail(model.Username, "Email confirmation", $"<h1>Email Confirmation</h1>" +
                    $"Your access Email is: {user.UserName}.</br></br>" +
                    $"To be able to use the Water Company website, " +
                    $"plase click in this link to confirm your email and " +
                    $"proceed to change your password:</br></br><a href = \"{tokenLink}\"><b>Confirm Email</b></a>");

                if (response.IsSuccess)
                {
                    ViewBag.Message = "The instructions have been sent through email to your new user";
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "The email couldn't be sent.");
                }
            }
            return View(model);
        }

        public IActionResult RegisterRequest()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("ChangeUser", "Account");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterRequest(RegisterRequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                Response response = _mailHelper.SendEmail("watercompanyjulio@gmail.com", "Account Register Request", $"<h1>Account Register Request</h1>" +
                    $"</br></br>             New request from a client to create an account with the following information:" +
                    $"</br></br>             First Name: {model.FirstName}," +
                    $"</br></br>             Last Name: {model.LastName}, "+
                    $"</br></br>             Phone number: {model.PhoneNumber}," +
                    $"</br></br>             Username: {model.Username}," +
                    $"</br></br>             Email: {model.Email}," +
                    $"</br></br><b>          Create the following client with the information given.</b>");

                if (response.IsSuccess)
                {
                    ViewBag.Message = "Your information has been sent to the site admin. Please kindly wait for further instructions.";
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "The request failed. Please try again. If the problem proceeds, get in touch with our support.");
                }
            }

            return View(model);
        }
        
        public async Task<IActionResult> ChangeUser()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            var model = new ChangeUserViewModel();
            
            if (user != null)
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.PhoneNumber = user.PhoneNumber;
                model.Address = user.Address;
                model.NIF = user.NIF;
                model.ImageUrl = user.ImageUrl;
            }

            model = _converterHelper.ToUserViewModel(user);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUser(ChangeUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = model.ImageUrl;
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                if (user != null)
                {
                    var isInRole = await _userHelper.IsUserInRoleAsync(user, "Client");
                    if (isInRole)
                    {
                        if (model.ImageFile != null && model.ImageFile.Length > 0)
                        {
                            path = await _imageHelper.UploadImageAsync(model.ImageFile, "clients");
                        }
                    }
                    else
                    {
                        if (model.ImageFile != null && model.ImageFile.Length > 0)
                        {
                            path = await _imageHelper.UploadImageAsync(model.ImageFile, "staffs");
                        }
                    }

                    // Update user properties
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.PhoneNumber = model.PhoneNumber;
                    user.Address = model.Address;
                    user.NIF = model.NIF;
                    user.ImageUrl = path;

                    // Update user data store
                    var response = await _userHelper.UpdateUserAsync(user);
                    if (response.Succeeded)
                    {
                        ViewBag.UserMessage = "User updated successfully";
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
                    }
                }
            }
            return View(model);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

                if (user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return this.RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        this.ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "User not found");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {

            }
            return View();
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }

        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "The email doesn't correspont to a registered user.");
                    return View(model);
                }

                var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
                var link = this.Url.Action(
                    "ResetPassword",
                    "Account",
                    new { token = myToken }, protocol: HttpContext.Request.Scheme);

                Response response = _mailHelper.SendEmail(model.Email, "WaterCompany Password Reset", $"<h1>WaterCompany Password Reset</h1>" +
                $"To reset the password click in this link:</br></br>" +
                $"<a href = \"{link}\">Reset Password</a>");

                if (response.IsSuccess)
                {
                    this.ViewBag.Message = "The instructions to recover your password has been sent to email.";
                }
                return this.View();
            }
            return this.View(model);
        }
        public IActionResult ResetPassword(string token)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.UserName);
            if (user != null)
            {
                var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    this.ViewBag.Message = "Password reset successful.";
                    return View();
                }
                this.ViewBag.Message = "Error while resetting the password.";
                return View(model);
            }
            this.ViewBag.Message = "User not found.";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user != null)
                {
                    var result = await _userHelper.ValidatePasswordAsync(
                        user,
                        model.Password);

                    if (result.Succeeded)
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email), // internal mechanism that creates an area where the user email will be registered
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // creates a random guid associated with the user email
                        }; // we can know the full proccess of the build that took place

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"])); // gets the key written by the dev at "appsettings.json" and converts to bytes
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // generates credentials and the token, using the algorithm HmacSha256(most used algorithm)
                        var token = new JwtSecurityToken(
                            _configuration["Tokens:Issuer"],
                            _configuration["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddDays(15), // token valid for 15 days (can change for what the app owner wants)
                            signingCredentials: credentials);
                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };
                        return this.Created(string.Empty, results); // the first parameter is always empty since we don't want to send anything but the object.
                    }
                }
            }

            return BadRequest(); // if it goes wrong
        }

    }
}
