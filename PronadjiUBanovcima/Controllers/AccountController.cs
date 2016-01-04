using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using PronadjiUBanovcima.Models;
using System.IO;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Web.Security;
using System.Data.Entity;
using ActionMailerNext.Mvc5;

namespace PronadjiUBanovcima.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
       
        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

      

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
                // Determine if user has entered their UserName or Email address.
                // TODO: research if there is a more efficient way to do this.
                using (var db = new ApplicationDbContext())
                {
                    model.UserName = (db.Users.Any(p => p.UserName == model.UserName)) ?
                    model.UserName :
                    (db.Users.Any(p => p.Email == model.UserName)) ?
                    db.Users.SingleOrDefault(p => p.Email == model.UserName).UserName :
                    model.UserName;
                }

                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null && user.IsConfirmed) 
                {
                    await SignInAsync(user, model.RememberMe);
                    if (UserManager.IsInRole(user.Id, "Admin"))

                        return RedirectToAction("Index", "Info");

                    else
                        return RedirectToAction("Manage", "Account");
                       // return RedirectToLocal(returnUrl);
                }
                else
                {

                    ModelState.AddModelError("", "Invalid username or password.");
                }
                return View(model);

        }

       
        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            using (var db = new ApplicationDbContext())
            {
                List<SelectListItem> listSelectListItem = new List<SelectListItem>();

                try
                {
                    foreach (Delatnost del in db.Delatnosti)
                    {
                        SelectListItem selItem = new SelectListItem()
                        {
                            Text = del.Naziv,
                            Value = del.Id.ToString(),
                            Selected = del.IsSelected

                        };
                        listSelectListItem.Add(selItem);
                    }
                }
                catch { }
                RegisterViewModel viewModel = new RegisterViewModel();
                viewModel.ListaDelatnosti = listSelectListItem;
                return View(viewModel);
            }
        }

        private string CreateConfirmationToken()
        {
            return ShortGuid.NewGuid();
        }

        
        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model, IEnumerable<string> selectedDels, HttpPostedFileBase imageUpload)
        {
            if (ModelState.IsValid)
            {
                
                List<int> delId = new List<int>();
                if (selectedDels != null)
                    foreach (var item in selectedDels)
                    {
                        delId.Add(Convert.ToInt32(item));
                    }
                
                using (var db = new ApplicationDbContext())
                {
                    List<Delatnost> dels = new List<Delatnost>();
                    foreach (var item in delId)
                    {
                        dels.Add(db.Delatnosti.Find(item));
                    }

                    var validImageTypes = new string[]
                   {
                       "image/gif",
                       "image/jpeg",
                       "image/png"
                   };
                    // ViewBag.Delatnosti = new SelectList(db.Delatnosti, "Id", "Naziv");
                    
                    if (imageUpload != null && !validImageTypes.Contains(imageUpload.ContentType))
                    {
                        ModelState.AddModelError("ImageUpload", "Izaberite JPG, GIF ili PNG format slike");
                        ViewBag.Error = "Slika nije validna!";
                        return View("Error");
                    }
                   
                        string confToken = CreateConfirmationToken();
                        var user = new ApplicationUser()
                        {
                            UserName = model.UserName,
                            Email = model.Email,
                            ConfirmationToken = confToken,
                            IsConfirmed= false
                        };
                        var idRez = db.Podaci.OrderByDescending(x => x.Id).FirstOrDefault();
                        int id;
                        if (idRez != null)
                            id = idRez.Id;
                        else id = 0;

                        List<Tag> tagovi = new List<Tag>();

                        for (int t = 1; t <= 3; t++)
                        {
                            string tag = Request.Form["tag" + t];
                            if (tag != "") 
                            {
                                
                                if (db.Tagovi.Where(x => x.Naziv.Contains(tag)).ToList().Count != 0)

                                    tagovi.AddRange(db.Tagovi.Where(x => x.Naziv.Contains(tag)).ToList());
                                
                                else
                                    tagovi.Add(new Tag() { Naziv = tag });
                            }

                        }
                       
                        
                        var info = new Info()
                        {
                            Id=id + 1,
                            Adresa = model.Adresa + " " + model.Broj + ", " + model.Mesto,
                            Email = model.Email,
                            Firma = model.Firma,
                            Telefon = model.Telefon,
                            ListaDelatnosti = dels,
                            ListaTagova = tagovi,
                            Klijent = user
                        };
                        var slika = new Slika
                        {
                            Alt = model.Firma,
                            KlijentId = info.Id
                        };
                        if (imageUpload != null && imageUpload.ContentLength > 0)
                        {
                        var uploadDir = "~/Images";
                        var imagePath = Path.Combine(Server.MapPath(uploadDir), imageUpload.FileName);
                        var imageUrl = Path.Combine(uploadDir, imageUpload.FileName);
                        imageUpload.SaveAs(imagePath);
                        slika.Putanja = imageUrl.Remove(0, 1);
                        }
                        else
                            slika.Putanja = @"/Images\default-user-image.png";
                        info.ListaSlika = new List<Slika>();
                        info.ListaSlika.Add(slika);

                        db.Podaci.Add(info);
                        db.Slike.Add(slika);
                        
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (DbEntityValidationException dbEx)
                        {
                            foreach (var validationErrors in dbEx.EntityValidationErrors)
                            {
                                foreach (var validationError in validationErrors.ValidationErrors)
                                {
                                    Trace.TraceInformation("Property: {0} Error: {1}",
                                                            validationError.PropertyName,
                                                            validationError.ErrorMessage);
                                }
                            }
                        }
                        
                        var result = await UserManager.AddPasswordAsync(user.Id, model.Password);
                      
                        if (result.Succeeded)
                        {

                            //await SignInAsync(user, isPersistent: false);
                            UserManager.AddToRole(user.Id, "Osnovni");
                            new MailController().VerificationEmail(user).Deliver();
                            return View("RegisterStepTwo"); 
                        }
                        else
                        {
                            AddErrors(result);
                        }
                    
                }
            }

            // If we got this far, something failed, redisplay form
            using (var db = new ApplicationDbContext())
            {
                List<SelectListItem> listSelectListItem = new List<SelectListItem>();

                try
                {
                    foreach (Delatnost del in db.Delatnosti)
                    {
                        SelectListItem selItem = new SelectListItem()
                        {
                            Text = del.Naziv,
                            Value = del.Id.ToString(),
                            Selected = del.IsSelected

                        };
                        listSelectListItem.Add(selItem);
                    }
                }
                catch { }
                RegisterViewModel viewModel = new RegisterViewModel();
                viewModel.ListaDelatnosti = listSelectListItem;
                return View(viewModel);
            }
        }

        private bool ConfirmAccount(string confToken)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            ApplicationUser user = context.Users.SingleOrDefault(u => u.ConfirmationToken == confToken);
            if (user != null)
            {
                user.IsConfirmed = true;
                DbSet<ApplicationUser> dbSet = context.Set<ApplicationUser>();
                dbSet.Attach(user);
                context.Entry(user).State = EntityState.Modified;
                context.SaveChanges();
                return true;
            }
            return false;
        }
        [AllowAnonymous]
        public ActionResult RegisterConfirmation(string Id)
        {
            if (ConfirmAccount(Id))
                return View("Registered");
            return View("ConfirmationFailure");
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.ChangeDataSuccess ? "Podaci su uspešno izmenjeni."
                :"";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model, HttpPostedFileBase imageUpload)
        {
            
            bool hasPassword = HasPassword();
            if (model.ConfirmPassword != null && model.NewPassword != null && model.OldPassword != null)
            {
                
                ViewBag.HasLocalPassword = hasPassword;
                ViewBag.ReturnUrl = Url.Action("Manage");
                if (hasPassword)
                {
                    if (ModelState.IsValid)
                    {
                        IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                        if (result.Succeeded)
                        {
                            return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                        }
                        else
                        {
                            AddErrors(result);
                        }
                    }
                }
                else
                {
                    // User does not have a password so remove any validation errors caused by a missing OldPassword field
                    ModelState state = ModelState["OldPassword"];
                    if (state != null)
                    {
                        state.Errors.Clear();
                    }

                    if (ModelState.IsValid)
                    {
                        IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                        if (result.Succeeded)
                        {
                            return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                        }
                        else
                        {
                            AddErrors(result);
                        }
                    }
                }
            }
            else
            {
                using (var db = new ApplicationDbContext())
                {
                    bool modified = false;
                    string currentUser = User.Identity.GetUserId();
                    var dbInfo = db.Podaci.Where(p => p.Klijent.Id.Equals(currentUser)).ToList();

                    if (ModelState.IsValid && dbInfo.Count == 1)
                    {
                        Info info = dbInfo[0];

                        var validImageTypes = new string[]
                            {
                                "image/gif",
                                "image/jpeg",
                                "image/png"
                             };

                         if (imageUpload != null && imageUpload.ContentLength > 0)
                        {
                            if (!validImageTypes.Contains(imageUpload.ContentType))
                        {
                            ModelState.AddModelError("ImageUpload", "Izaberite JPG, GIF ili PNG format slike");
                            ViewBag.Error = "Slika nije validna!";
                            return View("Error");
                        }
                            info.ListaSlika.Clear();
                            var slika = new Slika
                            {
                                Alt = info.Firma,
                                KlijentId = info.Id
                            };
                            var uploadDir = "~/Images";
                            var imagePath = Path.Combine(Server.MapPath(uploadDir), imageUpload.FileName);
                            var imageUrl = Path.Combine(uploadDir, imageUpload.FileName);
                            imageUpload.SaveAs(imagePath);
                            slika.Putanja = imageUrl.Remove(0, 1);
                            info.ListaSlika.Add(slika);
                            
                            var slike = db.Slike.Where(s => s.KlijentId == info.Id).ToList();
                            foreach (var s in slike)
                            {
                                db.Slike.Remove(s);
                            }
                            modified = true;
                        }

                         
                         if (model.Adresa != null && model.Broj != null)
                         {
                             string mesto = info.Adresa.Substring(info.Adresa.IndexOf(',') + 2);
                             info.Adresa = model.Adresa + " " + model.Broj + ", " + mesto;
                             modified = true;
                         }
                         if (model.Mesto != null)
                         {
                             info.Adresa = info.Adresa.Substring(0, info.Adresa.IndexOf(','));
                             info.Adresa += ", " + model.Mesto;
                             modified = true;
                         }
                        if(modified)
                        {
                            db.SaveChanges();
                            return RedirectToAction("Manage", new { Message = ManageMessageId.ChangeDataSuccess });
                        } 
                    }
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

       

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            ChangeDataSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }

    /// <summary>
    /// Represents a globally unique identifier (GUID) with a
    /// shorter string value. Sguid
    /// </summary>
    public struct ShortGuid
    {
        #region Static

        /// <summary>
        /// A read-only instance of the ShortGuid class whose value
        /// is guaranteed to be all zeroes.
        /// </summary>
        public static readonly ShortGuid Empty = new ShortGuid(Guid.Empty);

        #endregion

        #region Fields

        Guid _guid;
        string _value;

        #endregion

        #region Contructors

        /// <summary>
        /// Creates a ShortGuid from a base64 encoded string
        /// </summary>
        /// <param name="value">The encoded guid as a
        /// base64 string</param>
        public ShortGuid(string value)
        {
            _value = value;
            _guid = Decode(value);
        }

        /// <summary>
        /// Creates a ShortGuid from a Guid
        /// </summary>
        /// <param name="guid">The Guid to encode</param>
        public ShortGuid(Guid guid)
        {
            _value = Encode(guid);
            _guid = guid;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets the underlying Guid
        /// </summary>
        public Guid Guid
        {
            get { return _guid; }
            set
            {
                if (value != _guid)
                {
                    _guid = value;
                    _value = Encode(value);
                }
            }
        }

        /// <summary>
        /// Gets/sets the underlying base64 encoded string
        /// </summary>
        public string Value
        {
            get { return _value; }
            set
            {
                if (value != _value)
                {
                    _value = value;
                    _guid = Decode(value);
                }
            }
        }

        #endregion

        #region ToString

        /// <summary>
        /// Returns the base64 encoded guid as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _value;
        }

        #endregion

        #region Equals

        /// <summary>
        /// Returns a value indicating whether this instance and a
        /// specified Object represent the same type and value.
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is ShortGuid)
                return _guid.Equals(((ShortGuid)obj)._guid);
            if (obj is Guid)
                return _guid.Equals((Guid)obj);
            if (obj is string)
                return _guid.Equals(((ShortGuid)obj)._guid);
            return false;
        }

        #endregion

        #region GetHashCode

        /// <summary>
        /// Returns the HashCode for underlying Guid.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _guid.GetHashCode();
        }

        #endregion

        #region NewGuid

        /// <summary>
        /// Initialises a new instance of the ShortGuid class
        /// </summary>
        /// <returns></returns>
        public static ShortGuid NewGuid()
        {
            return new ShortGuid(Guid.NewGuid());
        }

        #endregion

        #region Encode

        /// <summary>
        /// Creates a new instance of a Guid using the string value,
        /// then returns the base64 encoded version of the Guid.
        /// </summary>
        /// <param name="value">An actual Guid string (i.e. not a ShortGuid)</param>
        /// <returns></returns>
        public static string Encode(string value)
        {
            Guid guid = new Guid(value);
            return Encode(guid);
        }

        /// <summary>
        /// Encodes the given Guid as a base64 string that is 22
        /// characters long.
        /// </summary>
        /// <param name="guid">The Guid to encode</param>
        /// <returns></returns>
        public static string Encode(Guid guid)
        {
            string encoded = Convert.ToBase64String(guid.ToByteArray());
            encoded = encoded
              .Replace("/", "_")
              .Replace("+", "-");
            return encoded.Substring(0, 22);
        }

        #endregion

        #region Decode

        /// <summary>
        /// Decodes the given base64 string
        /// </summary>
        /// <param name="value">The base64 encoded string of a Guid</param>
        /// <returns>A new Guid</returns>
        public static Guid Decode(string value)
        {
            value = value
              .Replace("_", "/")
              .Replace("-", "+");
            byte[] buffer = Convert.FromBase64String(value + "==");
            return new Guid(buffer);
        }

        #endregion

        #region Operators

        /// <summary>
        /// Determines if both ShortGuids have the same underlying
        /// Guid value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(ShortGuid x, ShortGuid y)
        {
            if ((object)x == null) return (object)y == null;
            return x._guid == y._guid;
        }

        /// <summary>
        /// Determines if both ShortGuids do not have the
        /// same underlying Guid value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(ShortGuid x, ShortGuid y)
        {
            return !(x == y);
        }

        /// <summary>
        /// Implicitly converts the ShortGuid to it's string equivilent
        /// </summary>
        /// <param name="shortGuid"></param>
        /// <returns></returns>
        public static implicit operator string(ShortGuid shortGuid)
        {
            return shortGuid._value;
        }

        /// <summary>
        /// Implicitly converts the ShortGuid to it's Guid equivilent
        /// </summary>
        /// <param name="shortGuid"></param>
        /// <returns></returns>
        public static implicit operator Guid(ShortGuid shortGuid)
        {
            return shortGuid._guid;
        }

        /// <summary>
        /// Implicitly converts the string to a ShortGuid
        /// </summary>
        /// <param name="shortGuid"></param>
        /// <returns></returns>
        public static implicit operator ShortGuid(string shortGuid)
        {
            return new ShortGuid(shortGuid);
        }

        /// <summary>
        /// Implicitly converts the Guid to a ShortGuid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static implicit operator ShortGuid(Guid guid)
        {
            return new ShortGuid(guid);
        }

        #endregion
    }

   
}