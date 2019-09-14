using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content.Services
{
    [ServiceConfiguration(typeof(LanguageSelectorService))]
    public class LanguageSelectorService
    {
        /// <summary>
        /// Create an instance of LanguageSelector with a given language along with fallback option. 
        /// If language is null, create a language selector with Master language.
        /// If language is not null, create a language selector with LanguageSelector.Fallback
        /// </summary>
        public virtual LanguageSelector CreateFallbackSelector(string language, bool shouldUseMasterIfFallbackNotExist = false)
        {
            //     (1)  LanguageSelector with Master means that content in master language should be returned as expected.
            //     (2)  LanguageSelector.Fallback(language, shouldUseMasterIfFallbackNotExist)
            //              means that the content in fallback language should be returned if content does not exist in the given language
            //              and in case (content does not exist in both given language and fallback), if shouldUseMasterIfFallbackNotExist = true, 
            //              then, content in Master should be obtained. 

            // Ex: CreateFallbackSelector("en") will return a selector with language is "en" with the fallback option enabled. 
            // and then this selector is passed to ContentLoader.Get() (or other api callers), the ContentLoader
            // will try to get the content in "en". If the content does not have any version in "en" published, the loader will get
            // the content in fallback language of "en". If nothing exists and shouldUseMasterIfFallbackNotExist = true, ContentLoader will try to get the content 
            // in master langage. 
            return string.IsNullOrWhiteSpace(language) ? LanguageSelector.MasterLanguage() : LanguageSelector.Fallback(language, shouldUseMasterIfFallbackNotExist);
        }
    }
}