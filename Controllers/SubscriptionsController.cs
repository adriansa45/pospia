using POS.Models;
using POS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace POS.Controllers
{
    [Authorize]
    public class SubscriptionsController : Controller
    {
        private readonly IServiceUser serviceUser;
        private readonly IAccountsRepository accountsRepository;
        private readonly ISubscriptionsRepository subscriptionsRepository;
        private readonly IDashboardService dashboardService;

        public SubscriptionsController(IServiceUser serviceUser,
                                     IAccountsRepository accountsRepository,
                                     ISubscriptionsRepository subscriptionsRepository,
                                     IDashboardService dashboardService)
        {
            this.serviceUser = serviceUser;
            this.accountsRepository = accountsRepository;
            this.subscriptionsRepository = subscriptionsRepository;
            this.dashboardService = dashboardService;
        }


        public async Task<IActionResult> Index()
        {
            var userId = serviceUser.GetUserId();
            var accountId = await accountsRepository.GetAccountIdByUserId(userId);
            var subscriptions = await subscriptionsRepository.GetSubscriptions(accountId);
            return View(subscriptions);
        }
        [HttpGet]
        public IActionResult Create()
        {
            var model = new Subscription();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Subscription sub)
        {
            var userId = serviceUser.GetUserId();
            var accountId = await accountsRepository.GetAccountIdByUserId(userId);
            sub.AccountId = accountId;

            if (!ModelState.IsValid)
            {
                return View(sub);
            }

            sub.Date = sub.Date.Add(DateTime.Now.TimeOfDay);

            await subscriptionsRepository.CreateSubscription(sub);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = serviceUser.GetUserId();
            var accountId = await accountsRepository.GetAccountIdByUserId(userId);
            var subscription = await subscriptionsRepository.GetSubscriptionById(accountId, id);

            if (subscription is null)
            {
                return RedirectToAction("NoFound", "Home");
            }

            return View(subscription);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Subscription subscription)
        {
            var userId = serviceUser.GetUserId();
            var accountId = await accountsRepository.GetAccountIdByUserId(userId);
            var sub = await subscriptionsRepository.GetSubscriptionById(accountId, subscription.Id);

            if (sub is null)
            {
                return RedirectToAction("NoFound", "Home");
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction("NoFound", "Home");
            }

            var amount = (subscription.Amount / subscription.IntervalDay) * 30;
            await subscriptionsRepository.EditSubscription(subscription);
            await dashboardService.UpdateDashboardBillSubscriptions(userId, amount, sub.Amount);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = serviceUser.GetUserId();
            var accountId = await accountsRepository.GetAccountIdByUserId(userId);
            var subscription = await subscriptionsRepository.GetSubscriptionById(accountId, id);

            if (subscription is null)
            {
                return RedirectToAction("NoFound", "Home");
            }

            return View(subscription);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSubscription(int id)
        {
            var userId = serviceUser.GetUserId();
            var accountId = await accountsRepository.GetAccountIdByUserId(userId);
            var subscription = await subscriptionsRepository.GetSubscriptionById(accountId, id);

            if (subscription is null)
            {
                return RedirectToAction("NoFound", "Home");
            }

            await subscriptionsRepository.Delete(id);
            await dashboardService.UpdateDashboardBillSubscriptions(userId, 0, subscription.Amount);
            return RedirectToAction("Index");
        }
    }
}
