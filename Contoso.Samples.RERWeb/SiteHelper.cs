using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;


namespace Contoso.Samples.RERWeb
{
    public class SiteHelper
    {
        public static bool CheckSiteIsOnHold(ClientContext ctx)
        {
            Trace.WriteLine("Inside CheckSiteIsOnHold");
            return true;
            string _result = string.Empty;
            if (ctx != null)
            {
                try
                {
                    Web web = ctx.Web;
                    ctx.Load(web, w => w.AllProperties);
                    ctx.ExecuteQuery();

                    if (web.AllProperties.FieldValues.ContainsKey("IsOnHold"))
                    {
                        Trace.WriteLine("Is site on hold: {0}", (string)web.AllProperties["IsOnHold"]);
                        return ((string)web.AllProperties["IsOnHold"] == "1") ? true : false;
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Exception " + ex.ToString());
                    throw;
                }
            }
            else
                Trace.WriteLine("Inside CheckSiteIsOnHold - Ctx is null");
            return false;
        }
    }
}