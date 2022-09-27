using Microsoft.AspNetCore.Mvc;
using NPMSTORE.Models;
using System.Collections.Generic;
using System.Linq;

namespace NPMSTORE.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly ModelContext _context;
        public FooterViewComponent(ModelContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            var homeVm = (from hp in _context.HomePages.Take(1).ToList()
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
            
            return View(homeVm);
        }
    }
}
