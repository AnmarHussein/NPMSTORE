using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NPMSTORE.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NPMSTORE.GenericClass;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Net.Mail;

namespace NPMSTORE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ModelContext _context;
        public HomeVM _home { get; set; }
        public HomeController(ILogger<HomeController> logger,ModelContext context)
        {
            _logger = logger;
            _context = context;
            _home = new HomeVM();
            
        }
        
        public async Task<IActionResult> Index()
        {
            
            //Get Data To Home Page
            var testimonilas = await _context.Testimonials.Where(t=>t.Approved == 1).Include(c=>c.Customer).OrderByDescending(t=>t.Id).Take(3).ToListAsync();
            var stors = await _context.Stors.OrderByDescending(s => s.Id).Take(3).ToListAsync();
            var productsLast =await _context.Products.OrderByDescending(s => s.CreateAt).Take(8).ToListAsync();
            var productsBestSaels = await _context.Products
                .Select(p=>new Product{
                    Seales= p.Price*p.Seales, // Return Total Seales
                    Id = p.Id,
                    Name =p.Name,
                    Image =p.Image,
                    Price=p.Price,  
                }).OrderByDescending(p=>p.Seales).Take(8).ToListAsync();


            _home = (from hp in _context.HomePages.Take(1).ToList()
                     join h in _context.Headers.Take(1).ToList() on hp.Id equals h.HomeId
                     join o in _context.Offers.Take(1).ToList() on hp.Id equals o.HomeId
                     join s in _context.Sidebars.Take(1).ToList() on hp.Id equals s.HomeId
                     join ab in _context.AboutUs.Take(1).ToList() on hp.Id equals ab.HomeId
                     where hp.Id == 1
                     select new HomeVM
                     {
                         home = hp,
                         header = h,
                         caption = _context.Captions.Where(c => c.HomeId == 1).Take(3).ToList(),
                         offer = o,
                         sidebar = s,
                         links = _context.Links.Where(c => c.HomeId == 1).Take(4).ToList(),
                         aboutU = ab
                     }).FirstOrDefault();

            var viewModel = Tuple.Create<IEnumerable<Stor>, IEnumerable<Product>, IEnumerable<Product>, IEnumerable<Testimonial>,HomeVM>(stors, productsLast, productsBestSaels, testimonilas,_home);

            return View(viewModel);
        }


        public IActionResult AboutUs()
        {
            var viewModel = _context.AboutUs.Where(c => c.HomeId == 1).FirstOrDefault();
            return View(viewModel);
        }




        #nullable enable
        public async Task<IActionResult> ShowStore(string? KeyName)
        {
            var stor = await _context.Stors.ToListAsync();
            var category = await _context.Categories.ToListAsync();
            var products = await _context.Products.ToListAsync();
            var storecategory = await _context.StoreCategories.ToListAsync();

            var categoryVM = (from s in stor
                              join sc in storecategory on s.Id equals sc.Storeid
                              join c in category on sc.Categoryid equals c.Id
                              group c by new { s } into g
                              select new CategoryVM
                              {
                                  stor = g.Key.s,
                                  category = g.ToList(),
                              }).ToList(); // Select name Category in  all store

            if (categoryVM == null)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                if(!String.IsNullOrEmpty(KeyName))
                {
                    return View(categoryVM.Where(s => s.stor.Name.ToLower().Contains(KeyName.ToLower())).ToList());
                }
                return View(categoryVM);
            }
                
            
        }
        #nullable enable
        public IActionResult allProducts(string? keySearch,decimal? storeid,decimal? Categoryid)
        {

            var stor = _context.Stors.ToList();
            var category = _context.Categories.ToList();
            var storecategory = _context.StoreCategories.ToList();
            var products =_context.Products.ToList();

            ViewData["categoryid"] = new SelectList(_context.Categories.ToList(), "Id", "Name");


            var viewModel = (from s in stor
                             join sc in storecategory on s.Id equals sc.Storeid
                             join c in category on sc.Categoryid equals c.Id 
                             join p in products on sc.Productsid equals p.Id
                             select new StoreCategoryVM
                             {
                                 store = s,
                                 category = c,
                                 product = p
                             }).ToList();
            if (!String.IsNullOrEmpty(keySearch))
            {
                viewModel = viewModel.Where(x=>x.store.Name.ToLower().Contains(keySearch.ToLower()) || x.category.Name.Contains(keySearch)).ToList();
            }
            else if(storeid != null)
            {
                viewModel = viewModel.Where(s=>s.store.Id == storeid).ToList(); 
            }
            else if(Categoryid != null)
            {
                viewModel = viewModel.Where(s => s.category.Id == Categoryid).ToList();
            }
            ViewBag.Message = TempData["Message"];

            return View(viewModel);
        }
        public IActionResult DeitaelsProducts(decimal id)
        {
            var stor = _context.Stors.ToList();
            var category = _context.Categories.ToList();
            var storecategory = _context.StoreCategories.ToList();
            var products = _context.Products.Include(attr => attr.ProductAttributes).ToList();

            var viewModel = (from s in stor
                             join sc in storecategory on s.Id equals sc.Storeid
                             join c in category on sc.Categoryid equals c.Id
                             join p in products on sc.Productsid equals p.Id
                             where p.Id == id
                             select new StoreCategoryVM
                             {
                                 store = s,
                                 category = c,
                                 product = p
                             }).First();
            
            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public IActionResult addToCart(decimal id,decimal quantity)
        {

            if (SessionHelper.GetObjectFromJson<List<CartVm>>(HttpContext.Session, "cart") == null)
            {
                List<CartVm> cart = new List<CartVm>();
                var prod = _context.Products.FirstOrDefault(x => x.Id == id);
                cart.Add(new CartVm { product = prod, Quantity = quantity });
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            else
            {
                List<CartVm> cart = SessionHelper.GetObjectFromJson<List<CartVm>>(HttpContext.Session, "cart");
                int index = isExistProductInCart(id);
                if(index != -1) 
                {
                    cart[index].Quantity +=1;
                }
                else
                {
                    cart.Add(new CartVm { product = _context.Products.Find(id), Quantity = quantity });
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            TempData["Message"] = "The Item Add To Cart ! If You Want Change Quantity Go To Shoping Cart ";
            return RedirectToAction("allProducts");
        }

        private int isExistProductInCart(decimal id)
        {
            List<CartVm> cart = SessionHelper.GetObjectFromJson<List<CartVm>>(HttpContext.Session, "cart");
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].product.Id.Equals(id))
                {
                    return i;
                }
            }
            return -1;
        }

        public IActionResult ShoppingCart()
        {
            if(SessionHelper.GetObjectFromJson<List<CartVm>>(HttpContext.Session, "cart") == null)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                List<CartVm> cart = SessionHelper.GetObjectFromJson<List<CartVm>>(HttpContext.Session, "cart");
                ViewBag.Message = TempData["Message"] ?? "";
                return View(cart);
            }
        }

        [HttpPost]
        public IActionResult UpdateQuantityCart(decimal id, decimal quantity)
        {
            List<CartVm> cart = SessionHelper.GetObjectFromJson<List<CartVm>>(HttpContext.Session, "cart");
            cart.Where(p => p.product.Id == id).FirstOrDefault().Quantity= quantity;
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction(nameof(ShoppingCart));
        }
        [HttpPost]
        public IActionResult deleteFromCart(decimal id)
        {
            List<CartVm> cart = SessionHelper.GetObjectFromJson<List<CartVm>>(HttpContext.Session, "cart");
            cart.Remove(cart.Where(p => p.product.Id == id).FirstOrDefault());
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction(nameof(ShoppingCart));
        }

        [HttpPost]
        public async Task<IActionResult> AddOrder([Bind("Id,CardName,CardNumber,SecurityCode,ExpiryDate,Amount")]Payment payment)
        {
            if (ModelState.IsValid)
            {
                var paymntsValid = _context.InfoCards.Where(p=>p.CardNumber ==payment.CardNumber).FirstOrDefault();
                if (paymntsValid.Balanc < payment.Amount)
                {

                    TempData["Message"] = "Money Is Not Enough To Complete The Purchase";
                    return RedirectToAction(nameof(ShoppingCart));
                }
                //Add Payments 
                payment.CreateAt = DateTime.Now;
                payment.Customerid = Int32.Parse(User.FindFirst("User_id").Value);
                _context.Add(payment);
                await _context.SaveChangesAsync();

                //Add Order
                Order order = new Order();
                order.OrderStat = 1;
                order.Customerid = Int32.Parse(User.FindFirst("User_id").Value);
                order.CreateAt = DateTime.Now;
                order.Paymentid = payment.Id;
                order.TotalCost = payment.Amount;

                _context.Add(order);
                await _context.SaveChangesAsync();

                List<CartVm> cart = SessionHelper.GetObjectFromJson<List<CartVm>>(HttpContext.Session, "cart");
                foreach(var item in cart)
                {
                    OrderProduct orderProduct = new OrderProduct();
                    Product editProduct = new Product();

                    // Add Product TO OrderPrdocsut
                    orderProduct.Productid = item.product.Id;
                    orderProduct.Orderid = order.Id;
                    orderProduct.Quantity = item.Quantity;

                    _context.Add(orderProduct);
                    await _context.SaveChangesAsync();


                    //Add Seales
                    var product = _context.Products.Find(item.product.Id);
                    product.Seales += item.Quantity;
                    _context.Update(product);
                    await _context.SaveChangesAsync();

                }


                HttpContext.Session.Remove("cart");

                TempData["OrderSuccess"] = "The Order Is Compleate !";
                return RedirectToAction("OrdersDetails","Users", new {id = order.Id});
            }
            
            return View(payment);
        }


        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
