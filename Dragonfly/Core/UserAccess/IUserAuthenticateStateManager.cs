using Dragonfly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Dragonfly.Core.UserAccess
{
    public interface IUserAuthenticateStateManager
    {
        bool CheckUserAccess(HttpRequestBase request, HttpResponseBase response);

        void LogOut(HttpRequestBase request, HttpResponseBase response);

        bool LogIn(HttpResponseBase response, AuthenticateModel authParameters);

        decimal GetUserIdFromCookies(HttpRequestBase request);
    }
}
