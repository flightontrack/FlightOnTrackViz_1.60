using System;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using FontNameSpace.Models;
using System.Linq;
using static FontNameSpace.Finals;

namespace FontNameSpace.Controllers
{
    public class PilotUserController : Controller
    {
        private Entities db = new Entities();

        [HttpPost]
        [AllowAnonymous]
        public void CreateUserPilot()
        {
            if (ModelState.IsValid)
            {
                var pilotRecordId = Int32.Parse(Request["param1"]);
                try
                {
                    //var pGuid = db.Pilots.Where(r => r.PilotID == pilotRecordId).FirstOrDefault().PilotGuid;
                    //string a_p = pGuid.Substring(10, 2).ToLower() + "&" + pGuid.Substring(20, 4).ToLower();
                    var user1 = db.Pilots.Where(r => r.PilotID == pilotRecordId).FirstOrDefault().PilotCode;
                    var user2 = db.Pilots.Where(r => r.PilotID == pilotRecordId).FirstOrDefault().SimNumber;
                    var spr = db.get_PilotP(user1, user2).ToList();
                    string a_p = spr[0];
                    //foreach (var item in spr)
                    //{
                    //    s = item;
                    //}

                    var UserName = user1 + "." + user2.Substring(user2.Length-4);
                    if (!WebSecurity.Initialized) { WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true); }
                    if (WebSecurity.UserExists(UserName)) Response.Write(ERROR_USEREXISTS);
                    else
                    {
                        WebSecurity.CreateUserAndAccount(UserName, a_p);
                        Response.Write(UserName);
                    }
                }
                catch (MembershipCreateUserException e)
                {
                    string error = e.Message;
                    LogHelper.onFailureLog("CreateUserPilot()", e);
                    Response.Write(error);
                    throw;
                }
            }
        }

    }
}
