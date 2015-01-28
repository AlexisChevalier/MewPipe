using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace MewPipe.Website.Oauth
{
    public class TokenMethodSelectorAtrribute : ActionMethodSelectorAttribute
    {
        public string GrantType { get; set; }
        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            return controllerContext.Controller.ValueProvider.GetValue("grant_type").RawValue.Equals(GrantType);
        }
    }
}