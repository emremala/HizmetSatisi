using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CinemaWeb
{
    [Serializable()]
    public class User
    {
        public User()
        {

        }

        //public new()
        //{

        //}

        public Guid USRID;
        public string USRNM;
        public string FULNM;
        public string AVATAR;
        public bool IS_SYSADM;      //Admin
        public bool IS_ADMIN;       //Kullanıcı
        public bool IS_HR;          //Personel

        public string Email;

        private DataSet _UserRights;
        public DataSet UserRights
        {
            get { return _UserRights; }
            set { _UserRights = value; }
        }
    }
}