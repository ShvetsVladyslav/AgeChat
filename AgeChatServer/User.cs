using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeChatServer
{
    class User
    {
        public int id { get; set; }
        public string username { get; set; }

        private bool isOnline = false;

        public void Online()
        {
            isOnline = true;
        }
        public void Offline()
        {
            isOnline = false;
        }
        public bool IsOnline()
        {
            return isOnline;
        }
    }
}
