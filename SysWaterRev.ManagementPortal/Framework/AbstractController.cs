using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity.Owin;

namespace SysWaterRev.ManagementPortal.Framework
{
    public abstract class AbstractController : Controller
    {
        private ApplicationUserManager usermanager;
        public ApplicationUserManager UserManager
        {
            get { return usermanager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            set { usermanager = value; }
        }
        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return Mapper.Map(source, destination);
        }

        public virtual ViewResult ViewNotFound(string message)
        {
            ViewBag.Message = message;
            return View("~/Views/Shared/Error.cshtml");
        }
        public async Task<string> SendEmailConfirmationTokenAsync(string userId, string subject)
        {
            var code = await UserManager.GenerateEmailConfirmationTokenAsync(userId);
            var callbackUrl = Url.Action("ConfirmEmail", "Account",
               new {userId, code }, Request.Url.Scheme);
            await UserManager.SendEmailAsync(userId, subject,
               "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
            return callbackUrl;
        }
    }
}