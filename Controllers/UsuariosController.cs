using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SosDog.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;
using System.Net.Mail;

namespace Dev_PUC_SoSDog.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UsuariosController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuarios.ToListAsync());
        }

        // POST: Usuarios/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string senha)
        {
            // 1. Busca o usuário no banco APENAS pelo E-mail
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);

            // 2. Verifica se achou o usuário (se não achou, é null) 
            // E depois verifica se a senha pura bate com o Hash salvo no banco
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(senha, usuario.Senha))
            {
                TempData["ErroLogin"] = "E-mail ou senha inválidos.";
                TempData["AbrirModalLogin"] = true; // Flag para reabrir o modal via JS
                return RedirectToAction("Index", "Home");
            }

            // 3. Tudo certo! Cria as "Credenciais" (Claims) do usuário para o Cookie
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, usuario.ID_Usuario.ToString()),
        new Claim(ClaimTypes.Name, usuario.Nome),
        new Claim(ClaimTypes.Email, usuario.Email)
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // 4. Gera o cookie e "Loga" o usuário
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        // POST: Usuarios/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Limpa o cookie e desloga
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.ID_Usuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // 1. O Bind foi atualizado apenas com os campos que realmente existem e que o usuário deve preencher

        public async Task<IActionResult> Create([Bind("Nome,Email,Senha,Telefone")] Usuario usuario, string ConfirmarSenha, IFormFile FotoUpload)
        {
            // 1. Removemos a validação automática da string 'Foto_Perfil' 
            // porque o usuário enviou um arquivo (FotoUpload), e nós vamos preencher a string manualmente abaixo.
            ModelState.Remove("Foto_Perfil");

            // Verifica se os outros campos obrigatórios vieram preenchidos
            if (!ModelState.IsValid)
            {
                TempData["ErroCadastro"] = "Preencha todos os campos obrigatórios corretamente.";
                TempData["AbrirModalCadastro"] = true;
                return RedirectToAction("Index", "Home");
            }

            // Validação de Senha
            if (usuario.Senha != ConfirmarSenha)
            {
                TempData["ErroCadastro"] = "As senhas não coincidem.";
                TempData["AbrirModalCadastro"] = true;
                return RedirectToAction("Index", "Home");
            }

            // Verifica E-mail duplicado
            var emailExiste = await _context.Usuarios.AnyAsync(u => u.Email == usuario.Email);
            if (emailExiste)
            {
                TempData["ErroCadastro"] = "Este e-mail já está em uso.";
                TempData["AbrirModalCadastro"] = true;
                return RedirectToAction("Index", "Home");
            }

            // ==========================================
            // LÓGICA DE UPLOAD DA IMAGEM
            // ==========================================
            if (FotoUpload != null && FotoUpload.Length > 0)
            {
                // Define a pasta de destino: wwwroot/uploads/usuarios
                string pastaDestino = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "usuarios");

                // Cria a pasta se ela não existir
                if (!Directory.Exists(pastaDestino))
                {
                    Directory.CreateDirectory(pastaDestino);
                }

                // Gera um nome único para o arquivo (para não sobrepor fotos com o mesmo nome)
                string nomeArquivo = Guid.NewGuid().ToString() + "_" + FotoUpload.FileName;
                string caminhoCompleto = Path.Combine(pastaDestino, nomeArquivo);

                // Salva o arquivo na pasta
                using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                {
                    await FotoUpload.CopyToAsync(stream);
                }

                // Agora sim, guardamos o NOME do arquivo na propriedade Foto_Perfil para ir pro banco de dados
                usuario.Foto_Perfil = nomeArquivo;
            }
            else
            {
                TempData["ErroCadastro"] = "Por favor, selecione uma foto de perfil.";
                TempData["AbrirModalCadastro"] = true;
                return RedirectToAction("Index", "Home");
            }

            // Aplica o Hash do BCrypt na senha
            usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);

            try
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();

                TempData["Sucesso"] = "Conta criada com sucesso! Faça seu login.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                TempData["ErroCadastro"] = "Erro ao salvar os dados. Tente novamente mais tarde.";
                TempData["AbrirModalCadastro"] = true;
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Usuarios/SolicitarReset
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SolicitarReset(string emailRecuperacao)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == emailRecuperacao);
            if (usuario == null)
            {
                // Por segurança, não avisamos que o e-mail não existe, apenas fingimos que enviou
                TempData["SucessoReset"] = "Se o e-mail existir, um token foi enviado para ele.";
                return RedirectToAction("ResetarSenha", new { email = emailRecuperacao });
            }

            // Gera um token de 6 dígitos
            Random random = new Random();
            string token = random.Next(100000, 999999).ToString();

            // Salva no banco com validade de 15 minutos
            usuario.ResetToken = token;
            usuario.ResetTokenExpiracao = DateTime.Now.AddMinutes(15);
            await _context.SaveChangesAsync();

            // Envia o E-mail (ATENÇÃO: Você precisa colocar um e-mail e senha real aqui)
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("SEU_EMAIL@gmail.com", "SUA_SENHA_DE_APP"),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("SEU_EMAIL@gmail.com"),
                    Subject = "SoSDog - Recuperação de Senha",
                    Body = $"Seu código de recuperação é: <b>{token}</b>. Ele expira em 15 minutos.",
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(emailRecuperacao);

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                TempData["ErroLogin"] = "Erro ao enviar o e-mail de recuperação.";
                return RedirectToAction("Index", "Home");
            }

            // Guardamos o e-mail para usar no formulário oculto
            TempData["EmailRecuperacao"] = emailRecuperacao;
            // Avisamos o Javascript para abrir o Modal do Token
            TempData["AbrirModalToken"] = "true";

            return RedirectToAction("Index", "Home");
        }

 

        // POST: Usuarios/ConfirmarReset
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarReset(string email, string token, string novaSenha, string confirmarNovaSenha)
        {
            if (novaSenha != confirmarNovaSenha)
            {
                TempData["ErroReset"] = "As senhas não coincidem.";
                return RedirectToAction("ResetarSenha", new { email = email });
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email && u.ResetToken == token);

            if (usuario == null || usuario.ResetTokenExpiracao < DateTime.Now)
            {
                TempData["ErroReset"] = "Token inválido ou expirado.";
                return RedirectToAction("ResetarSenha", new { email = email });
            }

            // Tudo certo! Aplica o BCrypt na nova senha, limpa o token e salva
            usuario.Senha = BCrypt.Net.BCrypt.HashPassword(novaSenha);
            usuario.ResetToken = null;
            usuario.ResetTokenExpiracao = null;

            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Senha alterada com sucesso! Faça seu login.";
            return RedirectToAction("Index", "Home"); // Volta pra home para fazer login
        }


        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID_Usuario,Nome,Email,Senha,Foto_Perfil,Data_Cadastro,LocalizacaoAtual,Bio,Telefone")] Usuario usuario)
        {
            if (id != usuario.ID_Usuario)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.ID_Usuario))
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
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.ID_Usuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.ID_Usuario == id);
        }
    }
}
