using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.DesignPattern.Mediator
{
    public interface IMediator
    {
        Task Notify(object sender, string message);
    }

}
