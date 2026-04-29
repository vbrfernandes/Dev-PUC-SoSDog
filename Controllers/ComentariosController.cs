using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SosDog.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Dev_PUC_SoSDog.Controllers
{
    public class ComentariosController : Controller
    {
        private readonly AppDbContext _context;

        public ComentariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Comentarios
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Comentarios.Include(c => c.Ocorrencia).Include(c => c.Usuario);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Comentarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comentario = await _context.Comentarios
                .Include(c => c.Ocorrencia)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.ID_Comentario == id);
            if (comentario == null)
            {
                return NotFound();
            }

            return View(comentario);
        }

        // GET: Comentarios/Create
        public IActionResult Create()
        {
            ViewData["ID_Ocorrencia"] = new SelectList(_context.Ocorrencias, "ID_Ocorrencia", "Descricao");
            ViewData["ID_Usuario"] = new SelectList(_context.Usuarios, "ID_Usuario", "Email");
            return View();
        }

        // POST: Comentarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID_Comentario,Texto,Data_hora,ID_Usuario,ID_Ocorrencia")] Comentario comentario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(comentario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ID_Ocorrencia"] = new SelectList(_context.Ocorrencias, "ID_Ocorrencia", "Descricao", comentario.ID_Ocorrencia);
            ViewData["ID_Usuario"] = new SelectList(_context.Usuarios, "ID_Usuario", "Email", comentario.ID_Usuario);
            return View(comentario);
        }

        // GET: Comentarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comentario = await _context.Comentarios.FindAsync(id);
            if (comentario == null)
            {
                return NotFound();
            }
            ViewData["ID_Ocorrencia"] = new SelectList(_context.Ocorrencias, "ID_Ocorrencia", "Descricao", comentario.ID_Ocorrencia);
            ViewData["ID_Usuario"] = new SelectList(_context.Usuarios, "ID_Usuario", "Email", comentario.ID_Usuario);
            return View(comentario);
        }

        // POST: Comentarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID_Comentario,Texto,Data_hora,ID_Usuario,ID_Ocorrencia")] Comentario comentario)
        {
            if (id != comentario.ID_Comentario)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comentario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComentarioExists(comentario.ID_Comentario))
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
            ViewData["ID_Ocorrencia"] = new SelectList(_context.Ocorrencias, "ID_Ocorrencia", "Descricao", comentario.ID_Ocorrencia);
            ViewData["ID_Usuario"] = new SelectList(_context.Usuarios, "ID_Usuario", "Email", comentario.ID_Usuario);
            return View(comentario);
        }

        // GET: Comentarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comentario = await _context.Comentarios
                .Include(c => c.Ocorrencia)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.ID_Comentario == id);
            if (comentario == null)
            {
                return NotFound();
            }

            return View(comentario);
        }

        // POST: Comentarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comentario = await _context.Comentarios.FindAsync(id);
            if (comentario != null)
            {
                _context.Comentarios.Remove(comentario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ListarPorOcorrencia(int ocorrenciaId)
        {
            var comentarios = await _context.Comentarios
                .Include(c => c.Usuario)
                .Where(c => c.ID_Ocorrencia == ocorrenciaId)
                .OrderByDescending(c => c.Data_hora)
                .Select(c => new {
                    usuarioNome = c.Usuario.Nome,
                    usuarioFoto = c.Usuario.Foto_Perfil,
                    texto = c.Texto,
                    data = c.Data_hora.ToString("dd/MM HH:mm")
                })
                .ToListAsync();

            return Json(comentarios);
        }

        // POST: Comentarios/AdicionarComentario
        [HttpPost]
        [Authorize] // Só usuários logados podem comentar
        public async Task<IActionResult> AdicionarComentario(int ocorrenciaId, string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return BadRequest(new { success = false, message = "O comentário não pode estar vazio." });

            // Pega o ID do usuário logado no sistema de Cookies
            var usuarioIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(usuarioIdStr)) return Unauthorized();

            var novoComentario = new Comentario
            {
                ID_Ocorrencia = ocorrenciaId,
                ID_Usuario = int.Parse(usuarioIdStr),
                Texto = texto,
                Data_hora = DateTime.Now
            };

            _context.Comentarios.Add(novoComentario);
            await _context.SaveChangesAsync();

            // Busca os dados do autor para retornar ao JS
            var usuario = await _context.Usuarios.FindAsync(novoComentario.ID_Usuario);

            return Json(new
            {
                success = true,
                usuarioNome = usuario.Nome,
                usuarioFoto = usuario.Foto_Perfil,
                texto = novoComentario.Texto,
                data = novoComentario.Data_hora.ToString("dd/MM HH:mm")
            });
        }

        private bool ComentarioExists(int id)
        {
            return _context.Comentarios.Any(e => e.ID_Comentario == id);
        }
    }
}
