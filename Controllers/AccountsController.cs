using POS.Models;
using POS.Services;
using Microsoft.AspNetCore.Mvc;

namespace POS.Controllers
{
    public class AccountsController: Controller
    {
        private readonly IAccountsRepository accountsRepository;
        private readonly IUserRepository userRepository;
        private readonly IServiceUser serviceUser;
        private readonly ITransactionsRepository transactionsRepository;
        private readonly IDashboardService dashboardService;

        public AccountsController(IAccountsRepository accountsRepository,
                                  IUserRepository userRepository,
                                  IServiceUser serviceUser,
                                  ITransactionsRepository transactionsRepository,
                                  IDashboardService dashboardService)
        {
            this.accountsRepository = accountsRepository;
            this.userRepository = userRepository;
            this.serviceUser = serviceUser;
            this.transactionsRepository = transactionsRepository;
            this.dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = serviceUser.GetUserId();
            var user = await userRepository.GetUserById(userId);
            var account = await accountsRepository.GetAccountByUserId(userId);
            var name = user.Name.Split(' ');

            if (user == null || account == null) 
            { 
                return RedirectToAction("NoFound","Home");
            }

            var profile = new Profile() {
                AccountId = account.Id,
                Name = name[0],
                LastName = name.Length == 1 ? name[0] : name[1],
                Email = user.Email,
                Balance = account.Balance,
                Salary = account.Salary,
                NextPay = account.PayDay,
                IntervalDays = account.Interval_Days
            };
            return View(profile);
        }

        public async Task<IActionResult> Edit()
        {
            var userId = serviceUser.GetUserId();
            var user = await userRepository.GetUserById(userId);
            var account = await accountsRepository.GetAccountByUserId(userId);
            var name = user.Name.Split(' ');

            if (user == null || account == null)
            {
                return RedirectToAction("NoFound", "Home");
            }

            var profile = new Profile()
            {
                AccountId = account.Id,
                Name = name[0],
                LastName = name[0],
                Email = user.Email,
                Balance = account.Balance,
                Salary = account.Salary,
                NextPay = account.PayDay,
                IntervalDays = account.Interval_Days
            };
            return View(profile);
        }

        [HttpPost]
        public async Task< IActionResult> Edit(Profile profile)
        {
            var userId = serviceUser.GetUserId();
            var accountOld = await accountsRepository.GetAccountByUserId(userId);
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (accountOld == null)
            {
                return RedirectToAction("NoFound", "Home");
            }

            var account = new Account()
            {
                Id = profile.AccountId,
                Balance = profile.Balance,
                Salary = profile.Salary,
                PayDay = profile.NextPay,
                Interval_Days = profile.IntervalDays
            };

            await accountsRepository.UpdateProfile(account);

            if (account.Balance != accountOld.Balance)
            {
                var newBalance = accountOld.Balance - account.Balance;
                var transaction = new Transaction()
                {
                    AccountId = account.Id,
                    Name = "Balance Correction",
                    Amount = newBalance,
                    Date = DateTime.Now,
                    Category = 7
                    
                };

                await transactionsRepository.AddTransaction(transaction);
                await dashboardService.UpdateDashboard(userId, newBalance, 0);
            }

            return RedirectToAction("Index", "Accounts");
        }
    }
}
