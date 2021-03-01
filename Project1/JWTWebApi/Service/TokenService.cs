using Microsoft.IdentityModel.Tokens;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Data;
using JWTWebApi.ADOConnection;
using JWTWebApi.Models;
using System.Net.Http;
using System.Net;
using System.Collections.Generic;
using System.Linq;

namespace JWTWebApi.Service
{
    public class TokenService : ITokenService
    {
        private static string _securityKey = ConfigurationManager.AppSettings["SecurityKey"];
        private static string _accessTokenDurationInMinutes = ConfigurationManager.AppSettings["AccessTokenDurationInMinutes"];
        public HttpRequestMessage Request { get; set; }

        public string GenerateAccessToken(string userName)
        {
            byte[] key = Convert.FromBase64String(_securityKey);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, value: userName) }),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_accessTokenDurationInMinutes)),
                SigningCredentials = new SigningCredentials(securityKey, algorithm: SecurityAlgorithms.HmacSha256Signature)
            };
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            //Insert to database
            //InsertToken(handler.WriteToken(token));
            return handler.WriteToken(token);
        }
        //Gọi khi đã xác định được chính xác người dùng
        public string ValidateToken(string token)
        {
            ClaimsPrincipal principal = GetPrincipal(token);
            if (principal == null)
                return "";
            ClaimsIdentity identity;
            try
            {
                identity = (ClaimsIdentity)principal.Identity;
            }
            catch (NullReferenceException) { return ""; }
            Claim usernameClaim = identity.FindFirst(type: ClaimTypes.Name);
            string username = usernameClaim.Value;
            return username;
        }
        //Gọi khi chưa xác định được người dùng.
        public string ValidateTokenByIpMachine(string ipMachine)
        {
            try
            {
                using (var db = new Database(ConfigurationConnectString.FoldioContext))
                {
                    using (var connection = new SqlConnection(ConfigurationConnectString.FoldioContext))
                    {
                        connection.Open();
                        var getToke = db.NewSpCommand("ST_GET_TOKEN_BY_IPMACHINE")
                            .Parameter("@_IpMachine", ipMachine)
                            .ExecuteNonQueryNoOpenConnect(connection, new SqlParameter("@_outToken", DbType.String));

                        connection.Close();
                        if (!string.IsNullOrEmpty(getToke))
                        {
                            ClaimsPrincipal principal = GetPrincipal(getToke);
                            if (principal == null)
                                return "";
                            ClaimsIdentity identity;
                            try
                            {
                                identity = (ClaimsIdentity)principal.Identity;
                            }
                            catch (NullReferenceException) { return ""; }
                            Claim usernameClaim = identity.FindFirst(type: ClaimTypes.Name);
                            string username = usernameClaim.Value;
                            return username;
                        }
                        return "";
                    }
                }
            }
            catch
            {
                return "";
            }
        }

        public ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null)
                    return null;
                byte[] key = Convert.FromBase64String(_securityKey);
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, parameters, out securityToken);
                return principal;
            }
            catch (NullReferenceException) { return null; }
        }
        public string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        public void InsertToken(string token, string user)
        {
            try
            {
                using (var db = new Database(ConfigurationConnectString.FoldioContext))
                {
                    using (var connection = new SqlConnection(ConfigurationConnectString.FoldioContext))
                    {
                        connection.Open();
                        //dbcmd.Parameters.Add("", VanBanDi.Ten);
                        var rsl = db.NewSpCommand("ST_ADD_NEW_TOKEN")
                            .Parameter("@_Token", token)
                            .Parameter("@_UserName", user)
                            .Parameter("@_IpMachine", "1")
                            .ExecuteNonQueryNoOpenConnect(connection, new SqlParameter("@_ResponseStatus", DbType.Int32));
                        var idrsl = Convert.ToInt32(rsl);
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetUrlByAppCode(string appCode)
        {
            var uri = "";
            if(string.IsNullOrEmpty(appCode)) return uri;
            try
            { 
                using (var db = new Database(ConfigurationConnectString.FoldioContext))
                {
                    var sqlCommand = new SqlCommand("ST_GET_URL_BY_APPCODE");
                    var strAppCode = new SqlParameter("@_AppCode", SqlDbType.NVarChar);
                    strAppCode.Value = appCode;
                    sqlCommand.Parameters.Add(strAppCode); 
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    var lRet = db.ExecuteListOutput<AppModel>(sqlCommand);
                    if (lRet.Any())
                        uri = lRet.FirstOrDefault().Url;
                    return uri;
                }
            }
            catch(Exception) { return ""; }
        }
        public int CheckLogin(string userName, string password)
        {
            var result = GetUserInfo(userName, password);
            if (result == null)
                return 0;
            else
            {
                if (result.ENABLED_FLAG == "N")
                    return -1;
                else
                {
                    if (result.PASSWORD == password)
                        return 1;
                    else
                        return -2;
                }
            } 
        }
        public UserModel GetUserInfo(string userName, string password)
        {
            try
            {
                using (var db = new Database(ConfigurationConnectString.FoldioContext))
                {
                    var sqlCommand = new SqlCommand("ST_GET_USERINFO");
                    var loginName = new SqlParameter("@_UserName", SqlDbType.NVarChar);
                    loginName.Value = userName;
                    sqlCommand.Parameters.Add(loginName);
                    var strPassword = new SqlParameter("@_Password", SqlDbType.NVarChar);
                    strPassword.Value = password;
                    sqlCommand.Parameters.Add(strPassword);
                    
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    var lRet = db.ExecuteListOutput<UserModel>(sqlCommand);
                    return lRet.FirstOrDefault();
                }
            }
            catch(Exception ex){return new UserModel();}
        }
    }
}