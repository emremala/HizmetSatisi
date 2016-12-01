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
            string COUNID = collection["USERCNTR"];
            string TOWNID = collection["USERTOWN"];

            using (DataVw dMan = new DataVw())
            {
                dsTenderD = dMan.ExecuteView_S("TENDERD", "*", "", "", "");
            }

            DataRow newrow = dsTenderD.Tables[0].NewRow();
            newrow["ID"] = Guid.NewGuid();
            newrow["TENDERID"] = Session["TENDERID"].ToString();
            newrow["TENDERDUSRID"] = Session["USRIDv"].ToString();
            newrow["COUNTRYID"] = COUNID;
            newrow["TOWNID"] = TOWNID;
            newrow["TANDERDATE"] = txtTANDERDATE;
            newrow["TEL"] = txtTEL;
            newrow["EMAİL"] = txtEMAİL;
            newrow["NOTE"] = txtNOTE;
            AgentGc data = new AgentGc();
            string veri = data.DataAdded("TENDERD", newrow, dsTenderD.Tables[0]);

            return Redirect("/Home/Index");
        }
        
        //public ActionResult GETTT(int ulkeId)
        //{

        //    DataSet dsUserCountry = new DataSet();
        //    using (DataVw dMan = new DataVw())
        //    {
        //        dsUserCountry = dMan.ExecuteView_S("TOWN", "TOWNNM,TOWNID", "", "", "COUNTRYID="+ ulkeId + "");
        //    }
        //    List<SelectListItem> dsUserCNTR = new List<SelectListItem>();
        //    foreach (DataRow dr in dsUserCountry.Tables[0].Rows)
        //    {
        //        dsUserCNTR.Add(new SelectListItem { Text = dr["TOWNNM"].ToString(), Value = dr["TOWNID"].ToString() });
        //    }
        //    ViewBag.dsUserCNTR = dsUserCNTR;
        //    return Json(dsUserCNTR);
        //}


        //public ActionResult GETT(string il_id)
        //{
        //    DataSet dsUserCountry = new DataSet();
        //    using (DataVw dMan = new DataVw())
        //    {
        //        dsUserCountry = dMan.ExecuteView_S("TOWN", "TOWNNM,TOWNID", "", "", "COUNTRYID=" + il_id + "");
        //    }
        //    List<SelectListItem> dsUserCNTR = new List<SelectListItem>();
        //    foreach (DataRow dr in dsUserCountry.Tables[0].Rows)
        //    {
        //        dsUserCNTR.Add(new SelectListItem { Text = dr["TOWNNM"].ToString(), Value = dr["TOWNID"].ToString() });
        //    }

        //    return Json(new SelectList(dsUserCNTR, "Value", "Text"));
        //}



        public ActionResult TenderGift(System.Web.Mvc.FormCollection collection)
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

            if (Convert.ToBoolean(Session["IsAuthenticated"]))
            {
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

       
            return View();
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