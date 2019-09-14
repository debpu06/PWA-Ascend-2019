using System;
using System.Web.Mvc;
using EPiServer.Core;
using EPiServer.Web.Mvc;
using EPiServer.Reference.Commerce.B2B.ServiceContracts;
using EPiServer.Reference.Commerce.Site.Features.MoseySupply.Models;
using EPiServer.Reference.Commerce.B2B.Models.Entities;
using EPiServer.Reference.Commerce.B2B.Models.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Site.Features.MoseySupply.ViewModels;

namespace EPiServer.Reference.Commerce.Site.Features.MoseySupply.Controllers
{
    [Authorize]
    public class MoseySupplyProfilePageController : PageController<MoseySupplyProfilePage>
    {
        private readonly IContentLoader _contentLoader;
        private readonly ICustomerService _customerService;
        private readonly IBudgetService _budgetService;
        public MoseySupplyProfilePageController(ICustomerService customerService, IContentLoader contentLoader, IBudgetService budgetService)
        {
            _customerService = customerService;
            _contentLoader = contentLoader;
            _budgetService = budgetService;
        }

        public ActionResult Index(MoseySupplyProfilePage currentPage)
        {
            BaseStartPage startPage = _contentLoader.Get<BaseStartPage>(ContentReference.StartPage);
            var viewModel = new MoseySupplyProfilePageViewModel
            {
                CurrentContent = currentPage, Contact = _customerService.GetCurrentContact(),
                OrganizationPage = startPage.OrganizationMainPage
            };
            var userOrganization = _customerService.GetCurrentContact().Organization;
            Budget currentPersonalBudget = null;
            if (userOrganization != null && userOrganization.OrganizationId != Guid.Empty)
                currentPersonalBudget = _budgetService.GetCustomerCurrentBudget(userOrganization.OrganizationId,
                    _customerService.GetCurrentContact().ContactId);
            if (currentPersonalBudget != null)
                viewModel.CurrentPersonalBudget = new BudgetViewModel(currentPersonalBudget);
            return View(viewModel);
        }
    }
}