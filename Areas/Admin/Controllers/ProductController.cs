using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebsiteBanHang_kirei_4.Models;

namespace WebsiteBanHang_kirei_4.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductController : Controller
    {
        // GET: Admin/Product
        public IActionResult Index()
        {
            return View();
        }

        // GET: Admin/Product/Details/5
        public IActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/Product/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Product/Edit/5
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            return View();
        }

        // POST: Admin/Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Product/Delete/5
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            return View();
        }

        // POST: Admin/Product/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                // TODO: Add delete logic here
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
