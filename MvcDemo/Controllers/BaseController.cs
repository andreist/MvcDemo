using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcDemo.Controllers
{
    public class BaseController : Controller
    {
        public T GetExtraState<T>(string key)
        {
            Object stateObject = Session[GetExtraStateKey(key)];
            return stateObject is T ? (T)stateObject : default(T);
        }

        private string GetExtraStateKey(string key)
        {
            return string.Format("ExtraState-{0}-{1}", GetType().Name, key);
        }

        public void SaveExtraState(string key, Object state)
        {
            Session[GetExtraStateKey(key)] = state;
        }
    }
}
