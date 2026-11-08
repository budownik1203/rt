# Latarnia: jak to zbudować technicznie (AI, mechaniki, kod)

Dokument dla Claude, który będzie pisał grę. Opisuje architekturę, struktury danych, algorytmy i kolejność prac. Opis zasad gry jest w `KONCEPT_Latarnia.md` i `EPOKI.md`; tutaj jest to, jak je zaimplementować. Decyzje oznaczone **[do potwierdzenia]** są propozycjami, o które trzeba zapytać użytkownika (w formularzu z pytaniami wielokrotnego wyboru).

---

## 1. Stos i zasady

- **Silnik:** Godot 4.3+ z C# (.NET 8). Eksport na Windows i Linux.
- **Dwa światy:**
  - `Latarnia.Sim`: czysta biblioteka C#, bez żadnej referencji do Godota. Cała logika gry jest tutaj.
  - `Latarnia.Game`: projekt Godota, który tylko czyta stan symulacji, rysuje go, odtwarza dźwięki i przekazuje polecenia gracza.
- **Determinizm:**
  - Stały krok: walka 20 ticków na sekundę, osada 1 tick na minutę gry.
  - Własny generator losowy (`Rng`, xoshiro128**) z ziarnem zapisywanym w stanie gry.
  - Kolejność iteracji zawsze stała: listy, nie słowniki z losową kolejnością.
  - Żadnej równoległości w kroku, która zmienia stan. Wątki tylko do obliczeń bez efektów ubocznych, np. ocena planów, której wyniki zbiera się w ustalonej kolejności.
  - Liczby: `float` z ostrożnością. Pełna zgodność między maszynami nie jest wymagana, wystarczy powtarzalność na tej samej wersji, do powtórek i testów. **[do potwierdzenia]** Jeśli kiedyś miałby być tryb wieloosobowy, trzeba przejść na stałoprzecinkowe `Fix32`.
- **Dane w plikach:** wszystkie liczby balansu w `data/*.json`. Kod ich nie zawiera.
- **Nazewnictwo:** **[ustalone 8.11]** identyfikatory w kodzie i klucze danych po angielsku (`Fighter`, `pipe_bow`), a teksty gry w plikach tłumaczeń (polski i angielski od startu), teksty w grze po polsku w plikach tłumaczeń (`locale/pl.csv`).

## 2. Struktura repozytorium

```
game/
  Latarnia.sln
  sim/Latarnia.Sim/
    Core/        Rng, Tick, Ids, Modifiers, EventBus, Serialization
    Data/        definicje (WeaponDef, ArmorDef, ClassDef, TechDef, BuildingDef, LawDef, MutantDef, EventDef...), loader JSON
    World/       Region, Node, Edge, Expedition, Caravan, Faction (strategiczne)
    Settlement/  Settlement, Building, Job, Recipe, Stockpile, LightMap, Person, Needs, Views, Morale, Legitimacy
    Politics/    Axes, Laws, Regimes, Factions (wewnętrzne), Decrees
    Tech/        TechBoard, Era, Phase, Breakthrough
    Combat/      Battle, Fighter, Weapon, Ballistics, Wounds, Suppression, Morale, Noise, Vision, Formations, Drills
    AI/          Commander (5 kroków), Instinct (wojownik), MutantBrains, FactionStrategy, Pasterz
    Report/      BattleLog, DecisionRecord, Grading
    Events/      EventSystem, Conditions DSL, Effects
  sim/Latarnia.Sim.Tests/   testy xUnit
  tools/Balance/            CLI: masowe walki i symulacje gospodarki bez grafiki
  godot/
    project.godot
    scenes/  Settlement.tscn, Battle.tscn, WorldMap.tscn, UI/...
    scripts/ (C#) Views/, Input/, Bridge/ (SimHost, który kroczy symulację)
    shaders/ pixel_post.gdshader, toon.gdshader, fog.gdshader
    assets/  modele .glb, tekstury, dźwięki
    ui/      motyw (Theme), tekstury papieru
  data/      *.json
  locale/    pl.csv
```

## 3. Dane (przykłady formatu)

```json
// data/weapons.json
{ "id": "karabin_powtarzalny", "era": 5, "hands": 2, "family": "karabiny", "type": "karabin_zwykly",
  "pen": 4, "range": [12, 20], "rof": 0.8, "mag": 5, "reload": 4.0, "noise": 30, "spread": 1.2,
  "weight": 4.2, "slots": ["luneta", "bagnet", "latarka", "tlumik"], "ammo": "nabój_karabinowy" }

// data/classes.json
{ "id": "strzelec_wyborowy", "school": "strzelecka", "tier": 3, "requires": ["strzelec"],
  "own_gear": ["karabin_wyborowy"], "related": ["karabiny"], "skills": ["karabiny", "skradanie"],
  "abilities": [{ "id": "celny_strzal", "cooldown": 20, "bonus": { "aim": 0.4 } }] }

// data/laws.json
{ "id": "racje_za_prace", "branch": "zywnosc", "axes": { "wlasnosc": 10 },
  "modifiers": [ { "target": "production.all", "op": "mul", "value": 1.1 },
                 { "target": "need.food.weak", "op": "add", "value": -0.2 } ] }
```

**System modyfikatorów** (jeden dla praw, cech, mutacji, technologii i ustrojów):
`Modifier { Target (ścieżka tekstowa), Op (add | mul | set | flag), Value, Condition? }`. Każda wartość w symulacji liczona jest jako: baza z danych, potem wszystkie `add`, potem wszystkie `mul`. Wynik trzymany jest w cache i unieważniany, gdy zmieni się źródło. Dzięki temu nowe prawo albo cechę dodaje się samym JSON-em.

**Warunki zdarzeń**: mały język wyrażeń, np. `season == "zima" && stock.racje < people * 7 && !flag.glodna_zima`, parsowany raz do drzewa. **[do potwierdzenia]** Własny parser, około 300 linii, zamiast skryptów C#, żeby zdarzenia dało się pisać w JSON.

## 4. Walka: pętla symulacji

Walka to `Battle` z listą `Fighter` (ludzie, zwierzęta, mutanty), mapą (siatka 0,5 m z kosztami ruchu, osłonami i wysokością), mapą światła, Mgłą i hałasem.

**Kolejność w jednym ticku (50 ms):**

1. **Percepcja:** dla jednej piątej wojowników na tick, czyli każdy co około 0,25 s.
   - Wzrok: stożek i zasięg.
   - Zasięg mnożony przez światło w miejscu celu i gęstość Mgły.
   - Linia wzroku liczona algorytmem Bresenhama po siatce osłon.
   - Słuch: zdarzenia hałasu z promieniem.
2. **Myślenie dowódców:** tylko ci, którym wypada cykl (zależnie od poziomu, rozłożone na różne ticki).
3. **Instynkt wojowników:** każdy, w każdym ticku, tani, oparty na ocenie użyteczności.
4. **Ruch:**
   - Składy: A* po siatce z kosztem osłony i strefy ognia, ścieżki cache'owane.
   - Roje: wspólne pole przepływu (flow field) do celu, liczone raz na kilka ticków.
   - Unikanie zderzeń: proste rozpychanie.
5. **Strzały i ciosy:** opisane w punkcie 5.
6. **Rany, krwawienie, przyduszenie, morale, zmęczenie:** aktualizacja stanów.
7. **Dziennik:** zdarzenia do `BattleLog` dla raportu i powtórki.

**Budżet:** kilkanaście składów i kilkaset mutantów przy 20 tickach na sekundę na zwykłym komputerze. Jeśli zabraknie czasu: percepcja rzadziej, roje jako grupy, dowódcy w wątku pomocniczym z wynikiem stosowanym w następnym ticku.

## 5. Balistyka, rany, przyduszenie

- **Strzał:**
  - Kierunek celu plus rozrzut, w którym celność wojownika, broń, ruch, przyduszenie, morale, światło i zmęczenie zwężają albo poszerzają stożek.
  - Pocisk to promień (raycast) po siatce z prędkością. Dla broni palnej wystarczy natychmiastowe trafienie z krótkim opóźnieniem; strzały i bełty lecą łukiem z czasem lotu.
  - Pierwsza bryła na drodze to trafienie: także swój albo osłona.
- **Bliskie przeloty:** każdy pocisk, który przeleci w odległości do 1,5 m od wojownika, podnosi mu przyduszenie o wartość zależną od kalibru i hałasu. Przyduszenie opada z czasem w osłonie.
- **Trafienie:**
  - Strefa wylosowana z rozkładu zależnego od postawy i osłony (głowa, tors, ręce, nogi).
  - Przebicie broni minus pancerz strefy: każdy brakujący punkt to −20% siły rany.
  - Ciężkość rany (lekka, ciężka, śmiertelna) z tabeli strefy i siły.
  - Skutki ran: krwotok jako licznik do śmierci, upuszczenie broni, brak ruchu, wstrząs.
- **Bratobójczy ogień:** przed strzałem wojownik sprawdza promień do celu na obecność swoich. Jeśli ktoś stoi na linii, robi krok w bok, klęka, czeka albo zmienia cel. Wyszkolenie taktyczne skraca czas tej reakcji. Trasy marszu omijają linie strzału osłaniających, bo dostają koszt w A*.
- **Mutanty bez ran:** prosty licznik trafień do wyłączenia albo organy (Rozrost) jako osobne strefy z własnym licznikiem.

## 6. AI dowódcy: pięć kroków w kodzie

AI dowódcy to klasyczne AI planujące, bez sieci neuronowych. Ograniczenia dowódcy wynikają z danych (jak często myśli, ile widzi, jak dokładnie ocenia), a nie z losowych pomyłek.

```csharp
sealed class Commander {
    public CommanderProfile P;   // poziom, specjalizacje, charakter (z danych i rozwoju)
    Knowledge K;                 // to, co skład widział i usłyszał, z wiekiem informacji
    Plan Current; float PlanAge;

    public void Think(Battle b) {               // co P.CycleSeconds
        K.Ingest(b.SquadPerception(Squad), P.MemorySeconds);
        var threats = ThreatClusterer.Cluster(K.Enemies, P.Level);              // krok 1
        var tasks   = TaskGenerator.From(threats, K.Terrain, P);                // krok 2
        var plan    = Assigner.Assign(tasks, Squad.Fighters, P);                // krok 3
        if (ShouldReplace(Current, plan)) { Current = plan; PlanAge = 0;        // krok 4
            Orders.IssueDiff(Current, Squad); }                                // krok 5
    }
    bool ShouldReplace(Plan cur, Plan next) =>
        cur == null || next.Emergency ||
        (PlanAge >= P.MinPlanSeconds && next.Score > cur.Score * (1 + P.ReplaceThreshold));
}
```

**Krok 1: obraz sytuacji.**
- Grupowanie wrogów po odległości (prosty DBSCAN z promieniem 4–6 m) w zagrożenia.
- Każde zagrożenie ma kierunek, siłę (suma wartości z Bestiariusza albo wartość ogólna dla nieznanych), typ i czas dotarcia.
- Słabszy dowódca widzi mniej grup (scala sąsiednie) i ma krótszą pamięć.

**Krok 2: zadania.**
- Każde zagrożenie tworzy zadanie, np. „zatrzymać lewą flankę”, z wymaganą siłą i preferowanymi rolami. Preferencje zależą od typu wroga i Bestiariusza, np. Ogar wymaga łamacza i bagnetów.
- Do tego zadania stałe: osłona tyłu, ranni, odwód, cel wyprawy.
- Priorytet = zagrożenie × bliskość × wrażliwość celu.

**Krok 3: przydział.**
- Macierz ocen: wojownik × zadanie = dopasowanie roli + odległość + stan + koszt porzucenia pozycji.
- Przydział zachłanny od najważniejszego zadania, aż siła wystarczy z zapasem. Reszta idzie do odwodu.
- Jakość tej oceny zależy od specjalizacji Zasoby ludzkie:

  | Poziom specjalizacji | Co dowódca bierze pod uwagę przy przydziale |
  | --- | --- |
  | 1 | tylko odległość |
  | 3 | także rola i amunicja |
  | 5 | wszystko, w tym zmęczenie i relacje między ludźmi |

**Krok 4: histereza.**
- Plan trwa co najmniej `MinPlanSeconds` (np. 4 s).
- Nowy plan zastępuje stary tylko wtedy, gdy jest wyraźnie lepszy (np. o 20%).
- Sytuacje nagłe przerywają od razu: granat, upadek lidera, wróg na tyłach.
- Już obowiązujący rozkaz nie jest powtarzany.

**Krok 5: rozkazy imienne.**
- `Orders.IssueDiff` wysyła tylko zmiany: „Bartek, Ola, trzymać lewą!”.
- Rozkaz ma opóźnienie (krzyk i reakcja) i jest zdarzeniem hałasu.
- Wykonanie zależy od morale, dyscypliny i relacji z dowódcą.
- Odpowiedzi wojowników („Nie utrzymam!”) to sygnały, które wymuszają przeplanowanie.

**Teren i formacje jako filtry:**
- **Orientacja w terenie:** liczba sprawdzanych kandydatów na pozycję (1 → 4 punkty, 5 → 40 punktów z oceną osłony, widoku i wąskich gardeł) oraz to, czy dowódca korzysta z wyliczonego wcześniej kluczowego terenu modułu.
- **Taktyka i formacje:** jakie formacje i drile są dostępne, jak dokładnie skład utrzymuje sloty i jak szybko je zmienia.

**Profil z danych:**

```json
{ "level": 3, "cycle": 2.0, "subgroups": 2, "predict": 1.0, "memory": 15,
  "minPlan": 4.0, "replaceThreshold": 0.2,
  "spec": { "kadry": 2, "teren": 4, "formacje": 1 }, "character": "ostrozny" }
```

Charakter (ostrożny, brawurowy, uparty, opiekuńczy) to mnożniki wag w krokach 2–4, np. brawurowy zaniża koszt ryzyka, uparty podnosi próg zmiany planu.

**Bestiariusz:** od poziomu 4 dowódca dostaje z wpisu Bestiariusza kontry wroga jako preferowane role i formacje. Nieznany wróg jest traktowany ogólnie (ma tylko siłę, bez kontr).

**Łańcuch dowodzenia:** po śmierci lidera skład ma 3–5 s chaosu (sam instynkt), potem dowodzi zastępca ze swoim profilem. Bez zastępcy skład spada do poziomu 1.

**Oficer kolumny:** ta sama klasa `Commander`, ale jednostkami są składy, a zadania brzmią Kowadło, Młot, Wsparcie, Odwód. Liderzy wykonują je na własnym poziomie.

## 7. Instynkt wojownika (zawsze aktywny)

Ocena użyteczności (utility AI) z małym zestawem działań:

| Działanie | Główne czynniki oceny |
| --- | --- |
| Strzelaj | widoczny cel w zasięgu, szansa trafienia, wolna linia strzału, rozkaz |
| Przeładuj | pusty albo prawie pusty magazynek, osłona, brak wroga blisko |
| Do osłony | przyduszenie, odsłonięcie, wyszkolenie ≥ 2 |
| Zmień broń | wróg bliżej niż 2 m, brak amunicji |
| Walcz wręcz | wróg w zasięgu ciosu |
| Pomóż rannemu | ranny w pobliżu, cecha Litościwy, rola sanitariusza |
| Uciekaj albo kul się | morale poniżej progów |
| Wykonaj rozkaz | aktualny rozkaz: waga zależna od dyscypliny i morale |

Wyszkolenie taktyczne odblokowuje działania i podnosi ich jakość:

| Poziom | Co umie wojownik |
| --- | --- |
| 1 | nie szuka osłony sam |
| 3 | ruch od osłony do osłony i przeładowanie w ukryciu |
| 4 | krycie wzajemne |
| 5 | sam poprawia oczywiste błędy dowódcy, np. odwraca się do niewykrytej flanki, gdy usłyszy kroki |

## 8. Formacje, techniki ruchu, drile

- **Formacja:**
  - Zestaw slotów (przesunięć względem lidera i kierunku marszu) z rolami preferowanymi w slocie.
  - Zwężanie: próbki szerokości przejścia co 2 m przed liderem; gdy przejście jest za wąskie, formacja przechodzi w wariant wąski (klin, potem rząd).
  - Formacje wręcz (mur tarcz, jeż, orbis) mają sztywne sloty i premie, które znikają, gdy ktoś wypadnie ze slotu.
- **Techniki ruchu:** podział składu na grupy (A i B). Ruch skokami to automat: A stoi i osłania, B idzie do punktu, potem zamiana.
- **Dril:** maszyna stanów z warunkami przejścia, wybierana przez dowódcę. Przykład reakcji na kontakt:

```
Kontakt → [Ogień + Krycie 1,5 s] → [Meldunek] → decyzja:
   wróg słabszy → [Grupa A tłumi] + [Grupa B flankuje po trasie omijającej linię ognia A]
   wróg silniejszy → [Zerwanie kontaktu: A tłumi, B cofa się 15 m, zamiana]
```

Wyszkolony skład wchodzi w dril szybciej, bo ma krótsze stany „meldunek” i „decyzja”.

## 9. Wrogowie: mózgi

| Profil | Zachowanie | Przykłady |
| --- | --- | --- |
| Rój | pole przepływu do światła lub hałasu, rozlewa się wokół przeszkód | Szczurza Fala, Biegacze |
| Flankujący drapieżnik | okrąża, czeka na odsłonięty bok, atakuje strzelców | Ogar, Wataha |
| Zasadzkowiec | czeka w ukryciu, uderza z bliska | Pajęczarz, Kopacz, Naśladowca |
| Wsparcie | trzyma się z tyłu, działa obszarowo | Krzykacz, Siewca |
| Przyciągany światłem | leci na najjaśniejszy punkt | Ćma Latarniowa, Gasiciel |
| Wielkolud | prosto na cel, organy jako strefy | Rozrost, bossowie |
| Dowodzący | ten sam `Commander` z zadaniami dla mutantów | Pasterz, Wielki Pasterz |

Frakcje ludzkie walczą tymi samymi klasami `Commander` i `Fighter` co gracz, z profilami dowódców z danych frakcji.

## 10. Raport po walce i ocena decyzji

- **Dziennik walki:** każda decyzja dowódcy zapisuje `DecisionRecord`:
  - co wtedy wiedział (migawka `Knowledge`);
  - jakie zagrożenia i zadania widział;
  - kogo przydzielił;
  - skutek po 5–10 s.
- **Ocena decyzji, w kolejności sprawdzania:**
  - **Brak reakcji:** zagrożenie było w `Knowledge` dłużej niż X s bez zadania.
  - **Spóźniona:** reakcja przyszła po czasie dłuższym niż czas dotarcia wroga.
  - **Błędna:** przydział z niskim dopasowaniem roli przy dostępnej lepszej osobie.
  - **Trafna:** pozostałe.
- **Przypisanie błędu specjalizacji:**
  - zły przydział to Zasoby ludzkie;
  - zła pozycja lub trasa to Teren;
  - zła formacja lub rozpad szyku to Formacje.
- **Powtórka:** stan zapisywany co 1 s plus zdarzenia, odtwarzane w widoku z nakładkami: sektory, przydziały, mapa zagrożeń i co dowódca wiedział.

## 11. Osada: symulacja

- **Tick:** 1 minuta gry. Produkcja liczona co godzinę, potrzeby i morale co godzinę, poglądy i Legitymacja raz na dzień.
- **Budynek:** definicja, poziom i epoka modelu, przydzieleni ludzie i receptury `Recipe { inputs, outputs, hours, workers, skill }`. Produkcja = receptura × umiejętność × modyfikatory × światło (budynek poza światłem nie działa).
- **Mapa światła:** suma kręgów latarni i lamp (promień z mocy i paliwa) na siatce budowy. Budowa jest możliwa tylko w świetle. Przypływ obniża promienie.
- **Ludzie:**
  - `Person`: statystyki, umiejętności (jedna lista), cechy jawne i ukryte, mutacje, Odmienność, poglądy na 3 osiach, relacje, potrzeby, morale, klasa i stopień, historia.
  - Ukryte cechy mają flagę `revealed` i wyzwalacze odkrycia.
- **Polityka:**
  - Kurs osady = średnia ważona praw.
  - Legitymacja = średnie zadowolenie z praw liczone z odległości poglądów każdego człowieka od kursu.
  - Frakcje wewnętrzne = grupy ludzi o bliskich poglądach (k-średnie raz na kilka dni).
- **Zdarzenia:** definicje w JSON (warunek, waga, opcje z efektami), losowane raz na dzień z puli spełniających warunek, z czasem odnowienia.
- **Ekonomia pierwszych dni:** patrz skrypt `narzedzia/ekonomia_sim2.py`. Liczby z niego to punkt wyjścia do `data/`.

## 12. Mapa świata

- **Graf:** `Node` (typ modułu, strefa, gęstość Mgły, frakcja, odkrycie i wiek informacji) i `Edge` (typ drogi, czas, zagrożenie, przeprawa).
- **Region:** około 150–250 węzłów z generatora modułów plus stałe miejsca fabuły.
- **Wyprawy i karawany:** agenci poruszający się po grafie godzina po godzinie, z zapasami i tempem marszu. Spotkania losowane na krawędzi; walka uruchamia `Battle` na mapie z modułu.
- **Frakcje (strategia):**
  - Każda frakcja ma `FactionStrategy`: cele, siłę, epokę, relacje i plan na N dni.
  - Implementacja jako prosty planista celów (GOAP-lite): cele typu „zdobyć paliwo”, „zniszczyć Chór”, „porywać ludzi”, a działania to rajd, karawana, poselstwo, budowa posterunku.
  - Ocena co kilka dni gry.
  - Szczegółowe strategie każdej frakcji są do ustalenia (`PYTANIA.md`).

## 13. Technologie i epoki

- `TechBoard` na epokę:
  - lista technologii z gałęzią, fazą, kosztem, wymaganiami i grupą wykluczenia (`exclusive_group`);
  - blokady ustrojów jako warunki.
- **Faza** otwiera się po 5 technologiach poprzedniej fazy i postawieniu budynku fazy.
- **Przełom** to obiekt zadania z czterema warunkami: technologie ★, odkrycie (flaga z wyprawy), budynek i próba. Próba to zdarzenie z szansą porażki zależną od umiejętności i przygotowania; porażka ma swoje skutki i czas do ponownej próby.
- **Odpowiedź Mgły:** po przełomie do puli wrogów i Przypływów dopisywane są nowe definicje.

## 14. Zapis gry

- Cały stan symulacji (osada, świat, walki w toku, ziarno RNG) serializowany do JSON albo MessagePack.
- Wersja zapisu w nagłówku i migracje przy zmianie formatu.
- Autozapis co dzień gry i przed walką.

## 15. Grafika w Godocie (render na żywo w stylu z Blendera)

- **Render:** scena 3D renderowana do `SubViewport` w niskiej rozdzielczości (np. 480×270 albo 640×360), potem powiększenie bez wygładzania do ekranu (filtr nearest, skala całkowita).
- **Shader materiałów (toon):** oświetlenie skwantowane do pasm o progach [0.04, 0.12, 0.30, 0.62], przesunięcie barwy cieni w stronę fioletu.
- **Postproces (shader ekranu na niskiej rozdzielczości):**
  - kontur 1 px z różnicy głębi (próg 0,07 dla postaci, 0,22 dla otoczenia; postacie odróżnione maską w kanale alfa albo osobnym buforze);
  - kwantyzacja do palety około 45 kolorów przez teksturę LUT;
  - dithering Bayera tylko w Mgle;
  - Mgła liniowa albo promienista od latarni.
  
  Algorytmy i parametry są w `PIPELINE_GRAFIKI.md` i w `post.py` w paczce źródeł; trzeba je przepisać na shadery Godota.
- **Modele:**
  - `.glb` z paczki `assets/latarnia_modele_godot.zip`;
  - animacje jako ścieżki NLA, które po imporcie są klipami w `AnimationPlayer`;
  - IK upieczone do FK.
- **Ekwipunek modułowy:** broń, hełmy i tarcze jako osobne siatki przyczepiane do kości (`BoneAttachment3D`). Na kości broni są puste punkty wylotu lufy i wyrzutu łusek.
- **Efekty:** `GPUParticles3D` z cząstkami przyciągniętymi do siatki pikseli (dym, ogień, iskry, łuski, Mgła), światła punktowe dla ognia i latarni, wspólny błysk wylotu.
- **Kamera:** ortograficzna, nachylenie około 50°, obrót o 90° skokami albo płynnie, przybliżanie skokami całkowitej skali.

## 16. Interfejs w Godocie

- Kontrolki Godota (`Control`) z własnym motywem (`Theme`): tekstury papieru, 9-slice dla kartek, fonty maszynowe i odręczne (licencje open).
- **Stałe strefy ekranu**, żeby nic na siebie nie nachodziło:
  - górny pasek: czas, dzień, zapasy;
  - lewa kolumna: zakładki ekranów;
  - prawa kolumna: panel kontekstowy;
  - dół: alarmy.
- Animacje przez `Tween`:
  - kartka wsuwa się z lekkim obrotem;
  - pieczątka uderza ze skalą 1,3 → 1,0 i lekkim wstrząsem;
  - lampa na biurku migocze.
  
  Szczegóły są w makietach i w liście zadań interfejsu.

## 17. Testy i narzędzia balansu

- **Testy jednostkowe:** balistyka, rany, modyfikatory, przydział zadań, histereza, gospodarka dnia.
- **Scenariusze balansu w CLI**, np. `dotnet run --project tools/Balance -- --scenario brama_ogary --a dowodca_l2 --b dowodca_l4 --runs 1000`. Wynik: odsetek wygranych, straty, czasy, najczęstsze błędy według specjalizacji.
- **Gospodarka:** ten sam CLI uruchamia osadę na N dni z prostym graczem-botem (jak w `ekonomia_sim2.py`) dla planów dobry, średni i zły.

## 18. Kolejność budowy

Patrz `PLAN_BUDOWY_GRY.md`. Najważniejsze: najpierw **prototyp walki** (sama symulacja + prosta grafika z kształtów, dymki rozkazów, raport). Sprawdzić, czy oglądanie walki dowódcy wciąga, zanim powstanie reszta. Potem mała osada, potem rozwój, świat, treść.

## 19. Jak pracować z użytkownikiem

- Pisze po polsku. Przed każdą większą pracą chce pytań wielokrotnego wyboru w formularzu (AskUserQuestion).
- Grafikę i kod robi Claude. Użytkownik projektuje i ocenia.
- Kod trzymać w folderze gry na komputerze użytkownika (`Pulpit\Gra\game`). Po każdej większej zmianie aktualizować `docs/` i zrobić zip całego folderu.
