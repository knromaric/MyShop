using System.Security.Principal;
using System.Web;

namespace MyShop.WebUI.Tests.Mocks
{
    public class HttpMockContext: HttpContextBase
    {
        private readonly MockRequest _request;
        private readonly MockResponse _response;
        private readonly HttpCookieCollection _cookies;
        private IPrincipal FakeUser;

        public HttpMockContext()
        {
            _cookies = new HttpCookieCollection();
            _request = new MockRequest(_cookies);
            _response = new MockResponse(_cookies);
        }

        public override IPrincipal User 
        {
            get
            {
                return this.FakeUser;
            }
            set
            {
                this.FakeUser = value; 
            }
        }



        public override HttpRequestBase Request => _request;
        public override HttpResponseBase Response => _response;



    }

    public class MockResponse: HttpResponseBase
    {
        private readonly HttpCookieCollection _cookies;

        public MockResponse(HttpCookieCollection cookies)
        {
            _cookies = cookies;
        }

        public override HttpCookieCollection Cookies => _cookies;
    }

    public class MockRequest : HttpRequestBase
    {
        private readonly HttpCookieCollection _cookies;

        public MockRequest(HttpCookieCollection cookies)
        {
            _cookies = cookies;
        }

        public override HttpCookieCollection Cookies => _cookies;
    }

}
