using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Numero.Data;
using Numero.Email;
using Numero.Helpers;
using Numero.Model;

namespace Numero.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController:Controller
    {
        public UserManager<ApplicationUser> _userManager;

        private readonly AppSettings _appsettings;

        private ApplicationDbContext _dbContext;

        private IEmailSender _emailSender;

        public AuthController(UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings, IEmailSender emailSender, IOptions<Config> connection, ApplicationDbContext dbContext)
        {
            _userManager = userManager;

            _appsettings = appSettings.Value;

            _dbContext = dbContext;

            _emailSender = emailSender;

        }

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] RegisterModel model)
        {
            Random rnd = new Random();
            var otp = rnd.Next(100000, 999999).ToString();


            List<string> errorList = new List<string>();

            var user = new ApplicationUser
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                VerificationToken = otp
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");

               
                //create wallet account default for users on creation and send email to users
                var userDetails = await _userManager.FindByEmailAsync(model.Email);

                var userFirstname = userDetails.FirstName;

                await _emailSender.SendEmailAsync(user.Email, userFirstname, otp);                

                var userid = new UserWallet
                {
                    UserId = userDetails.Id,
                    InitialAmount = 0.00M,
                    ActualAmount = 0.00M
                };

                _dbContext.User_Wallet.Add(userid);

                _dbContext.SaveChanges();

                return Ok(new { Username = user.UserName, email = user.Email, status = 1, message = "User Created" });               
            }

            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    errorList.Add(error.Description);
                }

            }
            return BadRequest(new JsonResult(errorList));
        }


        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                // check if Email is confirmed
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError(string.Empty, "User has not confirmed Email");
                    return Unauthorized(new { LoginError = "Confirm Token To Complete Registration" });
                }

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appsettings.Secret));
                int expiryInMinutes = Convert.ToInt32(_appsettings.ExpireTime);

                var token = new JwtSecurityToken(
                    issuer: _appsettings.Site,
                    audience: _appsettings.Audience,
                    expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
                    claims: claims,
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }

            return Unauthorized();
        }


        [HttpPost("Verify")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromBody] VerifyToken model)
        {

            var token = _userManager.Users
                .Where(u => u.Email == model.Email)
                .Select(u => u.VerificationToken)
                .SingleOrDefault();


            if (token != model.Token)
            {
                return BadRequest("The Token Supplied Doesn't Match");

            }

            var userConfirmEmail = await _userManager.FindByEmailAsync(model.Email);

            userConfirmEmail.EmailConfirmed = true;

            await _userManager.UpdateAsync(userConfirmEmail);

            return new JsonResult("Account Confirmed Login To Continue Transaction");
        }


        [HttpPost]
        [Route("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(model.Email);


            if(user == null)
            {
                return BadRequest("User Doesnt Exist");
            }


            var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResetUrl = Url.Action("ResetPassword", "auth", new { id = user.Id, token = passwordResetToken }, Request.Scheme);

            await _emailSender.SendEmailAsync(model.Email, "Password reset", $"Click <a href=\"" + passwordResetUrl + "\">here</a> to reset your password");

            return Content("Check your email for a password reset link");
         
        }




        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string id, string token, string password, string repassword)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new InvalidOperationException();

            if (password != repassword)
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match");
                return View();
            }

            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, token, password);
            if (!resetPasswordResult.Succeeded)
            {
                foreach (var error in resetPasswordResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View();
            }

            return Content("Password updated");
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult ForgotPassword()
        {
            return View();
   
        }


        [HttpPost]
        [Route("ResendOtp")]
        public async Task<IActionResult> ResetOtp(ResendOtp model)
        {
            Random rnd = new Random();
            var otp = rnd.Next(100000, 999999).ToString();


            await _emailSender.SendEmailAsync(model.Email, "User", otp);

            var userResendToken = await _userManager.FindByEmailAsync(model.Email);

            userResendToken.VerificationToken = otp;

            await _userManager.UpdateAsync(userResendToken);

            return new JsonResult("Otp Resent. Check Mail To Confirm.");
        }

    
        [HttpPost]
        [Route("Business")]
        public async Task<IActionResult> BusinessDetails(BusinessDetails bmodel)
        {
            var userDetails = await _userManager.FindByEmailAsync(bmodel.Email);

            var userId = userDetails.Id;

            if(userId != null)
            {
                
                return BadRequest(bmodel.Email + "Alread has a Business name");
            }

            else
            {
                var businessDetail = new BusinessDetails
                {
                    UserId = userId,
                    Email = bmodel.Email,
                    BusinessSector = bmodel.BusinessSector,
                    RcNumber = bmodel.RcNumber,
                    BusinessEmail = bmodel.BusinessEmail,
                    Address = bmodel.Address,
                    BusinesssName = bmodel.BusinesssName,
                    EmployeeNumber = bmodel.EmployeeNumber,
                    DirectorsNumber = bmodel.DirectorsNumber
                };

                _dbContext.BusinessDetail.Add(businessDetail);
                _dbContext.SaveChanges();

                return Ok("Business Details Created");
            }
            
        }



        [HttpPost]
        [Route("UpdateBusiness")]
        [Authorize]
        public async Task<IActionResult> UpdateBusinessDetails(BusinessDetailsUpdate bmodel)
        {
            var loggedinUser = _userManager.GetUserId(User);

            var userDetails = await _userManager.FindByEmailAsync(loggedinUser);

            var getEmail = userDetails.Email;

            if (getEmail == null)
            {
                return BadRequest(getEmail + "Does not have a Business Record");
            }

            var updated=  _dbContext.BusinessDetail.Where(o => o.Email == getEmail).FirstOrDefault();

            if(updated != null)
            {
                updated.BusinessSector = bmodel.BusinessSector;
                updated.RcNumber = bmodel.RcNumber;
                updated.BusinessEmail = bmodel.BusinessEmail;
                updated.Address = bmodel.Address;
                updated.BusinesssName = bmodel.BusinesssName;
                updated.EmployeeNumber = bmodel.EmployeeNumber;
                updated.DirectorsNumber = bmodel.DirectorsNumber;
            }
                _dbContext.SaveChanges();

            return Ok("Business Details Updated");
            
        }


    }
}



