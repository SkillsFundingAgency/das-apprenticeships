using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace SFA.DAS.Apprenticeships.TestHelpers;

public static class WebJobsBuilderExtensions
{
    public static IWebJobsBuilder ConfigureServices(this IWebJobsBuilder builder, Action<IServiceCollection> configure)
    {
        configure(builder.Services);
        return builder;
    }
}

public sealed class DummyHttpRequest : HttpRequest
{
    private readonly string _content;
    private Stream _stream;

    public DummyHttpRequest() : this("")
    {
    }

    public DummyHttpRequest(string content)
    {
        HttpContext = new DummyHttpContext(this);
        _content = content;
    }
    public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = new CancellationToken()) =>
        Task.FromResult((IFormCollection)new FormCollection(new Dictionary<string, StringValues>()));

    public override HttpContext HttpContext { get; }
    public override string Method { get; set; } = "Get";
    public override string Scheme { get; set; } = "http";
    public override bool IsHttps { get; set; }
    public override HostString Host { get; set; } = new HostString("dummy");
    public override PathString PathBase { get; set; }
    public override PathString Path { get; set; }
    public override QueryString QueryString { get; set; }
    public override IQueryCollection Query { get; set; } = new QueryCollection();
    public override string Protocol { get; set; }
    public override IHeaderDictionary Headers { get; } = new HeaderDictionary();
    public override IRequestCookieCollection Cookies { get; set; }
    public override long? ContentLength { get; set; }
    public override string ContentType { get; set; }

    public override Stream Body
    {
        get => _stream ??= new MemoryStream(Encoding.UTF8.GetBytes(_content));
        set => _stream = value;
    }

    public override bool HasFormContentType { get; } = false;
    public override IFormCollection Form { get; set; }
}

public class DummyHttpContext : HttpContext
{
    public DummyHttpContext(HttpRequest request)
    {
        Request = request;
    }

    public override void Abort()
    {
    }

    public override IFeatureCollection Features { get; } = new FeatureCollection();
    public override HttpRequest Request { get; }
    public override HttpResponse Response { get; } = null;
    public override ConnectionInfo Connection { get; } = null;
    public override WebSocketManager WebSockets { get; } = null;
    public override ClaimsPrincipal User { get; set; }
    public override IDictionary<object, object> Items { get; set; } = new Dictionary<object, object>();
    public override IServiceProvider RequestServices { get; set; }
    public override CancellationToken RequestAborted { get; set; }
    public override string TraceIdentifier { get; set; }
    public override ISession Session { get; set; }
    [Obsolete("Is obsolete")]
    public override AuthenticationManager Authentication { get; } = null;
}