using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPMSTORE.Models;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NPMSTORE.Controllers
{
    public class AcountController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public AcountController(ModelContext context , IWebHostEnvironment webHostEnvironment )
        {
            _context = context;
            _webHostEnviroment = webHostEnvironment;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            if(User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index","Admin");
            }
            ViewBag.ValidUser = TempData["ValidUser"];
            return View();
        }

        [HttpPost]
        public async  Task<IActionResult> Login([Bind("UserName,Password")] CustomersLogin user)
        {
            if(string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password))
            {
                return RedirectToAction("Login");
            }

            var customer = await _context.CustomersLogins.Where(u => u.UserName == user.UserName && u.Password == user.Password).Include(c=>c.Customer).FirstOrDefaultAsync();

            if(customer == null)
            {
                TempData["ValidUser"] = "UserName Or Password Is Not Valid !";
                return RedirectToAction("Login");
            }
            
            ClaimsIdentity identity = null;
            if(customer.Roleid == 2)//Admin
            {
                identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, customer.Customer.FullName),
                    new Claim("User_id",customer.Customer.Id.ToString()),
                    new Claim("User_Image",customer.Customer.Image),
                    new Claim(ClaimTypes.Role,"Admin"),

                }, CookieAuthenticationDefaults.AuthenticationScheme) ;

                var principal = new ClaimsPrincipal(identity);

                var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties
                {
                    IsPersistent = false,/*ExpiresUtc = DateTime.UtcNow.AddMinutes(1),*/

                }) ;
                return RedirectToAction("Index", "Admin");
            }

            if (customer.Roleid == 1) //User
            {
                identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, customer.Customer.FullName),
                    new Claim("User_id",customer.Customer.Id.ToString()),
                    new Claim("User_Image",customer.Customer.Image),
                    new Claim(ClaimTypes.Role,"User")
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);
                var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties
                {
                    IsPersistent = false,/*ExpiresUtc = DateTime.UtcNow.AddMinutes(1),*/

                });
                return RedirectToAction("Index", "Home");
            }

            return View();
        }


        [HttpGet]
        public IActionResult register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> register([Bind("Id,FullName,Gender,ImageFile,Bdate,Email,Address1,Address2,PhoneNumber")] Customer customer, string userName, string password,string re_password)
        {
            if(password != re_password)
            {
                return View("Error");
            }
            if (ModelState.IsValid)
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
                _context.Add(customer);
                await _context.SaveChangesAsync();

                // Add userLogin 
                CustomersLogin user = new CustomersLogin();
                user.UserName = userName;
                user.Password = password;
                user.Roleid = 1;
                user.Customerid = customer.Id;
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Login));
            }
            return View(customer);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {

            if (HttpContext.Request.Cookies.Count > 0)
            {
                var siteCookies = HttpContext.Request.Cookies.Where(c => c.Key.Contains(".AspNetCore.") || c.Key.Contains("Microsoft.Authentication") || c.Key.Contains(".data_user"));
                foreach (var cookie in siteCookies)
                {
                    Response.Cookies.Delete(cookie.Key);
                }
            }
            
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        

    }
}
