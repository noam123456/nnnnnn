﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shope.Models;

namespace Shope.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ShopeContext _context;

        public Cart ThisCart { get;  set; }

        public ProductsController(ShopeContext context)
        {
            _context = context;
            
        }


        // GET: Products
        public async Task<IActionResult> Index(string name, string price)
        {
            var filter = from m in _context.Product select m;
            if (!string.IsNullOrEmpty(name))
            {
                filter = filter.Where(s => s.TypeName.Contains(name));
            }
            if (!string.IsNullOrEmpty(price))
            {
                var p = int.Parse(price);
                filter = filter.Where(t => t.Price > p);
            }
            return View(await filter.ToListAsync());
            // return View(await _context.Mesima1.ToListAsync());
        }

        public async Task<IActionResult> ProductHome(string name, string price)
        {
            var filter = from m in _context.Product select m;
            if (!string.IsNullOrEmpty(name))
            {
                filter = filter.Where(s => s.TypeName.Contains(name));
            }
            if (!string.IsNullOrEmpty(price))
            {
                var p = int.Parse(price);
                filter = filter.Where(t => t.Price > p);
            }
            return View(await filter.ToListAsync());
            // return View(await _context.Mesima1.ToListAsync());
        }

        public IActionResult cart()
        {
            return View();
        }

        public IActionResult AddToCart(int productid , int unit)
        {
            Product p = _context.Product.Where(x => x.Id == productid).FirstOrDefault();

            for(int i=0; i < unit; i++) {
                if (p.Unit > 0)
                {
                    Global.CurrentCart.Products.Add(p);
                    Global.CurrentCart.TotalAmount += p.Price;
                }
            }
            return View("cart");
        }

        public IActionResult Payment()
        {
            if(Global.sessionID == 0)
            {
                return RedirectToAction("login", "Customers");
            }
            return View();

        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //var  from pro in _context.Product
            //            where pro.Id ==  id
            //            select pro;

            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .SingleOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        
        OrderAndProduct OrdAndPro;
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAuto()
        {
            Global.ord = new Order();
            if (ModelState.IsValid)
            {
                _context.Add(Global.ord);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customer, "Id", "Id");
            if (ModelState.IsValid)
            {
                int size = Global.CurrentCart.Products.Count;
                for (int i = 0; i <size; i++)
                {
                    OrdAndPro = new OrderAndProduct();
                    _context.Add(OrdAndPro);
                    await _context.SaveChangesAsync();
                    //return RedirectToAction(nameof(Index));
                    var result = (from p in _context.Product
                                  where p.Id == Global.CurrentCart.Products.FirstOrDefault().Id
                                  select p).FirstOrDefault();
                    result.Unit -= 1;
                    _context.SaveChanges();
                    if (!(i == size - 1)){
                        Global.CurrentCart.Products.RemoveAt(i);
                    }
                   
                } 
                Global.CurrentCart = new Cart();
                

            }
            return RedirectToAction("ProductHome", "Products");
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TypeName,Price,Color,Weight,Unit, Image,OrderId")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.SingleOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TypeName,Price,Color,Weight,Unit, Image,OrderId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .SingleOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.SingleOrDefaultAsync(m => m.Id == id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
    }
}
