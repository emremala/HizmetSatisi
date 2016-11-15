using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeAgent
{
    public class LoginInfo
    {
        string _id;

        public string UserId
        {
            get { return _id; }
            set { _id = value; }
        }
        string _ad;

        public string UserName
        {
            get { return _ad; }
            set { _ad = value; }
        }
        string _email;

        public string UserEmail
        {
            get { return _email; }
            set { _email = value; }
        }

        public LoginInfo()
        {

        }
    }
}
