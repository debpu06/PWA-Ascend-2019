using EPiServer.DataAbstraction;
using EPiServer.Forms.Core;
using EPiServer.Forms.Core.Data;
using EPiServer.Forms.Core.Models;
using EPiServer.Marketing.Testing.Core.DataClass;
using EPiServer.Marketing.Testing.Web.Repositories;
using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CmsDashboard.Models;
using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.ProfileStore;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CmsDashboard.Services
{
    [ServiceConfiguration(typeof(ICmsDashboardService))]
    public class CmsDashboardService : ICmsDashboardService
    {

        private readonly ICmsAuditor _cmsAuditor;
        private readonly IProfileStoreService _profileStoreService;
        private readonly IMarketingTestingWebRepository _marketingTestingWebRepository;
        private readonly FormRepository _formRepository;
        private readonly FormDataRepository _formDataRepository;

        public CmsDashboardService(ICmsAuditor cmsAuditor, 
            IProfileStoreService profileStoreService,
            IMarketingTestingWebRepository marketingTestingWebRepository, 
            FormRepository formRepository, 
            FormDataRepository formDataRepository)
        {
            _cmsAuditor = cmsAuditor;
            _profileStoreService = profileStoreService;
            _marketingTestingWebRepository = marketingTestingWebRepository;
            _formRepository = formRepository;
            _formDataRepository = formDataRepository;
        }

        public async Task<List<CmsDashboardModel>> GetAmountOfFormsSubmitted()
        {
            return await Task.Run(() =>
            {
                //Current Month
                var curStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var curEndDate = curStartDate.AddMonths(1).AddDays(-1);
                //Previous Month
                var preStartDate = curStartDate.AddMonths(-1);
                var preEndDate = curStartDate.AddDays(-1);

                var result = new List<CmsDashboardModel>();


                var curCount = 0;
                var preCount = 0;
                var forms = _formRepository.GetFormsInfo("en");
                foreach (var form in forms)
                {
                    var formIdentity = new FormIdentity(form.FormGuid, "en");
                    var currentMonthCount = _formDataRepository.GetSubmissionDataCount(formIdentity, curStartDate, DateTime.Now);
                    var previousMonthCount = _formDataRepository.GetSubmissionDataCount(formIdentity, preStartDate, preEndDate);
                    curCount += currentMonthCount;
                    preCount += previousMonthCount;
                }
                result.Add(new CmsDashboardModel { Key = curStartDate.ToString("MMMM, yyyy", CultureInfo.InvariantCulture), Value = curCount.ToString() });
                result.Add(new CmsDashboardModel { Key = preStartDate.ToString("MMMM, yyyy", CultureInfo.InvariantCulture), Value = curCount.ToString() });

                return result;
            });
        }

        public async Task<List<ContentTypeAudit>> GetBlockTypeAudits()
        {
            return await Task.Run(() =>
            {
                var bas = new List<ContentTypeAudit>();
                var blocks = _cmsAuditor.GetContentTypesOfType<BlockType>();
                foreach (var block in blocks)
                {
                    try
                    {
                        var ba = _cmsAuditor.GetContentTypeAudit(block.ContentTypeId, true, false);
                        bas.Add(ba);
                    }
                    catch { }
                }
                return bas.OrderByDescending(x => x.Usages.Count).Take(5).ToList();
            });
        }

        public async Task<List<ContentTypeAudit>> GetPageTypeAudits()
        {
            return await Task.Run(() =>
            {
                var pas = new List<ContentTypeAudit>();
                var blocks = _cmsAuditor.GetContentTypesOfType<PageType>();
                foreach (var block in blocks)
                {
                    try
                    {
                        var pa = _cmsAuditor.GetContentTypeAudit(block.ContentTypeId, false, true);
                        pas.Add(pa);
                    }
                    catch { }
                }
                return pas.OrderByDescending(x => x.Usages.Count).Take(5).ToList();
            });
        }

        public async Task<List<SiteAudit>> GetSiteAudits()
        {
            return await Task.Run(() =>
            {
                var sas = new List<SiteAudit>();
                var sites = _cmsAuditor.GetSiteDefinitions();
                foreach (var site in sites)
                {
                    try
                    {
                        var sa = _cmsAuditor.GetSiteAudit(site.Id);
                        sas.Add(sa);
                    }
                    catch { }
                }
                return sas;
            });
        }

        public async Task<List<VGAudit>> GetVisitorGroupAudit()
        {
            return await Task.Run(() => _cmsAuditor.GetVisitorGroups());
        }

        public async Task<List<CmsDashboardModel>> GetBounceRates()
        {
            //Current Month
            var curStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var curEndDate = curStartDate.AddMonths(1).AddDays(-1);
            //Previous Month
            var preStartDate = curStartDate.AddMonths(-1);
            var preEndDate = curStartDate.AddDays(-1);

            var currentMonthDays = Enumerable.Range(0, 1 + DateTime.Now.Subtract(curStartDate).Days)
                  .Select(offset => curStartDate.AddDays(offset))
                  .ToArray();

            var previousMonthDays = Enumerable.Range(0, 1 + preEndDate.Subtract(preStartDate).Days)
                  .Select(offset => preStartDate.AddDays(offset))
                  .ToArray();

            var result = new List<CmsDashboardModel>();
            var queryFormat = "?$top=10000&$orderBy=EventTime DESC&$filter=EventType eq epiPageView and EventTime eq {0}";

            double currentBouncedRate = 0;
            double previousBouncedRate = 0;
            foreach (var day in currentMonthDays)
            {
                var queryString = string.Format(queryFormat, day.ToString("yyyy-MM-dd"));
                var items = await _profileStoreService.GetVisualizationItems(queryString);
                if (items.VisualizationModels != null)
                {
                    var data = items.VisualizationModels.GroupBy(o => o.DeviceId).Select(g => new { g.Key, NumberOfItems = g.Count() });
                    var total = data.Count();
                    var bounced = data.Count(o => o.NumberOfItems == 1);
                    var bouncedRate = (double)bounced / total * 100;
                    currentBouncedRate += bouncedRate;
                }
            }
            result.Add(new CmsDashboardModel { Key = curStartDate.ToString("MMMM, yyyy", CultureInfo.InvariantCulture), Value = Math.Round(currentBouncedRate / currentMonthDays.Count(), 2).ToString() });

            foreach (var day in previousMonthDays)
            {
                var queryString = string.Format(queryFormat, day.ToString("yyyy-MM-dd"));
                var items = await _profileStoreService.GetVisualizationItems(queryString);
                if (items.VisualizationModels != null)
                {
                    var data = items.VisualizationModels.GroupBy(o => o.DeviceId).Select(g => new { g.Key, NumberOfItems = g.Count() });
                    var total = data.Count();
                    var bounced = data.Count(o => o.NumberOfItems == 1);
                    var bouncedRate = (double)bounced / total * 100;
                    previousBouncedRate += bouncedRate;
                }
            }
            result.Add(new CmsDashboardModel { Key = preStartDate.ToString("MMMM, yyyy", CultureInfo.InvariantCulture), Value = Math.Round(previousBouncedRate / previousMonthDays.Count(), 2).ToString() });

            return result;
        }

        public async Task<List<CmsDashboardModel>> GetTopLandingPages()
        {
            //Current Month
            var curStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var curEndDate = curStartDate.AddMonths(1).AddDays(-1);

            var currentMonthDays = Enumerable.Range(0, 1 + DateTime.Now.Subtract(curStartDate).Days)
                                      .Select(offset => curStartDate.AddDays(offset))
                                      .ToArray();

            var result = new List<CmsDashboardModel>();
            var data = new List<VisualizationModel>();
            var queryFormat = "?$top=10000&$orderBy=EventTime DESC&$filter=EventType eq epiPageView and EventTime eq {0}";
            foreach (var day in currentMonthDays)
            {
                var queryString = string.Format(queryFormat, DateTime.Now.ToString("yyyy-MM-dd"));
                var items = await _profileStoreService.GetVisualizationItems(queryString);
                if (items.VisualizationModels != null)
                {
                    data.AddRange(items.VisualizationModels);
                }
            }
            var pageData = data.GroupBy(o => o.Value).Select(g => new { Page = g.Key.Replace("Viewed ", ""), NumberOfItems = g.Count() });
            var pages = pageData.OrderByDescending(o => o.NumberOfItems).Take(5);
            foreach (var page in pages)
            {
                result.Add(new CmsDashboardModel { Key = page.Page, Value = page.NumberOfItems.ToString() });
            }
            return result;
        }

        public async Task<List<CmsDashboardModel>> GetTopLocations()
        {
            //Current Month
            var curStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var curEndDate = curStartDate.AddMonths(1).AddDays(-1);

            var currentMonthDays = Enumerable.Range(0, 1 + DateTime.Now.Subtract(curStartDate).Days)
                                      .Select(offset => curStartDate.AddDays(offset))
                                      .ToArray();

            var result = new List<CmsDashboardModel>();
            var data = new List<VisualizationModel>();
            var queryFormat = "?$top=10000&$orderBy=EventTime DESC&$filter=EventType eq epiPageView and EventTime eq {0}";
            foreach (var day in currentMonthDays)
            {
                var queryString = string.Format(queryFormat, DateTime.Now.ToString("yyyy-MM-dd"));
                var items = await _profileStoreService.GetVisualizationItems(queryString);
                if (items.VisualizationModels != null)
                {
                    data.AddRange(items.VisualizationModels);
                }
            }
            var locationData = data.GroupBy(o => o.CountryCode).Select(g => new { Country = $"{g.Key} - {MappingCountryCode(g.Key)}", NumberOfItems = g.Count() });
            var locations = locationData.OrderByDescending(o => o.NumberOfItems).Take(5);
            foreach (var location in locations)
            {
                result.Add(new CmsDashboardModel { Key = location.Country, Value = location.NumberOfItems.ToString() });
            }
            return result;
        }

        public async Task<List<CmsDashboardModel>> GetVisitors()
        {
            //Current Month
            var curStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var curEndDate = curStartDate.AddMonths(1).AddDays(-1);
            //Previous Month
            var preStartDate = curStartDate.AddMonths(-1);
            var preEndDate = curStartDate.AddDays(-1);

            var currentMonthDays = Enumerable.Range(0, 1 + DateTime.Now.Subtract(curStartDate).Days)
                  .Select(offset => curStartDate.AddDays(offset))
                  .ToArray();

            var previousMonthDays = Enumerable.Range(0, 1 + preEndDate.Subtract(preStartDate).Days)
                  .Select(offset => preStartDate.AddDays(offset))
                  .ToArray();

            var result = new List<CmsDashboardModel>();

            var currentVisitors = 0;
            var previousVisitors = 0;
            foreach (var day in currentMonthDays)
            {
                var queryFormat = "?$top=10000&$orderBy=EventTime DESC&$filter=EventType eq epiPageView and EventTime eq {0}";

                var queryString = string.Format(queryFormat, day.ToString("yyyy-MM-dd"));
                var items = await _profileStoreService.GetVisualizationItems(queryString);
                if (items.VisualizationModels != null)
                {
                    var data = items.VisualizationModels.Select(o => new
                    {
                        Visitor = GetObjectPropValue(o.User, "Email") ?? o.DeviceId
                    });
                    currentVisitors += data.Distinct().Count();
                }
            }
            result.Add(new CmsDashboardModel { Key = curStartDate.ToString("MMMM, yyyy", CultureInfo.InvariantCulture), Value = currentVisitors.ToString() });

            foreach (var day in previousMonthDays)
            {
                var queryFormat = "?$top=10000&$orderBy=EventTime DESC&$filter=EventType eq epiPageView and EventTime eq {0}";

                var queryString = string.Format(queryFormat, day.ToString("yyyy-MM-dd"));
                var items = await _profileStoreService.GetVisualizationItems(queryString);
                if (items.VisualizationModels != null)
                {
                    var data = items.VisualizationModels.Select(o => new
                    {
                        Visitor = GetObjectPropValue(o.User, "Email") ?? o.DeviceId
                    });
                    previousVisitors += data.Distinct().Count();
                }
            }
            result.Add(new CmsDashboardModel { Key = preStartDate.ToString("MMMM, yyyy", CultureInfo.InvariantCulture), Value = previousVisitors.ToString() });

            return result;
        }

        public async Task<List<IMarketingTest>> GetAbTestContents()
        {
            return await Task.Run(() =>
            {
                var mts = _marketingTestingWebRepository.GetTestList(new TestCriteria());
                return mts.OrderByDescending(x => x.CreatedDate).Take(5).ToList();
            });
        }

        public async Task<List<ABTestingModel>> GetAbTestingMostParticipationPercentage()
        {
            return await Task.Run(() =>
            {
                List<ABTestingModel> result = new List<ABTestingModel>();
                var testList = _marketingTestingWebRepository.GetTestList(new TestCriteria()).OrderByDescending(x => x.ParticipationPercentage).Take(5);
                foreach (var item in testList)
                {
                    result.Add(new ABTestingModel() { ABTestingName = item.Title, ParticipationPercentage = item.ParticipationPercentage });
                }
                return result;
            });
        }

        public async Task<List<ABTestingModel>> GetAbTestingMostPageViews()
        {
            return await Task.Run(() =>
            {
                List<ABTestingModel> result = new List<ABTestingModel>();
                var testList = _marketingTestingWebRepository.GetTestList(new TestCriteria()).OrderByDescending(x => x.Variants.Max(m => m.Views)).Take(5);
                foreach (var item in testList)
                {
                    result.Add(new ABTestingModel() { ABTestingName = item.Title, Views = item.Variants.Max(s => s.Views) });
                }
                return result;
            });
        }

        public async Task<List<ABTestingModel>> GetAbTestingMostConversions()
        {
            return await Task.Run(() =>
            {
                List<ABTestingModel> result = new List<ABTestingModel>();
                var testList = _marketingTestingWebRepository.GetTestList(new TestCriteria()).OrderByDescending(x => x.Variants.Max(m => m.Conversions)).Take(5);
                foreach (var item in testList)
                {
                    result.Add(new ABTestingModel() { ABTestingName = item.Title, Conversions = item.Variants.Max(s => s.Conversions) });
                }
                return result;
            });
        }

        private string GetObjectPropValue(object obj, string prop)
        {
            Type oType = obj.GetType();
            IList<PropertyInfo> properties = new List<PropertyInfo>(oType.GetProperties());
            if (properties.FirstOrDefault(o => o.Name == prop) != null)
            {
                var propertyValue = properties.FirstOrDefault(o => o.Name == prop).GetValue(oType, null).ToString();
                return propertyValue;
            }
            return null;
        }

        private string MappingCountryCode(string code)
        {
            if (Countries.ContainsKey(code)) return Countries[code];
            return "Unknown";
        }

        private Dictionary<string, string> Countries = new Dictionary<string, string> {
            {"A1", "Anonymous Proxy"},
            {"A2","Satellite Provider"},
            {"O1","Other Country"},
            {"AD","Andorra"},
            {"AE","United Arab Emirates"},
            {"AF","Afghanistan"},
            {"AG","Antigua and Barbuda"},
            {"AI","Anguilla"},
            {"AL","Albania"},
            {"AM","Armenia"},
            {"AO","Angola"},
            {"AP","Asia/Pacific Region"},
            {"AQ","Antarctica"},
            {"AR","Argentina"},
            {"AS","American Samoa"},
            {"AT","Austria"},
            {"AU","Australia"},
            {"AW","Aruba"},
            {"AX","Aland Islands"},
            {"AZ","Azerbaijan"},
            {"BA","Bosnia and Herzegovina"},
            {"BB","Barbados"},
            {"BD","Bangladesh"},
            {"BE","Belgium"},
            {"BF","Burkina Faso"},
            {"BG","Bulgaria"},
            {"BH","Bahrain"},
            {"BI","Burundi"},
            {"BJ","Benin"},
            {"BL","Saint Bartelemey"},
            {"BM","Bermuda"},
            {"BN","Brunei Darussalam"},
            {"BO","Bolivia"},
            {"BQ","Bonaire, Saint Eustatius and Saba"},
            {"BR","Brazil"},
            {"BS","Bahamas"},
            {"BT","Bhutan"},
            {"BV","Bouvet Island"},
            {"BW","Botswana"},
            {"BY","Belarus"},
            {"BZ","Belize"},
            {"CA","Canada"},
            {"CC","Cocos (Keeling) Islands"},
            {"CD","Congo, The Democratic Republic of the"},
            {"CF","Central African Republic"},
            {"CG","Congo"},
            {"CH","Switzerland"},
            {"CI","Cote d'Ivoire"},
            {"CK","Cook Islands"},
            {"CL","Chile"},
            {"CM","Cameroon"},
            {"CN","China"},
            {"CO","Colombia"},
            {"CR","Costa Rica"},
            {"CU","Cuba"},
            {"CV","Cape Verde"},
            {"CW","Curacao"},
            {"CX","Christmas Island"},
            {"CY","Cyprus"},
            {"CZ","Czech Republic"},
            {"DE","Germany"},
            {"DJ","Djibouti"},
            {"DK","Denmark"},
            {"DM","Dominica"},
            {"DO","Dominican Republic"},
            {"DZ","Algeria"},
            {"EC","Ecuador"},
            {"EE","Estonia"},
            {"EG","Egypt"},
            {"EH","Western Sahara"},
            {"ER","Eritrea"},
            {"ES","Spain"},
            {"ET","Ethiopia"},
            {"EU","Europe"},
            {"FI","Finland"},
            {"FJ","Fiji"},
            {"FK","Falkland Islands (Malvinas)"},
            {"FM","Micronesia, Federated States of"},
            {"FO","Faroe Islands"},
            {"FR","France"},
            {"GA","Gabon"},
            {"GB","United Kingdom"},
            {"GD","Grenada"},
            {"GE","Georgia"},
            {"GF","French Guiana"},
            {"GG","Guernsey"},
            {"GH","Ghana"},
            {"GI","Gibraltar"},
            {"GL","Greenland"},
            {"GM","Gambia"},
            {"GN","Guinea"},
            {"GP","Guadeloupe"},
            {"GQ","Equatorial Guinea"},
            {"GR","Greece"},
            {"GS","South Georgia and the South Sandwich Islands"},
            {"GT","Guatemala"},
            {"GU","Guam"},
            {"GW","Guinea-Bissau"},
            {"GY","Guyana"},
            {"HK","Hong Kong"},
            {"HM","Heard Island and McDonald Islands"},
            {"HN","Honduras"},
            {"HR","Croatia"},
            {"HT","Haiti"},
            {"HU","Hungary"},
            {"ID","Indonesia"},
            {"IE","Ireland"},
            {"IL","Israel"},
            {"IM","Isle of Man"},
            {"IN","India"},
            {"IO","British Indian Ocean Territory"},
            {"IQ","Iraq"},
            {"IR","Iran, Islamic Republic of"},
            {"IS","Iceland"},
            {"IT","Italy"},
            {"JE","Jersey"},
            {"JM","Jamaica"},
            {"JO","Jordan"},
            {"JP","Japan"},
            {"KE","Kenya"},
            {"KG","Kyrgyzstan"},
            {"KH","Cambodia"},
            {"KI","Kiribati"},
            {"KM","Comoros"},
            {"KN","Saint Kitts and Nevis"},
            {"KP","Korea, Democratic People's Republic of"},
            {"KR","Korea, Republic of"},
            {"KW","Kuwait"},
            {"KY","Cayman Islands"},
            {"KZ","Kazakhstan"},
            {"LA","Lao People's Democratic Republic"},
            {"LB","Lebanon"},
            {"LC","Saint Lucia"},
            {"LI","Liechtenstein"},
            {"LK","Sri Lanka"},
            {"LR","Liberia"},
            {"LS","Lesotho"},
            {"LT","Lithuania"},
            {"LU","Luxembourg"},
            {"LV","Latvia"},
            {"LY","Libyan Arab Jamahiriya"},
            {"MA","Morocco"},
            {"MC","Monaco"},
            {"MD","Moldova, Republic of"},
            {"ME","Montenegro"},
            {"MF","Saint Martin"},
            {"MG","Madagascar"},
            {"MH","Marshall Islands"},
            {"MK","Macedonia"},
            {"ML","Mali"},
            {"MM","Myanmar"},
            {"MN","Mongolia"},
            {"MO","Macao"},
            {"MP","Northern Mariana Islands"},
            {"MQ","Martinique"},
            {"MR","Mauritania"},
            {"MS","Montserrat"},
            {"MT","Malta"},
            {"MU","Mauritius"},
            {"MV","Maldives"},
            {"MW","Malawi"},
            {"MX","Mexico"},
            {"MY","Malaysia"},
            {"MZ","Mozambique"},
            {"NA","Namibia"},
            {"NC","New Caledonia"},
            {"NE","Niger"},
            {"NF","Norfolk Island"},
            {"NG","Nigeria"},
            {"NI","Nicaragua"},
            {"NL","Netherlands"},
            {"NO","Norway"},
            {"NP","Nepal"},
            {"NR","Nauru"},
            {"NU","Niue"},
            {"NZ","New Zealand"},
            {"OM","Oman"},
            {"PA","Panama"},
            {"PE","Peru"},
            {"PF","French Polynesia"},
            {"PG","Papua New Guinea"},
            {"PH","Philippines"},
            {"PK","Pakistan"},
            {"PL","Poland"},
            {"PM","Saint Pierre and Miquelon"},
            {"PN","Pitcairn"},
            {"PR","Puerto Rico"},
            {"PS","Palestinian Territory"},
            {"PT","Portugal"},
            {"PW","Palau"},
            {"PY","Paraguay"},
            {"QA","Qatar"},
            {"RE","Reunion"},
            {"RO","Romania"},
            {"RS","Serbia"},
            {"RU","Russian Federation"},
            {"RW","Rwanda"},
            {"SA","Saudi Arabia"},
            {"SB","Solomon Islands"},
            {"SC","Seychelles"},
            {"SD","Sudan"},
            {"SE","Sweden"},
            {"SG","Singapore"},
            {"SH","Saint Helena"},
            {"SI","Slovenia"},
            {"SJ","Svalbard and Jan Mayen"},
            {"SK","Slovakia"},
            {"SL","Sierra Leone"},
            {"SM","San Marino"},
            {"SN","Senegal"},
            {"SO","Somalia"},
            {"SR","Suriname"},
            {"SS","South Sudan"},
            {"ST","Sao Tome and Principe"},
            {"SV","El Salvador"},
            {"SX","Sint Maarten"},
            {"SY","Syrian Arab Republic"},
            {"SZ","Swaziland"},
            {"TC","Turks and Caicos Islands"},
            {"TD","Chad"},
            {"TF","French Southern Territories"},
            {"TG","Togo"},
            {"TH","Thailand"},
            {"TJ","Tajikistan"},
            {"TK","Tokelau"},
            {"TL","Timor-Leste"},
            {"TM","Turkmenistan"},
            {"TN","Tunisia"},
            {"TO","Tonga"},
            {"TR","Turkey"},
            {"TT","Trinidad and Tobago"},
            {"TV","Tuvalu"},
            {"TW","Taiwan"},
            {"TZ","Tanzania, United Republic of"},
            {"UA","Ukraine"},
            {"UG","Uganda"},
            {"UM","United States Minor Outlying Islands"},
            {"US","United States"},
            {"UY","Uruguay"},
            {"UZ","Uzbekistan"},
            {"VA","Holy See (Vatican City State)"},
            {"VC","Saint Vincent and the Grenadines"},
            {"VE","Venezuela"},
            {"VG","Virgin Islands, British"},
            {"VI","Virgin Islands, U.S."},
            {"VN","Vietnam"},
            {"VU","Vanuatu"},
            {"WF","Wallis and Futuna"},
            {"WS","Samoa"},
            {"YE","Yemen"},
            {"YT","Mayotte"},
            {"ZA","South Africa"},
            {"ZM","Zambia"},
            {"ZW","Zimbabwe"} };
    }
}