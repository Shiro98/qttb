using System.Net;
using System.Net.Http;
using System.Net.Http.Headers; 
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace JWTWebApi.Service
{
    public class CustomAuthenticationFilter : AuthorizeAttribute, IAuthenticationFilter
    {
        private readonly ITokenService _tokenService = new TokenService();
        public bool AllowMultiple
        {
            get { return false; }
        }
        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = context.Request;
            AuthenticationHeaderValue authorization = request.Headers.Authorization;
            if (authorization == null)
            {
                context.ErrorResult = new AuthenticationFailureResul(reasonPhrase: "Thiếu thông tin Header!", request);
                return;
            }
            if (authorization == null && authorization.Scheme != "Bearer")
            {
                context.ErrorResult = new AuthenticationFailureResul(reasonPhrase: "Authorization Schema không đúng!", request);
                return;
            }

            string[] TokenAndUser = authorization.Parameter.Split(':');
            var Token = TokenAndUser[0];
            var userName = TokenAndUser[1]; 

            if (string.IsNullOrEmpty(Token))
            {
                context.ErrorResult = new AuthenticationFailureResul(reasonPhrase: "Token hết hiệu lực!", request);
                return;
            }
            var validUserName = _tokenService.ValidateToken(Token);

            if (userName != validUserName)
            {
                context.ErrorResult = new AuthenticationFailureResul(reasonPhrase: "Token người dùng không khớp!", request);
                return;
            }

            context.Principal = _tokenService.GetPrincipal(Token);
            
        }

        public async Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            var result = await context.Result.ExecuteAsync(cancellationToken);
            if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                result.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(scheme: "Basic", parameter: "realm=localhost"));
            }
            context.Result = new ResponseMessageResult(result);
        }
        public class AuthenticationFailureResul : IHttpActionResult
        {
            public string ReasonPhrese;
            public HttpRequestMessage Request { get; set; }
            public AuthenticationFailureResul(string reasonPhrase, HttpRequestMessage request)
            {
                ReasonPhrese = reasonPhrase;
                Request = request;
            }
            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult(Execute());
            }
            public HttpResponseMessage Execute()
            {
                HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                responseMessage.RequestMessage = Request;
                responseMessage.ReasonPhrase = ReasonPhrese;
                return responseMessage;
            }
        }
       
    }
}