using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NPMSTORE.Models;

namespace NPMSTORE.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly ModelContext _context;

        public OrdersController(ModelContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Orders.Include(o => o.Customer).Include(o => o.Payment);
            return View(await modelContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Payment)
                .SingleOrDefaultAsync(o => o.Id == id);

            var products = (from op in await _context.OrderProducts
                          .Where(o=>o.Orderid ==id)
                          .Include(p=>p.Product)
                          .Include(o=>o.Order).ToListAsync()
                         select new OrderProduct{ Product = op.Product ,Quantity = op.Quantity}).ToList();

           var viewModel = new Tuple<Order,IEnumerable<OrderProduct>>(order,products);
           

            if (order == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["Customerid"] = new SelectList(_context.Customers, "Id", "Email");
            ViewData["Paymentid"] = new SelectList(_context.Payments, "Id", "CardNumber");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TotalCost,OrderStat,Paymentid,Customerid")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.CreateAt = DateTime.Now;
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Customerid"] = new SelectList(_context.Customers, "Id", "Email", order.Customerid);
            ViewData["Paymentid"] = new SelectList(_context.Payments, "Id", "CardNumber", order.Paymentid);
            return View(order);
        }



        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["Customerid"] = new SelectList(_context.Customers, "Id", "Email", order.Customerid);
            ViewData["Paymentid"] = new SelectList(_context.Payments, "Id", "ExpiryDate", order.Paymentid);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,TotalCost,OrderStat,Paymentid,Customerid")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    order.CreateAt = DateTime.Now;
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["Customerid"] = new SelectList(_context.Customers, "Id", "Email", order.Customerid);
            ViewData["Paymentid"] = new SelectList(_context.Payments, "Id", "ExpiryDate", order.Paymentid);
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if(id <= 0)
            {
                return NotFound();
            }
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        // {add,delete,edit } products in orders 
        
        public IActionResult AddProducts(int orderid)
        {
            ViewData["Orderid"] = new SelectList(_context.Orders, "Id", "Id",orderid);
            
            // To Get only Products dose not exists in this orders
            var products = (from p in _context.Products.ToList()
                           join op in _context.OrderProducts.ToList() on p.Id equals op.Productid
                           where op.Orderid == orderid
                           select new { p.Id }).ToList();
            ViewData["Productid"] = new SelectList(_context.Products.Where(t=> !products.Select(p=>p.Id).Contains(t.Id)), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProducts([Bind("Quantity,Orderid,Productid")] OrderProduct orderProduct)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderProduct);
                await _context.SaveChangesAsync();

                var orderChange = await _context.Orders.FindAsync(orderProduct.Orderid);

                orderChange.TotalCost = _context.OrderProducts
                    .Include(p => p.Product)
                    .Include(o => o.Order)
                    .Where(p => p.Orderid == orderProduct.Orderid)
                    .Sum(x => x.Quantity * x.Product.Price);

                _context.Orders.Update(orderChange);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Orderid"] = new SelectList(_context.Orders, "Id", "Id", orderProduct.Orderid);
            ViewData["Productid"] = new SelectList(_context.Products, "Id", "Name", orderProduct.Productid);
            return View(orderProduct);
        }

        [HttpGet]
        [Route("Orders/EditProducts/{orderid:int}/{productid:int}")]
        public async Task<IActionResult> EditProducts(decimal? orderid, decimal? productid)
        {
            if (orderid == null || productid == null)
            {
                return NotFound();
            }

            var orderProduct = await _context.OrderProducts.SingleOrDefaultAsync(op=>op.Orderid == orderid && op.Productid == productid);
            if (orderProduct == null)
            {
                return NotFound();
            }
            ViewData["Orderid"] = new SelectList(_context.Orders, "Id", "Id", orderProduct.Orderid);
            ViewData["Productid"] = new SelectList(_context.Products, "Id", "Name", orderProduct.Productid);
            return View(orderProduct);
        }

        // POST: OrderProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Orders/EditProducts/{orderid:int}/{productid:int}")]
        public async Task<IActionResult> EditProducts([Bind("Quantity,Orderid,Productid")] OrderProduct orderProduct)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderProduct);
                    await _context.SaveChangesAsync();

                    // Upadte The Column Total Cost
                    var orderChange = await _context.Orders.FindAsync(orderProduct.Orderid);

                    orderChange.TotalCost = _context.OrderProducts
                        .Include(p => p.Product)
                        .Include(o => o.Order)
                        .Where(p => p.Orderid == orderProduct.Orderid)
                        .Sum(x => x.Quantity * x.Product.Price);

                    _context.Orders.Update(orderChange);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Orderid"] = new SelectList(_context.Orders, "Id", "Id", orderProduct.Orderid);
            ViewData["Productid"] = new SelectList(_context.Products, "Id", "Name", orderProduct.Productid);
            return View(orderProduct);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteProduct(decimal orderid,decimal productid)
        {
            if (orderid <= 0 || productid <=0)
            {
                return NotFound();
            }
            // Delete The Products In Orders
            var order = await _context.OrderProducts.Where(p => p.Orderid == orderid && p.Productid == productid).FirstOrDefaultAsync();
            _context.OrderProducts.Remove(order);
            await _context.SaveChangesAsync();


            // Upadte The Column Total Cost
            var orderChange = await _context.Orders.FindAsync(orderid);

            orderChange.TotalCost = _context.OrderProducts
                .Include(p => p.Product)
                .Include(o => o.Order)
                .Where(p => p.Orderid == orderid)
                .Sum(x => x.Quantity * x.Product.Price);

            _context.Orders.Update(orderChange);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(decimal id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }

        
}
}
