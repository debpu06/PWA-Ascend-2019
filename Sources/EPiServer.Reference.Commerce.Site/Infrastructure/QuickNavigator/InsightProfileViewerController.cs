using System.Web.Mvc;
using InsightFormFieldMapper.Interfaces;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.QuickNavigator
{
    [Authorize(Roles = "WebAdmins,Administrators")]
    public class InsightProfileViewerController : Controller
    {
        private readonly IProfileStoreConfig _profileStoreConfig;
        private string resourceGetProfile = "api/v1.0/Profiles";

        public InsightProfileViewerController(IProfileStoreConfig profileStoreConfig)
        {
            _profileStoreConfig = profileStoreConfig;
        }

        public ActionResult ViewProfile()
        {
            string url = "/episerver/profiles/";

            try
            {
                var profile = GetProfile(Request.Cookies["_madid"]?.Value);
                url += "#profiles/" + profile["ProfileId"];
            }
            catch
            {
                // ignored 
            }
            return new RedirectResult(url, false);
        }

        private JToken GetProfile(string deviceId)
        {
            // Set up the request
            var client = new RestClient(_profileStoreConfig.RootApiUrl);
            var request = new RestRequest(resourceGetProfile, Method.GET);
            request.AddHeader("Authorization", $"epi-single {_profileStoreConfig.SubscriptionKey}");

            // Filter the profiles based on the current device id
            request.AddParameter("$filter", "DeviceIds eq " + deviceId);

            // Execute the request to get the profile
            var getProfileResponse = client.Execute(request);
            var getProfileContent = getProfileResponse.Content;

            // Get the results as a JArray object
            var profileResponseObject = JObject.Parse(getProfileContent);
            var profileArray = (JArray)profileResponseObject["items"];

            // Expecting an array of profiles with one item in it
            var profileObject = profileArray.First;

            return profileObject;
        }
    }
}