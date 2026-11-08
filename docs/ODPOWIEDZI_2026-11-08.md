# Latarnia: odpowiedzi na decyzje przed startem

Wypełniono: 145 / 146 (8.11.2026, 12:58:28)

## Technika i start prototypu
- **T1 Język kodu i danych:** Wszystko po angielsku, polski tylko w tekstach gry
- **T2 Format danych gry i zapisu:** JSON na start (łatwy do czytania i poprawiania), szybszy format binarny później dla zapisów (polecane)
- **T3 Gdzie testujesz prototyp:** Instaluję u siebie Godot 4.7 .NET i .NET 8 SDK, uruchamiam projekt z folderu (polecane)
- **T4 Twój komputer (wpisz kartę graficzną i procesor w opinii):** — brak odpowiedzi —
  - opinia: gtx 1660 s i amd ryzen 7 5800x 8 rdzeni
- **T5 Prototyp walki: kształty czy od razu modele:** Od razu modele .glb z animacjami
- **T6 Obrót kamery:** Płynny, dowolny kąt
- **T7 Mody:** Tak, folder mods/ z danymi (broń, wrogowie, wydarzenia), bez skryptów (polecane)

## Walka: zasady
- **W1 Ranni a zabici:** 70% ranni do uratowania, 30% zabici od razu (polecane)
- **W2 Długość typowej nocnej obrony:** Długo: 3–6 minut, mniej walk
- **W3 Ile walk w grze:** 2–4 na tydzień gry (około 1 walka na 30–45 minut grania) (polecane)
- **W4 Tempo gry:** Osada do x4, walka tylko x1 i x2
- **W5 Kamera w kluczowych momentach:** Tak, sama przybliża i zwalnia, z możliwością wyłączenia (polecane)
- **W6 Amunicja:** Co do sztuki w składzie na wyprawie, w osadzie w paczkach po 20 (polecane)
- **W7 Zużycie i zacięcia broni:** Tak: zacięcia rosną przy złej konserwacji i obcym sprzęcie, broń się zużywa (polecane)
- **W8 Podnoszenie broni poległych:** Tak, sami, gdy skończy się amunicja albo stracą broń (polecane)
- **W9 Celowanie w strefy (głowa, nogi):** Każdy celuje w strefy zależnie od wyszkolenia
- **W10 Noc i dzień w walce:** W nocy widać tylko w świetle (latarnia, flary, latarki) i z Oczami Mgły (polecane)
- **W11 Pogoda w walce:** Tak: deszcz gasi ogień i ogranicza widoczność, śnieg spowalnia, wiatr rozwiewa dym i Oddech (polecane)
- **W12 Niszczenie pancerza:** Tak, pancerz traci punkty i wymaga naprawy w warsztacie (polecane)
- **W13 Jeńcy w walkach z ludźmi:** Tak: wrogowie się poddają, a o jeńcach decyduje ustrój i prawa (polecane)
- **W14 Psy i zwierzęta juczne:** Bez zwierząt w walce
- **W15 Walka we wnętrzach budynków:** Tak, budynki z piętrami jako osobne poziomy siatki (później niż prototyp) (polecane)
- **W16 Wysokość terenu:** Daje premię do celności, zasięgu i widoczności (polecane)
- **W17 Rozprzestrzenianie ognia:** Ogień tylko w miejscu trafienia
- **W18 Granaty:** Mogą odbić się od framugi, ale wrogowie nie odrzucają (polecane)
- **W19 Noce bez ataku:** Tak: krótkie zdarzenia (odgłosy, szept Mgły, zwiad Mgły, ślady rano) (polecane)

## AI dowódców i wojowników
- **A1 Architektura AI dowódcy:** Akceptuję w całości (polecane)
- **A2 Czy dowódca ma klasę i walczy:** Tak: dowódca to wojownik z klasą i profilem dowódcy, walczy mniej, bo myśli; klasa wpływa na styl (polecane)
- **A3 Charakter dowódcy:** Zmienia się powoli z przeżyciami (po klęsce ostrożny bardziej ostrożny) (polecane)
- **A4 Odmowa rozkazu:** Tak, przy niskim morale, złej relacji z dowódcą i cechach (Tchórzliwy, Odważny) (polecane)
- **A5 Dymki rozkazów:** Po polsku z imionami, 5–10 wariantów frazy na sytuację, zależnie od charakteru (polecane)
- **A6 Pamięć dowódcy o wrogach:** Tak, premia za osobiste doświadczenie z danym typem wroga (polecane)
- **A7 Co myśli dowódca: kiedy widać:** Tylko w powtórce, na żywo same dymki (polecane)
- **A8 Rywalizacja dowódców:** Lekka, przez relacje: zazdrość o awanse i chwałę (polecane)
- **A9 Dowódca bez Akademii:** Tak, do poziomu 2 (polecane)

## Prototyp walki: zawartość
- **P1 Klasy w prototypie:** 4 szkoły: walka wręcz (Tarczownik, Rębacz), strzelecka (Strzelec), sanitarna (Sanitariusz), zwiadu (Zwiadowca) (polecane)
- **P2 Wrogowie w prototypie:** Dodać Szabrowników (walka z ludźmi)
- **P3 Broń w prototypie:** Nóż, rura, maczeta, dzida, włócznia z tarczą, łuk z rurek, kusza, młot (polecane)
- **P4 Mapa prototypu:** Cała mała osada z kręgiem światła
- **P5 Skład w prototypie:** 6 osób z dowódcą, możliwość zmiany poziomu dowódcy (1–3) i specjalizacji do porównań (polecane)
- **P6 Raport po walce w prototypie:** Oś czasu, ocena decyzji dowódcy, wkład klas, diagnoza („potrzebny dowódca z Formacjami 3”) (polecane)

## Ludzie, klasy, cechy
- **L1 Katalog 52 klas (KLASY.md):** Akceptuję, ale chcę przejrzeć każdą szkołę osobno
- **L2 Ile ukrytych cech:** 1–3 ukryte obok 2–3 jawnych (polecane)
- **L3 Katalog ukrytych cech:** Rozpisz pełny katalog według propozycji (dobre i złe, warunki odkrycia) (polecane)
- **L4 Dzieci w szkołach klas:** Szkoły klas od 16 lat; wcześniej tylko szkoła dla dzieci i warsztaty (polecane)
- **L5 Starzenie się:** Tak, powolne: starsi słabną, mogą przejść do Starszyzny, rzadko umierają ze starości (polecane)
- **L6 Związki i narodziny:** W tle: pary, narodziny, żałoba, jako wydarzenia (polecane)
- **L7 Zmutowani jako dowódcy:** Tak, ale osie Mgły wpływają na posłuch (przy Czystości gorszy) (polecane)
- **L8 Postacie z historią:** 10–15 na kampanię, z wątkami osobistymi (polecane)
- **L9 Nazwy stopni wojskowych:** Jedne stopnie dla wszystkich
- **L10 Sylwetki ludzi:** 3 sylwetki i 2 wzrosty, kobiety i mężczyźni na wspólnym szkielecie (polecane)
- **L11 Portrety w aktach:** Osobne dokładniejsze głowy 3D renderowane jako pixel art 64×64 w sepii (polecane)

## Osada i ekonomia
- **O1 Przydział pracy:** Tabela priorytetów 1–4 dla rodzajów pracy (jak RimWorld), przydział sam; ręcznie można przypiąć (polecane)
- **O2 Transport surowców:** Wspólny magazyn w kręgu światła; tragarze tylko do posterunków i karawan (polecane)
- **O3 Pożary i zawalenia:** Tak: ogień, Przypływ, zawalenie przy ataku Rozrostu (polecane)
- **O4 Rozwój budynków:** Bez poziomów w epoce; przebudowa do kolejnej epoki za 60% kosztu nowego (polecane)
- **O5 Choroby i epidemie:** Tak: zimą i w ciasnych domach, z Kwarantanną i Szpitalem (polecane)
- **O6 Przestępczość:** Rozbudowana, z gangami
- **O7 Pieniądz:** Bez pieniądza w osadzie (wymiana); sól Gildii jako waluta handlowa od Kuźni (polecane)
  - opinia: tylko nie sól a jakiś rodzeaj monety
- **O8 Zmiana nocna:** Od Pary przy lampach gazowych (polecane)
- **O9 Drogi w osadzie:** Ścieżki powstają same od ruchu (polecane)
  - opinia: 1 + 2
- **O10 Wysokość terenu w osadzie:** Ma znaczenie: wzgórze lepsze na wieże, niecki zbierają Mgłę i wodę (polecane)
- **O11 Woda:** Pompa psuje się co jakiś czas (wydarzenie, naprawa złomem), zimą zamarza (polecane)
- **O12 Budynki z zestawu części post-apo:** Akceptuję kierunek i opisy (polecane)
- **O13 Racjonowanie a morale:** Zostaje tak: to dobry dylemat (polecane)

## Polityka i ustroje
- **X1 64 nazwy ustrojów (USTROJE.md):** Akceptuję jako robocze, poprawimy przy pisaniu tekstów (polecane)
- **X2 Przypisanie praw do 6 osi:** Claude przypisuje wszystkie prawa i dopisuje brakujące dla nowych osi, Ty przeglądasz (polecane)
- **X3 Wersje komisarza:** Wersję wyznaczają osie Władza i Porządek: 4 wersje (np. Rzecznik, Politruk, Żandarm, Komisarz); Kaznodzieja zależny od religii (polecane)
- **X4 Sejm: jak przechodzi prawo:** Głosowanie posłów z ruchów obywatelskich (liczba posłów ∝ poparciu); prawo potrzebuje większości; gracz może przekonywać ruchy przed głosowaniem (polecane)
  - opinia: wszystki 3
- **X5 Wybory i kadencje:** Raz w roku gry
- **X6 Autorytaryzm: napięcie i pucz:** Napięcie 0–100 rośnie od niepopularnych dekretów i porażek, spada od zwycięstw i święt; od 70 groźba puczu dowódców lub frakcji (polecane)
- **X7 Postać przywódcy:** W Autorytaryzmie jest przywódca z cechami (można go stracić, następca), w Demokracji wybierany przewodniczący; gracz to „wola osady” (polecane)
- **X8 Religia jako osobny system:** Rozbudowana: dogmaty, herezje, święci, jak w Crusader Kings
- **X9 Odejście frakcji wewnętrznych:** Tak, niezadowolona frakcja może odejść i założyć osadę Dezerterów (polecane)
- **X10 Zamach gracza:** Tak, wydarzenie z dużym kosztem Legitymacji i ryzykiem buntu (polecane)
- **X11 Propaganda:** Przez budynki i prawa (plakaty, radiowęzeł, pomnik), bez osobnego zasobu (polecane)
- **X12 Podgląd skutków prawa:** Tak: zmiana osi, kto się ucieszy, kto zbuntuje (procent mieszkańców), zmiana Legitymacji i „głosy z ulicy” (polecane)
- **X13 Jak pokazać 6 osi w Ratuszu:** 6 suwaków na kartce z pinezką kursu osady i rozrzutem poglądów mieszkańców pod spodem (polecane)
- **X14 Prawa nieodwracalne:** Więcej nieodwracalnych

## Frakcje
- **F1 Frakcje działają same na mapie:** Tak, z własnymi planami (rajdy, handel, sojusze, wojny między sobą) (polecane)
- **F2 Wzrost i upadek frakcji:** Rosną i giną bez gracza, ale wolno; mogą wchłonąć inną (polecane)
- **F3 Ile frakcji w kampanii:** 6–8 z 11, losowanych na start, plus stałe Zakon Latarników i Chór Mgły (polecane)
- **F4 Epoki frakcji:** Frakcje też się rozwijają, wolniej niż gracz (Zakon zawsze 2 epoki przed graczem) (polecane)
- **F5 Szabrownicy:** Akceptuję; wracają co porę roku z nowym Hersztem, część może przyłączyć się do osady (polecane)
- **F6 Synowie Czystości:** Akceptuję; mogą być sojusznikiem przy Czystości kosztem czystek; Wielki Inkwizytor jako boss (polecane)
- **F7 Chór Mgły:** Akceptuję; naturalny sojusznik przy Przemianie; Prorok Chóru jako boss (polecane)
- **F8 Ludzie Toru:** Akceptuję; pociąg można zdobyć, ale bardzo trudno, a bez nich tory pustoszeją (polecane)
- **F9 Łowcy Głów:** Akceptuję; można od nich kupować ludzi kosztem Legitymacji (taniej przy Indywidualizmie) (polecane)
- **F10 Zakon Latarników:** Akceptuję; zna część prawdy o Latarniach (droga do Teorii Światła) (polecane)
- **F11 Dezerterzy:** Akceptuję; można ich przywrócić wydarzeniem i amnestią (polecane)
- **F12 Bractwo Bunkra:** Akceptuję; obcych przyjmują tylko po długiej przyjaźni (polecane)
- **F13 Gildia Solna:** Akceptuję; może zadłużyć gracza, długi spłacane towarem, niespłacone kończą się najazdem najemników (polecane)
- **F14 Dzieci Lasu:** Akceptuję bez kary za wyrąb
- **F15 Republika Tamy:** Akceptuję; połączenie sieci to droga do zakończenia Sieć latarni (polecane)
- **F16 Przywódcy frakcji:** Każda frakcja ma przywódcę i 2–3 nazwanych dowódców z historią (polecane)

## Mapa świata i wyprawy
- **M1 Wielkość regionu:** Do wyboru: mały (100 węzłów), średni (180), duży (250) (polecane)
- **M2 Czas przejścia drogi:** 2–8 godzin gry na drogę (polecane)
- **M3 Składy na mapie:** Pionki ruszają się na żywo, czas płynie (polecane)
- **M4 Miejsca jednorazowe:** 20–30 na region (polecane)
- **M5 Pogoda na mapie:** Fronty pogodowe przesuwają się po regionie i zmieniają czas przejścia (polecane)
- **M6 Przypływ na mapie:** Widoczna fala Mgły z zapowiedzią na 1 dzień, zalewa drogi (polecane)
- **M7 Generator regionu:** Strefy (miasto, wieś, las, bagna, przemysł) rosnące ku Sercu Mgły, moduły dobierane według strefy i gęstości Mgły (polecane)
- **M8 Przechwytywanie karawan frakcji:** Tak, jako rajd z konsekwencjami dyplomatycznymi (polecane)

## Epoki i badania
- **E1 Zawartość epok (EPOKI.md):** Akceptuję jako bazę do balansu (polecane)
- **E2 Plansza epoki:** Cała widoczna od razu, przyszłe fazy zamazane (polecane)
- **E3 Porażka próby przełomu:** Może zabić: 10–20% szans na ofiary przy porażce (polecane)
- **E4 Fałszywe plany z wypraw:** Tak, rzadko; wykrywa je ktoś z Nauką 6+ (polecane)
- **E5 Tempo badań:** Akceptuję (UZUPELNIENIA.md, tabela tempa) (polecane)
- **E6 Cofanie epoki:** Nie, wiedza nie znika (polecane)

## Wrogowie
- **R1 Poziom zagrożenia z czasem:** Rośnie przez Przypływy i „Mgła się uczy”, a nie przez sztuczne skalowanie statystyk (polecane)
- **R2 Bossowie:** 2–3 fazy walki każdy (polecane)
- **R3 Wędrujące stada mutantów:** Tak, widoczne na mapie po zwiadzie (polecane)
- **R4 Nowi wrogowie (Gawra, Wodnik, Szerszenie, Tkacz, Gruczolak):** Dodać wszystkich 5 (polecane)

## Broń i sprzęt
- **B1 Warianty broni i modyfikacje (BRON.md):** Akceptuję (polecane)
- **B2 Imiona broni:** Broń, która zabiła wielu, zyskuje imię i premię do morale właściciela (polecane)
- **B3 Typy amunicji:** Od Naboju: zwykła, zapalająca, przeciwpancerna (polecane)
- **B4 Przeróbki sprzętu:** Tak, w warsztacie (polecane)

## Interfejs
- **U1 Stałe strefy ekranu:** Akceptuję (polecane)
- **U2 Interfejs reaguje na stan osady:** Tak, wszystko (polecane)
- **U3 Animacje interfejsu:** Wszystkie (polecane)
- **U4 Przedmioty na biurku:** Mają znaczenie (lampa = paliwo, kubek = morale, zegar = pora dnia) (polecane)
- **U5 Pismo odręczne:** Tylko w notatkach i podpisach, liczby zawsze drukiem (polecane)
- **U6 Powiadomienia:** Kartki przypinane do tablicy (polecane)
- **U7 Minimapa osady:** Bez minimapy, krąg światła jest mapą, z przybliżaniem (polecane)
- **U8 Brakujące makiety:** Zrobić w interfejsie v2 przed etapem osady (polecane)
- **U9 Statystyki kampanii:** W Kronice (polecane)

## Grafika
- **G1 Test shadera przed nowymi modelami:** Tak, najpierw test na 3 modelach, potem decyzje o modelach (polecane)
- **G2 Postacie w bitwie są małe i zlewają się:** Większe przybliżenie domyślne, mocniejsze barwy składu, obwódki pod jednostkami (polecane)
  - opinia: bez przyblizeniqa
- **G3 Zmutowani w osadzie:** Mutacje widoczne na modelu (trzecie ramię, chityna, świecące oczy) (polecane)

## Dźwięk i muzyka
- **D1 Styl muzyki:** Ambient, akordeon, smyczki i drony; w walce perkusja i niskie tony (polecane)
  - opinia: zalezne od ustroju i epoki i walki
- **D2 Głosy postaci:** Bez głosów
- **D3 Dźwięk Mgły:** Szum i szepty, które nasilają się z Odmiennością (polecane)
- **D4 Dźwięki interfejsu:** Maszyna do pisania, papier, pieczątka (polecane)

## Fabuła, wydarzenia, ton
- **Z1 Pula około 155 wydarzeń (WYDARZENIA.md, UZUPELNIENIA.md):** Akceptuję jako bazę; rozpisać do 250 później (polecane)
- **Z2 Narrator:** Tekstowy kronikarz osady komentuje dni w Kronice (polecane)
- **Z3 Archiwum Zasłony:** Tak, fragmenty sprzeczne ze sobą, odblokowują zakończenia (polecane)
- **Z4 Przemoc wobec dzieci na ekranie:** Pokazywać wprost jak resztę

## Kampania i ustawienia
- **K1 Poziomy trudności:** Kronika (łatwiej), Latarnia (zwykły), Mgła (twardy) (polecane)
- **K2 Tryb żelazny:** Opcjonalny (polecane)
- **K3 Starty kampanii:** 3 starty: Uciekinierzy (10 osób, mało zapasów), Kolejarze (8 osób, wagon z narzędziami), Odmieńcy (osada zmutowanych) (polecane)
- **K4 Ustawienia kampanii:** Wszystkie do ustawienia (polecane)
- **K5 Zwycięstwo:** Tylko 3 zakończenia i porażki, bez punktów (polecane)
- **K6 Gra po zakończeniu:** Tak, tryb „po końcu świata” (polecane)

## Wydanie gry
- **Y1 Cel projektu:** Gra do wydania na Steam (polecane)
- **Y2 Język gry:** Od razu polski i angielski
- **Y3 Wersja demo:** Pionowy wycinek Złomu jako demo (jedna zima, jeden region) (polecane)
