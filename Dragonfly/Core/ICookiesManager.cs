using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Dragonfly.Core
{
    /// <summary>
    /// The interface implements classes which manage the cookies in the system.
    /// </summary>
    interface ICookiesManager
    {
        /// <summary>
        /// Method gets the cookie name by it type.
        /// </summary>
        /// <param name="type">Type of the cookie.</param>
        /// <returns>Name of the cookie.</returns>
        string GetCookTypeName(CookieType type);

        string GetCookieValue(HttpRequestBase request, CookieType type);

        decimal GetCookieValueDecimal(HttpRequestBase requst, CookieType type);

        void SetToCookie(HttpResponseBase resp, CookieType type, string value);

        /// <summary>Method delete a cookie from the user browser.</summary>
        /// <param name="resp">Response, which will send to user.</param>
        /// <param name="type">Type of a cookie to delete.</param>
        void DeleteCookie(HttpResponseBase resp, CookieType type);
    }
}
