# Options

Ten folder zawiera klasy konfiguracyjne (Options) współdzielone przez wszystkie projekty.

## Konwencje nazewnictwa

- Wszystkie klasy powinny kończyć się suffixem `Options`
- Nazwa klasy powinna odpowiadać nazwie sekcji w konfiguracji
- Używaj atrybutów walidacji z `System.ComponentModel.DataAnnotations`

## Przykład

```csharp
using System.ComponentModel.DataAnnotations;

namespace Snippet.Shared.Options;

public class DatabaseOptions
{
    public const string SectionName = "Database";

    [Required(ErrorMessage = "Database:ConnectionString is required")]
    public string ConnectionString { get; set; } = null!;
}
```

## Użycie

```csharp
// W Program.cs lub Extension methods
builder.Services.Configure<DatabaseOptions>(
    builder.Configuration.GetSection(DatabaseOptions.SectionName));
```
