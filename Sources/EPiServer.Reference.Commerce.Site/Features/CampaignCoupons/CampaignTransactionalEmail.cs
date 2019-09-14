using System;
using System.IO;
using System.Net;
using System.Text;
using EPiServer.Logging;
using EPiServer.ServiceLocation;

namespace EPiServer.Reference.Commerce.Site.Features.CampaignCoupons
{
    [ServiceConfiguration(typeof(ICampaignTransactionalEmail))]
    public class CampaignTransactionalEmail : ICampaignTransactionalEmail
    {
        private static readonly ILogger Logger = LogManager.GetLogger();

        public bool SendTrasactionalEmail(string authorisationCode, string email, string mailingId)
        {
            string apiUrl = "http://api.broadmail.de/http/form/{0}/sendtransactionmail?bmRecipientId={1}&bmMailingId={2}";
            apiUrl = string.Format(apiUrl, authorisationCode, email, mailingId);
            string response = ApiRequest(apiUrl);
            return response.Contains("enqueued");
        }

        public bool SendTrasactionalEmail(string authorisationCode, string email, string mailingId, string htmlEmailContent)
        {
            string apiUrl = "http://api.broadmail.de/http/form/{0}/sendtransactionmail";
            string postData = "bmRecipientId={0}&bmMailingId={1}&receipt={2}";
            apiUrl = string.Format(apiUrl, authorisationCode);
            postData = string.Format(postData, Uri.EscapeDataString(email), mailingId, Uri.EscapeUriString(htmlEmailContent));
            string response = ApiRequest(apiUrl, postData, "POST");
            return response.Contains("enqueued");
        }

        private string ApiRequest(string url, string postData = null, string requestMethod = WebRequestMethods.Http.Get)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = requestMethod;
            if (request.Method == WebRequestMethods.Http.Post)
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                // Set the ContentType property of the WebRequest.  
                request.ContentType = "application/x-www-form-urlencoded";
                // Set the ContentLength property of the WebRequest.  
                request.ContentLength = byteArray.Length;
                // Get the request stream.  
                Stream dataStream = request.GetRequestStream();
                // Write the data to the request stream.  
                dataStream.Write(byteArray, 0, byteArray.Length);
            }
            string responseText = string.Empty;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return responseText;
                }
                try
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    responseText = reader.ReadToEnd();
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error Reading response content: {ex.Message}", new object[0]);
                    return responseText;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, new object[0]);
            }
            return responseText;
        }
    }
}