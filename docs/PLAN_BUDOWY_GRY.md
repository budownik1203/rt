# Plan budowy gry: Godot 4 + C#

## Zasady architektury

- **Dwa projekty:**
  - `Latarnia.Sim`: czysta biblioteka C#, .NET 8, bez zależności od Godota;
  - `Latarnia.Game`: projekt Godota, który tylko rysuje, odtwarza dźwięk i obsługuje interfejs.
- **Symulacja deterministyczna:**
  - stały krok (np. 20 ticków na sekundę), własny RNG z ziarnem, bez `float` zależnego od platformy w krytycznych miejscach (albo stałoprzecinkowo);
  - walki rozstrzygane w tle i testy balansu (tysiące walk bez rysowania) używają tej samej symulacji.
- **Dane w plikach** (JSON albo YAML): broń, pancerze, klasy i szkoły, mutanty, prawa, technologie, budynki, moduły map. Edytowalne bez kodu.
- **Wątki:** AI dowódców w osobnych wątkach, a cykle decyzji rozłożone na klatki.
- **Zapis gry:** serializacja stanu symulacji.
- **Testy:** projekt `Latarnia.Sim.Tests` (xUnit). Do tego scenariusze balansu, np. dowódca 2 kontra 4 w tej samej sytuacji, 1000 powtórzeń.

## Struktura folderu `game/`

```
game/
  Latarnia.sln
  sim/Latarnia.Sim/         Combat/, AI/, Units/, Settlement/, Politics/, Tech/, World/, Data/
  sim/Latarnia.Sim.Tests/
  godot/                    project.godot, scenes/, scripts/ (C#), shaders/, assets/ (z .glb), ui/
  data/                     *.json z danymi gry
  tools/                    konwertery, symulator balansu z linii poleceń
```

## Etapy

### Etap 1: prototyp walki (najważniejszy, sprawdza rdzeń gry)

Cel: sprawdzić, czy oglądanie walki składu z dowódcą wciąga.

1. **Symulacja walki:**
   - siatka 0,5 m na jednej mapie: fragment osady z palisadą i bramą zbudowany z tych samych budynków co osada, bo nocna obrona toczy się na mapie osady;
   - postacie ze statystykami, ranami, morale, przyduszeniem, zmęczeniem;
   - balistyka z rozrzutem, pełny ogień bratobójczy, osłony.
2. **Broń epoki Złomu i Kuźni** (6–8 rodzajów) według tabeli broni w koncepcie, plus sloty i broń boczna.
3. **Cztery mutanty z pełnym zachowaniem:** Szczurza Fala, Ogar, Krzykacz, Nosiciel.
4. **AI dowódcy** w pięciu krokach z konceptu: zagrożenia, zadania, przydział, rozkaz tylko gdy ma sens (histereza), rozkazy nazwane.
   - Poziom 1–3 i trzy specjalizacje.
   - Formacje: klin, rząd, mur tarcz, jeż. Drile: kontakt, flanka.
5. **AI wojownika:** wyszkolenie taktyczne 1–5 i zachowanie klasy (tarczownik, strzelec, sanitariusz, zwiadowca).
6. **Widok w Godocie:**
   - modele `.glb` i animacje;
   - shader pixel art (cel-shading, kontur, paleta, render w niskiej rozdzielczości);
   - kamera po przekątnej z przybliżaniem i obrotem;
   - dymki rozkazów, nakładki, tempo i pauza, flara.
7. **Raport po walce:** oś czasu zdarzeń, ocena dowódcy według specjalizacji, wkład klas.
8. **Narzędzie balansu:** uruchomienie N walk bez rysowania i statystyki.

### Etap 2: mała osada

- Osada z latarnią i paliwem (promień światła), około 10 ludzi, budowa w kręgu światła.
- Kilka budynków, panel budynku po kliknięciu (ludzie, produkcja, rozbudowa).
- Kartka zaopatrzenia, czas z pauzą (aktywna pauza), alarmy.
- Nocna obrona, która uruchamia walkę z etapu 1, i jedna wyprawa.
- Ludzie: akta z ukrytymi cechami, które odkrywają się w walce, i pierwsze szkoły (walka wręcz, strzelecka, zwiad, sanitarna).

### Etap 3: rozwój

- Kompleks szkoleniowy (11 szkół, drzewka klas, przeszkolenie, druga kwalifikacja, komisarz).
- Technologie i epoki: drugie i trzecie, z odpowiednikami sprzętu szkół.
- Prawa, osie i ustroje, Legitymacja, Ratusz.
- Mapa świata: węzły, wyprawy, karawany, kilka składów naraz.

### Etap 4: świat

- Frakcje ludzkie (11) z dyplomacją, Stworzenia Mgły, ludzie zmienieni, Przypływy, mechanika „Mgła się uczy”.
- Standaryzacja i Wzory, kolumny pod oficerem, skala mikropaństwa.
- Wydarzenia, kronika, zakończenia.

### Etap 5: treść i szlif

- Wszystkie 7 epok, budynki epok (osobne modele), pełne animacje, dźwięk, balans kampanii na około 80 godzin.

## Otwarte decyzje techniczne (zapytać użytkownika)

- ~~Siatka czy swobodny ruch~~: ustalone 8.11, siatka 0,5 m do logiki i płynny ruch w widoku, kratka osady 2 m, zasięgi broni w metrach.
- Format danych: JSON czy YAML.
- Czy prototyp walki ma od razu działać z modelami 3D, czy najpierw z prostymi kształtami (proponowane: najpierw kształty, żeby szybko sprawdzić AI).
