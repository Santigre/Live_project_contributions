using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using Newtonsoft.Json;
using JobPlacementDashboard.Models;
using Microsoft.AspNet.Identity;

namespace JobPlacementDashboard.Controllers
{
    public class JPBulletinsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(string sortType)
        {
            //Set message for no results page
            if ((sortType == null) || (sortType == "") || (sortType == "All")) ViewBag.SortMessage = "results";
            else ViewBag.SortMessage = sortType;

            //Sorting Selection
            var BulletinList = new List<JPBulletin>();
            ViewBag.AdviceSortParm = sortType == "Advice" ? "Advice" : "Advice";
            ViewBag.JobPostingSortParm = sortType == "Job Postings" ? "Job Postings" : "Job Postings";
            ViewBag.EventsSortParm = sortType == "Events" ? "Events" : "Events";
            ViewBag.AllSortParm = sortType == "All" ? "All" : "All";

            switch (sortType)
            {
                case "Advice":
                    BulletinList = db.JPBulletins.Where(x => x.BulletinCategoryEnum == BulletinCategoryEnum.Advice).ToList();
                    ViewBag.ResultsFound = BulletinList.Count > 0 ? true : false;
                    break;
                case "Job Postings":
                    BulletinList = db.JPBulletins.Where(x => x.BulletinCategoryEnum == BulletinCategoryEnum.JobPostings).ToList();
                    ViewBag.ResultsFound = BulletinList.Count > 0 ? true : false;
                    break;
                case "Events":
                    BulletinList = db.JPBulletins.Where(x => x.BulletinCategoryEnum == BulletinCategoryEnum.Events).ToList();
                    ViewBag.ResultsFound = BulletinList.Count > 0 ? true : false;
                    break;
                case "All":
                    BulletinList = db.JPBulletins.ToList();
                    ViewBag.ResultsFound = BulletinList.Count > 0 ? true : false;
                    break;
                default:
                    BulletinList = db.JPBulletins.ToList();
                    ViewBag.ResultsFound = BulletinList.Count > 0 ? true : false;
                    break;

            }
      
            return View(BulletinList.OrderByDescending(x => x.BulletinDate));
        }

        // GET: JPBulletins/Create
        public ActionResult Create()
        {
            return View();
        }
 

        // POST: JPBulletins/SaveMeetupEvents
        [HttpPost]
        public void SaveMeetupEvents(List<string> eventNames, List<string> eventLinks, List<string> eventDates, List<string> eventLocations)
        {
            var all = from c in db.JPMeetupEvents select c;
            db.JPMeetupEvents.RemoveRange(all);         
            
            for (int i = 0; i < eventNames.Count; i++)
            {
                var meetupEvent = new JPMeetupEvent();
                DateTime eventDate;
                DateTime.TryParse(eventDates[i],out eventDate);
                meetupEvent.JPEventName = eventNames[i];
                meetupEvent.JPEventLink = eventLinks[i];
                meetupEvent.JPEventDate = eventDate;
                meetupEvent.JPLocation = eventLocations[i];
                db.JPMeetupEvents.Add(meetupEvent);
            }

            db.SaveChanges();
        }

        // POST: JPBulletins/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BulletinId,BulletinCategoryEnum,BulletinBody,BulletinDate")] JPBulletin jPBulletin)
        {
            jPBulletin.BulletinDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.JPBulletins.Add(jPBulletin);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(jPBulletin);
        }

        // GET: JPBulletins/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JPBulletin jPBulletin = db.JPBulletins.Find(id);
            if (jPBulletin == null)
            {
                return HttpNotFound();
            }
            return View(jPBulletin);
        }

        // POST: JPBulletins/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BulletinId,BulletinBody,BulletinDate")] JPBulletin jPBulletin)
        {
            if (ModelState.IsValid)
            {
                db.Entry(jPBulletin).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(jPBulletin);
        }

        // GET: JPBulletins/Delete/5
        
        public void Delete(int? id)
        {
            if (id == null)
            {
                new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JPBulletin jPBulletin = db.JPBulletins.Find(id);
            if (jPBulletin == null)
            {
                HttpNotFound();
            }
            db.JPBulletins.Remove(jPBulletin);
            db.SaveChanges();            
        }

        // POST: JPBulletins/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    JPBulletin jPBulletin = db.JPBulletins.Find(id);
        //    db.JPBulletins.Remove(jPBulletin);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}



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
