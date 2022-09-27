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
    public class ProductsController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public ProductsController(ModelContext context, IWebHostEnvironment webHostEnviroment)
        {
            _context = context;
            _webHostEnviroment = webHostEnviroment;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var product = await _context.StoreCategories.Where(p=>p.Productsid != null).Include(p => p.Products).Include(c => c.Category).Include(s=>s.Store).ToListAsync();
            return View(product);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.StoreCategories
                .Include(p => p.Products).ThenInclude(p => p.ProductAttributes)
                .Include(c => c.Category)
                .Include(s => s.Store)
                .FirstOrDefaultAsync(m => m.Productsid == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["Storeid"] = new SelectList(_context.Stors.ToList(), "Id", "Name");
            ViewData["Categoryid"] = new SelectList(_context.Categories.ToList(), "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StoreCategory componnet)
        {
            if (ModelState.IsValid)
            {
                if (componnet.Products.ImageFile != null)
                {
                    string wwwRootPath = _webHostEnviroment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + "_" +
                    componnet.Products.ImageFile.FileName;
                    string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await componnet.Products.ImageFile.CopyToAsync(fileStream);
                    }
                    componnet.Products.Image = fileName;
                }

                componnet.Products.CreateAt = DateTime.Now;
                _context.Add(componnet.Products);
                await _context.SaveChangesAsync();


                componnet.Productsid = componnet.Products.Id;
                _context.Add(componnet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(componnet);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null){return NotFound();}

            var storecate = await _context.StoreCategories.SingleOrDefaultAsync(p=>p.Id==id);
            var product = await _context.Products.SingleOrDefaultAsync(p=>p.Id == storecate.Productsid);


            var viewModel = new StoreCategoryVM
            {
                product = product,
                storeCategory = storecate
            };

            ViewData["Storeid"] = new SelectList(_context.StoreCategories, "Id", "Name", storecate.Storeid);
            ViewData["Categoryid"] = new SelectList(_context.StoreCategories, "Id", "Name", storecate.Categoryid);

            if (viewModel == null)
            {
                return NotFound();
            }
            return View(viewModel);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id,StoreCategoryVM componnet)
        {
            if (id != componnet.storeCategory.Id) { return NotFound();}
            if (ModelState.IsValid)
            {
                try
                {
                    if (componnet.product.ImageFile != null)
                    {
                        string wwwRootPath = _webHostEnviroment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + "_" +
                        componnet.product.ImageFile.FileName;
                        string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await componnet.product.ImageFile.CopyToAsync(fileStream);
                        }
                        componnet.product.Image = fileName;
                    }

                    componnet.product.CreateAt = DateTime.Now;
                    _context.Update(componnet.product);
                    _context.Update(componnet.storeCategory);
                    await _context.SaveChangesAsync();
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(componnet.product.Id))
                    {
                        return NotFound();
                    }
                    return NotFound();
                }


            return RedirectToAction(nameof(Index));
            }
            return View(componnet);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal Id)
        {
            var product = await _context.Products.FindAsync(Id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            
            var products = _context.StoreCategories.Where(c => c.Productsid == Id).ToList();
            if(products.Count > 0)
            {
                // To Prevent Traking
                using (ModelContext context = new ModelContext())
                {
                    context.StoreCategories.RemoveRange(products);
                    context.SaveChanges();
                }
            }


            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(decimal id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        [HttpGet]
        [Route("Products/AddAttr/{Productid:int}")]
        public IActionResult AddAttr(int Productid)
        {
            if(!ProductExists(Productid))
            {
                return NotFound();
            }

            ViewData["Productid"] = Productid;
            return View();
        }

        //    Now Products Attriubute


        // POST: ProductAttributes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAttr([Bind("Id,Name,Describtion,Productid")] ProductAttribute productAttribute)
        {
            if (ModelState.IsValid)
            {
                productAttribute.Id = 0; 
                _context.Add(productAttribute);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productAttribute);
        }

        // GET: ProductAttributes/Edit/5
        public async Task<IActionResult> EditAttr(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productAttribute = await _context.ProductAttributes.FindAsync(id);
            if (productAttribute == null)
            {
                return NotFound();
            }
            ViewData["Productid"] = new SelectList(_context.Products, "Id", "Details", productAttribute.Productid);
            return View(productAttribute);
        }

        // POST: ProductAttributes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAttr(decimal id, [Bind("Id,Name,Describtion,Productid")] ProductAttribute productAttribute)
        {
            if (id != productAttribute.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productAttribute);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Productid"] = new SelectList(_context.Products, "Id", "Details", productAttribute.Productid);
            return View(productAttribute);
        }

        // POST: ProductAttributes/Delete/5
        [HttpPost, ActionName("DeleteAttr")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAttr(decimal id)
        {
            var productAttribute = await _context.ProductAttributes.FindAsync(id);
            _context.ProductAttributes.Remove(productAttribute);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}
