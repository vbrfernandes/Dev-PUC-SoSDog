# 📋 Relatório Detalhado do Projeto SoSDog

## 📌 Informações Gerais

- **Nome do Projeto:** Dev-PUC-SoSDog
- **Plataforma:** ASP.NET Core MVC / Razor Pages
- **Versão .NET:** .NET 10.0
- **Framework de BD:** Entity Framework Core 10.0.5
- **Banco de Dados:** SQL Server (LocalDB)
- **Ambiente de Desenvolvimento:** Visual Studio 2026 Community
- **Repositório Git:** https://github.com/vbrfernandes/Dev-PUC-SoSDog
- **Branch Atual:** master

---

## 🏗️ Arquitetura e Estrutura

### Padrão Arquitetural
- **Padrão:** MVC (Model-View-Controller)
- **Componentes:** Controllers, Models, Views, DbContext
- **Camada de Dados:** Entity Framework Core com SQL Server

### Estrutura de Diretórios Principais

```
Dev-PUC-SoSDog/
├── Controllers/              # Controladores MVC
│   ├── HomeController.cs
│   ├── UsuariosController.cs
│   ├── OcorrenciasController.cs
│   ├── ComentariosController.cs
│   └── FavoritosController.cs
├── Models/                   # Modelos de Dados
│   ├── AppDbContext.cs       # Context do Entity Framework
│   ├── Usuario.cs
│   ├── Ocorrencia.cs
│   ├── Comentario.cs
│   ├── Favorito.cs
│   └── ErrorViewModel.cs
├── Views/                    # Visualizações Razor
│   ├── Shared/
│   │   ├── _Layout.cshtml
│   │   ├── _Layout.cshtml.css
│   │   ├── _ValidationScriptsPartial.cshtml
│   │   └── Error.cshtml
│   ├── Home/
│   ├── Usuarios/
│   ├── Ocorrencias/
│   ├── Comentarios/
│   └── Favoritos/
├── Migrations/               # Migrações do EF Core
│   ├── 20260420000952_InitialCreate.cs
│   ├── 20260420000952_InitialCreate.Designer.cs
│   └── AppDbContextModelSnapshot.cs
├── wwwroot/                  # Arquivos estáticos
│   ├── css/
│   ├── js/
│   └── lib/                  # Bibliotecas (jQuery, Bootstrap, Leaflet)
├── Program.cs                # Configuração da aplicação
├── appsettings.json          # Configurações
├── Dev-PUC-SoSDog.csproj     # Arquivo do projeto
└── Dockerfile                # Configuração Docker

```

---

## 📦 Dependências e Pacotes

### Pacotes NuGet Instalados

| Pacote | Versão | Descrição |
|--------|--------|-----------|
| Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation | 10.0.7 | Compilação em tempo de execução de views Razor |
| Microsoft.EntityFrameworkCore | 10.0.5 | ORM para acesso a dados |
| Microsoft.EntityFrameworkCore.SqlServer | 10.0.5 | Provider SQL Server para EF Core |
| Microsoft.EntityFrameworkCore.Tools | 10.0.5 | Ferramentas de migration (CLI) |
| Microsoft.VisualStudio.Web.CodeGeneration.Design | 10.0.2 | Scaffolding de Controllers e Views |
| Microsoft.VisualStudio.Azure.Containers.Tools.Targets | 1.23.0 | Suporte a Docker |
| NuGet.Packaging | 7.3.1 | Gerenciamento de pacotes NuGet |
| NuGet.Protocol | 7.3.1 | Protocolo NuGet |

### Bibliotecas Frontend (cdnjs/jsDelivr)
- **Swiper:** 11 (Carrossel de imagens)
- **Font Awesome:** 6.4.0 (Ícones)
- **Leaflet:** 1.9.4 (Mapas interativos)
- **jQuery:** (na pasta wwwroot/lib)
- **Bootstrap:** (na pasta wwwroot/lib)
- **jQuery Validation:** (na pasta wwwroot/lib)

---

## 🗄️ Modelos de Dados (Domain Model)

### 1. **Usuario** (Usuario.cs)

#### Propriedades
```csharp
- ID_Usuario (int, PK)
- Nome (string, obrigatório, max 100 chars)
- Email (string, obrigatório, validação de email)
- Senha (string, obrigatório, DataType.Password)
- Foto_Perfil (string?, opcional)
- Data_Cadastro (DateTime, default: DateTime.Now)
- LocalizacaoAtual (string?, opcional)
- Bio (string?, opcional)
- Telefone (int?, privado com propriedade pública)
```

#### Relacionamentos
- ✅ 1:N com **Ocorrencia** (OcorrenciasRegistradas)
- ✅ 1:N com **Comentario** (Comentarios)
- ✅ 1:N com **Favorito** (Favoritos)

#### Métodos (Assinaturas)
- `CadastrarConta()` - Registrar novo usuário
- `EditarPerfil()` - Atualizar dados do perfil
- `ExcluirConta()` - Deletar conta
- `RedefinirSenha()` - Recuperação de senha
- `Logout()` - Saída do sistema

---

### 2. **Ocorrencia** (Ocorrencia.cs)

#### Propriedades
```csharp
- ID_Ocorrencia (int, PK)
- Tipo (string, obrigatório) // Ex: "Perdido", "Rua", "Adoção"
- Status (string, obrigatório) // Ex: "Aberto", "Resolvido", "Em Análise"
- Foto_Animal (string?, opcional)
- Descricao (string, obrigatório)
- Latitude (float, obrigatório)
- Longitude (float, obrigatório)
- Data_Registro (DateTime, default: DateTime.Now)
- ID_Usuario (int, FK, obrigatório)
```

#### Relacionamentos
- ✅ N:1 com **Usuario** (Usuario)
- ✅ 1:N com **Comentario** (Comentarios) - DeleteBehavior.Restrict
- ✅ 1:N com **Favorito** (FavoritadosPor) - DeleteBehavior.Restrict

#### Métodos (Assinaturas)
- `AtualizarStatus()` - Mudar status de uma ocorrência
- `RegistrarCuidados()` - Registrar cuidados do animal

---

### 3. **Comentario** (Comentario.cs)

#### Propriedades
```csharp
- ID_Comentario (int, PK)
- Texto (string, obrigatório)
- Data_hora (DateTime, default: DateTime.Now)
- ID_Usuario (int, FK, obrigatório)
- ID_Ocorrencia (int, FK, obrigatório)
```

#### Relacionamentos
- ✅ N:1 com **Usuario** (Usuario)
- ✅ N:1 com **Ocorrencia** (Ocorrencia) - DeleteBehavior.Restrict

---

### 4. **Favorito** (Favorito.cs)

#### Propriedades
```csharp
- ID_Favorito (int, PK)
- ID_Usuario (int, FK, obrigatório)
- ID_Ocorrencia (int, FK, obrigatório)
```

#### Relacionamentos
- ✅ N:1 com **Usuario** (Usuario)
- ✅ N:1 com **Ocorrencia** (Ocorrencia) - DeleteBehavior.Restrict

#### Função
Tabela de junção para marcar ocorrências como favoritas

---

### 5. **ErrorViewModel** (ErrorViewModel.cs)

Modelo padrão para tratamento de erros em views

---

## 🔗 Diagrama de Relacionamentos

```
┌─────────────────────────────────────────────────────────────┐
│                          USUARIO                             │
├─────────────────────────────────────────────────────────────┤
│ PK: ID_Usuario                                               │
│ - Nome, Email, Senha                                         │
│ - Foto_Perfil, Data_Cadastro                                │
│ - LocalizacaoAtual, Bio, Telefone                           │
└─────────────────────────────────────────────────────────────┘
    │
    │ 1:N (OcorrenciasRegistradas)
    │
    ▼
┌─────────────────────────────────────────────────────────────┐
│                      OCORRENCIA                              │
├─────────────────────────────────────────────────────────────┤
│ PK: ID_Ocorrencia                                            │
│ FK: ID_Usuario                                               │
│ - Tipo, Status, Foto_Animal                                 │
│ - Descricao, Latitude, Longitude                            │
│ - Data_Registro                                              │
└─────────────────────────────────────────────────────────────┘
    │
    ├─ 1:N (Comentarios) ──────────────────┐
    │                                        │
    └─ 1:N (FavoritadosPor)                 │
                                             │
        ┌────────────────────────────────────┼────────────────┐
        ▼                                    ▼                ▼
┌──────────────────────┐          ┌──────────────────────┐
│    COMENTARIO        │          │     FAVORITO         │
├──────────────────────┤          ├──────────────────────┤
│ PK: ID_Comentario    │          │ PK: ID_Favorito      │
│ FK: ID_Usuario       │          │ FK: ID_Usuario       │
│ FK: ID_Ocorrencia    │          │ FK: ID_Ocorrencia    │
│ - Texto              │          │                      │
│ - Data_hora          │          │                      │
└──────────────────────┘          └──────────────────────┘
        │                                    │
        └────────────────┬───────────────────┘
                         │
                         ▼
        (Relacionam-se novamente com Usuario)
```

---

## 🎮 Controllers

### 1. **HomeController.cs**

#### Responsabilidades
- Página inicial do projeto
- Listagem de ocorrências

#### Actions
- `Index()` - Exibe lista de ocorrências
- `Privacy()` - Página de privacidade

#### Dependências
- AppDbContext (injeção de dependência)
- Entity Framework Core

---

### 2. **UsuariosController.cs**

#### Responsabilidades
- CRUD de usuários
- Gerenciar dados de perfil

#### Actions Principais
- `Index()` - Lista todos os usuários
- `Details(id)` - Detalhe de um usuário
- `Create()` - Formulário de novo usuário (GET/POST)
- `Edit(id)` - Editar usuário (GET/POST)
- `Delete(id)` - Deletar usuário (GET/POST)

#### Métodos HTTP
- GET, POST, DELETE

---

### 3. **OcorrenciasController.cs**

#### Responsabilidades
- CRUD de ocorrências (casos de perdidos, rua, etc)
- Gerenciar situações de animais

#### Actions Principais
- `Index()` - Lista todas as ocorrências com usuário associado
- `Details(id)` - Detalhe de uma ocorrência
- `Create()` - Criar nova ocorrência (GET/POST)
- `Edit(id)` - Editar ocorrência (GET/POST)
- `Delete(id)` - Deletar ocorrência (GET/POST)

#### Features
- Include de dados relacionados (Usuario)
- ViewData para seleção de usuários
- Validação de antiforgery

---

### 4. **ComentariosController.cs**

#### Responsabilidades
- CRUD de comentários em ocorrências
- Gerenciar discussões sobre casos

---

### 5. **FavoritosController.cs**

#### Responsabilidades
- CRUD de favoritos
- Marcar/desmarcar ocorrências como favoritas

---

## 🎨 Views (Razor)

### Estrutura de Views

```
Views/
├── Shared/
│   ├── _Layout.cshtml          # Layout padrão (Header, Nav, Footer)
│   ├── _Layout.cshtml.css      # Estilos do layout
│   ├── _ValidationScriptsPartial.cshtml  # Scripts de validação
│   └── Error.cshtml            # Página de erro
├── _ViewImports.cshtml         # Imports globais (@using, @addTagHelper)
├── _ViewStart.cshtml           # Arquivo executado antes de cada view
├── Home/
│   ├── Index.cshtml            # Página inicial
│   └── Privacy.cshtml          # Página de privacidade
├── Usuarios/
│   ├── Index.cshtml            # Lista de usuários
│   ├── Create.cshtml           # Formulário de novo usuário
│   ├── Edit.cshtml             # Formulário de edição
│   ├── Delete.cshtml           # Confirmação de exclusão
│   └── Details.cshtml          # Detalhe de usuário
├── Ocorrencias/
│   ├── Index.cshtml            # Lista de ocorrências
│   ├── Create.cshtml           # Novo caso
│   ├── Edit.cshtml             # Editar caso
│   ├── Delete.cshtml           # Deletar caso
│   └── Details.cshtml          # Detalhe do caso
├── Comentarios/
│   ├── Index.cshtml
│   ├── Create.cshtml
│   ├── Edit.cshtml
│   ├── Delete.cshtml
│   └── Details.cshtml
└── Favoritos/
    ├── Index.cshtml
    ├── Create.cshtml
    ├── Edit.cshtml
    ├── Delete.cshtml
    └── Details.cshtml
```

### Layout Principal (_Layout.cshtml)

**Componentes:**
- ✅ Header com logo (ícone de pata)
- ✅ Navegação principal
  - Botão "Mapa dos Casos" (#mapa)
  - Botão "Feed de Casos" (#feed)
- ✅ Barra de pesquisa
- ✅ Área de login/cadastro
  - Menu flutuante para usuário logado
  - Botão "USUÁRIO" (apareça quando logado)
- ✅ RenderBody() para conteúdo dinâmico

**Bibliotecas Incluídas:**
- Swiper 11 (carrossel)
- Font Awesome 6.4.0 (ícones)
- Leaflet 1.9.4 (mapas)
- Bootstrap (responsive design)
- CSS customizado (site.css)

---

## 📊 Banco de Dados

### Configuração
- **Tipo:** SQL Server Express LocalDB
- **Connection String:** `Server=(localdb)\\mssqllocaldb;Database=SosDogDB;Trusted_Connection=True;MultipleActiveResultSets=true`
- **Context:** AppDbContext

### DbSets Registrados
```csharp
public DbSet<Usuario> Usuarios { get; set; }
public DbSet<Ocorrencia> Ocorrencias { get; set; }
public DbSet<Comentario> Comentarios { get; set; }
public DbSet<Favorito> Favoritos { get; set; }
```

### Migrações
- **Data:** 20260420000952
- **Nome:** InitialCreate
- **Status:** Aplicada

### Configurações de Relacionamentos (OnModelCreating)

**DeleteBehavior:**
- Comentario → Ocorrencia: `DeleteBehavior.Restrict`
- Favorito → Ocorrencia: `DeleteBehavior.Restrict`

**Motivo:** Evitar "Multiple Cascade Paths" - garante que a exclusão de uma ocorrência não cause conflitos com comentários e favoritos

---

## ⚙️ Configuração da Aplicação (Program.cs)

### Services Registrados
1. **Controllers com Views**
   ```csharp
   builder.Services.AddControllersWithViews();
   ```

2. **Razor Pages com Compilação em Tempo de Execução**
   ```csharp
   builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
   ```

3. **DbContext com SQL Server**
   ```csharp
   builder.Services.AddDbContext<AppDbContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
   ```

### Middleware Pipeline
```csharp
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();
```

### Rota Padrão
```
Pattern: {controller=Home}/{action=Index}/{id?}
```

---

## 📋 Configurações (appsettings.json)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SosDogDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### Pontos-Chave
- ✅ Logging configurado para ASP.NET Core em nível "Warning"
- ✅ Todos os hosts permitidos (`*`)
- ✅ MultipleActiveResultSets ativo para operações assíncronas

---

## 🚀 Recursos Frontend

### CSS e JavaScript

**wwwroot/**
- `css/site.css` - Estilos customizados
- `js/` - Scripts customizados (se houver)

### Bibliotecas Externas
- **jQuery:** Manipulação DOM e AJAX
- **jQuery Validation:** Validação de formulários
- **Bootstrap:** Framework responsivo
- **Swiper:** Carrossel de imagens
- **Font Awesome:** Ícones SVG
- **Leaflet:** Mapas interativos (integração com OpenStreetMap)

---

## 🔐 Segurança

### Implementações Atuais
- ✅ Validação de AntiForge (CSRF Protection)
- ✅ Data Annotations para validação de modelo
- ✅ String length limitations
- ✅ Email validation
- ✅ Password DataType

### Recomendações
- ⚠️ Implementar autenticação (JWT ou Identity)
- ⚠️ Implementar autorização (Claims-based ou Role-based)
- ⚠️ Hash de senhas (bcrypt, PBKDF2)
- ⚠️ HTTPS obrigatório em produção

---

## 📱 Responsive Design

- ✅ Meta viewport configurado
- ✅ Bootstrap framework
- ✅ CSS customizado responsivo
- ✅ Ícones font-awesome escaláveis

---

## 🐳 Docker

- Dockerfile presente no projeto
- Configuração para container Linux
- Pronto para deploy em container

---

## 🔄 Fluxo de Dados

```
Usuário (Browser)
    ↓
Request HTTP
    ↓
Router (Program.cs) → Controllers
    ↓
Action Method
    ↓
AppDbContext (EF Core)
    ↓
SQL Server (LocalDB)
    ↓
Response (View Razor)
    ↓
Browser (HTML + CSS + JS)
```

---

## 📌 Namespace Mismatches Identificados

⚠️ **Observação Importante:**
O projeto tem uma inconsistência de namespace:
- **Program.cs:** `namespace Dev_PUC_SoSDog`
- **Models:** `namespace SosDog.Models`
- **Controllers:** `namespace Dev_PUC_SosDog.Controllers`

Isso pode causar problemas. Recomenda-se normalizar para um único namespace raiz.

---

## 📈 Estatísticas do Projeto

| Métrica | Valor |
|---------|-------|
| Modelos | 5 (Usuario, Ocorrencia, Comentario, Favorito, ErrorViewModel) |
| Controllers | 5 (Home, Usuarios, Ocorrencias, Comentarios, Favoritos) |
| Views | 28+ arquivos Razor |
| Migrações | 1 (InitialCreate) |
| Pacotes NuGet | 8 principais |
| DbSets | 4 |
| Tabelas Relacionadas | 4 tabelas com 7 relacionamentos |

---

## 🎯 Funcionalidades Principais

1. **Gerenciamento de Usuários**
   - Cadastro, login, perfil
   - Bio e localização

2. **Registro de Ocorrências**
   - Criar casos de animais perdidos/rua
   - Geolocalização (latitude/longitude)
   - Upload de fotos
   - Status do caso

3. **Sistema de Comentários**
   - Comentar em casos
   - Atribuição de autor
   - Timestamp

4. **Sistema de Favoritos**
   - Marcar casos como favoritos
   - Acompanhamento de casos

5. **Mapa Interativo**
   - Visualização geográfica de casos (Leaflet)
   - Integração com coordenadas de ocorrências

6. **Feed de Casos**
   - Visualização de todos os casos
   - Filtros e buscas

---

## ✅ Checklist de Conclusões

- ✅ Arquitetura MVC bem definida
- ✅ Entity Framework Core configurado
- ✅ Relacionamentos entre entidades implementados
- ✅ Views Razor com layouts reutilizáveis
- ✅ CRUD básico implementado para todas as entidades
- ✅ Frontend com Bootstrap e bibliotecas atualizadas
- ⚠️ Autenticação/Autorização não implementada
- ⚠️ Hash de senhas não implementado
- ⚠️ Validação de imagens não implementada
- ⚠️ Tratamento de exceções precisa ser melhorado
- ⚠️ Testes unitários não encontrados

---

## 📚 Próximos Passos Recomendados

1. Implementar autenticação (ASP.NET Core Identity)
2. Adicionar autorização baseada em roles/claims
3. Hash de senhas com bcrypt
4. Validação de uploads de imagens
5. Tratamento robusto de exceções
6. Logging centralizado
7. Testes unitários e de integração
8. API REST para mobile (opcional)
9. Cache de dados frequentes
10. Paginação em listas grandes

---

*Relatório Gerado em:* `2024`
*Projeto:* SoSDog - Plataforma de Adoção e Resgate de Animais
*Versão:* 1.0.0
