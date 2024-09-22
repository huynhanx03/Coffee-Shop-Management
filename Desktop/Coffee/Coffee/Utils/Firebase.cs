using FireSharp;
using FireSharp.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.Utils
{
    public class Firebase : IDisposable
    {
        private IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "OE2s7YKNZ21b1UPiSRbOhgwzdbdKDCmZjz8sVXGj",
            BasePath = "https://coffee-4053c-default-rtdb.asia-southeast1.firebasedatabase.app/"
        };

        public FirebaseClient Client { get; private set; }

        public Firebase()
        {
            Client = new FirebaseClient(config);
        }

        public void Dispose()
        {
            Client = null;
        }
    }
}
