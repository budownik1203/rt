# Analiza krytyczna projektu Latarnia (8 listopada 2026)

Ten plik to druga, ostrzejsza analiza po `ANALIZA.md`, zrobiona przez nowego Claude po przejęciu projektu. `ANALIZA.md` opisuje głównie ryzyka projektowe. Ten plik dokłada rzeczy sprawdzone w praktyce:

- uruchomione obie symulacje ekonomii;
- policzone koszty badań ze wszystkich epok;
- przejrzane pliki `.glb`;
- porównane dokumenty między sobą.

Każdy punkt ma postać: problem, dowód i propozycja.

---

## 1. Narzędzia i technologie użyte w projekcie

| Obszar | Narzędzie | Stan | Uwagi |
| --- | --- | --- | --- |
| Silnik gry | Godot 4 + C# (.NET 8) | wybrany, **nic nie zbudowane** | brak `game/`, brak projektu Godota |
| Symulacja | czysty C#, xUnit do testów | tylko w planie | — |
| Grafika 3D | Blender jako moduł Pythona (`bpy` 5.x), Cycles na CPU | 57 skryptów, około 5700 linii | ścieżki na sztywno `/home/claude/latarnia3d` i czcionki DejaVu z Linuksa: **skrypty nie zadziałają u Ciebie na Windowsie bez poprawek** |
| Postproces pixel art | Python: numpy, PIL, OpenEXR (`post.py`) | działa w Blenderze | docelowo musi go zastąpić shader w Godocie, a tego nikt jeszcze nie sprawdził |
| Eksport | glTF binarny `.glb` | 214 plików, 17 MB | szczegóły w punkcie 5 |
| Makiety UI | jedna strona HTML/CSS/JS (`ui/interfejs_v2.html`) plus generator w Pythonie (`gen.py`) | gotowe | to makieta przeglądarkowa, nie interfejs w Godocie; przy przenoszeniu trzeba go zbudować od nowa w systemie Control |
| Ekonomia | Python (`ekonomia_sim.py`, `ekonomia_zima_sim.py`) | działa | wyniki są niewiarygodne, szczegóły w punkcie 3 |
| Dokumentacja | Markdown, około 335 KB w 17 plikach | bardzo rozbudowana | — |

**Wniosek:** wszystkie aktywa poza dokumentami powstały w chmurowym środowisku poprzedniego Claude i nie są przygotowane do pracy na Twoim komputerze. Nie ma jeszcze żadnego potoku, który dałoby się uruchomić u Ciebie.

---

## 2. Sprzeczności w dokumentach (błędy do poprawy)

### 2.1. Dwa różne systemy wejścia w nową epokę (poważne)

- **Koncept, tabela „Progi epok”:** do Kuźni wchodzi się przez Dzidy i włócznie, Studnię, Ratusz i 6 technologii.
- **Koncept, sekcja „Przełom” i `EPOKI.md`:** do Kuźni wchodzi się przez Mielerz, Glinę ogniotrwałą, Miech, odkrycie z wyprawy i udany wytop w Dymarce.

To dwa różne systemy i oba są w dokumencie. Tak samo w każdej kolejnej epoce.

**Do tego stare progi wymagają technologii, które w `EPOKI.md` są wyborami „albo”:**

- Kwarantanna (albo Izba przemiany);
- Szpital (albo Lazaret);
- Saletrzarnia (albo Kopalnia).

Gracz, który wybrał Izbę przemiany, **nigdy nie wszedłby w Czarny proch**.

Pojawiają się też nazwy, których nie ma w `EPOKI.md`: Prądnica, Badanie mutacji, Mosiężne łuski.

**Propozycja:** usunąć tabelę „Progi epok” i skrót technologii z konceptu, a zostawić odsyłacz do `EPOKI.md`.

### 2.2. Sekcja „Oprawa graficzna” wciąż opisuje grę 2D

Historia mówi, że sprzeczności 2D i 3D poprawiono, ale w koncepcie zostało sporo starego opisu:

- „kafel 32×32”;
- „sprite 32×32 składany z warstw”;
- „wzrost około 22 px”;
- „chód na 6 klatkach” (a w specyfikacji animacji jest 12);
- „oświetlenie 2D z cieniami rzucanymi przez ściany”;
- tabela „co wychodzi dobrze” o sprite'ach;
- „Godot: dobre 2D”;
- portrety 32×32.

Rozdzielczość też się nie zgadza: koncept podaje **960×540**, a `PIPELINE_GRAFIKI.md` podaje **640×360**.

Opis modeli twierdzi, że nowsze epoki to „te same bryły z innymi materiałami”, a decyzja z 6 listopada mówi o osobnych modelach.

**Propozycja:** przepisać tę sekcję od zera pod 3D renderowane jako pixel art.

### 2.3. Kampania: 50 czy 80 godzin

`PLAN_BUDOWY_GRY.md` (etap 5) mówi o kampanii na około 50 godzin, a koncept o około 80 godzinach.

### 2.4. Liczby ekonomii w trzech wersjach

| Wartość | Koncept | `EPOKI.md` | Symulacja zimy |
| --- | --- | --- | --- |
| Smolarnia | 8 drewna na 4 paliwa (2:1), do 4 paliwa na osobę | 2:1 | 3 drewna na 2 paliwa, do 6 na osobę (komentarz w kodzie mówi o 8 drewnie, a kod liczy 9) |
| Szałas zimą | 3 drewna | 3 | 2,5 |
| Barak zimą | — | 2 | 1,5 |
| Latarnia w dzień | ×0,5 | — | ×0,25 |
| Drwal | 10 | 10 (zimą 7) | 12 (zimą 8) |

Zmiany z symulacji zimy czekają na Twoją zgodę (`[zmiana]`). Do tego czasu tabela 10 dni w koncepcie i symulacja zimy liczą inną grę.

### 2.5. Drobniejsze

- **Ogar** jest zwykłym psem, ale sekcja „Co zabija mutanty” pisze, że jego „chityna odbija nóż”, a tabela surowców podaje chitynę z Ogara.
- **Pracownia:** koncept mówi, że punkty nauki dają naukowcy w Pracowni, ale Pracowni nie ma na liście budynków Złomu ani wśród znanych od startu. `UZUPELNIENIA.md` ratuje to „stołem w latarni”.
- **Plac ćwiczeń:** w tabeli technologii jest w Kuźni, a w innych miejscach jest znany od startu.
- **Drwal:** praca daje +1 Odmienności dziennie, a budynek Wyrąb daje „+1 Odmienności dziennie”. Nie wiadomo, czy to się sumuje.
- **Dwa prawa o egzekucjach:** „Egzekucje publiczne” (W +20, †) i dekret „Rozstrzeliwanie na miejscu” (W +15). Nie wiadomo, jak się mają do siebie.

---

## 3. Symulacje ekonomii: co naprawdę pokazują

Obie symulacje uruchomiłem.

### 3.1. `ekonomia_sim.py` (dni 1–10)

- **Plan nie robi tego, co opisuje koncept.** Pierwszego dnia 3 budowniczych daje 30 roboczogodzin. Smolarnia i Kuchnia zużywają 26, a na wrak zostają 4 z potrzebnych 6. Wrak nie jest więc rozebrany pierwszego dnia, a złom spada do 0.
- **Wraków można rozebrać dowolnie dużo.** Symulacja nie sprawdza, ile ruin jest naprawdę (start ma 3 wraki). Dobry plan akurat mieści się w limicie, ale to przypadek.
- **Nadmiarowe przydziały znikają po cichu.** Gdy plan przydzieli więcej ludzi, niż jest, kod ucina drwali, czyli inną pracę, niż gracz wybrał.
- **Dobry plan ma zimne noce.** Drewno spada do 0 w 5. i 10. dniu. Koncept tego nie mówi.
- **Symulacja w ogóle nie liczy:**
  - wody (start 15 wody przy zużyciu 10 dziennie, więc bez naprawy pompy woda kończy się w 2. dniu);
  - Odmienności zbieraczy i drwali;
  - ocalałych z wypraw (15%);
  - śmierci;
  - dodatkowych wrogów przy wyższej mocy latarni.
- **Morale nie ma skutków.** Zły plan ma morale 0 od 5. dnia i nic się nie dzieje. Opis „zły upada do 4. dnia” nie wynika z kodu.

### 3.2. `ekonomia_zima_sim.py` (dni 1–36)

To ważniejszy plik, bo ma zastąpić liczby w koncepcie.

- **Pokazowy przebieg to katastrofa dobrego gracza.** Skrypt drukuje przebieg „dobry, ziarno 1”:
  - ludzie spadają z 12 do 3;
  - morale spada do 0 od 21. dnia;
  - drewno jest na zerze od 17. dnia;
  - w magazynie leży przy tym około 65 paliwa.

  Plan trzyma rezerwę paliwa i obsadza smolarzy, ale nie ma ludzi na drwali, więc ludzie zamarzają obok pełnego składu paliwa. **Symulacja testuje słabą heurystykę planu, a nie ekonomię gry.**
- **Średni plan wypada lepiej niż dobry:**

  | Plan | Ludzi na końcu | Morale |
  | --- | --- | --- |
  | średni | 19,4 | 87 |
  | dobry | 18,0 | 69 |

  Albo plany są źle nazwane, albo „dobra” strategia (odmawianie przybyszom, rezerwa paliwa) jest w tej ekonomii zła.
- **Zła gra nie karze.** Zły plan kończy z 14,7 osobami, czyli więcej niż na starcie, mimo ciemności w 100% gier i morale 0. Przybysze przychodzą do osady w ciemności, a morale 0 nie ma żadnego skutku.
- **Dobry gracz traci średnio 1,8 osoby,** a tylko 39% gier kończy się bez zgonu. Może to być zamierzone (jak we Frostpunku), ale nigdzie tego nie zapisano jako decyzji.

**Propozycja:** zanim liczby z symulacji zimy trafią do konceptu:

- przepisać ją porządnie: limit ruin, woda, skutki morale (strajk, ucieczka, bunt), przybysze tylko do światła, priorytet ogrzewania nad rezerwą paliwa;
- dodać zachłannego „rozsądnego gracza” zamiast trzech ręcznych heurystyk.

Docelowo ekonomia powinna być w C# w `Latarnia.Sim` i testowana tym samym kodem co gra. Dziś Python i koncept rozjeżdżają się przy każdej zmianie.

---

## 4. Tempo badań się nie domyka (poważne dla kampanii)

Policzyłem sumę kosztów technologii z `EPOKI.md` i porównałem ją z tempem z `UZUPELNIENIA.md` (punkty na badacza: Nauka × 2 z premią epoki, typowa liczba badaczy, plany z wypraw dające 40% kosztu).

| Epoka | Suma kosztów | Potrzeba punktów na dzień (60 dni, 60% z badań) | Daje typowa obsada (Nauka 5) | Epoka trwa |
| --- | --- | --- | --- | --- |
| Złom | 1910 | około 19 | 20–30 | 40–60 dni ✓ |
| Kuźnia | 5155 | około 52 | 37–50 | około 70 dni |
| Czarny proch | 10 700 | około 107 | 50–62 | **około 110 dni** |
| Para | 15 690 | około 157 | 75–105 | **około 105 dni** |
| Nabój | 27 630 | około 276 | 90–120 | **około 150 dni** |
| Silnik | 39 000 | około 390 | 140–175 | **około 150 dni** |
| Światło | 43 800 | około 438 | 200–240 | **około 120 dni** |

Nie trzeba badać wszystkiego, bo wybory „albo” i część technologii można pominąć. Mimo to od Prochu epoki trwają mniej więcej **2 razy dłużej niż założony rok**. Kampania wyszłaby około 130–150 godzin, a nie 80.

Koszty rosną około 20 razy, a wydajność badacza tylko 2 razy.

**Propozycja:** jedno z dwóch:

- przyciąć koszty późnych epok mniej więcej o połowę;
- przyjąć, że liczba badaczy rośnie z populacją (20–30 przy 150+ osobach).

Trzeba też zdecydować, czy gracz musi badać wszystko, czy tylko około 60% drzewka.

---

## 5. Modele 3D: ryzyka techniczne

Przejrzałem pliki `.glb`.

- **Postać to około 50 osobnych siatek** (żołnierz z karabinem: 50 siatek, 11 materiałów, około 4000 trójkątów). Budynek to około 24 siatki.
  - Koncept zakłada kilkaset mutantów i kilkanaście składów naraz. To łatwo kilkanaście tysięcy wywołań rysowania.
  - Dochodzi do tego przebieg konturu i cienie od wielu świateł (latarnia, ogniska, latarki), więc w Godocie to się zadławi.
  - **Propozycja:** przy eksporcie scalać części postaci w jedną siatkę na kości z atlasem materiałów, a dla rojów (szczury) użyć instancjonowania (MultiMesh).
- **Części są sztywne, bez skinningu.** Przy pixel arcie w małej rozdzielczości to zaleta (czysto i lekko). Przy zbliżeniu kamery stawy mogą jednak wyglądać „robotycznie”. Trzeba to sprawdzić w teście shadera na największym przybliżeniu.
- **82 jednostki frakcji nie mają klipów.** Mają te same nazwy kości, więc klipy z `animacje/` powinny pasować, ale tylko przy identycznych proporcjach szkieletu. Nikt tego nie sprawdził.
- **Rendery pokazowe:**
  - na planszy budynków są artefakty cieni (poszarpane plamy wokół modeli);
  - w bitwie skład zlewa się w jedną plamę, a postacie są małe i słabo czytelne przy domyślnym oddaleniu.

  Koncept sam to zapisał jako „do poprawy po teście”, ale nic z tym nie zrobiono.
- **Najważniejsze:** wszystko, co ładnie wygląda, to postproces wypiekany offline (Cycles, EXR z wieloma warstwami, kwantyzacja palety całej klatki). W Godocie na żywo:
  - nie będzie Cycles;
  - kontur z bufora głębi, paleta przez LUT i dithering trzeba napisać od nowa;
  - może wyglądać inaczej.

  **To największa niewiadoma grafiki i powinna być sprawdzona przed jakimkolwiek nowym modelem.**

---

## 6. Problemy projektowe (gra, nie dokumenty)

### 6.1. Ustroje: nakładają się i mają dziury

Każdy ustrój jest zdefiniowany przez 2 z 3 osi:

| Ustrój | Osie |
| --- | --- |
| Komuna | W−, Ł− |
| Rada | W−, Ł+ |
| Junta | W+, M− |
| Reżim | W+, Ł− |
| Kościół Mgły | W+, M+ |
| Wolne Miasto | Ł+, M+ |

- **Nakładanie:** osada W+60, Ł−60, M−60 spełnia naraz warunek Junty i Reżimu. Osada W+50, Ł+50, M+50 spełnia Kościół i Wolne Miasto. Nie ma reguły, który ustrój wtedy wygrywa.
- **Dziury:** nie ma ustroju dla:
  - Wspólnoty z Przemianą (np. komuna zmutowanych);
  - Wspólnoty z Czystością;
  - Wodza z własnością prywatną bez osi Mgły (klasyczna oligarchia czy plutokracja).

  Gracz może trafić w kurs, który nie daje żadnego ustroju, i nie wiadomo, co wtedy.

**Propozycja:** dopisać:

- regułę pierwszeństwa (wygrywa oś z większą wartością albo gracz wybiera przy proklamacji);
- nazwany stan „bez ustroju” (np. „Osada”) z premią do elastyczności praw.

### 6.2. Jednostka „kratka” nie jest zdefiniowana

Kratka pojawia się wszędzie:

- promień latarni to 7–16 kratek;
- zasięg karabinu to 12/20 kratek;
- skrzydło szkoły stoi najwyżej 2 kratki od Placu;
- ogień rozlewa się na 3 kratki.

Walka ma siatkę 0,5 m.

- Jeśli kratka ma 0,5 m, to karabin strzela skutecznie na **6 m**, a krąg latarni ma promień 3,5 m.
- Jeśli kratka ma 1 m, to karabin ma 12–20 m, czyli i tak mniej więcej 10 razy mniej niż w rzeczywistości przy założonej „realistycznej śmiertelności”.

**Trzeba ustalić osobno kratkę osady (budowanie) i metr walki,** a zasięgi broni podawać w metrach.

### 6.3. Gracz bierny i dwie warstwy czasu

ANALIZA B1 i B3 to opisuje i się z nią zgadzam. Dorzucam jedno: nocny atak co 2–3 dni i wyprawy co 2–3 dni dają w 80-godzinnej kampanii około 300–500 walk. Większość pójdzie w tle. Raport po walce staje się więc ważniejszy niż samo oglądanie, a jest dziś słabiej zaprojektowany niż AI.

Brakuje:

- jak wygląda raport walki rozstrzygniętej w tle;
- jak go streścić po 20 walkach (raport zbiorczy na dzień albo tydzień).

### 6.4. Prawa drażliwe

Praca dzieci, przetwarzanie zmarłych, egzekucje i getto dla zmutowanych pasują do klimatu Frostpunka, ale nie ma żadnych zasad:

- jak się je pokazuje;
- czy są ostrzeżenia na starcie;
- jaka jest granica (np. przemoc wobec dzieci tylko w tekście i nigdy na ekranie).

Wpływa to też na możliwość wydania gry (oceny wiekowe i sklepy).

### 6.5. Za dużo systemów naraz w pierwszych godzinach

Już w Złomie dochodzą:

- paliwo, przydział pracy, wyprawy i obrony;
- Plac ćwiczeń, Odmienność, pory roku i Przypływ;
- Bestiariusz, Ratusz i Legitymacja.

To dużo na pierwsze 12 godzin, a samouczka nie ma (ANALIZA B10). Tabela „nowe mechaniki w epokach” w `EPOKI.md` jest dobrym początkiem, ale Złom jest w niej przeładowany.

---

## 7. Czego brakuje całkiem

- **Kodu:** zero linii C#. Prototyp walki był głównym celem od 5 listopada, a od tamtej pory przybyło około 200 KB dokumentów.
- **Testu shadera w Godocie** (punkt 5).
- **Pętli minuty, czyli co gracz robi przez typowe 12 minut dnia przy tempie x1.** Ekonomia jest policzona dniami, ale nie wiadomo, ile realnych decyzji wypada na minutę gry, a czy nie będzie nudno, zależy właśnie od tego.
- **Dźwięku i muzyki, samouczka, poziomów trudności, zapisu, ułatwień dostępu** (ANALIZA B10).
- **Kontroli wersji:** nie ma repozytorium git. Przy pracy wielu sesji Claude na kilku kontach i wysyłaniu zipów po każdej zmianie to proszenie się o utratę pracy. Właśnie tak powstały dwie wersje projektu (luźne pliki i nowsza paczka).

---

## 8. Czego nie rozumiem (pytania do Ciebie)

1. **Ile walk na dzień** gra przewiduje w połowie kampanii i ile z nich chcesz oglądać? Od tego zależy, czy rdzeniem jest oglądanie, czy raport.
2. **Czy śmierć ludzi w pierwszej zimie dobrego gracza jest zamierzona,** a jeśli tak, ilu?
3. **Kratka:** ile metrów, osobno w osadzie i w walce?
4. **Czy gracz ma badać całe drzewko epoki,** czy wystarczy kluczowa ścieżka i około połowy technologii?
5. **Kiedy osada nie ma ustroju** (kurs między narożnikami), to jak się nazywa i co daje?
6. **Mapa taktyczna obrony osady:** czy to ta sama mapa co widok osady (budynki, palisada, które zbudowałeś), czy osobny moduł? Koncept sugeruje pierwsze, plan budowy (brama z palisadą jako osobna mapa) drugie, a różnica w pracy jest ogromna.
7. **Czy Ty kiedykolwiek grałeś w makiety lub rendery jako gracz,** czy tylko je oglądałeś? Inaczej: czy ktoś poza Tobą ma testować prototyp?
8. **Wydanie:** to gra do wydania (Steam), czy projekt dla siebie? Zmienia to podejście do treści drażliwych, dźwięku i zakresu.

---

## 9. Proponowana kolejność

1. Zatwierdzić albo odrzucić zmiany `[zmiana]` i pytania ★ z `PYTANIA.md`, plus pytania 1–6 z punktu 8 tego pliku.
2. Jedno popołudnie porządków w dokumentach: punkt 2 tego pliku (usunąć stare progi epok, przepisać „Oprawę graficzną”, ujednolicić liczby).
3. Założyć repozytorium git w folderze `Gra`.
4. **Test shadera w Godocie** na 3 modelach z paczki (postać, budynek, Ogar), razem ze scaleniem siatek.
5. **Prototyp walki** w C#: na początku kształty, potem modele.
6. Przenieść ekonomię do C# i testować ją tym samym kodem.

Do punktów 4–5 potrzebny jest Godot 4 (.NET) zainstalowany u Ciebie albo włączony dostęp do sieci w ustawieniach konta, żeby Claude mógł go zainstalować w swoim środowisku.
