using EPiServer.Find.Personalization.Models;
using EPiServer.Reference.Commerce.Site.Features.Blocks.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.PersonalisedFind.Models.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Features.PersonalisedFind.Models.ViewModels
{
    public class PersonalisedFindAttributesViewModel : BlockViewModel<PersonalisedFindAttributesBlock>
    {
        public IEnumerable<PreferenceAttributeData> CurrentPersonalizationAttributes { get; set; }

        public PersonalisedFindAttributesViewModel(PersonalisedFindAttributesBlock block) : base(block)
        {
        }
    }
}