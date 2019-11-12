using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Numero.Data;
using Numero.Helpers;
using Numero.Model;
using System.Linq;

namespace Numero.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class WalletController:ControllerBase
    {
        public UserManager<ApplicationUser> _userManager;

        private ApplicationDbContext _dbContext;

        public WalletController(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;

            _dbContext = dbContext;

        }

        //get user wallet details
        [HttpGet]
        public async Task<IActionResult> GetWallet()
        {
            var loggedinUser = _userManager.GetUserId(User);

            var userDetails = await _userManager.FindByEmailAsync(loggedinUser);

            var getUserId = userDetails.Id;

            var wallet = _dbContext.User_Wallet.FirstOrDefault(u => u.UserId == getUserId);

            return Ok(new {
                        ActualAmount = wallet.ActualAmount,
                        InitialAmount = wallet.InitialAmount,
                        message = "Users Wallet Details",
                        User = wallet.User.Email
            });

        }

    }
}
