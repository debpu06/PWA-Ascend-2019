using EPiServer.Reference.Commerce.Site.Features.PersonalisedFind.Models.Blocks;
using EPiServer.Reference.Commerce.Site.Features.PersonalisedFind.Models.ViewModels;
using EPiServer.Web.Mvc;
using System.Web.Mvc;
using EPiServer.Find;
using EPiServer.Find.Personalization;

namespace EPiServer.Reference.Commerce.Site.Features.PersonalisedFind.Controllers
{
    public class PersonalisedFindAttributesBlockController : BlockController<PersonalisedFindAttributesBlock>
    {
        private readonly IClient _client;
        public PersonalisedFindAttributesBlockController(IClient client)
        {
            _client = client;
            _client.Personalization().Refresh();
        }

        public override ActionResult Index(PersonalisedFindAttributesBlock currentBlock)
        {
            var model = new PersonalisedFindAttributesViewModel(currentBlock);

            try
            {
                var prefData = _client.Personalization().Conventions.PreferenceRepository.Load();
                var attributes = prefData.Attributes;
                if (prefData?.Attributes != null)
                {
                    model.CurrentPersonalizationAttributes = attributes;
                }
            }
            catch { }
         
            return PartialView(model);
        }
    }
}
