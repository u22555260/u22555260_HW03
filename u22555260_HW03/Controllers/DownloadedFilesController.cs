using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using u22555260_HW03.Models;

namespace u22555260_HW03.Controllers
{
    public class DownloadedFilesController : Controller
    {
        private LibraryEntities1 db = new LibraryEntities1();

        // GET: DownloadedFiles
        public async Task<ActionResult> Index()
        {
            var downloadedFiles = db.DownloadedFiles.Include(d => d.students);
            return View(await downloadedFiles.ToListAsync());
        }

        // GET: DownloadedFiles/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DownloadedFiles downloadedFiles = await db.DownloadedFiles.FindAsync(id);
            if (downloadedFiles == null)
            {
                return HttpNotFound();
            }
            return View(downloadedFiles);
        }

        // GET: DownloadedFiles/Create
        public ActionResult Create()
        {
            ViewBag.studentID = new SelectList(db.students, "studentId", "name");
            return View();
        }

        // POST: DownloadedFiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "FileID,FileName,FileType,UserID,DateDownloaded,studentID,FilePath")] DownloadedFiles downloadedFiles)
        {
            if (ModelState.IsValid)
            {
                db.DownloadedFiles.Add(downloadedFiles);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.studentID = new SelectList(db.students, "studentId", "name", downloadedFiles.studentID);
            return View(downloadedFiles);
        }

        // GET: DownloadedFiles/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DownloadedFiles downloadedFiles = await db.DownloadedFiles.FindAsync(id);
            if (downloadedFiles == null)
            {
                return HttpNotFound();
            }
            ViewBag.studentID = new SelectList(db.students, "studentId", "name", downloadedFiles.studentID);
            return View(downloadedFiles);
        }

        // POST: DownloadedFiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "FileID,FileName,FileType,UserID,DateDownloaded,studentID,FilePath")] DownloadedFiles downloadedFiles)
        {
            if (ModelState.IsValid)
            {
                db.Entry(downloadedFiles).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.studentID = new SelectList(db.students, "studentId", "name", downloadedFiles.studentID);
            return View(downloadedFiles);
        }

        // GET: DownloadedFiles/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DownloadedFiles downloadedFiles = await db.DownloadedFiles.FindAsync(id);
            if (downloadedFiles == null)
            {
                return HttpNotFound();
            }
            return View(downloadedFiles);
        }

        // POST: DownloadedFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            DownloadedFiles downloadedFiles = await db.DownloadedFiles.FindAsync(id);
            db.DownloadedFiles.Remove(downloadedFiles);
            await db.SaveChangesAsync();
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
