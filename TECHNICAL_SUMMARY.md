# 📋 RESUMO TÉCNICO COMPLETO - PROJETO SoSDog

**Repositório:** https://github.com/vbrfernandes/Dev-PUC-SoSDog  
**Branch:** master  
**Stack:** .NET 10 + ASP.NET Core MVC + SQL Server  
**Tipo:** Web Application (Controllers + Razor Views)  
**Última Atualização:** 2025

---

## 📌 ÍNDICE

1. [Objetivo Principal](#1-objetivo-principal-do-sistema)
2. [Stack Tecnológica](#2-stack-tecnológica-e-arquitetura)
3. [Entidades e Relacionamentos](#3-principais-entidades-e-relacionamentos)
4. [Fluxo de Dados](#4-fluxo-de-dados-principal)
5. [Ciclo de Vida HTTP](#5-ciclo-de-vida-de-uma-requisição-http)
6. [Controllers e Ações](#6-controllers-e-ações)
7. [Schema do Banco](#7-schema-do-banco-de-dados)
8. [Configuração](#8-configuração-e-dependências)
9. [Segurança](#9-security-considerations)
10. [Melhorias Futuras](#10-technical-debt-e-melhorias-futuras)

---

## 1. OBJETIVO PRINCIPAL DO SISTEMA

**SoSDog** é uma aplicação web comunitária para **reportar e gerenciar ocorrências de cães** perdidos ou em situação de rua. O sistema permite que usuários:

- 📍 **Registrem ocorrências** de cães perdidos ou em condições de rua com localização geográfica
- 💬 **Compartilhem informações** através de comentários nas ocorrências
- ❤️ **Favoritarem ocorrências** para acompanhamento personalizado
- 👤 **Autenticação de usuários** via e-mail e senha com cookies
- 📸 **Gerenciamento de perfil** com foto de perfil e telefone de contato

**Público-alvo:** Comunidade de protetores de animais, pessoas que encontraram cães perdidos, tutores procurando seus pets.

---

## 2. STACK TECNOLÓGICA E ARQUITETURA

### 🛠️ Tecnologias Utilizadas

| Componente | Tecnologia | Versão |
|-----------|-----------|---------|
| **Framework Web** | ASP.NET Core | 10 |
| **Linguagem** | C# | 13 |
| **Padrão Arquitetural** | MVC (Model-View-Controller) | - |
| **Banco de Dados** | SQL Server | LocalDB (dev) |
| **ORM** | Entity Framework Core | 10 |
| **Autenticação** | Cookie-based (ASP.NET Core) | Built-in |
| **Criptografia** | BCrypt.Net-Next | 4.1.0 |
| **Front-end** | Razor Views + HTML/CSS | - |
| **Compilação** | Nullable Reference Types | Enabled |
| **Implicit Usings** | Global Usings | Enabled |

### 🏗️ Arquitetura (Camadas)

```
┌──────────────────────────────────────────────┐
│        PRESENTATION LAYER (Views)            │
│  Razor Views (.cshtml) + HTML/CSS/JavaScript│
└────────────────────┬─────────────────────────┘
                     │
┌────────────────────▼─────────────────────────┐
│      CONTROLLER LAYER (Business Logic)       │
│  • HomeController                            │
│  • UsuariosController (Auth)                 │
│  • OcorrenciasController (CRUD)              │
│  • ComentariosController (Comments)          │
│  • FavoritosController (Favorites)           │
└────────────────────┬─────────────────────────┘
                     │
┌────────────────────▼─────────────────────────┐
│  DATA ACCESS LAYER (Entity Framework Core)   │
│  • AppDbContext (DbContext)                  │
│  • Models & Data Annotations                 │
│  • Migrations                                │
└────────────────────┬─────────────────────────┘
                     │
┌────────────────────▼─────────────────────────┐
│   DATABASE LAYER (SQL Server LocalDB)        │
│  • Usuarios                                  │
│  • Ocorrencias                               │
│  • Comentarios                               │
│  • Favoritos                                 │
└──────────────────────────────────────────────┘
```

### 🎨 Padrões de Design Utilizados

- **Repository Pattern** (implícito via EF Core DbContext)
- **Dependency Injection** (ASP.NET Core built-in container)
- **Authentication via Claims/Cookie** (ASP.NET Core Identity)
- **Data Validation via Annotations** (DataAnnotations attributes)
- **Migrations Pattern** (EF Core Migrations)

---

## 3. PRINCIPAIS ENTIDADES E RELACIONAMENTOS

### 📊 Diagrama ER (Entidade-Relacionamento)

```
┌─────────────────────────────────────┐
│            USUARIO                  │
├─────────────────────────────────────┤
│ PK: ID_Usuario                      │
│ Nome (string, 100 chars)            │
│ Email (string, unique)              │
│ Senha (string, encrypted)           │
│ Foto_Perfil? (string, nullable)     │
│ Telefone? (string, nullable)        │
└─────────┬───────────────────────────┘
          │
          ├─────────────┬──────────────┬──────────────┐
        1:N           1:N            1:N            1:N
          │            │              │              │
    ┌─────▼──────┐ ┌──▼───────────┐ ┌─▼────────────┐
    │ OCORRENCIA │ │ COMENTARIO   │ │  FAVORITO    │
    ├────────────┤ ├──────────────┤ ├──────────────┤
    │ PK: ID_Oco │ │ PK: ID_Com   │ │ PK: ID_Fav   │
    │ Tipo       │ │ Texto        │ │ FK: ID_User  │
    │ Status     │ │ Data_hora    │ │ FK: ID_Oco   │
    │ Foto_Animal│ │ FK: ID_User  │ └──────────────┘
    │ Descricao  │ │ FK: ID_Oco   │
    │ Latitude   │ └──────────────┘
    │ Longitude  │
    │ Data_Reg   │
    │ FK: ID_User│
    └────────────┘
```

### 📌 Detalhamento das Entidades

#### **USUARIO**

```csharp
Responsabilidade: Representar usuários do sistema

Propriedades:
  • ID_Usuario (PK, Identity)
    └─ Identificador único, auto-incrementado

  • Nome (string, Required, MaxLength: 100)
    └─ Nome completo do usuário

  • Email (string, Required, Unique, EmailAddress)
    └─ E-mail válido, usado para login

  • Senha (string, Required, DataType.Password)
    └─ Armazenada com hash BCrypt (em produção)

  • Foto_Perfil? (string, Nullable)
    └─ URL/caminho da foto, default: "default-user.png"

  • Telefone? (string, Nullable, Phone)
    └─ Contato telefônico validado

Relacionamentos (1:N):
  • OcorrenciasRegistradas → Ocorrências criadas pelo usuário
  • Comentarios → Comentários feitos pelo usuário
  • Favoritos → Ocorrências favoritadas pelo usuário

Delete Behavior:
  • ON DELETE CASCADE (se usuário deletado, tudo dele é deletado)
```

#### **OCORRENCIA**

```csharp
Responsabilidade: Representar uma ocorrência de cão perdido/na rua

Propriedades:
  • ID_Ocorrencia (PK, Identity)
    └─ Identificador único

  • Tipo (string, Required)
    └─ Valores: "Perdido", "Rua", etc.

  • Status (string, Required)
    └─ Valores: "Aberto", "Resolvido", etc.

  • Foto_Animal? (string, Nullable)
    └─ URL/caminho da foto do animal

  • Descricao (string, Required)
    └─ Descrição detalhada da ocorrência

  • Latitude (float, Required)
    └─ Coordenada geográfica

  • Longitude (float, Required)
    └─ Coordenada geográfica

  • Data_Registro (DateTime, Required)
    └─ Timestamp automático (DateTime.UtcNow)

  • ID_Usuario (FK, Required)
    └─ Referência ao usuário que registrou

Relacionamentos (1:N):
  • Usuario → Usuário que criou (obrigatório)
  • Comentarios → Comentários sobre a ocorrência
  • FavoritadosPor → Usuários que favoritaram

Delete Behavior:
  • ON DELETE CASCADE (se usuário deletado, ocorrências deletadas)
```

#### **COMENTARIO**

```csharp
Responsabilidade: Permitir discussão e compartilhamento de informações

Propriedades:
  • ID_Comentario (PK, Identity)
    └─ Identificador único

  • Texto (string, Required)
    └─ Conteúdo do comentário

  • Data_hora (DateTime)
    └─ Timestamp automático (DateTime.Now)

  • ID_Usuario (FK, Required)
    └─ Autor do comentário

  • ID_Ocorrencia (FK, Required)
    └─ Ocorrência comentada

Relacionamentos (N:1):
  • Usuario → Quem comentou
  • Ocorrencia → Em qual ocorrência

Delete Behavior:
  • Usuario: ON DELETE CASCADE
  • Ocorrencia: ON DELETE RESTRICT (impede deletar ocorrência com comentários)
```

#### **FAVORITO**

```csharp
Responsabilidade: Tabela de junção (Many-to-Many) entre Usuário e Ocorrência

Propriedades:
  • ID_Favorito (PK, Identity)
    └─ Identificador único

  • ID_Usuario (FK, Required)
    └─ Quem favoritou

  • ID_Ocorrencia (FK, Required)
    └─ O que foi favoritado

Relacionamentos (N:1):
  • Usuario → Usuário que favoritou
  • Ocorrencia → Ocorrência favoritada

Delete Behavior:
  • Usuario: ON DELETE CASCADE
  • Ocorrencia: ON DELETE RESTRICT (impede deletar ocorrência favoritada)
```

### 🗑️ Regras de Cascata

| Relacionamento | Behavior | Efeito |
|---------------|----------|--------|
| Comentario → Ocorrencia | RESTRICT | ❌ Impede deletar ocorrência com comentários |
| Favorito → Ocorrencia | RESTRICT | ❌ Impede deletar ocorrência favoritada |
| Ocorrencia → Usuario | CASCADE | ✓ Deleta ocorrências quando usuário deletado |
| Comentario → Usuario | CASCADE | ✓ Deleta comentários quando usuário deletado |
| Favorito → Usuario | CASCADE | ✓ Deleta favoritos quando usuário deletado |

---

## 4. FLUXO DE DADOS PRINCIPAL

### 🔐 Fluxo 1: AUTENTICAÇÃO (Login/Logout)

```
┌───────────────────────────────────────────────────────────┐
│ 1. ACESSO INICIAL                                         │
│    Usuário acessa: GET /Home/Index                        │
│    └─→ Se não autenticado: Vê formulário de login         │
│    └─→ Se autenticado: Acesso direto ao conteúdo         │
└──────────────────┬────────────────────────────────────────┘
                   │
┌──────────────────▼────────────────────────────────────────┐
│ 2. SUBMISSÃO DO FORMULÁRIO                                │
│    POST /Usuarios/Login                                   │
│    Dados: email (string), senha (string)                  │
│    Headers: Cookie (se houver sessão anterior)            │
└──────────────────┬────────────────────────────────────────┘
                   │
┌──────────────────▼────────────────────────────────────────┐
│ 3. BUSCA NO BANCO                                         │
│    SELECT * FROM Usuarios                                 │
│    WHERE Email = @email AND Senha = @senha                │
│    Resultado: Usuario object OR null                      │
└────────────┬─────────────────────────────────────────────┘
             │
      ┌──────┴──────────┐
      │                 │
   ✓ SUCESSO        ✗ FALHA
      │                 │
┌─────▼──────────────┐┌─────▼──────────────────┐
│ CRIA CLAIMS        ││ ARMAZENA ERRO          │
│ ├─ NameIdentifier  ││ TempData["ErroLogin"]  │
│ ├─ Name            ││ Redirect Home          │
│ └─ Email           ││                        │
│                    ││ User.Identity.IsAuth   │
│ SIGN-IN ASYNC      ││ = false                │
│ CookieAuthScheme   │└────────────────────────┘
│ Nome: "SoSDogAuth" │
└─────┬──────────────┘
      │
┌─────▼──────────────────────────────┐
│ COOKIE CRIADO E ENVIADO            │
│ HttpOnly: true (segurança)         │
│ Secure: true (HTTPS em prod)       │
│ SameSite: Strict (CSRF protection) │
└─────┬──────────────────────────────┘
      │
┌─────▼──────────────────────────────┐
│ REDIRECT → Home/Index              │
│ Status: 302 Found                  │
│ Header: Set-Cookie: SoSDogAuth=... │
└─────┬──────────────────────────────┘
      │
┌─────▼──────────────────────────────┐
│ CLIENTE RECEBE COOKIE               │
│ User.Identity.IsAuthenticated      │
│ = true                             │
│ User.FindFirst(ClaimTypes.Name)    │
│ = Nome do usuário                  │
└────────────────────────────────────┘

LOGOUT: POST /Usuarios/Logout
  → SignOutAsync()
  → Cookie deletado
  → Redirect Home
  → User.IsAuthenticated = false
```

### 📝 Fluxo 2: REGISTRAR OCORRÊNCIA

```
┌────────────────────────────────────────────────────────────┐
│ 1. USUÁRIO LOGADO CLICA "REGISTRAR OCORRÊNCIA"            │
│    GET /Ocorrencias/Create                                │
└──────────────────┬─────────────────────────────────────────┘
                   │
┌──────────────────▼─────────────────────────────────────────┐
│ 2. CARREGAR FORMULÁRIO VAZIO                              │
│    OcorrenciasController.Create() [GET]                   │
│    └─ Carrega SelectList de Usuarios em ViewData          │
│    └─ Retorna View com modelo vazio                       │
└──────────────────┬─────────────────────────────────────────┘
                   │
┌──────────────────▼─────────────────────────────────────────┐
│ 3. USUÁRIO PREENCHE FORMULÁRIO                            │
│    • Tipo: "Perdido" ou "Rua"                             │
│    • Status: "Aberto"                                     │
│    • Descrição: "Cão branco, sem coleira..."              │
│    • Localização: Latitude & Longitude                    │
│    • Foto: Upload ou referência                           │
│    • Clica [SALVAR]                                       │
└──────────────────┬─────────────────────────────────────────┘
                   │
┌──────────────────▼─────────────────────────────────────────┐
│ 4. SUBMISSÃO DO FORMULÁRIO                                │
│    POST /Ocorrencias/Create                               │
│    Dados: Tipo, Status, Descricao, Latitude,              │
│    Longitude, ID_Usuario, Foto_Animal, Data_Registro      │
└──────────────────┬─────────────────────────────────────────┘
                   │
┌──────────────────▼─────────────────────────────────────────┐
│ 5. VALIDAÇÃO DO MODELO (ModelState)                       │
│    • Tipo: Required ✓                                     │
│    • Status: Required ✓                                   │
│    • Descricao: Required ✓                                │
│    • Latitude: float Required ✓                           │
│    • Longitude: float Required ✓                          │
│                                                            │
│    Se falhar: Retorna View com erros                      │
│    Se passar: Continua...                                 │
└──────────────────┬─────────────────────────────────────────┘
                   │ (Válido)
┌──────────────────▼─────────────────────────────────────────┐
│ 6. INSTANCIAR OBJETO OCORRENCIA                           │
│    var ocorrencia = new Ocorrencia {                      │
│        Tipo = model.Tipo,                                 │
│        Status = model.Status,                             │
│        Descricao = model.Descricao,                       │
│        Latitude = model.Latitude,                         │
│        Longitude = model.Longitude,                       │
│        ID_Usuario = model.ID_Usuario,                     │
│        Data_Registro = DateTime.UtcNow                    │
│    };                                                     │
└──────────────────┬─────────────────────────────────────────┘
                   │
┌──────────────────▼─────────────────────────────────────────┐
│ 7. ADICIONAR AO CONTEXTO & SALVAR                         │
│    _context.Add(ocorrencia);                              │
│    await _context.SaveChangesAsync();                     │
│                                                            │
│    ↓ EF Core:                                             │
│    • Rastreia mudanças (Change Tracking)                  │
│    • Gera SQL INSERT                                      │
│    • Executa: INSERT INTO Ocorrencias (...)               │
│    • Retorna ID gerado (IDENTITY)                         │
└──────────────────┬─────────────────────────────────────────┘
                   │
┌──────────────────▼─────────────────────────────────────────┐
│ 8. REDIRECIONAMENTO                                       │
│    return RedirectToAction(nameof(Index));                │
│    Status: 302 Found                                      │
│    Location: /Ocorrencias/Index                           │
└──────────────────┬─────────────────────────────────────────┘
                   │
┌──────────────────▼─────────────────────────────────────────┐
│ 9. LISTAR OCORRÊNCIAS                                     │
│    GET /Ocorrencias/Index                                 │
│    SELECT * FROM Ocorrencias                              │
│    Include(o => o.Usuario)                                │
│    └─ Nova ocorrência aparece na lista                    │
└────────────────────────────────────────────────────────────┘
```

### 💬 Fluxo 3: VISUALIZAR DETALHES E COMENTAR

```
┌────────────────────────────────────────────────────────────┐
│ 1. CLIQUE NO DETALHE                                      │
│    GET /Ocorrencias/Details/5                             │
└──────────────────┬─────────────────────────────────────────┘
                   │
┌──────────────────▼─────────────────────────────────────────┐
│ 2. BUSCAR OCORRÊNCIA COM USUÁRIO                          │
│    SELECT * FROM Ocorrencias o                            │
│    JOIN Usuarios u ON o.ID_Usuario = u.ID_Usuario         │
│    WHERE o.ID_Ocorrencia = 5                              │
└──────────────────┬─────────────────────────────────────────┘
                   │
┌──────────────────▼─────────────────────────────────────────┐
│ 3. RENDERIZAR VIEW COM:                                   │
│    • Foto do animal                                       │
│    • Tipo e Status                                        │
│    • Descrição completa                                   │
│    • Localização (Latitude/Longitude)                     │
│    • Nome do usuário + contato                            │
│    • Data do registro                                     │
│    • Lista de comentários (com autor/data)                │
│    • Botão [♡ Favoritar] (se logado)                      │
│    • Form [Comentar] (se logado)                          │
└──────────────────┬─────────────────────────────────────────┘
                   │
┌──────────────────▼─────────────────────────────────────────┐
│ 4. USUÁRIO LOGADO COMENTA                                 │
│    Preenche: textarea com texto                           │
│    Clica [COMENTAR]                                       │
└──────────────────┬─────────────────────────────────────────┘
                   │
┌──────────────────▼─────────────────────────────────────────┐
│ 5. SUBMISSÃO DO COMENTÁRIO                                │
│    POST /Comentarios/Create                               │
│    Dados: texto, ID_Ocorrencia, ID_Usuario                │
└──────────────────┬─────────────────────────────────────────┘
                   │
┌──────────────────▼─────────────────────────────────────────┐
│ 6. VALIDAR & SALVAR                                       │
│    var comentario = new Comentario {                      │
│        Texto = model.Texto,                               │
│        Data_hora = DateTime.Now,                          │
│        ID_Usuario = userID,                               │
│        ID_Ocorrencia = model.ID_Ocorrencia                │
│    };                                                     │
│    _context.Add(comentario);                              │
│    await _context.SaveChangesAsync();                     │
└──────────────────┬─────────────────────────────────────────┘
                   │
┌──────────────────▼─────────────────────────────────────────┐
│ 7. REDIRECIONAMENTO                                       │
│    Redirect → /Ocorrencias/Details/5                      │
│    ↓ Nova Query ao banco                                  │
│    Novo comentário aparece na lista                       │
└────────────────────────────────────────────────────────────┘
```

### ❤️ Fluxo 4: FAVORITAR OCORRÊNCIA

```
┌─────────────────────────────────────────────────────────┐
│ 1. USUÁRIO CLICA ♡ (Favoritar)                          │
│    GET /Favoritos/Create?ID_Ocorrencia=5                │
└────────────────┬────────────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────────────┐
│ 2. SUBMISSÃO                                            │
│    POST /Favoritos/Create                               │
│    Dados: ID_Usuario, ID_Ocorrencia                     │
└────────────────┬────────────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────────────┐
│ 3. VALIDAÇÃO                                            │
│    ✓ ModelState.IsValid                                 │
│    Verifica duplicação? (TODO - Melhoria)               │
└────────────────┬────────────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────────────┐
│ 4. SALVAR FAVORITO                                      │
│    INSERT INTO Favoritos (ID_Usuario, ID_Ocorrencia)    │
│    VALUES (@userID, @occurrenceID)                      │
└────────────────┬────────────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────────────┐
│ 5. REDIRECIONAMENTO                                     │
│    Redirect → /Favoritos/Index                          │
│    Ocorrência aparece em "Meus Favoritos"               │
└─────────────────────────────────────────────────────────┘
```

---

## 5. CICLO DE VIDA DE UMA REQUISIÇÃO HTTP

```
┌─────────────────────────────────────────────────────────────┐
│                  CLIENTE (Browser)                          │
│  1. Usuário navega/clica em um link                         │
│  2. Browser envia HTTP Request                              │
└─────────────┬───────────────────────────────────────────────┘
              │ GET /Ocorrencias/Details/5
              │ Host: localhost:5000
              │ Cookie: SoSDogAuth=...
              │
┌─────────────▼───────────────────────────────────────────────┐
│            ASP.NET CORE PIPELINE                            │
│                                                             │
│ [1] Middleware: Logging/Diagnostics                        │
│     └─→ Registra informações da requisição                 │
│                                                             │
│ [2] Middleware: HTTPS Redirection                          │
│     └─→ Força HTTPS em produção                            │
│                                                             │
│ [3] Middleware: Static File Server                         │
│     └─→ Se for arquivo estático (css, js), retorna         │
│                                                             │
│ [4] Middleware: Routing                                    │
│     └─→ Mapeia URL → Controller/Action                    │
│     └─→ Resultado: OcorrenciasController.Details(5)       │
│                                                             │
│ [5] Middleware: Authentication                             │
│     └─→ Lê Cookie "SoSDogAuth"                             │
│     └─→ Desserializa Claims                                │
│     └─→ HttpContext.User = ClaimsPrincipal                 │
│                                                             │
│ [6] Middleware: Authorization                              │
│     └─→ Verifica [Authorize] attributes                    │
│     └─→ Se não autenticado → 401 Unauthorized              │
│     └─→ Se não autorizado → 403 Forbidden                  │
│                                                             │
│ [7] Dependency Injection                                   │
│     └─→ Cria OcorrenciasController                         │
│     └─→ Injeta: AppDbContext, ILogger, etc                │
│                                                             │
└─────────────┬───────────────────────────────────────────────┘
              │
┌─────────────▼───────────────────────────────────────────────┐
│            CONTROLLER ACTION EXECUTION                      │
│                                                             │
│ public async Task<IActionResult> Details(int? id) {       │
│     // 1. Validação de parâmetros                          │
│     if (id == null) return NotFound();                     │
│                                                             │
│     // 2. Query ao banco via EF Core                       │
│     var ocorrencia = await _context.Ocorrencias            │
│         .Include(o => o.Usuario)                          │
│         .FirstOrDefaultAsync(m => m.ID_Ocorrencia == id); │
│                                                             │
│     // 3. Validação do resultado                           │
│     if (ocorrencia == null) return NotFound();             │
│                                                             │
│     // 4. Retorna View com modelo                          │
│     return View(ocorrencia);                               │
│ }                                                          │
│                                                             │
└─────────────┬───────────────────────────────────────────────┘
              │
┌─────────────▼───────────────────────────────────────────────┐
│           ENTITY FRAMEWORK CORE                             │
│                                                             │
│ 1. Gera SQL Query:                                         │
│    SELECT o.*, u.*                                         │
│    FROM Ocorrencias o                                      │
│    JOIN Usuarios u ON o.ID_Usuario = u.ID_Usuario         │
│    WHERE o.ID_Ocorrencia = 5                               │
│                                                             │
│ 2. Executa query ao banco                                  │
│                                                             │
│ 3. Materializa DataReader em objetos C#                    │
│    └─→ Ocorrencia object (com Usuario carregado)           │
│                                                             │
│ 4. Change Tracking                                         │
│    └─→ Rastreia mudanças para futuras operações            │
│                                                             │
└─────────────┬───────────────────────────────────────────────┘
              │
┌─────────────▼───────────────────────────────────────────────┐
│          SQL SERVER DATABASE                                │
│                                                             │
│ Execute SQL, retorna dados                                 │
│                                                             │
└─────────────┬───────────────────────────────────────────────┘
              │
┌─────────────▼───────────────────────────────────────────────┐
│            VIEW RENDERING (Razor)                           │
│                                                             │
│ 1. Processa Details.cshtml                                 │
│ 2. Injecta Ocorrencia model                                │
│ 3. Renderiza HTML:                                         │
│    <h1>@Model.Tipo</h1>                                    │
│    <p>@Model.Descricao</p>                                 │
│    <p>Autor: @Model.Usuario.Nome</p>                       │
│    ...                                                     │
│ 4. Carrega assets (CSS, JS)                                │
│                                                             │
└─────────────┬───────────────────────────────────────────────┘
              │
┌─────────────▼───────────────────────────────────────────────┐
│           HTTP RESPONSE CONSTRUCTION                        │
│                                                             │
│ Status: 200 OK                                             │
│ Content-Type: text/html; charset=utf-8                     │
│ Content-Length: 5234                                       │
│ Set-Cookie: SoSDogAuth=...; HttpOnly; Secure              │
│ ...                                                        │
│ Body: <html><head>...</head><body>...</body></html>        │
│                                                             │
└─────────────┬───────────────────────────────────────────────┘
              │
┌─────────────▼───────────────────────────────────────────────┐
│                  CLIENTE (Browser)                          │
│                                                             │
│ 1. Recebe HTTP Response (200 OK)                           │
│ 2. Processa HTML                                           │
│ 3. Carrega CSS/JavaScript referenciado                     │
│ 4. Renderiza página visualmente                            │
│ 5. Armazena Cookie "SoSDogAuth"                            │
│ 6. Exibe conteúdo ao usuário                               │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## 6. CONTROLLERS E AÇÕES

| Controller | Action | Método | Propósito | Requer Auth |
|-----------|--------|--------|----------|------------|
| **Home** | Index | GET | Listar ocorrências públicas | ❌ |
| **Home** | Privacy | GET | Página de privacidade | ❌ |
| **Usuarios** | Index | GET | Listar todos os usuários | ⚠️ |
| **Usuarios** | Login | POST | Autenticar usuário | ❌ |
| **Usuarios** | Logout | POST | Desautenticar | ✅ |
| **Usuarios** | Create | GET/POST | Registrar novo usuário | ❌ |
| **Usuarios** | Details | GET | Ver perfil | ✅ |
| **Usuarios** | Edit | GET/POST | Editar perfil | ✅ |
| **Usuarios** | Delete | GET/POST | Deletar conta | ✅ |
| **Ocorrencias** | Index | GET | Listar ocorrências | ❌ |
| **Ocorrencias** | Details | GET | Ver detalhes | ❌ |
| **Ocorrencias** | Create | GET/POST | Registrar ocorrência | ✅ |
| **Ocorrencias** | Edit | GET/POST | Editar ocorrência | ✅ |
| **Ocorrencias** | Delete | GET/POST | Deletar ocorrência | ✅ |
| **Comentarios** | Index | GET | Listar comentários | ❌ |
| **Comentarios** | Details | GET | Ver comentário | ❌ |
| **Comentarios** | Create | GET/POST | Adicionar comentário | ✅ |
| **Comentarios** | Edit | GET/POST | Editar comentário | ✅ |
| **Comentarios** | Delete | GET/POST | Deletar comentário | ✅ |
| **Favoritos** | Index | GET | Listar meus favoritos | ✅ |
| **Favoritos** | Details | GET | Ver favorito | ✅ |
| **Favoritos** | Create | GET/POST | Favoritar ocorrência | ✅ |
| **Favoritos** | Delete | GET/POST | Remover favorito | ✅ |

---

## 7. SCHEMA DO BANCO DE DADOS

```sql
-- ========================================
-- Tabela: Usuarios
-- ========================================
CREATE TABLE Usuarios (
    ID_Usuario INT PRIMARY KEY IDENTITY(1,1),
    Nome NVARCHAR(100) NOT NULL,
    Email NVARCHAR(MAX) NOT NULL UNIQUE,
    Senha NVARCHAR(MAX) NOT NULL,
    Foto_Perfil NVARCHAR(MAX) NULL 
        DEFAULT 'default-user.png',
    Telefone NVARCHAR(MAX) NULL
);

-- ========================================
-- Tabela: Ocorrencias
-- ========================================
CREATE TABLE Ocorrencias (
    ID_Ocorrencia INT PRIMARY KEY IDENTITY(1,1),
    Tipo NVARCHAR(MAX) NOT NULL,
    Status NVARCHAR(MAX) NOT NULL,
    Foto_Animal NVARCHAR(MAX) NULL,
    Descricao NVARCHAR(MAX) NOT NULL,
    Latitude FLOAT NOT NULL,
    Longitude FLOAT NOT NULL,
    Data_Registro DATETIME2 NOT NULL 
        DEFAULT GETUTCDATE(),
    ID_Usuario INT NOT NULL,

    FOREIGN KEY (ID_Usuario) 
        REFERENCES Usuarios(ID_Usuario) 
        ON DELETE CASCADE
);

-- ========================================
-- Tabela: Comentarios
-- ========================================
CREATE TABLE Comentarios (
    ID_Comentario INT PRIMARY KEY IDENTITY(1,1),
    Texto NVARCHAR(MAX) NOT NULL,
    Data_hora DATETIME2 NOT NULL 
        DEFAULT GETDATE(),
    ID_Usuario INT NOT NULL,
    ID_Ocorrencia INT NOT NULL,

    FOREIGN KEY (ID_Usuario) 
        REFERENCES Usuarios(ID_Usuario) 
        ON DELETE CASCADE,

    FOREIGN KEY (ID_Ocorrencia) 
        REFERENCES Ocorrencias(ID_Ocorrencia) 
        ON DELETE RESTRICT
);

-- ========================================
-- Tabela: Favoritos
-- ========================================
CREATE TABLE Favoritos (
    ID_Favorito INT PRIMARY KEY IDENTITY(1,1),
    ID_Usuario INT NOT NULL,
    ID_Ocorrencia INT NOT NULL,

    FOREIGN KEY (ID_Usuario) 
        REFERENCES Usuarios(ID_Usuario) 
        ON DELETE CASCADE,

    FOREIGN KEY (ID_Ocorrencia) 
        REFERENCES Ocorrencias(ID_Ocorrencia) 
        ON DELETE RESTRICT
);

-- ========================================
-- ÍNDICES RECOMENDADOS (Performance)
-- ========================================
CREATE INDEX IX_Usuarios_Email 
    ON Usuarios(Email);

CREATE INDEX IX_Ocorrencias_ID_Usuario 
    ON Ocorrencias(ID_Usuario);

CREATE INDEX IX_Ocorrencias_Status 
    ON Ocorrencias(Status);

CREATE INDEX IX_Ocorrencias_Tipo 
    ON Ocorrencias(Tipo);

CREATE INDEX IX_Comentarios_ID_Ocorrencia 
    ON Comentarios(ID_Ocorrencia);

CREATE INDEX IX_Comentarios_ID_Usuario 
    ON Comentarios(ID_Usuario);

CREATE INDEX IX_Favoritos_ID_Usuario 
    ON Favoritos(ID_Usuario);

CREATE INDEX IX_Favoritos_ID_Ocorrencia 
    ON Favoritos(ID_Ocorrencia);

CREATE INDEX IX_Favoritos_Usuario_Ocorrencia 
    ON Favoritos(ID_Usuario, ID_Ocorrencia);
```

---

## 8. CONFIGURAÇÃO E DEPENDÊNCIAS

### 📦 NuGet Packages

```xml
<!-- Program.cs & Startup Configuration -->
<PackageReference Include="Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation" 
    Version="10.0.7" />

<!-- Entity Framework Core -->
<PackageReference Include="Microsoft.EntityFrameworkCore" 
    Version="10.0.5" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" 
    Version="10.0.5" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" 
    Version="10.0.5" />

<!-- Criptografia de Senha -->
<PackageReference Include="BCrypt.Net-Next" 
    Version="4.1.0" />

<!-- Ferramentas de Desenvolvimento -->
<PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" 
    Version="10.0.2" />
<PackageReference Include="Microsoft.VisualStudio.Azure.Containers.Tools.Targets" 
    Version="1.23.0" />

<!-- NuGet Management -->
<PackageReference Include="NuGet.Packaging" Version="7.3.1" />
<PackageReference Include="NuGet.Protocol" Version="7.3.1" />
```

### ⚙️ Configuração de Inicialização (Program.cs)

```csharp
// 1. Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. MVC
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

// 3. Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Index";
        options.AccessDeniedPath = "/Home/Index";
        options.Cookie.Name = "SoSDogAuth";
    });

// 4. Middleware Pipeline
app.UseAuthentication();
app.UseAuthorization();

// 5. Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

### 🔌 Connection String (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": 
      "Server=(localdb)\\mssqllocaldb;Database=SosDogDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

---

## 9. SECURITY CONSIDERATIONS

| Área | Status | Observação |
|------|--------|-----------|
| **Hash de Senha** | ❌ TODO | Senhas armazenadas em **texto plano** - CRÍTICO! |
| **SQL Injection** | ✅ Protegido | EF Core usa parameterized queries |
| **CSRF** | ⚠️ Parcial | ValidateAntiForgeryToken presente |
| **XSS** | ✅ Protegido | Razor escapa HTML por padrão |
| **Authorization** | ❌ Fraco | Sem verificação de ownership |
| **HTTPS** | ⚠️ Dev | Desativado em desenvolvimento |
| **Cookie Security** | ✅ OK | HttpOnly + Secure flags |

### 🔴 CRÍTICO: Implementar Hash de Senha

```csharp
// Em UsuariosController.Create():
// ❌ ATUAL (INSEGURO):
usuario.Senha = senhaPlana;

// ✅ CORRETO (SEGURO):
usuario.Senha = BCrypt.Net.BCrypt.HashPassword(senhaPlana, 12);

// ✅ VERIFICAÇÃO NO LOGIN:
var isPasswordValid = BCrypt.Net.BCrypt.Verify(senhaPlana, usuarioDb.Senha);
if (!isPasswordValid) return Unauthorized();
```

---

## 10. TECHNICAL DEBT E MELHORIAS FUTURAS

### 🐛 Issues Conhecidas

1. **Senhas em texto plano** - Implementar BCrypt hash
2. **Sem autorização** - Adicionar verificação de ownership
3. **Sem paginação** - Listas podem ficar grandes
4. **Sem cache** - Queries repetidas ao banco
5. **Sem validação de duplicação** - Múltiplos favoritos da mesma ocorrência

### 🚀 Melhorias Sugeridas

- [ ] Implementar paginação em Index actions
- [ ] Adicionar filtros de busca (por tipo, status, localização)
- [ ] Integração com Google Maps para visualizar localizações
- [ ] Sistema de notificações (quando comentam numa ocorrência favoritada)
- [ ] Upload de imagens (atual: apenas URLs)
- [ ] Relatórios de estatísticas
- [ ] API REST (para possível app mobile)
- [ ] Testes unitários (xUnit/NUnit)
- [ ] Logging estruturado (Serilog)
- [ ] Docker containerization

---

## 11. RESUMO EXECUTIVO

**SoSDog** é um projeto ASP.NET Core MVC em fase inicial de desenvolvimento (.NET 10) que visa conectar a comunidade para ajudar cães perdidos ou em situação de rua.

### Arquitetura
- **Camadas:** MVC (Controllers → Views) + EF Core ORM
- **Banco:** SQL Server LocalDB (desenvolvimento)
- **Autenticação:** Cookie-based com Claims

### Funcionalidades Core
- Registrar e listar ocorrências de cães
- Sistema de comentários comunitários
- Favoritar ocorrências para acompanhamento
- Autenticação básica de usuários

### Prioridades de Melhoria
1. 🔴 Hash de senhas (CRÍTICO)
2. 🟡 Autorização e ownership validation
3. 🟢 Performance (paginação, cache)

---

## 📄 INFORMAÇÕES DO PROJETO

- **Repositório GitHub:** https://github.com/vbrfernandes/Dev-PUC-SoSDog
- **Branch Atual:** master
- **Framework:** .NET 10 (LTS)
- **IDE:** Visual Studio Community 2026
- **Banco de Dados:** SQL Server (LocalDB)
- **Ambiente Local:** C:\Users\vitor\OneDrive\Área de Trabalho\Dev-PUC-SoSDog\Dev-PUC-SoSDog\

---

**Última Atualização:** 2025  
**Propósito:** Documentação técnica e onboarding de novos desenvolvedores

---

## 📞 SUPORTE E CONTRIBUIÇÃO

Para dúvidas ou sugestões sobre este documento, abra uma issue no repositório GitHub ou entre em contato com o time de desenvolvimento.
