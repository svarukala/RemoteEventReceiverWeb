using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;
using Microsoft.SharePoint.Client.Taxonomy;
using Microsoft.SharePoint.Client.UserProfiles;
using System.Collections;
using System.Diagnostics;

namespace Contoso.Samples.RERWeb.Services
{
    public class SampleRERService : IRemoteEventService
    {
        /// <summary>
        /// Handles events that occur before an action occurs, such as when a user adds or deletes a list item.
        /// </summary>
        /// <param name="properties">Holds information about the remote event.</param>
        /// <returns>Holds information returned from the remote event.</returns>
        public SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties)
        {
            SPRemoteEventResult _result = new SPRemoteEventResult();
            System.Diagnostics.Trace.WriteLine("Inside ProcessEvent method");
            try 
            {
                _result.Status = SPRemoteEventServiceStatus.Continue;
                switch (properties.EventType)
                {
                    case SPRemoteEventType.ItemAdded:
                        HandleItemAdded(properties);
                        break;
                    case SPRemoteEventType.ItemAdding:
                        HandleItemAdding(properties, _result);
                        break;
                    case SPRemoteEventType.ListAdded:
                        HandleListAdded(properties);
                        break;
                    case SPRemoteEventType.SiteDeleting:
                        HandleSiteDeleting(properties, _result);
                        break;
                    case SPRemoteEventType.WebDeleting:
                        HandleWebDeleting(properties, _result);
                        break;
                }
                //_result.Status = SPRemoteEventServiceStatus.Continue;
            }
            catch(Exception)
            {
                //You should log here.               
            }
            return _result;
        }

        /// <summary>
        /// Handles events that occur after an action occurs, such as after a user adds an item to a list or deletes an item from a list.
        /// </summary>
        /// <param name="properties">Holds information about the remote event.</param>
        public void ProcessOneWayEvent(SPRemoteEventProperties properties)
        {
            throw new NotImplementedException();
        }

        public void HandleWebDeleting(SPRemoteEventProperties properties, SPRemoteEventResult result)
        {
            System.Diagnostics.Trace.WriteLine("Inside HandleWebDeleting method");
            result.ErrorMessage = "Sorry! can't delete this web. Contact admin.";
            result.Status = SPRemoteEventServiceStatus.CancelWithError;

            //string webUrl = properties.WebEventProperties.FullUrl;
            //Uri webUri = new Uri(webUrl);
            //string realm = TokenHelper.GetRealmFromTargetUrl(webUri);
            //string accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, webUri.Authority, realm).AccessToken;
            //Trace.WriteLine("About to get the ctx for realm: {0}", realm);


            //using (ClientContext ctx = TokenHelper.GetClientContextWithAccessToken(webUrl, accessToken))
            //{
            //    if (ctx != null)
            //    {
            //        try
            //        {
            //            Trace.WriteLine("Web delete event for: " + properties.WebEventProperties.FullUrl);

            //            result.ErrorMessage = "Sorry! can't delete this web. Contact admin.";
            //            result.Status = SPRemoteEventServiceStatus.CancelWithError;

            //        }
            //        catch (Exception ex)
            //        {
            //            Trace.WriteLine(ex.ToString());
            //        }
            //    }
            //    else
            //        Trace.WriteLine("Context is null");
            //}
        }
        public void HandleSiteDeleting(SPRemoteEventProperties properties, SPRemoteEventResult result)
        {
            System.Diagnostics.Trace.WriteLine("Inside HandleSiteDeleting method");
            result.ErrorMessage = "Sorry! can't delete this site. Contact admin.";
            result.Status = SPRemoteEventServiceStatus.CancelWithError;


            //string webUrl = properties.WebEventProperties.FullUrl;
            //Uri webUri = new Uri(webUrl);
            //string realm = TokenHelper.GetRealmFromTargetUrl(webUri);
            //string accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, webUri.Authority, realm).AccessToken;
            //Trace.WriteLine("About to get the ctx for realm: {0}", realm);


            //using (ClientContext ctx = TokenHelper.GetClientContextWithAccessToken(webUrl, accessToken))
            //{
            //    if (ctx != null)
            //    {
            //        try
            //        {
            //            Trace.WriteLine("Site delete event for: " + properties.WebEventProperties.FullUrl);
            //            result.ErrorMessage = "Sorry! can't delete this site. Contact admin.";
            //            result.Status = SPRemoteEventServiceStatus.CancelWithError;

            //            //if (SiteHelper.CheckSiteIsOnHold(ctx))
            //            //{
            //            //    //ignore
            //            //}
            //        }
            //        catch (Exception ex)
            //        {
            //            Trace.WriteLine(ex.ToString());
            //        }
            //    }
            //    else
            //        Trace.WriteLine("Context is null");
            //}
        }


        /// <summary>
        /// Used to Handle the ItemAdding Event
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="result"></param>
        public void HandleItemAdding(SPRemoteEventProperties properties,SPRemoteEventResult result)
        {
            System.Diagnostics.Trace.WriteLine("Inside HandleAutoTaggingItemAdding method");

            try
            {
                string webUrl = properties.ItemEventProperties.WebUrl; //properties.AppEventProperties.HostWebFullUrl.AbsoluteUri; //on installation, associate w/ web 
                Uri webUri = new Uri(webUrl);
                string realm = TokenHelper.GetRealmFromTargetUrl(webUri);
                string accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, webUri.Authority, realm).AccessToken;
                Trace.WriteLine("About to get the ctx for realm: {0}", realm);


                using (ClientContext ctx = TokenHelper.GetClientContextWithAccessToken(webUrl, accessToken))
                {
                    if (ctx != null)
                    {
                        try
                        {
                            var itemProperties = properties.ItemEventProperties;
                            var _userLoginName = properties.ItemEventProperties.UserLoginName;
                            var _afterProperites = itemProperties.AfterProperties;
                            Trace.WriteLine("Current user login: {0}", _userLoginName);
                            Web web = ctx.Web;
                            ctx.Load(web, w => w.Title, w => w.Description);
                            ctx.ExecuteQuery();
                            Trace.WriteLine("HATWeb url:" + ctx.Url);
                            Trace.WriteLine("HATWeb title:" + web.Title);
                            Trace.WriteLine("HATList title: " + properties.ItemEventProperties.ListTitle);

                            ListHelper.ChangeListSettings(ctx, properties.ItemEventProperties.ListTitle);

                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex.ToString());
                        }
                    }
                    else
                    {
                        Trace.WriteLine("ClientContext is null");
                    }
                }
            }
            catch (Exception e1)
            {
                Trace.WriteLine("Failed to to get app only token: {0}", e1.ToString());
            }



            //using (ClientContext ctx = TokenHelper.CreateRemoteEventReceiverClientContext(properties))
            //{
            //    if (ctx != null)
            //    {
            //        try
            //        {
            //            var itemProperties = properties.ItemEventProperties;
            //            var _userLoginName = properties.ItemEventProperties.UserLoginName;
            //            var _afterProperites = itemProperties.AfterProperties;
            //            Trace.WriteLine("Current user login: {0}", _userLoginName);
            //            if (!_afterProperites.ContainsKey(ScenarioHandler.FLD_CLASSIFICATION_INTERNAL_NAME))
            //            {
            //                Trace.WriteLine("Does not contain the key {0}", ScenarioHandler.FLD_CLASSIFICATION_INTERNAL_NAME);
            //                string _classficationToSet = ProfileHelper.GetProfilePropertyFor(ctx, _userLoginName, Constants.UPA_CLASSIFICATION_PROPERTY);
            //                if (!string.IsNullOrEmpty(_classficationToSet))
            //                {
            //                    Trace.WriteLine("Classification  to set: {0}", _classficationToSet);
            //                    var _formatTaxonomy = AutoTaggingHelper.GetTaxonomyFormat(ctx, _classficationToSet);
            //                    result.ChangedItemProperties.Add(ScenarioHandler.FLD_CLASSIFICATION_INTERNAL_NAME, _formatTaxonomy);
            //                }
            //            }
            //        }
            //        catch(Exception ex)
            //        {
            //            Trace.WriteLine(ex.ToString());
            //        }
            //    }
            //    else
            //    {
            //        Trace.WriteLine("ClientContext is null");
            //    }
            //}
        }
   
        /// <summary>
        /// Used to handle the ItemAdded event.
        /// </summary>
        /// <param name="properties"></param>
        public void HandleItemAdded(SPRemoteEventProperties properties)
        {
            System.Diagnostics.Trace.WriteLine("Inside HandleItemAdded method");
            string webUrl = properties.ItemEventProperties.WebUrl; //properties.AppEventProperties.HostWebFullUrl.AbsoluteUri; //on installation, associate w/ web 
            Uri webUri = new Uri(webUrl);
            string realm = TokenHelper.GetRealmFromTargetUrl(webUri);
            string accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, webUri.Authority, realm).AccessToken;
            Trace.WriteLine("About to get the ctx for realm: {0}", realm);


            using (ClientContext ctx = TokenHelper.GetClientContextWithAccessToken(webUrl, accessToken))
            {
                if (ctx != null)
                {
                    try
                    { 
                        string _userLoginName = properties.ItemEventProperties.UserLoginName;
                        List _library = ctx.Web.Lists.GetById(properties.ItemEventProperties.ListId);
                        var _itemToUpdate = _library.GetItemById(properties.ItemEventProperties.ListItemId);
                        ctx.Load(_itemToUpdate);
                        ctx.ExecuteQuery();
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.ToString());
                    }
                }
            }
        }

        public void HandleListAdded(SPRemoteEventProperties properties)
        {
            System.Diagnostics.Trace.WriteLine("Inside HandleListAdded method");
            string webUrl = properties.ListEventProperties.WebUrl; //properties.AppEventProperties.HostWebFullUrl.AbsoluteUri; //on installation, associate w/ web 
            Uri webUri = new Uri(webUrl);
            string realm = TokenHelper.GetRealmFromTargetUrl(webUri);
            string accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, webUri.Authority, realm).AccessToken;
            Trace.WriteLine("About to get the ctx for realm: {0}", realm);


            using (ClientContext ctx = TokenHelper.GetClientContextWithAccessToken(webUrl, accessToken))
            {
                if (ctx != null)
                {
                    try
                    {
                        Trace.WriteLine("List added event raisd for list: " + properties.ListEventProperties.ListTitle);
                        ListHelper.ChangeListSettings(ctx, properties.ListEventProperties.ListTitle);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.ToString());
                    }
                }
                else
                    Trace.WriteLine("Context is null");
            }
        }
    }
}
