using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChronoPlurk.Services
{
    public interface INavigationInjectionRedirect
    {
        object GetRedirectedViewModel();
    }
}
