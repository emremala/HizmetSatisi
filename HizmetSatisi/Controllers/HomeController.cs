using OfficeAgent.Cryption;
using OfficeAgent.Data;
using OfficeAgent.Object;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static HizmetSatisi.Controllers.AccountController;

namespace HizmetSatisi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
           
            #region Anasayfa Tüm Teklifler
            DataSet dsTenders = new DataSet();
            using (DataVw dMan = new DataVw())
            {
                dsTenders = dMan.ExecuteView_S("TENDER", "*", "", "", "");
            }
            List<TenderList> TendersInfo = new List<TenderList>();
            foreach (DataRow dr in dsTenders.Tables[0].Rows)
            {
                TendersInfo.Add(new TenderList
                {
                    ID = (Guid)dr["ID"],
                    TENDERNAME = dr["TENDERNAME"].ToString(),
                    TENDERNOTE = dr["TENDERNOTE"].ToString(),
                    TENDERIMAGE = dr["TENDERIMAGE"].ToString(),
                    TENDERUSRID = (Guid)dr["TENDERUSRID"]

                });
            }


            ViewBag.Tenders = TendersInfo;
            #endregion
            return View();
        }
        public ActionResult Json(int? id)//Json ile il id'sini gönderdik.
        {
            DataSet dsUserCountry = new DataSet();
            using (DataVw dMan = new DataVw())
            {
                dsUserCountry = dMan.ExecuteView_S("TOWN", "TOWNNM,TOWNID", id.ToString(), "", "COUNTRYID=");
            }
            List<SelectListItem> dsUserCNTR = new List<SelectListItem>();
            foreach (DataRow dr in dsUserCountry.Tables[0].Rows)
            {
                dsUserCNTR.Add(new SelectListItem { Text = dr["TOWNNM"].ToString(), Value = dr["TOWNID"].ToString() });
            }
            return Json(dsUserCNTR, JsonRequestBehavior.AllowGet);//id null değilse veritabanından çektiğimiz kayıtları döndürdük.
        }

        public ActionResult TenderSave(string txtTANDERDATE, string txtTEL, string txtEMAİL, string txtNOTE, System.Web.Mvc.FormCollection collection)
        {
            DataSet dsTenderD = new DataSet();
            DataSet dsTenderUser = new DataSet();
            string COUNID = collection["USERCNTR"];
            string TOWNID = collection["USERTOWN"];
            string TenderUserId = Session["TENDERID"].ToString();
            using (DataVw dMan = new DataVw())
            {
                dsTenderD = dMan.ExecuteView_S("TENDERD", "*", "", "", "");
            }
            using (DataVw dMan = new DataVw())
            {
                dsTenderUser = dMan.ExecuteView_S("TENDER", "TENDERUSRID", TenderUserId,"", "ID=");

            }
            TenderUserId = dsTenderUser.Tables[0].Rows[0][0].ToString();
            DataRow newrow = dsTenderD.Tables[0].NewRow();
            newrow["ID"] = Guid.NewGuid();
            newrow["TENDERID"] = Session["TENDERID"].ToString();
            newrow["TENDERUSERID"] = TenderUserId;
            newrow["TENDERDUSRID"] = Session["USRIDv"].ToString();
            newrow["COUNTRYID"] = COUNID;
            newrow["TOWNID"] = TOWNID;
            newrow["TANDERDATE"] = txtTANDERDATE;
            newrow["TEL"] = txtTEL;
            newrow["EMAİL"] = txtEMAİL;
            newrow["NOTE"] = txtNOTE;
            newrow["STATUS"] = "False";
            newrow["PAYSTATUS"] = "False";
            AgentGc data = new AgentGc();
            string veri = data.DataAdded("TENDERD", newrow, dsTenderD.Tables[0]);

            return Redirect("/Home/Index");
        }



       

        public ActionResult TenderGift(System.Web.Mvc.FormCollection collection)
        {

            if (Session["USRSTATUS"] != null)
            {
                if (Session["USRSTATUS"].ToString() == "True" && Session["USRSTATUS"].ToString() != null)
                {
                    if (Convert.ToBoolean(Session["IsAuthenticated"]))
                    {
                        DataSet dsUserCountry = new DataSet();
                        using (DataVw dMan = new DataVw())
                        {
                            dsUserCountry = dMan.ExecuteView_S("COUNTRY", "COUNTRYID,COUNTRYNM", "", "", "");
                        }
                        List<SelectListItem> dsUserCNTR = new List<SelectListItem>();
                        foreach (DataRow dr in dsUserCountry.Tables[0].Rows)
                        {
                            dsUserCNTR.Add(new SelectListItem { Text = dr["COUNTRYNM"].ToString(), Value = dr["COUNTRYID"].ToString() });
                        }
                        ViewBag.USERCNTR = dsUserCNTR;
                        string USRID = collection["btnTENDERID"];
                        Session["TENDERID"] = USRID;
                        DataSet dsUser = new DataSet();

                        using (DataVw dMan = new DataVw())
                        {
                            dsUser = dMan.ExecuteView_S("USR", "*", Session["USRIDv"].ToString(), "", "ID =");
                        }

                        List<UserList> UserTenderInfo = new List<UserList>();
                        foreach (DataRow dr in dsUser.Tables[0].Rows)
                        {
                            string Status = "";
                            if (dr[7].ToString() == "True") { Status = "Admin Hesabı"; }
                            if (dr[6].ToString() == "True") { Status = "Müşteri Hesabı"; }
                            if (dr[8].ToString() == "True") { Status = "Satıcı Hesabı"; }
                            UserTenderInfo.Add(new UserList
                            {
                                AVATAR = dr["AVATAR"].ToString(),
                                ID = (Guid)dr["ID"],
                                USRNM = dr["USRNM"].ToString(),
                                STATUS = Status,
                                EMAIL = dr["EMAIL"].ToString(),
                                FULNM = dr["FULNM"].ToString(),

                            });
                        }


                        ViewBag.UserTenderInfo = UserTenderInfo;
                        return View();
                    }
                    else
                    {

                        return Redirect("/Account/Login");
                    }
                }
                else
                {
                    return Redirect("/Home/Index");

                } 
            }
            else
            {
                return Redirect("/Home/Index");
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Cust()
        {
         
            DataSet dsUser = new DataSet();
            
            using (DataVw dMan = new DataVw())
            {
                dsUser = dMan.ExecuteView_S("USR", "*", Session["USRIDv"].ToString(), "", "ID =");
            }

            List<UserList> CustInfo = new List<UserList>();
            foreach (DataRow dr in dsUser.Tables[0].Rows)
            {
                string Status = "";
                if (dr[7].ToString() == "True") { Status = "Admin Hesabı"; }
                if (dr[6].ToString() == "True") { Status = "Müşteri Hesabı"; }
                if (dr[8].ToString() == "True") { Status = "Satıcı Hesabı"; }
                CustInfo.Add(new UserList
                {
                    ID = (Guid)dr["ID"],
                    USRNM = dr["USRNM"].ToString(),
                    PWD = CryptionHelper.Decrypt(dr["PWD"].ToString(), "tb"),
                    EMAIL = dr["EMAIL"].ToString(),
                    FULNM = dr["FULNM"].ToString(),
                    
                });
            }


            ViewBag.CustInfo = CustInfo;
            DataSet dsTenderDs = new DataSet();
            using (DataVw dMan = new DataVw())
            {
                dsTenderDs = dMan.ExecuteView_S("TENDERADMIN", "*", Session["USRIDv"].ToString(), "", "TENDERDUSRID =");
            }

            List<TenderDList> dsTenderD = new List<TenderDList>();
            foreach (DataRow dr in dsTenderDs.Tables[0].Rows)
            {
                string STATUS;
                if (dr["STATUS"].ToString() == "True") { STATUS = "Ödeme Sayfasına Gidiniz.."; } else { STATUS = "Onay Bekleniyor.."; }
                dsTenderD.Add(new TenderDList
                {
                    ID = (Guid)dr["ID"],
                    TENDERUSERID = (Guid)dr["TENDERUSERID"],
                    TENDERNAME = dr["TENDERNAME"].ToString(),
                    FULNM = dr["FULNM"].ToString(),
                    COUNTRYNM = dr["COUNTRYNM"].ToString(),
                    TOWNNM = dr["TOWNNM"].ToString(),
                    TANDERDATE = dr["TANDERDATE"].ToString(),
                    TEL = dr["TEL"].ToString(),
                    EMAIL = dr["EMAİL"].ToString(),
                    NOTE = dr["NOTE"].ToString(),
                    STATUS = STATUS,

                });
            }



            ViewBag.TenderAD = dsTenderD;
            DataSet dsTenderDsApp = new DataSet();
            using (DataVw dMan = new DataVw())
            {
                dsTenderDsApp = dMan.ExecuteView_S("TENDERADMINAPP", "*", Session["USRIDv"].ToString(), "", "TENDERDUSRID =");
            }
            List<TenderDList> dsTenderDAp = new List<TenderDList>();
            foreach (DataRow dr in dsTenderDsApp.Tables[0].Rows)
            {
                dsTenderDAp.Add(new TenderDList
                {
                    ID = (Guid)dr["ID"],
                    TENDERUSERID = (Guid)dr["TENDERUSERID"],
                    TENDERNAME = dr["TENDERNAME"].ToString(),
                    FULNM = dr["FULNM"].ToString(),
                    COUNTRYNM = dr["COUNTRYNM"].ToString(),
                    TOWNNM = dr["TOWNNM"].ToString(),
                    TANDERDATE = dr["TANDERDATE"].ToString(),
                    TEL = dr["TEL"].ToString(),
                    EMAIL = dr["EMAİL"].ToString(),
                    NOTE = dr["NOTE"].ToString(),
                });
            }



            ViewBag.TenderADApp = dsTenderDAp;
            DataSet dsTenderDsConf = new DataSet();
            using (DataVw dMan = new DataVw())
            {
                dsTenderDsConf = dMan.ExecuteView_S("TENDERADMINCONF", "*", Session["USRIDv"].ToString(), "", "TENDERDUSRID =");
            }
            List<TenderDList> dsTenderConf = new List<TenderDList>();
            foreach (DataRow dr in dsTenderDsConf.Tables[0].Rows)
            {
                dsTenderConf.Add(new TenderDList
                {
                    ID = (Guid)dr["ID"],
                    TENDERUSERID = (Guid)dr["TENDERUSERID"],
                    TENDERNAME = dr["TENDERNAME"].ToString(),
                    FULNM = dr["FULNM"].ToString(),
                    COUNTRYNM = dr["COUNTRYNM"].ToString(),
                    TOWNNM = dr["TOWNNM"].ToString(),
                    TANDERDATE = dr["TANDERDATE"].ToString(),
                    TEL = dr["TEL"].ToString(),
                    EMAIL = dr["EMAİL"].ToString(),
                    NOTE = dr["NOTE"].ToString(),
                });
            }



            ViewBag.TenderADConf = dsTenderConf;
            return View();
        }


        public ActionResult Payment(string txtFULNM,string txtMM,string txtYY,string txtKN,string txtCVC)
         {
            if (txtFULNM==null&&txtMM== null && txtYY== null && txtKN== null && txtCVC== null)
            {
                return View();
            }
            else
            {
                return Redirect("/Home/Cust");
                
            }

            
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Admin()
        {
            return View();
        }

        public ActionResult Seller()
        {
            DataSet dsTenderDs = new DataSet();
            using (DataVw dMan = new DataVw())
            {
                dsTenderDs = dMan.ExecuteView_S("TENDERISHR", "*", Session["USRIDv"].ToString(), "", "TENDERUSERID=");
            }
            List<TenderDList> dsTenderD = new List<TenderDList>();
            foreach (DataRow dr in dsTenderDs.Tables[0].Rows)
            {
                dsTenderD.Add(new TenderDList
                {
                    ID = (Guid)dr["ID"],
                    TENDERUSERID = (Guid)dr["TENDERUSERID"],
                    TENDERNAME = dr["TENDERNAME"].ToString(),
                    FULNM = dr["FULNM"].ToString(),
                    COUNTRYNM = dr["COUNTRYNM"].ToString(),
                    TOWNNM = dr["TOWNNM"].ToString(),
                    TANDERDATE = dr["TANDERDATE"].ToString(),
                    TEL = dr["TEL"].ToString(),
                    EMAIL = dr["EMAİL"].ToString(),
                    NOTE = dr["NOTE"].ToString(),


                });
            }



            ViewBag.TenderD = dsTenderD;
            DataSet dsTenderApds = new DataSet();
            using (DataVw dMan = new DataVw())
            {
                dsTenderApds = dMan.ExecuteView_S("TENDERISHRAPP", "*", Session["USRIDv"].ToString(), "", "TENDERUSERID=");
            }
            List<TenderDList> dsTenderAppD = new List<TenderDList>();
            foreach (DataRow dr in dsTenderApds.Tables[0].Rows)
            {
                string PAYSTATUS;
                if (dr["PAYSTATUS"].ToString() == "True") {PAYSTATUS = "Ödendi"; } else { PAYSTATUS = "Ödeme Bekleniyor.."; }

                dsTenderAppD.Add(new TenderDList
                {
                    ID = (Guid)dr["ID"],
                    TENDERUSERID = (Guid)dr["TENDERUSERID"],
                    TENDERNAME = dr["TENDERNAME"].ToString(),
                    FULNM = dr["FULNM"].ToString(),
                    COUNTRYNM = dr["COUNTRYNM"].ToString(),
                    TOWNNM = dr["TOWNNM"].ToString(),
                    TANDERDATE = dr["TANDERDATE"].ToString(),
                    TEL = dr["TEL"].ToString(),
                    EMAIL = dr["EMAİL"].ToString(),
                    NOTE = dr["NOTE"].ToString(),
                    STATUS = PAYSTATUS,
                });
            }



            ViewBag.TenderAppD = dsTenderAppD;
            DataSet dsUser = new DataSet();

            using (DataVw dMan = new DataVw())
            {
                dsUser = dMan.ExecuteView_S("USR", "*", Session["USRIDv"].ToString(), "", "ID =");
            }

            List<UserList> Seller = new List<UserList>();
            foreach (DataRow dr in dsUser.Tables[0].Rows)
            {
                string Status = "";
                if (dr[7].ToString() == "True") { Status = "Admin Hesabı"; }
                if (dr[6].ToString() == "True") { Status = "Müşteri Hesabı"; }
                if (dr[8].ToString() == "True") { Status = "Satıcı Hesabı"; }
                Seller.Add(new UserList
                {
                    ID = (Guid)dr["ID"],
                    USRNM = dr["USRNM"].ToString(),
                    PWD = CryptionHelper.Decrypt(dr["PWD"].ToString(), "tb"),
                    EMAIL = dr["EMAIL"].ToString(),
                    FULNM = dr["FULNM"].ToString(),
                   
                });
            }


            ViewBag.SellerInfo = Seller;
            #region Kişiye Özel Teklifleri
            DataSet dsTender = new DataSet();
            string usr = Session["USRIDv"].ToString();
            using (DataVw dMan = new DataVw())
            {
                dsTender = dMan.ExecuteView_S("TENDER", "*",usr, "", "TENDERUSRID =");
            }

            List<TenderList> TenderInfo = new List<TenderList>();
            foreach (DataRow dr in dsTender.Tables[0].Rows)
            {

                TenderInfo.Add(new TenderList
                {
                    ID = (Guid)dr["ID"],
                    TENDERNAME = dr["TENDERNAME"].ToString(),
                    TENDERNOTE = dr["TENDERNOTE"].ToString(),
                    TENDERIMAGE = dr["TENDERIMAGE"].ToString(),
                    TENDERUSRID =(Guid)dr["TENDERUSRID"]

                });
            }
            ViewBag.Tender = TenderInfo;
            #endregion

            return View();
        }
    }
}