using DataAccessLayer.Repositories.interfaces;
using Globals.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Backend_DormScout.Middleware
{
    public class JwtRefreshMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;

        public JwtRefreshMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
        }

        public async Task Invoke(HttpContext context, IUserRepository _userRepository)
        {
            var accessToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (!string.IsNullOrEmpty(accessToken))
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

                    SecurityToken validatedToken;
                    var principal = tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero // disable clock skew to compare the times exactly
                    }, out validatedToken);

                    // Check if the token has expired
                    var expiryTimeUnix = principal.Claims.FirstOrDefault(x => x.Type == "exp")?.Value;
                    if (expiryTimeUnix != null)
                    {
                        var expiryTimeUtc = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiryTimeUnix));
                        if (expiryTimeUtc <= DateTime.UtcNow)
                        {
                            /*string refreshToken = Request.Cookies["Swimmingclub.RefreshToken"];
                            PostAuthenticateResponseModel postAuthenticateResponseModel = await _memberRepository.RenewToken(refreshToken, IpAddress());
                            SetTokenCookie(postAuthenticateResponseModel.RefreshToken);*/
                            // Token has expired, refresh it
                            //var refreshToken = principal.Claims.FirstOrDefault(x => x.Type == "refresh_token")?.Value;
                            /*var refreshToken = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "refresh_token")?.Value;
                            if (!string.IsNullOrEmpty(refreshToken))
                            {
                                var refreshedToken = await _memberRepository.RenewToken(refreshToken, httpContextAccessor.HttpContext.Connection.LocalIpAddress.MapToIPv4().ToString());
                                if (refreshedToken != null)
                                {
                                    // Set the new access token in the response header
                                    context.Response.Headers.Add("Authorization", "Bearer " + refreshedToken.RefreshToken);
                                }
                            }*/

                            string refreshToken = context.Request.Cookies["Backend_DormScout.RefreshToken"];
                            var refreshedToken = await _userRepository.RenewToken(refreshToken, context.Connection.LocalIpAddress.MapToIPv4().ToString());
                            CookieOptions cookieOptions = new()
                            {
                                HttpOnly = true,
                                Expires = DateTime.UtcNow.AddMinutes(20), //TOKEN REFRESH
                                IsEssential = true,
                            };
                            context.Request.Headers.Add("Authorization", refreshedToken.JwtToken);

                            context.Response.Cookies.Append("Backend_DormScout.RefreshToken", refreshedToken.RefreshToken, cookieOptions);
                        }
                    }
                }
                catch (SecurityTokenValidationException)
                {
                    // Token is invalid or has been tampered with
                }
                catch (Exception ex)
                {
                    // Handle other exceptions
                }
            }

            await _next(context);
        }

        /*private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                return Request.Headers["X-Forwarded-For"];
            }
            else
            {
                return httpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

            }

        }

        private void SetTokenCookie(string token)
        {
            CookieOptions cookieOptions = new()
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddMinutes(2), //TOKEN REFRESH
                IsEssential = true,
            };

            Response.Cookies.Append("Swimmingclub.RefreshToken", token, cookieOptions);
        }*/
    }
}
