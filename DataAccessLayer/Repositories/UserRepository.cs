using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using DataAccessLayer.Repositories.interfaces;
using Globals.Entities;
using Globals.Enums;
using Globals.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models.Coordinates;
using Models.Notes;
using Models.PlaceImages;
using Models.Places;
using Models.PossibleDates;
using Models.Users;
using Newtonsoft.Json.Linq;
using Stripe;

namespace DataAccessLayer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly AppSettings _appSettings;
        private readonly RoleManager<Role> _roleManager;
        private readonly Backend_DormScoutContext _context;
        private readonly ClaimsPrincipal _user;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserRepository(Backend_DormScoutContext backend_DormScoutContext, SignInManager<User> signInManager, UserManager<User> userManager, IOptions<AppSettings> appSettings, RoleManager<Role> roleManager, IHttpContextAccessor httpContextAccessor)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _appSettings = appSettings.Value;
            _context = backend_DormScoutContext;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _user = _httpContextAccessor.HttpContext.User;
        }

        public async Task<string> GeneratePasswordResetToken(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }
            try
            {
                string test = await _userManager.GeneratePasswordResetTokenAsync(user);
                return test;
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                throw new Exception("Error generating password reset token.", ex);
            }
        }

        public async Task<bool> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            User user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            //IdentityResult result = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.NewPassword);
            //this is because i dont use it from email otherwise it needs to be in model
            string resetToken = await GeneratePasswordResetToken(user);
            IdentityResult result = await _userManager.ResetPasswordAsync(user, resetToken, resetPasswordModel.NewPassword);
            return result.Succeeded;
        }

        /*public async Task<bool> ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            User user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);
            if (user == null)
            {
                // User not found, handle accordingly (e.g., do not reveal this information to the user)
                return false;
            }

            string resetToken = await GeneratePasswordResetToken(user);

            // TODO: Send the reset token to the user (e.g., via email)

            return true;
        }*/

        public async Task<string> GenerateJwtToken(User user)
        {
            var roleNames = await _userManager.GetRolesAsync(user).ConfigureAwait(false);

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim("Email", user.Email)/*,
                new Claim("UserName", user.UserName)*/
            };

            foreach (string roleName in roleNames)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleName));
            }

            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Issuer = "Backend_DormScout Web API",
                Subject = new ClaimsIdentity(claims.ToArray()),
                Expires = DateTime.UtcNow.AddSeconds(600), //TOKEN JWT change for experation time
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            byte[] randomBytes = RandomNumberGenerator.GetBytes(64);

            //The refresh token expires time must be the same as the refresh token cookie expires time set in the SetTokenCookie method in UserController
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddMinutes(2), //TOKEN REFRESH
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress,
            };
        }

        public async Task<PostAuthenticateResponseModel> Authenticate(PostAuthenticateRequestModel postAuthenticateRequestModel, string ipAddress)
        {
            User user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == postAuthenticateRequestModel.Email);

            if (user == null)
            {
                throw new Exception("invalid e-mail");
            }

            //Verify password when user was found by UserName
            SignInResult signInResult = await _signInManager.CheckPasswordSignInAsync(user, postAuthenticateRequestModel.Password, lockoutOnFailure: false);

            if (!signInResult.Succeeded)
            {
                throw new Exception("invalid password");
            }

            //Authentication was successfull so generate JWT and refresh tokens
            string jwtToken = await GenerateJwtToken(user);
            RefreshToken refreshToken = GenerateRefreshToken(ipAddress);

            //Save refresh token
            user.RefreshTokens.Add(refreshToken);

            await _userManager.UpdateAsync(user);

            return new PostAuthenticateResponseModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                ImageData = user.ImageData,
                UserName = user.UserName,
                Score = user.Score,
                Gender = user.Gender,
                PhoneNumber = user.PhoneNumber,
                Country = user.Country,
                Province = user.Province,
                City = user.City,
                PostalCode = user.PostalCode,
                Address = user.Address,
                JwtToken = jwtToken,
                RefreshToken = refreshToken.Token,
                Roles = await _userManager.GetRolesAsync(user),
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
                
            };
        }

        public async Task<PostAuthenticateResponseModel> RenewToken(string token, string ipAddress)
        {
            User user = await _userManager.Users.FirstOrDefaultAsync(x => x.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
            {
                throw new Exception("no user was found with this token");
            }

            RefreshToken refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            //Refresh token is no longer active
            if (!refreshToken.IsActive)
            {
                throw new Exception("refresh token is expired");
            }

            //Replace old refresh token with a new one
            RefreshToken newRefreshToken = GenerateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;

            //Generate new jwt
            string jwtToken = await GenerateJwtToken(user);

            user.RefreshTokens.Add(refreshToken);

            await _userManager.UpdateAsync(user);

            return new PostAuthenticateResponseModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                ImageData = user.ImageData,
                UserName = user.UserName,
                Score = user.Score,
                Gender = user.Gender,
                PhoneNumber = user.PhoneNumber,
                Country = user.Country,
                Province = user.Province,
                City = user.City,
                PostalCode = user.PostalCode,
                Address = user.Address,
                JwtToken = jwtToken,
                RefreshToken = newRefreshToken.Token,
                Roles = await _userManager.GetRolesAsync(user),
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task DeactivateToken(string token, string ipAddress)
        {
            User user = await _userManager.Users.FirstOrDefaultAsync(x => x.RefreshTokens.Any(x => x.Token == token));

            if (user == null)
            {
                throw new Exception("no user was found with this token");
            }

            RefreshToken refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            //Refresh token is no longer active
            if (!refreshToken.IsActive)
            {
                throw new Exception("refresh token is expired");
            }

            //Revoke token and save
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;

            await _userManager.UpdateAsync(user);
        }

        public async Task<List<GetUserModel>> GetUsers()
        {
            List<GetUserModel> users = await _context.Users.Select(x => new GetUserModel
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                DateOfBirth = x.DateOfBirth,
                Email = x.Email,
                ImageData = x.ImageData,
                UserName = x.UserName,
                Score = x.Score,
                Gender = x.Gender,
                PhoneNumber = x.PhoneNumber,
                Country = x.Country,
                Province = x.Province,
                City = x.City,
                PostalCode = x.PostalCode,
                Address = x.Address,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            }).AsNoTracking()
            .ToListAsync();

            return users;
        }

        public async Task<GetUserModel> GetUser(Guid id)
        {
            GetUserModel user = await _context.Users.Select(x => new GetUserModel
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                DateOfBirth = x.DateOfBirth,
                Email = x.Email,
                ImageData = x.ImageData,
                UserName = x.UserName,
                Score = x.Score,
                Gender = x.Gender,
                PhoneNumber = x.PhoneNumber,
                Country = x.Country,
                Province = x.Province,
                City = x.City,
                PostalCode = x.PostalCode,
                Address = x.Address,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            }).AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

            return user;
        }

        public async Task<GetUserModel> GetUser()
        {
            //need to get id
            Guid userId = new Guid(_user.Identity.Name);
            GetUserModel user = await _context.Users.Select(x => new GetUserModel
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                DateOfBirth = x.DateOfBirth,
                Email = x.Email,
                ImageData = x.ImageData,
                UserName = x.UserName,
                Score = x.Score,
                Gender = x.Gender,
                PhoneNumber = x.PhoneNumber,
                Country = x.Country,
                Province = x.Province,
                City = x.City,
                PostalCode = x.PostalCode,
                Address = x.Address,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            }).AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId);

            return user;
        }

        public async Task<GetUserModel> PostUser(PostUserModel postUserModel, string ipAddress)
        {
            User user = new User
            {
                FirstName = postUserModel.FirstName,
                LastName = postUserModel.LastName,
                DateOfBirth = postUserModel.DateOfBirth,
                Email = postUserModel.Email,
                ImageData = postUserModel.ImageData,
                UserName = postUserModel.UserName,
                Score = 2.5,
                Gender = postUserModel.Gender,
                PhoneNumber = postUserModel.PhoneNumber,
                Country = postUserModel.Country,
                Province = postUserModel.Province,
                City = postUserModel.City,
                PostalCode = postUserModel.PostalCode,
                Address = postUserModel.Address,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            IdentityResult result = await _userManager.CreateAsync(user, postUserModel.Password);
            _ = _userManager.AddToRoleAsync(user, "User").Result;
            //toDO not finshed stripe
            //this is code to create a stripe account

            /*DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            TimeSpan timeSpan = DateTime.UtcNow - unixEpoch;
            long unixTimestamp = (long)timeSpan.TotalSeconds;*/


            /*var optionsPaymentMethod = new PaymentMethodCreateOptions
            {
                Type = "card",
                Card = new PaymentMethodCardOptions
                {
                    Number = "4242424242424242",
                    ExpMonth = 8,
                    ExpYear = 2026,
                    Cvc = "314",
                },
            };
            var servicePaymentMethod = new PaymentMethodService();
            PaymentMethod paymentMethod = servicePaymentMethod.Create(optionsPaymentMethod);
            string paymentMethodId = paymentMethod.Id;*/

            /*var optionsAccount = new AccountCreateOptions
            {
                Type = "costum",
                Country = user.Country,
                Email = user.Email,
                Capabilities = new AccountCapabilitiesOptions
                {
                    CardPayments = new AccountCapabilitiesCardPaymentsOptions
                    {
                        Requested = true,
                    },
                    Transfers = new AccountCapabilitiesTransfersOptions { Requested = true },
                },
                BusinessType = "individual",
                Individual =
                {
                    Address =
                    {
                        City = user.City, Country = user.Country, Line1 = user.Address, PostalCode = user.PostalCode, State = user.Province
                    },
                    Dob =
                    {
                        Day = user.DateOfBirth.Day, Month = user.DateOfBirth.Month, Year = user.DateOfBirth.Year
                    },
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Gender = user.Gender.ToString().ToLower(),
                    Phone = user.PhoneNumber,
                },
                TosAcceptance =
                {
                    Date = DateTime.UtcNow,
                    Ip = ipAddress,
                }
                
            };

            var serviceAccount = new AccountService();
            Account account = serviceAccount.Create(optionsAccount);
            string accountId = account.Id;*/

            /*var optionsCustomer = new CustomerCreateOptions
            {
                Name = user.FirstName + " " + user.LastName,
                Email = user.Email,
                Address =
                    {
                        City = user.City, Country = user.Country, Line1 = user.Address, PostalCode = user.PostalCode, State = user.Province
                    },
                Phone = user.PhoneNumber,
                PaymentMethod = paymentMethodId
            };
            var serviceCustomer = new CustomerService();
            Customer customer = serviceCustomer.Create(optionsCustomer);
            string customerId = customer.Id;*/
            //toDo check if both are created correctly


            //user.StripeAccountId = accountId;
            //user.StripeCostumerId = customerId;
            await _context.SaveChangesAsync();
            //the end of the stripe account code

            GetUserModel usermodel = new GetUserModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                ImageData = user.ImageData,
                UserName = user.UserName,
                Score = user.Score,
                Gender = user.Gender,
                PhoneNumber = user.PhoneNumber,
                Country = user.Country,
                Province = user.Province,
                City = user.City,
                PostalCode = user.PostalCode,
                Address = user.Address,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
                //StripeAccountId = user.StripeAccountId,
            };

            return usermodel;
        }


        public async Task<GetUserModel> PutUser(Guid id, PutUserModel putUserModel)
        {
            User user = await _context.Users.FirstOrDefaultAsync(p => p.Id == id);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            if (putUserModel.UserName != null)
            {
                user.UserName = putUserModel.UserName;
            }

            if (putUserModel.DateOfBirth != null)
            {
                user.DateOfBirth = (DateTime)putUserModel.DateOfBirth;
            }

            if (putUserModel.ImageData != null)
            {
                user.ImageData = putUserModel.ImageData;
            }

            user.UpdatedAt = DateTime.UtcNow;


            await _context.SaveChangesAsync();

            GetUserModel usermodel = new GetUserModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                ImageData = user.ImageData,
                UserName = user.UserName,
                Score = user.Score,
                Gender = user.Gender,
                PhoneNumber = user.PhoneNumber,
                Country = user.Country,
                Province = user.Province,
                City = user.City,
                PostalCode = user.PostalCode,
                Address = user.Address,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            
            return usermodel;
        }
    }
}
