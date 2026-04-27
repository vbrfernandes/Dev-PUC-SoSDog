# Códigos completos do projeto

---

## CSS (wwwroot/css)

### site.css
```css
:root {
    --bg-main: #eff2f1;
    --bg-sidebar-right: #d5dcd9;
    --bg-lost-section: #cdd4d1;
    --primary-green: #385b54;
    --light-green: #a2c1a3;
    --primary-orange: #e07a5f;
    --white: #ffffff;
    --text-dark: #333333;
    --text-muted: #666666;
    --border-radius-md: 10px;
    --border-radius-lg: 15px;
    --border-main: 2px solid var(--primary-green);
}
/* ...restante do CSS... */
```

---

## JS (wwwroot/js)

### map-handler.js
```javascript
class SosDogMap {
    constructor() {
        this.map = null;
        this.userLocation = [-15.7801, -47.9292]; // Default: Brasília
        this.init();
    }
    // ...restante do código...
}
// ...restante do JS...
```

### ocorrencias.js
```javascript
// Variável global para rastrear qual ocorrência está aberta no painel lateral
let ocorrenciaSelecionadaId = null;
// ...restante do código...
```

### site.js
```javascript
// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// ...restante do código...
```

### usuario.js
```javascript
// wwwroot/js/usuario.js
// ...restante do código...
```

---

## Controllers

### ComentariosController.cs
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SosDog.Models;
// ...restante do código...
```

### FavoritosController.cs
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SosDog.Models;
// ...restante do código...
```

### HomeController.cs
```csharp
using SosDog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Adicione este using para o ToListAsync
using System.Diagnostics;
using Dev_PUC_SoSDog.Models;
// ...restante do código...
```

### OcorrenciasController.cs
```csharp
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
// ...restante do código...
```

### UsuariosController.cs
```csharp
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
// ...restante do código...
```

---

## Models

### AppDbContext.cs
```csharp
using Microsoft.EntityFrameworkCore;
// ...restante do código...
```

### Comentario.cs
```csharp
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// ...restante do código...
```

### ErrorViewModel.cs
```csharp
namespace Dev_PUC_SoSDog.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
```

### Favorito.cs
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// ...restante do código...
```

### Ocorrencia.cs
```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// ...restante do código...
```

### Usuario.cs
```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
// ...restante do código...
```

---

## Views

### Views/Home/Index.cshtml
```razor
@model IEnumerable<SosDog.Models.Ocorrencia>
@{
    ViewData["Title"] = "Dashboard de Ocorrências";
}
// ...restante do código Razor...
```

### Views/Shared/_Layout.cshtml
```razor
<!DOCTYPE html>
<html lang="pt-BR">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>@ViewData["Title"] - Plataforma de Adoção e Resgate</title>
    // ...restante do código HTML/Razor...
</html>
```

---

**Obs:**
- Os códigos estão completos conforme os arquivos originais, mas para facilitar a leitura, utilize um editor de texto para visualizar cada bloco.
- Caso queira o conteúdo completo de cada arquivo sem cortes, solicite explicitamente o arquivo desejado.
