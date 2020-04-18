using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ACT.Data.Models;

namespace ACT.UI
{
    public class ClientLoadsController : Controller
    {
        private ACTEntities db = new ACTEntities();

        // GET: ClientLoads
        public ActionResult Index()
        {
            var clientLoads = db.ClientLoads.Include(c => c.ChepClient).Include(c => c.Client).Include(c => c.Vehicle);
            return View(clientLoads.ToList());
        }

        // GET: ClientLoads/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientLoad clientLoad = db.ClientLoads.Find(id);
            if (clientLoad == null)
            {
                return HttpNotFound();
            }
            return View(clientLoad);
        }

        // GET: ClientLoads/Create
        public ActionResult Create()
        {
            ViewBag.ClientId = new SelectList(db.ChepClients, "Id", "ModifiedBy");
            ViewBag.ClientId = new SelectList(db.Clients, "Id", "ModifiedBy");
            ViewBag.VehicleId = new SelectList(db.Vehicles, "Id", "ObjectType");
            return View();
        }

        // POST: ClientLoads/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ClientId,VehicleId,CreatedOn,ModifiedOn,ModifiedBy,LoadNumber,LoadDate,EffectiveDate,NotifyeDate,AccountNumber,ClientDescription,DeliveryNote,ReferenceNumber,ReceiverNumber,Equipment,OriginalQuantity,NewQuantity,ReconcileInvoice,ReconcileDate,PODNumber,PCNNumber,PRNNumber,Status")] ClientLoad clientLoad)
        {
            if (ModelState.IsValid)
            {
                db.ClientLoads.Add(clientLoad);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClientId = new SelectList(db.ChepClients, "Id", "ModifiedBy", clientLoad.ClientId);
            ViewBag.ClientId = new SelectList(db.Clients, "Id", "ModifiedBy", clientLoad.ClientId);
            ViewBag.VehicleId = new SelectList(db.Vehicles, "Id", "ObjectType", clientLoad.VehicleId);
            return View(clientLoad);
        }

        // GET: ClientLoads/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientLoad clientLoad = db.ClientLoads.Find(id);
            if (clientLoad == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientId = new SelectList(db.ChepClients, "Id", "ModifiedBy", clientLoad.ClientId);
            ViewBag.ClientId = new SelectList(db.Clients, "Id", "ModifiedBy", clientLoad.ClientId);
            ViewBag.VehicleId = new SelectList(db.Vehicles, "Id", "ObjectType", clientLoad.VehicleId);
            return View(clientLoad);
        }

        // POST: ClientLoads/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ClientId,VehicleId,CreatedOn,ModifiedOn,ModifiedBy,LoadNumber,LoadDate,EffectiveDate,NotifyeDate,AccountNumber,ClientDescription,DeliveryNote,ReferenceNumber,ReceiverNumber,Equipment,OriginalQuantity,NewQuantity,ReconcileInvoice,ReconcileDate,PODNumber,PCNNumber,PRNNumber,Status")] ClientLoad clientLoad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clientLoad).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClientId = new SelectList(db.ChepClients, "Id", "ModifiedBy", clientLoad.ClientId);
            ViewBag.ClientId = new SelectList(db.Clients, "Id", "ModifiedBy", clientLoad.ClientId);
            ViewBag.VehicleId = new SelectList(db.Vehicles, "Id", "ObjectType", clientLoad.VehicleId);
            return View(clientLoad);
        }

        // GET: ClientLoads/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientLoad clientLoad = db.ClientLoads.Find(id);
            if (clientLoad == null)
            {
                return HttpNotFound();
            }
            return View(clientLoad);
        }

        // POST: ClientLoads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ClientLoad clientLoad = db.ClientLoads.Find(id);
            db.ClientLoads.Remove(clientLoad);
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
