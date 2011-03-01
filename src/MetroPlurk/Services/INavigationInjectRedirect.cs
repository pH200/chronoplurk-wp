using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroPlurk.Services
{
    public interface INavigationInjectionRedirect
    {
        object GetRedirectedViewModel();
    }
}
