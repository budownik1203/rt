# Do zrobienia (stan na 7 listopada 2026)

## Najpierw (stan na 8 listopada w nocy)

Decyzje z 8 listopada są w `DECYZJE_2026-11-08.md` i w większości wpisane do konceptu. Zostało:

1. ~~Poprawiona symulacja ekonomii~~: zrobione 8.11 (`narzedzia/ekonomia_sim2.py`).
   - Dobry gracz traci około 1 osoby.
   - Opieka i Selekcja są zbalansowane.
   - Liczby są ujednolicone w koncepcie i `EPOKI.md`.
2. **Przypisanie praw do 6 osi** i **wersje komisarza według osi** Władza i Porządek (`USTROJE.md`).
3. **Około 27 wzmianek starych ustrojów** w koncepcie (Junta, Kościół Mgły, Reżim itd. w wydarzeniach, mutacjach, standaryzacji) do przepisania na bieguny. Uwaga: Synowie Czystości jako „skrajna Junta” to opis frakcji i może zostać.
4. **Przegląd 64 nazw ustrojów** przez użytkownika.
5. **Repozytorium git** (`latarnia`, prywatne): czeka, aż użytkownik je założy.
6. ~~Test shadera w Godocie~~: zrobiony 8.11 (`docs/TEST_SHADERA.md`). Następne kroki:
   - paleta z rampami materiałów;
   - obwódki pod jednostkami;
   - scalanie siatek (około 860 wywołań rysowania na 3 postacie i 5 budynków to za dużo);
   - pomiar klatek na sekundę u użytkownika.
7. **Prototyp walki:** symulacja i AI zrobione 8.11 wieczorem (`docs/PROTOTYP_WALKI.md`). Dalej:
   - atakujący z dowódcą lepsi niż bez;
   - Formacje i Teren w raporcie;
   - poziomy 4–5 i charaktery;
   - podgląd w Godocie.
   - Do decyzji użytkownika: świecący Nosiciel i Krzykacz, niższa śmiertelność, flary 2 na walkę.
7a. **Symulator bitew 3D** zrobiony 8.11 wieczorem (`docs/SYMULATOR.md`, `game/symulator`). Dalej:
   - ocena animacji na żywo przez użytkownika i pomiar klatek na GTX 1660 Super;
   - nóż u pasa strzelców do zwarcia;
   - dźwięk;
   - model latarni z Blendera;
   - lepszy dowodzony szturm (skład kontra skład).
8. **Pozostałe pytania ★** z `PYTANIA.md`, których nie objęły decyzje z 8 listopada.


## A. Koncept: do rozpisania

1. **Ekonomia w liczbach (dni 1–36 gotowe 8.11, patrz koncept i narzedzia/ekonomia_sim2.py):**
   - koszty wszystkich budynków;
   - tempo produkcji;
   - zużycie (jedzenie, woda, paliwo do latarni);
   - zależność promienia światła od paliwa.
   Zostało: dalsze dni, zima i budynki kolejnych epok.
2. **Epoki:** rozpisane w pełni w `EPOKI.md` (zakładka Epoki w dokumencie): technologie w fazach z kosztami, wybory, budynki, sprzęt, surowce, społeczeństwo. Zostało: tempo badań (punkty nauki na dzień w kolejnych epokach) i balans kosztów.
2a. **Zima i dni 11–36:** policzone (UZUPELNIENIA.md, sekcja 6). Po akceptacji zaktualizować tabele 10 dni w koncepcie według nowych liczb.
3. **Szkoły i klasy w szczegółach:**
   - dla każdej z 11 szkół i każdej klasy: liczby zdolności (zasięgi, czasy odnowienia, premie) i warunki awansu;
   - koszty kursów i przeszkolenia;
   - pełne drzewka mistrzów;
   - opis „do czego służy” dla każdej klasy, bo użytkownik chce go widzieć w grze.
4. **Ukryte cechy:** pełny katalog cech (dobre i złe), warunki odkrycia każdej i jak często się pojawiają.
5. **Komisarz:**
   - liczby (zasięg, siła gaszenia paniki);
   - zasady dekretu „Rozstrzeliwanie na miejscu” i jego wpływ na osie i Legitymację;
   - reakcje frakcji ideowych.
6. **Mapa świata:**
   - znacznie większa mapa i katalog modułów lokacji (ruiny, wsie, bunkry, przeprawy, gniazda);
   - generator, czyli jak moduły łączą się w region;
   - ruch: kilka składów naraz, karawany, obejścia, przeprawy, pogoda, Przypływy na mapie.
7. **Pula wydarzeń:** pełna lista wydarzeń, bo na razie jest około 15 przykładów.
8. **Postacie z historiami:** przywódcy 11 frakcji, bohaterowie wydarzeń, kilku wyjątkowych mieszkańców osady.
9. **Frakcje w liczbach:** siła, jak rosną i jak reagują na prawa gracza.
10. **Bestiariusz w liczbach:** życie, obrażenia, prędkości, ewolucje „Mgła się uczy”.
11. **Budynki późniejszych epok:** lista, które budynki zmieniają wygląd w każdej epoce i co nowego robią.
12. **Komisarz według ustrojów:** pełne drzewka dla 6 wersji (Politruk, Rzecznik, Żandarm, Komisarz, Kaznodzieja, Nadzorca) i liczby dla Sądu polowego.
13. **Wspólna lista umiejętności:** dla każdej klasy przypisać 2–3 umiejętności „swoje”.
14. **Mapa w dwóch warstwach:** szybkości poszczególnych sposobów marszu, środki transportu w liczbach, przeprawy.

Decyzje z 7 listopada (już wpisane do konceptu):
- jedna wspólna lista umiejętności;
- komisarz zależny od ustroju;
- 7 ekranów zamiast 12;
- mapa w dwóch warstwach ze sposobami ruchu;
- budynki z zestawu części post-apo;
- poprawione sprzeczności.
Sprzęt szkół zostaje osobny w każdej z 7 epok.

## B. Grafika (Blender → docelowo shader w Godocie)

1. **Budynki w stylu post-apo** (uwaga użytkownika):
   - mniej prostych domków z prostokąta i trójkątnego dachu;
   - wczesne epoki sklecone z wraków, kontenerów, blachy, opon i plandek;
   - kolejne epoki coraz bardziej klasyczne (cegła), ale zawsze trochę zrujnowane.
   Obecne 39 budynków to wersje z wczesnych epok w starym, prostszym stylu i trzeba je przerobić. Najpierw trzeba zrobić zestaw części post-apo (kontenery, blacha falista, palety, opony, plandeki, cegła z odzysku, gruz), a potem składać z niego budynki (koncept, sekcja Oprawa graficzna).
2. **Budynki późniejszych epok** (Proch, Para, Nabój, Silnik, Światło) jako osobne modele z animacjami (kominy, koła, maszyny) i efektami.
3. **Kompleks szkoleniowy:** Plac i 11 skrzydeł, każde w wersjach epok.
4. **Wygląd klas wojowników:** 11 szkół, klasy podstawowe, specjalizacje i mistrzowie, każda w sprzęcie swojej epoki (tabela „Sprzęt szkół w epokach” w koncepcie).
5. **Dym i ogień:** docelowe cząsteczki w Godocie: kłęby z cieniowaniem, języki ognia, żar, iskry, wybuchy, Mgła.
6. **Animacje:**
   - Stworzeń Mgły i ludzi zmienionych przez Mgłę;
   - specjalnych jednostek frakcji (Pokutnik z biczem, Sieciarz, Szaman, Elektryk…);
   - komisarza (okrzyk, zawracanie uciekających, egzekucja jako zdarzenie);
   - wynoszenia rannego;
   - psów bojowych.
7. **Poprawki:**
   - czworonogi w galopie są za długie;
   - Oddech i Kłębowisko mają za proste bryły;
   - twarze w portretach są toporne, więc trzeba osobnych, dokładniejszych głów do portretów w aktach.
8. **Render na żywo w Godocie:**
   - przenieść postproces (pasma cel-shadingu, przesunięcie barwy w cieniu, kontur 1 px z głębi, paleta, dithering w Mgle) do shaderów;
   - render do małej rozdzielczości i powiększenie bez wygładzania (opis w `PIPELINE_GRAFIKI.md`).

## C. Interfejs (uwagi użytkownika do makiet z 6 listopada)

1. Więcej stylizacji i detalu wszędzie: faktura papieru, zszywki, plamy, pismo odręczne, przybory na biurku. Liczby zostają czytelne.
2. **Nic nie może na siebie nachodzić.** Na ekranie Osady zakładki ekranów zasłaniają pasek dnia i czasu. Trzeba zaprojektować stałe strefy ekranu.
3. **Osada:** klik w budynek otwiera jego panel: rozbudowa, przydział ludzi, ustawienie produkcji.
4. **Kompleks:**
   - panel budynku szkoły z drzewkiem klas i opisem każdej klasy;
   - lista kursantów zgodnych z kursem (niezgodni wyszarzeni z powodem);
   - koszt przeszkolenia.
5. **Ludzie** (zakładka oceniona bardzo dobrze), do dodania:
   - przy każdej broni informacja: sprzęt klasy, pokrewny czy obcy;
   - druga kwalifikacja;
   - ukryte cechy jako rubryki „?”.
6. **Mapa:** znacznie większa, z przewijaniem i przybliżaniem, z większą liczbą węzłów i możliwości ruchu.
7. **Ratusz:** bardziej stylizowany, z dekretem „Rozstrzeliwanie na miejscu”.
8. Docelowo zostaje 7 ekranów:
   - Osada (panele budynków, w tym Kompleks i Akademia);
   - Ludzie;
   - Zbrojownia z Wzorami;
   - Mapa (z dyplomacją);
   - Ratusz;
   - Zeszyty (Badania, Bestiariusz, Kronika);
   - Walka i raport.
   Brakuje makiet: Zbrojowni, Zeszytów, paneli budynków i dyplomacji na mapie.

## D. Gra (Godot 4 + C#)

Patrz `PLAN_BUDOWY_GRY.md`. Nic jeszcze nie zaczęte.

## Interfejs v2 (7 listopada)

Plik `ui/interfejs_v2.html` to jedna strona HTML do otwarcia w przeglądarce. Plik `ui/interfejs_v2_zrodlo.html` to ta sama strona bez wklejonych obrazów.

- **Stałe strefy ekranu:**
  - góra: czas i zapasy;
  - lewa kolumna: zakładki;
  - prawa: panel kontekstowy;
  - dół: alarmy.
  
  Nic na siebie nie nachodzi.
- **7 ekranów:**
  - **Osada:** klik w budynek otwiera panel z ludźmi, produkcją i przebudową; jest też Kompleks z drzewkiem klas, opisem i kursantami zgodnymi z kursem.
  - **Ludzie:** broń z oznaczeniem sprzęt klasy, pokrewny albo obcy, druga kwalifikacja, ukryte cechy „?”, poglądy.
  - **Zbrojownia.**
  - **Mapa:** 150 węzłów, przeciąganie i przybliżanie, trasa z tempem marszu, listy frakcji.
  - **Ratusz:** podgląd skutków prawa, pieczątka, dekret „Rozstrzeliwanie na miejscu”.
  - **Zeszyty:** plansza epoki, Bestiariusz, Kronika z atramentem.
  - **Walka:** dymki i raport z osią czasu i pieczątkami ocen.
- **Efekty:**
  - wsuwanie kartek, pieczątki, drżenie przy alarmie;
  - lampa migocze według mocy;
  - Mgła na krawędziach przy słabym światle;
  - brud papieru przy głodzie, plamy tkanki przy Odmienności (przełączniki na dole ekranu).
- **Do zrobienia dalej:**
  - prawdziwe czcionki (maszynowa, odręczna);
  - dźwięki interfejsu;
  - ekran Akademii i dyplomacji jako osobne widoki;
  - więcej detalu biurka (kubek, przybory, mapa pod szkłem).
