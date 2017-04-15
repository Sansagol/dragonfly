using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Dragonfly.Core.UserAccess
{
    public interface IUserStateManager
    {
        bool CheckUserAccess(HttpRequestBase request);
    }
}
