﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WaterCompanyWeb.Data;
using WaterCompanyWeb.Data.Entities;
using WaterCompanyWeb.Helpers;
using WaterCompanyWeb.Models;
using static System.Net.Mime.MediaTypeNames;

namespace WaterCompanyWeb.Controllers
{
    public class StaffsController : Controller
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IUserHelper _userHelper;
        private readonly IImageHelper _imageHelper;
        private readonly DataContext _context;
        private readonly IConverterHelper _converterHelper;
        private readonly UserManager<User> _userManager;

        public StaffsController(IStaffRepository staffRepository,
            IUserHelper userHelper,
            IImageHelper imageHelper,
            DataContext context,
            IConverterHelper converterHelper,
            UserManager<User> userManager)
        {
            _staffRepository = staffRepository;
            _userHelper = userHelper;
            _imageHelper = imageHelper;
            _context = context;
            _converterHelper = converterHelper;
            _userManager = userManager;
        }

        // GET: Staffs
        public async Task<IActionResult> Index()
        {
            return View(_staffRepository.GetAllWithUsers());
        }

        // GET: Staffs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("StaffNotFound");
            }

            var staff = await _staffRepository.GetByIdAsync(id.Value);
            if (staff == null)
            {
                return new NotFoundViewResult("StaffNotFound");
            }

            return View(staff);
        }

        // GET: Staffs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("StaffNotFound");
            }

            var staff = await _staffRepository.GetByIdAsync(id.Value);
            if (staff == null)
            {
                return new NotFoundViewResult("StaffNotFound");
            }
            var model = _converterHelper.ToStaffViewModel(staff);
            return View(model);
        }

        // POST: Staffs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StaffViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var path = model.ImageUrl;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        path = await _imageHelper.UploadImageAsync(model.ImageFile, "staffs");
                    }

                    var staff = _converterHelper.ToStaff(model, path, false);

                    staff.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                    await _staffRepository.UpdateAsync(staff);


                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _staffRepository.ExistAsync(model.Id))
                    {
                        new NotFoundViewResult("StaffNotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Staffs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("StaffNotFound");
            }

            var staff = await _staffRepository.GetByIdAsync(id.Value);
            if (staff == null)
            {
                return new NotFoundViewResult("StaffNotFound");
            }

            return View(staff);
        }

        // POST: Staffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var staff = await _staffRepository.GetByIdAsync(id);
            await _staffRepository.DeleteAsync(staff);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult StaffNotFound()
        {
            return View();
        }

        
    }
}
