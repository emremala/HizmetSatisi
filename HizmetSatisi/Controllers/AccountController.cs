using CinemaWeb;
using Microsoft.Owin.Security;
using OfficeAgent;
using OfficeAgent.Cryption;
using OfficeAgent.Data;
using OfficeAgent.Object;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

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

            [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address")]
            public string EMAIL { get; set; }
            public string CARDNO { get; set; }
            public string AVATAR { get; set; }
            public string STATUS { get; set; }
            public string STKDAY { get; set; }
            public string STKMONTH { get; set; }
        }
        public class TenderList
        {
            public Guid ID { get; set; }
            public string TENDERNAME { get; set; }
            public string TENDERNOTE { get; set; }
            public string TENDERIMAGE { get; set; }
            public Guid TENDERUSRID { get; set; }

        }
        public class TenderDList
        {
            public Guid ID { get; set; }
            public Guid TENDERUSERID { get; set; }
            public string TENDERNAME { get; set; }
            public string FULNM { get; set; }
            public string COUNTRYNM { get; set; }
            public string TOWNNM { get; set; }
            public string TANDERDATE { get; set; }
            public string TEL { get; set; }
            public string EMAIL { get; set; }
            public string NOTE { get; set; }
            public string STATUS { get; set; }


        }

        // GET: Account


        public ActionResult Login()
        {
            return View();
        }

        #region Müşteri - Satıcı Kontrol İşlemi
        public ActionResult Register()
        {
            DataSet dsUserType = new DataSet();
            using (DataVw dMan = new DataVw())
            {
                dsUserType = dMan.ExecuteView_S("USR", "*", "", "", "ID = ");
            }
            List<SelectListItem> usrtyp = new List<SelectListItem>();

            usrtyp.Add(new SelectListItem { Text = "Müşteri", Value = "0" });
            usrtyp.Add(new SelectListItem { Text = "Satıcı", Value = "1" });

            ViewBag.USRTYP = usrtyp;

            return View();
        }
        #endregion
        #region Manage Sayfası Bilgilerini Görüntüleme
        public ActionResult UserUpdate(string btnUpdate,string txtFULNM,string txtUSRNM,string txtEMAIL, string txtPWD, HttpPostedFileBase file)
        {
            DataSet dsUSRUp = new DataSet();
            using (DataVw dMan = new DataVw())
            {
                dsUSRUp = dMan.ExecuteView_S("USR", "*", "", "", "");
            }
            DataSet dsİmage = new DataSet();
            using (DataVw dMan = new DataVw())
            {
                dsİmage = dMan.ExecuteView_S("USR", "AVATAR", btnUpdate, "", "ID=");
            }

            string filefo = "";
            if (file != null)
            {
                string pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/images/tender"), pic);
                string pathd = "~/images/tender/" + pic;
                // file is uploaded
                file.SaveAs(path);
                filefo = pathd;
                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                }
            }
            else
            {
                filefo = dsİmage.Tables[0].Rows[0][0].ToString();
            }
            
            DataRow newrow = dsUSRUp.Tables[0].Rows[0];
            newrow["ID"] =btnUpdate;
            newrow["USRNM"] = txtUSRNM;
            newrow["PWD"] = CryptionHelper.Encrypt(txtPWD, "tb");
            newrow["FULNM"] = txtFULNM;
            newrow["EMAIL"] = txtEMAIL;
            newrow["AVATAR"] =filefo;
            AgentGc data = new AgentGc();
            string veri = data.DataModified("USR", newrow, dsUSRUp.Tables[0]);
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
                string Status = "";
                if (dr["IS_SYSADM"].ToString() == "true") { Status = "Admin Hesabı"; }
                if (dr["IS_ADMIN"].ToString() == "true") { Status = "Müşteri Hesabı"; }
                if (dr["IS_HR"].ToString() == "true") { Status = "Satıcı Hesabı"; }
                userList.Add(new UserList
                {
                    ID = (Guid)dr["ID"],
                    USRNM = dr["USRNM"].ToString(),
                    PWD = CryptionHelper.Decrypt(dr["PWD"].ToString(), "tb"),
                    EMAIL = dr["EMAIL"].ToString(),
                    FULNM = dr["FULNM"].ToString(),
                    
                });
            }

            ViewBag.UserList = userList;
            DataSet dsManage = new DataSet();
            string USRIDMNG = Session["USRIDv"].ToString();
            using (DataVw dMan = new DataVw())
            {
                dsManage = dMan.ExecuteView_S("USR", "*", USRIDMNG, "", "ID = ");
            }

            List<UserList> userLManage = new List<UserList>();
            foreach (DataRow dr in dsManage.Tables[0].Rows)
            {


                userLManage.Add(new UserList
                {
                    ID = (Guid)dr["ID"],
                    USRNM = dr["USRNM"].ToString(),
                    PWD = CryptionHelper.Decrypt(dr["PWD"].ToString(), "tb"),
                    EMAIL = dr["EMAIL"].ToString(),
                    FULNM = dr["FULNM"].ToString(),
                    AVATAR=dr["AVATAR"].ToString(),

                });
            }

            ViewBag.UserUpdate = userLManage;
            return View();
        }
        #endregion
        #region Kullanıcı Bilgi Güncelleme
        public ActionResult SelectUserInfo(System.Web.Mvc.FormCollection collection)
        {
            string USRID = collection.AllKeys[0].ToString();
            return Redirect("/Account/SelectUserInfoChange");
        }
        [HttpPost]
        public ActionResult SelectUserInfoChange(string txtUSRNM, string txtFULNM, string txtPWD, string txtEMAIL , HttpPostedFileBase file, System.Web.Mvc.FormCollection collection)
        {
            DataSet dsUser = new DataSet();
            string USRID = collection.AllKeys[4].ToString();
            string filefo = "";
            using (DataVw dMan = new DataVw())
            {
                dsUser = dMan.ExecuteView_S("USR", "*", USRID, "", "ID = ");
            }
            DataSet dsİmage = new DataSet();
            using (DataVw dMan = new DataVw())
            {
                dsİmage = dMan.ExecuteView_S("USR", "AVATAR", USRID, "", "ID=");
            }

            if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/images/tender"), pic);
                    string pathd = "~/images/tender/" + pic;
                    // file is uploaded
                    file.SaveAs(path);
                    filefo = pathd;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }
                }
            else
            {
                filefo = dsİmage.Tables[0].Rows[0][0].ToString();
            }

            DataRow newrow = dsUser.Tables[0].Rows[0];
                newrow["ID"] = USRID;
                newrow["USRNM"] = txtUSRNM;
                newrow["FULNM"] = txtFULNM;
                newrow["EMAIL"] = txtEMAIL;
                newrow["PWD"] = CryptionHelper.Encrypt(txtPWD, "tb");
                newrow["AVATAR"] = filefo;
                Session["avatarimg"] = filefo;
                AgentGc data = new AgentGc();
                string veri = data.DataModified("USR", newrow, dsUser.Tables[0]);
               
                //ViewBag.addmessageinfo = veri;
                //return Redirect("/Account/Manage");
            
            return Redirect("/Account/Manage");
        }
        #endregion
        #region Veri Çekme Tender
        public ActionResult SelectTenderInfo(System.Web.Mvc.FormCollection collection)
        {
            //string USRID = collection.AllKeys[0].ToString();
            return Redirect("/Account/SelectTender");
        }
        public ActionResult SelectTender(string txtTENDERNAME, string txtTENDERNOTE, HttpPostedFileBase file, System.Web.Mvc.FormCollection collection)
        {
            using (DataVw dMan = new DataVw())
            {
                dsUser = dMan.ExecuteView_S("TENDER", "*", "", "", "");
            }
            //string USRID = collection.AllKeys[3].ToString();
            string filefo = "";
            if (txtTENDERNAME.ToString() == "" || txtTENDERNOTE.ToString() == "" )
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Eksik veri girişi! Tüm Alanları Doldurunuz.');</script>");  ////Alert Mesajı Göndermek için.
                //ViewBag.addmessage = "Eksik veri girişi! Tüm Alanları Doldurunuz.";
                //return Redirect("/Account/Manage");
            }
            else
            {
                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/images/tender"), pic);
                    string pathd = "~/images/tender/" + pic;
                    // file is uploaded
                    file.SaveAs(path);
                    filefo = pathd;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }

                }


                //DataRow newrow = dsUser.Tables[0].Rows[0];
                //newrow["ID"] = Guid.NewGuid();
                //newrow["TENDERNAME"] = txtTENDERNAME;
                //newrow["TENDERNOTE"] = txtTENDERNOTE;
                //newrow["TENDERIMAGE"] = filefo;
                //newrow["TANDERUSRID"] = Session["USRIDv"].ToString();
                DataTable table = new DataTable();
                table.Columns.Add("ID", typeof(Guid));
                table.Columns.Add("TENDERNAME", typeof(string));
                table.Columns.Add("TENDERNOTE", typeof(string));
                table.Columns.Add("TENDERIMAGE", typeof(string));
                table.Columns.Add("TANDERUSRID", typeof(Guid));
                table.Rows.Add(Guid.NewGuid(), txtTENDERNAME, txtTENDERNOTE, filefo, Session["USRIDv"].ToString());

                AgentGc data = new AgentGc();
                string veri = data.DataAdded("TENDER", table.Rows[0], dsUser.Tables[0]);
                //return Content("<script language='javascript' type='text/javascript'>alert('" + veri + "');</script>");
                //ViewBag.addmessageinfo = veri;
                return Redirect("/Home/Seller");
            }
            return Redirect("/Home/Seller");
        }
        #endregion
        #region Kullanıcı LogIn Kontrolü
        [HttpPost]
        public ActionResult Control(string txtUsername, string txtPassword)
        {
            HomeController HomeCont = new HomeController();
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
                    Session["USRSTATUS"] = row["IS_ADMIN"].ToString();
                    Session["USRSTATUSADM"] = row["IS_SYSADM"].ToString();
                    Session["USRIDv"] = row["ID"].ToString();
                    Session["name"] = row["FULNM"].ToString();
                    //Session["admin"] = true;
                    //Session["loginError"] = true;
                    Session["IsAuthenticated"] = true;
                    Session["ADMIN"] = row["IS_SYSADM"].ToString();

                    if (row["IS_SYSADM"].ToString() == "True")
                    {
                        Session["IS_SYSADM"] = true;
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
                    else if(row["IS_ADMIN"].ToString() == "True")
                    {
                        Session["IsAuthenticated"] = true;
                        Session["loginRoles"] = false;
                        Session["CUST"] = true;
                        Session["IS_ADMIN"] = true;
                        if (row["AVATAR"].ToString() == "")
                        {
                            Session["avatarimg"] = "~/images/profil/nullavatar.jpg";
                        }
                        else
                        {
                            Session["avatarimg"] = row["AVATAR"].ToString();
                        }
                       
                        return Redirect("/Home/Cust");
                    }
                    else
	                {

                        Session["SELLER"] = true;
                        Session["IsAuthenticated"] = true;
                        Session["loginRoles"] = false;
                        //Session["admin"] = false;
                        Session["IS_HR"] = true;
                        if (row["AVATAR"].ToString() == "")
                        {
                            Session["avatarimg"] = "~/images/profil/nullavatar.jpg";
                        }
                        else
                        {
                            Session["avatarimg"] = row["AVATAR"].ToString();
                        }

                        
                    }
                   
                    return Redirect("/Home/Seller");
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
        #endregion
        #region Yeni Kullanıcı Ekleme Register
        [HttpPost]
        public ActionResult UserAdd(string txtUSRNM, string txtFULNM, string txtPWD, string txtEMAIL, HttpPostedFileBase file, FormCollection collection)
        {
            string usrType = collection["USRTYP"];

            string filefo = "";
            using (DataVw dMan = new DataVw())
            {
                dsUser = dMan.ExecuteView_S("USR", "*", "", "", "");
            }

            string filtrstr = string.Format("USRNM='{0}'", txtUSRNM);
            DataRow[] dr = dsUser.Tables[0].Select(filtrstr);

            if (txtUSRNM.ToString() == "" || txtFULNM.ToString() == "" || txtPWD.ToString() == "" || txtEMAIL.ToString() == "")
            {
                Session["useraddsuccess"] = false;
                ViewBag.addmessage = "Eksik veri girişi! Tüm Alanları Doldurunuz.";
                return Redirect("/Account/Register");
            }
            else
            {
                if (dr.Length == 0)
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
                    newrow["IS_SYSADM"] = 0;
                    if (usrType == "0")
                    {
                        newrow["IS_ADMIN"] = 1;
                        newrow["IS_HR"] = 0;
                    }
                    else
                    {
                        newrow["IS_ADMIN"] = 0;
                        newrow["IS_HR"] = 1;
                    }
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
            return Redirect("/Account/Register");

        }
        #endregion
        #region Çıkış LogOn İşlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Session.Abandon();
            //AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }
        #endregion
        public ActionResult ConfirmationADMIN(string BtnOde, System.Web.Mvc.FormCollection collection)
        {
            DataSet dsTenderAD = new DataSet();
            using (DataVw dMan = new DataVw())
            {
                dsTenderAD = dMan.ExecuteView_S("TENDERD", "*", BtnOde, "", "ID = ");
            }
            DataRow newrow = dsTenderAD.Tables[0].Rows[0];
            newrow["ID"] = (Guid)dsTenderAD.Tables[0].Rows[0]["ID"];
            newrow["TENDERID"] = dsTenderAD.Tables[0].Rows[0]["TENDERID"];
            newrow["TENDERUSERID"] = dsTenderAD.Tables[0].Rows[0]["TENDERUSERID"];
            newrow["TENDERDUSRID"] = dsTenderAD.Tables[0].Rows[0]["TENDERDUSRID"];
            newrow["COUNTRYID"] = dsTenderAD.Tables[0].Rows[0]["COUNTRYID"];
            newrow["TOWNID"] = dsTenderAD.Tables[0].Rows[0]["TOWNID"];
            newrow["TANDERDATE"] = dsTenderAD.Tables[0].Rows[0]["TANDERDATE"];
            newrow["TEL"] = dsTenderAD.Tables[0].Rows[0]["TEL"];
            newrow["EMAİL"] = dsTenderAD.Tables[0].Rows[0]["EMAİL"];
            newrow["NOTE"] = dsTenderAD.Tables[0].Rows[0]["NOTE"];
            newrow["STATUS"] = dsTenderAD.Tables[0].Rows[0]["STATUS"];
            newrow["PAYSTATUS"] = "True";
            AgentGc data = new AgentGc();
            string veri = data.DataModified("TENDERD", newrow, dsTenderAD.Tables[0]);
            return Redirect("/Home/Payment");
            
        }
        public ActionResult TenderUpdate(string btnUpdate, string txtTENDERNAME, string txtTENDERNOTE, HttpPostedFileBase file, System.Web.Mvc.FormCollection collection)
        {
            DataSet dsTenderUp = new DataSet();
            using (DataVw dMan = new DataVw())
            {
                dsTenderUp = dMan.ExecuteView_S("TENDER", "*", "", "", "");
            }
            DataSet dsİmage = new DataSet();
            using (DataVw dMan = new DataVw())
            {
                dsİmage = dMan.ExecuteView_S("TENDER", "TENDERIMAGE", btnUpdate, "", "ID=");
            }

            string filefo = ""; 
            if (file!=null)
            {
                string pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/images/tender"), pic);
                string pathd = "~/images/tender/" + pic;
                // file is uploaded
                file.SaveAs(path);
                filefo = pathd;
                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                }
            }
            else
            {
                filefo = dsİmage.Tables[0].Rows[0][0].ToString();
            }
            
            DataRow newrow = dsTenderUp.Tables[0].Rows[0];
            newrow["ID"] =btnUpdate;
            newrow["TENDERNAME"] =txtTENDERNAME;
            newrow["TENDERNOTE"] =txtTENDERNOTE;
            newrow["TENDERIMAGE"] = filefo;
            newrow["TENDERUSRID"] = Session["USRIDv"].ToString();
            AgentGc data = new AgentGc();
            string veri = data.DataModified("TENDER", newrow, dsTenderUp.Tables[0]);
            return Redirect("/Home/Seller");  
        }
        public ActionResult Confirmation(string BtnKbl, string BtnRed, System.Web.Mvc.FormCollection collection)
        {
            DataSet dsTenderD = new DataSet();
            string kbl = collection["BtnKbl"];
            string red = collection["BtnRed"];
            if (kbl != null)
            {
                using (DataVw dMan = new DataVw())
                {
                    dsTenderD = dMan.ExecuteView_S("TENDERD", "*", kbl, "", "ID = ");
                }
                DataRow newrow = dsTenderD.Tables[0].Rows[0];
                newrow["ID"] = (Guid)dsTenderD.Tables[0].Rows[0]["ID"];
                newrow["TENDERID"] = dsTenderD.Tables[0].Rows[0]["TENDERID"];
                newrow["TENDERUSERID"] = dsTenderD.Tables[0].Rows[0]["TENDERUSERID"];
                newrow["TENDERDUSRID"] = dsTenderD.Tables[0].Rows[0]["TENDERDUSRID"];
                newrow["COUNTRYID"] = dsTenderD.Tables[0].Rows[0]["COUNTRYID"];
                newrow["TOWNID"] = dsTenderD.Tables[0].Rows[0]["TOWNID"];
                newrow["TANDERDATE"] = dsTenderD.Tables[0].Rows[0]["TANDERDATE"];
                newrow["TEL"] = dsTenderD.Tables[0].Rows[0]["TEL"];
                newrow["EMAİL"] = dsTenderD.Tables[0].Rows[0]["EMAİL"];
                newrow["NOTE"] = dsTenderD.Tables[0].Rows[0]["NOTE"];
                newrow["STATUS"] = "True";
                newrow["PAYSTATUS"] = dsTenderD.Tables[0].Rows[0]["PAYSTATUS"];
                AgentGc data = new AgentGc();
                string veri = data.DataModified("TENDERD", newrow, dsTenderD.Tables[0]);
                return Redirect("/Home/Seller");
            }
            if (red != null)
            {
                using (DataVw dMan = new DataVw())
                {
                    dsTenderD = dMan.ExecuteView_S("TENDERD", "*", red, "", "ID = ");
                }
                DataRow newrow = dsTenderD.Tables[0].Rows[0];
                newrow["ID"] = dsTenderD.Tables[0].Rows[0]["ID"];
                newrow["TENDERID"] = dsTenderD.Tables[0].Rows[0]["TENDERID"];
                newrow["TENDERUSERID"] = dsTenderD.Tables[0].Rows[0]["TENDERUSERID"];
                newrow["TENDERDUSRID"] = dsTenderD.Tables[0].Rows[0]["TENDERDUSRID"];
                newrow["COUNTRYID"] = dsTenderD.Tables[0].Rows[0]["COUNTRYID"];
                newrow["TOWNID"] = dsTenderD.Tables[0].Rows[0]["TOWNID"];
                newrow["TANDERDATE"] = dsTenderD.Tables[0].Rows[0]["TANDERDATE"];
                newrow["TEL"] = dsTenderD.Tables[0].Rows[0]["TEL"];
                newrow["EMAİL"] = dsTenderD.Tables[0].Rows[0]["EMAİL"];
                newrow["NOTE"] = dsTenderD.Tables[0].Rows[0]["NOTE"];
                newrow["STATUS"] = "False";
                newrow["PAYSTATUS"] = dsTenderD.Tables[0].Rows[0]["PAYSTATUS"];
                AgentGc data = new AgentGc();
                string veri = data.DataModified("TENDERD", newrow, dsTenderD.Tables[0]);
                return Redirect("/Home/Seller");
            }
            return View();
        }
    }
}