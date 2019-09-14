using System.Web.Mvc;
using EPiServer.Reference.Commerce.Site.Features.Blocks.Models;
using EPiServer.Reference.Commerce.Site.Features.Blocks.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.VirtualProducts.Models;
using EPiServer.Web.Mvc;
using Mediachase.Commerce.Customers;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Controllers
{
    public class ElevatedRoleBlockController : BlockController<ElevatedRoleBlock>
    {
        public override ActionResult Index(ElevatedRoleBlock currentBlock)
        {
            var viewModel = new ElevatedRoleViewModel(currentBlock);
            var currentContact = CustomerContext.Current.CurrentContact;
            if (currentContact != null)
            {
                var contact = new ElevatedRoleContact(currentContact);
                if(contact.ElevatedRole == ElevatedRoles.Reader.ToString())
                {
                    viewModel.IsAccess = true;
                }
            }
            return PartialView("~/Views/Blocks/ElevatedRoleBlock.cshtml", viewModel);
        }
    }
}
