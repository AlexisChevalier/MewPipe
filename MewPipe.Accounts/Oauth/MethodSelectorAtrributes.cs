using System.Reflection;
using System.Web.Mvc;

namespace MewPipe.Accounts.Oauth
{
    public class TokenMethodSelectorAtrribute : ActionMethodSelectorAttribute
    {
        public string GrantType { get; set; }
        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            var type = controllerContext.Controller.ValueProvider.GetValue("grant_type").RawValue as string[];

            if (type == null)
            {
                return false;
            }

            return Equals(type[0], GrantType);
        }
    }
}