using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using OfficeAgent.Data;
using OfficeAgent.Cryption;
using OfficeAgent.Configuration;
using OfficeAgent.Object;

namespace HizmetSatisi.Controllers
{
    public class UserList
    {
        public Guid ID { get; set; }
        public string USRNM { get; set; }
        public string PWD { get; set; }
        public string FULNM { get; set; }
        public string EMAIL { get; set; }
        public string AVATAR { get; set; }
        public string STATUS { get; set; }
        
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
    public class DefaultController : Controller
    {
        // GET: Default
        
        public ActionResult Admin()
        {
            DataSet dsTenderApds = new DataSet();
            using (DataVw dMan = new DataVw())
            {
                dsTenderApds = dMan.ExecuteView_S("TENDERADMIN", "*", "", "", "");
            }
            List<TenderDList> dsTenderAppD = new List<TenderDList>();
            foreach (DataRow dr in dsTenderApds.Tables[0].Rows)
            {
                string PAYSTATUS;
                if (dr["STATUS"].ToString()=="True")
                {
                    if (dr["PAYSTATUS"].ToString() == "True")
                    {
                        PAYSTATUS = "Onaylandı ve Ödendi..";
                    }
                    else
                    {
                        PAYSTATUS = "Onaylandı ve Ödeme Bekleniyor.. ";
                    }
                }
                else
                {
                    PAYSTATUS = "Onay Bekliyor.. ";
                }
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



            ViewBag.TenderRapor = dsTenderAppD;
            DataSet dsTender = new DataSet();
            string usr = Session["USRIDv"].ToString();
            using (DataVw dMan = new DataVw())
            {
                dsTender = dMan.ExecuteView_S("TENDER", "*", "", "", "");
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
                    TENDERUSRID = (Guid)dr["TENDERUSRID"]

                });
            }
            ViewBag.AdminTender = TenderInfo;
            DataSet dsAdminUser = new DataSet();
            using (DataVw dMan = new DataVw())
            {
                dsAdminUser = dMan.ExecuteView_S("USR", "*", "True", "", "IS_AC=");
            }
            List<UserList> AdminUser = new List<UserList>();
            foreach (DataRow dr in dsAdminUser.Tables[0].Rows)
            {
                string Status = "";
                if (dr["IS_SYSADM"].ToString() == "True") { Status = "Admin Hesabı"; }
                if (dr["IS_ADMIN"].ToString() == "True") { Status = "Müşteri Hesabı"; }
                if (dr["IS_HR"].ToString() == "True") { Status = "Satıcı Hesabı"; }
                AdminUser.Add(new UserList
                {
                    ID = (Guid)dr["ID"],
                    USRNM = dr["USRNM"].ToString(),
                    PWD = CryptionHelper.Decrypt(dr["PWD"].ToString(), "tb"),
                    FULNM = dr["FULNM"].ToString(),
                    EMAIL = dr["EMAIL"].ToString(),
                    STATUS = Status,
                    AVATAR = dr["AVATAR"].ToString(),
                });
            }
            ViewBag.AdminUser = AdminUser;

            DataSet dsAdminUserOpen = new DataSet();
            using (DataVw dMan = new DataVw())
            {
                dsAdminUserOpen = dMan.ExecuteView_S("USR", "*", "False", "", "IS_AC=");
            }
            List<UserList> AdminUserOpen = new List<UserList>();
            foreach (DataRow dr in dsAdminUserOpen.Tables[0].Rows)
            {
                string Status = "";
                if (dr["IS_SYSADM"].ToString() == "True") { Status = "Admin Hesabı"; }
                if (dr["IS_ADMIN"].ToString() == "True") { Status = "Müşteri Hesabı"; }
                if (dr["IS_HR"].ToString() == "True") { Status = "Satıcı Hesabı"; }
                AdminUserOpen.Add(new UserList
                {
                    ID = (Guid)dr["ID"],
                    USRNM = dr["USRNM"].ToString(),
                    PWD = CryptionHelper.Decrypt(dr["PWD"].ToString(), "tb"),
                    FULNM = dr["FULNM"].ToString(),
                    EMAIL = dr["EMAIL"].ToString(),
                    STATUS = Status,
                    AVATAR = dr["AVATAR"].ToString(),
                });
            }
            ViewBag.AdminUserOpen = AdminUserOpen;
            return View();
        }
        public ActionResult Bloke(string btnBloke)
        {
            DataSet dsUser = new DataSet();
            string USRID = btnBloke;
            
            using (DataVw dMan = new DataVw())
            {
                dsUser = dMan.ExecuteView_S("USR", "*", USRID, "", "ID = ");
            }
            DataRow newrow = dsUser.Tables[0].Rows[0];
            newrow["ID"] = USRID;
            newrow["USRNM"] = dsUser.Tables[0].Rows[0]["USRNM"].ToString();
            newrow["FULNM"] = dsUser.Tables[0].Rows[0]["FULNM"].ToString();
            newrow["EMAIL"] = dsUser.Tables[0].Rows[0]["EMAIL"].ToString();
            newrow["PWD"] = dsUser.Tables[0].Rows[0]["PWD"].ToString();
            newrow["AVATAR"] = dsUser.Tables[0].Rows[0]["AVATAR"].ToString();
            newrow["IS_ADMIN"] = dsUser.Tables[0].Rows[0]["IS_ADMIN"].ToString();
            newrow["IS_SYSADM"] = dsUser.Tables[0].Rows[0]["IS_SYSADM"].ToString();
            newrow["IS_HR"] = dsUser.Tables[0].Rows[0]["IS_HR"].ToString();
            newrow["IS_AC"] = "False";
            AgentGc data = new AgentGc();
            string veri = data.DataModified("USR", newrow, dsUser.Tables[0]);

            //ViewBag.addmessageinfo = veri;
            //return Redirect("/Account/Manage");

            return Redirect("/Default/Admin");

        }
        public ActionResult OpenBloke(string btnBloke)
        {
            DataSet dsUser = new DataSet();
            string USRID = btnBloke;

            using (DataVw dMan = new DataVw())
            {
                dsUser = dMan.ExecuteView_S("USR", "*", USRID, "", "ID = ");
            }
            DataRow newrow = dsUser.Tables[0].Rows[0];
            newrow["ID"] = USRID;
            newrow["USRNM"] = dsUser.Tables[0].Rows[0]["USRNM"].ToString();
            newrow["FULNM"] = dsUser.Tables[0].Rows[0]["FULNM"].ToString();
            newrow["EMAIL"] = dsUser.Tables[0].Rows[0]["EMAIL"].ToString();
            newrow["PWD"] = dsUser.Tables[0].Rows[0]["PWD"].ToString();
            newrow["AVATAR"] = dsUser.Tables[0].Rows[0]["AVATAR"].ToString();
            newrow["IS_ADMIN"] = dsUser.Tables[0].Rows[0]["IS_ADMIN"].ToString();
            newrow["IS_SYSADM"] = dsUser.Tables[0].Rows[0]["IS_SYSADM"].ToString();
            newrow["IS_HR"] = dsUser.Tables[0].Rows[0]["IS_HR"].ToString();
            newrow["IS_AC"] = "True";
            AgentGc data = new AgentGc();
            string veri = data.DataModified("USR", newrow, dsUser.Tables[0]);

            //ViewBag.addmessageinfo = veri;
            //return Redirect("/Account/Manage");

            return Redirect("/Default/Admin");

        }
    }
}