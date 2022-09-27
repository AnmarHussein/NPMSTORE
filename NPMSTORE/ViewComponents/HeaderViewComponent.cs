using Microsoft.AspNetCore.Mvc;
using NPMSTORE.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NPMSTORE.GenericClass
{
    public class HeaderViewComponent: ViewComponent
    {
        private readonly ModelContext _context;

        public HeaderViewComponent(ModelContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            var header = _context.Headers.FirstOrDefault(h=>h.HomeId==1);
            int countCatr = 0;
            if (SessionHelper.GetObjectFromJson<List<CartVm>>(HttpContext.Session, "cart") != null)
            {
                countCatr = SessionHelper.GetObjectFromJson<List<CartVm>>(HttpContext.Session, "cart").Count();

            }
            var viewModel = Tuple.Create<Header, int>(header, countCatr);
            return View(viewModel);
        }
    }
}
