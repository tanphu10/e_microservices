using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Sagas.OrderManager
{
    public interface ISagaOrderManager<in TTInput,out TOutput> where TTInput:class where TOutput:class
    {

        public TOutput CreateOrder(TTInput input);
        public TOutput RollbackOrder(string username,string documentNo,long orderId);


    }
}
