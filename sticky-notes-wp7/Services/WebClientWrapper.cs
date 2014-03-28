namespace StickyNotes.Services
{
    using System;
    using System.Net;
    using System.Security;

    /// <summary>
    /// Provides a wrapper for WebClient, which by default doesn't
    /// allow access to HttpWebRequest or HttpWebResponse objects.
    /// </summary>
    public class HttpWebClient : WebClient
    {
        // Required by the runtime to inherit the base
        [SecuritySafeCritical]
        public HttpWebClient() { }

        // Provide the last HttpWebResponse as a public property
        public HttpWebResponse HttpWebResponse { get; private set; }

        // Override the base method and intercept the response object
        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            try
            {
                return this.HttpWebResponse = base.GetWebResponse(request, result) as HttpWebResponse;
            }
            catch (WebException webException)
            {
                // Exceptions occur on non-2xx status codes
                return this.HttpWebResponse = webException.Response as HttpWebResponse;

                // Re-throw the exception after interception
                throw (webException);
            }
        }

        // Override the base method and intercept the request object
        public HttpWebRequest HttpWebRequest { get; private set; }

        protected override WebRequest GetWebRequest(System.Uri address)
        {
            return this.HttpWebRequest = base.GetWebRequest(address) as HttpWebRequest;
        }
    }
}