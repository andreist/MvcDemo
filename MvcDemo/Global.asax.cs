using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Text.RegularExpressions;
using MvcDemo.App_Start;
using NLog;
using MvcDemo.BLL;

namespace MvcDemo
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        #region Properties

        private static string _staticFilePattern = "[^.]+[.](jpg|bmp|gif|css|axd|swf|js|ashx|ico|zip|rar|png|pdf|doc|xls|docx|xlsx|ppt|pptx|txt|axd)";
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion


        #region EventHandlers

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutoMapperConfig.Init();
        }

        private void Application_BeginRequest(Object sender, EventArgs e)
        {
            if (!IsMediaRequest())
            {
                UnitOfWorkBl.InitUow();


                string message = String.Format("Starting a new httprequest - RequestUrl = {0} ", HttpContext.Current.Request.Url);
                _logger.Log(LogLevel.Info, message);
            }
        }

        private void Application_EndRequest(Object sender, EventArgs e)
        {
            if (!IsMediaRequest())
            {
                string message = String.Format("End of httpRequest - RequestUrl = {1} - duration : {0:N0} ms ", HttpContext.Current.Request.Url, (DateTime.Now - HttpContext.Current.Timestamp).TotalMilliseconds);
                _logger.Log(LogLevel.Info, message);

                UnitOfWorkBl.DisposeUow();
            }
        }

        private void Application_Error(Object sender, EventArgs e)
        {
            Exception ex = HttpContext.Current.Server.GetLastError();
            if (ex is HttpUnhandledException)
            {
                ex = ex.InnerException ?? ex;
            }

            _logger.Log(LogLevel.Error, ex.Message);
        }

        #endregion


        #region Methods

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        private bool IsMediaRequest()
        {
            string path = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.ToLower();
            bool mediaFile = ((path.IndexOf("/styles", StringComparison.InvariantCultureIgnoreCase) != -1)
                 || (path.IndexOf("/scripts", StringComparison.InvariantCultureIgnoreCase) != -1)
                 || (path.IndexOf("/Content", StringComparison.InvariantCultureIgnoreCase) != -1)
                 );

            return mediaFile || Regex.IsMatch(path, _staticFilePattern, RegexOptions.IgnoreCase);
        }

        #endregion
    }
}