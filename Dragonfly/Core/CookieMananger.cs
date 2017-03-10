using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dragonfly.Core
{
    /// <summary>
    /// The class manage of the cookies.
    /// </summary>
    internal class CookieMananger : ICookiesManager
    {
        private Dictionary<CookieType, string> _CookiesNames = new Dictionary<CookieType, string>
        {
            { CookieType.UserAccessToken, "Access_Token" },
            { CookieType.UserId , "User_Id" },
            { CookieType.UserName , "User_Name" },
        };

        /// <summary>
        /// Method gets the cookie name by it type.
        /// </summary>
        /// <param name="type">Type of the cookie.</param>
        /// <returns>Name of the cookie.</returns>
        public string GetCookTypeName(CookieType type)
        {
            string name = _CookiesNames[type];
            return !string.IsNullOrWhiteSpace(name) ? name : string.Empty;
        }

        public string GetCookieValue(HttpRequestBase request, CookieType type)
        {
            string value = string.Empty;
            if (request.Cookies[GetCookTypeName(type)] != null)
                value = request.Cookies.Get(GetCookTypeName(type)).Value;
            return value;
        }

        public decimal GetCookieValueDecimal(HttpRequestBase requst, CookieType type)
        {
            decimal value = 0;
            string name = GetCookTypeName(type);
            if (requst.Cookies[name] != null)
            {
                string origValue = requst.Cookies.Get(name).Value;
                Decimal.TryParse(origValue, out value);
            }
            return value;
        }

        public void SetToCookie(HttpResponseBase resp, CookieType type, string value)
        {
            HttpCookie cookie = new HttpCookie(GetCookTypeName(type), value);
            resp.Cookies.Remove(GetCookTypeName(type));
            resp.SetCookie(cookie);
        }

        /// <summary>Method delete a cookie from the user browser.</summary>
        /// <param name="resp">Response, which will send to user.</param>
        /// <param name="type">Type of a cookie to delete.</param>
        public void DeleteCookie(HttpResponseBase resp, CookieType type)
        {
            if (resp.Cookies[GetCookTypeName(type)] != null)
            {
                var cook = resp.Cookies.Get(GetCookTypeName(type));
                cook.Expires = DateTime.Now.AddDays(-1);
                resp.Cookies.Add(cook);
            }
        }
    }
}