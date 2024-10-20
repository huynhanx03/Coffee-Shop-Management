using Coffee.API;
using Coffee.ViewModel.AdminVM;
using Coffee.ViewModel.AdminVM.Customer;
using Coffee.ViewModel.AdminVM.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.DesignPattern.Mediator
{
    public class ConcreteMediator : IMediator
    {
        private static ConcreteMediator _ins;

        public static ConcreteMediator Ins
        {
            get
            {
                if (_ins == null)
                    _ins = new ConcreteMediator();
                return _ins;
            }
            private set
            {
                _ins = value;
            }
        }

        public OrderViewModel orderViewModel { get; set; }
        public MainAdminViewModel mainAdminViewModel { get; set; }
        
        public async Task Notify(object sender, string message)
        {
            var parts = message.Split(' ');

            if (parts[0] == "UpdateOrderCount")
            {
                mainAdminViewModel.UpdateOrderCount(int.Parse(parts[1]));
            }
            else if (parts[0] == "LoadOrderList")
            {
                await orderViewModel.loadOrderList();
            }
        }
    }
}
