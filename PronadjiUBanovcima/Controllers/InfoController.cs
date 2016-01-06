using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PronadjiUBanovcima.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;


namespace PronadjiUBanovcima.Controllers
{
    [Authorize(Roles="Admin")]
    public class InfoController : Controller
    {


         public InfoController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

         public InfoController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }


        private ApplicationDbContext db = new ApplicationDbContext();
        public UserManager<ApplicationUser> UserManager { get; private set; }
        // GET: /Info/
        [AllowAnonymous]
        public ActionResult Index(string SearchString)
        {
            List<Info> podaci = new List<Info>();

            if (SearchString != "")
            {
                podaci = db.Podaci.Where(p => p.Firma.Contains(SearchString)).ToList();
                List<Info> podaciRaw = db.Podaci.ToList();
                List<Delatnost> delatnosti = db.Delatnosti.Where(d => d.Naziv.Contains(SearchString)).ToList();

                int[] delIds = new int[delatnosti.Count];
                for (int i = 0; i < delatnosti.Count; i++)
                {
                    delIds[i] = delatnosti[i].Id;
                }

                podaci.AddRange(db.Podaci
                   .Where(x => x.ListaDelatnosti.Any(r => delIds.Contains(r.Id))).ToList());

                List<Tag> tagovi = db.Tagovi.Where(t => t.Naziv.Contains(SearchString)).ToList();
                int[] tagIds = new int[tagovi.Count];
                for (int i = 0; i < tagovi.Count; i++)
                    tagIds[i] = tagovi[i].Id;

                podaci.AddRange(db.Podaci.Where(x => x.ListaTagova.Any(t => tagIds.Contains(t.Id))).ToList());
                podaci = podaci.Distinct().ToList();
            }
            else
                podaci = db.Podaci.ToList();

                     return View(podaci);
        }
        [HttpPost]
        [AllowAnonymous]
       
        // GET: /Info/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Info info = db.Podaci.Find(id);
            if (info == null)
            {
                return HttpNotFound();
            }
            return View(info);
        }

        // GET: /Info/Create
        public ActionResult Create()
        {
            return View();
        }

        //// POST: /Info/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Firma,Opis,Telefon,Adresa,Sajt,Email")] Info info)
        {
            if (ModelState.IsValid)
            {
                db.Podaci.Add(info);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(info);
        }

        // GET: /Info/Edit/5
        
        public  ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Info info = db.Podaci.Find(id);
            if (info == null)
            {
                return HttpNotFound();
            }
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var role =  RoleManager.FindByName("Osnovni");
         
            bool osnovni = info.Klijent.Roles.ToList<IdentityUserRole>()[0].Role == role;
            ViewBag.Korisnik = osnovni;
            return View(info);
        }

        // POST: /Info/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Firma,Opis,Telefon,Adresa,Sajt,Email")] Info info)
        {
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var role = RoleManager.FindByName("Osnovni");
            bool osnovni = Request.Form["[0].Role.Name"] == role.Name;
            ViewBag.Korisnik = osnovni;
            string klijent = Request.Form["Klijent.Id"];

            if (ModelState.IsValid)
            {
                string korisnickiPaket = Request.Form["rbtn"];
               
                string staraRola = Rola(korisnickiPaket);
                UserManager.RemoveFromRole(klijent, staraRola);
                UserManager.AddToRole(klijent, korisnickiPaket);
                db.Entry(info).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(info);
        }

        private string Rola(string korisnickiPaket)
        {
            if (korisnickiPaket == "Osnovni")
                return "Premium";
            return "Osnovni";
        }

        // GET: /Info/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Info info = db.Podaci.Find(id);
            if (info == null)
            {
                return HttpNotFound();
            }
            return View(info);
        }

        // POST: /Info/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Info info = db.Podaci.Find(id);
            db.Podaci.Remove(info);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
