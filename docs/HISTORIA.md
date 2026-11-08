# Historia projektu Latarnia (5–6 listopada 2026)

Ten plik streszcza rozmowy, z których powstał projekt, w kolejności, w jakiej zapadały decyzje. Szczegóły każdej decyzji są w `KONCEPT_Latarnia.md`.

## 1. Koncept (5 listopada)

1. Prośba o grę post-apo o przetrwaniu. Wybrany rdzeń to zarządzanie osadą, ton dziwny z mutantami, a pierwotnie widok 2D z góry w pixel arcie.
2. **Ustroje:** drzewo praw, które naprawdę pcha osadę w komunizm, faszyzm, totalitaryzm albo demokrację (w tym prawa jak praca dzieci). Rozwiązanie hybrydowe: drzewo praw plus trzy osie (Władza: Wspólnota–Wódz; Własność: Kolektyw–Prywatne; Mgła: Czystość–Przemiana) daje sześć ustrojów. Do tego Legitymacja i sztandary frakcji, które się rozprzestrzeniają.
3. **Drzewko technologii:**
   - 7 epok: Złom, Kuźnia, Czarny proch, Para, Nabój, Silnik, Światło;
   - 5 gałęzi z progami, które wymagają technologii z innych gałęzi;
   - badania za punkty, surowce i znaleziska, w tym inżynierię wsteczną.
4. **Wojownicy:**
   - każdy jest osobną postacią;
   - wyposażenie w slotach: ręka główna, ręka dodatkowa, broń boczna, dodatki do broni, głowa, tors, ubiór; pas i plecak to jeden slot;
   - **Standaryzacja** to technologia z połowy gry, która odblokowuje Wzory wyposażenia, bez premii do statystyk.
5. **Dowódcy:**
   - mają poziom ogólny 1–5 i trzy specjalizacje (Zasoby ludzkie, Orientacja w terenie, Taktyka i formacje) szkolone w Akademii;
   - AI wydaje rozkaz tylko wtedy, gdy to ma sens (histereza), i przydziela konkretnych, pasujących żołnierzy do zagrożeń;
   - formacje, techniki ruchu i drile są oparte na prawdziwych podręcznikach wojskowych (źródła w koncepcie).
6. **Walka:**
   - realistyczna śmiertelność (pies nie przeżyje 4 strzałów z karabinu), pełny ogień bratobójczy;
   - walki trwają tyle, ile trzeba;
   - czas rzeczywisty z pauzą, który zatrzymuje się podczas oglądania walki.
7. **Świat:**
   - fikcyjna Europa Środkowa, kampania sandbox na około 50 godzin;
   - Mgła jest skutkiem eksperymentu, a latarnia należy do przedwojennej sieci;
   - mapa to sieć lokacji: stałe miejsca fabuły plus moduły generowane proceduralnie;
   - dyplomacja średnia.
8. **Interfejs:** diegetyczny (teczki, maszynopisy, mapy na papierze), osada jako ekran główny, panele i tabele.
9. **Silnik:** wybrany Godot 4 + C# z symulacją w czystym C#. Rust z Bevy odrzucony, bo interfejs jest za słaby, a API się zmienia.
10. Użytkownik zdecydował, że grafikę i kod robi Claude, kod trafia do folderu na komputerze użytkownika, a pierwszy wycinek to sama walka.

## 2. Grafika (5–6 listopada)

- Najpierw próby 2D pixel artu. Kolejne uwagi użytkownika:
  - za mało detalu;
  - złe proporcje i źle trzymana broń;
  - ludzie za duzi względem latarni (latarnia ma mieć około 4,5 wzrostu człowieka) i za masywni;
  - chód wyglądał jak kulenie.
- Potem test 3D low-poly renderowanego do pixel artu. **Wybrany kierunek:** „już teraz jest to naprawdę zajebiste”, z kamerą z przybliżaniem i dowolnym obrotem postaci (zamiast 8 kierunków).
- **Animacje:**
  - zbadane zasady animacji i zbudowany system póz z krzywymi;
  - klipy: chód, bieg, strzał, przeładowanie, osłona, dzida, młot, granat, śmierć, patrol;
  - potem przejścia i reakcje oraz animacje mutantów;
  - użytkownik ocenił, że każda grupa wymaga jeszcze pracy, a tempo 12 kl/s zostaje.
- **W nocy z 5 na 6 listopada** (praca bez nadzoru) powstały: 39 budynków, 13 broni, 18 jednostek frakcji, 12 mutantów, życie osady, bitwa przy bramie i eksport .glb.
- **6 listopada:**
  - poprawione animacje (strzał, przeładowanie, wypad, granat, wypadająca broń przy śmierci) i efekty;
  - arsenał 14 broni w rękach i scena osady nocą;
  - eksport animacji do Godota.

## 3. Decyzje z 6 listopada

- Lanca latarniowa jako broń epoki Światła zostaje.
- Barwy i znaki frakcji są docelowe (11 frakcji, tabela w koncepcie).
- Ogar to zwykły pies lekko zmieniony przez Mgłę. **Mocne objawy infekcji mają tylko Stworzenia Mgły**, a zwierzęta i ludzie są zmienieni słabiej.
- Budynki późniejszych epok to osobne modele z własnymi animacjami i efektami.
- Dym i ogień jako cząsteczki w Godocie, w docelowej jakości.
- Więcej przeciwników we wszystkich grupach: frakcje ludzkie, ludzie zmienieni, Stworzenia Mgły, a frakcje mają po 6–8 jednostek.
- **Klasy i szkoły:**
  - wojownicy dzielą się na klasy wyspecjalizowane w sprzęcie;
  - kompleks szkoleniowy to Plac i skrzydła jako osobne budynki;
  - rozwój to umiejętności plus drzewko klas z awansami, a wojownicy rozwijają się jak dowódcy, także zdolnościami bojowymi.
- **Uwagi do makiet interfejsu (wieczór 6 listopada):**
  - więcej stylizacji, nic nie może się nakładać;
  - kompleks jako budynek, a kursanci zgodni z kursem;
  - przeszkolenie drogie;
  - szkoły w drzewkach jak w Mount & Blade, z odpowiednikami sprzętu w każdej epoce od początku;
  - szkoła chorążych zamieniona na **szkołę komisarzy** (aktywne morale, rozstrzeliwanie jako dekret), a chorągwie jako przedmioty; więcej szkół;
  - opisy klas i specjalizacji;
  - większa mapa z większą swobodą ruchu;
  - **ukryte cechy** odkrywane w czasie;
  - klikanie budynków w osadzie;
  - budynki bardziej post-apo, a w kolejnych epokach coraz bardziej klasyczne (cegła), ale wciąż zrujnowane.
  Wszystko to jest już wpisane do konceptu. Wykonanie tych uwag w grafice i interfejsie jeszcze nie ruszyło (`DO_ZROBIENIA.md`).
- Użytkownik na razie przerywa budowę gry z tym Claude i przekazuje projekt nowemu Claude na innym koncie, stąd ten pakiet.

## Odrzucone lub zmienione pomysły

- 2D pixel art rysowany ręcznie: zastąpiony przez 3D renderowane do pixel artu.
- 8 kierunków postaci: zastąpione dowolnym obrotem.
- Chitynowy, jaszczurzy Ogar: zastąpiony zwykłym psem (stary model jest jako `ogar_chityna` w kodzie).
- Szkoła chorążych: zastąpiona szkołą komisarzy, a sztandar jest przedmiotem.
- Szkoły dostępne dopiero od późniejszych epok: teraz każda szkoła działa od Złomu z odpowiednikiem sprzętu.
- Rust z Bevy: odrzucony na rzecz Godota z C#.


## 7 listopada 2026

- **Analiza całego konceptu.** Poprawione sprzeczności:
  - 2D zamienione na 3D renderowane jako pixel art;
  - przeglądarka zamieniona na komputer;
  - trzy „Place” połączone w jeden Plac ćwiczeń;
  - opisany stopień I (Rekrut);
  - dodany pistolet skałkowy.
- **Decyzje:**
  - jedna wspólna lista umiejętności;
  - komisarz zależny od ustroju (Politruk, Rzecznik, Żandarm, Komisarz, Kaznodzieja, Nadzorca);
  - 7 ekranów zamiast 12;
  - mapa w dwóch warstwach ze sposobami ruchu;
  - budynki z zestawu części post-apo;
  - sprzęt szkół zostaje osobny w każdej epoce.
- **Ekonomia pierwszych 10 dni:** start jako walka o przetrwanie, paliwo i jedzenie po równo, dzień trwa 12 minut, 10 osób. Symulacja: dobry gracz głoduje w 8% gier, średni w 78%, zły upada.
- **Epoki:**
  - Użytkownik nie chciał szybkiej Kuźni: epoka ma być długim etapem, a wejście w następną przełomem.
  - Ustalono epokę na około rok gry, bez limitów czasu, z kampanią około 80 godzin.
  - Każda epoka ma trzy fazy i własne zagrożenia.
  - Przełom to technologie, odkrycie z wyprawy, budynek z próbą, a potem odpowiedź Mgły.
  - Wszystkie 7 epok rozpisane w pełni (`EPOKI.md`), z 2–3 wyborami wykluczającymi się na epokę.
- **Zima (dni 11–36):** odłożona, aż będzie znana zawartość epoki Złomu. Pierwsza próba symulacji była błędna.

## Noc z 6 na 7 listopada (praca samodzielna)

- Dokument techniczny: jak zbudować AI i mechaniki (`TECHNIKA_AI_I_MECHANIKI.md`).
- Dogłębna analiza z propozycjami (`ANALIZA.md`) i 162 pytania (`PYTANIA.md`).
- Budynki w cyklu epok z opisami wyglądu jako prompty (`BUDYNKI.md`) i nowe mechaniki każdej epoki (`EPOKI.md`).
- Bestiariusz w liczbach z usuniętymi dublami (`WROGOWIE.md`) i broń z wersjami, dodatkami i amunicją (`BRON.md`).
- Uzupełnienia: ukryte cechy, klasy w liczbach, wydarzenia, postacie (`UZUPELNIENIA.md`).
- Interfejs v2 (`ui/interfejs_v2.html`).
- Lista rzeczy na rano (`RANO_DO_SPRAWDZENIA.md`).
- Po każdej zmianie wysyłany był zip całego folderu.

## 7 listopada po południu

- Zima i dni 11–36 policzone.
  - Część liczb ekonomii poprawiona: Smolarnia 3 drewna na 2 paliwa, drwal 12, latarnia w dzień ×0,25, warta pół dnia jednej osoby, ogrzewanie zimą.
  - Zmiany są oznaczone [zmiana] i czekają na zgodę.
- Pełny katalog 52 klas (`KLASY.md`).
- Pula wydarzeń rozszerzona do około 155 (`WYDARZENIA.md`).

## 8 listopada 2026 (nowy Claude, nowe konto)

- Projekt przejęty. Folder `Pulpit\Gra` zaktualizowany do wersji z `Latarnia_paczka.zip` (wcześniej leżała tam starsza wersja bez ANALIZA, PYTANIA, KLASY i innych).
- Analiza krytyczna (`ANALIZA_KRYTYCZNA.md`):
  - sprzeczne systemy wejścia w epokę;
  - stary opis 2D w Oprawie graficznej;
  - liczby ekonomii w trzech wersjach;
  - błędy obu symulacji ekonomii (średni plan lepszy od dobrego, zła gra nie karze);
  - tempo badań od Prochu około 2 razy za wolne;
  - około 50 siatek na postać w `.glb`;
  - ustroje nakładają się i mają dziury;
  - niezdefiniowana „kratka”;
  - 8 pytań do użytkownika.
- **Druga połowa nocy z 7 na 8 listopada**, decyzje z rozmowy głosowej i formularzy (`DECYZJE_2026-11-08.md`):
  - nowy system ustrojów z 6 osiami i 64 nazwami (`USTROJE.md`), szósta oś to Opieka ↔ Selekcja (`OS_OPIEKA_SELEKCJA.md`, kandydatki w `OS_SZOSTA.md`);
  - przełom jako jedyne wejście w epokę;
  - Oprawa graficzna przepisana pod 3D, render 640×360;
  - kampania 80 godzin, koszty późnych epok obcięte o połowę;
  - kratka 2 m, zasięgi w metrach;
  - każda walka oglądana, plan przed walką i flary;
  - zatwierdzone zmiany wrogów;
  - treści drażliwe pokazywane wprost.
- **Symulacja ekonomii v2** (`narzedzia/ekonomia_sim2.py`, 8 listopada):
  - napisana od nowa: limit ruin, woda, skutki morale, przybysze tylko do światła, przydział pracy według braków, Barak zamiast szałasów, zapas paliwa na Przypływ;
  - jeden zestaw liczb: Smolarnia 3:2 (do 6 na osobę), drwal 12 (zimą 8), latarnia w dzień 25%, szałas zimą 3, Barak zimą 2;
  - wyniki: dobry gracz traci około 1 osoby, średni około 3, zły upada; Opieka ratuje ludzi kosztem zapasów, Selekcja daje około 27% więcej zapasów kosztem zgonów;
  - projekt trafił do repozytorium `budownik1203/rt` na GitHubie.

