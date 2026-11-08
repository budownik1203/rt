# Prototyp walki: symulacja i AI dowódcy (stan 8 listopada 2026, wieczór)

Pierwszy grywalny wycinek bez grafiki: czysty, deterministyczny C# (`game/sim`), podgląd z góry jako GIF. Widok 3D w Godocie podłączy się później do tej samej symulacji, a symulacja nie wie nic o Godocie.

## Jak uruchomić

```
cd game/sim
export PATH=$HOME/.dotnet:$PATH DOTNET_ROOT=$HOME/.dotnet
dotnet build Latarnia.Tools -c Release
T=Latarnia.Tools/bin/Release/net8.0/Latarnia.Tools.dll
dotnet $T bench 1000                      # 1000 walk na poziom dowódcy (1, 2, 3) w każdym scenariuszu
dotnet $T report krzykacz 3 1001          # raport po walce + dziennik rozkazów
dotnet $T replay mieszany 3 1001 w.json   # zapis klatek
python3 ../../narzedzia/replay_gif.py w.json w.gif --do 75 --kadr 46 --skala 11
dotnet $T cmp 150 "szczury;ogary" "1,1,1,1;3,4,4,1"   # porównanie profili: poziom,Kadry,Teren,Formacje
dotnet $T diag 60                         # czy raport wskazuje słabą specjalizację
dotnet test Latarnia.Sim.Tests            # 8 testów (determinizm, poziomy, raport, flary)
```

Pozostałe komendy diagnostyczne: `why` (kto rani skład), `roles` (statystyki na osobę), `dec` (decyzje dowódcy krok po kroku), `ff` (każdy ogień bratobójczy z kontekstem), `stuck` (co robi każdy na końcu walki).

## Co jest w środku

| Plik | Zawartość |
|---|---|
| `Core/Core.cs` | Vec2, deterministyczny RNG (xorshift128+), MathX |
| `World/Grid.cs`, `Maps.cs` | siatka 0,5 m, światło latarni wypiekane z cieniami, pole przepływu do latarni, A*, linia wzroku; mapa: mała osada z palisadą, bramą od południa i wyłomem od wschodu |
| `Data/Defs.cs` | 8 broni ludzi, ataki mutantów, 4 gatunki, role |
| `Units/Fighter.cs` | wojownik: rany w strefach, krwawienie, morale, zmęczenie, przygniecenie ogniem, percepcja, rozkaz |
| `Combat/Battle.cs` | krok 50 ms: percepcja (stożek, światło, słuch), dowódcy, instynkt, ruch z masą, pociski z ogniem bratobójczym, rany, morale, flary, fale wrogów |
| `AI/Commander.cs` | dowódca w 5 krokach (niżej) |
| `AI/Terrain.cs` | wybór pozycji (Teren) i szyki (Formacje): tłum, linia, mur tarcz, krąg, jeż, rozproszenie przed Nosicielem |
| `AI/Instinct.cs` | instynkt wojownika: utility AI wykonujące rozkaz z dyscypliną zależną od wyszkolenia |
| `AI/EnemyBrains.cs` | rój szczurów, Ogar (skrada się, kleszcze stada, ugryź i odskocz), Krzykacz (krzyk z ciemności, odsłania się po krzyku), Nosiciel (idzie w skupisko, wybucha) |
| `AI/Phrases.cs` | dymki po polsku: imiona i punkty odniesienia („Bartek, Ola, trzymać bramę!”) |
| `Report/Report.cs` | raport: każda rana składu przypisana do decyzji dowódcy, wnioski według specjalizacji |
| `Scenarios/Scenarios.cs` | 6 scenariuszy + Szabrownicy z wybranym poziomem dowódcy (`szabrownicy1`, `szabrownicy3`, `szab:L,K,T,F`) |

### Dowódca w 5 krokach

1. **Obraz sytuacji.** Meldunki ludzi w zasięgu głosu, wrogowie widziani i słyszani (krzyk Krzykacza i świst strzały zdradzają pozycję). Grupowanie w zagrożenia ze stałą tożsamością między cyklami. Słabszy dowódca widzi mniej grup (scala je) i krócej pamięta.
2. **Zadania.** Trzymać (przesmyk albo skraj światła), ogień wsparcia, zdjąć Krzykacza, zdjąć Nosiciela z daleka, ratować rannego, pilnować tyłów, szturm (atakujący), odwód. Rozpoznawanie typu zagrożenia:
   - szybkie (Ogary) albo z wielu stron: krąg przy latarni;
   - rój: przesmyk;
   - bomba: rozproszyć się i strzelać z daleka;
   - łucznicy w ciemności (poziom 3): szturm na nich.
3. **Przydział.** Kolejność: ranni, zadania specjalne strzelców, linia (podział walczących wręcz między zagrożenia według priorytetu), krąg, tyły, wsparcie ogniowe, odwód. Macierz ocen zależy od Kadr:
   - Kadry 1–2 wstawiają strzelców i sanitariuszkę do linii;
   - Kadry 3 i więcej trzymają ich z boku.
4. **Histereza.** Minimalny czas planu, próg lepszego planu, ponowna ocena starego planu wobec rosnącego zagrożenia („linia nie wytrzyma”), sytuacje nagłe, meldunki z linii.
5. **Rozkazy jako różnice.** Okrzyk tylko dla tych, którym zmienia się zadanie (przesunięcia w szyku bez krzyku), opóźnienie zależne od poziomu, dymki. Do tego **flary** (2 na walkę): poziom 2 oświetla widziane zagrożenie w ciemności, poziom 3 także sam hałas.

Poziom 1 nie planuje: każdy działa na instynkt. Gdy dowódca padnie, są 3–5 s chaosu, potem dowodzenie przejmuje zastępca o poziom niżej.

## Wyniki (1000 walk na poziom, ten sam skład 6 osób, te same ziarna losowania)

| Scenariusz | Poziom 1: wygrane | zabici | padli | Poziom 2: wygrane | zabici | padli | Poziom 3: wygrane | zabici | padli |
|---|---|---|---|---|---|---|---|---|---|
| Ogary (5+3, kleszcze) | 100% | 0,03 | 0,94 | 100% | 0,02 | 1,05 | 100% | 0,01 | 0,97 |
| Szczurza Fala (26+18) | 98% | 0,34 | 0,66 | 100% | 0,00 | 0,07 | 100% | 0,00 | 0,04 |
| Krzykacz z Ogarami | 98% | 0,19 | 0,73 | 99% | 0,04 | 0,54 | 99% | 0,01 | 0,56 |
| Nosiciele | 98% | 0,23 | 1,26 | 98% | 0,23 | 1,42 | 99% | 0,06 | 0,80 |
| Szabrownicy (7 ludzi, dowódca poz. 2) | 20% | 0,28 | 5,10 | 29% | 0,38 | 4,81 | 37% | 0,22 | 4,40 |
| Noc Mgły (wszystko, 3 fale) | 85% | 0,66 | 2,18 | 98% | 0,10 | 0,97 | 97% | 0,03 | 0,94 |

Suma zabitych na 6 walk: poziom 1 to **1,73**, poziom 2 to **0,77**, poziom 3 to **0,33** (ponad 5 razy mniej niż poziom 1). Pełna tabela w `docs/prototyp_walki/bench_1000.txt`.

**Czytanie:** „padli” to wyłączeni z walki (ranni do uratowania plus zabici). Wynik 100% wygranych w kilku scenariuszach oznacza, że są za łatwe, żeby różnica wyszła w wygranych, więc różnicę widać w zabitych i padłych.

## Ocena wobec 4 kryteriów

1. **Poziomy się różnią.** Kryterium spełnione w zabitych: poziom 3 traci kilka razy mniej ludzi niż poziom 1 w każdym scenariuszu (szczegóły w tabeli). Poziom 2 jest pośrodku. Wyjątek: przy Nosicielach poziom 2 bywa gorszy od 1, bo ma jedną podgrupę i zbija ludzi w szyk, w który idzie bomba. Ogary to scenariusz, w którym dowódca zmienia najmniej: instynkt i światło robią większość roboty.
2. **Zachowanie wygląda ludzko.** W dużej części tak:
   - rozkazy są imienne, z punktem odniesienia, i nie powtarzają się co cykl;
   - ludzie meldują („Nie utrzymam!”, „Zachodzą od tyłu!”), a meldunek przyspiesza przeplanowanie;
   - strzelcy nie strzelają przez plecy swoich, robią krok w bok, szukają linii strzału;
   - sanitariuszka opatruje i wynosi pod latarnię;
   - dowódca rzuca flarę na Krzykacza w ciemności.

   Słabsze strony:
   - w GIF-ach plan zmienia się 8–12 razy na minutę walki, w tym kilka razy nagle (prawdziwy dowódca zmieniałby rzadziej);
   - krąg wygląda dobrze, ale ludzie zbijają się w kupkę.
3. **Raport trafnie diagnozuje.** Częściowo:
   - słabe Kadry (1 przy pozostałych 4) raport wskazuje w 97% walk;
   - słabego Terenu i słabych Formacji raport prawie nie wykrywa;
   - słabe Formacje dają nawet *mniejsze* straty niż mocne (test `diag`), więc część zachowań Formacji wciąż szkodzi (podejrzany: tyły i krąg przy mało licznych zagrożeniach), a raport nie ma czego wykryć.
4. **Wrogowie używają tego samego AI.** Kod jest ten sam: Szabrownicy mają tego samego `Commander` i `Instinct` z flagą ataku. Działa koncentracja sił, Herszt prowadzi szturm, łucznicy zajmują pozycje z widokiem, a ranni są ratowani dopiero po szturmie. **Nie działa jeszcze** zależność od poziomu u atakujących: banda z dowódcą poziomu 3 wypada *słabiej* niż banda bez dowódcy. Nasz skład (poziom 2) wygrywa 20% walk z bandą poziomu 1 i 28–33% z bandą poziomu 3. Hipotezy: dowodzony szturm traci tempo na rozkazy, a łucznicy bez rozkazu strzelają swobodnie z ciemności. To jest pierwsza rzecz do następnej sesji.

## Zmiany w mechanice zrobione po drodze (do potwierdzenia przez użytkownika)

Te zmiany były potrzebne, żeby decyzje dowódcy w ogóle miały znaczenie:

- **Brama i wyłom** były narysowane poza palisadą, więc osada była zamknięta. Teraz są w ścianie.
- **Światło latarni** liczyło cień od samej latarni (cała osada w półmroku). Naprawione.
- **Śmiertelność niższa:**
  - celność improwizowanej broni nocą 0,22 + 0,045 za punkt;
  - trafienie wręcz 0,46 bazowo;
  - ciężka rana tułowia wyłącza w 45%;
  - szczur zadaje drobne rany (krwawienie 0,035).

  Walki trwają 25–100 s. Koncept mówi o obronie 3–6 minut, więc śmiertelność można jeszcze zmniejszyć, gdy dojdą fale.
- **Masa w przepychaniu:** człowiek trzymający pozycję stoi jak ściana wobec szczura. Bez tego przesmyk nic nie dawał.
- **Cios z boku i od tyłu:** +10% i +22%. Za każdego dodatkowego napastnika +5%. Dzięki temu szyk ma znaczenie.
- **Nosiciel fosforyzuje** (widać go z 16 m w ciemności), a **Krzykacz po krzyku świeci** przez 3,5 s. Bez tego w ciemności za palisadą nie dało się ich zdjąć z daleka. To decyzja projektowa: czy zostaje?
- **Flary dowódcy:** 2 na walkę, promień 10 m, 20 s. Wynika z decyzji „plan i flary” z ankiety.

## Następne kroki

1. Atakujący z dowódcą muszą być lepsi niż bez niego:
   - zgranie szturmu w czasie (czekać, aż wszyscy dojdą);
   - łucznicy na flankach w ciemności;
   - cel: najsłabszy punkt obrony.
2. Formacje:
   - zmierzyć osobno krąg, tyły i mur tarcz (komenda `cmp`) i poprawić to, co szkodzi;
   - dać raportowi sygnały Formacji (kto dostał w plecy, kto stał poza szykiem).
3. Teren: sygnały do raportu (pozycja bez osłony wobec łuczników, walka w ciemności).
4. Poziomy 4–5 (profil jest gotowy, zachowań brak) i charaktery dowódców (Ostrożny, Brawurowy, Uparty, Opiekuńczy).
5. Podgląd w Godocie: ta sama symulacja, modele `.glb` i shader z testu.
