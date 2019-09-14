using EPiServer.Core.PropertySettings;
using EPiServer.Editor.TinyMCE;
using EPiServer.Reference.Commerce.Shared.Extensions;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using System;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.PropertySettings
{
    [ServiceConfiguration(ServiceType = typeof(Core.PropertySettings.PropertySettings))]
    public class DefaultTinyMCEConfiguration : PropertySettings<TinyMCESettings>
    {
        public DefaultTinyMCEConfiguration()
        {
            IsDefault = true;
            DisplayName = "Default Settings";
            Description = "Default settings from code";
        }

        public override Guid ID => new Guid("e5c3a320-5565-4275-964e-1b3e823d0893");

        public override TinyMCESettings GetPropertySettings()
        {
            var settings = new TinyMCESettings();
            settings.ToolbarRows.Add(new ToolbarRow(new[] { TinyMCEButtons.EPiServerLink, TinyMCEButtons.Unlink, TinyMCEButtons.Image, TinyMCEButtons.EPiServerImageEditor, TinyMCEButtons.Media, TinyMCEButtons.EPiServerPersonalizedContent, TinyMCEButtons.Separator, TinyMCEButtons.Cut, TinyMCEButtons.Copy, TinyMCEButtons.Paste, TinyMCEButtons.PasteWord, TinyMCEButtons.Separator, TinyMCEButtons.Fullscreen }));
            settings.ToolbarRows.Add(new ToolbarRow(new[] { TinyMCEButtons.Bold, TinyMCEButtons.Italic, TinyMCEButtons.Separator, TinyMCEButtons.BulletedList, TinyMCEButtons.NumericList, TinyMCEButtons.Separator, TinyMCEButtons.StyleSelect, TinyMCEButtons.Undo, TinyMCEButtons.Redo, TinyMCEButtons.Search }));

            settings.NonVisualPlugins.Add("advimage");
            settings.NonVisualPlugins.Add("epifilebrowser");

            if (PrincipalInfo.CurrentPrincipal.IsInRole("Administrators"))
            {
                //Chance to personalize. Let's allow administrators to access the html editor.
                settings.ToolbarRows[0].Buttons.Add(TinyMCEButtons.Code);
            }

            //WYSIWYGs on Mosey/Fabrikam should allow editors to justify text
            if (SiteDefinition.Current.IsMosey())
            {
                var secondRowButtons = settings.ToolbarRows[1].Buttons;
                secondRowButtons.Insert(secondRowButtons.IndexOf(TinyMCEButtons.StyleSelect) - 1, TinyMCEButtons.JustifyLeft);
                secondRowButtons.Insert(secondRowButtons.IndexOf(TinyMCEButtons.JustifyLeft), TinyMCEButtons.JustifyRight);
                secondRowButtons.Insert(secondRowButtons.IndexOf(TinyMCEButtons.JustifyRight), TinyMCEButtons.JustifyCenter);

                //we could probably add some site-specific editor styles here if we thought it was necessary
            }

            settings.Height = 200;
            settings.Width = 600;

            return settings;
        }
    }
}