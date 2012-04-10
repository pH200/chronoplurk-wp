using System;
using Plurto.Core;

namespace ChronoPlurk.Helpers
{
    public static class LoginHelper
    {
        public static bool IsLoginError(PlurkError error)
        {
            switch (error)
            {
                case PlurkError.RequiresLogin:
                case PlurkError.InvalidAccessToken:
                case PlurkError.MissingAccessToken:
                    return true;
            }
            return false;
        }
    }
}
