using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PronadjiUBanovcima.Models;

namespace PronadjiUBanovcima.Controllers
{
    public class DelatnostController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Delatnost/
        public ActionResult Index()
        {
            return View(db.Delatnosti.ToList());
        }

        // GET: /Delatnost/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Delatnost delatnost = db.Delatnosti.Find(id);
            if (delatnost == null)
            {
                return HttpNotFound();
            }
            return View(delatnost);
        }

        // GET: /Delatnost/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Delatnost/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Naziv")] Delatnost delatnost)
        {
            if (ModelState.IsValid)
            {
                db.Delatnosti.Add(delatnost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(delatnost);
        }

        // GET: /Delatnost/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Delatnost delatnost = db.Delatnosti.Find(id);
            if (delatnost == null)
            {
                return HttpNotFound();
            }
            return View(delatnost);
        }

        // POST: /Delatnost/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Naziv")] Delatnost delatnost)
        {
            if (ModelState.IsValid)
            {
                db.Entry(delatnost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(delatnost);
        }

        // GET: /Delatnost/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Delatnost delatnost = db.Delatnosti.Find(id);
            if (delatnost == null)
            {
                return HttpNotFound();
            }
            return View(delatnost);
        }

        // POST: /Delatnost/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Delatnost delatnost = db.Delatnosti.Find(id);
            db.Delatnosti.Remove(delatnost);
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
