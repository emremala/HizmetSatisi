using OfficeAgent.Cryption;
using OfficeAgent.Data;
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
            return View();
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
                dsUser = dMan.ExecuteView_S("USR", "*","", "", "");
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
                    CARDNO = dr["CARDNO"].ToString(),
                    STATUS = Status,
                    CVC = dr["CVC"].ToString(),
                    STKDAY = dr["STKDAY"].ToString(),
                    STKMONTH = dr["STKMONTH"].ToString()
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
    }
}