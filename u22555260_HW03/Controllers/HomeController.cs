using Newtonsoft.Json;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using u22555260_HW03.Models;
using System.Data;
using OfficeOpenXml;
using System.IO;
using System.Web.UI.WebControls;
using System.Net.Mime;
using System;

namespace u22555260_HW03.Controllers
{
    public class HomeController : Controller
    {

        private LibraryEntities1 db = new LibraryEntities1();

        public async Task<ActionResult> Index()
        {
               var viewModel = new CombindedTableViews
                {
                    Authors = await db.authors.ToListAsync(),
                    Books = await db.books.ToListAsync(),
                    Borrows = await db.borrows.ToListAsync(),
                    Students = await db.students.ToListAsync(),
                    Types = await db.types.ToListAsync()
                };

                return View(viewModel);
            
        }
       

        public async Task<ActionResult> Maintain()
        {
            var viewModel = new CombindedTableViews
            {
                Authors = await db.authors.ToListAsync(),
                Books = await db.books.ToListAsync(),
                Borrows = await db.borrows.ToListAsync(),
                Students = await db.students.ToListAsync(),
                Types = await db.types.ToListAsync()
            };

            return View(viewModel);
        }

        public async Task<ActionResult> Report()
        {

            var bookData = await db.borrows
                    .GroupBy(book => book.books.name) 
                    .Select(group => new BookCategoryViewModel
                    {
                        Categories = group.Key,
                        NumberOfBooks = group.Count()
                    })
                    .ToListAsync();

            ViewBag.ChartData = JsonConvert.SerializeObject(bookData);

            var viewModel = new BookCategoryViewModel
            {
                downloadedFiles = await db.DownloadedFiles.ToListAsync(),
            };

            return View(viewModel);

        }

        public void ExportReport(string fileName)
        {      
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var bookData = db.borrows
                    .GroupBy(book => book.books.name)
                    .Select(group => new BookCategoryViewModel
                    {
                        Categories = group.Key,
                        NumberOfBooks = group.Count()
                    })
                    .ToList();

            var package = new ExcelPackage();

            var worksheet = package.Workbook.Worksheets.Add("Reports");
            worksheet.Cells["A1"].LoadFromCollection(bookData, true);


            var tempFilePath = "C:/Users/ghost/Desktop/Reports/"+fileName+ ".xlsx";
           // package.SaveAs(new FileInfo(tempFilePath));


            var savedReport = new DownloadedFiles
            {
                FileName = fileName,
                FileType = "xslx",
                UserID = 1,
                DateDownloaded = DateTime.Now,
                FilePath = tempFilePath,
                studentID = 1,
            };

            db.DownloadedFiles.Add(savedReport);
            db.SaveChangesAsync();    

            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename="+fileName+".xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(package.GetAsByteArray());
            Response.End();

                 
        }

        public void SavePDF(string fileName, string fileType)
        {
            var tempFilePath = "C:/Users/ghost/Desktop/Reports/" + fileName + "." + fileType;

            var savePdf = new DownloadedFiles
            { 
                FileName = fileName,
                FileType = "pdf",
                UserID = 1, 
                DateDownloaded = DateTime.Now,  
                FilePath= tempFilePath,
                studentID=1,
            };

            db.DownloadedFiles.Add(savePdf);
            db.SaveChanges();
        }

        public async Task<ActionResult> AuthorDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            authors authors = await db.authors.FindAsync(id);
            if (authors == null)
            {
                return HttpNotFound();
            }
            return View(authors);
        } 

        // GET: authors/Create
        public ActionResult AuthorCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AuthorCreate([Bind(Include = "authorId,name,surname")] authors authors)
        {
            if (ModelState.IsValid)
            {
                db.authors.Add(authors);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(authors);
        }

        // GET: authors/Edit/5
        public async Task<ActionResult> AuthorEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            authors authors = await db.authors.FindAsync(id);
            if (authors == null)
            {
                return HttpNotFound();
            }
            return View(authors);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AuthorEdit([Bind(Include = "authorId,name,surname")] authors authors)
        {
            if (ModelState.IsValid)
            {
                db.Entry(authors).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(authors);
        }

        // GET: authors/Delete/5
        public async Task<ActionResult> AuthorDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            authors authors = await db.authors.FindAsync(id);
            if (authors == null)
            {
                return HttpNotFound();
            }
            return View(authors);
        }

        // POST: authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AuthorDeleteConfirmed(int id)
        {
            authors authors = await db.authors.FindAsync(id);
            db.authors.Remove(authors);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        //Book

        public async Task<ActionResult> BookDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            books books = await db.books.FindAsync(id);
            if (books == null)
            {
                return HttpNotFound();
            }
            return View(books);
        }

        // GET: books/Create
        public ActionResult BookCreate()
        {
            ViewBag.authorId = new SelectList(db.authors, "authorId", "name");
            ViewBag.typeId = new SelectList(db.types, "typeId", "name");
            return View();
        }

        // POST: books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BookCreate([Bind(Include = "bookId,name,pagecount,point,authorId,typeId")] books books)
        {
            if (ModelState.IsValid)
            {
                db.books.Add(books);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.authorId = new SelectList(db.authors, "authorId", "name", books.authorId);
            ViewBag.typeId = new SelectList(db.types, "typeId", "name", books.typeId);
            return View(books);
        }

        // GET: books/Edit/5
        public async Task<ActionResult> BookEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            books books = await db.books.FindAsync(id);
            if (books == null)
            {
                return HttpNotFound();
            }
            ViewBag.authorId = new SelectList(db.authors, "authorId", "name", books.authorId);
            ViewBag.typeId = new SelectList(db.types, "typeId", "name", books.typeId);
            return View(books);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BookEdit([Bind(Include = "bookId,name,pagecount,point,authorId,typeId")] books books)
        {
            if (ModelState.IsValid)
            {
                db.Entry(books).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.authorId = new SelectList(db.authors, "authorId", "name", books.authorId);
            ViewBag.typeId = new SelectList(db.types, "typeId", "name", books.typeId);
            return View(books);
        }

        // GET: books/Delete/5
        public async Task<ActionResult> BookDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            books books = await db.books.FindAsync(id);
            if (books == null)
            {
                return HttpNotFound();
            }
            return View(books);
        }

        // POST: books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BookDeleteConfirmed(int id)
        {
            books books = await db.books.FindAsync(id);
            db.books.Remove(books);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //Borrow

        public async Task<ActionResult> BorrowDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            borrows borrows = await db.borrows.FindAsync(id);
            if (borrows == null)
            {
                return HttpNotFound();
            }
            return View(borrows);
        }

        // GET: borrows/Create
        public ActionResult BorrowCreate()
        {
            ViewBag.bookId = new SelectList(db.books, "bookId", "name");
            ViewBag.studentId = new SelectList(db.students, "studentId", "name");
            return View();
        }

        // POST: borrows/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BorrowCreate([Bind(Include = "borrowId,studentId,bookId,takenDate,broughtDate")] borrows borrows)
        {
            if (ModelState.IsValid)
            {
                db.borrows.Add(borrows);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.bookId = new SelectList(db.books, "bookId", "name", borrows.bookId);
            ViewBag.studentId = new SelectList(db.students, "studentId", "name", borrows.studentId);
            return View(borrows);
        }

        // GET: borrows/Edit/5
        public async Task<ActionResult> BorrowEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            borrows borrows = await db.borrows.FindAsync(id);
            if (borrows == null)
            {
                return HttpNotFound();
            }
            ViewBag.bookId = new SelectList(db.books, "bookId", "name", borrows.bookId);
            ViewBag.studentId = new SelectList(db.students, "studentId", "name", borrows.studentId);
            return View(borrows);
        }

        // POST: borrows/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BorrowEdit([Bind(Include = "borrowId,studentId,bookId,takenDate,broughtDate")] borrows borrows)
        {
            if (ModelState.IsValid)
            {
                db.Entry(borrows).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.bookId = new SelectList(db.books, "bookId", "name", borrows.bookId);
            ViewBag.studentId = new SelectList(db.students, "studentId", "name", borrows.studentId);
            return View(borrows);
        }

        // GET: borrows/Delete/5
        public async Task<ActionResult> BorrowDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            borrows borrows = await db.borrows.FindAsync(id);
            if (borrows == null)
            {
                return HttpNotFound();
            }
            return View(borrows);
        }

        // POST: borrows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BorrowDeleteConfirmed(int id)
        {
            borrows borrows = await db.borrows.FindAsync(id);
            db.borrows.Remove(borrows);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //Student

        public async Task<ActionResult> StudentDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            students students = await db.students.FindAsync(id);
            if (students == null)
            {
                return HttpNotFound();
            }
            return View(students);
        }

        // GET: students/Create
        public ActionResult StudentCreate()
        {
            return View();
        }

        // POST: students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StudentCreate([Bind(Include = "studentId,name,surname,birthdate,gender,class,point")] students students)
        {
            if (ModelState.IsValid)
            {
                db.students.Add(students);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(students);
        }

        // GET: students/Edit/5
        public async Task<ActionResult> StudentEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            students students = await db.students.FindAsync(id);
            if (students == null)
            {
                return HttpNotFound();
            }
            return View(students);
        }

        // POST: students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StudentEdit([Bind(Include = "studentId,name,surname,birthdate,gender,class,point")] students students)
        {
            if (ModelState.IsValid)
            {
                db.Entry(students).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(students);
        }

        // GET: students/Delete/5
        public async Task<ActionResult> StudentDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            students students = await db.students.FindAsync(id);
            if (students == null)
            {
                return HttpNotFound();
            }
            return View(students);
        }

        // POST: students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StudentDeleteConfirmed(int id)
        {
            students students = await db.students.FindAsync(id);
            db.students.Remove(students);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //Type

        public async Task<ActionResult> TypeDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            types types = await db.types.FindAsync(id);
            if (types == null)
            {
                return HttpNotFound();
            }
            return View(types);
        }

        // GET: types/Create
        public ActionResult TypeCreate()
        {
            return View();
        }

        // POST: types/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TypeCreate([Bind(Include = "typeId,name")] types types)
        {
            if (ModelState.IsValid)
            {
                db.types.Add(types);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(types);
        }

        // GET: types/Edit/5
        public async Task<ActionResult> TypeEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            types types = await db.types.FindAsync(id);
            if (types == null)
            {
                return HttpNotFound();
            }
            return View(types);
        }

        // POST: types/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TypeEdit([Bind(Include = "typeId,name")] types types)
        {
            if (ModelState.IsValid)
            {
                db.Entry(types).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(types);
        }

        // GET: types/Delete/5
        public async Task<ActionResult> TypeDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            types types = await db.types.FindAsync(id);
            if (types == null)
            {
                return HttpNotFound();
            }
            return View(types);
        }

        // POST: types/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TypeDeleteConfirmed(int id)
        {
            types types = await db.types.FindAsync(id);
            db.types.Remove(types);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: DownloadedFiles/Details/5
        public async Task<ActionResult> DownloadDetails(int? id)
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
        public ActionResult DownloadCreate()
        {
            ViewBag.studentID = new SelectList(db.students, "studentId", "name");
            return View();
        }

        // POST: DownloadedFiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DownloadCreate([Bind(Include = "FileID,FileName,FileType,UserID,DateDownloaded,studentID,FilePath")] DownloadedFiles downloadedFiles)
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
        public async Task<ActionResult> DownloadEdit(int? id)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DownloadEdit([Bind(Include = "FileID,FileName,FileType,UserID,DateDownloaded,studentID,FilePath")] DownloadedFiles downloadedFiles)
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
        public async Task<ActionResult> DownloadDelete(int? id)
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
        public async Task<ActionResult> DownloadDeleteConfirmed(int id)
        {
            DownloadedFiles downloadedFiles = await db.DownloadedFiles.FindAsync(id);
            db.DownloadedFiles.Remove(downloadedFiles);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}