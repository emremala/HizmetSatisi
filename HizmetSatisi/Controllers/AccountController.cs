using CinemaWeb;
using OfficeAgent;
using OfficeAgent.Cryption;
using OfficeAgent.Data;
using OfficeAgent.Object;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace HizmetSatisi.Controllers
{
    public class AccountController : Controller
    {
        public DataSet dsUser = new DataSet();
        public static User UserData;

        #region LoginInfo Class Çağır

        LoginInfo _li;

        public LoginInfo UserInfo
        {
            get { return _li; }
        }

        #endregion

        public class UserList
        {
            public Guid ID { get; set; }
            public string USRNM { get; set; }
            public string PWD { get; set; }
            public string FULNM { get; set; }
            public string EMAIL { get; set; }
            public string CARDNO { get; set; }
            public string CVC { get; set; }
            public string STKDAY { get; set; }
            public string STKMONTH { get; set; }
        }

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Manage()
        {
            DataSet dsUser = new DataSet();
            string USRID = Session["USRIDv"].ToString();
            using (DataVw dMan = new DataVw())
            {
                dsUser = dMan.ExecuteView_S("USR", "*", USRID, "", "ID = ");
            }

            List<UserList> userList = new List<UserList>();
            foreach (DataRow dr in dsUser.Tables[0].Rows)
            {
                userList.Add(new UserList
                {
                    ID = (Guid)dr["ID"],
                    USRNM = dr["USRNM"].ToString(),
                    PWD = CryptionHelper.Decrypt(dr["PWD"].ToString(), "tb"),
                    EMAIL = dr["EMAIL"].ToString(),
                    FULNM = dr["FULNM"].ToString(),
                    CARDNO = dr["CARDNO"].ToString(),
                    CVC = dr["CVC"].ToString(),
                    STKDAY = dr["STKDAY"].ToString(),
                    STKMONTH = dr["STKMONTH"].ToString()
                });
            }

            ViewBag.UserList = userList;

            return View();
        }


        [HttpPost]
        public ActionResult Control(string txtUsername, string txtPassword)
        {
            UserManager uMan = new UserManager(txtUsername, txtPassword);
            _li = uMan.CheckLogin();

            using (DataVw dMan = new DataVw())
            {
                dsUser = dMan.ExecuteView_S("USR", "*", txtUsername, "", "USRNM =");
            }

            if (dsUser.Tables[0].Rows.Count > 0)
            {
                DataRow row = dsUser.Tables[0].Rows[0];

                UserData = new User();
                UserData.USRID = (Guid)row["ID"];

                UserData.USRNM = Convert.ToString(row["USRNM"]);
                UserData.FULNM = Convert.ToString(row["FULNM"]);
                UserData.AVATAR = Convert.ToString(row["AVATAR"]);
                UserData.Email = Convert.ToString(row["EMAIL"]);
                UserData.IS_ADMIN = Convert.ToBoolean(row["IS_ADMIN"]);
                UserData.IS_SYSADM = Convert.ToBoolean(row["IS_SYSADM"]);
                UserData.IS_HR = Convert.ToBoolean(row["IS_HR"]);

                if (txtUsername.ToString() == row["USRNM"].ToString() && txtPassword.ToString() == CryptionHelper.Decrypt(row["PWD"].ToString(), "tb").ToString())
                {
                    Session["USRIDv"] = row["ID"].ToString();
                    Session["name"] = row["FULNM"].ToString();
                    //Session["admin"] = true;
                    //Session["loginError"] = true;
                    Session["IsAuthenticated"] = true;
                    Session["ADMIN"] = row["IS_SYSADM"].ToString();

                    if (row["IS_SYSADM"].ToString() == "True")
                    {
                        //Session["loginRoles"] = true;
                        //Session["admin"] = true;
                        if (row["AVATAR"].ToString() == "")
                        {
                            Session["avatarimg"] = "~/images/profil/nullavatar.jpg";
                        }
                        else
                        {
                            Session["avatarimg"] = row["AVATAR"].ToString();
                        }

                        return Redirect("/Home/Admin");
                    }
                    else
                    {
                        Session["IsAuthenticated"] = true;
                        Session["loginRoles"] = false;
                        Session["admin"] = false;
                        if (row["AVATAR"].ToString() == "")
                        {
                            Session["avatarimg"] = "~/images/profil/nullavatar.jpg";
                        }
                        else
                        {
                            Session["avatarimg"] = row["AVATAR"].ToString();
                        }
                    }

                    return Redirect("/Home/Index");
                }

                Session["loginError"] = true;
                Session["IsAuthenticated"] = false;

                //int loginErrorCount = Convert.ToInt32(Session["wrongpiece"]);

                //Session["wrongpiece"] = loginErrorCount + 1;
                //Session["wrongdate"] = DateTime.Now;
                //Session["IP"] = GetIp();

                return Redirect("/Account/Login");
            }
            else
            {
                Session["loginError"] = true;
                return Redirect("/Account/Login");
            }
        }

        [HttpPost]
        public ActionResult UserAdd(string txtUSRNM, string txtFULNM, string txtPWD, string txtEMAIL, HttpPostedFileBase file)
        {
            string filefo = "";
            using (DataVw dMan = new DataVw())
            {
                dsUser = dMan.ExecuteView_S("USR", "*", "", "", "");
            }

            if (txtUSRNM.ToString() == "" || txtFULNM.ToString() == "" || txtPWD.ToString() == "" || txtEMAIL.ToString() == "")
            {
                Session["useraddsuccess"] = false;
                ViewBag.addmessage = "Eksik veri girişi! Tüm Alanları Doldurunuz.";
                return Redirect("/Account/Register");
            }
            else
            {
                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/images/profil"), pic);
                    string pathd = "~/images/profil/" + pic;
                    // file is uploaded
                    file.SaveAs(path);
                    filefo = pathd;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }

                }

                DataRow newrow = dsUser.Tables[0].NewRow();
                newrow["ID"] = Guid.NewGuid();
                newrow["USRNM"] = txtUSRNM;
                newrow["FULNM"] = txtFULNM;
                newrow["EMAIL"] = txtEMAIL;
                newrow["PWD"] = CryptionHelper.Encrypt(txtPWD, "tb");
                newrow["IS_ADMIN"] = 1;
                newrow["IS_SYSADM"] = 0;
                newrow["IS_HR"] = 0;
                newrow["CHNG_PWD"] = 0;
                if (filefo == "")
                {
                    newrow["AVATAR"] = "~/images/profil/nullavatar.jpg";
                }
                else
                {
                    newrow["AVATAR"] = filefo;
                }
                newrow["AVATAR"] = filefo;
                newrow["EDATE"] = DateTime.Now;
                //newrow["EUSRID"] = null;
                //newrow["UDATE"] = DateTime.Now;
                //newrow["UUSRID"] = null;
                newrow["NOTE"] = "En Son Kayıt İşlemi Gerçekleştirdi.";
                AgentGc data = new AgentGc();
                string veri = data.DataAdded("USR", newrow, dsUser.Tables[0]);
                Session["useraddsuccess"] = true;
                ViewBag.addmessageinfo = veri;
                return Redirect("/Account/Login");
            }
        }

    }
}