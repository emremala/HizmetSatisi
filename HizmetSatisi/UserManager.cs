using OfficeAgent.Cryption;
using OfficeAgent.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeAgent
{
    public class UserManager
    {
        string _userId;
        string _userPwd;
        string _userNewPwd;

        LoginInfo _li;

        public LoginInfo UserInfo
        {
            get { return _li; }
            set { _li = value; }
        }

        public UserManager(string id, string pwd)
        {
            _userId = id;
            _userPwd = pwd;
        }

        public UserManager(string id, string pwd, string newPwd)
        {
            _userId = id;
            _userPwd = pwd;
            _userNewPwd = newPwd;
        }
        
        #region LoginInfo Class

        public LoginInfo CheckLogin()
        {
            LoginInfo li = new LoginInfo();

            string sql = "";
            List<SqlParameter> sPar = new List<SqlParameter>();

            using (DataManager dMan = new DataManager())
            {
                sql = "SELECT * FROM USR WHERE USRNM=@USRNM And PWD=@PWD";
                sPar.Add(new SqlParameter("USRNM", _userId));
                //sPar.Add(new SqlParameter("UserPwd", _userPwd));
                sPar.Add(new SqlParameter("PWD", CryptionHelper.Encrypt(_userPwd, "tb")));
                DataTable dt = dMan.ExecuteDataTable(sql, sPar);
                if (dt.Rows.Count == 0)
                {
                    //Kullanıcı Yok Demek
                    return null;
                }

                li.UserId = dt.Rows[0]["ID"].ToString();
                li.UserName = dt.Rows[0]["USRNM"].ToString();
                li.UserEmail = dt.Rows[0]["EMAIL"].ToString();
            }

            return li;
        }

        #endregion
    }
}
