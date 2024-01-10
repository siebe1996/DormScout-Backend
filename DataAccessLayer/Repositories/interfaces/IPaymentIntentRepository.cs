using Models.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.interfaces
{
    public interface IPaymentIntentRepository
    {
        Task<ClientSecretModel> Create(PaymentIntentCreateRequestModel request);
    }
}
