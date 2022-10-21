using POS.Models;
using POS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace POS.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly IServiceUser serviceUser;
        private readonly ITransactionsRepository transactionsRepository;
		private readonly IAccountsRepository accountsRepository;
        private readonly IDashboardService dashboardService;
        private readonly ICategoriesRepository categoryRepository;

        public TransactionsController(IServiceUser serviceUser,
                                     ITransactionsRepository transactionsRepository,
                                     IAccountsRepository accountsRepository,
                                     IDashboardService dashboardService,
                                     ICategoriesRepository categoryRepository )
        {
            this.serviceUser = serviceUser;
            this.transactionsRepository = transactionsRepository;
			this.accountsRepository = accountsRepository;
            this.dashboardService = dashboardService;
            this.categoryRepository = categoryRepository;
        }

        
        public async Task<IActionResult> Index()
        {
            var userId = serviceUser.GetUserId();
            var accountId = await accountsRepository.GetAccountIdByUserId(userId);
            var transations = await transactionsRepository.GetTransactions(accountId);
            return View(transations);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = serviceUser.GetUserId();
            var model = new TransactionViewModel();
            model.Categories = await GetCategoriesList(userId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TransactionViewModel transaction)
        {
            var userId = serviceUser.GetUserId();
            var accountId = await accountsRepository.GetAccountIdByUserId(userId);
            transaction.AccountId = accountId;

            if (!ModelState.IsValid)
            {
                return View(transaction);
            }

            transaction.Date = transaction.Date.Add(DateTime.Now.TimeOfDay);

            await transactionsRepository.AddTransaction(transaction);
            await accountsRepository.UpdateBalance(accountId, transaction.Amount, 0);
            await dashboardService.UpdateDashboard(userId, transaction.Amount, 0);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = serviceUser.GetUserId();
            var accountId = await accountsRepository.GetAccountIdByUserId(userId);
            var transaction = await transactionsRepository.GetTransactionById(accountId, id);


            if (transaction is null)
            {
                return RedirectToAction("NoFound", "Home");
            }

            var model = new TransactionViewModel()
            {
                Id = transaction.Id,
                AccountId = accountId,
                Name = transaction.Name,
                Amount = transaction.Amount,
                Date = transaction.Date,
                Category = transaction.Category,
                Categories = await GetCategoriesList(userId, transaction.Category)
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TransactionViewModel transactionView)
        {
            var userId = serviceUser.GetUserId();
            var accountId = await accountsRepository.GetAccountIdByUserId(userId);
            var transaction = await transactionsRepository.GetTransactionById(accountId, transactionView.Id);

            if (transaction is null)
            {
                return RedirectToAction("NoFound", "Home");
            }

            decimal oldAmount = transaction.Amount;

            if (!ModelState.IsValid)
            {
                return RedirectToAction("NoFound", "Home");
            }

            await transactionsRepository.EditTransaction(transactionView);
            await accountsRepository.UpdateBalance(accountId, transactionView.Amount, oldAmount);
            await dashboardService.UpdateDashboard(userId, transactionView.Amount, oldAmount);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = serviceUser.GetUserId();
            var accountId = await accountsRepository.GetAccountIdByUserId(userId);
            var transaction = await transactionsRepository.GetTransactionById(accountId, id);

            if (transaction is null)
            {
                return RedirectToAction("NoFound", "Home");
            }

            return View(transaction);
        } 

        [HttpPost]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var userId = serviceUser.GetUserId();
            var accountId = await accountsRepository.GetAccountIdByUserId(userId);
            var transaction = await transactionsRepository.GetTransactionById(accountId, id);

            if (transaction is null)
            {
                return RedirectToAction("NoFound", "Home");
            }

            await transactionsRepository.Delete(id);
            await accountsRepository.DeleteTransactionToAccount(accountId, transaction.Amount);
            await dashboardService.UpdateDashboard(userId, 0, transaction.Amount);
            return RedirectToAction("Index");
        }


        public async Task<IEnumerable<SelectListItem>> GetCategoriesList(int userId, int categoryId)
        {
            var categories = await categoryRepository.GetCategories(userId);
            Console.WriteLine(categoryId);
            return categories.Select(c => new SelectListItem (c.Name, c.Id.ToString(), categoryId == c.Id));
        }

        public async Task<IEnumerable<SelectListItem>> GetCategoriesList(int userId)
        {
            var categories = await categoryRepository.GetCategories(userId);
            return categories.Select(c => new SelectListItem(c.Name, c.Id.ToString()));
        }
    }
}
