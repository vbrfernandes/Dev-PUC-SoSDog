using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SosDog.Models;

namespace Dev_PUC_SoSDog.Controllers
{
    public class FavoritosController : Controller
    {
        private readonly AppDbContext _context;

        public FavoritosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Favoritoes
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Favoritos.Include(f => f.Ocorrencia).Include(f => f.Usuario);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Favoritoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favorito = await _context.Favoritos
                .Include(f => f.Ocorrencia)
                .Include(f => f.Usuario)
                .FirstOrDefaultAsync(m => m.ID_Favorito == id);
            if (favorito == null)
            {
                return NotFound();
            }

            return View(favorito);
        }

        // GET: Favoritoes/Create
        public IActionResult Create()
        {
            ViewData["ID_Ocorrencia"] = new SelectList(_context.Ocorrencias, "ID_Ocorrencia", "Descricao");
            ViewData["ID_Usuario"] = new SelectList(_context.Usuarios, "ID_Usuario", "Email");
            return View();
        }

        // POST: Favoritoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID_Favorito,ID_Usuario,ID_Ocorrencia")] Favorito favorito)
        {
            if (ModelState.IsValid)
            {
                _context.Add(favorito);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ID_Ocorrencia"] = new SelectList(_context.Ocorrencias, "ID_Ocorrencia", "Descricao", favorito.ID_Ocorrencia);
            ViewData["ID_Usuario"] = new SelectList(_context.Usuarios, "ID_Usuario", "Email", favorito.ID_Usuario);
            return View(favorito);
        }

        // GET: Favoritoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favorito = await _context.Favoritos.FindAsync(id);
            if (favorito == null)
            {
                return NotFound();
            }
            ViewData["ID_Ocorrencia"] = new SelectList(_context.Ocorrencias, "ID_Ocorrencia", "Descricao", favorito.ID_Ocorrencia);
            ViewData["ID_Usuario"] = new SelectList(_context.Usuarios, "ID_Usuario", "Email", favorito.ID_Usuario);
            return View(favorito);
        }

        // POST: Favoritoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID_Favorito,ID_Usuario,ID_Ocorrencia")] Favorito favorito)
        {
            if (id != favorito.ID_Favorito)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(favorito);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FavoritoExists(favorito.ID_Favorito))
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
            ViewData["ID_Ocorrencia"] = new SelectList(_context.Ocorrencias, "ID_Ocorrencia", "Descricao", favorito.ID_Ocorrencia);
            ViewData["ID_Usuario"] = new SelectList(_context.Usuarios, "ID_Usuario", "Email", favorito.ID_Usuario);
            return View(favorito);
        }

        // GET: Favoritoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favorito = await _context.Favoritos
                .Include(f => f.Ocorrencia)
                .Include(f => f.Usuario)
                .FirstOrDefaultAsync(m => m.ID_Favorito == id);
            if (favorito == null)
            {
                return NotFound();
            }

            return View(favorito);
        }

        // POST: Favoritoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var favorito = await _context.Favoritos.FindAsync(id);
            if (favorito != null)
            {
                _context.Favoritos.Remove(favorito);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FavoritoExists(int id)
        {
            return _context.Favoritos.Any(e => e.ID_Favorito == id);
        }
    }
}
