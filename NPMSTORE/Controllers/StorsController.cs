using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NPMSTORE.Models;

namespace NPMSTORE.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StorsController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public StorsController(ModelContext context , IWebHostEnvironment webHostEnviroment)
        {
            _context = context;
            _webHostEnviroment = webHostEnviroment;
        }

        // GET: Stors
        public async Task<IActionResult> Index()
        {
            var storecategory = await _context.StoreCategories.ToListAsync();
            var stor = await _context.Stors.ToListAsync();
            var products = await _context.Products.ToListAsync();


            //Join TO feth Data Stor, Each Products contain
            var viewModel1 = (
                             from s in stor
                             join sc in storecategory on s.Id equals sc.Storeid 
                             group s by new { s.Name, s.Id ,s.Image } into g
                             select new ProductsVM{
                                 ImgStore = g.Key.Image,
                                 NameStore =g.Key.Name ,
                                 IdStoe =g.Key.Id,
                                 productlist = (from p in products
                                               join scs in storecategory on p.Id equals scs.Productsid
                                               where scs.Productsid != null && scs.Storeid == g.Key.Id
                                               select new Product{Price = p.Price , Seales = p.Seales}
                                               ).ToList() ?? null
                             }).ToList();
            return View(viewModel1); 
        }

        // GET: Stors/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var stor = await _context.Stors.ToListAsync();
            var category = await _context.Categories.ToListAsync();
            var products = await _context.Products.ToListAsync();
            var storecategory = await _context.StoreCategories.ToListAsync();

            var categoryVM = (from s in stor
                             join sc in storecategory on s.Id equals sc.Storeid
                             join c in category on sc.Categoryid equals c.Id
                             where sc.Storeid == id
                             group c by new { c.Name,c.Id } into g
                             select new CategoryVM
                             {
                                 cateName = g.Key.Name,
                                 cateId = g.Key.Id,
                                 category = g.ToList(),
                             }).ToList(); // Select name Category in  all store

            var productsVM = (
                              from s in stor
                              join sc in storecategory on s.Id equals sc.Storeid
                              where s.Id == id
                              group s by new { s.Name, s.Id, s.Image,s.Detailes } into g
                              select new ProductsVM
                              {
                                  ImgStore = g.Key.Image,
                                  NameStore = g.Key.Name,
                                  DetiStore = g.Key.Detailes,
                                  IdStoe = g.Key.Id,
                                  productlist = (from p in products
                                                 join scs in storecategory on p.Id equals scs.Productsid
                                                 where scs.Productsid != null && scs.Storeid == g.Key.Id
                                                 select new Product {Id = p.Id, Name=p.Name, Price = p.Price, Seales = p.Seales }
                                                ).ToList() ?? null
                              }).ToList();

            if (stor == null)
            {
                return NotFound();
            }
            var ModelView = Tuple.Create<IEnumerable<ProductsVM>, IEnumerable<CategoryVM>>(productsVM, categoryVM);
            return View(ModelView);
        }

        // GET: Stors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Stors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ImageFile,Detailes")] Stor stor)
        {
            if (ModelState.IsValid)
            {
                if (stor.ImageFile != null)
                {
                    string wwwRootPath = _webHostEnviroment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + "_" +
                    stor.ImageFile.FileName;
                    string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await stor.ImageFile.CopyToAsync(fileStream);
                    }
                    stor.Image = fileName;
                }

                stor.Customerid = 10;
                _context.Add(stor);
                await _context.SaveChangesAsync();
                
                // Add The Category Others in this 
                StoreCategory cate = new StoreCategory();
                cate.Storeid = stor.Id;
                cate.Categoryid = 3; // Category Others
                _context.Add(cate);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(stor);
        }

        // GET: Stors/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stor = await _context.Stors.FindAsync(id);
            if (stor == null)
            {
                return NotFound();
            }
            return View(stor);
        }

        // POST: Stors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,Name,ImageFile,Image,Detailes")] Stor stor)
        {
            if (id != stor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (stor.ImageFile != null)
                    {
                        string wwwRootPath = _webHostEnviroment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + "_" +
                        stor.ImageFile.FileName;
                        string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await stor.ImageFile.CopyToAsync(fileStream);
                        }
                        stor.Image = fileName;
                    }
                    stor.Customerid = 10;
                    _context.Update(stor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StorExists(stor.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Customerid"] = new SelectList(_context.Customers, "Id", "Email", stor.Customerid);
            return View(stor);
        }

        // GET: Stors/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stor = await _context.Stors
                .Include(s => s.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stor == null)
            {
                return NotFound();
            }

            return View(stor);
        }

        // POST: Stors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var stor = await _context.Stors.FindAsync(id);
            _context.Stors.Remove(stor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // Action to Show Categries in Stro

        [HttpGet]
        [Route("Stors/CateDetails/{storeid:int}/{cateid:int}")]
        public async Task<IActionResult> CateDetails(decimal? storeid, decimal? cateid)
        {
            var stor = await _context.Stors.FindAsync(storeid);
            var cate = await _context.Categories.FindAsync(cateid);
            if(cate == null)
            {
                return NotFound();
            }
            var products = (from  sc in _context.StoreCategories.ToList()
                             join p in _context.Products.ToList() on sc.Productsid equals p.Id
                             where sc.Storeid == storeid && sc.Categoryid == cateid && sc.Productsid != null
                             select new Product{Name = p.Name , Id = p.Id , Image = p.Image ,Price = p.Price ,Seales = p.Seales}).ToList();

            AllProductInCategoryVM viewModel = new AllProductInCategoryVM();
            viewModel.storID = stor.Id;
            viewModel.storName = stor.Name;
            viewModel.cateId = cate.Id;
            viewModel.cateName = cate.Name;
            viewModel.prodList = products;

            return View(viewModel);
        }

        [HttpPost]
        [Route("Stors/CateDetails/{storeid:int}/{cateid:int}"),ActionName("CateDetails")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CateDelete(decimal storeid, decimal cateid)
        {
            var stor = await _context.Stors.AnyAsync(s=>s.Id == storeid);
            var cate = await _context.Categories.AnyAsync(c => c.Id ==cateid);
            if(!cate || !stor)
            {
                return NotFound();
            }
            var category = (from sc in _context.StoreCategories.ToList()
                           where sc.Storeid == storeid && sc.Categoryid == cateid
                           select new StoreCategory{Id=sc.Id,Productsid = sc.Productsid ,Categoryid= sc.Categoryid ,Storeid=sc.Storeid
                           }).ToList();
            
            // To Prevent Traking
            using(ModelContext context = new ModelContext())
            {
                context.StoreCategories.RemoveRange(category);
                context.SaveChanges();
            }
            
            return RedirectToAction(nameof(CateDetails));
        }

        private bool StorExists(decimal id)
        {
            return _context.Stors.Any(e => e.Id == id);
        }
    }
}
