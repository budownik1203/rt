# Przed startem prac nad grą: pełna lista (8 listopada 2026)

Zebrane ze wszystkich dokumentów: `PYTANIA.md` (162 pytania), `DO_ZROBIENIA.md`, `ANALIZA_KRYTYCZNA.md`, `DECYZJE_2026-11-08.md`, oznaczenia „do potwierdzenia” i sprzeczności, które jeszcze zostały.

**Najważniejsze:** pierwszy etap gry to prototyp walki (`PLAN_BUDOWY_GRY.md`, etap 1). Do jego startu potrzeba tylko części A. Części B–E dotyczą osady, polityki i świata, czyli etapów 2–5. Można je ustalać równolegle z kodowaniem walki i nie blokują startu.

Oznaczenia:
- 🔴 blokuje start prototypu walki;
- 🟡 potrzebne przed etapem 2 (osada);
- ⚪ można później;
- (Pnn) to numer pytania w `PYTANIA.md`, gdzie jest propozycja odpowiedzi.

---

## A. 🔴 Blokuje prototyp walki

### A1. Decyzje techniczne (szybkie, propozycje gotowe)

1. **Kod po angielsku, dane i teksty po polsku** (P156, propozycja: tak).
2. **Format danych i zapisu: JSON** na start (P157, propozycja: tak).
3. **Twój komputer do testów:**
   - jaka karta graficzna i procesor (P160);
   - czy instalujesz u siebie Godota 4.7 .NET i .NET 8 SDK, żeby uruchamiać prototyp.
4. **Kamera:** obrót płynny (tak mówi koncept) czy skokowy co 90° (P143 proponuje skokowy, bo pixel art lepiej wygląda w stałych kątach). To sprzeczność do rozstrzygnięcia przed testem shadera.
5. **Prototyp najpierw na prostych kształtach czy od razu na modelach .glb** (`PLAN_BUDOWY_GRY.md`, otwarta decyzja; propozycja: kształty, żeby szybko sprawdzić AI, a modele po teście shadera).

### A2. Zasady walki (propozycje w `PYTANIA.md`, wystarczy „zgoda” albo zmiana)

6. **Architektura AI dowódcy** z `TECHNIKA_AI_I_MECHANIKI.md`: 5 kroków, histereza, ocena użyteczności (P31 ★).
7. **Ranni a zabici:** 70% wyłączonych z walki to ranni do uratowania, 30% zabici (P16 ★).
8. **Długość nocnej obrony:** 1–3 minuty przy tempie x1 (P6). Tempa gry x1, x2, x4, x8, a walka tylko do x4 (P8).
9. **Kamera w walce:** sama przybliża się w kluczowych momentach, z możliwością wyłączenia (P4).
10. **Pakiet szczegółowych zasad walki (P18–P30):**
    - amunicja co do sztuki;
    - zacięcia i zużycie broni;
    - podnoszenie broni poległych;
    - celowanie w strefy tylko przez Snajpera;
    - noc i dzień;
    - wysokość daje premię;
    - ogień się rozprzestrzenia;
    - granaty się odbijają.

    Proponuję zatwierdzić pakietowo. Pogoda, jeńcy, psy i wnętrza budynków mogą wejść później.
11. **AI wojowników i dowódcy (P32–P40):**
    - dowódca ma klasę i walczy;
    - charakter zmienia się powoli;
    - odmowa rozkazu przy niskim morale;
    - dymki po polsku z imionami, 5–10 fraz na sytuację;
    - myśli dowódcy widać tylko w powtórce.

### A3. Dane do prototypu

12. **Klasy w prototypie:** 4 szkoły (wręcz, strzelecka, sanitarna, zwiadu), po 1–2 klasy (P41 ★). Liczby zdolności są w `KLASY.md`, ale całość jest oznaczona jako „propozycja do potwierdzenia”.
13. **Jednostki w `WROGOWIE.md`:** prędkości i zasięgi zmysłów trzeba przeliczyć na metry i metry na sekundę. Dziś część jest w starych kratkach („prędkość 8”), a część w metrach („krzyk w promieniu 15 m”). To mogę zrobić sam.
14. **Mapa prototypu:** fragment osady z palisadą i bramą. Trzeba ustalić, jakie budynki na nim stoją (propozycja: 6–8 budynków Złomu, palisada z bramą, latarnia na mocy 1).

### A4. Grafika potrzebna do widoku walki

15. **Test shadera pixel art w Godocie** na 3 modelach (P141 ★). Mogę go zrobić sam, a Ty oceniasz wynik.
16. **Eksport modeli:**
    - scalanie około 50 siatek postaci w jedną;
    - ukrywanie błysku z lufy przy wczytaniu;
    - broń przypięta do dłoni.

    To technika, robię sam.

---

## B. 🟡 Przed etapem 2 (osada)

### B1. Decyzje o osadzie

17. **Tabela priorytetów pracy** jak w RimWorldzie (P51 ★).
18. **Wspólny magazyn w świetle**, a tragarze tylko do posterunków i karawan (P52 ★).
19. **Podgląd skutków prawa przed uchwaleniem** (P64 ★).
20. **Pakiet osady (P53–P63):**
    - pożary i zawalenia;
    - przebudowa do kolejnej epoki za 60% kosztu zamiast poziomów;
    - choroby i epidemie;
    - lekka przestępczość;
    - brak pieniądza, sól Gildii jako waluta handlowa;
    - ścieżki powstają same;
    - wysokość terenu ma znaczenie.
21. **Woda:** po naprawie pompy przestaje się liczyć (wynik `ekonomia_sim2.py`). Albo pompa się psuje (wydarzenie), albo trzeba zmniejszyć jej wydajność.
22. **Ludzie (P42–P50):**
    - ukryte cechy (1–3 obok 2–3 jawnych);
    - starzenie się;
    - pary i narodziny;
    - zmutowani jako dowódcy;
    - bohaterowie unikalni;
    - portrety 64×64.

    Nazwy stopni (P42) opierają się dziś na starych ustrojach i trzeba je przepisać na osie.

### B2. Grafika i interfejs osady

23. **Zestaw części post-apo i nowe budynki Złomu.** Obecne 39 budynków to „domki z dachem”, które sam odrzuciłeś.
24. **Interfejs v2:** stałe strefy ekranu (P130 ★) i interfejs reagujący na stan osady (P131 ★). Brakujące makiety: Zbrojownia, Zeszyty, panel budynku, dyplomacja, Akademia (P140).
25. **Kurs osady na 6 osiach w Ratuszu:** P137 proponuje trójkąt, a to pasowało do 3 osi. Potrzebny nowy pomysł na pokazanie 6 osi.

---

## C. 🟡 Koncept do domknięcia (polityka, ludzie, treść)

26. **Przypisanie wszystkich praw do 6 osi** (`USTROJE.md`, do przeliczenia).
27. **Wersje komisarza według osi** Władza i Porządek (dziś według 6 starych ustrojów).
28. **Około 27 wzmianek o starych ustrojach** w koncepcie (wydarzenia, mutacje, standaryzacja) do przepisania na bieguny. Robię sam.
29. **Przegląd 64 nazw ustrojów** (`USTROJE.md`).
30. **Religia jako osobny system.** Postanowiliśmy, że nie jest osią, ale nie ma jeszcze projektu. P68 proponuje Kult Latarni i Kościół Mgły, które były starymi ustrojami.
31. **Sejm i wybory:** jak działają głosowania, ruchy obywatelskie i kadencje (P65 opisuje stare wybory w Radzie).
32. **Autorytaryzm:** liczby wskaźnika napięcia i puczu, postać przywódcy (P66, P67).
33. **Poziomy trudności:** Kronika, Latarnia, Mgła (P11 ★), tryb żelazny i ustawienia kampanii (P12–P15).
34. **Przemoc wobec dzieci nigdy na ekranie:** propozycja Claude do potwierdzenia (koncept, sekcja Lore).
35. **Potwierdzenie gotowych propozycji:**
    - `KLASY.md` (52 klasy);
    - `WYDARZENIA.md` i `UZUPELNIENIA.md` (około 155 wydarzeń, ukryte cechy, postacie);
    - `BRON.md` (P125 ★);
    - zawartość epok (P113 ★).

---

## D. ⚪ Świat, frakcje i reszta (etapy 3–5)

36. **Frakcje działają same na mapie** (P74 ★) **i rosną albo giną bez gracza** (P75 ★). Strategie 11 frakcji do potwierdzenia (P77–P105).
37. **Mapa świata (P106–P112):** wielkość regionu, czas przejścia drogi, pogoda na mapie, Przypływ jako fala. Do zrobienia: generator i katalog modułów.
38. **Epoki (P114–P119):**
    - plansza epoki z zamazanymi przyszłymi fazami;
    - porażka próby przełomu może zabić;
    - fałszywe plany z wypraw.
39. **Wrogowie (P121–P124):**
    - poziomy zagrożenia;
    - fazy bossów;
    - wędrujące stada;
    - 5 nowych wrogów z `WROGOWIE.md`.
40. **Broń (P126–P129):**
    - imiona broni;
    - typy amunicji od Naboju;
    - przeróbki w warsztacie.
41. **Dźwięk i muzyka (P148–P151):** styl muzyki (P148 ★), okrzyki po polsku, dźwięk Mgły, dźwięki interfejsu.
42. **Fabuła (P152–P155):** narrator Kroniki, Archiwum Zasłony.
43. **Rdzeń kampanii (P7, P9, P10):**
    - noce bez ataku z własnymi wydarzeniami;
    - brak zwycięstwa punktowego;
    - gra po zakończeniu.
44. **Mody** z folderu `mods/` (P158). Bez trybu wieloosobowego (P159).

---

## E. Porządki w dokumentach (robię sam, bez decyzji)

45. **`PYTANIA.md`:** oznaczyć pytania już rozstrzygnięte 8 listopada (P1, P2, P3, P17, P106, P120, P142, P153, P161, P162). P3 i P153 rozstrzygnięto inaczej, niż proponowano: każda walka jest oglądana, a treści są pokazywane wprost.
46. **`CLAUDE.md`:** nieaktualny „Stan projektu (6 listopada)”, opis ustrojów w „Grze w trzech zdaniach” i zasada „zip po każdej zmianie” (teraz jest git).
47. **`RANO_DO_SPRAWDZENIA.md`:** wszystko z niego jest już rozstrzygnięte, więc trzeba go oznaczyć jako zamknięty.
48. **`UZUPELNIENIA.md`:** znacznik [zmiana] przy starej ekonomii (już oznaczony jako nieaktualny).
49. **`WROGOWIE.md`:** jednostki prędkości i zasięgów (punkt 13).

---

## Proponowana kolejność

1. **Jeden formularz z decyzjami z części A** (punkty 1–12, 14). Większość ma gotowe propozycje, więc to około 10 minut.
2. **Ja robię sam:**
   - porządki z części E;
   - przeliczenie jednostek wrogów;
   - test shadera;
   - eksport modeli.
3. **Start prototypu walki.** W tym czasie, w wolnych chwilach, przechodzimy części B i C formularzami.
