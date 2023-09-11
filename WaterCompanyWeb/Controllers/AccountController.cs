﻿using Microsoft.AspNetCore.Identity;
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
using WaterCompanyWeb.Data.Entities;
using WaterCompanyWeb.Helpers;
using WaterCompanyWeb.Models;

namespace WaterCompanyWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;

        public AccountController(IUserHelper userHelper,
            IMailHelper mailHelper,
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager,
            UserManager<User> userManager
            )
        {
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _configuration = configuration;
            _roleManager = roleManager;
            _userManager = userManager;
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
                }
            }

            this.ModelState.AddModelError(string.Empty, "Failed to login");
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
            var role = _roleManager.Roles.ToList();
            ViewBag.Roles = new SelectList(role, "Name", "Name");
            return View();

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
                        LastName = model.FirstName,
                        Email = model.Username,
                        UserName = model.Username,
                        PhoneNumber = model.PhoneNumber
                    };
                    var result = await _userHelper.AddUserAsync(user, model.Password, model.SelectedRole);
                    if (result.Succeeded)
                    {
                        // Check if the selected role exists
                        var roleExists = await _roleManager.RoleExistsAsync(model.SelectedRole);
                        if (!roleExists)
                        {
                            // If the role doesn't exist, create the role
                            await _roleManager.CreateAsync(new IdentityRole(model.SelectedRole));
                        }

                        // Add the user to the selected role
                        await _userHelper.AddUserToRoleAsync(user, model.SelectedRole);


                        string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                        string tokenLink = Url.Action("ConfirmEmail", "Account", new
                        {
                            userid = user.Id,
                            token = myToken
                        }, protocol: HttpContext.Request.Scheme);

                        Response response = _mailHelper.SendEmail(model.Username, "Email confirmation", $"<h1>Email Confirmation</h1>" +
                            $"Your access Email is: {user.UserName}. \n" +
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
                    else
                    {
                        // Handle user creation failure
                        // Show error messages
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }
            ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
            return View(model);
        }

        public IActionResult RegisterRequest()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterRequest(RegisterRequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                Response response = _mailHelper.SendEmail("watercompanyjulio@gmail.com", "Account Register Request", $"<h1>Account Register Request</h1>" +
                    $"New request from a client to create an account with the following information: \n" +
                    $"<a>First Name: {model.FirstName}, </a>" +
                    $"<a>Last Name: {model.LastName}, </a>" +
                    $"Phone number: {model.PhoneNumber}, \n" +
                    $"Username: {model.Username}, \n" +
                    $"Email: {model.Email}, \n" +
                    $"<b>Create the following client with the information given.</b></a>");

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

        public async Task<IActionResult> UserList()
        {
            var users = await _userHelper.GetAllUsersAsync();

            // Create a URL that includes the current URL as a query parameter
            var currentUrl = Url.Action("UserList", "Account", null, Request.Scheme);

            // Get all roles to be used in the view
            var allRoles = await _roleManager.Roles.ToListAsync();

            var usersWithRoles = new List<UserWithRolesViewModel>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                // Map roles to their names and filter out empty names
                var roleNames = roles.Select(role => allRoles.FirstOrDefault(r => r.Name == role)?.Name)
                                     .Where(name => !string.IsNullOrEmpty(name))
                                     .ToList();

                usersWithRoles.Add(new UserWithRolesViewModel
                {
                    User = user,
                    Roles = roleNames,
                    UserListUrl = currentUrl // Pass the URL to the view model
                });
            }

            return View(usersWithRoles);
        }
        public async Task<IActionResult> ChangeUser()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            var model = new ChangeUserViewModel();
            if (user != null)
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUser(ChangeUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    var response = await _userHelper.UpdateUserAsync(user);
                    if (response.Succeeded)
                    {
                        ViewBag.UserMessage = "User updated";
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
