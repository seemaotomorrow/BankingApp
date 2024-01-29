using System.Net;
using System.Text;

namespace AdminWebsiteTest;

public class FakeHttpMessageHandler : HttpMessageHandler
{
    private HttpResponseMessage _fakeResponse;

    public void SetFakeResponse(HttpStatusCode statusCode, string content = null)
    {
        _fakeResponse = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(content ?? "", Encoding.UTF8, "application/json")
        };
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
    {
        return Task.FromResult(_fakeResponse);
    }
}
