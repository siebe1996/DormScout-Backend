using Models.Users;
using Models.UsersRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.interfaces
{
    public interface IUserRepository
    {
        Task<PostAuthenticateResponseModel> Authenticate(PostAuthenticateRequestModel postAuthenticateRequestModel, string ipAddress);
        Task<PostAuthenticateResponseModel> RenewToken(string token, string ipAddress);
        Task DeactivateToken(string token, string ipAddress);
        Task<List<GetUserModel>> GetUsers();
        Task<GetUserModel> GetUser(Guid id);
        Task<GetUserModel> GetUser();
        Task<GetUserModel> PostUser(PostUserModel postUserModel, string ipAddress);
        Task<GetUserModel> PutUser(Guid id, PutUserModel putUserModel);
        Task<bool> ResetPassword(ResetPasswordModel resetPasswordModel);
        //Task<bool> ForgotPassword(ForgotPasswordModel forgotPasswordModel);
    }
}
