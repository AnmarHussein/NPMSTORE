using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NPMSTORE.GenericClass;
using NPMSTORE.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NPMSTORE.Controllers
{
    [Authorize(Roles="Admin")]
    public class AdminController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public AdminController(ModelContext context, IWebHostEnvironment webHostEnviroment)
        {
            _context = context;
            _webHostEnviroment = webHostEnviroment;
        }


        public async Task<IActionResult> Index()
        {
            //Data User 

           TempData["user_id"] = HttpContext.Session.GetInt32("user_id") ?? null;
           TempData["UserImage"] = await _context.Customers.Where(x => x.Id == HttpContext.Session.GetInt32("user_id")).Select(x=>x.Image).FirstOrDefaultAsync();
            
            // Get All Payments in Data To 
            ViewBag.amountLastMonth = await _context.Orders.Where(p => p.CreateAt >= DateTime.Now.AddMonths(-1)).SumAsync(p => p.TotalCost);
            ViewBag.amountLastYear = await _context.Orders.Where(p => p.CreateAt >= DateTime.Now.AddYears(-1)).SumAsync(p => p.TotalCost);


            //Get All Products And Stors And Cusetomers
            ViewBag.totalCustomers = await _context.Customers.CountAsync();
            ViewBag.totalStors = await _context.Stors.CountAsync();
            ViewBag.totalProducts = await _context.Products.CountAsync();


            //Start The Chart Code

            // Chart statistic
            var storecategory = await _context.StoreCategories.ToListAsync();
            var stor = await _context.Stors.ToListAsync();
            var products = await _context.Products.ToListAsync();
            var category = await _context.Categories.ToListAsync();

            //Join TO feth Data Stor, Each Products contain
            var ChartStore = (
                             from s in stor
                             join sc in storecategory on s.Id equals sc.Storeid
                             group s by new { s.Name, s.Id } into g
                             select new chartDonantVM
                             {
                                storeName = g.Key.Name,
                                totalCost = (decimal)g.Select(s => s.StoreCategories.Where(s=>s.Productsid != null).Sum(p => p.Products.Price * p.Products.Seales)).First(),
                                countProduct = g.Select(g => g.StoreCategories.Select(p => p.Products.Id)).Count(),
                                countCategory = (from c in category
                                                join sc in storecategory on c.Id equals sc.Categoryid
                                                where sc.Storeid == g.Key.Id
                                                group sc by c.Id into gc
                                                select new { gc.Key }).Count(),
                             }).ToList();
            var testimonials = await _context.Testimonials.Where(t=>t.Approved == 0).Include(c=>c.Customer).Take(5).ToListAsync();
            var viewModel = Tuple.Create<IEnumerable<chartDonantVM>,IEnumerable<Testimonial>>(ChartStore, testimonials);
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Search()
        { 
            var viewModel = await _context.Orders.Where(x=>x.OrderStat == 1).Include(o=>o.Customer).Include(p=>p.OrderProducts).ThenInclude(p=>p.Product).ToListAsync();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Search(DateTime? startDate , DateTime? endDate)
        {
            var viewModel = await _context.Orders.Include(o => o.Customer).Include(p => p.OrderProducts).ThenInclude(p => p.Product).ToListAsync();

            if (startDate == null || endDate == null)  
            {
                return View(viewModel);
            }

            if(startDate == null && endDate != null)
            {
                var resualt = viewModel.Where(s=>s.CreateAt.Date == endDate ).ToList();
                return View(resualt);
            }
            else if(startDate != null && endDate == null)
            {
                var resualt = viewModel.Where(s => s.CreateAt.Date == startDate).ToList();
                return View(resualt);
            }
            else
            {
                var resualt = viewModel.Where(s => s.CreateAt.Date >= startDate && s.CreateAt <= endDate).ToList();
                return View(resualt);
            }

        }


        [HttpGet]
        public async Task<IActionResult> reports()
        {
            var viewModel = await _context.Orders.Where(x => x.OrderStat == 1).Include(o => o.Customer).Include(p => p.OrderProducts).ThenInclude(p => p.Product).ToListAsync();
            ViewBag.TotalQy = viewModel.Sum(p => p.OrderProducts.Sum(p => p.Quantity));
            ViewBag.TotalPrice = viewModel.Sum(p => p.OrderProducts.Sum(p => p.Quantity * p.Product.Price));
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> reports(DateTime? startDate, DateTime? endDate)
        {
            var viewModel = await _context.Orders.Include(o => o.Customer).Include(p => p.OrderProducts).ThenInclude(p => p.Product).ToListAsync();
            ViewBag.TotalQy = viewModel.Sum(p => p.OrderProducts.Sum(p => p.Quantity));
            ViewBag.TotalPrice = viewModel.Sum(p => p.OrderProducts.Sum(p => p.Quantity * p.Product.Price));

            if (startDate == null || endDate == null)
            {
                return View(viewModel);
            }

            if (startDate == null && endDate != null)
            {
                ViewBag.TotalQy = viewModel.Where(s => s.CreateAt.Date == endDate).Sum(p => p.OrderProducts.Sum(p => p.Quantity));
                ViewBag.TotalPrice = viewModel.Where(s => s.CreateAt.Date == endDate).Sum(p => p.OrderProducts.Sum(p => p.Quantity * p.Product.Price));
                
                var resualt = viewModel.Where(s => s.CreateAt.Date == endDate).ToList();

                return View(resualt);
            }
            else if (startDate != null && endDate == null)
            {
                ViewBag.TotalQy = viewModel.Where(s => s.CreateAt.Date == startDate).Sum(p => p.OrderProducts.Sum(p => p.Quantity));
                ViewBag.TotalPrice = viewModel.Where(s => s.CreateAt.Date == startDate).Sum(p => p.OrderProducts.Sum(p => p.Quantity * p.Product.Price));

                var resualt = viewModel.Where(s => s.CreateAt.Date == startDate).ToList();
                return View(resualt);
            }
            else
            {
                ViewBag.TotalQy = viewModel.Where(s => s.CreateAt.Date >= startDate && s.CreateAt <= endDate).Sum(p => p.OrderProducts.Sum(p => p.Quantity));
                ViewBag.TotalPrice = viewModel.Where(s => s.CreateAt.Date >= startDate && s.CreateAt <= endDate).Sum(p => p.OrderProducts.Sum(p => p.Quantity * p.Product.Price));

                var resualt = viewModel.Where(s => s.CreateAt.Date >= startDate && s.CreateAt <= endDate).ToList();
                return View(resualt);
            }

        }


        // Testimonials

        public async Task<IActionResult> Testimonials()
        {
            var modelContext = _context.Testimonials.Include(t => t.Customer);
            return View(await modelContext.ToListAsync());
        }
        public async Task<IActionResult> ApproveOrRejectTestimonial(decimal? testiId,decimal approved)
        {
            if (testiId == null)
            {
                return NotFound();
            }

            var testimonial = await _context.Testimonials.FindAsync(testiId);
            
            testimonial.Approved = (approved == 0) ? 1 : 0 ;
            _context.Testimonials.Update(testimonial);
            await _context.SaveChangesAsync();
            return RedirectToAction("Testimonials");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTestimonials(decimal? testiId)
        {
            if (testiId == null)
            {
                return NotFound();
            }

            var testimonial = await _context.Testimonials.FindAsync(testiId);
            _context.Testimonials.Remove(testimonial);
            await _context.SaveChangesAsync();
            return RedirectToAction("Testimonials");
        }



        // GET: ManageHome/Create
        public IActionResult EditHomePage()
        {
            var viewModel = (from hp in _context.HomePages.Take(1).ToList()
                             join h in _context.Headers.Take(1).ToList() on hp.Id equals h.HomeId
                             join o in _context.Offers.Take(1).ToList() on hp.Id equals o.HomeId
                             join s in _context.Sidebars.Take(1).ToList() on hp.Id equals s.HomeId
                             where hp.Id == 1
                             select new HomeVM
                             {
                                 home = hp,
                                 header = h,
                                 caption = _context.Captions.Where(c => c.HomeId == 1).Take(3).ToList(),
                                 offer = o,
                                 sidebar = s,
                                 links = _context.Links.Where(c => c.HomeId == 1).Take(4).ToList()
                             }
                             ).FirstOrDefault();
            return View(viewModel);
        }

        // POST: ManageHome/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditHomePage(HomeVM homePage)
        {
            if (ModelState.IsValid)
            {
                
                if (homePage.header != null)
                {
                    if (homePage.header.ImageFile != null)
                    {
                        string wwwRootPath = _webHostEnviroment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + "_" +
                        homePage.header.ImageFile.FileName;
                        string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await homePage.header.ImageFile.CopyToAsync(fileStream);
                        }
                        homePage.header.Logo = fileName;
                    }
                    _context.Update(homePage.header);
                    await _context.SaveChangesAsync();
                }
                else if(homePage.sidebar != null)
                {
                    if (homePage.sidebar.ImageFile != null)
                    {
                        string wwwRootPath = _webHostEnviroment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + "_" +
                        homePage.sidebar.ImageFile.FileName;
                        string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await homePage.sidebar.ImageFile.CopyToAsync(fileStream);
                        }
                        homePage.sidebar.Image = fileName;
                    }
                    _context.Update(homePage.sidebar);
                    await _context.SaveChangesAsync();
                }
                else if (homePage.offer != null)
                {
                    if (homePage.offer.ImageFile != null)
                    {
                        string wwwRootPath = _webHostEnviroment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + "_" +
                        homePage.offer.ImageFile.FileName;
                        string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await homePage.offer.ImageFile.CopyToAsync(fileStream);
                        }
                        homePage.offer.Image = fileName;
                    }
                    _context.Update(homePage.offer);
                    await _context.SaveChangesAsync();
                }
                else if(homePage.caption != null)
                {
                    _context.UpdateRange(homePage.caption);
                    await _context.SaveChangesAsync();
                }
                else if (homePage.links != null)
                {
                    _context.UpdateRange(homePage.links);
                    await _context.SaveChangesAsync();
                }


                return RedirectToAction(nameof(Index));
            }
            return View(homePage);
        }


        public IActionResult EditAboutUs()
        {
            var viewModel = (from hp in _context.HomePages.Take(1).ToList()
                             join ab in _context.AboutUs.Take(1).ToList() on hp.Id equals ab.HomeId
                             where hp.Id == 1
                             select new HomeVM
                             {
                                 home = hp,
                                 aboutU=ab,
                             }
                             ).FirstOrDefault();
            return View(viewModel);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAboutUs(HomeVM homePage)
        {
            if (ModelState.IsValid)
            {
                if (homePage.aboutU != null)
                {
                    _context.Update(homePage.aboutU);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(homePage);
        }


    }
}
