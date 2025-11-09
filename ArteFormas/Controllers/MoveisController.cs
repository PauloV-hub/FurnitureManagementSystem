using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ArteFormas.Data;
using ArteFormas.Models;

namespace ArteFormas.Controllers
{
    public class MoveisController : Controller
    {
        private readonly ArteFormasContext _context;

        public MoveisController(ArteFormasContext context)
        {
            _context = context;
        }

        // GET: Movels
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movel.ToListAsync());
        }

        // GET: MoveisById
        [HttpGet]
        public async Task<IActionResult> GetMovelById(int id)
        {
            var movel = await _context.Movel.FindAsync(id);
            if (movel == null)
            {
                return NotFound();
            }

            return Json(new
            {
                id = movel.Id,
                nome = movel.Nome,
                preco = movel.Preco
            });
        }

        // GET: Movels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movel = await _context.Movel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movel == null)
            {
                return NotFound();
            }

            return View(movel);
        }

        // GET: Movels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Descricao,Preco,Material")] Movel movel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movel);
        }

        // GET: Movels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movel = await _context.Movel.FindAsync(id);
            if (movel == null)
            {
                return NotFound();
            }
            return View(movel);
        }

        // POST: Movels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Descricao,Preco,Material")] Movel movel)
        {
            if (id != movel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovelExists(movel.Id))
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
            return View(movel);
        }

        // GET: Movels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movel = await _context.Movel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movel == null)
            {
                return NotFound();
            }

            return View(movel);
        }

        // POST: Movels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movel = await _context.Movel.FindAsync(id);
            if (movel != null)
            {
                _context.Movel.Remove(movel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovelExists(int id)
        {
            return _context.Movel.Any(e => e.Id == id);
        }
    }
}
