﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AMS.Data;
using AMS.Models;
using Microsoft.AspNetCore.Authorization;
using AMS.Services;
using Microsoft.Extensions.Logging;

namespace AMS.Controllers
{
    [Authorize]
    public class TicketAssetsController : Controller
    {
        private readonly ILogger<TicketAssetsController> logger;
        private readonly AmsContext _context;
        private readonly IUserService userService;

        public TicketAssetsController(ILogger<TicketAssetsController> logger, AmsContext context, IUserService userService)
        {
            this.logger = logger;
            _context = context;
            this.userService = userService;
        }

        // GET: TicketAssets
        public async Task<IActionResult> Index()
        {
            var amsContext = _context.TicketAsset.Include(t => t.Asset).Include(t => t.Ticket);
            return View(await amsContext.ToListAsync());
        }

        // GET: TicketAssets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketAsset = await _context.TicketAsset
                .Include(t => t.Asset)
                .Include(t => t.Ticket)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketAsset == null)
            {
                return NotFound();
            }

            return View(ticketAsset);
        }

        // GET: TicketAssets/Create
        public async Task<IActionResult> Create()
        {
            await SetViewData();
            return View();
        }

        // POST: TicketAssets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AssetId,TicketId")] TicketAsset ticketAsset)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ticketAsset);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            await SetViewData(ticketAsset);
            return View(ticketAsset);
        }

        private async Task SetViewData(TicketAsset ticketAsset = null)
        {
            ViewData["AssetId"] = await userService.GetAssetsSelectAsync(ticketAsset?.AssetId);
            ViewData["TicketId"] = await userService.GetTicketsSelectAsync(ticketAsset?.TicketId);
        }

        // GET: TicketAssets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketAsset = await _context.TicketAsset.FindAsync(id);
            if (ticketAsset == null)
            {
                return NotFound();
            }
            await SetViewData(ticketAsset);
            return View(ticketAsset);
        }

        // POST: TicketAssets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AssetId,TicketId")] TicketAsset ticketAsset)
        {
            if (id != ticketAsset.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticketAsset);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketAssetExists(ticketAsset.Id))
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
            await SetViewData(ticketAsset);
            return View(ticketAsset);
        }

        // GET: TicketAssets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketAsset = await _context.TicketAsset
                .Include(t => t.Asset)
                .Include(t => t.Ticket)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketAsset == null)
            {
                return NotFound();
            }

            return View(ticketAsset);
        }

        // POST: TicketAssets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticketAsset = await _context.TicketAsset.FindAsync(id);
            _context.TicketAsset.Remove(ticketAsset);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketAssetExists(int id)
        {
            return _context.TicketAsset.Any(e => e.Id == id);
        }
    }
}
