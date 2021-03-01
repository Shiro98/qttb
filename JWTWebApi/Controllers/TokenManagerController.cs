using JWTWebApi.Service;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http; 

namespace JWTWebApi.Controllers
{

    
    public class TokenManagerController : ApiController
    {
        private readonly ITokenService tokenService = new TokenService();
        public HttpRequestMessage RequestResponse { get; set; }

        [HttpGet]
        public HttpResponseMessage GenerateToken(string userName)
        {  
            if (!string.IsNullOrEmpty(userName))
                return Request.CreateResponse(HttpStatusCode.OK, value: tokenService.GenerateAccessToken(userName));
            else
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, message: "Tài khoản không hợp lệ!"); 
        }
        [HttpGet]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetValidateToken()
        {
            return Request.CreateResponse(HttpStatusCode.OK, value: "Tài khoản hợp lệ!");
        }
    }
}
