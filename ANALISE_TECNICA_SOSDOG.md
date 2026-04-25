# Análise Técnica Completa - SoSDog

**Projeto:** SoSDog - Sistema de Gerenciamento de Ocorrências de Cães  
**Versão:** .NET 10  
**Tipo de Projeto:** ASP.NET Core Razor Pages + MVC  
**Data da Análise:** 2025  
**Repositório:** https://github.com/vbrfernandes/Dev-PUC-SoSDog

---

## 1. Objetivo Principal do Sistema

### Descrição Geral
O **SoSDog** é uma aplicação web desenvolvida para auxiliar no gerenciamento e compartilhamento de ocorrências envolvendo cães. O sistema permite que usuários:

- **Registrem ocorrências** de cães perdidos, encontrados ou em situação de rua
- **Visualizem ocorrências** em um dashboard interativo com mapa integrado
- **Compartilhem informações** através de comentários em casos específicos
- **Favorizem casos** para acompanhamento futuro
- **Recuperem senhas** através de fluxo seguro com tokens

### Casos de Uso Principais
1. **Usuário sem conta** → Visualiza ocorrências públicas
2. **Novo usuário** → Cria conta com foto de perfil e validação de e-mail
3. **Usuário logado** → Registra, comenta e favorita ocorrências
4. **Recuperação de conta** → Solicita reset de senha via token de 6 dígitos

### Público-Alvo
- Protetores de cães
- Voluntários em resgate animal
- Comunidade de amantes de animais
- Instituições de proteção animal

---

## 2. Stack Tecnológica e Arquitetura

### 2.1 Arquitetura Geral
**Padrão Arquitetural:** ASP.NET Core MVC com componentes Razor Pages

```
┌─────────────────────────────────────────────────┐
│           Frontend (Views - Razor)              │
│      (Leaflet Maps, Bootstrap, FontAwesome)     │
└──────────────────┬──────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────┐
│      Controllers (MVC Pattern)                  │
│  HomeController, UsuariosController, etc.      │
└──────────────────┬──────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────┐
│   Business Logic & Data Access Layer           │
│        (Entity Framework Core)                  │
└──────────────────┬──────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────┐
│   SQL Server (LocalDB)                         │
│   Database: SosDogDB                           │
└─────────────────────────────────────────────────┘
```

### 2.2 Stack Tecnológico Detalhado

#### Backend
| Componente | Tecnologia | Versão | Propósito |
|-----------|-----------|--------|----------|
| Framework | ASP.NET Core | 10.0 | Web application framework |
| ORM | Entity Framework Core | 10.0.5 | Data access & mapping |
| Banco de Dados | SQL Server | LocalDB | Persistência de dados |
| Autenticação | Cookie Authentication | 10.0.7 | Gerenciamento de sessão |
| Hash de Senha | BCrypt.Net-Next | 4.1.0 | Segurança de credenciais |
| Email | SmtpClient | Nativo | Envio de tokens de reset |

#### Frontend
| Componente | Tecnologia | Propósito |
|-----------|-----------|----------|
| Template Engine | Razor Pages/MVC | Renderização de views |
| Maps | Leaflet | Visualização de coordenadas |
| CSS Framework | Bootstrap | Styling responsivo |
| Icons | FontAwesome | Ícones da interface |
| Runtime Compilation | Razor Runtime Compilation | Hot reload em desenvolvimento |

#### DevOps
| Componente | Tecnologia | Propósito |
|-----------|-----------|----------|
| Containerização | Docker | Preparado para containers |
| Gerenciamento de Código | Git | Version control |

### 2.3 Padrões de Projeto Utilizados

1. **MVC Pattern** - Separação de concerns entre Models, Controllers e Views
2. **Dependency Injection** - Injeção de AppDbContext e IWebHostEnvironment
3. **Unit of Work** - DbContext como coordinador de transações
4. **Repository Pattern** - Implícito através de DbSet<T>
5. **Authentication/Authorization** - Cookie-based authentication com Claims

### 2.4 Configuração de Autenticação

```csharp
// Método: Cookie Authentication
// Localização: Program.cs
// Token Format: Claims-based
// Armazenamento: HTTP Cookies (SoSDogAuth)
// Redireccionamento: /Home/Index para usuários não autenticados
```

---

## 3. Principais Entidades e Relacionamentos

### 3.1 Diagrama de Entidades (ER)

```
┌──────────────────┐
│    USUARIO       │
├──────────────────┤
│ ID_Usuario (PK)  │◄──┐
│ Nome             │   │ 1:N
│ Email (UNIQUE)   │   │
│ Senha (Hash)     │   │
│ Foto_Perfil      │   │
│ Telefone         │   │
│ ResetToken       │   │
│ ResetTokenExpir. │   │
└──────────────────┘   │
    ▲                  │
    │                  │
    │1:N               │
    │                  │
    ├─────────┬────────┴──────────┐
    │         │                   │
    │    ┌────┴──────────┐   ┌───┴────────────┐
    │    │  OCORRENCIA   │   │  COMENTARIO    │
    │    ├───────────────┤   ├────────────────┤
    │    │ID_Ocorrencia  │   │ID_Comentario   │
    └───►│(PK)           │   │(PK)            │
         │Tipo           │   │Texto           │
         │Status         │   │Data_hora       │
         │Foto_Animal    │   │ID_Usuario (FK) │
         │Descricao      │   │ID_Ocorrencia   │
         │Latitude       │   │(FK)            │
         │Longitude      │   │                │
         │Data_Registro  │   │Relaciona: 1:N  │
         │ID_Usuario(FK) │   │                │
         │               │◄──┴─────────────┘
         │               │
         │        ▲      │
         │        │      │
         │        │1:N   │
         │        │      │
         │    ┌───┴──────┴───┐
         │    │  FAVORITO    │
         │    ├──────────────┤
         │    │ID_Favorito   │
         │    │(PK)          │
         │    │ID_Usuario(FK)│
         │    │ID_Ocorrencia │
         │    │(FK)          │
         │    │Relaciona: N:N│
         │    │(via tabela)  │
         │    └──────────────┘
         │
         └─ Registra: 1:N
```

### 3.2 Descrição Detalhada das Entidades

#### **USUARIO**
```csharp
namespace SosDog.Models
{
    public class Usuario
    {
        [Key]
        public int ID_Usuario { get; set; }                    // Identificador único

        [Required, StringLength(100)]
        public string Nome { get; set; }                       // Nome do usuário

        [Required, EmailAddress]
        public string Email { get; set; }                      // Email único para autenticação

        [Required, DataType(DataType.Password)]
        public string Senha { get; set; }                      // Senha em hash (BCrypt)

        [Required]
        public string Foto_Perfil { get; set; }               // Caminho relativo da foto

        [Required, Phone]
        public string Telefone { get; set; }                  // Contato de emergência

        // Campos para recuperação de senha
        public string? ResetToken { get; set; }               // Token de 6 dígitos
        public DateTime? ResetTokenExpiracao { get; set; }    // Validade: 15 minutos

        // Relacionamentos
        public virtual ICollection<Ocorrencia> OcorrenciasRegistradas { get; set; }
        public virtual ICollection<Comentario> Comentarios { get; set; }
        public virtual ICollection<Favorito> Favoritos { get; set; }
    }
}
```

**Função:** Representa um usuário do sistema com autenticação segura e reset de senha.

---

#### **OCORRENCIA**
```csharp
namespace SosDog.Models
{
    public class Ocorrencia
    {
        [Key]
        public int ID_Ocorrencia { get; set; }                 // Identificador único

        [Required]
        public string Tipo { get; set; }                       // "Perdido", "Encontrado", "Rua"

        [Required]
        public string Status { get; set; }                     // "Aberto", "Resolvido"

        public string? Foto_Animal { get; set; }              // Foto do cão (optional)

        [Required]
        public string Descricao { get; set; }                 // Descrição detalhada

        [Required]
        public float Latitude { get; set; }                   // Coordenada GPS (para mapa)

        [Required]
        public float Longitude { get; set; }                  // Coordenada GPS (para mapa)

        [Required]
        public DateTime Data_Registro { get; set; }           // Timestamp da ocorrência

        // Chave estrangeira
        [Required]
        public int ID_Usuario { get; set; }                  // Quem registrou

        [ForeignKey("ID_Usuario")]
        public virtual Usuario Usuario { get; set; }

        // Relacionamentos
        public virtual ICollection<Comentario> Comentarios { get; set; }
        public virtual ICollection<Favorito> FavoritadosPor { get; set; }
    }
}
```

**Função:** Núcleo do sistema - representa um caso de cão perdido/encontrado com localização GPS.

---

#### **COMENTARIO**
```csharp
namespace SosDog.Models
{
    public class Comentario
    {
        [Key]
        public int ID_Comentario { get; set; }               // Identificador único

        [Required]
        public string Texto { get; set; }                    // Conteúdo do comentário

        public DateTime Data_hora { get; set; }              // Quando foi postado

        // Chaves estrangeiras (Relacionamento 2:1 com Usuario e Ocorrencia)
        [Required]
        public int ID_Usuario { get; set; }                 // Autor do comentário

        [ForeignKey("ID_Usuario")]
        public virtual Usuario Usuario { get; set; }

        [Required]
        public int ID_Ocorrencia { get; set; }              // Onde foi comentado

        [ForeignKey("ID_Ocorrencia")]
        public virtual Ocorrencia Ocorrencia { get; set; }
    }
}
```

**Função:** Facilita colaboração entre usuários sobre um caso específico.

---

#### **FAVORITO**
```csharp
namespace SosDog.Models
{
    public class Favorito
    {
        [Key]
        public int ID_Favorito { get; set; }                 // Identificador único

        // Chaves estrangeiras (Relacionamento N:N entre Usuario e Ocorrencia)
        [Required]
        public int ID_Usuario { get; set; }                 // Quem favoritou

        [ForeignKey("ID_Usuario")]
        public virtual Usuario Usuario { get; set; }

        [Required]
        public int ID_Ocorrencia { get; set; }              // O que foi favoritado

        [ForeignKey("ID_Ocorrencia")]
        public virtual Ocorrencia Ocorrencia { get; set; }
    }
}
```

**Função:** Implementa relacionamento N:N para que usuários salvem ocorrências de interesse.

---

### 3.3 Análise de Relacionamentos

| Relacionamento | Tipo | Cardinalidade | Descrição |
|---|---|---|---|
| Usuario → Ocorrencia | 1:N | Um usuário registra muitas ocorrências | Restrição: Cascade |
| Usuario → Comentario | 1:N | Um usuário faz muitos comentários | Restrição: Cascade |
| Usuario → Favorito | 1:N | Um usuário favorita múltiplas ocorrências | Restrição: Cascade |
| Ocorrencia → Comentario | 1:N | Uma ocorrência tem muitos comentários | Restrição: **Restrict** |
| Ocorrencia → Favorito | 1:N | Uma ocorrência é favoritada por muitos | Restrição: **Restrict** |

**Nota sobre DeleteBehavior.Restrict:** Implementado em `OnModelCreating` para evitar conflitos de "Multiple Cascade Paths".

---

## 4. Fluxo de Dados Principal

### 4.1 Diagrama de Fluxo Completo

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         PÚBLICO (Não Autenticado)                       │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  [Página Inicial] ──► HomeController.Index() ◄─────┐                  │
│       ↓                    ↓                        │                  │
│   Visualiza Casos   SELECT * FROM Ocorrencias    AppDbContext          │
│                                                     │                  │
│                                            ┌────────┘                  │
│                                            │                           │
│                                    ┌───────▼───────┐                  │
│                                    │  SQL Server   │                  │
│                                    │   LocalDB     │                  │
│                                    │  SosDogDB     │                  │
│                                    └───────────────┘                  │
│                                                                         │
│  [Modal Login] ─────────────────────────────────────┐                 │
│       ↓                                              │                 │
│   email + senha ────────────────────────────────────┤                 │
│                                                      ▼                 │
└─────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────┐
│               AUTENTICAÇÃO (UsuariosController)                         │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  POST /Usuarios/Login                                                  │
│     ↓                                                                   │
│  Busca Usuário: SELECT * FROM Usuarios WHERE Email = @email           │
│     │                                                                   │
│     ├─ Não encontrado ──────────► TempData["ErroLogin"]               │
│     │                                                                   │
│     └─ Encontrado ──► BCrypt.Verify(senha, hash)                      │
│                           │                                            │
│                           ├─ Falso ───────► TempData["ErroLogin"]    │
│                           │                                            │
│                           └─ Verdadeiro                                │
│                                  ↓                                      │
│                         Cria Claims (ID, Nome, Email)                 │
│                                  ↓                                      │
│                         SignInAsync(CookieAuth)                        │
│                                  ↓                                      │
│                    Set-Cookie: SoSDogAuth={token}                     │
│                                  ↓                                      │
│                        Redireciona para /Home/Index                   │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────┐
│            USUÁRIO LOGADO - Registrar Ocorrência                        │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  [Clica em "Reportar"]                                                 │
│       ↓                                                                 │
│  GET /Ocorrencias/Create                                               │
│       ↓                                                                 │
│  Exibe Form com:                                                        │
│    - Tipo (select)                                                      │
│    - Status (select)                                                    │
│    - Latitude/Longitude (Leaflet Map Picker)                           │
│    - Descrição (textarea)                                               │
│    - Foto Animal (file upload)                                          │
│       ↓                                                                 │
│  POST /Ocorrencias/Create                                              │
│       ↓                                                                 │
│  ┌─ Validações                                                          │
│  │  - ModelState.IsValid                                                │
│  │  - Coordenadas válidas                                               │
│  │  - Arquivo de foto (se enviado)                                     │
│  │                                                                      │
│  ├─ Falha ──────────► Retorna View com erros                          │
│  │                                                                      │
│  └─ Sucesso                                                             │
│       ↓                                                                 │
│  INSERT INTO Ocorrencias                                               │
│  (Tipo, Status, Descricao, Latitude, Longitude,                       │
│   Data_Registro, ID_Usuario)                                           │
│       ↓                                                                 │
│  SaveChangesAsync() ──────────────────► SQL Server                    │
│       ↓                                                                 │
│  Redireciona para /Ocorrencias/Index (lista)                           │
│       ↓                                                                 │
│  SELECT * FROM Ocorrencias INCLUDE (Usuario)                          │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────┐
│         VISUALIZAR DETALHES & COMENTAR                                  │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  GET /Ocorrencias/Details/{id}                                         │
│       ↓                                                                 │
│  SELECT * FROM Ocorrencias WHERE ID = @id                             │
│  INCLUDE (Usuario) INCLUDE (Comentarios.Usuario)                       │
│       ↓                                                                 │
│  Exibe:                                                                 │
│    - Detalhes da ocorrência                                            │
│    - Localização no Leaflet Map                                        │
│    - Lista de comentários                                              │
│       ↓                                                                 │
│  POST /Comentarios/Create                                              │
│       ↓                                                                 │
│  INSERT INTO Comentarios                                               │
│  (Texto, ID_Usuario, ID_Ocorrencia, Data_hora)                         │
│       ↓                                                                 │
│  SaveChangesAsync() ──────────────────► SQL Server                    │
│       ↓                                                                 │
│  Atualiza lista de comentários na página                              │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────┐
│            FAVORITAR OCORRÊNCIA                                          │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  POST /Favoritos/Create                                                │
│       ↓                                                                 │
│  Valida:                                                                │
│    - ID_Ocorrencia existe?                                             │
│    - Usuário já favoritou?                                             │
│       ↓                                                                 │
│  INSERT INTO Favoritos (ID_Usuario, ID_Ocorrencia)                    │
│       ↓                                                                 │
│  SaveChangesAsync() ──────────────────► SQL Server                    │
│       ↓                                                                 │
│  Retorna JSON (success/error)                                          │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────┐
│         RECUPERAÇÃO DE SENHA                                             │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  POST /Usuarios/SolicitarReset                                         │
│       ↓                                                                 │
│  Busca Email: SELECT * FROM Usuarios WHERE Email = @email            │
│       ↓                                                                 │
│  Gera Token (Random 6 dígitos)                                         │
│       ↓                                                                 │
│  UPDATE Usuarios SET                                                   │
│    ResetToken = @token,                                                │
│    ResetTokenExpiracao = DateTime.Now + 15min                          │
│       ↓                                                                 │
│  SendEmailAsync(email, token) ──────────► Gmail SMTP                 │
│       ↓                                                                 │
│  TempData["AbrirModalToken"] = true                                    │
│       ↓                                                                 │
│  POST /Usuarios/ConfirmarReset                                         │
│       ↓                                                                 │
│  Valida:                                                                │
│    - Token correto?                                                     │
│    - Não expirou?                                                       │
│    - Senhas coincidem?                                                  │
│       ├─ Falha ──────────► TempData["ErroReset"]                      │
│       └─ Sucesso                                                        │
│            ↓                                                             │
│            UPDATE Usuarios SET                                          │
│              Senha = BCrypt.Hash(novaSenha),                            │
│              ResetToken = NULL                                          │
│            ↓                                                             │
│            TempData["Sucesso"] = "Senha alterada"                      │
│            ↓                                                             │
│            Redireciona para /Home/Index                                │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

### 4.2 Fluxos de Dados Críticos

#### **Fluxo 1: Autenticação**
```
Email + Senha 
    ↓
UsuariosController.Login()
    ↓
Busca no DB + Validação BCrypt
    ↓
Gera Claims + Cookie
    ↓
User.Identity logado ✓
```

#### **Fluxo 2: Registar Caso**
```
Usuário Logado + Formulário
    ↓
OcorrenciasController.Create()
    ↓
Validação + Upload Foto
    ↓
INSERT Ocorrencia
    ↓
Caso visível para comunidade ✓
```

#### **Fluxo 3: Colaboração (Comentários)**
```
Usuário Logado + Detalhe do Caso
    ↓
ComentariosController.Create()
    ↓
INSERT Comentario
    ↓
Outro usuário vê comentário ✓
```

#### **Fluxo 4: Rastreamento (Favoritos)**
```
Usuário Logado + Caso de Interesse
    ↓
FavoritosController.Create()
    ↓
INSERT Favorito (N:N)
    ↓
Usuário acessa /Favoritos/Index ✓
```

### 4.3 Transformações de Dados

| Origem | Transformação | Destino |
|--------|---------------|---------|
| Formulário Login | `Verify(senha, hash)` | Claims |
| Foto Upload | `Stream → FileSystem` | `/wwwroot/uploads/usuarios` |
| Coordenadas Leaflet | `Float (Lat/Long)` | Ocorrencia (BD) |
| Email | `SmtpClient.SendMailAsync()` | Caixa de entrada |
| Ocorrencia (List) | `Model → View` | Dashboard Mapa |

---

## 5. Configuração do Banco de Dados

### 5.1 Connection String
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SosDogDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

**Componentes:**
- **Server:** `(localdb)\\mssqllocaldb` - Instância local do SQL Server
- **Database:** `SosDogDB` - Nome do banco
- **Trusted_Connection:** True - Autenticação Windows
- **MultipleActiveResultSets:** true - Suporta múltiplas consultas simultâneas

### 5.2 Migrations Aplicadas

| Arquivo | Descrição | Data |
|---------|-----------|------|
| `20260423230652_InitialCreate.cs` | Criação inicial de tabelas | 23/04/2026 |
| `20260424185541_AddResetToken.cs` | Adição de campos para reset de senha | 24/04/2026 |

### 5.3 Delete Behavior (Estratégia de Deleção)

```csharp
// OnModelCreating - AppDbContext

// Comentarios → Ocorrencia: RESTRICT
modelBuilder.Entity<Comentario>()
    .HasOne(c => c.Ocorrencia)
    .WithMany(o => o.Comentarios)
    .HasForeignKey(c => c.ID_Ocorrencia)
    .OnDelete(DeleteBehavior.Restrict); // Não deleta ocorrência se tiver comentários

// Favoritos → Ocorrencia: RESTRICT
modelBuilder.Entity<Favorito>()
    .HasOne(f => f.Ocorrencia)
    .WithMany(o => o.FavoritadosPor)
    .HasForeignKey(f => f.ID_Ocorrencia)
    .OnDelete(DeleteBehavior.Restrict); // Não deleta ocorrência se tiver favoritos
```

**Justificativa:** Evita conflitos de "Multiple Cascade Paths" (múltiplos caminhos de exclusão em cascata).

---

## 6. Estrutura de Arquivos do Projeto

```
Dev-PUC-SoSDog/
│
├── Controllers/                    # Controladores MVC
│   ├── HomeController.cs          # Dashboard e landing page
│   ├── UsuariosController.cs      # Autenticação, registro, reset de senha
│   ├── OcorrenciasController.cs   # CRUD de casos
│   ├── ComentariosController.cs   # Gerenciamento de comentários
│   └── FavoritosController.cs     # Gerenciamento de favoritos
│
├── Models/                         # Entidades do Banco de Dados
│   ├── Usuario.cs                 # Usuário do sistema
│   ├── Ocorrencia.cs              # Caso de cão perdido/encontrado
│   ├── Comentario.cs              # Comentário em um caso
│   ├── Favorito.cs                # Favorito (relação N:N)
│   ├── AppDbContext.cs            # DbContext (EF Core)
│   └── ErrorViewModel.cs          # Modelo de erro
│
├── Views/                          # Razor Pages e Partials
│   ├── Home/
│   │   ├── Index.cshtml           # Dashboard com mapa Leaflet
│   │   └── Privacy.cshtml
│   ├── Usuarios/
│   │   ├── Create.cshtml          # Registro de usuário
│   │   ├── Details.cshtml
│   │   ├── Edit.cshtml
│   │   └── Delete.cshtml
│   ├── Ocorrencias/
│   │   ├── Index.cshtml           # Lista de ocorrências
│   │   ├── Create.cshtml          # Novo caso
│   │   ├── Details.cshtml         # Detalhes + comentários + mapa
│   │   ├── Edit.cshtml
│   │   └── Delete.cshtml
│   ├── Comentarios/               # CRUD de comentários
│   ├── Favoritos/                 # CRUD de favoritos
│   ├── Shared/
│   │   ├── _Layout.cshtml         # Master layout
│   │   ├── _ValidationScriptsPartial.cshtml
│   │   └── _Layout.cshtml.css
│   ├── _ViewStart.cshtml          # Configuração global de views
│   └── _ViewImports.cshtml        # Imports globais
│
├── Migrations/                     # Histórico de alterações do BD
│   ├── 20260423230652_InitialCreate.cs
│   ├── 20260423230652_InitialCreate.Designer.cs
│   ├── 20260424185541_AddResetToken.cs
│   ├── 20260424185541_AddResetToken.Designer.cs
│   └── AppDbContextModelSnapshot.cs
│
├── Properties/                     # Configurações do projeto
│   └── launchSettings.json
│
├── wwwroot/                        # Arquivos estáticos
│   ├── css/
│   ├── js/
│   ├── img/
│   ├── lib/                        # Bibliotecas CDN
│   └── uploads/
│       └── usuarios/               # Fotos de perfil dos usuários
│
├── Program.cs                      # Configuração da aplicação
├── appsettings.json                # Configurações
├── appsettings.Development.json    # Configurações de desenvolvimento
├── Dev-PUC-SoSDog.csproj          # Arquivo do projeto
└── Dockerfile                      # Configuração Docker

```

---

## 7. Dependências NuGet

```xml
<ItemGroup>
    <!-- Criptografia de Senha -->
    <PackageReference Include="BCrypt.Net-Next" Version="4.1.0" />

    <!-- Razor Runtime Compilation (Hot Reload) -->
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation" Version="10.0.7" />

    <!-- Entity Framework Core -->
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.5" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.5" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.5" />

    <!-- Docker Support -->
    <PackageReference Include="Microsoft.VisualStudio.Azure.Containers.Tools.Targets" Version="1.23.0" />

    <!-- Code Generation -->
    <PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="10.0.2" />

    <!-- Package Management -->
    <PackageReference Include="NuGet.Packaging" Version="7.3.1" />
    <PackageReference Include="NuGet.Protocol" Version="7.3.1" />
</ItemGroup>
```

---

## 8. Fluxos de Casos de Uso

### UC1: Criar Conta
```
Ator: Novo Usuário
Pré-condições: Sem autenticação
Fluxo Principal:
  1. Acessa /Home/Index
  2. Clica em "Cadastre-se" (Modal)
  3. Preenche: Nome, Email, Telefone, Senha, Confirmação
  4. Seleciona foto de perfil
  5. POST /Usuarios/Create
  6. Sistema valida (email único, senhas iguais, foto presente)
  7. Hash BCrypt da senha
  8. INSERT Usuario
  9. Sucesso: Mensagem "Conta criada com sucesso"
  10. Redireciona para login
```

### UC2: Autenticar (Login)
```
Ator: Usuário Registrado
Pré-condições: Possui conta ativa
Fluxo Principal:
  1. Acessa /Home/Index
  2. Clica em "Login" (Modal)
  3. Preenche Email e Senha
  4. POST /Usuarios/Login
  5. Sistema busca usuário por email
  6. Valida com BCrypt.Verify()
  7. Gera Claims (ID, Nome, Email)
  8. SignInAsync() → Cookie "SoSDogAuth"
  9. Sucesso: Redirecionado para /Home/Index (agora logado)

Fluxo Alternativo (Erro):
  - Email/Senha inválidos → TempData["ErroLogin"]
  - Modal reabre automaticamente via JavaScript
```

### UC3: Registrar Caso
```
Ator: Usuário Logado
Pré-condições: Autenticado
Fluxo Principal:
  1. Clica em "Reportar" (botão laranja)
  2. GET /Ocorrencias/Create
  3. Preenche formulário:
     - Tipo: Perdido / Encontrado / Rua
     - Status: Aberto / Resolvido
     - Descrição: Detalhes do caso
     - Localização: Clica no mapa Leaflet (Latitude/Longitude)
     - Foto: Upload de imagem
  4. POST /Ocorrencias/Create
  5. Valida ModelState
  6. Salva foto em /wwwroot/uploads/usuarios/
  7. INSERT Ocorrencia (com ID_Usuario do logado)
  8. Sucesso: Redirecionado para /Ocorrencias/Index
  9. Caso aparece na lista e mapa para toda comunidade
```

### UC4: Visualizar e Comentar Caso
```
Ator: Qualquer Usuário (Logado para comentar)
Pré-condições: Caso existe
Fluxo Principal:
  1. Visualiza /Home/Index (Mapa com casos)
  2. Clica em um case-card
  3. GET /Ocorrencias/Details/{id}
  4. Sistema carrega:
     - Detalhes do caso
     - Localização no mapa Leaflet
     - Lista de comentários (com autor e timestamp)
  5. Se logado, pode:
     - Escrever comentário em textarea
     - POST /Comentarios/Create
     - INSERT Comentario (com ID_Usuario)
     - Comentário aparece na lista em tempo real
```

### UC5: Favoritar Caso
```
Ator: Usuário Logado
Pré-condições: Caso existe, usuário autenticado
Fluxo Principal:
  1. Visualiza caso em /Ocorrencias/Details/{id}
  2. Clica em ❤️ "Favoritar"
  3. POST /Favoritos/Create
  4. Sistema valida:
     - Ocorrência existe?
     - Já não foi favoritado?
  5. INSERT Favorito (ID_Usuario, ID_Ocorrencia)
  6. Sucesso: Ícone muda (❤️ filled)
  7. Caso aparece em /Favoritos/Index do usuário
```

### UC6: Recuperar Senha
```
Ator: Usuário com Conta Ativa
Pré-condições: Esqueceu a senha
Fluxo Principal:
  1. Em /Home/Index, clica em "Esqueci a Senha"
  2. Modal: Pede Email
  3. POST /Usuarios/SolicitarReset
  4. Sistema busca usuário por email
  5. Gera Random token de 6 dígitos
  6. UPDATE Usuario SET ResetToken = @token, Expiracao = Now + 15min
  7. SendMailAsync() via Gmail SMTP
     - Email contém: "Seu código é: 123456"
  8. TempData["AbrirModalToken"] = true
  9. Modal "Confirmação de Reset" aparece
  10. Usuário digita:
      - Token (6 dígitos)
      - Nova Senha
      - Confirmação
  11. POST /Usuarios/ConfirmarReset
  12. Valida token e data de expiração
  13. UPDATE Senha = BCrypt.Hash(novaSenha)
  14. Limpa ResetToken
  15. Sucesso: "Senha alterada com sucesso"
  16. Redireciona para login
```

---

## 9. Funcionalidades Principais

### ✅ Funcionalidades Implementadas

1. **Autenticação Segura**
   - Registro com validação de email único
   - Hash BCrypt de senha
   - Cookie-based authentication
   - Upload de foto de perfil

2. **Gerenciamento de Ocorrências**
   - CRUD completo (Create, Read, Update, Delete)
   - Categorização por Tipo (Perdido, Encontrado, Rua)
   - Status (Aberto, Resolvido)
   - Coordenadas GPS (Latitude/Longitude)
   - Integração com Leaflet Maps

3. **Sistema de Comentários**
   - Comentários por ocorrência
   - Rastreamento de autor e data/hora
   - Relacionamento N:1 com Ocorrência

4. **Favoritos (Watchlist)**
   - Relação N:N entre Usuários e Ocorrências
   - Dashboard de casos favoritos
   - Toggle favoritar/desfavoritar

5. **Recuperação de Senha**
   - Token de 6 dígitos (validade 15 min)
   - Envio por email via SMTP (Gmail)
   - Fluxo seguro com validação de token
   - Atualização de senha com BCrypt

6. **Dashboard Interativo**
   - Mapa Leaflet com marcadores de casos
   - Sidebar com lista de casos
   - Filtro por urgência (em desenvolvimento)
   - Visualização responsiva

### 🔄 Em Desenvolvimento

- [ ] Urgência (filtro de casos urgentes)
- [ ] Notificações em tempo real
- [ ] Upload de múltiplas fotos por caso
- [ ] Rating/Avaliação de usuários

### 📋 Possíveis Melhorias Futuras

1. **Segurança**
   - HTTPS obrigatório em produção
   - CSRF tokens em todos os forms
   - Rate limiting em login
   - Validação de arquivo (MIME type)

2. **Performance**
   - Paginação em listas grandes
   - Cache de ocorrências frequentemente acessadas
   - Compressão de imagens

3. **UX/UI**
   - Busca e filtro avançado de casos
   - Notificações push
   - Interface mobile-first
   - Dark mode

4. **Dados**
   - Histórico de alterações em casos
   - Analytics de casos resolvidos
   - Export de relatórios

---

## 10. Configuração e Deployment

### 10.1 Requisitos Mínimos
- **.NET Runtime:** 10.0+
- **SQL Server:** 2019+
- **Node.js:** (opcional, para frontend tooling)
- **Docker:** (opcional, para containerização)

### 10.2 Ambiente Local (Desenvolvimento)
```powershell
# Clone o repositório
git clone https://github.com/vbrfernandes/Dev-PUC-SoSDog.git
cd Dev-PUC-SoSDog

# Restaurar dependências
dotnet restore

# Aplicar migrations
dotnet ef database update

# Executar
dotnet run
# Acessa: https://localhost:7xxx
```

### 10.3 Variáveis de Ambiente Necessárias
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=SosDogDB;..."
  },
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUser": "seu_email@gmail.com",
    "SmtpPassword": "senha_de_app"  // Use App Password, não senha real!
  }
}
```

### 10.4 Docker
```dockerfile
# Dockerfile já existe no projeto
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out
ENTRYPOINT ["dotnet", "out/Dev-PUC-SoSDog.dll"]
```

---

## 11. Segurança

### 11.1 Implementações Atuais

✅ **BCrypt:** Hashing de senha com salt  
✅ **Cookie Auth:** Session management segura  
✅ **Validação de Entrada:** Data annotations no Model  
✅ **Validação de Email:** Format validation  
✅ **Telefone:** Phone validation  
✅ **Token com Expiração:** Reset de senha com TTL  

### 11.2 Considerações de Segurança

⚠️ **ATENÇÃO - Credenciais Hardcoded:**
```csharp
// UsuariosController.cs - SolicitarReset()
new NetworkCredential("SEU_EMAIL@gmail.com", "SUA_SENHA_DE_APP")
// ⚠️ NÃO FAZER ISSO EM PRODUÇÃO!
// Use: Azure Key Vault, Environment Variables, ou appsettings.json encriptado
```

✅ **Recomendações:**
- Usar `Configuration["Email:SmtpPassword"]` do appsettings.json
- Em produção, usar Azure Key Vault ou AWS Secrets Manager
- Validar MIME type de uploads
- Limitar tamanho de arquivo
- Sanitizar inputs HTML

---

## 12. Resumo Executivo

| Aspecto | Descrição |
|--------|-----------|
| **Objetivo** | Plataforma colaborativa para gerenciar casos de cães perdidos/encontrados |
| **Stack** | .NET 10 + ASP.NET Core + Entity Framework Core + SQL Server |
| **Arquitetura** | MVC com Controllers, Models, Views (Razor Pages) |
| **Banco de Dados** | SQL Server LocalDB, 4 tabelas principais |
| **Autenticação** | Cookie-based com BCrypt |
| **Principais Features** | CRUD de casos, comentários, favoritos, reset de senha |
| **Status** | Em desenvolvimento ativo (v1.0) |
| **Repositório** | https://github.com/vbrfernandes/Dev-PUC-SoSDog |

---

## 13. Diagrama de Classes UML Simplificado

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         MODELS (Entities)                               │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  ┌───────────────────────────────────────────────────────────────────┐ │
│  │ <<Entity>> Usuario                                                 │ │
│  ├───────────────────────────────────────────────────────────────────┤ │
│  │ - ID_Usuario: int <<PK>>                                          │ │
│  │ - Nome: string                                                    │ │
│  │ - Email: string <<UNIQUE>>                                        │ │
│  │ - Senha: string <<BCrypt>>                                        │ │
│  │ - Foto_Perfil: string                                             │ │
│  │ - Telefone: string                                                │ │
│  │ - ResetToken: string?                                             │ │
│  │ - ResetTokenExpiracao: DateTime?                                  │ │
│  ├───────────────────────────────────────────────────────────────────┤ │
│  │ + OcorrenciasRegistradas: ICollection<Ocorrencia>                 │ │
│  │ + Comentarios: ICollection<Comentario>                            │ │
│  │ + Favoritos: ICollection<Favorito>                                │ │
│  └───────────────────────────────────────────────────────────────────┘ │
│                                   │                                     │
│                                   │ 1:N                                 │
│                                   │                                     │
│  ┌────────────────────────────────┴──────────────────────────────────┐ │
│  │ <<Entity>> Ocorrencia                                              │ │
│  ├────────────────────────────────────────────────────────────────────┤ │
│  │ - ID_Ocorrencia: int <<PK>>                                       │ │
│  │ - Tipo: string (Perdido|Encontrado|Rua)                           │ │
│  │ - Status: string (Aberto|Resolvido)                               │ │
│  │ - Foto_Animal: string?                                            │ │
│  │ - Descricao: string                                               │ │
│  │ - Latitude: float                                                 │ │
│  │ - Longitude: float                                                │ │
│  │ - Data_Registro: DateTime                                         │ │
│  │ - ID_Usuario: int <<FK>>                                          │ │
│  ├────────────────────────────────────────────────────────────────────┤ │
│  │ + Usuario: Usuario                                                │ │
│  │ + Comentarios: ICollection<Comentario> (1:N)                      │ │
│  │ + FavoritadosPor: ICollection<Favorito> (N:N)                     │ │
│  └────────────────────────────────────────────────────────────────────┘ │
│                                   │                                     │
│                ┌──────────────────┴───────────────────────┐            │
│                │ 1:N                                1:N   │            │
│                │                                        │            │
│  ┌─────────────▼─────────────────┐   ┌──────────────────┴─────────┐  │
│  │ <<Entity>> Comentario         │   │ <<Entity>> Favorito         │  │
│  ├───────────────────────────────┤   ├─────────────────────────────┤  │
│  │ - ID_Comentario: int <<PK>>   │   │ - ID_Favorito: int <<PK>>   │  │
│  │ - Texto: string               │   │ - ID_Usuario: int <<FK>>    │  │
│  │ - Data_hora: DateTime         │   │ - ID_Ocorrencia: int <<FK>> │  │
│  │ - ID_Usuario: int <<FK>>      │   ├─────────────────────────────┤  │
│  │ - ID_Ocorrencia: int <<FK>>   │   │ + Usuario: Usuario          │  │
│  ├───────────────────────────────┤   │ + Ocorrencia: Ocorrencia    │  │
│  │ + Usuario: Usuario            │   └─────────────────────────────┘  │
│  │ + Ocorrencia: Ocorrencia      │                                    │
│  └───────────────────────────────┘                                    │
│                                                                         │
│  ┌──────────────────────────────────────────────────────────────────┐ │
│  │ <<DbContext>> AppDbContext                                       │ │
│  ├──────────────────────────────────────────────────────────────────┤ │
│  │ - DbSet<Usuario> Usuarios                                        │ │
│  │ - DbSet<Ocorrencia> Ocorrencias                                  │ │
│  │ - DbSet<Comentario> Comentarios                                  │ │
│  │ - DbSet<Favorito> Favoritos                                      │ │
│  ├──────────────────────────────────────────────────────────────────┤ │
│  │ + OnModelCreating(ModelBuilder): void                            │ │
│  └──────────────────────────────────────────────────────────────────┘ │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## 14. Endpoints Principais

### Usuarios
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/Usuarios/Index` | Lista todos os usuários |
| GET | `/Usuarios/Details/{id}` | Detalhes de um usuário |
| GET | `/Usuarios/Create` | Formulário de cadastro |
| POST | `/Usuarios/Create` | Cria novo usuário |
| GET | `/Usuarios/Edit/{id}` | Formulário de edição |
| POST | `/Usuarios/Edit/{id}` | Atualiza usuário |
| POST | `/Usuarios/Login` | Autenticação |
| POST | `/Usuarios/Logout` | Desautenticação |
| POST | `/Usuarios/SolicitarReset` | Solicita reset de senha |
| POST | `/Usuarios/ConfirmarReset` | Confirma reset de senha |

### Ocorrencias
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/Ocorrencias/Index` | Lista de casos |
| GET | `/Ocorrencias/Details/{id}` | Detalhes do caso |
| GET | `/Ocorrencias/Create` | Formulário novo caso |
| POST | `/Ocorrencias/Create` | Cria novo caso |
| GET | `/Ocorrencias/Edit/{id}` | Formulário edição |
| POST | `/Ocorrencias/Edit/{id}` | Atualiza caso |
| GET | `/Ocorrencias/Delete/{id}` | Confirmação exclusão |
| POST | `/Ocorrencias/Delete/{id}` | Deleta caso |

### Comentarios
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/Comentarios/Index` | Lista de comentários |
| GET | `/Comentarios/Details/{id}` | Detalhes do comentário |
| POST | `/Comentarios/Create` | Cria novo comentário |
| GET | `/Comentarios/Edit/{id}` | Formulário edição |
| POST | `/Comentarios/Edit/{id}` | Atualiza comentário |
| GET | `/Comentarios/Delete/{id}` | Confirmação exclusão |
| POST | `/Comentarios/Delete/{id}` | Deleta comentário |

### Favoritos
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/Favoritos/Index` | Casos favoritos do usuário |
| POST | `/Favoritos/Create` | Adiciona favorito |
| GET | `/Favoritos/Delete/{id}` | Remove favorito |

### Home
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/Home/Index` | Dashboard principal com mapa |
| GET | `/Home/Privacy` | Página de privacidade |

---

## 15. Conclusão

O **SoSDog** é um projeto bem estruturado, seguindo boas práticas de desenvolvimento .NET com:

✅ **Arquitetura clara:** MVC com Controllers, Models e Views bem separados  
✅ **Banco de dados normalizado:** 4 entidades com relacionamentos bem definidos  
✅ **Segurança:** Autenticação com BCrypt, Cookie sessions  
✅ **UX moderna:** Integração com Leaflet Maps, Bootstrap responsivo  
✅ **Funcionalidades robustas:** CRUD completo, comentários, favoritos, recuperação de senha  

**Potencial:** Escalável para incluir mais funcionalidades (notificações, machine learning para match de casos, API pública, etc.)

---

**Documento gerado em:** 2025  
**Análise realizada sobre:** SoSDog v1.0  
**Status do Projeto:** Em Desenvolvimento Ativo ✅
