using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

namespace Contoso.Samples.RERWeb
{

    public static class ListHelper
    {

        public static void ChangeListSettings(ClientContext ctx, string listName)
        {
            Trace.WriteLine("Inside ChangeListSettings");
            string _result = string.Empty;
            if (ctx != null)
            {
                try
                {
                    Web web = ctx.Web;
                    ctx.Load(web, w => w.Title, w => w.Description);
                    ctx.ExecuteQuery();
                    Trace.WriteLine("Web url:" + ctx.Url);
                    Trace.WriteLine("Web title:" + web.Title);
                    Trace.WriteLine("List name:" + listName);

                    ctx.Load(web.Lists, lists => lists.Include(list => list.Title, list => list.Id));
                    ctx.ExecuteQuery();

                    foreach (List list in web.Lists)
                    {
                        Trace.WriteLine(list.Title);
                    }

                    List lst = web.Lists.GetByTitle(listName);
                    lst.ContentTypesEnabled = true;
                    lst.EnableFolderCreation = false;
                    lst.UpdateListVersioning(true, true);
                    lst.Update();
                    ctx.ExecuteQuery();

                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Failed to update list settings " + ex.ToString());
                    throw;
                }
            }
            else
                Trace.WriteLine("Inside ChangeListSettigns - Ctx is null");
        }
    }
}