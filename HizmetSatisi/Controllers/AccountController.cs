﻿using CinemaWeb;
using Microsoft.Owin.Security;
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
            public string EMAIL { get; set; }
            public string CARDNO { get; set; }
            public string CVC { get; set; }
            public string STATUS { get; set; }
            public string STKDAY { get; set; }
            public string STKMONTH { get; set; }
        }

        // GET: Account
        

        public ActionResult Login()
        {
            return View();
        }
        

        public ActionResult Register()
        {
            List<SelectListItem> usrtyp = new List<SelectListItem>();

            usrtyp.Add(new SelectListItem { Text = "Müşteri", Value = "0" });
            usrtyp.Add(new SelectListItem { Text = "Satıcı", Value = "1" });

            ViewBag.USRTYP = usrtyp;

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
                    CARDNO = dr["CARDNO"].ToString(),
                    STATUS = Status,
                    CVC = dr["CVC"].ToString(),
                    STKDAY = dr["STKDAY"].ToString(),
                    STKMONTH = dr["STKMONTH"].ToString()
                });
            }

            ViewBag.UserList = userList;

            return View();
        }

        public ActionResult SelectUserInfo(System.Web.Mvc.FormCollection collection)
        {
            string USRID = collection.AllKeys[0].ToString();

            return Redirect("/Account/SelectUserInfoChange");
        }

        [HttpPost]
        public ActionResult SelectUserInfoChange(string txtUSRNM, string txtFULNM, string txtPWD, string txtEMAIL, string txtCARDNO, string txtCVC, string txtSTKDAY, string txtSTKMONTH, HttpPostedFileBase file, System.Web.Mvc.FormCollection collection)
        {
            DataSet dsUser = new DataSet();
            string USRID = collection.AllKeys[8].ToString();
            string filefo = "";
            using (DataVw dMan = new DataVw())
            {
                dsUser = dMan.ExecuteView_S("USR", "*", USRID, "", "ID = ");
            }

            if (txtUSRNM.ToString() == "" || txtFULNM.ToString() == "" || txtPWD.ToString() == "" || txtEMAIL.ToString() == "" || txtCARDNO.ToString() == "" || txtCVC.ToString() == "" || txtSTKDAY.ToString() == "" || txtSTKMONTH.ToString() == "")
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
                    string path = System.IO.Path.Combine(Server.MapPath("~/images/avatar"), pic);
                    string pathd = "~/images/avatar/" + pic;
                    // file is uploaded
                    file.SaveAs(path);
                    filefo = pathd;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }

                }

                DataRow newrow = dsUser.Tables[0].Rows[0];
                newrow["ID"] = USRID;
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
                    newrow["AVATAR"] = "~/images/avatar/nullavatar.jpg";
                }
                else
                {
                    newrow["AVATAR"] = filefo;
                }
                newrow["CARDNO"] = txtCARDNO;
                newrow["CVC"] = txtCVC;
                newrow["STKDAY"] = txtSTKDAY;
                newrow["STKMONTH"] = txtSTKMONTH;
                //newrow["EDATE"] = DateTime.Now;
                //newrow["EUSRID"] = null;
                newrow["UDATE"] = DateTime.Now;
                //newrow["UUSRID"] = null;
                newrow["NOTE"] = "En Son Güncelleme İşlemi Gerçekleştirdi.";
                AgentGc data = new AgentGc();
                string veri = data.DataModified("USR", newrow, dsUser.Tables[0]);
                return Content("<script language='javascript' type='text/javascript'>alert('" + veri + "');</script>");
                //ViewBag.addmessageinfo = veri;
                //return Redirect("/Account/Manage");
            }
            return Redirect("/Account/Manage");
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
                    else if(row["IS_ADMIN"].ToString() == "True")
                    {
                        Session["IsAuthenticated"] = true;
                        Session["loginRoles"] = false;
                        Session["CUST"] = true;
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

        //private IAuthenticationManager AuthenticationManager
        //{
        //    get
        //    {
        //        return HttpContext.GetOwinContext().Authentication;
        //    }
        //}
        

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Session.Abandon();
            //AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

    }
}