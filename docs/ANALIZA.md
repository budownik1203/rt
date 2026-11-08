# Analiza projektu i propozycje zmian (7 listopada 2026)

Analiza całego konceptu: co działa, co jest ryzykiem, co się dubluje i czego brakuje. Każda propozycja ma postać „jest → proponuję” i czeka na decyzję użytkownika. Pytania do nich są w `PYTANIA.md`.

---

## A. Co jest mocne (nie ruszać)

- **Rdzeń:** latarnia i paliwo jako zegar presji. Promień światła jest naraz miejscem do budowy, długością obwodu do obrony i przynętą dla mutantów, więc jedna liczba łączy ekonomię, obronę i walkę.
- **Dowódca jako zasób.** AI z ludzkimi ograniczeniami zamiast losowych błędów, a do tego raport, który mówi, która specjalizacja zawiodła. Tego nie ma prawie żadna gra.
- **Polityka z poglądów ludzi**, a trzecia oś (Czystość ↔ Przemiana) wyrasta wprost ze świata.
- **Epoki jako długie etapy z przełomem**, z odpowiedzią Mgły po każdym kroku naprzód.
- **Spójny styl:** 3D low-poly jako pixel art, papierowy interfejs, polski, swojski krajobraz.

## B. Największe ryzyka i propozycje

### B1. Gracz bierny w walce

- **Jest:** w walce gracz tylko patrzy. Ma jedną flarę i rozkazy przed wyprawą.
- **Ryzyko:** po kilkudziesięciu walkach oglądanie nudzi, a walki w tle odbierają poczucie wpływu.
- **Proponuję:**
  1. **Plan przed walką** dla obrony i ataku zaplanowanego: gracz zaznacza na mapie punkt zbiórki, kierunek głównego zagrożenia i strefę, której nie wolno oddać. Dowódca traktuje to jako zadania stałe, a raport ocenia też plan gracza.
  2. **Reżyserowana kamera** w kluczowych momentach: najazd na flankę, zbliżenie na upadek dowódcy, krótkie spowolnienie.
  3. **Druga flara od epoki Prochu** (Dzwony alarmowe) i trzecia od Silnika (Radio). Wpływ gracza rośnie z technologią, ale zawsze jest mały.

### B2. Skala zarządzania ludźmi

- **Jest:** do 400 ludzi, każdy z poglądami, cechami, relacjami i potrzebami. Przydział ręczny.
- **Ryzyko:** od 50 osób mikrozarządzanie zabija przyjemność.
- **Proponuję:**
  1. **Tabela priorytetów pracy** jak w RimWorldzie: dla każdej osoby priorytety 1–4 dla rodzajów pracy. Przydział robi się sam.
  2. **Zarządca budynku** od Pary (Urząd ewidencji), który sam dobiera ludzi.
  3. **Teczka staje się rejestrem** z filtrami przy ponad 40 osobach. Pojedyncze akta otwiera się tylko na żądanie albo przy zdarzeniu.

### B3. Czas osady a wyprawy i walki

- **Jest:** czas osady zatrzymuje się, gdy oglądasz walkę. Wyprawy trwają dni, a składów może być kilka.
- **Ryzyko:** dwie walki naraz (obrona i wyprawa) albo walka w czasie budowy.
- **Proponuję:**
  - **Kolejka walk.** Gdy wybucha walka, gra pauzuje i pyta: oglądać czy rozstrzygnąć w tle. Kilka walk naraz ogląda się po kolei; symulacja reszty czeka w zamrożonym stanie.
  - **Rozstrzygnięcie w tle** zawsze tą samą symulacją.

### B4. Śmiertelność i przywiązanie

- **Jest:** realistyczna śmiertelność, nazwani weterani, kampania około 80 godzin.
- **Ryzyko:** gracz traci ulubieńców przez jeden pech i kończy grę.
- **Proponuję:**
  - Około 70% wyłączeń z walki to ranni, którzy przeżyją, jeśli zostaną wyniesieni. Śmierć następuje głównie przez brak pomocy, Opiekunkę albo przegraną walkę.
  - Trwałe rany i protezy jako ślad, nie koniec.
  - **Poziomy trudności:** Kronika (łatwiej), Latarnia (zwykły), Mgła (twardy).
  - **Tryb żelazny** (jeden zapis) jako opcja, nie domyślnie.

### B5. „Mgła się uczy” musi być czytelna

- **Jest:** mutanty ewoluują w odpowiedzi na Wzory gracza.
- **Ryzyko:** gracz czuje się karany, a nie wie za co.
- **Proponuję:**
  - **Licznik adaptacji** dla 6 kategorii: światło, pancerz, broń palna, ogień, wręcz, cisza.
  - Przekroczenie progu daje zapowiedź w raporcie zwiadu na 2–3 dni przed zmianą.
  - Adaptacja słabnie, gdy gracz zmieni styl.
  - Najwyżej 2 aktywne adaptacje naraz.

### B6. Treść ponad siły

- **Jest:**
  - 7 epok, około 170 technologii i 11 szkół z około 55 klasami;
  - 11 frakcji z około 80 jednostkami i około 40 typów wrogów;
  - budynki w wersjach epok.
- **Ryzyko:** projekt nie dojdzie do grywalnej gry.
- **Proponuję pionowy wycinek** jako pierwszy cel po prototypie walki:
  - epoka Złomu w pełni;
  - jeden region (około 40 węzłów);
  - 4 szkoły: walki wręcz, strzelecka, sanitarna, zwiadu;
  - 8 wrogów: Szczurza Fala, Ogar, Wataha, Krzykacz, Nosiciel, Biegacz, Pajęczarz, Szabrownicy;
  - 2 frakcje;
  - pierwsza zima i pierwszy Przypływ.
  
  Reszta wchodzi epoka po epoce.

### B7. Polityka mało przejrzysta

- **Jest:** 3 osie, 6 ustrojów, drzewo praw, Legitymacja, frakcje wewnętrzne, poglądy każdego.
- **Ryzyko:** gracz nie rozumie, dlaczego wybuchł strajk.
- **Proponuję:**
  - **Podgląd skutków prawa przed uchwaleniem:** przesunięcie kursu, kto się ucieszy, a kto zbuntuje (procent mieszkańców) i zmiana Legitymacji.
  - **„Głosy z ulicy”:** 3 cytaty mieszkańców na Ratuszu z powodami niezadowolenia.

### B8. Dowódca a klasa

- **Jest:** dowódca ma osobny rozwój (Akademia, poziom i 3 specjalizacje), a wojownik ma klasę ze szkoły.
- **Niejasne:** czy dowódca ma też klasę i czy walczy.
- **Proponuję:** dowódca to zwykły wojownik z klasą i stopniem, któremu dodatkowo nadano profil dowódcy. Walczy, choć mniej, bo myśli. Klasa wpływa na jego styl: strzelec wyborowy jako dowódca trzyma dystans.

### B9. Interfejs: diegetyczny, ale czytelny

- **Jest:** papier, teczki, pieczątki.
- **Proponuję:**
  - **Stałe strefy ekranu:** góra to czas i zapasy, lewa kolumna to zakładki, prawa to panel kontekstowy, dół to alarmy.
  - **Katalog animacji interfejsu:**
    - wsuwanie kartki;
    - uderzenie pieczątki;
    - przewracanie strony w teczce;
    - migotanie lampy biurka zależne od paliwa;
    - Mgła wpełzająca na krawędzie ekranu przy niskim świetle;
    - drżenie papieru przy alarmie;
    - atrament, który „wsiąka” przy nowym wpisie w Kronice.
  - **Interfejs reaguje na stan osady:** brudniejszy papier przy głodzie, czerwone pieczątki przy zagrożeniu, plamy tkanki na kartkach przy wysokiej Odmienności.

### B10. Brakujące systemy

Nie są nigdzie opisane. Każdy jest w pytaniach.

- Dźwięk i muzyka: styl, dźwięki interfejsu, dźwięki Mgły, okrzyki po polsku.
- Samouczek, czyli pierwsze dni jako prowadzony wstęp.
- Poziomy trudności i ustawienia kampanii (ziarno, wielkość regionu, częstość Przypływów).
- Zapis, powtórki walk, statystyki kampanii, osiągnięcia.
- Ułatwienia dostępu (skalowanie interfejsu, czcionka bez stylizacji, tryby dla daltonistów).
- Treści drażliwe: praca dzieci, przetwarzanie zmarłych, egzekucje. Potrzebny ton, granice i ostrzeżenia na starcie.

## C. Dublowanie i niespójności (do poprawy)

| Jest | Proponuję |
| --- | --- |
| Kłębowisko i Rozrost to dwie masy tkanki | Kłębowisko to młody Rozrost: rośnie, gdy pożera trupy, i po kilku walkach staje się Rozrostem |
| Siewca, Matka Roju i Zarośl rodzą mutanty | Zarośl to teren (anomalia), Siewca to gniazdo na Zarośli, a Matka Roju to Siewca, który przetrwał Przypływ (boss) |
| Biegacz, Odmieniec, Śpiący, Wydrążony, Kokon to osobne typy | jedna **linia przemiany**: Kokon → Biegacz → Odmieniec → Wydrążony; Śpiący to stan uśpienia Odmieńców w Mgle |
| Naśladowca i Fałszywy Latarnik wabią | Fałszywy Latarnik to odmiana Naśladowcy, która naśladuje światło zamiast głosu |
| Nici i Opiekunka wciągają ludzi w Mgłę | Nici to anomalia terenu (krawędź światła), Opiekunka to stworzenie, które porywa rannych |
| Rozrost jest w grupie „Ludzie zmienieni” | przenieść do Stworzeń Mgły |
| Surowce: chityna z Ogara | Ogar to zwykły pies, więc chityna pochodzi tylko ze Stworzeń Mgły i Rozrostu, a z Ogara jest skóra i tłuszcz |
| Tabela technologii: Plac ćwiczeń w epoce Kuźni | Plac ćwiczeń jest znany od startu (z niego wychodzi kompleks szkół) |
| Pracownia i Prosektorium | Pracownia (badania) i Prosektorium (Bestiariusz) zostają osobno, ale Prosektorium wymaga Pracowni |
| Sztandary: plakaty, radiowęzeł i pomnik w osadzie od początku | plakaty od Złomu (Sztandar), Pomnik od Pary, Radiowęzeł od Silnika |
| Oddech, Ćma, Nosiciel i Niosący Zarodnik to cztery wrogi z zarodnikami | zostają, ale z różnymi rolami: Oddech to ruchoma anomalia, Ćma leci na światło, Nosiciel wybucha przy śmierci, Niosący Zarodnik to człowiek-bomba Chóru |

## D. Liczby, których brakuje

- **Tempo badań w epokach:** punkty nauki na badacza i budynki nauki (Pracownia, Laboratorium) w kolejnych epokach.
- **Ekonomia po 10. dniu:** zima, Przypływ, populacja 20–60; symulacja do poprawienia.
- **Klasy:** zdolności każdej klasy w liczbach (zasięgi, odnowienia, premie).
- **Bestiariusz w liczbach:** szkic jest w `WROGOWIE.md`.
- **Frakcje:** siła, wzrost, reakcje i strategie. Szkice są w `PYTANIA.md`.
- **Wydarzenia:** pełna pula. Jest około 15 przykładów, potrzeba około 150.

## E. Proponowana kolejność dalszych prac

1. Odpowiedzi na pytania z `PYTANIA.md`, najpierw sekcje oznaczone ★.
2. Poprawki z sekcji C w koncepcie.
3. Prototyp walki w Godocie: symulacja + proste kształty + dymki + raport. Plan w `TECHNIKA_AI_I_MECHANIKI.md`.
4. Test shadera pixel art w Godocie na 3 modelach z paczki.
5. Interfejs v2 według B9.
6. Pionowy wycinek Złomu (B6).
