using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcDemo
{
    /// <summary>
    /// To send exceptions as json we define [HandleJsonException] attribute
    /// </summary>
    public class ExceptionInformation
    {
        public string Message { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
    }
}