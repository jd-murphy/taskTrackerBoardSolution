﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using taskTrackerBoardProject.Models;

namespace taskTrackerBoardProject.Controllers
{
    public class TasksController : Controller
    {
        private TaskDBContext db = new TaskDBContext();

        // GET: Tasks
        public ActionResult Index(string taskTag, string searchString)
        {
            var tagList = new List<string>();
            var tagQry = from d in db.Tasks
                         orderby d.Tag
                         select d.Tag;
            tagList.AddRange(tagQry.Distinct());
            ViewBag.taskTag = new SelectList(tagList);

            var tasks = from t in db.Tasks
                        select t;

            if (!String.IsNullOrEmpty(searchString))
            {
                tasks = tasks.Where(s => (s.Title.Contains(searchString) ||
                                            s.Description.Contains(searchString) ||
                                            s.Tag.Contains(searchString)));
            }

            if (!String.IsNullOrEmpty(taskTag))
            {
                tasks = tasks.Where(x => x.Tag == taskTag);
            }

            return View(tasks);
        }

        // GET: Tasks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = db.Tasks.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            return View(task);
        }

        // GET: Tasks/Create
        public ActionResult Create(int boardId)
        {
            Task t = new Task();
            t.BoardID = boardId;
            t.DueDate = DateTime.Now;
            return View(t);
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Description,CreatedDate,DueDate,Tag,CurrentStatus,BoardID")] Task task)
        {  
            if (ModelState.IsValid)
            {
                db.Tasks.Add(task);
                db.SaveChanges();
                return RedirectToAction("Details", "Boards", new { id = task.BoardID });
            }

            return View(task);
        }

        // GET: Tasks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = db.Tasks.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            return View(task);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Description,CreatedDate,DueDate,Tag,CurrentStatus,BoardID")] Task task)
        {
            if (ModelState.IsValid)
            {
                db.Entry(task).State = EntityState.Modified;
                db.SaveChanges();
                //return RedirectToAction("Index");
                return RedirectToAction("Details", "Boards", new { id = task.BoardID });

            }
            return View(task);
        }

        [HttpPost]
        public JsonResult UpdateStatusAjax(int? taskID, string newStatus)
        {
            if (taskID == null || newStatus == null)
            {
                return Json(new { taskIDUpdated = "Error", newTaskStatus = "Error" });
            }
            Task task = db.Tasks.Find(taskID);
            if (task == null)
            {
                return Json(new { taskIDUpdated = "Error", newTaskStatus = "Error" });
            }
            task.CurrentStatus = (Status) Enum.Parse(typeof(Status), newStatus);
            db.Entry(task).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { taskIDUpdated = taskID, newTaskStatus = newStatus });
        }

        // GET: Tasks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = db.Tasks.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Task task = db.Tasks.Find(id);
            db.Tasks.Remove(task);
            db.SaveChanges();
            return RedirectToAction("Details", "Boards", new { id = task.BoardID });
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
