using System;
using System.Net;

namespace BackBox.iOS
{
    public class WebClientEx : WebClient
    {
        #region Private Members

        private readonly CookieContainer container = new CookieContainer();

        #endregion

        #region Protected Methods

        protected override WebRequest GetWebRequest(Uri address)
        {
            var r = base.GetWebRequest(address);
            var request = r as HttpWebRequest;

            if (request != null)
            {
                request.CookieContainer = container;
            }

            return r;
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            var response = base.GetWebResponse(request, result);

            ReadCookies(response);

            return response;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            var response = base.GetWebResponse(request);

            ReadCookies(response);

            return response;
        }

        #endregion

        #region Private Methods

        private void ReadCookies(WebResponse r)
        {
            var response = r as HttpWebResponse;
            if (response != null)
            {
                var cookies = response.Cookies;
                container.Add(cookies);
            }
        }

        #endregion
    }
}