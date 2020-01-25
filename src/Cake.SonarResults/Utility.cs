using System;
using System.Collections.Generic;
using System.Text;

namespace Cake.SonarResults
{
    public static class Utility
    {
        public static bool ValidateUrl(string url)
        {
            Uri uriResult;

            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (result) return true;
            return false;
        }
    }
}
