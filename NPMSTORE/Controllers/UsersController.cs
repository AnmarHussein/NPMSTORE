using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPMSTORE.GenericClass;
using NPMSTORE.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace NPMSTORE.Controllers
{
    [Authorize(Roles = "User")]
    public class UsersController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;
        public UsersController(ModelContext context, IWebHostEnvironment webHostEnviroment)
        {
            _context = context;
            _webHostEnviroment = webHostEnviroment;
        }
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> ShowProfile()
        {
            var customer =await _context.Customers.Include(c=>c.CustomersLogin).SingleOrDefaultAsync(c=>c.Id == Int32.Parse(User.FindFirst("User_id").Value));
            
            return View(customer);
        }

        public async Task<IActionResult> EditProfile()
        {
            var customer = await _context.Customers.Include(c => c.CustomersLogin).SingleOrDefaultAsync(i => i.Id == Int32.Parse(User.FindFirst("User_id").Value));
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile([Bind("Id,FullName,Image,Gender,ImageFile,Bdate,Email,Address1,Address2,PhoneNumber,CustomersLogin")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                if(customer.Id != Int32.Parse(User.FindFirst("User_id").Value))
                {
                    return RedirectToAction("EditProfile");
                }
                try
                {
                    if (customer.ImageFile != null)
                    {
                        string wwwRootPath = _webHostEnviroment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + "_" +
                        customer.ImageFile.FileName;
                        string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await customer.ImageFile.CopyToAsync(fileStream);
                        }
                        customer.Image = fileName;
                    }
                    _context.Update(customer);

                    CustomersLogin user = new CustomersLogin();
                    user = customer.CustomersLogin;
                    user.Customerid = customer.Id;

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (customer ==null)
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
            return View(customer);
        }


       
        public async Task<IActionResult> ShowOrders()
        {
            var orders = await _context.Orders
                .Where(o=>o.Customerid == Int32.Parse(User.FindFirst("User_id").Value))
                .Include(c=>c.Payment)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> OrdersDetails(decimal id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Payment)
                .SingleOrDefaultAsync(o => o.Id == id && o.Customerid == Int32.Parse(User.FindFirst("User_id").Value));
            if(order == null)
            {
                return RedirectToAction(nameof(ShowOrders));
            }
            var products = (from op in await _context.OrderProducts
                          .Where(o => o.Orderid == id && o.Order.Customerid == Int32.Parse(User.FindFirst("User_id").Value))
                          .Include(p => p.Product)
                          .Include(o => o.Order).ToListAsync()
                            select new OrderProduct { Product = op.Product, Quantity = op.Quantity }).ToList();
            
            var viewModel = new Tuple<Order, IEnumerable<OrderProduct>>(order, products);

            
            return View(viewModel);
        }

        public async Task<IActionResult> Testimonials()
        {
            var modelView = _context.Testimonials.Where(t=>t.Customerid == Int32.Parse(User.FindFirst("User_id").Value)).Include(t => t.Customer);
            return View(await modelView.ToListAsync());
        }

        [HttpGet]
        public IActionResult CreateTestimonials()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTestimonials(Testimonial testimonial)
        {
            if (ModelState.IsValid)
            {
                testimonial.Customerid = Int32.Parse(User.FindFirst("User_id").Value);
                testimonial.Approved = 0;
                _context.Add(testimonial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Testimonials));
            }
            return View(testimonial);
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


        [HttpGet]
        public async Task<IActionResult> reports()
        {
            var viewModel = await _context.Orders
                .Where(c => c.OrderStat == 1 &&  c.Customerid == Int32.Parse(User.FindFirst("User_id").Value))
                .Include(o => o.Customer)
                .Include(p => p.OrderProducts)
                .ThenInclude(p => p.Product)
                .ToListAsync();

            ViewBag.TotalQy = viewModel.Sum(p => p.OrderProducts.Sum(p => p.Quantity));
            ViewBag.TotalPrice = viewModel.Sum(p => p.OrderProducts.Sum(p => p.Quantity * p.Product.Price));
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> reports(DateTime? startDate, DateTime? endDate)
        {
            var viewModel = await _context.Orders
                            .Where(c=>c.Customerid == Int32.Parse(User.FindFirst("User_id").Value))
                            .Include(o => o.Customer)
                            .Include(p => p.OrderProducts)
                            .ThenInclude(p => p.Product)
                            .ToListAsync();

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



        //Send Emails
        private bool SendEmail(string body)
        {
            //var viewHtml =  this.RenderViewAsync("OrdersDetails", viewModel);
            //SendEmail(viewHtml.Trim());


            //Fetching Email Body Text from EmailTemplate File.  
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("newqroma@gmail.com");
            mailMessage.To.Add("anmar.hussein.okour@gmail.com");
            mailMessage.Subject = "Hello Wlcom To Aramo";
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;

            bool senEmail = false;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = new NetworkCredential("newqroma@gmail.com", "cfodqutfkmzlouut");
            smtp.EnableSsl = true;


            try
            {
                smtp.Send(mailMessage);
            }
            catch (SmtpFailedRecipientException ex)
            {
                var x = ex.Message;
                // ex.FailedRecipient and ex.GetBaseException() should give you enough info.
            }
            senEmail = true;
            return senEmail;
        }
    }

}
