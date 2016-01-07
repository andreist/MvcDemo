using System.Collections.Generic;
using System.Web.Mvc;
using System.Net;

namespace MvcDemo
{
    public class HandleJsonExceptionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAjaxRequest() || filterContext.Exception == null)
                return;

            filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var exInfo = new List<ExceptionInformation>();
            for (var ex = filterContext.Exception; ex != null; ex = ex.InnerException)
            {
                exInfo.Add(new ExceptionInformation
                {
                    Message = ex.Message,
                    Source = ex.Source,
                    StackTrace = ex.StackTrace
                });
            }
            filterContext.Result = new JsonResult
                                       {
                                           Data = exInfo,
                                           JsonRequestBehavior = JsonRequestBehavior.AllowGet
                                       };
            filterContext.ExceptionHandled = true;
        }
    }
}