using EPiServer.Core;
using EPiServer.Reference.Commerce.B2B.Filters;
using EPiServer.Reference.Commerce.B2B.Models.ViewModels;
using EPiServer.Reference.Commerce.B2B.ServiceContracts;
using EPiServer.Reference.Commerce.Site.Features.Profile.Pages;
using EPiServer.Reference.Commerce.Site.Features.Profile.Services;
using EPiServer.Reference.Commerce.Site.Features.Profile.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Site.Features.Shared.Services;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;
using EPiServer.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Profile.Controllers
{
    /// <summary>
    /// Manage credit cards of user and organization
    /// </summary>
    [Authorize]
    public class CreditCardController : PageController<CreditCardPage>
    {
        private readonly IContentLoader _contentLoader;
        private readonly ICreditCardService _creditCardService;
        private readonly IOrganizationService _organizationService;
        private readonly ICustomerService _customerService;

        /// <summary>
        /// Construct credit card controller
        /// </summary>
        /// <param name="contentLoader">Service to load content</param>
        /// <param name="creditCardService">Service to manipulate credit card data</param>
        /// <param name="organizationService">Service to manipulate organization data</param>
        /// <param name="customerService">Service to manipute </param>
        /// <param name="controllerExceptionHandler">Controller to handle exception</param>
        public CreditCardController(
            IContentLoader contentLoader,
            ICreditCardService creditCardService,
            IOrganizationService organizationService,
            ICustomerService customerService,
            ControllerExceptionHandler controllerExceptionHandler
            )
        {
            _contentLoader = contentLoader;
            _creditCardService = creditCardService;
            _organizationService = organizationService;
            _customerService = customerService;
        }

        #region List

        /// <summary>
        /// List all credit card of current user
        /// </summary>
        /// <param name="currentPage">Current credit card page</param>
        /// <returns></returns>
        public ActionResult Index(CreditCardPage currentPage)
        {
            return currentPage.B2B ? B2B(currentPage) : List(currentPage);
        }

        /// <summary>
        /// List all credit card of current user, with b2b navigation on view
        /// </summary>
        /// <param name="currentPage">Current credit card page</param>
        /// <returns></returns>
        [NavigationAuthorize("Admin")]
        public ActionResult B2B(CreditCardPage currentPage)
        {
            return List(currentPage);
        }

        /// <summary>
        /// List all credit card of current user
        /// </summary>
        /// <param name="currentPage">Current credit card page</param>
        /// <returns></returns>
        private ActionResult List(CreditCardPage currentPage)
        {
            var model = new CreditCardCollectionViewModel
            {
                CurrentContent = currentPage,
                CreditCards = new List<CreditCardModel>(),
                CurrentContact = _customerService.GetCurrentContact(),
                IsB2B = currentPage.B2B
            };
            model.CreditCards = _creditCardService.List(currentPage.B2B, false);
            return View("Index", model);
        }

        #endregion

        #region Remove
        /// <summary>
        /// Remove credit card by id
        /// </summary>
        /// <param name="creditCardId">Credit card id</param>
        /// <returns></returns>
        [HttpPost]
        [AllowDBWrite]
        public ActionResult Remove(string creditCardId)
        {
            _creditCardService.Delete(creditCardId);
            return RedirectToAction("Index");
        }
        #endregion

        #region Add-Edit

        /// <summary>
        /// Add/Edit Credit card of current customer or current organization
        /// </summary>
        /// <param name="currentPage">Current credit card page</param>
        /// <param name="creditCardId">Credit card id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditForm(CreditCardPage currentPage, string creditCardId)
        {
            return currentPage.B2B ? CreditCardEditViewB2B(currentPage, creditCardId) : CreditCardEditView(currentPage, creditCardId);
        }

        /// <summary>
        /// Add/Edit Credit card of current customer or current organization
        /// </summary>
        /// <param name="currentPage">Current credit card page</param>
        /// <param name="creditCardId">Credit card id</param>
        /// <returns></returns>
        [NavigationAuthorize("Admin")]
        private ActionResult CreditCardEditViewB2B(CreditCardPage currentPage, string creditCardId)
        {
            return CreditCardEditView(currentPage, creditCardId);
        }

        /// <summary>
        /// Add/Edit Credit card of current customer or current organization
        /// </summary>
        /// <param name="currentPage">Current credit card page</param>
        /// <param name="creditCardId">Credit card id</param>
        /// <returns></returns>
        private ActionResult CreditCardEditView(CreditCardPage currentPage, string creditCardId)
        {
            CreditCardViewModel viewModel = new CreditCardViewModel
            {
                CreditCard = new CreditCardModel
                {
                    CreditCardId = creditCardId
                },
                CurrentContent = currentPage,
                IsB2B = currentPage.B2B
            };

            if (currentPage.B2B)
            {
                viewModel.Organizations = viewModel.GetAllOrganizationAndSub(_organizationService.GetCurrentUserOrganization());
            }

            String errorMessage;
            if (_creditCardService.IsValid(viewModel.CreditCard.CreditCardId, out errorMessage))
            {
                _creditCardService.LoadCreditCard(viewModel.CreditCard);
            }
            else
            {
                viewModel.ErrorMessage = errorMessage;
            }

            return View("EditForm", viewModel);
        }


        /// <summary>
        /// Save credit card
        /// </summary>
        /// <param name="viewModel">data model of credit card</param>
        /// <returns></returns>
        [HttpPost]
        [AllowDBWrite]
        public ActionResult Save(CreditCardViewModel viewModel)
        {
            _creditCardService.Save(viewModel.CreditCard);
            return RedirectToAction("Index");
        }

        #endregion

        #region Other
        private BaseStartPage GetStartPage()
        {
            return _contentLoader.Get<BaseStartPage>(ContentReference.StartPage);
        }

        #endregion

    }
}