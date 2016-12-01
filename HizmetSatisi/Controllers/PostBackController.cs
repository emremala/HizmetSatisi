using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using OfficeAgent.Cryption;
using OfficeAgent.Data;


namespace HizmetSatisi.Controllers
{
    public class PostBackController : Controller
    {
        // GET: PostBack
        public ActionResult Index()
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
            return View();
        }
    }
}