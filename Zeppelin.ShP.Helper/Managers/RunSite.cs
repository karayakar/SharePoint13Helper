using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeppelin.ShP.Helper.Helpers;

namespace Zeppelin.ShP.Helper.Managers
{
    public class RunSite
    {
        public delegate void RunManageSiteWithAdminDelegate(SPSite site, SPWeb web);
        public static string currentSiteUrl = SPContext.Current.Web.Url;

        public static void Privileges(RunManageSiteWithAdminDelegate myDelegate)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(currentSiteUrl))
                {
                    site.AllowUnsafeUpdates = true;
                    using (SPWeb web = site.OpenWeb(currentSiteUrl))
                    {
                        web.AllowUnsafeUpdates = true;
                        myDelegate.Invoke(site, web);
                    }
                }
            });
        }

        public static void AsUser(string UserLogin, RunManageSiteWithAdminDelegate myDelegate)
        {
            SPUser user = SPContext.Current.Web.SiteUsers[UserLogin];
            using (SPSite site = new SPSite(currentSiteUrl, user.UserToken))
            {
                site.AllowUnsafeUpdates = true;
                using (SPWeb web = site.OpenWeb(currentSiteUrl))
                {
                    web.AllowUnsafeUpdates = true;
                    myDelegate.Invoke(site, web);
                }
            }
        }

        public static void ByUrl(string webUrl, RunManageSiteWithAdminDelegate myDelegate)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(webUrl))
                {
                    site.AllowUnsafeUpdates = true;
                    using (SPWeb web = site.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;
                        myDelegate.Invoke(site, web);
                    }
                }
            });
        }

        //public static void AsUserByUrl(string Url, string UserLogin, RunManageSiteWithAdminDelegate myDelegate)
        //{

        //    SPSecurity.RunWithElevatedPrivileges(delegate()
        //    {
        //        SPUser user = SPContext.Current.Web.SiteUsers[UserLogin];
        //        using (SPSite site = new SPSite(currentSiteUrl))
        //        {
        //            site.AllowUnsafeUpdates = true;
        //            using (SPWeb web = site.OpenWeb(Url))
        //            {
        //                web.AllowUnsafeUpdates = true;
        //                myDelegate.Invoke(site, web);
        //            }
        //        }
        //    });
        //}

        public static void RootSiteByUrl(string Url, RunManageSiteWithAdminDelegate myDelegate)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(Url))
                {
                    site.AllowUnsafeUpdates = true;
                    using (SPWeb web = site.RootWeb)
                    {
                        web.AllowUnsafeUpdates = true;
                        myDelegate.Invoke(site, web);
                    }
                }
            });
        }

        public static void ByWeb(SPWeb cWeb, RunManageSiteWithAdminDelegate myDelegate)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(cWeb.Url))
                {
                    site.AllowUnsafeUpdates = true;
                    using (SPWeb web = site.OpenWeb(cWeb.ServerRelativeUrl))
                    {
                        web.AllowUnsafeUpdates = true;
                        myDelegate.Invoke(site, web);
                    }
                }
            });
        }
    }
}
