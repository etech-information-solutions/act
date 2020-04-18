using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ACT.Data.Models;

namespace ACT.UI.Views.ClientLoads
{
    public class ChepLoadsController : Controller
    {
        private ACTEntities db = new ACTEntities();

        // GET: ChepLoads
        public ActionResult Index()
        {
            return View(db.ChepLoads.ToList());
        }

        // GET: ChepLoads/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChepLoad chepLoad = db.ChepLoads.Find(id);
            if (chepLoad == null)
            {
                return HttpNotFound();
            }
            return View(chepLoad);
        }

        // GET: ChepLoads/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ChepLoads/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CreatedOn,ModfiedOn,ModifiedBy,LoadDate,NotifyDate,EffectiveDate,PostingType,AccountNumber,ClientDescription,DeliveryNote,ReferenceNumber,ReceiverNumber,Equipment,OriginalQuantity,NewQuantity,DocketNumber,Status")] ChepLoad chepLoad)
        {
            if (ModelState.IsValid)
            {
                db.ChepLoads.Add(chepLoad);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(chepLoad);
        }

        // GET: ChepLoads/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChepLoad chepLoad = db.ChepLoads.Find(id);
            if (chepLoad == null)
            {
                return HttpNotFound();
            }
            return View(chepLoad);
        }

        // POST: ChepLoads/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CreatedOn,ModfiedOn,ModifiedBy,LoadDate,NotifyDate,EffectiveDate,PostingType,AccountNumber,ClientDescription,DeliveryNote,ReferenceNumber,ReceiverNumber,Equipment,OriginalQuantity,NewQuantity,DocketNumber,Status")] ChepLoad chepLoad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chepLoad).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(chepLoad);
        }

        // GET: ChepLoads/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChepLoad chepLoad = db.ChepLoads.Find(id);
            if (chepLoad == null)
            {
                return HttpNotFound();
            }
            return View(chepLoad);
        }

        // POST: ChepLoads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ChepLoad chepLoad = db.ChepLoads.Find(id);
            db.ChepLoads.Remove(chepLoad);
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
