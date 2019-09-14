using EPiServer.Commerce.Order;
using EPiServer.Core;
using EPiServer.Reference.Commerce.B2B.Extensions;
using EPiServer.Reference.Commerce.Site.Features.AddressBook.Services;
using EPiServer.Reference.Commerce.Site.Features.Cart.Services;
using EPiServer.Reference.Commerce.Site.Features.Loyalty.Models;
using EPiServer.Reference.Commerce.Site.Features.OrderHistory.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Site.Features.Social.Common.Exceptions;
using EPiServer.Reference.Commerce.Site.Features.Social.Models.ActivityStreams;
using EPiServer.Reference.Commerce.Site.Features.Social.Repositories.ActivityStreams;
using EPiServer.Reference.Commerce.Site.Features.Social.Repositories.Common;
using EPiServer.Reference.Commerce.Site.Features.SocialProfile.Pages;
using EPiServer.Reference.Commerce.Site.Features.SocialProfile.ViewModels;
using EPiServer.Security;
using EPiServer.Social.Comments.Core;
using EPiServer.Social.Common;
using EPiServer.Web.Mvc;
using Mediachase.Commerce.Customers;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace EPiServer.Reference.Commerce.Site.Features.SocialProfile.Controllers
{
    public class SocialProfilePageController : PageController<SocialProfilePage>
    {
        private readonly IAddressBookService _addressBookService;
        private readonly IOrderRepository _orderRepository;
        private readonly ICartService _cartService;
        private readonly IUserRepository _userRepository;
        private readonly ICommunityFeedRepository _feedRepository;
        private readonly IUserImpersonation _userImpersonation;
        private readonly ICommentService _commentService;
        private const string TierLocation = "/Assets/Mosey/images/tiers/";

        public SocialProfilePageController(IAddressBookService addressBookService,
            IOrderRepository orderRepository,
            ICartService cartService,
            IUserRepository userRepository,
            ICommunityFeedRepository feedRepository,
            IUserImpersonation userImpersonation,
            ICommentService commentService)
        {
            _addressBookService = addressBookService;
            _orderRepository = orderRepository;
            _cartService = cartService;
            _userRepository = userRepository;
            _feedRepository = feedRepository;
            _userImpersonation = userImpersonation;
            _commentService = commentService;
        }

        public ActionResult Index(SocialProfilePage currentPage, string user)
        {
            var userId = $"String:{user}";
            var contact = CustomerContext.Current.GetContactByUserId(userId);
            if (contact == null)
            {
                return RedirectToAction("Index", new { node = ContentReference.StartPage });
            }
            var principal = _userImpersonation.CreatePrincipal(user);
            var loyaltyContact = new LoyaltyContact(contact);
            var model = new SocialProfilePageViewModel(currentPage)
            {
                User = user,
                LoyaltyContact = loyaltyContact,
                Orders = GetOrderHistoryViewModels(contact),
                TierUrl = GetTierUrl(loyaltyContact.Tier),
                Feeds = GetSocialActivityFeed(principal.Identity.GetUserId()),
                Comments = GetComments(user, 1, 1000)
            };

            return View(model);
        }

        public ActionResult AddAComment(SocialCommentViewModel model)
        {
            AddComment(model);
            return RedirectToAction("Index", new { user = model.User });
        }

        private List<OrderViewModel> GetOrderHistoryViewModels(CustomerContact contact)
        {
            var purchaseOrders = _orderRepository.Load<IPurchaseOrder>(new Guid(contact.PrimaryKeyId.Value.ToString()), _cartService.DefaultCartName)
                .OrderByDescending(x => x.Created).ToList();

            var viewModel = new List<OrderViewModel>();

            foreach (var purchaseOrder in purchaseOrders)
            {
                // Assume there is only one form per purchase.
                var form = purchaseOrder.GetFirstForm();
                var billingAddress = new AddressModel();
                var payment = form.Payments.FirstOrDefault();
                if (payment != null)
                {
                    billingAddress = _addressBookService.ConvertToModel(payment.BillingAddress);
                }
                var orderViewModel = new OrderViewModel
                {
                    PurchaseOrder = purchaseOrder,
                    Items = form.GetAllLineItems().Select(lineItem => new OrderHistoryItemViewModel
                    {
                        LineItem = lineItem,
                    }).GroupBy(x => x.LineItem.Code).Select(group => group.First()),
                    BillingAddress = billingAddress,
                    ShippingAddresses = new List<AddressModel>()
                };

                foreach (var orderAddress in purchaseOrder.Forms.SelectMany(x => x.Shipments).Select(s => s.ShippingAddress))
                {
                    var shippingAddress = _addressBookService.ConvertToModel(orderAddress);
                    orderViewModel.ShippingAddresses.Add(shippingAddress);
                }

                viewModel.Add(orderViewModel);
            }

            return viewModel;
        }

        private string GetTierUrl(string tier)
        {
            switch (tier)
            {
                case "Bronze":
                    return $"{TierLocation}bronze.svg";
                case "Silver":
                    return $"{TierLocation}silver.svg";
                case "Gold":
                    return $"{TierLocation}gold.svg";
                case "Platium":
                    return $"{TierLocation}platium.svg";
                case "Diamond":
                    return $"{TierLocation}diamond.svg";
                default:
                    return $"{TierLocation}classic.svg";
            }
        }

        private List<CommunityFeedItemViewModel> GetSocialActivityFeed(string userId)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    return _feedRepository.Get(new CommunityFeedFilter
                    {
                        Subscriber = userId,
                        PageSize = 10
                    }).ToList();
                }
                else
                {
                    return new List<CommunityFeedItemViewModel>();
                }
            }
            catch (SocialRepositoryException)
            {
                return null;
            }
        }

        private void AddComment(SocialCommentViewModel model)
        {
            // Instantiate a reference for the profile
            var profile = EPiServer.Social.Common.Reference.Create($"profile://{model.User}");

            // Instantiate a reference for the contributor
            var contributor = EPiServer.Social.Common.Reference.Create(model.Nickname);

            // Compose a comment representing the review
            var comment = new Comment(profile, contributor, model.Body, true);

            // Add the composite comment for the product
            _commentService.Add(comment);
        }

        private List<Comment> GetComments(string user, int page, int limit)
        {
            // Instantiate a reference for the product
            var profile = EPiServer.Social.Common.Reference.Create($"profile://{user}");

            var criteria = new Criteria<CommentFilter>
            {
                Filter = new CommentFilter
                {
                    Visibility = Visibility.Visible,
                    Parent = profile
                },
                PageInfo = new PageInfo
                {
                    PageSize = limit,
                    CalculateTotalCount = true,
                    PageOffset = (page - 1) * limit
                },
                OrderBy = new List<SortInfo>
                {
                    new SortInfo(CommentSortFields.Created, false)
                }
            };

            try
            {
                var cmts = _commentService.Get(criteria);
                return cmts.Results.ToList();
            }
            catch (SocialAuthenticationException ex)
            {
                throw new SocialRepositoryException("The application failed to authenticate with Episerver Social.", ex);
            }
            catch (MaximumDataSizeExceededException ex)
            {
                throw new SocialRepositoryException("The application request was deemed too large for Episerver Social.", ex);
            }
            catch (SocialCommunicationException ex)
            {
                throw new SocialRepositoryException("The application failed to communicate with Episerver Social.", ex);
            }
            catch (SocialException ex)
            {
                throw new SocialRepositoryException("Episerver Social failed to process the application request.", ex);
            }
        }

    }
}