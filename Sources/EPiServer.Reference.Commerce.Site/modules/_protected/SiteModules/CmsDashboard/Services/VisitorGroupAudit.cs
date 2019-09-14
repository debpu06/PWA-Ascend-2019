using System;
using EPiServer.Core;
using EPiServer.Personalization.VisitorGroups;
using EPiServer.ServiceLocation;
using System.Linq;
using EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CmsDashboard.Models;
using EPiServer.Security;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.CmsDashboard.Services
{
    public class VisitorGroupAudit
    {
        public void Execute()
        {
            //Add implementation
            var repo = ServiceLocator.Current.GetInstance<IContentRepository>();
            var vgrepo = ServiceLocator.Current.GetInstance<IVisitorGroupRepository>();
            var visitorgrouplist = vgrepo.List().ToList();
            int cnt = 0;
            int usesfound = 0;
            VisitorGroupUse.CleanUp();

            //Add implementation
            foreach (var cr in repo.GetDescendents(ContentReference.RootPage))
            {
                var c = repo.Get<IContentData>(cr);
                if (c is ISecurable)
                {
                    var sec = (c as ISecurable).GetSecurityDescriptor();
                    if (sec is ContentAccessControlList)
                    {
                        var cacl = (sec as ContentAccessControlList);
                        foreach (var ca in cacl.Where(cac => cac.Value.EntityType == SecurityEntityType.VisitorGroup))
                        {
                            usesfound++;
                            VisitorGroupUse vgu = new VisitorGroupUse();
                            vgu.Seen = DateTime.Now;
                            vgu.Content = cr;
                            vgu.VisitorGroup = visitorgrouplist.Where(vvg => vvg.Name == ca.Value.Name).Select(vvg => vvg.Id.ToString()).FirstOrDefault();
                            vgu.PropertyName = "(Content Access Rights)";
                            vgu.ContentName = (c as IContent).Name;
                            vgu.ContentType = (c is PageData) ? "Page" : (c is BlockData) ? "Block" : "Other";
                            VisitorGroupUse.Save(vgu);
                        }
                    }
                    //Look for EntityType="VisitorGroup" and then match on name?!
                }
                foreach (var p in c.Property)
                {
                    if (p.Value == null) continue;
                    if (p.PropertyValueType == typeof(ContentArea))
                    {
                        var ca = p.Value as ContentArea;
                        if (ca == null) continue;
                        foreach (var f in ca.Items.Where(l => l.AllowedRoles != null && l.AllowedRoles.Any()))
                        {
                            //Match! This page uses the visitor groups in l.AllowedRoles. Record.
                            foreach (var r in f.AllowedRoles)
                            {
                                usesfound++;
                                VisitorGroupUse vgu = new VisitorGroupUse();
                                vgu.Seen = DateTime.Now;
                                vgu.VisitorGroup = r;
                                vgu.Content = cr;
                                vgu.PropertyName = p.Name;
                                vgu.ContentName = (c as IContent).Name;
                                vgu.ContentType = (c is PageData) ? "Page" : (c is BlockData) ? "Block" : "Other";
                                VisitorGroupUse.Save(vgu);
                            }
                        }
                    }
                    else if (p.PropertyValueType == typeof(XhtmlString))
                    {
                        var ca = p.Value as XhtmlString;
                        if (ca == null) continue;
                        foreach (var f in ca.Fragments.Where(fr => fr is EPiServer.Core.Html.StringParsing.PersonalizedContentFragment))
                        {

                            var j = f as EPiServer.Core.Html.StringParsing.PersonalizedContentFragment;
                            var roles = j.GetRoles();
                            foreach (var r in roles)
                            {
                                usesfound++;
                                VisitorGroupUse vgu = new VisitorGroupUse();
                                vgu.Seen = DateTime.Now;
                                vgu.VisitorGroup = r;
                                vgu.Content = cr;
                                vgu.PropertyName = p.Name;
                                vgu.ContentName = (c as IContent).Name;
                                vgu.ContentType = (c is PageData) ? "Page" : (c is BlockData) ? "Block" : "Other";
                                VisitorGroupUse.Save(vgu);
                            }
                        }

                    }
                }
                cnt++;
            }
        }
    }
}
