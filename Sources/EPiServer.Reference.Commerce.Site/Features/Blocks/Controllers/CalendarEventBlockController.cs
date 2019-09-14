using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Reference.Commerce.Site.Features.Alloy.Services;
using EPiServer.Reference.Commerce.Site.Features.Blocks.Models;
using EPiServer.Reference.Commerce.Site.Features.Blocks.ViewModels;
using EPiServer.Web.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Blocks.Models
{
    [TemplateDescriptor(Default = true)]
    public class CalendarEventBlockController : BlockController<CalendarEventBlock>
    {
        private ContentLocator contentLocator;
        private IContentLoader contentLoader;
        public CalendarEventBlockController(ContentLocator contentLocator, IContentLoader contentLoader)
        {
            this.contentLocator = contentLocator;
            this.contentLoader = contentLoader;
        }

        public override ActionResult Index(CalendarEventBlock currentBlock)
        {
            var events = FindEvents(currentBlock);

            if (currentBlock.ViewMode.Equals("List"))
            {
                events = events.Where(x => DateTime.Parse(x.GetPropertyValue("StartDate")) >= DateTime.Now).OrderBy(x => x.GetPropertyValue("StartDate")).Take(currentBlock.Count);
            }

            var model = new CalendarEventBlockModel(currentBlock)
            {
                Events = events
            };

            ViewData.GetEditHints<CalendarEventBlockModel, CalendarEventBlock>()
                .AddConnection(x => x.ViewMode, x => x.ViewMode);

            if (currentBlock.ViewMode.Equals("List"))
            {
                return PartialView("~/Views/CalendarEventBlock/Agenda.cshtml", model);
            }
            else
            {
                return PartialView(model);
            }
        }

        private IEnumerable<PageData> FindEvents(CalendarEventBlock currentBlock)
        {
            IEnumerable<PageData> events;
            var root = currentBlock.Root;
            if (currentBlock.Recursive)
            {
                events = contentLocator.GetAll<PageData>(root);
            }
            else
            {
                events = contentLoader.GetChildren<PageData>(root);
            }

            if (currentBlock.CategoryFilter != null && currentBlock.CategoryFilter.Any())
            {
                events = events.Where(x => x.Category.Intersect(currentBlock.CategoryFilter).Any());
            }
            return events;
        }
    }
}
