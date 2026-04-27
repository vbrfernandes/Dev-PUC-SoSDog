using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SosDog.Models;
using System.Security.Claims; // Necessário para pegar o ID do usuário
using Microsoft.AspNetCore.Authorization; // Para garantir que só logados criem

namespace Dev_PUC_SoSDog.Controllers
{
    public class OcorrenciasController : Controller
    {
        private readonly AppDbContext _context;

        public OcorrenciasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Ocorrencias
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Ocorrencias.Include(o => o.Usuario);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Ocorrencias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ocorrencia = await _context.Ocorrencias
                .Include(o => o.Usuario)
                .FirstOrDefaultAsync(m => m.ID_Ocorrencia == id);
            if (ocorrencia == null)
            {
                return NotFound();
            }

            return View(ocorrencia);
        }

        // GET: Ocorrencias/Create
        public IActionResult Create()
        {
            ViewData["ID_Usuario"] = new SelectList(_context.Usuarios, "ID_Usuario", "Email");
            return View();
        }

        // POST: Ocorrencias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        // POST: Ocorrencias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // Garante que o usuário está logado
        public async Task<IActionResult> Create([Bind("Tipo,Descricao,Latitude,Longitude,Sexo,Cor_Pelagem,Porte,Sociabilidade,Faixa_Etaria,Endereco,Recebeu_Agua,Recebeu_Comida")] Ocorrencia ocorrencia, IFormFile Foto_Animal)
        {
            // 1. Ignorar a validação de campos que não vêm do formulário
            ModelState.Remove("Usuario");
            ModelState.Remove("Status");
            ModelState.Remove("Foto_Animal");
            if (ModelState.IsValid)
            {
                // 1. Vincular o usuário logado automaticamente
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return Unauthorized();
                ocorrencia.ID_Usuario = int.Parse(userId);

                // 2. Configurações automáticas de sistema
                ocorrencia.Data_Registro = DateTime.UtcNow;

                // Gera um código único para o animal (Ex: DOG-4829)
                ocorrencia.Codigo_Cachorro = "DOG-" + new Random().Next(1000, 9999).ToString();

                // Se a pessoa marcou que deu água ou comida, salva a data e hora atual
                if (ocorrencia.Recebeu_Agua || ocorrencia.Recebeu_Comida)
                {
                    ocorrencia.Data_Ultima_Alimentacao = DateTime.UtcNow;
                }

                // 3. Processar Upload da Foto
                if (Foto_Animal != null && Foto_Animal.Length > 0)
                {
                    string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/ocorrencias");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    string fileName = Guid.NewGuid().ToString() + "_" + Foto_Animal.FileName;
                    string filePath = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Foto_Animal.CopyToAsync(stream);
                    }

                    ocorrencia.Foto_Animal = "/images/ocorrencias/" + fileName;
                }

                _context.Add(ocorrencia);
                await _context.SaveChangesAsync();

                TempData["Sucesso"] = "Ocorrência registrada com sucesso!";
                return RedirectToAction("Index", "Home");
            }
            // SE DER ERRO: Em vez de return View(), voltamos para a página principal com erro
            TempData["Erro"] = "Não foi possível registrar a ocorrência. Verifique se todos os campos obrigatórios foram preenchidos e se uma foto foi enviada.";
            return RedirectToAction("Index", "Home");
        }

        // GET: Ocorrencias/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var ocorrencia = await _context.Ocorrencias.FindAsync(id);
            if (ocorrencia == null) return NotFound();

            // Segurança: Impede que um usuário edite a ocorrência de outro
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (ocorrencia.ID_Usuario != userId) return Forbid();

            if (ocorrencia == null) return NotFound();



            return PartialView("_EditarOcorrenciaModal", ocorrencia);
        }

        // POST: Ocorrencias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, Ocorrencia ocorrencia, IFormFile NovaFoto)
        {
            if (id != ocorrencia.ID_Ocorrencia)
            {
                return NotFound();
            }

            // 1. Ignora a validação do campo Usuario e Status, igual fizemos no Create
            ModelState.Remove("Usuario");
            ModelState.Remove("Comentarios");
            ModelState.Remove("FavoritadosPor");
            ModelState.Remove("Foto_Animal");
            ModelState.Remove("NovaFoto");

            if (ModelState.IsValid)
            {
                try
                {
                    // 2. Processar Upload de NOVA Foto (se o usuário selecionou uma)
                    if (NovaFoto != null && NovaFoto.Length > 0)
                    {
                        string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/ocorrencias");
                        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                        string fileName = Guid.NewGuid().ToString() + "_" + NovaFoto.FileName;
                        string filePath = Path.Combine(folder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await NovaFoto.CopyToAsync(stream);
                        }

                        // Sobrescreve o caminho da foto antiga pela nova
                        ocorrencia.Foto_Animal = "/images/ocorrencias/" + fileName;
                    }
                    // Se não enviou foto nova, o input hidden do modal mantém a Foto_Animal antiga!

                    _context.Update(ocorrencia);
                    await _context.SaveChangesAsync();

                    TempData["Sucesso"] = "Ocorrência atualizada com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OcorrenciaExists(ocorrencia.ID_Ocorrencia))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // 3. Redireciona para a página principal (Home) onde está o mapa, e não para o Index antigo de Ocorrencias
                return RedirectToAction("Index", "Home");
            }

            // Se der erro de validação, volta para o mapa com um aviso
            TempData["Erro"] = "Não foi possível salvar as alterações. Verifique os dados.";
            return RedirectToAction("Index", "Home");
        }

        // GET: Ocorrencias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ocorrencia = await _context.Ocorrencias
                .Include(o => o.Usuario)
                .FirstOrDefaultAsync(m => m.ID_Ocorrencia == id);
            if (ocorrencia == null)
            {
                return NotFound();
            }

            return View(ocorrencia);
        }

        // POST: Ocorrencias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ocorrencia = await _context.Ocorrencias.FindAsync(id);
            if (ocorrencia != null)
            {
                _context.Ocorrencias.Remove(ocorrencia);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OcorrenciaExists(int id)
        {
            return _context.Ocorrencias.Any(e => e.ID_Ocorrencia == id);
        }
    }
}
