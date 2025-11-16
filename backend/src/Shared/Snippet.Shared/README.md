# Snippet.Shared

Biblioteka współdzielona zawierająca kod używany przez wszystkie projekty w rozwiązaniu Snippet (z wyjątkiem projektów Domain).

### Planowane dodatki

- **Contracts**: Kontrakty komunikacji między modułami
- **Api**: Wspólne interfejsy API do komunikacji międzymodułowej
- **Extensions**: Metody rozszerzające współdzielone między projektami
- **Constants**: Stałe globalne

## Zasady użycia

- ✅ **Może być używana przez**: Application, Infrastructure, Web
- ❌ **NIE MOŻE być używana przez**: Domain (zachowanie czystości domeny)

## Referencje

Ta biblioteka nie ma zależności od innych projektów w rozwiązaniu Snippet, co zapewnia jej uniwersalność i brak cyklicznych zależności.
