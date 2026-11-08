# Latarnia: prompt startowy dla Claude

Przejmujesz projekt gry **Latarnia** od poprzedniej instancji Claude, która pracowała z tym samym użytkownikiem na innym koncie. Nie masz dostępu do tamtych rozmów ani artefaktów na claude.ai. **Cała potrzebna wiedza jest w tym folderze.** Zanim cokolwiek zrobisz, przeczytaj pliki w tej kolejności:

1. `CLAUDE.md` (ten plik): kim jest użytkownik, jak z nim pracować, stan projektu.
1a. `RANO_DO_SPRAWDZENIA.md`: co jest do potwierdzenia po pracy z nocy 6/7 listopada.
2. `docs/KONCEPT_Latarnia.md`: pełny dokument koncepcyjny (około 100 KB). To źródło prawdy o grze.
2c. `docs/ANALIZA.md` i `docs/PYTANIA.md`: analiza z propozycjami zmian i pytania do przejścia z użytkownikiem. Najbliższa rozmowa powinna zacząć się od pytań ★.
2b. `docs/TECHNIKA_AI_I_MECHANIKI.md`: JAK zbudować grę technicznie: architektura C#, AI dowódcy w 5 krokach z kodem, instynkt wojownika, balistyka, osada, mapa, shadery w Godocie, testy. Czytać przed pisaniem kodu.
2a. `docs/EPOKI.md`: wszystkie 7 epok w pełni (technologie w fazach, wybory, budynki, sprzęt, surowce, społeczeństwo). Druga zakładka dokumentu koncepcyjnego.
3. `docs/HISTORIA.md`: jak projekt powstawał, kolejne decyzje i odrzucone pomysły.
4. `docs/DO_ZROBIENIA.md`: otwarte tematy koncepcyjne, graficzne i interfejsu.
5. `docs/PLAN_BUDOWY_GRY.md`: architektura i kolejność budowy gry w Godot 4 + C#.
6. `docs/PIPELINE_GRAFIKI.md`: jak powstaje grafika (Blender → pixel art) i jak jej kod jest zorganizowany.

## Użytkownik

- Projektant gier z Polski. **Pisz po polsku.** Często dyktuje głosem, więc w wiadomościach zdarzają się przekręcone słowa: „prąd” to „prompt”, „Cloud” to „Claude”, „Mountain Blade” to „Mount & Blade”. Rozumiej intencję.
- Lubi gry: Mount & Blade, Warhammer 40k (np. komisarze), RimWorld, Frostpunk, Darkest Dungeon.
- Myśli szeroko i szybko dorzuca nowe systemy. Twoim zadaniem jest układać je spójnie, wskazywać konflikty i proponować uproszczenia, ale bez rozmywania jego wizji.

### Jak z nim pracować (zapisane preferencje)

- **Przed rozpoczęciem większej pracy zadawaj pytania doprecyzowujące jako wybór wielokrotny w interaktywnym formularzu** (narzędzie do pytań z opcjami), nie zwykłym tekstem. Maksymalnie 2–4 pytania naraz, z rekomendowaną opcją na pierwszym miejscu.
- Gdy mówi „dalej”, „lecimy”, „pracuj dalej”, kontynuuj z następnym tematem. Gdy mówi „daj mi od razu kolejne kwestie do ustalenia”, kończ odpowiedź listą następnych decyzji.
- Gdy idzie spać i każe działać samodzielnie, pracuj bez czekania: decyzje, które wymagają jego wyboru, pomiń i zapisz w dokumencie jako „do decyzji”.
- Gdy pisze „czekaj na moje potwierdzenie”, zatrzymaj się i nic nie zaczynaj.
- **Poprzeczka jakości jest wysoka.** Grafika ma wyglądać jak współczesna gra, „nie jak Roblox”:
  - proporcje postaci poprawne, a broń trzymana w dłoniach;
  - warstwy ubrań i pancerza nie przechodzą przez siebie;
  - animacje z zasadami animacji (antycypacja, odrzut, przesada).
  Sprawdzaj swoje rendery i bądź szczery, co wyszło słabo.
- Lubi szczerą ocenę i propozycje zmian („przeanalizuj i zasugeruj”).
- Interfejs ma być diegetyczny („papier na krawędziach”), ale czytelny, i nic nie może na siebie nachodzić.

## Gra w trzech zdaniach

Latarnia to gra o zarządzaniu osadą post-apo w fikcyjnej Europie Środkowej, gdzie świat zalewa Mgła z mutantami, a jedyną ochroną jest światło przedwojennej latarni. Gracz buduje osadę w kręgu światła, uchwala prawa, które przesuwają ustrój (komuna, rada, junta, reżim Latarnika, Kościół Mgły, Wolne Miasto), i rozwija ludzi przez szkoły i klasy. W walce gracz nie steruje jednostkami, tylko ogląda autobitwy składów prowadzonych przez dowódców z AI zależną od ich wyszkolenia, a potem czyta raport.

## Przyjęte decyzje techniczne

- **Silnik:** Godot 4 z C#. Symulacja (walka, AI, ekonomia, polityka) jest w czystym, deterministycznym C# ze stałym krokiem czasu i oddzielona od silnika. Godot tylko rysuje i obsługuje interfejs. Wersja na desktop (Windows i Linux).
- **Grafika:** low-poly 3D renderowane jako pixel art z cel-shadingiem, konturem 1 px, ograniczoną paletą i ditheringiem w Mgle. Kamera po przekątnej (nachylenie około 52°), z przybliżaniem i dowolnym obrotem postaci.
  - Docelowo render na żywo w Godocie: shader cel-shadingu i konturu oraz renderowanie do małej rozdzielczości z powiększeniem bez wygładzania.
  - Obecne pokazy są wypiekane w Blenderze, a algorytmy postprocesu opisuje `docs/PIPELINE_GRAFIKI.md`.
- **Animacje:** 12 klatek na sekundę.
- **Pierwszy grywalny wycinek:** sama walka. Skład z dowódcą kontra mutanty na jednej mapie, z dymkami rozkazów i raportem po walce.

## Stan projektu (6 listopada 2026)

Zrobione:

- **Koncept:** około 30 sekcji (patrz `docs/KONCEPT_Latarnia.md`), plus zakładka Epoki (`docs/EPOKI.md`). Ustalenia z 7 listopada:
  - epoka to około roku gry (60 dni) bez limitów czasu, kampania około 80 godzin;
  - przełom wymaga technologii, odkrycia, próby, a potem przychodzi odpowiedź Mgły;
  - jedna lista umiejętności;
  - komisarz zależny od ustroju;
  - 7 ekranów;
  - mapa w dwóch warstwach;
  - budynki z zestawu części post-apo;
  - ekonomia pierwszych 10 dni policzona (walka o przetrwanie, paliwo i jedzenie po równo).
  
  Najnowsze zmiany to klasy i szkoły w drzewkach jak w Mount & Blade, szkoła komisarzy, ukryte cechy oraz nowi wrogowie i 11 frakcji.
- **Grafika 3D (Blender):**
  - postacie z ubiorem warstwowym i bronią z 7 epok (19 rodzajów);
  - animacje ludzi, przejścia i reakcje, mutanty;
  - 39 budynków z wczesnych epok;
  - 11 frakcji (82 jednostki ze znakami);
  - nowi wrogowie;
  - wszystko wyeksportowane do `.glb` (`assets/latarnia_modele_godot.zip`).
- **Makiety interfejsu:** 7 klikalnych ekranów: Osada, Ludzie, Kompleks, Walka, Raport, Mapa, Ratusz (`assets/latarnia_makiety_ui.zip`). Użytkownik zgłosił do nich uwagi, które są w `docs/DO_ZROBIENIA.md`.

**Nie ma jeszcze ani linijki kodu gry w Godocie.**

## Zawartość folderu

```
CLAUDE.md                         ten plik
README.md                         krótki opis dla użytkownika
docs/KONCEPT_Latarnia.md          pełny dokument koncepcyjny
docs/EPOKI.md                     zawartość wszystkich 7 epok
docs/TECHNIKA_AI_I_MECHANIKI.md   architektura, AI, mechaniki w kodzie
docs/BUDYNKI.md                   budynki: cykl epok i opisy wyglądu każdej wersji (gotowe prompty)
docs/WROGOWIE.md                  bestiariusz w liczbach, scalone duble, nowi wrogowie, adaptacje Mgły
docs/BRON.md                      broń wszystkich epok: wersje, dodatki, amunicja, przeróbki
docs/WYDARZENIA.md                wydarzenia 41–140 (z UZUPELNIENIA i konceptem razem około 155)
docs/KLASY.md                     52 klasy 11 szkół: opisy, sprzęt, zdolności z liczbami, warunki awansu
docs/UZUPELNIENIA.md              zima i dni 11–36, ukryte cechy, klasy w liczbach, tempo badań, 40 nowych wydarzeń, postacie
docs/PYTANIA.md                   162 pytania do ustalenia z propozycjami (zacząć od ★)
docs/ANALIZA.md                   analiza projektu: ryzyka, duble, propozycje zmian
docs/DECYZJE_2026-11-08.md      decyzje z 8.11 (co wpisane, co czeka)
docs/USTROJE.md                   6 osi, efekty biegunów, 64 nazwy ustrojów
docs/OS_OPIEKA_SELEKCJA.md        szósta oś w szczegółach
docs/OS_SZOSTA.md                 20 kandydatek na szóstą oś (zapas mechanik na prawa)
docs/ANALIZA_KRYTYCZNA.md         druga analiza (8.11): sprzeczności, błędy symulacji, tempo badań, ryzyka modeli, pytania
docs/HISTORIA.md                  historia projektu i decyzji
narzedzia/ekonomia_sim2.py        symulacja ekonomii dni 1–36 (jedyne źródło liczb ekonomii); stare wersje w narzedzia/stare/
narzedzia/setup_srodowiska.sh     instalacja Godota, .NET, Blendera w nowej sesji
docs/DO_ZROBIENIA.md              lista otwartych zadań
docs/PLAN_BUDOWY_GRY.md           architektura i kamienie milowe w Godocie
docs/PIPELINE_GRAFIKI.md          jak powstaje grafika, konwencje kodu Blendera
assets/latarnia_modele_godot.zip  modele .glb (postacie z animacjami, frakcje, wrogowie, budynki, broń) + README
assets/latarnia_zrodla_grafiki.zip skrypty Pythona (Blender bpy) i emblematy
assets/latarnia_grafiki_pokazowe.zip GIF-y i plansze (pokazy animacji, osada, bitwa, frakcje, wrogowie)
assets/latarnia_makiety_ui.zip    kod makiet interfejsu (.dc.html), generator i obrazy
```

Rozpakuj archiwa do `assets/` przed pracą. Kod gry Godot zakładaj w `game/` (patrz plan).

## Pierwsze kroki dla Ciebie

1. Przeczytaj dokumenty z listy na górze.
2. Zapytaj użytkownika w formularzu, czym chce zacząć. Najpewniej będzie to jedno z trzech:
   - dopracowanie konceptu (`DO_ZROBIENIA.md`, część A);
   - poprawki grafiki i interfejsu (część B i C);
   - start prototypu walki w Godocie (`PLAN_BUDOWY_GRY.md`, etap 1).
3. Pracuj w tym folderze. Dokument koncepcyjny traktuj jako żywy: zmiany ustalone z użytkownikiem wpisuj od razu do `docs/KONCEPT_Latarnia.md`, a decyzje dopisuj do `docs/HISTORIA.md`.


## Zasada pracy: zip po każdej zmianie

Użytkownik pracuje na kończącym się limicie. Po każdej większej zmianie zrób zip całego folderu (`Latarnia_paczka.zip`) i wyślij go do pobrania, żeby mógł podmienić folder na komputerze i kontynuować w nowym czacie.

## Uwaga o dokumencie online

Od 7 listopada pliki w `docs/` są nowsze niż dokument online (claude.ai). Nowe pliki (TECHNIKA, ANALIZA, PYTANIA, BUDYNKI, WROGOWIE, BRON) istnieją tylko w folderze. Źródłem prawdy jest folder.


## Środowisko narzędzi (od 8 listopada)

Sieć w ustawieniach konta jest włączona. Na starcie nowej sesji uruchom `bash narzedzia/setup_srodowiska.sh`, który instaluje Godota 4.7.2 .NET, .NET 8 SDK, Blendera (`bpy` 5.2.3) i xvfb. Render z Godota bez monitora robi się przez `xvfb-run -a godot --rendering-driver opengl3`. Skrypty grafiki z `assets/latarnia_zrodla_grafiki.zip` mają na sztywno ścieżkę `/home/claude/latarnia3d`, więc rozpakuj je tam albo popraw ścieżki.
