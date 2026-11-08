# Latarnia — dokument koncepcyjny

Nov 5, 2026 · @budownik1203

## Wizja i pętla rozgrywki

Latarnia to grimdarkowa gra post-apo o zarządzaniu osadą, w której jedyną ochroną przed żywą, mutującą Mgłą jest światło starej latarni. Widok z góry po przekątnej: 3D low-poly renderowane jako pixel art, czas rzeczywisty z pauzą, kampania na około 80 godzin (7 epok, każda około roku gry).

Gracz nie steruje walką. Buduje osadę, ustala prawa, wyposaża ludzi i szkoli dowódców, a potem obserwuje, jak jego składy radzą sobie same.

**Pętla rozgrywki:**

1. **Dzień:** przydział ról, budowa, produkcja, prawa.
2. **Wyprawy:** składy wychodzą w Mgłę po złom, paliwo, ludzi i wiedzę, eskortują karawany, prowadzą dyplomację.
3. **Walki:** toczą się same, gracz obserwuje, a po walce dostaje raport.
4. **Noc:** Mgła napiera, mutanty atakują osadę.
5. **Rozwój:** łupy, wiedza z Bestiariusza i paliwo zasilają technologie, Akademię i politykę, co zmienia ludzi i kolejne walki.

**Wyróżniki:**

- **Dowódca jako zasób:** wynik walki zależy od tego, jak myśli lider składu, nie tylko od sprzętu.
- **Mutacja jako pokusa:** daje przydatne zdolności, ale podnosi Odmienność i zmienia poglądy ludzi.
- **Polityka wyrastająca ze świata:** trzecia oś ustrojów to Czystość ↔ Przemiana.
- **Odczuwalna skala:** od „starego karabinu Zenka” do tabel Wzorów mikropaństwa.
- **Epoki technologiczne:** od dzid i łuków do coraz lepszego sprzętu.

## Decyzje z ankiety (8 listopada 2026)

Odpowiedzi na 146 pytań z ankiety są w `ODPOWIEDZI_2026-11-08.md`. Ta sekcja streszcza ustalenia, które zmieniają albo uzupełniają koncept. Przy sprzeczności z resztą dokumentu ważniejsza jest ta sekcja.

**Technika i wydanie:**

- Kod i klucze danych po angielsku, po polsku tylko teksty gry.
- Dane w JSON (zapisy później w formacie binarnym). Mody z folderu `mods/`: dane, bez skryptów.
- Gra trafi na Steam, od razu po polsku i angielsku, więc teksty od pierwszego dnia są w plikach tłumaczeń. Demo to pionowy wycinek Złomu (jedna zima, jeden region).
- Testy na komputerze użytkownika: GTX 1660 Super i Ryzen 7 5800X. Budżet grafiki ustawiamy pod tę kartę: render w 640×360, mało świateł z cieniami naraz, scalone siatki postaci.
- Prototyp od razu na modelach `.glb` z animacjami, ale po teście shadera.
- **Kamera obraca się płynnie, pod dowolnym kątem.** Pixel art przy obrocie migocze, więc przyciągamy kamerę do siatki tekseli i stabilizujemy kontur.

**Walka:**

- 70% wyłączonych z walki to ranni do uratowania.
- **Nocna obrona trwa 3–6 minut.** Walki są 2–4 na tydzień gry i każda jest oglądana.
- **Tempo:** osada do x4, walka tylko x1 i x2 oraz pauza.
- Kamera sama przybliża i zwalnia w kluczowych momentach (można wyłączyć), ale **domyślnego przybliżenia na start walki nie ma**. Czytelność składu dają mocniejsze barwy i obwódki pod jednostkami.
- **Każdy celuje w strefy ciała** z precyzją zależną od wyszkolenia (Snajper najlepiej).
- **Ogień pali się tylko w miejscu trafienia**, bez rozprzestrzeniania po terenie.
- **Bez zwierząt gracza w walce.** Wrogie zwierzęta i mutanty zostają. Do decyzji: co ze szkołą Tresury i Psiarzem.
- Zatwierdzone propozycje:
  - amunicja co do sztuki;
  - zużycie i zacięcia;
  - podnoszenie broni;
  - noc tylko w świetle;
  - pogoda;
  - niszczenie pancerza;
  - jeńcy;
  - wnętrza budynków (po prototypie);
  - wysokość;
  - odbicia granatów;
  - zdarzenia nocy bez ataku.
- AI dowódców przyjęte w całości (wszystkie propozycje A1–A9).

**Prototyp walki:**

- 4 szkoły: Tarczownik, Rębacz, Strzelec, Sanitariusz, Zwiadowca. Skład ma 6 osób, a poziom dowódcy (1–3) i jego specjalizacje można zmieniać.
- Broń: nóż, rura, maczeta, dzida, włócznia z tarczą, łuk z rurek, kusza, młot.
- **Wrogowie: Szczurza Fala, Ogar, Krzykacz, Nosiciel i Szabrownicy**, czyli od razu walka z ludźmi, gdzie wróg ma tego samego „mózg” dowódcy.
- **Mapa: cała mała osada z kręgiem światła.**
- Raport po walce: oś czasu, ocena decyzji, wkład klas, diagnoza.

**Ludzie:**

- Szkoły klas od 16 lat.
- Ukryte cechy: 1–3 obok 2–3 jawnych, z pełnym katalogiem do rozpisania.
- Starzenie się jest powolne, a pary i narodziny dzieją się w tle.
- Zmutowani mogą być dowódcami, ale oś Mgły wpływa na posłuch.
- 10–15 postaci z historią na kampanię.
- **Jedne stopnie wojskowe dla wszystkich ustrojów.**
- 3 sylwetki i 2 wzrosty, portrety 64×64 w sepii.
- Katalog klas jest przyjęty, ale użytkownik chce przejrzeć każdą szkołę osobno.

**Osada:**

- Zatwierdzone propozycje:
  - tabela priorytetów pracy;
  - wspólny magazyn;
  - pożary i zawalenia;
  - przebudowa do epoki za 60% kosztu;
  - choroby;
  - zmiana nocna od Pary;
  - wysokość terenu;
  - budynki z zestawu części post-apo;
  - racjonowanie jako dylemat.
- **Przestępczość rozbudowana, z gangami** (system do zaprojektowania).
- **Pieniądz:** w osadzie wymiana, a w handlu moneta (nie sól) od Kuźni.
- **Drogi:** ścieżki powstają same od ruchu, a gracz może też układać drogi, które przyspieszają ruch.
- **Pompa psuje się co jakiś czas i zamarza zimą.** Studnia odzyskuje sens.

**Polityka:**

- 64 nazwy ustrojów przyjęte roboczo.
- Prawa do 6 osi przypisuje Claude, a użytkownik przegląda.
- 4 wersje komisarza według osi Władza i Porządek, a Kaznodzieja zależy od religii.
- **Sejm łączy trzy mechanizmy:** głosowanie posłów z ruchów, referendum i doradztwo. Do decyzji: kiedy który.
- **Wybory raz w roku gry.**
- Autorytaryzm: napięcie 0–100 i pucz od 70. Przywódca z cechami i następcą.
- **Religia rozbudowana:** dogmaty, herezje, święci, w stylu Crusader Kings (system do zaprojektowania).
- Pozostałe ustalenia:
  - frakcje wewnętrzne mogą odejść;
  - gracz może przeprowadzić zamach;
  - propaganda działa przez budynki i prawa;
  - podgląd skutków prawa;
  - 6 suwaków osi w Ratuszu;
  - **więcej praw nieodwracalnych.**

**Frakcje, mapa, epoki, wrogowie, broń:**

- Wszystkie propozycje przyjęte, z jednym wyjątkiem: **Dzieci Lasu nie karzą za wyrąb.**
- 5 nowych wrogów (Gawra, Wodnik, Szerszenie, Tkacz, Gruczolak) dochodzi do gry.

**Interfejs, grafika, dźwięk, fabuła, kampania:**

- Wszystkie propozycje interfejsu przyjęte, a test shadera robimy przed nowymi modelami.
- **Muzyka zależy od ustroju, epoki i walki**, czyli jest adaptacyjna.
- **Bez głosów postaci.**
- Fabuła i kampania według propozycji:
  - wydarzenia;
  - tekstowy kronikarz;
  - Archiwum Zasłony;
  - trzy poziomy trudności i tryb żelazny;
  - 3 starty i wszystkie ustawienia kampanii;
  - tylko zakończenia, bez punktów;
  - gra po końcu świata.
- **Treści drażliwe, także przemoc wobec dzieci, są pokazywane wprost** (decyzja użytkownika).

## Świat

Akcja dzieje się w fikcyjnej Europie Środkowej po nadejściu Mgły, z polskimi imionami i swojskim krajobrazem: bloki, ruiny miasteczek, lasy, bagna, tory.

- **Mgła:** żywa anomalia. Obniża widoczność, podnosi Odmienność każdego, kto w niej przebywa, i rodzi mutanty.
- **Latarnia:** jej światło odpycha Mgłę. Promień zależy od paliwa: większy daje więcej miejsca na budowę, ale też dłuższy obwód do obrony.
- **Terytorium:** osada rośnie przez latarnie pomocnicze i posterunki świetlne dalej w Mgle. Każda potrzebuje paliwa i obrony. Państwo to dosłownie sieć światła na mapie.
- **Przypływy:** co jakiś czas fala Mgły zalewa część osady. Budynki poza światłem zarastają tkanką: można je oczyścić albo zostawić, bo wtedy produkują dziwne surowce. Po każdym Przypływie mutanty ewoluują.
- **Pory roku:** zima (więcej paliwa na ogrzewanie, mniej jedzenia, gęstsza Mgła), wiosna (Przypływ), lato (sezon wypraw i handlu), jesień (żniwa i przygotowania).
- **Inne światła:** na mapie są inne osady z własnymi latarniami i ustrojami. Można z nimi handlować, walczyć albo ukraść im paliwo.
- **Pamięć osady:** cmentarz i Kronika z imionami poległych i tym, jak zginęli. Dowódca po wielu walkach staje się legendą, a jego śmierć mocno uderza w morale.
- **Zakończenia:** zgasić Mgłę, przemienić całą osadę albo zbudować sieć latarni. Finał rozgrywa się przy Sercu Mgły.

## Lore i kampania

Mgła to skutek przedwojennego eksperymentu, a latarnie to resztki sieci, która miała ją trzymać w ryzach. Kampania jest czystym sandboxem bez wątku głównego: prawdę składa się z fragmentów, a opowieść tworzą ludzie osady. Nazwy własne są robocze.

**Przeszłość (robocza wersja):**

- **Program Zasłona:** tajny program wojskowo-naukowy, który miał stworzyć samoodnawiającą się zasłonę, nieprzenikalną dla wrogiego rozpoznania i zdolną ukryć całe terytorium.
- **Co poszło nie tak:** zasłona okazała się żywa. Zaczęła się rozmnażać i przerabiać wszystko, co w niej przebywa: tak powstały mutanty i Odmienność.
- **Pasterze:** program przewidywał operatorów połączonych z Mgłą, którzy mieli ją kierować. Pasterze to to, co z nich zostało.
- **Sieć latarni:** zabezpieczenie programu. Światło o określonym widmie, przepuszczone przez specjalne soczewki, unieszkodliwia Mgłę. Latarnie stały pierścieniem wokół ośrodków programu. Większość zgasła, a twoja należy do nielicznych działających.
- **Zakon Latarników** wywodzi się z dawnej obsługi i ochrony sieci. Wie więcej niż inni i chce mieć monopol na światło.
- **Serce Mgły** to główny ośrodek programu, zarośnięty kompleks badawczy, z którego Mgła wciąż się rozchodzi.
- **Epoka Światła** w drzewku technologii to odtwarzanie wiedzy programu, a nie wynalazki od zera.

**Jak gracz poznaje prawdę:** nie ma wprowadzenia ani narratora. Prawda leży w Archiwum Zasłony, czyli fragmentach zbieranych na wyprawach: dokumenty, taśmy, dzienniki, plany latarni, zapiski operatorów. Większość jest w podziemiach, bunkrach i strefach przemysłowych. Sekcje mutantów i Bestiariusz dokładają wiedzę biologiczną. Fragmenty bywają sprzeczne, bo pisali je ludzie, którzy sami nie wiedzieli wszystkiego.

**Sandbox bez wątku głównego:** gra nie prowadzi gracza przez akty. Kształt 80 godzin (około 6–8 lat w grze) daje rosnąca presja świata:

| Okres | Co narasta |
| --- | --- |
| Lata 1–2 | przetrwanie, pierwsze wyprawy, słabe Przypływy, epoki Złomu i Kuźni |
| Lata 3–4 | rozrost terytorium, frakcje zaczynają się interesować osadą, większe Przypływy |
| Lata 5–6 | Mgła uczy się Wzorów, wojny frakcji, Wędrujący Las, Wielki Pasterz |
| Lata 7–8 | największe Przypływy, wyprawy ku Sercu, decyzja o zakończeniu |

Presja to zegary świata: Przypływy rosną, Mgła powoli się rozszerza, a frakcje rozwijają się niezależnie od gracza.

**Zakończenia** odblokowują warunki, nie fabuła: wiedza z Archiwum, technologie, kontrola terytorium i ustrój.

| Zakończenie | Na czym polega | Typowa droga |
| --- | --- | --- |
| Zgasić Mgłę | uruchomić w Sercu przedwojenny protokół wygaszenia | Czystość, Oczyszczacze, pełne Archiwum |
| Przemiana | przejąć rolę operatora i połączyć osadę z Mgłą | Przemiana, Kontrolowane mutacje, Kościół Mgły lub Wolne Miasto |
| Sieć latarni | odbudować pierścień i zamknąć Mgłę w strefie | Sieć latarni, terytorium, sojusze lub podboje |

Porażki: zgaszona Latarnia, upadek osady, utrata wszystkich ludzi. Rewolucja i zmiana władzy nie kończą gry, tylko zmieniają osadę.

**Ludzki dramat:** groza Mgły jest tłem, a w centrum są ludzie i wybory.

- **Wydarzenia osobiste** z puli z warunkami (pora roku, ustrój, poglądy, epoka, relacje): żałoba, zdrada, przyjaźń na wyprawie, dziecko, któremu pojawia się mutacja, ktoś ukrywający Odmienność.
- **Dylematy** bez dobrej odpowiedzi: przyjąć chorego przybysza, zostawić rannego, wydać zmutowanego Synom Czystości w zamian za pokój, ukarać bohatera za złamanie prawa.
- **Postacie z historiami:** kilkanaście ręcznie napisanych postaci z rozgałęzionymi wątkami, które pojawiają się losowo w różnych kampaniach. Przykłady: stary Latarnik, który pamięta Program; lekarka szukająca lekarstwa na Odmienność; kaznodzieja Chóru; dezerter z Zakonu.
- **Kronika osady** zapisuje to, co się wydarzyło, i sama staje się opowieścią kampanii.

**Treści drażliwe** (decyzja z 8 listopada): praca dzieci, egzekucje, przetwarzanie zmarłych i kategorie przydatności są **pokazywane wprost**, także jako sceny w grze, a na starcie jest ostrzeżenie o treściach. Gra nie moralizuje: skutki wynikają z ludzi.

Ankieta z 8 listopada: przemoc wobec dzieci też jest pokazywana wprost. Uwaga do wydania: to podnosi ocenę wiekową, a w części krajów (np. Niemcy, Australia) grozi odmową klasyfikacji. Wersja na te rynki może wymagać zamiany takich scen na tekst.

## Pula wydarzeń

Wydarzenia to główne źródło ludzkiego dramatu w sandboxie: wybierają bohaterów spośród prawdziwych mieszkańców osady, stawiają dylematy bez dobrej odpowiedzi i zostawiają ślad w poglądach, relacjach i Kronice. Liczby są robocze.

**Budowa wydarzenia:**

- **Warunki:** pora roku, ustrój i kurs osi, epoka, prawa, stan zasobów, cechy, poglądy, relacje i Odmienność konkretnych ludzi.
- **Bohaterowie:** dobierani z mieszkańców, którzy pasują do warunków (np. matka z dzieckiem, weteran z Fanatykiem w składzie). Dzięki temu każda historia dotyczy kogoś, kogo gracz zna.
- **Wybory:** 2–4 opcje. Każda coś daje i coś kosztuje, żadna nie jest oczywista.
- **Skutki:** poglądy na osiach, morale, Legitymacja, zasoby, relacje, Odmienność, ranny lub martwy człowiek, flagi dla kolejnych wydarzeń.
- **Łańcuchy:** wydarzenie może mieć ciąg dalszy po kilku dniach lub tygodniach, zależny od wcześniejszego wyboru.
- **Pamięć:** ludzie pamiętają, jak władza ich potraktowała, co wpływa na relację z dowódcami i Legitymację. Kronika zapisuje każde większe wydarzenie.
- **Losowanie:** waga, czas odnowienia, wydarzenia jednorazowe i powtarzalne.

**Częstotliwość:** około jedno drobne wydarzenie dziennie, jedno duże na tydzień i wydarzenia sezonowe (Przypływ, żniwa, zima). Docelowo 250+ wydarzeń w pełnej grze, około 30 w pierwszej wersji.

**Kategorie:**

| Kategoria | O czym |
| --- | --- |
| Osobiste | żałoba, przyjaźń, miłość, zdrada, narodziny, choroba, ukryta Odmienność |
| Społeczne | kłótnie, kradzieże, strajki, frakcje, przybysze u bramy |
| Polityczne | żądania frakcji, próby puczu, skutki praw, proklamacja ustroju |
| Mgła | Przypływy, Echo, szept Mgły, zarośnięte budynki, Pielgrzymi |
| Drogi i wyprawy | zdarzenia na trasie, znaleziska, ranni, wołanie o pomoc |
| Dyplomatyczne | posłowie, ultimata, uciekinierzy z innych osad, karawany |
| Archiwum Zasłony | fragmenty prawdy o Programie, które zmieniają poglądy |

**Przykładowe wydarzenia:**

| Wydarzenie | Warunek | Wybory i skutki |
| --- | --- | --- |
| Dziecko z świecącymi oczami | dziecko w osadzie, Odmienność rodzica 25+ | ukryć (ryzyko wykrycia, Czyści wściekli) · zgłosić do rejestru (rodzice w żalu, Legitymacja u Czystych +) · wygnać rodzinę (Czystość +, Kronika pamięta) |
| Ranny zostaje na drodze | odwrót z wyprawy, ranny bez noszy | wrócić po niego (ryzyko dla składu) · zostawić (morale składu −, może wrócić jako Odmieniec) · dobić (morale −−, Okrutni obojętni) |
| Głodna zima | zima, racje poniżej tygodnia | zmniejszyć racje wszystkim · odebrać racje wygnańcom i więźniom · zaproponować prawo Przetwarzanie zmarłych |
| Przybysze u bramy | duży promień światła, wiosna | wpuścić wszystkich (więcej rąk, ryzyko Naśladowcy) · tylko przez kwarantannę · odmówić (Echo częściej, poglądy się przesuwają) |
| Bohater złamał prawo | weteran ukradł leki dla rannego towarzysza | ukarać według prawa (Legitymacja +, weterani −) · ułaskawić (Legitymacja −, weterani +) · zmienić prawo |
| Kaznodzieja Chóru | kurs Mgły powyżej 0 albo dużo zmutowanych | pozwolić głosić (Przemiana +) · wygnać · wydać Synom Czystości w zamian za pokój |
| Szept Mgły | człowiek z Odmiennością 50+ | zamknąć go na noc · obserwować, dokąd idzie (ryzyko, możliwe znalezisko) · nic nie robić |
| Strajk w Kuźni | Niepokoje, zmiany 12-godzinne | ustąpić (uchylić prawo) · stłumić siłą (Władza +, morale −) · negocjować (racje za pracę) |
| Taśma z bunkra | znalezisko z Archiwum | odtworzyć wszystkim (poglądy się przesuwają, morale − lub +) · utajnić (tylko władza wie) · zniszczyć |
| Dezerterzy proszą o powrót | wcześniej wygnani ludzie | przyjąć (Legitymacja − u twardych, +ręce) · przyjąć pod warunkiem pracy karnej · odmówić (stają się wrogami) |

**Przykład łańcucha: Ukryta Odmienność**

1. Medyk zauważa chitynę pod bandażem wojownika. Wybory: zgłosić, przemilczeć, porozmawiać z nim.
2. Po kilku tygodniach (jeśli przemilczano): wojownik ratuje skład dzięki mutacji, a skład to widzi. Wybory: nagrodzić publicznie, ukarać za ukrywanie, uciszyć świadków.
3. Skutek: osada przesuwa się w stronę Przemiany albo Czystości, wojownik zostaje legendą albo dezerterem, a Kronika zapisuje historię z jego imieniem.

**Zasady pisania:** krótkie teksty (2–4 zdania) z imionami prawdziwych mieszkańców, kilka wariantów treści na wydarzenie, brak moralizowania. Wydarzenia są plikami danych, więc można je dopisywać bez zmian w kodzie.

## Ustroje i prawa

Ustrój wyłania się z praw. Każde prawo przesuwa kurs osady na **sześciu osiach**, a strona każdej osi daje nazwę ustroju: 2 do potęgi 6, czyli 64 ręcznie nazwane ustroje. Pełny opis z nazwami jest w `USTROJE.md` (decyzja z 8 listopada, zastępuje dawne 3 osie i 6 ustrojów).

| Oś | Biegun minus | Biegun plus |
| --- | --- | --- |
| Władza | Demokracja | Autorytaryzm |
| Gospodarka | Kolektywizm | Indywidualizm |
| Mgła | Czystość | Przemiana |
| Świat | Otwartość | Zamknięcie |
| Porządek | Swoboda | Dyscyplina |
| Los słabych | Opieka | Selekcja (`OS_OPIEKA_SELEKCJA.md`) |

**Zasady:**

- **Biegun działa**, gdy kurs na jego osi przekracza 40. Każdy biegun ma własne efekty i mechaniki specjalne, a efekty się sumują, więc balansuje się 12 zestawów, a nie 64 ustroje.
- **Proklamacja ustroju** jest możliwa, gdy co najmniej 4 osie mają aktywny biegun.
- **Demokracja:** Sejm głosuje nad prawami, a ruchy obywatelskie wyrastają z poglądów mieszkańców. Gracz musi budować poparcie albo dostosować się do społeczeństwa.
- **Autorytaryzm:** rada przyboczna, dekrety od razu i wskaźnik napięcia, który przy wysokiej wartości grozi puczem.
- **Religia** będzie osobnym systemem, a nie osią.

**Legitymacja** to drugi licznik obok morale. Prawa niezgodne z poglądami ludzi ją obniżają, a niska Legitymacja daje strajki, potem pucz frakcji. Zmiana ustroju jest wydarzeniem do przetrwania, nie przełącznikiem.

**Polityka oddolna:** każdy mieszkaniec ma własne położenie na sześciu osiach, które zmienia się przez wydarzenia (zmutował brat, więc bliżej Przemiany; dziecko zginęło w Mgle, więc bliżej Czystości; ktoś go wyniósł rannego z walki, więc bliżej Opieki). Z ludzi o podobnych poglądach powstają frakcje i ruchy.

Ustrój wpływa też na walkę: w Demokracji składy wybierają liderów, przy Dyscyplinie nikt się nie wycofuje bez rozkazu, a przy Przemianie zmutowani wpadają w szał.

## Drzewo praw w liczbach

Każde prawo przesuwa kurs osady na osiach, zmienia gospodarkę lub morale i można je uchylić, ale drogo; kilka praw jest nieodwracalnych. Wszystkie liczby są robocze do balansu.

**Oznaczenia osi:**

- W = Władza (minus Demokracja, plus Autorytaryzm);
- Ł = Gospodarka (minus Kolektywizm, plus Indywidualizm);
- M = Mgła (minus Czystość, plus Przemiana).

Nowe osie Świat (Ś), Porządek (P) i Los słabych (O) są w `USTROJE.md`, a przypisanie praw do nich jest do przeliczenia. Kurs osady na każdej osi mieści się w zakresie od −100 do 100.

**Mechanika:**

- **Uchwalenie** trwa 1 dzień, a między kolejnymi prawami muszą minąć 2 dni.
- **Uchylenie** trwa 3 dni, kosztuje jednorazowo −15 Legitymacji i cofa przesunięcie osi. Prawa nieodwracalne są oznaczone †.
- **Zadowolenie człowieka** z praw = 100 minus połowa sumarycznej odległości między jego poglądami a kursem osady na sześciu osiach (minimum 0).
- **Legitymacja** = średnie zadowolenie mieszkańców, przy czym dowódcy i weterani liczą się podwójnie.
- **Przyzwyczajenie:** przy morale powyżej 40 poglądy każdego przesuwają się o 1 punkt dziennie w stronę kursu osady; Fanatyk przesuwa się w przeciwną stronę, jeśli się nie zgadza.
- **Ustrój** można proklamować, gdy co najmniej 4 osie mają aktywny biegun (kurs powyżej 40). Proklamacja kosztuje −10 Legitymacji. Gdy któraś z tych osi wróci do środka, zaczyna się kryzys ustrojowy (szczegóły w `USTROJE.md`).

**Progi Legitymacji:**

| Legitymacja | Stan | Skutek |
| --- | --- | --- |
| 70–100 | Stabilna | produkcja +10%, prawa uchwalane w pół dnia |
| 40–69 | Normalna | bez zmian |
| 25–39 | Niepokoje | strajk w jednym losowym budynku na dzień, frakcje rosną szybciej |
| 10–24 | Bunt | frakcja próbuje puczu w ciągu kilku dni |
| 0–9 | Upadek | rewolucja albo zamach, zmiana władzy |

**Praca:**

| Prawo | Osie | Efekt |
| --- | --- | --- |
| Zmiany 12-godzinne | W +5 | produkcja +20%, morale −10, więcej chorób i wypadków |
| Przymus pracy | W +10, Ł −5 | bezczynni i więźniowie pracują, morale −10 |
| Dzieci w szkołach | W −5 | umiejętności dzieci rosną o 50% szybciej, mniej rąk teraz |
| Dzieci w warsztatach | W +5, Ł −5 | dzieci od 10 lat pracują z 50% wydajności, jako dorośli mają −2 do umiejętności, morale rodzin −10 |
| Spółdzielnie | Ł −10 | produkcja równa, morale +5 |
| Własność warsztatów | Ł +15 | produkcja +15% w warsztatach z właścicielem, nierówności rośną |

**Prawo i kara:**

| Prawo | Osie | Efekt |
| --- | --- | --- |
| Sądy | W −10 | przestępczość −20%, kary łagodne, wymaga Sądu |
| Praca karna | W +5 | skazani pracują bez płacy, morale skazańych −20 |
| Wygnanie w Mgłę | W +10 | przestępczość −30%, wygnani mogą wrócić jako Dezerterzy lub Odmieńcy |
| Egzekucje publiczne † | W +20 | przestępczość −50%, morale −10 na stałe, strach przed władzą |

**Żywność:**

| Prawo | Osie | Efekt |
| --- | --- | --- |
| Równe racje | Ł −10 | morale +5, brak premii dla wojowników |
| Racje za pracę | Ł +10 | produkcja +10%, słabsi i chorzy głodują |
| Podwójne racje dla wojowników | W +5 | morale wojowników +10, cywilów −5 |
| Prawo do łupu | Ł +15 | wojownicy zatrzymują 20% łupu, morale wojowników +10, wspólny magazyn mniejszy |
| Przetwarzanie zmarłych † | Ł −5 | każdy zmarły daje racje i tłuszcz na paliwo, morale −20 przez 10 dni, Zabobonni bardzo przeciw |

**Mutanci:**

| Prawo | Osie | Efekt |
| --- | --- | --- |
| Rejestr zmutowanych | M −10 | Odmienność każdego widoczna dla gracza, Czyści zadowoleni |
| Getto | M −20 | zmutowani mieszkają osobno, ich morale −20, szybciej rosną Dezerterzy |
| Równouprawnienie | M +15 | morale zmutowanych +15, Czyści przeciw |
| Przymusowe wyprawy dla zmutowanych | W +10, M −10 | wyprawy tańsze w ochronie, zmutowani giną częściej |
| Badania na zmutowanych | M −5 | badania Mgły +30%, morale zmutowanych −15 |

**Wiara i propaganda:**

| Prawo | Osie | Efekt |
| --- | --- | --- |
| Sztandar osady | W +5 | odblokowuje sztandary, morale +5 w składach ze sztandarem |
| Święto Światła | W −5 | raz na porę roku morale +15 na 3 dni, kosztuje paliwo |
| Wolność słowa | W −10 | Legitymacja +5, frakcje rosną szybciej |
| Cenzura | W +10 | frakcje rosną o połowę wolniej, obce ideologie słabiej przenikają |
| Kult Latarni | W +10, M −10 | odporność na Echo i Krzykacza +, Kościół Mgły niemożliwy bez kryzysu |
| Tolerancja Mgły | M +10 | mniej strachu przed Mgłą, łatwiejsze rekrutowanie zmutowanych |

**Unikalne prawa ustrojów** (stary podział, do przypisania biegunom według tabeli w `USTROJE.md`):

| Ustrój | Prawo | Efekt |
| --- | --- | --- |
| Komuna | Rotacja ról | morale +10, umiejętności rosną o 25% wolniej |
| Komuna | Wspólny magazyn łupu | cały łup wspólny, Normy produkcji tańsze |
| Komuna | Rady robotnicze | Legitymacja +10, uchwalanie praw trwa dwa razy dłużej |
| Rada | Wybory dowódców | morale składów +10, poziom wybranego dowódcy losowo ±1 |
| Rada | Wolny handel | ceny w handlu −15% |
| Rada | Trybunał ławników | przestępczość −20%, Legitymacja +5 |
| Junta | Pobór | każdy dorosły jest w rezerwie bojowej, produkcja −10% |
| Junta | Wygnanie zmutowanych | Odmienność 25+ oznacza wygnanie, Czystość w osadzie, Dezerterzy rosną |
| Junta | Kult sztandaru | sztandar daje podwójne morale, wymagany w każdym składzie |
| Reżim Latarnika | Racjonowanie światła | zużycie paliwa −20%, dzielnice dalekie od latarni morale −15 |
| Reżim Latarnika | Donosiciele | bunty wykrywane kilka dni wcześniej, zaufanie i relacje słabną |
| Reżim Latarnika | Kasty | wydajność +10%, nikt nie zmienia roli bez zgody władzy |
| Kościół Mgły | Rytualne mutacje † | raz na porę roku wybrana mutacja dla wybranej osoby |
| Kościół Mgły | Prorocy | wizje zapowiadają Przypływy i większe ataki |
| Kościół Mgły | Ofiary dla Mgły † | Przypływ słabszy, morale −10, Echo pojawia się częściej |
| Wolne Miasto | Rynek mutacji | kupno i sprzedaż mutacji przez handlarzy |
| Wolne Miasto | Kontrakty najemnicze | najemnicy Ludzi Toru 30% tańsi |
| Wolne Miasto | Obywatelstwo za opłatą | przybysze płacą za wejście, więcej zasobów, większe nierówności |

Prawa uchwala się w Ratuszu.

## Sztandary i wpływ

Sztandar to przedmiot, nie rola: każdy może go nieść, kosztem wolnej ręki dodatkowej.

- **W osadzie:** plakaty (od Złomu, technologia Sztandar), pomnik (od Pary) i radiowęzeł (od Silnika) wzmacniają obecny ustrój i tłumią frakcje opozycyjne.
- **W składzie:** niesiony sztandar podnosi morale i odporność na strach przed mutantami. Utrata sztandaru albo śmierć niosącego mocno uderza w morale.
- **Poza osadą:** karawany i składy roznoszą sztandar do innych osad. Wpływ narasta, aż osada przechodzi pod twój ustrój (sojusznik lub wasal) albo wybucha w niej rewolucja, której można pomóc.
- **W drugą stronę:** obce ideologie przenikają do twojej osady przez handel i kontakty, np. Chór Mgły przeciąga Przemienionych.

## Epoki technologiczne i drzewko technologii

Gra przechodzi przez epoki technologiczne: na początku dzidy i łuki, potem coraz lepszy sprzęt. Podział na siedem epok to propozycja, którą łatwo zmienić.

| Epoka | Broń | Pancerz i ubiór | Organizacja |
| --- | --- | --- | --- |
| 1. Złom | dzidy, łuki, procy, noże, pałki, tarcze z blachy; broń palna tylko jako rzadkie znalezisko bez amunicji | łachy, hełm z garnka, kamizelka z blachy | ręczne ubieranie, Ewidencja zbrojowni |
| 2. Kuźnia | kusze, kute miecze, topory i młoty, okute tarcze | kolczuga z drutu, woskowany płaszcz | Akademia I (skrzydło dowódców), szkoła dla dzieci, sąd |
| 3. Czarny proch | rusznice i obrzyny jednostrzałowe, bomby ręczne | płyty, maska gazowa | Akademia II, kontramarsz, Sygnały ręczne |
| 4. Para | lufy gwintowane, pierwsze części seryjne | ubranie robocze z warsztatów, protezy | Standaryzacja i Wzory |
| 5. Nabój | karabiny i rewolwery powtarzalne, granaty odłamkowe, bagnety, lunety | mundury, hełmy wojskowe, hełm z maską, kombinezony przeciwchemiczne | Normy produkcji, Akademia III |
| 6. Silnik | broń samopowtarzalna, lekka broń maszynowa, tłumiki, latarki podlufowe | lepsze protezy | Ujednolicony kaliber, radio, Kwatermistrzostwo, Akademia IV |
| 7. Światło | broń zapalająca, światło jako broń, broń z technologii Latarni i Mgły | przetworzona chityna, kontrolowane mutacje (zależnie od ustroju) | Akademia V, kolumny pod oficerem |

**Zasady epok:**

- Nową epokę otwiera zestaw technologii plus kluczowy budynek (Kuźnia, Wytwórnia prochu, Warsztat maszynowy, Rusznikarnia).
- Na wyprawach można znaleźć sprzęt z wyższej epoki, ale bez amunicji i części, więc staje się problemem logistycznym.
- Inne frakcje bywają w innych epokach: Szabrownicy w Złomie, Zakon Latarników w epoce Silnika.
- Epoka zmienia taktykę: we wczesnych epokach dominują formacje do walki wręcz (mur tarcz, jeż), w późniejszych formacje strzeleckie i ruch skokami.

**Gałęzie drzewka:**

- **Światło:** zasięg latarni, nowe paliwa, przenośne lampy, światło jako broń.
- **Przetrwanie:** uprawy w ciemności, filtry wody, medycyna, kwarantanna, Hełm z maską, Protezy.
- **Uzbrojenie:** ścieżka przez epoki, dodatki do broni, Ewidencja zbrojowni, Standaryzacja, Normy produkcji, Ujednolicony kaliber, Kwatermistrzostwo.
- **Mgła:** badanie mutacji, kontrolowane mutacje (niedostępne dla Junty), oczyszczacze (niedostępne dla Kościoła Mgły).
- **Społeczeństwo:** szkoła, szpital, Akademia (trzy kierunki: Zasoby ludzkie, Orientacja w terenie, Taktyka i formacje), Sygnały ręczne, Plac ćwiczeń.

**Pełna rozpiska wszystkich 7 epok** (technologie w fazach, wybory, budynki, sprzęt, surowce, społeczeństwo i wygląd) jest w zakładce Epoki. Tabela technologii niżej to skrót.

**Tempo epok i przełom:**

- **Epoka to długi etap gry**, około roku gry (60 dni) jako wskazówka do balansu. Gra nie ma limitów czasowych: przejście zależy tylko od spełnienia warunków. Cała kampania przez 7 epok to około 80 godzin.
- **Trzy fazy w każdej epoce:**
  - wczesna, środkowa i późna, po 15–22 technologie na epokę;
  - kolejna faza otwiera się po 5 technologiach poprzedniej i jednym budynku fazy;
  - faza zmienia wygląd i działanie budynków, np. Szałas przechodzi w Barak.
- **Przełom, czyli wejście w nową epokę, wymaga czterech rzeczy:**
  1. kluczowych technologii późnej fazy;
  2. kluczowego odkrycia z wyprawy;
  3. budynku przełomu i udanej próby, która może się nie powieść;
  4. potem przychodzi odpowiedź Mgły.

| Przejście | Kluczowe odkrycie | Budynek | Próba (może się nie udać) | Odpowiedź Mgły |
| --- | --- | --- | --- | --- |
| Złom → Kuźnia | Podręcznik kowala albo palenisko w ruinach cegielni | Dymarka | Pierwszy wytop: 3 dni i 30 węgla drzewnego. Porażka to pęknięta dymarka, oparzenia i ponowna próba po 5 dniach. | ogień i dym ściągają Bezokich i Pajęczarzy, Przypływy są o stopień silniejsze |
| Kuźnia → Czarny proch | Receptura prochu z apteki albo kopalni saletry | Wytwórnia prochu | Pierwszy ładunek pod murem. Porażka to wybuch wytwórni i ofiary. | huk uczy Mgłę: więcej Krzykaczy i Śpiących, Opiekunka zaczyna polować |
| Czarny proch → Para | Kocioł z lokomotywowni przy torach | Kotłownia | Pierwsze ciśnienie: kocioł pracuje dobę pod nadzorem. Porażka to rozerwany kocioł. | ciepło i metal przyciągają Rozrost i Kopaczy, w regionie wyrasta pierwszy Siewca |
| Para → Nabój | Matryce z fabryki amunicji | Rusznikarnia | Pierwsza seria 100 nabojów na strzelnicy. Porażka to partia zacięć i ranny strzelec. | Pasterz zaczyna dowodzić rojami, pojawiają się Wydrążeni w twoim sprzęcie |
| Nabój → Silnik | Działający silnik z bazy wojskowej (wiedza zaginiona) | Warsztat silnikowy z prądnicą | Pierwsze uruchomienie: noc z prądem w całej osadzie, hałas i światło ściągają atak. | Zrośnięci i Dawni Żołnierze, w gęstych strefach Mgła atakuje także w dzień |
| Silnik → Światło | Soczewka latarni morskiej i próbki z bossów | Laboratorium Światła | Rozpalenie nowego światła: całonocne oblężenie. | budzi się Serce Mgły, ruszają Wielki Pasterz i Chór Mgły, zaczyna się finał |

**Zagrożenia epok** (epoka, w której wróg pojawia się pierwszy raz; frakcje mogą być wcześniej znane z plotek i handlu):

| Epoka | Mutanty i stworzenia | Frakcje |
| --- | --- | --- |
| Złom | Szczurza Fala, Ogar, Krzykacz, Nosiciel, Padlinniki | Szabrownicy, Dzieci Lasu |
| Kuźnia | Bezoki, Pajęczarz, Wataha, Ryjec, Ćma Latarniowa | Łowcy Głów, Synowie Czystości |
| Czarny proch | Śpiący, Opiekunka, Pijawiec, Rogacz, Biegacz | Dezerterzy, Gildia Solna |
| Para | Rozrost, Kopacz, Siewca, Kłębowisko, Kokon | Ludzie Toru, Chór Mgły, Bractwo Bunkra |
| Nabój | Pasterz, Naśladowca, Odmieniec, Wydrążony, Zwiewnik | Zakon Latarników (pierwszy kontakt) |
| Silnik | Zrośnięci, Dawny Żołnierz, Nici, Oddech, Gasiciel, Fałszywy Latarnik | Republika Tamy |
| Światło | Matka Roju, Echa, Wielki Pasterz | Chór Mgły w pełnej sile |

## Drzewko technologii w szczegółach

Technologie leżą w pięciu gałęziach, a nową epokę otwiera zestaw kluczowych technologii z kilku gałęzi naraz. Płaci się punktami nauki i surowcami, a część wiedzy przychodzi tylko ze znalezisk z wypraw. Nazwy, koszty i liczby są robocze.

**Jak się bada:**

- **Punkty nauki** dają badacze (zależnie od umiejętności Nauka). Na starcie jeden badacz pracuje przy stole w latarni. Pracownia (budynek Złomu) mieści do 3 badaczy i daje +25% punktów. Prosektorium daje punkty tylko dla gałęzi Mgła, z sekcji mutantów, i wymaga Pracowni.
- **Surowce:** wiele technologii wymaga też materiałów, np. metalu, części albo chityny, na prototyp.
- **Plany i książki** z wypraw dają 50–100% postępu technologii, czasem z wyższej epoki. Wtedy wiedza czeka, aż osada spełni zależności.
- **Inżynieria wsteczna:** rozebranie znalezionego sprzętu w odpowiednim budynku daje 25–60% postępu, ale niszczy przedmiot. Gracz wybiera: użyć jedynego karabinu powtarzalnego czy go rozebrać.
- **Ludzie z wiedzą:** przybysz z cechą Inżynier, Lekarz albo Chemik przyspiesza swoją gałąź o 25%.
- **Wiedza zaginiona:** kilka technologii zdobywa się wyłącznie ze znalezisk, np. Silnik, Soczewka latarni morskiej, Przedwojenny lek.

**Koszt według epoki:**

| Epoka | Punkty nauki za technologię | Typowe dodatkowe surowce |
| --- | --- | --- |
| 1. Złom | 40–140 (wczesna 40–60, środkowa 60–90, późna 100–140) | drewno, złom |
| 2. Kuźnia | 150–250 | metal, skóra |
| 3. Czarny proch | 300–500 | metal, proch |
| 4. Para | 500–800 | metal, części |
| 5. Nabój | 800–1200 | części, proch, mosiądz z wypraw |
| 6. Silnik | 1200–1800 | części, paliwo, znaleziska |
| 7. Światło | 2000–3000 | chityna, tkanka, gruczoły, próbki z bossów |

**Technologie, zależności i wejście w nową epokę** są opisane wyłącznie w `EPOKI.md`. Nową epokę otwiera tylko przełom: kluczowe technologie, odkrycie z wyprawy, budynek z próbą, a potem odpowiedź Mgły (tabela „Tempo epok i przełom” wyżej). Dawna tabela progów epok i skrót technologii zostały usunięte 8 listopada, bo były sprzeczne z przełomem.

**Blokady ustrojowe:** w epokach Pary i Silnika Mgła ma rozwidlenie: Oczyszczacze (droga Czystości, niedostępne przy aktywnym biegunie Przemiany) albo Inkubator tkanki i Kontrolowane mutacje (droga Przemiany, niedostępne przy aktywnym biegunie Czystości). Zmiana ustroju w trakcie gry nie kasuje zbadanych technologii, ale ich użycie przy obcym ustroju kosztuje Legitymację.

**Fazy epok:** pełna lista technologii każdej epoki w trzech fazach, z kosztami, wyborami i kluczowymi technologiami przełomu, jest w zakładce Epoki. Do przejścia przez epokę wystarcza około 60% jej drzewka, a resztę zostawia się na inne podejścia w kolejnych kampaniach. Koszty od epoki Czarnego prochu obcięto 8 listopada mniej więcej o połowę, żeby każda epoka mieściła się w około 60 dniach.

## Ludzie, wojownicy i ekwipunek

Każdy wojownik to osobna postać ze statystykami, cechami, mutacjami, poglądami i historią walk. Gracz rekrutuje pojedynczych ludzi, wyposaża ich i składa z nich składy.

**Statystyki bojowe:**

| Statystyka | Na co wpływa |
| --- | --- |
| Zdrowie | ile ran wytrzyma |
| Celność | trafienia bronią dystansową |
| Walka wręcz | trafienia i bloki w zwarciu |
| Siła | obrażenia wręcz, udźwig, ile pancerza zniesie |
| Zwinność | ruch, uniki, przeładowanie |
| Percepcja | wykrywanie wrogów, sieci, zasadzek |
| Opanowanie | odporność na strach, bazowe morale |
| Wytrzymałość | zmęczenie w długiej walce wręcz |

**Sloty:**

| Slot | Przykłady | Zasada |
| --- | --- | --- |
| Głowa | hełm z garnka, hełm wojskowy, maska gazowa, hełm z maską | hełm albo maska, oba naraz dopiero po technologii |
| Tors (pancerz) | kamizelka z blachy, kolczuga z drutu, płyty, chityna | główna ochrona fizyczna, dużo waży |
| Ubiór | łachy, ubranie robocze, płaszcz, mundur, kombinezon | mają go wszyscy, także cywile |
| Ręka główna | dzida, włócznia, miecz, młot, łuk, karabin | broń dwuręczna zajmuje obie ręce |
| Ręka dodatkowa | tarcza, pistolet, nóż, lampa, sztandar | wolna tylko przy broni jednoręcznej |
| Broń boczna | nóż, toporek, pistolet | wyciągana automatycznie |
| Ekwipunek | amunicja, apteczki, granaty, flary, łup, pale | limit wagi i miejsc |

**Zmiana broni (AI):** wróg bliżej niż około 2 kratki od strzelca oznacza broń boczną. Brak amunicji oznacza broń boczną albo cofnięcie się za tarczownika. Tarczownik używa obu rąk naraz. Karabin z bagnetem walczy wręcz bez zmiany broni.

**Dodatki do broni (gniazda zależą od modelu):**

- **Karabin:** luneta (zasięg i celność, gorzej z bliska), bagnet, latarka podlufowa (widzenie w Mgle, ale przyciąga mutanty), tłumik, większy magazynek.
- **Tarcza:** wizjer, kolce, okucie, uchwyt na lampę.
- **Włócznia i dzida:** hak, grot zapalający, okute drzewce.

**Ubiór:**

| Ubiór | Kto nosi | Co daje |
| --- | --- | --- |
| Łachy | cywile na start | nic, słabo chroni przed zimnem |
| Ubranie robocze | rzemieślnicy, rolnicy | szybsza praca, mniej wypadków |
| Woskowany płaszcz | zwiadowcy, tragarze | deszcz, zimno, lekka ochrona przed Mgłą |
| Mundur | wojownicy | morale, dyscyplina, wytrzymałość |
| Kombinezon przeciwchemiczny | wyprawy w Mgłę | silna ochrona przed Mgłą, może się rozedrzeć |
| Kombinezon z tkanki mutantów | tylko Kościół Mgły | szybsze mutacje, Mgła nie widzi noszącego |

Rozdarty ubiór przestaje chronić i Mgła zaczyna działać. Lepsze ubranie cywilów to mniej chorób zimą i wyższe morale.

**Mutacje i Odmienność:** przebywanie w Mgle podnosi Odmienność i daje mutacje, np. trzecie ramię (szybsza budowa), oczy Mgły (widzenie w Mgle, demaskowanie Naśladowców), skóra z chityny (pancerz). Po przekroczeniu progu osoba słyszy Mgłę, sabotuje osadę, a w końcu odchodzi w Mgłę i wraca jako wróg. Progi i pełny katalog mutacji są w następnej sekcji.

**Weterani** lepiej wybierają osłony, rzadziej panikują i sami reagują na zagrożenie, mogą więc naprawić błąd słabego dowódcy.

**Trwałe rany:** blizny, kulenie, utrata ręki. Wojownik bez ręki używa tylko broni jednoręcznej i wypada ze swojego Wzoru. Technologia Protezy to naprawia, a niektóre mutacje odrastają w miejscu utraconej kończyny.

## Klasy, szkoły i rozwój wojowników

Wojownik rozwija się tak samo jak dowódca: ma poziom wyszkolenia taktycznego, umiejętności, kwalifikację (klasę) i zdolności bojowe. Klasy układają się w drzewka jak w Mount & Blade: szkoła uczy klas podstawowych, z których awansuje się na coraz wyższe specjalizacje. Każdy może wziąć do ręki dowolną broń, ale pełną skuteczność daje dopiero kwalifikacja na sprzęt danej klasy. Strzelec wyborowy z karabinem szturmowym strzela jak zwykły strzelec, a z karabinem wyborowym trafia w słabe punkty z dwa razy większej odległości.

### Kompleks szkoleniowy

- **Budynek w osadzie:** kompleks to normalne budynki na mapie osady. W środku stoi Plac ćwiczeń (rekrutacja, stopień I, wyszkolenie taktyczne 2–3, symulowane walki), a wokół niego skrzydła, czyli osobne budynki szkół. Klik w budynek otwiera jego panel: kursy, kursanci, instruktor, rozbudowa.
- **Gdzie można stawiać:** skrzydło działa najwyżej 2 kratki od Placu albo od innego skrzydła. Każde sąsiednie skrzydło daje +5% do tempa kursów. Atak na kompleks uderza w całe szkolenie naraz.
- **Wszystkie szkoły od początku:** każdą szkołę można zbudować już w epoce Złomu, bo każda ma odpowiednik sprzętu w każdej epoce (tabela niżej). Nowa epoka zmienia sprzęt i model budynku, nie samo istnienie szkoły. Skład drużyny da się więc rozwijać od pierwszych dni.
- **Poziomy skrzydła (I–III):** poziom wyznacza najwyższy stopień, jaki można zdobyć w szkole. Do stopnia IV (mistrza) potrzebny jest instruktor, który sam ma stopień IV.
- **Kursanci pasujący do kursu:** po wybraniu kursu lista pokazuje tylko ludzi, którzy spełniają jego warunki. Na kurs wyższego stopnia wchodzą tylko ludzie z niższym stopniem tej samej ścieżki. Pozostali są wyszarzeni z powodem, np. „brak 2 walk” albo „inna szkoła”.
- **Kurs kosztuje:** zabiera wojownika ze służby (od 3 dni na stopień II do 10 dni na stopień IV) i zużywa amunicję albo materiały na ćwiczenia.

**Przeszkolenie (zmiana klasy) jest drogie:**

- Wojownik traci jeden stopień, a umiejętności starej ścieżki spadają o 30%.
- Kurs trwa dwa razy dłużej i kosztuje dwa razy więcej.
- Morale spada o 10, bo człowiek czuje, że zmarnował lata.
- Zmiana w obrębie tej samej szkoły (np. ze Strzelca wyborowego na Kaemistę) jest dwa razy tańsza.

**Druga kwalifikacja:** od stopnia III wojownik może zdobyć jedną dodatkową kwalifikację, ale tylko na stopień II (np. Strzelec wyborowy i Sanitariusz polowy). Kurs dodatkowy trwa dwa razy dłużej.

### Szkoły i drzewka klas

Każda szkoła uczy 1–2 klas podstawowych (stopień II). Z nich wychodzą specjalizacje (stopień III), a z tych mistrzowie (stopień IV). Każda klasa ma w akcie opis: do czego służy, kiedy ją brać i jaki sprzęt jest jej własny.

**Stopień I to Rekrut:** człowiek po podstawowym przeszkoleniu na Placu ćwiczeń, jeszcze bez klasy. Każdy sprzęt traktuje jak pokrewny (80%), ale nie ma zdolności. Cywil bez stopnia walczy tak, jakby cały sprzęt był obcy.

| Szkoła | Do czego jest | Klasy podstawowe (II) | Specjalizacje (III) | Mistrzowie (IV) |
| --- | --- | --- | --- | --- |
| Walki wręcz | szyk, obrona, łamanie pancerza | Tarczownik, Rębacz | Mur (Tarczownik ciężki), Dzidnik, Łamacz, Nożownik | Mistrz tarczy, Pierwszy w wyłomie, Kat (Łamacz) |
| Strzelecka | ogień na średnim i dalekim dystansie | Strzelec | Strzelec wyborowy, Strzelec liniowy | Snajper, Weteran ognia |
| Ciężka broń | ogień zaporowy, przyduszanie, broń wałowa | Obsługa broni ciężkiej | Kaemista, Moździerzysta | Mistrz zaporowy |
| Szturmowa | walka na bliskim dystansie, wejście do budynków | Szturmowiec | Grenadier, Oczyszczacz | Weteran szturmowy |
| Zwiadu | wykrywanie, ciche zabijanie, prowadzenie wypraw | Zwiadowca | Tropiciel, Cichy łowca, Przewodnik | Oko Mgły |
| Saperska | umocnienia przed walką, pułapki, ładunki, rozbrajanie | Saper | Fortyfikator, Minerka, Ogniomistrz | Mistrz oblężeń |
| Sanitarna | utrzymanie ludzi przy życiu w walce i po niej | Sanitariusz | Felczer, Sanitariusz bojowy | Lekarz polowy |
| Łączności | zasięg dowodzenia, meldunki, wezwanie wsparcia | Goniec | Sygnalista, Radiooperator | Oficer łączności |
| Komisarzy | aktywne pilnowanie morale i dyscypliny | Opiekun składu, potem wersja ustroju (Politruk, Żandarm, Kaznodzieja…) | Komisarz liniowy, Komisarz polowy | Komisarz frontowy |
| Latarników | światło w walce, ochrona przed Mgłą i jej stworzeniami | Niosący światło | Latarnik bojowy, Oczyszczyciel | Strażnik Płomienia |
| Tresury | psy bojowe i zwierzęta juczne | Psiarz | Przewodnik sfory, Poganiacz juków | Pan sfory |

**Sprzęt szkół w epokach:** każda szkoła ma swój sprzęt już w epoce Złomu, a każda kolejna epoka go ulepsza.

| Szkoła | Złom | Kuźnia | Czarny proch | Para | Nabój | Silnik | Światło |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Walki wręcz | rura, tarcza z blachy | miecz, dzida, tarcza okuta | halabarda, kirys | topór parowy (lekki) | bagnet, tarcza balistyczna | tarcza z wizjerem | ostrze żarowe |
| Strzelecka | proca, łuk z rurek | łuk refleksyjny, kusza | muszkiet | karabin pneumatyczny | karabin powtarzalny, wyborowy | karabin samopowtarzalny | karabin z celownikiem świetlnym |
| Ciężka broń | katapulta z resorów | kusza wałowa | hakownica, moździerz | działko parowe | ciężki karabin | karabin maszynowy | działo świetlne |
| Szturmowa | maczeta, koktajle zapalające | toporek, granaty z beczek | pistolet skałkowy, granaty prochowe | strzelba parowa | strzelba powtarzalna, rewolwer | pistolet maszynowy | pistolet świetlny |
| Zwiadu | nóż, lekki łuk | kusza lekka, sieć | pistolet skałkowy, lunetka | gogle, linka | karabinek z tłumikiem | lornetka nocna | oczy z soczewek |
| Saperska | łopata, pale, doły | pułapki na zwierzę, kolczatki | ładunki prochowe | wiertła, kotwy | miny, drut kolczasty | ładunki kierunkowe, miotacz ognia | miny świetlne |
| Sanitarna | bandaże, zioła | piła chirurgiczna, szyny | opium, nosze | sterylizator | morfina, opaska uciskowa | antybiotyki, plazma | aparat oczyszczania |
| Łączności | gwizdki, róg | bęben, chorągiewki | flary | tuba parowa | telegraf polowy | radio | radio z przekaźnikiem |
| Komisarzy | pałka, głos | bat, dzwon | pistolet | megafon parowy | rewolwer, odznaka | radio do propagandy | megafon świetlny |
| Latarników | pochodnia, lampa naftowa | lustra-reflektory | flary magnezowe | lampa karbidowa | reflektor ręczny | reflektor bojowy | lanca latarniowa |
| Tresury | psy z osady | psy w skórzanych napierśnikach | psy meldunkowe | wozy z psami | psy w maskach | psy saperskie | psy z lampami |

**Chorągiew to przedmiot, nie klasa:** szkoła chorążych znika. Sztandar, proporzec czy dzwon wkłada się do slotu ręki dodatkowej. Daje bierny bonus do morale w promieniu, a jego utrata uderza w cały skład.

**Komisarz (aktywne morale, wersja zależy od ustroju):**

- W każdym ustroju robi to samo: gasi panikę okrzykiem, zawraca uciekających i stawia ich z powrotem w szyku. Różni się metoda, ostateczny środek i skutek polityczny.
- Jest dobrym celem dla wroga: śmierć komisarza uderza w morale całego składu.
- Zanim osada ma wyraźny ustrój, szkoła uczy neutralnego Opiekuna składu: tylko okrzyk i zawracanie, bez kar.
- Po zmianie ustroju komisarze przechodzą krótki kurs przestawienia (3 dni). Ci, których poglądy są daleko od nowego ustroju, mogą odmówić, odejść albo stanąć na czele opozycji.
- Dekret „Rozstrzeliwanie na miejscu” (Prawo i kara, W +15) działa w pełni w Juncie i Reżimie Latarnika. W innych ustrojach da się go uchwalić, ale każda egzekucja mocno obniża Legitymację. W Radzie zamiast niego działa Sąd polowy po walce.

| Ustrój | Klasa podstawowa | Czym działa | Ostateczny środek | Skutek |
| --- | --- | --- | --- | --- |
| Komuna | Politruk | agitacja, wspólna sprawa, przykład | degradacja, praca karna po powrocie | morale wraca szybko, gdy prawa są kolektywne |
| Rada | Rzecznik | perswazja, zaufanie składu | sąd polowy po walce, bez kar na miejscu | słabszy w ostrej panice, nie obniża Legitymacji |
| Junta | Żandarm | rozkaz, kara cielesna, groźba | rozstrzelanie na miejscu, wygnanie zmutowanego | najszybciej gasi panikę, tłumi inicjatywę |
| Reżim Latarnika | Komisarz | strach, donos, kartoteka | rozstrzelanie na miejscu | wysoka dyscyplina, stale niższe morale |
| Kościół Mgły | Kaznodzieja | wiara, ekstaza, kazanie | ofiara dla Mgły: Odmienność rośnie całemu składowi | zmutowani wpadają w szał bojowy |
| Wolne Miasto | Nadzorca | premia, kara z żołdu, kontrakt | zerwanie kontraktu i wyrzucenie ze składu | najemnicy słuchają chętnie, ideowi gorzej |

Specjalizacje (Komisarz liniowy, Komisarz polowy) i mistrz (Komisarz frontowy) noszą nazwę wersji, np. Żandarm liniowy albo Kaznodzieja frontowy.

**Do przeliczenia (8 listopada):** tabela wyżej opiera się na sześciu starych ustrojach. W nowym systemie 6 osi wersję komisarza mają wyznaczać osie Władza i Porządek.

**Umiejętności broni:** rodziny broni są częścią wspólnej listy umiejętności (sekcja „Ludzie i mutacje w szczegółach”). W rodzinie są typy: karabin zwykły, wyborowy i szturmowy to jedna rodzina, ale każda klasa ma swój typ.

**Sprzęt a klasa:**

- Sprzęt klasy: 100% skuteczności i działają zdolności bojowe.
- Sprzęt pokrewny (ta sama rodzina albo druga kwalifikacja): 80% skuteczności, a zdolności działają tylko biernie.
- Sprzęt obcy: 50% skuteczności i brak zdolności, do tego wolniejsze przeładowanie i więcej zacięć.

Akta wojownika pokazują przy każdej broni, w której z tych trzech grup się mieści.

**Wyszkolenie taktyczne (1–5):** to zachowanie pojedynczego wojownika, niezależne od klasy.

- 1: stoi i strzela.
- 2: sam szuka osłony (Plac ćwiczeń I).
- 3: ruch od osłony do osłony i przeładowanie w ukryciu (Plac ćwiczeń II).
- 4: krycie wzajemne i dyscyplina ognia (tylko z walk).
- 5: sam poprawia błędy dowódcy (tylko z walk).

### Ukryte cechy

Na początku nie wiesz, jakie cechy ma nowy człowiek: w aktach stoją puste rubryki „?”. Cechę odkrywa się na kilka sposobów:

- **Zdarzenie w walce:** po 40 sekundach przyduszenia raport pisze, że Mróz „nie drgnął”, i odkrywa się Zimna krew.
- **Kurs albo trening:** instruktor zauważa Pewną rękę.
- **Obserwacja przez lata:** Mowa, Wiara, Zabobony.
- **Komisarz albo felczer:** komisarz widzi tchórzostwo, felczer ukryte choroby.

Niektóre cechy rodzą się dopiero z doświadczenia, np. Drżące ręce albo Pustka po Echu. Dzięki temu raport „ten człowiek ma Zimną krew” to sygnał: warto go awansować i w niego inwestować.

Czasem cecha ujawnia się w najgorszym momencie, np. ukryty Lęk przed Mgłą w pierwszej wyprawie.

**Raport po walce** przypisuje sukcesy i błędy do klas, tak jak u dowódców. Na przykład: „Kaemista bez amunicyjnego zamilkł w 40. sekundzie”. Wiadomo wtedy, które skrzydło rozbudować.

## Ludzie i mutacje w szczegółach

Każdy mieszkaniec ma umiejętności, potrzeby, 2–3 cechy, poglądy na trzech osiach, relacje i poziom Odmienności. Mutacje dają realne korzyści, ale zawsze coś kosztują i odsuwają człowieka od reszty osady. Liczby poniżej to wartości robocze do balansu.

**Umiejętności (0–10, jedna wspólna lista dla cywilów i wojowników):** rosną od używania, a szkoła, kursy i Akademia je przyspieszają. Klasa wojownika wskazuje 2–3 umiejętności jako swoje: rosną u niego szybciej, a kursy podnoszą ich limit. Dawne osobne „rodziny broni” i umiejętności niebojowe są częścią tej listy.

| Grupa | Umiejętność | Do czego |
| --- | --- | --- |
| Praca | Praca fizyczna | budowa, uprawy, noszenie, kopanie okopów |
| Praca | Rzemiosło i inżynieria | kuźnia, szycie, naprawy, produkcja broni, umocnienia polowe, pułapki |
| Praca | Medycyna | leczenie w osadzie i w walce, stabilizowanie rannych |
| Praca | Nauka | badania, Bestiariusz, sekcje zwłok |
| Ludzie | Przywództwo | dowodzenie, podstawa do Akademii |
| Ludzie | Mowa | handel, dyplomacja, propaganda, przemawianie w walce |
| Teren | Skradanie | ciche podejście, zwiad, zasadzki |
| Teren | Tresura | psy bojowe, zwierzęta juczne |
| Broń | Łuki i kusze | łuki, kusze, proce |
| Broń | Broń prochowa | muszkiety, rusznice, broń skałkowa |
| Broń | Karabiny | karabin zwykły, wyborowy, szturmowy |
| Broń | Broń maszynowa i ciężka | kaemy, moździerze, broń wałowa |
| Broń | Broń krótka | pistolety, rewolwery, obrzyny |
| Broń | Broń drzewcowa | dzidy, włócznie, halabardy |
| Broń | Broń obuchowa i ciężka | młoty, pałki, topory dwuręczne |
| Broń | Ostrza | noże, maczety, miecze |
| Broń | Tarcza | blok, szyk, napieranie |
| Broń | Granaty i ładunki | rzut, ładunki, rozbrajanie |

**Potrzeby:** jedzenie, ciepło (ubiór i paliwo), odpoczynek, światło (ludzie mieszkający daleko od latarni boją się ciemności), bezpieczeństwo, wiara lub idea, bliscy. Niezaspokojone potrzeby obniżają morale i przesuwają poglądy.

**Cechy (przykłady):**

| Cecha | Efekt |
| --- | --- |
| Odważny | wyższe Opanowanie, czasem ignoruje rozkaz odwrotu |
| Tchórzliwy | szybciej wpada w panikę, rzadziej ginie |
| Okrutny | lepiej znosi egzekucje i twardą pracę, gorzej z jeńcami |
| Litościwy | wynosi rannych z własnej inicjatywy, cierpi przy surowych prawach |
| Zabobonny | Echo i Krzykacz działają mocniej, łatwo trafia do Kościoła Mgły |
| Twardziel | szybciej wraca do zdrowia po ranach |
| Chorowity | częściej choruje zimą |
| Pracowity / Leniwy | szybsza lub wolniejsza praca |
| Fanatyk | jego poglądy przesuwają się dwa razy szybciej i ciągną innych |
| Samotnik / Towarzyski | słabsze lub silniejsze premie od bliskich w składzie |

**Poglądy:** trzy wartości od −100 do 100 na osiach Władza, Własność i Mgła. Zmieniają je wydarzenia: śmierć bliskiego w Mgle (w stronę Czystości), mutacja bliskiego lub własna (w stronę Przemiany), głód przy równych racjach (w stronę Prywatnego), uratowanie przez dowódcę (w stronę Wodza), propaganda i sztandary.

**Relacje:** rodzina, przyjaźń, wrogość. Przyjaciel obok w składzie podnosi morale, śmierć bliskiego daje szok albo żądzę zemsty (agresja, ignorowanie rozkazów). Wrogowie w jednym składzie gorzej wykonują wspólne zadania.

**Cykl życia:** dzieci przychodzą z przybyszami lub rodzą się w osadzie i dorastają w trakcie kampanii. Prawo o pracy dzieci decyduje, czy pracują wcześniej (więcej rąk teraz, słabsze umiejętności i niższe morale później). Dzieci nie walczą.

**Odmienność (0–100):**

| Próg | Stan | Co się dzieje |
| --- | --- | --- |
| 0–24 | Czysty | brak objawów |
| 25–49 | Naznaczony | 1–2 mutacje, Czyści zaczynają być nieufni |
| 50–74 | Odmieniony | pełne mutacje, sny o Mgle, zdarzenia typu szept Mgły (sabotaż, ucieczka) |
| 75–99 | Na granicy | słyszy Mgłę, może odejść w Mgłę w nocy |
| 100 | Przemiana | odchodzi i wraca jako wróg z imieniem: Odmieniec, a rzadko Pasterz |

Nowa mutacja pojawia się przy przekroczeniu progów 15, 35, 55 i 75, losowana z puli zależnej od źródła. Technologia Kontrolowane mutacje pozwala wybierać.

**Źródła Odmienności:** czas w Mgle pomnożony przez ochronę ubioru i głowy, rozdarty ubiór, krzyk Krzykacza, Pijawiec, zarodniki Nosiciela i Ćm, rytuały Kościoła Mgły, przebywanie przy zarośniętych budynkach. **Spadek:** kwarantanna i oczyszczanie (gałąź Czystości) obniżają Odmienność powoli, ale nabyte mutacje zostają.

**Katalog mutacji:**

| Mutacja | Zysk | Koszt |
| --- | --- | --- |
| Oczy Mgły | widzenie w Mgle, demaskuje Naśladowców | jasne światło obniża celność |
| Trzecie ramię | szybsza praca, dodatkowy przedmiot w ręce (np. sztandar i broń) | pancerz i ubiór szyte na miarę, nie pasuje do Wzorów |
| Skóra z chityny | naturalny pancerz torsu | nie założy zwykłego pancerza, widoczna dla wszystkich |
| Płuca Mgły | nie potrzebuje maski, odporność na zarodniki | źle śpi w czystym powietrzu przy latarni |
| Echolokacja | wykrywa Bezokich i Kopaczy, widzi w Martwym Świetle | hałas broni mocniej go przydusza |
| Żywa tkanka | szybsze gojenie, odrastające kończyny | dwa razy większy głód |
| Szpony | walka wręcz bez broni, wolna ręka | gorsza celność bronią palną |
| Głos Krzykacza | krzyk przydusza wrogów | obniża też morale własnych |
| Zapach Mgły | mutanty dłużej go nie zauważają | ludzie i zwierzęta w osadzie mu nie ufają |
| Mięśnie Rozrostu | dużo więcej Siły | mniej Zwinności, pancerz tylko na miarę |
| Więź z Pasterzem | słyszy Pasterza, wie, skąd przyjdzie atak | Pasterz może go na chwilę przejąć w walce |

**Mutacje a osada i ustrój:** zmutowane ciała często nie pasują do Wzorów, więc potrzebują własnych. Junta wygania już od Naznaczonego, Kościół Mgły celowo podnosi Odmienność, Wolne Miasto handluje mutacjami, a w Reżimie Latarnika zmutowani noszą naszywki. Każda mutacja w osadzie przesuwa poglądy ludzi wokół niej.

## Standaryzacja i Wzory

Standaryzacja to badanie z mid-game'u, które nie daje premii do statystyk: nagrodą jest sama logistyka. Na początku broń pochodzi ze złomu, każda sztuka jest inna, a kalibry, części i dodatki się różnią. Ta niewygoda jest zamierzona.

**Ścieżka w drzewku:**

1. **Ewidencja zbrojowni** (wczesna gra): lista magazynu, filtry, podgląd, kto co nosi.
2. **Standaryzacja** (mid-game): odblokowuje Wzory, czyli szablony wyposażenia przypisane do roli, np. Strzelec-2: karabin z lunetą i bagnetem, nóż, lekki pancerz, 2 apteczki. Nowy członek składu dostaje zestaw z magazynu, a braki po stratach uzupełniają się same.
3. **Normy produkcji** (mid–late): zlecenie „20 kompletów według Wzoru Strzelec-2” zamiast osobnych zamówień na każdy element.
4. **Ujednolicony kaliber** (late): jeden lub dwa kalibry, prostsze zaopatrzenie i naprawy, stara broń na przetop.
5. **Kwatermistrzostwo** (late): kwatermistrz kolumny zaopatruje składy w terenie według Wzorów.

Wzory obejmują wszystkie sloty, także ubiór, i mogą dotyczyć cywilów (np. Robotnik: ubranie robocze, maska).

**Ustroje a standaryzacja:** Komuna i Reżim Latarnika mają tańsze Normy produkcji. Wolne Miasto kupuje gotowe komplety od handlarzy. Junta wymusza sztandar w każdym składzie. Kościół Mgły może wpisać mutację jako wymaganie roli.

Ponieważ Mgła uczy się twoich Wzorów, standaryzacja służy do szybkiej zmiany wyposażenia, a nie do ustawienia go raz na całą grę.

## Wrogowie

Mutanty są groźne przez liczebność, szybkość, zaskoczenie i dziwność, nie przez pasek życia. Idą do światła, reagują na hałas, a pokonane dają surowce: chitynę, tkankę, gruczoły, kości.

Wrogowie Mgły dzielą się na trzy grupy według pochodzenia, a siła objawów rośnie od zwierząt do Stworzeń Mgły:

- zwierzęta tylko lekko zmienione;
- ludzie zmienieni wyraźnie;
- Stworzenia Mgły, które nie mają żadnego naturalnego pierwowzoru, są samą infekcją.

**Zwierzęta zmienione przez Mgłę** (lekkie objawy: wciąż rozpoznawalne zwierzęta, które są wychudzone, wyłysiałe, mają białe oczy, wrzody i małe narosty):

| Wróg | Wygląd i zachowanie | Kontra |
| --- | --- | --- |
| Szczurza Fala | zwykłe szczury z wyłysiałymi plackami; dziesiątki zalewają skład | mur tarcz, broń obszarowa, ogień |
| Ogar | zwykły pies, wychudzony, z białymi oczami, odsłoniętymi dziąsłami i wrzodami na grzbiecie; szybki, obchodzi skład i atakuje strzelców | łamacz, bagnety, osłona tyłów |
| Wataha | wilki polujące razem: dwa wilki odciągają, reszta zachodzi od tyłu | szyk zamknięty, zwiadowca, ogień na wycie |
| Ryjec | dzik z przerośniętymi kłami; ryje pod palisadą i szarżuje na nogi | dzidy, pułapki, łamacz |
| Rogacz | jeleń albo tur z przerośniętymi rogami; szarża przewraca tarczowników | dzidy wbite w ziemię, jeż, rozstąpienie szyku |
| Padlinniki | stado wron i kruków; dziobią rannych i kradną łup | strzelcy, śrót, osłona tragarza |
| Ćma Latarniowa | wielka ćma; leci na światło, wybucha zarodnikami | brak latarek, strzelcy, kombinezon |
| Pajęczarz | pająk wielkości psa; sieci w ruinach unieruchamiają | zwiadowca, noże, ogień |

**Stworzenia Mgły** (zrodzone bezpośrednio z Mgły, bez pierwowzoru w naturze: pulsująca tkanka, worki zarodników, świecące gruczoły, mgła sącząca się z ciała; to tutaj są najmocniejsze objawy infekcji):

| Wróg | Zachowanie | Kontra |
| --- | --- | --- |
| Bezoki | gałąź tkanki bez oczu, poluje na dźwięk | tłumiki, kusze, walka wręcz |
| Kopacz | robak z tkanki, wynurza się pod środkiem szyku | zwiadowca wyczuwa drgania, szyk rozproszony |
| Pijawiec | przyczepia się do ciała i zostaje po walce | felczer, ogień, kombinezon |
| Zwiewnik | kształt z gęstej mgły, widoczny tylko w świetle; przechodzi przez tarcze | latarnik bojowy, flary, światło |
| Nici | macki z mgły wyciągają ludzi spoza kręgu światła | krąg światła, ostrza, nie zostawać z tyłu |
| Kłębowisko | toczy się jak masa tkanki, wchłania trupy i rośnie | ogień, palić zwłoki po walce |
| Oddech | chmura, która dusi i ignoruje pancerz | maski, wiatr, ucieczka z niecki |
| Gasiciel | rzuca się na źródła światła i je gasi | osłona latarnika, zapasowe lampy |
| Siewca | nieruchoma wieża tkanki, wypuszcza zarodniki i rodzi Bezokich | ogień, saper, szturm na rdzeń |
| Matka Roju | rodzi kolejne fale | przebicie się do gniazda, ogień |

**Ludzie zmienieni przez Mgłę** (dawni ludzie: wyraźne objawy, ale wciąż ludzka sylwetka, strzępy ubrań i sprzętu):

| Wróg | Zachowanie | Kontra |
| --- | --- | --- |
| Odmieniec | wolny, nie czuje bólu, rozrywa pancerz i ubiór; czasem ktoś z twojej osady | trafienie w głowę lub kręgosłup, łamacz, sztandar na morale |
| Krzykacz | z tyłu, krzyk podnosi Odmienność i panikę | pierwszy cel strzelca z lunetą, maska |
| Naśladowca | naśladuje głosy rannych, wciąga w zasadzkę | zwiadowca z oczami Mgły, doświadczony dowódca |
| Śpiący | stoją w Mgle, budzą się na hałas i światło | cichy zwiad, ominięcie |
| Nosiciel | wybucha przy śmierci | zabić z dystansu |
| Zrośnięci | po śmierci rozpadają się na walczące części | łamacz, broń obszarowa |
| Dawny Żołnierz | strzela, kryje się, rzuca granaty | tarcze, osłony, ogień tłumiący |
| Opiekunka | porywa rannych w Mgłę | wynoszenie rannych, misja ratunkowa |
| Pielgrzymi | neutralni, idą ku Sercu Mgły | nie atakować, iść za nimi |
| Echo | widmo kogoś poległego, uderza w morale | światło, sztandar |
| Pasterz | kieruje innymi mutantami jak dowódca | zabić go, a reszta się rozpierzcha |
| Rozrost | ogromny, ma kilka organów do zniszczenia (należy do Stworzeń Mgły, patrz zmiany niżej) | pełny, zbalansowany skład, ogień |

Kolejni ludzie zmienieni przez Mgłę:

| Wróg | Zachowanie | Kontra |
| --- | --- | --- |
| Biegacz | świeżo przemieniony, szybki i jeszcze bez pancerza z tkanki; atakuje w grupach | strzelcy, mur tarcz |
| Wydrążony | sama skóra i pancerz poległego, kierowane tkanką w środku; często nosi twój sprzęt | łamacz, ogień (tkanka pod pancerzem) |
| Fałszywy Latarnik | przemieniony Latarnik ze świecącą lampą zwabia wyprawy w Mgłę | zwiadowca z oczami Mgły, nie iść za obcym światłem |
| Kokon | człowiek w trakcie przemiany; można go uratować, jeśli szybko wypali się tkankę | wybór: uratować (felczer + ogień) albo zabić |

**Ludzkie frakcje:** każda ma ustrój, własne jednostki i dowódców o tej samej architekturze co gracz. Jednostki frakcji korzystają z tych samych klas i stopni co twoi wojownicy, więc ich siła zależy od szkół, jakie mają. Barwy i znaki są docelowe. Znak pojawia się na sztandarach, tarczach, opaskach, budynkach i pojazdach, a barwy na ubiorze. Na tle Mgły wszystkie barwy są przygaszone, żeby nie gryźły się z paletą gry.

| Frakcja | Charakter | Barwy | Znak | Jednostki |
| --- | --- | --- | --- | --- |
| Szabrownicy | banda, uciekają po śmierci Herszta | rdza i brudna żółć | żółta strzała w dół (znak „tu jest łup”), malowana farbą | Obdartus (rura), Nożownik, Obrzyn, Butelkarz (koktajle zapalające), Rozbijacz (młot, blacha), Psiarz (zwykłe psy), Herszt |
| Synowie Czystości | skrajna Junta, najpierw atakują twoich zmutowanych | biel i krwista czerwień | zamknięte oko w płomieniu | Pokutnik (bicz, bez pancerza), Akolita (dzida i tarcza), Strzelec Czystości, Płomieniarz, Kat (topór), Templariusz (ciężka zbroja), Egzorcysta (dzwon), Inkwizytor |
| Chór Mgły | kult Przemiany, walczy razem z mutantami | blady fiolet i kość | otwarte usta w spirali | Wierny, Odmieniony Wierny, Śpiewak (pieśń wzmacnia mutanty), Niosący Zarodnik (ludzka bomba), Sługa w tkance, Kapłan, Pasterz Chóru |
| Ludzie Toru | nomadzi z pociągiem pancernym, handel i najemnicy | czerń sadzy i mosiądz | dwie szyny przecięte kołem | Konwojent, Hamulcowy (strzelba), Palacz (łopata, odporny na ogień), Kaemista z wagonu, Zwiadowca na drezynie, Najemnik Toru, Maszynista |
| Łowcy Głów | porywają cywilów z karawan i obrzeży | bagienna zieleń i czerwień | sieć z hakiem | Sieciarz, Łowca (kusza z usypiaczem), Ogłuszacz (pałka), Tropiciel, Poganiacz (bicz, prowadzi jeńców), Herszt Łowców |
| Zakon Latarników | technokracja, wystandaryzowani, dowódcy 4–5 | granat i żółć lampy | latarnia w kole z ośmioma promieniami | Kadet, Strzelec Zakonu, Tarczownik Zakonu, Sygnalista, Inżynier, Latarnik bojowy (lanca), Oficer |
| Dezerterzy | twoi wygnańcy, znają słabe punkty osady | twoje barwy, przekreślone | twój sztandar do góry nogami | te same klasy co twoi wojownicy, w sprzęcie, który ze sobą zabrali |
| Bractwo Bunkra | resztki wojska ze schronu, wcześnie mają broń z epoki Naboju, ufają tylko swoim | oliwka i ostrzegawczy pomarańcz | maska gazowa w trójkącie ostrzegawczym | Szeregowiec w masce, Kaemista, Snajper, Saper, Medyk, Pancerny (ciężki kombinezon), Sierżant |
| Gildia Solna | liga kupiecka, bije monetę i handluje solą, wynajmuje najemników | biel soli i błękit | waga z kryształem soli | Strażnik karawany, Pikinier karawany, Kusznik Gildii, Najemnik, Zabójca (cichy nóż), Poganiacz wołów, Faktor |
| Dzieci Lasu | żyją na skraju Mgły w lasach, pułapki i łuki | mech i ochra | poroże z gałęzi | Łucznik, Traper, Oszczepnik, Zwiadowca z psem, Strzelec z muszkietem, Strażnik Gaju (topór), Szaman (zioła, maski z kory) |
| Republika Tamy | społeczność elektrowni wodnej, własna sieć światła, rywal latarni | stal i turkus | błyskawica nad łukiem tamy | Milicjant, Elektryk (pałka elektryczna), Strzelec, Operator reflektora, Inżynier, Gwardzista Tamy (pancerz), Komisarz |

**Zmiany zatwierdzone 8 listopada** (szczegóły w `WROGOWIE.md`):

- **Linia przemiany:** Kokon, potem Biegacz, Odmieniec i Wydrążony to kolejne stadia jednej linii. Śpiący to stan uśpienia Odmieńców w Mgle.
- **Kłębowisko** to młody Rozrost: rośnie, pożerając trupy, i po kilku walkach staje się Rozrostem. Rozrost przechodzi do Stworzeń Mgły.
- **Siewca**, który przetrwa Przypływ, staje się Matką Roju (bossem). Zarośl to teren, na którym rosną Siewcy.
- **Nici i Oddech** to anomalie terenu, a nie stworzenia.
- **Fałszywy Latarnik** to odmiana Naśladowcy, która naśladuje światło zamiast głosu.
- **Licznik adaptacji Mgły:** każda kategoria (światło, pancerz, broń palna, ogień, wręcz, cisza) ma licznik. Przy 60 zwiad zapowiada zmianę, przy 80 Mgła adaptuje się na 15 dni. Najwyżej 2 adaptacje naraz.

**Anomalie:** Cisza (rozkazy i meldunki nie działają), Pętla (zakrzywiona przestrzeń), Czerwony Deszcz (niszczy ubiór), Martwe Światło (gasną latarki), Rozlewisko (ciężki pancerz wciąga na dno), Zarośl (nieruchoma tkanka rodząca mutanty).

**Bossowie:** Topielec (atakuje latarnię podczas Przypływu), Wędrujący Las (Zarośl idąca na osadę), Wielki Pasterz (kieruje regionem), Prorok Chóru i Wielki Inkwizytor, Serce Mgły (finał).

**Konflikty kontr:** Szczurza Fala wymaga ścisłego szyku, a Kopacz rozproszonego. Bezoki wymaga ciszy, Ćmy ciemności, a Echo groźne jest tylko bez światła. Nosiciel wymaga dystansu, a Bezoki walki wręcz. Nie ma jednego idealnego Wzoru, wygrywa dowódca, który zmienia taktykę w trakcie walki.

**Mgła się uczy:** mutanty ewoluują w odpowiedzi na Wzory gracza. Dużo latarek oznacza więcej Ćm, ciężkie pancerze więcej Kopaczy i Rozlewisk, dużo broni palnej więcej Bezokich, dużo ognia Ogary odporne na ogień. Zwiadowcy zapowiadają te zmiany.

## Walka

Walka toczy się w czasie rzeczywistym z przyspieszeniem (x1, x2, x4) i trwa tyle, ile potrzeba do rozstrzygnięcia: od 10 sekund zasadzki do kilku minut obrony podczas Przypływu. Śmiertelność jest realistyczna, więc wygrywa ten, kto pierwszy zobaczy, ma lepszą pozycję i zmusi wroga do schowania się.

**Środowisko:** światło ułatwia widzenie, ale przyciąga mutanty. Hałas (strzały, krzyki, meldunki) ma zasięg słyszalności. Mgła obniża widoczność i podnosi Odmienność. Mapa ma osłony: murki, wraki, gruz.

**Rany zamiast punktów życia:**

- Strefy trafień: głowa, tors, ręce, nogi.
- Trafienie zadaje ranę lekką, ciężką albo śmiertelną, zależnie od broni, przebicia i strefy.
- Skutki: krwotok (śmierć z czasem bez pomocy), złamana ręka (wypada broń), trafienie w nogę (brak ruchu), wstrząs (kilka sekund poza walką).
- Celne trafienie z karabinu w tors zwykle od razu wyłącza człowieka z walki.
- Ranny pada i krwawi. Medyk lub towarzysz może go ustabilizować i wynieść, inaczej umiera albo porywa go Opiekunka.
- **Pancerz kontra przebicie:** gdy przebicie jest równe lub większe od pancerza, rana przechodzi w całości; każdy brakujący punkt osłabia ją (robocza wartość: 20% na punkt).

**Co zabija mutanty:** Szczurza Fala i Nosiciel giną od jednego trafienia. Ogar od 1–2 trafień z karabinu w tors. Chityna Stworzeń Mgły i Rozrostu odbija nóż, pistolet i strzały pod kątem. Odmieniec idzie dalej mimo ran i ginie od trafienia w głowę lub kręgosłup. Rozrost nie ma paska życia, tylko kilka organów do zniszczenia po kolei.

**Broń (wartości robocze do balansu):**

| Broń | Epoka | Ręce | Przebicie | Zasięg (metry: skuteczny / maksymalny) | Tempo | Hałas | Uwagi |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Nóż | 1 | 1 | 1 | wręcz | szybkie | cicha | broń boczna |
| Dzida | 1 | 2 | 3 | 2 m | średnie | cicha | zatrzymuje szarżę |
| Włócznia | 1 | 1 | 2 | 2 m | średnie | cicha | z tarczą |
| Łuk | 1 | 2 | 2 | 40 / 70 m | średnie | cicha | potrzebna Siła |
| Młot | 2 | 2 | 6 | wręcz | wolne | średnia | łamacz, ogłusza |
| Kusza | 2 | 2 | 4 | 50 / 75 m | wolne | cicha | przeciw Bezokim |
| Obrzyn | 3 | 1 | 2 | 15 / 25 m | wolne | bardzo głośna | stożek |
| Pistolet | 5 | 1 | 2 | 30 / 50 m | szybkie | głośna | broń boczna |
| Karabin | 5 | 2 | 4 | 60 / 100 m | średnie | bardzo głośna | podstawa strzelca |

**Pistolet skałkowy** (epoka 3): 1 ręka, przebicie 2, zasięg 20 / 30 m, bardzo wolne przeładowanie, głośny. Pistolet w tabeli wyżej to pistolet nabojowy z epoki Naboju. W tabeli sprzętu szkół „pistolet” w epoce Czarnego prochu oznacza pistolet skałkowy.

**Morale (0–100, osobno dla każdego):**

| Próg | Stan | Efekt |
| --- | --- | --- |
| 70+ | Pewny | normalnie |
| 40–69 | Roztrzęsiony | gorsza celność, wolniejsze wykonywanie rozkazów |
| 15–39 | Panika | może zignorować rozkaz, cofa się, strzela na oślep |
| poniżej 15 | Złamany | ucieka albo się kuli |

Morale obniżają straty, widok rannych, Krzykacz, Echo, przewaga wroga, brak amunicji i odcięcie od dowódcy. Podnoszą sztandar, bliski dowódca, zabicie dużego wroga, mundur i ustrój.

**Przyduszenie:** pociski przelatujące blisko podnoszą przyduszenie. Przyduszony strzela gorzej, nie wychyla się i wolniej wykonuje rozkazy. Zwierzęta reagują strachem: płoszą się albo wpadają w szał.

**Zmęczenie:** długa walka wręcz męczy, co obniża skuteczność. Dlatego potrzebna jest rotacja linii.

**Bratobójczy ogień:** pociski i odłamki lecą fizycznie i trafiają swoich. AI aktywnie tego unika: przed strzałem sprawdza, czy ktoś swój nie stoi na linii, i robi krok w bok, przyklęka albo czeka; idący wybierają trasę, która nie przecina linii strzału osłaniających. Do trafień we własnych dochodzi przy panice, słabym wyszkoleniu, walce wręcz z kimś swoim, granacie od framugi i chaosie po śmierci dowódcy. Raport to odnotowuje. W Juncie za taki strzał grozi kara, w Kościele Mgły uchodzi za wolę Mgły.

## AI dowódcy

Dowódca to planujące AI gry taktycznej, bez sieci neuronowych i modeli językowych. Myśli w pięciu krokach, wydaje rozkazy tylko wtedy, gdy coś zmieniają, i wie tylko to, co zobaczyli lub usłyszeli jego ludzie. Słabszy dowódca nie myli się losowo, tylko ma ludzkie ograniczenia.

**Dwie warstwy:**

- **Wojownik (instynkt, zawsze):** strzela do widocznego celu, zmienia broń, przeładowuje, kryje się, pilnuje linii strzału, wykonuje ostatni rozkaz.
- **Dowódca (myślenie, co 1–3 s):** pięć kroków poniżej.

**Pięć kroków:**

1. **Obraz sytuacji:** wrogowie grupowani w zagrożenia z kierunkiem, siłą, typem (z Bestiariusza) i czasem dotarcia; gruba mapa zagrożeń i osłon.
2. **Zadania:** każde zagrożenie to zadanie z wymaganą siłą i preferowanymi rolami, np. zatrzymać lewą flankę: siła 18, najlepiej łamacz i tarczownik.
3. **Przydział ludzi:** ocena każdego wojownika pod każde zadanie (dopasowanie roli, odległość, stan, koszt odejścia z obecnej pozycji); przydział od najważniejszego zadania, aż siła wystarczy z zapasem, reszta zostaje lub idzie do odwodu.
4. **Rozkaz tylko z sensem:** plan trwa minimalny czas (np. 4 s), nowy plan zastępuje stary tylko wtedy, gdy jest wyraźnie lepszy (np. o 20%), sytuacje nagłe przerywają to od razu, a rozkaz już obowiązujący nie jest powtarzany.
5. **Rozkazy imienne:** „Uwaga, lewa flanka! Bartek, Ola, trzymać lewą! Reszta, front!” Wojownicy odpowiadają („Mam ich!”, „Nie utrzymam!”), co uruchamia przeplanowanie.

**Poziom ogólny (1–5):**

| Poziom | Cykl decyzji | Podgrupy | Przewidywanie | Pamięć o wrogach |
| --- | --- | --- | --- | --- |
| 1 | brak, sam instynkt | brak | brak | zapomina od razu |
| 2 | co 3 s | 1 | brak | kilka sekund |
| 3 | co 2 s | 2 | około 1 s | kilkanaście sekund |
| 4 | co 1,5 s | 3 + odwód | 2–3 s | długa |
| 5 | co 1 s | koordynacja kolumny | 3–5 s | pełna |

**Trzy specjalizacje (1–5, obok poziomu ogólnego):**

| Specjalizacja | Odpowiada za | Słaby | Mocny |
| --- | --- | --- | --- |
| Zasoby ludzkie | role, przydziały, osłona, rotacja zmęczonych, amunicja, ranni | snajper na szturmie, medyk pod ogniem | snajper zostaje z tyłu, łamacz idzie na pancerz, granaty nie są marnowane |
| Orientacja w terenie | pozycja, trasa, kluczowy teren | staje na otwartym, nie widzi wąskich gardeł | opiera flankę o budynek, wzniesienie dla strzelców, omija lejki śmierci, punkt zbiórki |
| Taktyka i formacje | formacje, techniki ruchu, drile | jedna formacja, wszyscy idą naraz | dobiera i zmienia formacje, czysto wykonuje drile |

Specjalizacje się składają: dowódca z Terenem 5 i Formacjami 1 wybierze świetną pozycję, ale dojdzie do niej tłumem. Raport przypisuje każdy błąd do konkretnej specjalizacji, więc wiadomo, który kierunek Akademii szkolić.

**Pozostałe mechanizmy:**

- **Sektory obserwacji:** od Formacji 3 każdy pilnuje innego kierunku, więc flanki są wykrywane wcześniej.
- **Ogień i manewr:** część składu tłumi, reszta się przemieszcza lub flankuje.
- **Plan awaryjny:** od poziomu 4 każdy plan ma warunek zerwania i punkt zbiórki.
- **Charakter dowódcy:** ostrożny, brawurowy, uparty, opiekuńczy; przesuwa wagi rozkazów niezależnie od poziomu.
- **Bestiariusz:** od poziomu 4 dowódca korzysta z wiedzy o wrogach. Nieznany wróg jest traktowany ogólnie nawet przez najlepszego dowódcę. Wiedzę dają walki, sekcje zwłok, badania i zwiad.
- **Wiedza z oczu i uszu:** meldunki trzeba wykrzyczeć, a krzyk robi hałas. W anomalii Cisza nie działają, pomagają Sygnały ręczne.
- **Wykonanie rozkazu:** zależy od morale, dyscypliny (ustrój, mundur) i relacji z dowódcą.
- **Łańcuch dowodzenia:** po upadku lidera 3–5 sekund chaosu, potem zastępca. Brak zastępcy oznacza spadek do poziomu 1.
- **Oficer kolumny:** wydaje rozkazy składom (Kowadło, Młot, Wsparcie, Odwód), liderzy wykonują je na własnym poziomie.
- **Wrogowie używają tego samego mózgu:** ludzkie frakcje i Pasterz mają dowódców tej samej architektury; zwierzęta mają proste profile (drapieżnik, rój, zasadzkowiec).
- **Rozwój:** trafne decyzje dają doświadczenie, Akademia (trzy kierunki) wyznacza limit poziomu, szkolenie zabiera dowódcę ze służby na kilka dni.

**Wydajność:** cykle dowódców rozłożone na różne klatki, percepcja wojowników co około 0,2 s, kluczowy teren wyliczany raz przy generowaniu mapy, wspólne mapy ruchu dla rojów, AI w osobnym wątku w razie potrzeby. Cel: kilkanaście składów i kilkaset mutantów naraz na zwykłym komputerze.

## Formacje, ruch i drile

Składy używają prawdziwych formacji i technik z różnych epok wojskowości. Formacja to szablon pozycji względem lidera, który sam się zwęża w ciasnym terenie i rozszerza na otwartym. Które formacje są dostępne, zależy od epoki uzbrojenia i od specjalizacji Formacje.

**Formacje strzeleckie (współczesne regulaminy):**

| Formacja | Kiedy | Mocne strony | Słabe strony |
| --- | --- | --- | --- |
| Klin | podstawowa, otwarty teren | ogień do przodu i na boki | trudniejsza kontrola |
| Rząd | las, ruiny, korytarze | łatwa kontrola, jedna trasa | ogień do przodu zasłonięty |
| Kolumna przestawna | ulica, droga | dwa rzędy po obu stronach, sektory na okna naprzeciwko | słaba do przodu |
| Linia | szturm, obrona frontu | maksimum ognia do przodu | odsłonięte boki |
| Rzut | zagrożenie z jednej strony | silny ogień na tę stronę | słaba druga strona |
| Romb / obrona okrężna | postój, otoczenie | ochrona ze wszystkich stron | wolny ruch |

**Formacje do walki wręcz (starożytność i średniowiecze, głównie przeciw mutantom):**

- **Mur tarcz:** tarczownicy ramię w ramię, dzidy z drugiego szeregu. Przeciw rojom. Po przerwaniu szybko się rozpada.
- **Jeż (schiltron):** krąg dzid, przed nim wbite pale. Przeciw szarży Rogaczy i Ogarów.
- **Rotacja linii:** zmęczona pierwsza linia cofa się, druga zajmuje jej miejsce, jak u Rzymian.
- **Kontramarsz:** strzelcy z bronią jednostrzałową lub kuszami w kilku szeregach; pierwszy strzela i cofa się przeładować, następny wychodzi naprzód.
- **Tarcze i strzelcy (pike and shot):** blok tarcz i dzid w środku, strzelcy na skrzydłach. Naturalna formacja składu z mieszanym uzbrojeniem.
- **Orbis:** krąg ostatniej obrony plecami do siebie, ranni w środku.

**Techniki ruchu:** marsz (kontakt mało prawdopodobny, szybko), marsz z osłoną (kontakt możliwy, tył trzyma większy dystans), ruch skokami (kontakt spodziewany, jedna grupa zawsze stoi i osłania drugą; wariant kolejny i przemienny).

**Drile** to gotowe, wyuczone reakcje: dowódca je uruchamia, a skład wykonuje bez dalszego myślenia, więc wyszkolony skład reaguje szybciej.

| Dril | Przebieg | Wymaga |
| --- | --- | --- |
| Reakcja na kontakt | ogień, osłona, meldunek, decyzja; jedna grupa tłumi, druga flankuje | wszyscy |
| Zerwanie kontaktu | grupa ogniowa tłumi, reszta się wycofuje, potem zamiana | Formacje 2+ |
| Daleka zasadzka | osłona, ogień, flankowanie albo zerwanie kontaktu | Formacje 2+ |
| Bliska zasadzka | wróg w zasięgu granatu: przebić się przez zasadzkę pod ogniem tłumiącym | Formacje 3+ |
| Własna zasadzka w kształcie L | dwie grupy pod kątem prostym, ogień z frontu i z boku | Formacje 4 + Teren 3 |
| Przejście przez strefę niebezpieczną | zabezpieczenie obu stron, przejście parami, dym | Formacje 2+ |
| Wejście do budynku | stos przy drzwiach, granat, wejście po wybuchu | Formacje 3+ |
| Mur, jeż, orbis na rozkaz | przejście w formację wręcz w 2–3 s | Formacje 2+ |

**Granaty:** rzucane przed wejściem do pomieszczenia (drzwi to lejek śmierci), w rój za murem tarcz, w gniazdo, pod Rozrost. Rzucający wybiera wcześniej bezpieczne miejsce na wypadek odbicia od framugi i kryje się po rzucie. Granaty dymne zasłaniają przejście ulicą i odwrót, a w Ciszy zastępują sygnał. Decyzja o rzucie uwzględnia odległość od swoich, osłonę, wartość celu i zapas granatów.

**Wykonanie w kodzie:** formacja to zestaw slotów względem lidera, technika ruchu dzieli skład na grupy, dril to automat stanów, kluczowy teren jest wyliczany raz, linia strzału sprawdzana tylko przy strzale. Specjalizacje dowódcy działają jako filtry i wagi w tych systemach.

## Oglądanie walki i raport

Gracz ogląda walkę na żywo, a po niej dostaje raport, który mówi, czy zawiódł skład, sprzęt czy dowódca. Czas w osadzie zatrzymuje się na czas oglądania.

- **Dymki i odpowiedzi:** rozkazy dowódcy i meldunki wojowników nad głowami, ze strzałkami. Brak reakcji też jest widoczny, np. znak zapytania, gdy wróg obchodzi skład niezauważony.
- **Automatyczne spowolnienie** w kluczowych momentach: wykrycie flanki, upadek dowódcy, wybuch Nosiciela.
- **Raport po walce:** oś czasu kluczowych momentów, rany zadane i otrzymane przez każdego, każda decyzja dowódcy z oceną (trafna, spóźniona, błędna, brak reakcji), przydziały ludzi i diagnoza, np. „Flanka niezauważona, potrzebny dowódca z Formacjami 3 albo łamacz na lewym skrzydle”.
- **Powtórka z nakładkami:** sektory obserwacji, przydziały do zadań, mapa zagrożeń i to, co dowódca wtedy wiedział.
- **Plan przed walką** (obrona i zaplanowane ataki): gracz zaznacza na mapie punkt zbiórki, kierunek głównego zagrożenia i strefę, której nie wolno oddać. Dowódca traktuje to jako zadania stałe, a raport ocenia też plan gracza.
- **Flary sygnałowe:** 1 interwencja na walkę w Złomie, 2 od Czarnego prochu (Dzwony alarmowe), 3 od Silnika (Radio). Flara to wymuszony odwrót, odwód z osady w obronie albo zgaszenie świateł. Flary trzeba wyprodukować.
- **Rozkazy przed wyprawą:** agresja, próg odwrotu, priorytet celów, ratować łup czy ludzi.
- **Każda walka jest oglądana** (decyzja z 8 listopada). Walk jest więc mniej, ale każda się liczy. Robocze tempo to około 2–4 walk na tydzień gry. W tle, tą samą symulacją, rozstrzygają się tylko eskorty karawan na stałych szlakach. Gdy wybuchają dwie walki naraz, ogląda się je po kolei, a druga czeka zamrożona.
- **Plac ćwiczeń:** test składu przeciw wrogowi z Bestiariusza w symulacji. Kosztuje czas i amunicję, nie życie, i daje dowódcom trochę doświadczenia do limitu Akademii.
- **Zwiad przed wyprawą:** daje częściowe wpisy w Bestiariuszu o tym, co czeka w strefie, kosztem czasu.

## Osada i ekonomia

Osadę buduje się swobodnie na siatce w promieniu światła, a gospodarka opiera się na około 15 surowcach w krótkich łańcuchach 2–3 kroków, żeby uwaga gracza została przy ludziach i walce. Liczby i nazwy to propozycja do balansu.

**Zasady budowy:**

- **Kratka osady ma 2 m** (decyzja z 8 listopada). Promień latarni 7–16 kratek to 14–32 m. Walka ma siatkę 0,5 m, a zasięgi broni są podawane w metrach. Obszary (ogień, wybuchy) podaje się w kratkach po 2 m.
- **Nocna obrona toczy się na mapie osady:** budynki, palisada i brama, które zbudował gracz, są polem walki.

- Budynki stawia się dowolnie, ale tylko w promieniu światła latarni lub latarni pomocniczej. Poza światłem podczas Przypływu zarastają tkanką.
- Im dalej od latarni mieszkają ludzie, tym bardziej boją się ciemności (potrzeba światła), więc centrum jest najcenniejszym miejscem.
- Ruiny w promieniu można rozebrać na złom i gruz.
- Palisady, mury i bramy kształtują nocną obronę: dowódca z wysoką Orientacją w terenie wykorzystuje je jak wąskie gardła.

**Surowce:**

| Surowiec | Skąd | Na co |
| --- | --- | --- |
| Paliwo | ropa i olej z wypraw, tłuszcz mutantów, później gruczoły | latarnia, lampy, ogień bojowy, maszyny |
| Drewno | wyręb przy granicy Mgły, rozbiórka | budowa, opał zimą, drzewca, łuki |
| Złom | rozbiórka ruin, wyprawy | przetop na metal, części zastępcze |
| Metal | przetop złomu w Kuźni | broń, pancerze, narzędzia, budowa |
| Gruz i cegła | rozbiórka | mury, mocne budynki |
| Żywność | uprawy w ciemności (grzyby, ziemniaki), pola przy latarni, polowania, łowienie | racje |
| Racje | Kuchnia, Wędzarnia | jedzenie osady i wypraw |
| Woda | studnia, filtry | picie, uprawy, szpital |
| Tkanina | len i konopie, szmaty z wypraw | ubiór, bandaże, sztandary |
| Skóra | zwierzęta i mutanty | ubiór, pasy, osłony |
| Chityna | Stworzenia Mgły, Rozrost (nie zwierzęta) | pancerze, tarcze |
| Tkanka | mutanty, zarośnięte budynki | leki, badania, kombinezony Kościoła Mgły |
| Proch | saletra, siarka, węgiel drzewny (od epoki Czarnego prochu) | amunicja, granaty |
| Amunicja | Rusznikarnia: proch + metal | broń palna |
| Leki | zioła, tkanka, Apteka | leczenie, kwarantanna |
| Części | wyprawy, Warsztat maszynowy (od epoki Pary) | broń powtarzalna, maszyny, naprawa latarni |

**Przykładowe łańcuchy:** złom → metal → broń / pancerz · żywność → racje · saletra, siarka i węgiel → proch → amunicja · len → tkanina → ubiór · mutant → chityna → pancerz · mutant → tłuszcz → paliwo.

**Budynki (propozycja):**

| Grupa | Budynki |
| --- | --- |
| Światło | Latarnia (poziomy soczewek), Lampa pomocnicza, Posterunek świetlny, Skład paliwa |
| Mieszkanie | Szałas → Barak → Dom |
| Żywność i woda | Piwnica grzybowa, Pole przy latarni, Kuchnia, Wędzarnia, Studnia, Filtr wody |
| Produkcja | Złomowisko, Kuźnia, Szwalnia, Garbarnia, Wytwórnia prochu, Rusznikarnia, Topielnia tłuszczu, Warsztat maszynowy |
| Wojsko | Zbrojownia, Akademia, Plac ćwiczeń, Wieża strażnicza, Palisada i mur, Brama |
| Społeczeństwo | Ratusz, Szpital, Kwarantanna, Szkoła, Kaplica lub świątynia, Radiowęzeł, Pomnik, Sąd i areszt, Cmentarz |
| Nauka | Pracownia, Prosektorium (wpisy do Bestiariusza) |
| Handel | Targ, Stacja karawan |

**Paliwo i latarnia:** promień wynika z ilości spalanego paliwa na godzinę, a poziomy soczewek podnoszą wydajność. Noc i zima zużywają więcej, a Przypływ wymaga pełnej mocy. Latarnie pomocnicze mają mniejszy promień i własny zapas, który trzeba dowozić tragarzami lub karawaną. Gdy zapas się skończy, posterunek gaśnie i zostaje odcięty.

**Praca:** cywilów przydziela się do budynków, a prawa decydują o długości zmian, pracy dzieci i o tym, czy zmutowani dostają najgorsze prace. Wojownicy mogą mieć drugą rolę w osadzie między wyprawami.

**Skąd biorą się ludzie:**

- **Przybysze:** uciekinierzy przychodzą do światła, częściej przy większym promieniu i lepszej sławie osady. Gracz decyduje: przyjąć, odmówić albo wpuścić przez kwarantannę. Przybysz może mieć wysoką Odmienność, a nawet być Naśladowcą.
- **Wyprawy:** ocaleni znalezieni i uratowani przez składy.
- **Narodziny:** rodziny potrzebują domu i jedzenia, dzieci dorastają w trakcie kampanii.
- **Jeńcy i handel:** wcielanie jeńców (zależnie od ustroju: pobór w Juncie, reedukacja w Komunie, najem w Wolnym Mieście), wykup ludzi od Łowców Głów, najemnicy od Ludzi Toru (drodzy i mało lojalni).

Każdy nowy człowiek przychodzi z własnymi poglądami, więc duża fala przybyszów potrafi przesunąć politykę osady.

**Pory roku w gospodarce:**

| Pora | Paliwo | Żywność | Wyprawy |
| --- | --- | --- | --- |
| Wiosna | pełna moc na Przypływ | zasiewy | ograniczone przez Przypływ |
| Lato | najmniejsze zużycie | wzrost upraw | sezon wypraw i handlu |
| Jesień | gromadzenie zapasów | żniwa, wędzenie | ostatnie karawany |
| Zima | największe zużycie, ogrzewanie | tylko zapasy i piwnice | trudne, gęsta Mgła |

**Handel:** wymienny, a najbardziej płynnymi towarami są paliwo, amunicja i leki. Ceny zależą od frakcji i jej epoki technologicznej.

### Ekonomia pierwszych 36 dni (liczby robocze, jeden zestaw od 8 listopada)

Start jest walką o przetrwanie: od pierwszego dnia brakuje paliwa i jedzenia po równo. Liczby sprawdziła symulacja `narzedzia/ekonomia_sim2.py` (dni 1–36: jesień, pierwsza zima i pierwszy Przypływ), opisana na końcu tej sekcji. Tabela `LICZBY` w tym skrypcie jest jedynym źródłem liczb ekonomii, a ta sekcja ją streszcza.

**Czas:**

- 1 dzień gry trwa około 12 minut przy tempie x1: 14 godzin dnia to około 8 minut, 10 godzin nocy około 4 minut.
- Zmiana robocza trwa 10 godzin.
- Pora roku trwa 15 dni, więc rok ma 60 dni. Gra zaczyna się wczesną jesienią, a pierwsza zima przychodzi 16. dnia.

**Start:**

- **10 osób:**
  - 3 wojowników, którzy w dzień pracują jak inni;
  - 5 dorosłych;
  - starzec z Nauką 5, który bada;
  - dziecko, które nie pracuje i je połowę racji.
- **Zapasy:**
  - paliwo 20 (około 2,5 dnia);
  - racje 25;
  - drewno 20;
  - złom 10;
  - woda 15.
- **Budynki:** latarnia bez soczewek, 3 szałasy, zepsuta pompa (naprawa: 4 złomu, potem 12 wody dziennie).
- **Ruiny w promieniu światła:** 3 wraki aut, stodoła, dom w ruinie.
- **Znane od startu:**
  - Szałas, Kuchnia, Smolarnia, Piwnica grzybowa;
  - Wyrąb, Palisada, Wieża strażnicza, Beczki na deszczówkę.
- **Pierwsze badania (50–100 punktów):**
  - Studnia, Skład paliwa, Lampy oliwne;
  - Dzidy i włócznie, Obserwacja Mgły, Ratusz.

  Badacz daje dziennie 2 punkty za każdy punkt Nauki, więc starzec daje 10 punktów dziennie.

**Zużycie dzienne:**

- racje: 1 na dorosłego, 0,5 na dziecko, czyli 9,5 na start;
- woda: 1 na osobę;
- woda: pompa po naprawie daje 12 dziennie, beczki 4, Studnia 20;
- ogrzewanie: szałas 1 drewna na noc jesienią i wiosną, 3 zimą; Barak (dom dla 8 osób) 1 jesienią, 2 zimą.

**Latarnia:** w dzień ta sama moc zużywa 25% paliwa. Paliwo na dobę w tabeli liczone jest dla jesieni (14 godzin dnia, 10 godzin nocy); zimą noc ma 14 godzin.

| Moc | Promień (kratki po 2 m) | Paliwo na godzinę nocy | Paliwo na dobę jesienią | Paliwo na dobę zimą |
| --- | --- | --- | --- | --- |
| 1 (tląca się) | 7 (14 m) | 0,5 | 6,75 | 8,25 |
| 2 | 10 (20 m) | 1 | 13,5 | 16,5 |
| 3 | 13 (26 m) | 2 | 27 | 33 |
| 4 (Przypływ) | 16 (32 m) | 3,5 | 47,25 | 57,75 |

- Każdy kolejny poziom mocy podwaja koszt, a promień rośnie tylko o 3 kratki. Więcej światła to też wyższy poziom zagrożenia w nocy, bo mutanty widzą osadę z daleka.
- Soczewki I dają +20% promienia przy tym samym zużyciu.
- **Brak paliwa:** latarnia gaśnie i Mgła wchodzi do osady.
  - Każda godzina ciemności to +5 Odmienności dla wszystkich i morale −20 za noc.
  - Po 2 godzinach budynki zaczynają zarastać.
  - Atak w tej nocy jest pewny.

**Praca (na osobę na zmianę):**

| Praca | Daje | Uwagi |
| --- | --- | --- |
| Drwal | 12 drewna (zimą 8, wiosną 10) | pracuje przy granicy Mgły, +1 Odmienności dziennie |
| Smolarz | 2 paliwa z 3 drewna, najwyżej 6 paliwa na osobę | wymaga Smolarni, do 2 smolarzy na Smolarnię |
| Zbieracz | 2,5 żywności, spada do 1 do 7. dnia | okolica się wyczerpuje, +2 Odmienności dziennie, zdarzenia |
| Grzybiarz | 3,5 żywności | pierwszy zbiór 4 dni po budowie piwnicy |
| Kucharz | +25% racji z całej żywności | jeden na 15 osób |
| Budowniczy | 10 roboczogodzin | budowa i rozbiórka |
| Wartownik | ochrona nocy | 2 osoby po pół zmiany, czyli razem jedna osoba mniej do pracy |
| Badacz | 2 punkty nauki za punkt Nauki |  |

**Budynki epoki Złomu (pierwsze dni):**

| Budynek | Drewno | Złom | Roboczogodziny | Co daje |
| --- | --- | --- | --- | --- |
| Szałas | 6 | 2 | 10 | dom dla 4 osób |
| Kuchnia | 8 | 4 | 12 | +25% racji |
| Smolarnia | 6 | 6 | 14 | paliwo z drewna, do 2 smolarzy |
| Piwnica grzybowa | 10 | 0 | 15 (w ruinie z piwnicą 6) | żywność bez wychodzenia w Mgłę, do 3 grzybiarzy |
| Beczki na deszczówkę | 0 | 3 | 3 | 4 wody dziennie, w deszcz 8, w suszę 0 |
| Wieża strażnicza | 10 | 4 | 12 | wcześniejsze wykrycie ataku |
| Palisada | 2 na kratkę | 0 | 1 na kratkę | wąskie gardła dla obrony |
| Studnia (po badaniu) | 4 (+6 gruzu) | 0 | 20 | 20 wody dziennie |

**Ruiny do rozbiórki:**

| Ruina | Daje | Roboczogodziny |
| --- | --- | --- |
| Wrak auta | 10 złomu, 2 paliwa | 6 |
| Stodoła | 25 drewna, 2 złomu | 20 |
| Dom w ruinie | 15 drewna, 10 złomu, gruz | 12 |

**Wyprawy w pierwszych dniach:**

- 3 osoby na jeden dzień.
- Łup:
  - paliwo 5–11;
  - racje 4–10;
  - złom 4–12.
- To samo miejsce za drugim razem daje połowę.
- Ryzyko:
  - 35% szans na rannego, który wypada z pracy na 3 dni;
  - 15% szans na ocalałego, czyli nowego człowieka i nową gębę do wyżywienia.
- W zasięgu jednego dnia drogi jest około 6 miejsc.

**Noce:**

| Noc | Atak | Tłuszcz po obronie |
| --- | --- | --- |
| 2. | szczury | 1 |
| 4. | 2 Ogary (pierwszy kryzys) | 6 |
| 7. | Szczurza Fala | 3 |
| 9. | 3 Ogary i Krzykacz | 8 |

Tłuszcz wytapia się w kuchni na paliwo: 2 tłuszczu dają 1 paliwa. Każdy poziom mocy latarni powyżej 1 dodaje do ataku jednego wroga.

**Morale (0–100, start 70):**

| Zdarzenie | Zmiana |
| --- | --- |
| spokojny dzień | +2 |
| racje 75% | −5 dziennie |
| racje 50% | −12 dziennie |
| brak racji | −15 dziennie |
| noc bez światła | −20 |
| zimna noc bez drewna | −8 |
| śmierć mieszkańca | −10 |

**Wynik symulacji `ekonomia_sim2.py` (500 gier na styl, dni 1–36):**

| Styl | Zgony | Gry bez zgonów | Ciemność | Ludzi 36. dnia | Morale | Przypływ bez strat | Zapasy 36. dnia |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Dobry | około 1 | 36% | 0% | 18 | 53 | 98% | 530 |
| Dobry z Opieką | 0,7 | 50% | 0% | 19 | 69 | 99% | 462 |
| Dobry z Selekcją | 1,2 | 27% | 0% | 18 | 50 | 99% | 672 |
| Średni | 2,9 | 28% | 17% | 15 | 70 | 76% | 338 |
| Zły | 7,4 | 0% | 100% | 2 | 0 | 0% | 33 |

- **Dobry gracz:**
  - stawia Smolarnię i Kuchnię pierwszego dnia, naprawia pompę i rozbiera wraki;
  - przydziela ludzi tam, gdzie zapasu starczy na najkrócej;
  - od 6. dnia gromadzi drewno i paliwo na zimę, zamienia szałasy na Baraki i racjonuje przy niskich zapasach;
  - od 24. dnia odkłada około 55 paliwa na noc Przypływu.

  Traci średnio 1 osobę, zgodnie z celem 1–2 zgonów w pierwszej zimie (decyzja z 8 listopada).
- **Średni gracz** robi wszystko 2 dni później, nie racjonuje i ma mniejszy zapas na zimę. Traci 3 osoby, a co czwarty Przypływ kosztuje go straty. Ma wyższe morale, bo nie racjonuje, ale płaci za to ludźmi.
- **Zły gracz** pali latarnię na mocy 2 dla miejsca, stawia Smolarnię późno i nie robi wypraw. Ciemność, ucieczki i upadek osady.
- **Oś Opieka ↔ Selekcja:** Opieka ratuje ludzi i morale kosztem zapasów, Selekcja daje o około 27% więcej zapasów kosztem zgonów. Żadna nie wygrywa.

**Najważniejsze decyzje początku:**
- kiedy postawić Smolarnię;
- czy racjonować;
- kogo wysłać na wyprawę;
- ile drewna i paliwa odłożyć na zimę;
- czy rozjaśnić latarnię, żeby mieć miejsce na budowę.

**Uwagi do dalszego balansu:**
- Woda nie jest wąskim gardłem, gdy pompa jest naprawiona. Studnia ma sens dopiero przy większej osadzie albo gdy pompa się zepsuje (wydarzenie).
- Zimą osada potrzebuje prawie wszystkich rąk. Barak, Sidła i zapasy z jesieni decydują o przetrwaniu.

## Mapa świata i wyprawy

Świat to sieć lokacji połączonych drogami, składana proceduralnie z ręcznie zaprojektowanych modułów wokół stałych miejsc fabuły. Wyprawy przechodzą od węzła do węzła, a czas w osadzie płynie dalej. Nazwy i liczby są robocze.

**Budowa mapy:**

- **Węzły** to miejsca: ruiny, osady, gniazda, składy, anomalie. **Drogi** mają typ (szosa, tor, leśna ścieżka, kanał), czas przejścia w godzinach, gęstość Mgły i poziom zagrożenia.
- **Gęstość Mgły** w każdym węźle (0–5) rośnie w stronę Serca Mgły i decyduje, jak szybko rośnie Odmienność. Latarnie pomocnicze obniżają ją wokół siebie, a Przypływ podnosi i zalewa część dróg.
- **Mgła wojny:** nieodkryte węzły są zakryte. Zwiad je odsłania, ale informacje się starzeją, bo Mgła i frakcje też się ruszają.

**Dwie warstwy mapy:**

- **Region:** duża mapa kampanii z około 150–250 węzłami, odkrywana stopniowo, z przewijaniem i przybliżaniem. Na niej toczą się wyprawy, karawany, ruchy frakcji i Przypływy.
- **Lokacja:** wejście do węzła otwiera mapę taktyczną złożoną z modułów, na której toczy się eksploracja albo walka.

**Ruch na mapie:**

- **Tempo marszu:**
  - zwykły;
  - forsowny: szybciej, ale zmęczenie i hałas;
  - ukradkowy: wolniej, rzadsze spotkania;
  - nocny: tylko z lampami albo Oczami Mgły.
- **Obóz:** postój z wartami. Ognisko grzeje, ale przyciąga mutanty. Saperzy mogą postawić tymczasowe umocnienia, a latarnicy tymczasowe światło.
- **Podział i łączenie:** skład można rozdzielić (np. zwiadowcy przodem) i połączyć w węźle. Kilka składów idzie naraz, a pod oficerem jako kolumna.
- **Środki transportu:**
  - wóz konny lub psi: więcej ładunku, tylko po drogach;
  - drezyna i wagon po torach: szybko, ale tory kontrolują Ludzie Toru;
  - łódź i tratwa: kanałami i rozlewiskami;
  - od epoki Silnika pojazdy.
- **Przeprawy:**
  - mosty, czasem zniszczone, do naprawy;
  - brody;
  - promy;
  - zablokowane drogi i objazdy.
- **Pogoda i Przypływ** zamykają drogi i zmieniają czas przejścia.
- **Rozkazy na trasie:** planowane postoje i automatyczny powrót po przekroczeniu progów (amunicja, ranni, Odmienność).
- **Ślady:** mutanty i frakcje też chodzą po mapie i zostawiają tropy, które zwiad potrafi odczytać.

**Generowanie z modułów:**

- **Stałe miejsca** w każdej kampanii: twoja Latarnia, Serce Mgły, siedziby frakcji i kluczowe miejsca fabuły (np. węzłowa stacja kolejowa, elektrownia, Latarnia Zakonu).
- **Moduły** to ręcznie zaprojektowane fragmenty świata z tagami: osiedle bloków, PGR, most, kościół z cmentarzem, stacja benzynowa, las z bunkrem, zakład przemysłowy, schron. Generator składa z nich regiony według reguł: strefa, gęstość Mgły, epoka znalezisk, frakcja kontrolująca.
- **Mapy taktyczne walk** powstają z tych samych modułów. Dzięki temu kluczowy teren (osłony, wąskie gardła, lejki śmierci) można przygotować w module z wyprzedzeniem, a AI dowódcy zawsze ma czytelny teren.

**Strefy świata:**

| Strefa | Teren walki | Typowi wrogowie | Łup |
| --- | --- | --- | --- |
| Miasto | bloki, ulice, skrzyżowania, klatki schodowe | Dawny Żołnierz, Naśladowca, Szabrownicy | broń, plany, leki |
| Przedmieścia i działki | płoty, altany, ogrody | Szczurza Fala, Padlinniki | żywność, narzędzia, nasiona |
| Wieś i PGR | pola, obory, silosy | Rogacz, Odmieniec, Łowcy Głów | saletra, skóra, zwierzęta |
| Las | drzewa, wąwozy, leśne bunkry | Ogar, Pajęczarz, Bezoki | drewno, zioła, skóra |
| Bagna i rozlewiska | grzęzawisko, groble, wraki łodzi | Pijawiec, Nosiciel, Topielec | tkanka, torf, rzadkie zioła |
| Tory i stacje | perony, wagony, nastawnie | Ludzie Toru, Śpiący | części, paliwo, metal |
| Przemysł | hale, kominy, zbiorniki | Rozrost, Zrośnięci, Zakon Latarników | części, paliwo, chemikalia |
| Podziemia | piwnice, schrony, kanały | Kopacz, Matka Roju, Krzykacz | przedwojenne plany, wiedza zaginiona |
| Serce Mgły | anomalie, zarośnięty świat | Wielki Pasterz, Chór Mgły | próbki, finał kampanii |

**Rodzaje wypraw:**

| Wyprawa | Cel |
| --- | --- |
| Poszukiwanie | złom, paliwo, plany, książki, sprzęt do inżynierii wstecznej |
| Zwiad | odkrywanie węzłów, wpisy do Bestiariusza |
| Ratunek | ocaleni, porwani przez Opiekunkę lub Łowców Głów |
| Eskorta karawany | handel z inną osadą |
| Poselstwo | dyplomacja, roznoszenie sztandaru |
| Polowanie | mutanty na chitynę, tkankę, tłuszcz |
| Oczyszczanie | wypalenie Zarośli, zniszczenie gniazda |
| Budowa posterunku | postawienie latarni pomocniczej, nowe terytorium |
| Rajd | atak na frakcję lub osadę |

**Przebieg wyprawy:**

1. Wybór celu, trasy i składu, rozkazy przed wyprawą.
2. Zapasy w ekwipunku i u tragarzy: racje, paliwo do lamp, amunicja, leki.
3. Podróż od węzła do węzła, która trwa godziny lub dni. Na drogach pojawiają się zdarzenia z wyborem.
4. W lokacji: eksploracja albo walka na mapie taktycznej.
5. Powrót z łupem, rannymi i nowymi wpisami w Bestiariuszu.

Przykładowe zdarzenia na drodze: ranny nieznajomy, który woła o pomoc (człowiek albo Naśladowca); zamknięty bunkier; kolumna Pielgrzymów; karawana Łowców Głów z jeńcami; droga zalana przez Mgłę, więc objazd albo marsz w gęstej Mgle.

**Karawany i handel:** karawana to tragarze, towary i eskorta. Po zawarciu umowy handlowej szlak działa regularnie, a jego walki rozstrzygają się w tle. Ryzyko napadu zależy od drogi, frakcji w okolicy i siły eskorty.

**Dyplomacja:**

- **Relacja** z każdą osadą i frakcją od −100 do 100: wrogość, napięcie, neutralność, przyjaźń, sojusz. Na relację wpływa też zgodność ustrojów, czyli odległość między kursami na trzech osiach.
- **Działania:** umowa handlowa, pakt o nieagresji, sojusz (np. wspólna obrona przed Przypływem), żądanie trybutu, ultimatum, wojna, wymiana jeńców, poselstwo.
- **Wpływ sztandarów** narasta przez karawany i poselstwa, aż osada przejdzie pod twój ustrój, zostanie wasalem albo wybuchnie w niej rewolucja.
- **Frakcje mają własne cele** i działają na mapie niezależnie od gracza: Synowie Czystości chcą zniszczyć Chór Mgły, Łowcy Głów porywają ludzi, Zakon Latarników dąży do monopolu na paliwo. Między sobą też toczą wojny.

## Interfejs i technologia

Głównym ekranem jest osada, a interfejs jest diegetyczny: teczki, kartoteki, maszynopisy i mapy na papierze z epoki. Gra powstaje jako tytuł desktopowy w Godot 4 z C#, z symulacją oddzieloną od silnika.

**Ekrany:**

| Ekran | Jak wygląda | Co się tam robi |
| --- | --- | --- |
| Osada (główny) | widok z góry z kręgiem światła, kartka zaopatrzenia | budowa, praca, czas; klik w budynek otwiera jego panel (rozbudowa, ludzie, produkcja), w tym skrzydła Kompleksu szkoleniowego i Akademię dowódców |
| Ludzie | teczka akt z portretem, mutacje na czerwono, rubryki „?” ukrytych cech | karty postaci, przy dużej skali rejestr z filtrami |
| Zbrojownia | stół z przedmiotami, sylwetka wojownika, arkusz Wzorów | wyposażanie, Wzory, stan magazynu |
| Mapa świata | papierowa mapa na stole, pinezki, pionki składów, listy i depesze przypięte do osad | wyprawy, trasy, karawany i cała dyplomacja (umowy, żądania, ultimata) |
| Ratusz | dekrety z pieczęcią, wskaźnik kursu na trzech osiach | prawa, ustrój, Legitymacja |
| Zeszyty | biurko z trzema zeszytami: Badania, Bestiariusz, Kronika | drzewko technologii, wiedza o wrogach, historia kampanii i poległych |
| Walka i raport | widok taktyczny z dymkami, po walce maszynopis meldunku | oglądanie, tempo, flara; potem oś czasu, oceny i powtórka |

**Zasady interfejsu:**

- **Panele i tabele:** na początku karty postaci i przeciąganie, przy dużej skali tabele, rejestry i filtry (np. stan Wzorów).
- **Czytelność ponad klimat:** kluczowe liczby zawsze czytelną czcionką, podpowiedzi jako przypięte karteczki, skróty klawiszowe do wszystkiego.
- **Aktywna pauza:** podczas pauzy można wydawać polecenia budowy i pracy.
- **Alarmy:** dzwon i kartka na tablicy przy Przypływie, ataku, buncie, powrocie wyprawy.

**Technologia:**

- **Silnik:** Godot 4 z C#. Dobry system interfejsu, render 3D wystarczający do niskiej rozdzielczości z shaderem pixel art, wszystkie pliki tekstowe (łatwe do pisania i edycji kodu), wydajność C# wystarczająca na setki jednostek i AI. Eksport na Windows i Linux.
- **Symulacja oddzielona od silnika:** cała logika gry (walka, AI, ekonomia, polityka) w czystym C#, deterministyczna, ze stałym krokiem czasu. Godot tylko rysuje i obsługuje interfejs.
- **Dzięki temu:** walki rozstrzygane w tle używają tej samej symulacji, a automatyczne testy balansu uruchamiają tysiące walk bez rysowania, np. dowódca 2 kontra 4 w tej samej sytuacji.
- **Dane gry w plikach:** broń, mutanty, prawa, technologie i moduły map w plikach danych, które łatwo edytować bez kodu.
- **Wydajność:** AI dowódców w osobnych wątkach, cykle rozłożone na klatki.
- **Zapis gry:** serializacja stanu symulacji.

Rozważany był też Rust z Bevy: bardzo wydajny, ale ze słabym jeszcze interfejsem i często zmieniającym się API, co przy tak dużej grze i diegetycznym UI jest zbyt ryzykowne.

## Oprawa graficzna

**3D low-poly renderowane na żywo jako pixel art.** Sekcja przepisana 8 listopada: dawny opis 2D (sprite'y 32×32, kafle, oświetlenie 2D) jest nieaktualny. Technika jest opisana w `PIPELINE_GRAFIKI.md` i `TECHNIKA_AI_I_MECHANIKI.md` (rozdział o grafice w Godocie).

**Render:**

- Kamera ortograficzna po przekątnej: nachylenie około 52°, **płynny obrót pod dowolnym kątem** (kamera przyciągana do siatki tekseli, żeby pixel art nie migotał), przybliżanie w kilku krokach. Postacie obracają się płynnie w dowolnym kierunku.
- Scena renderowana do **640×360** i powiększana ×3 do 1080p bez wygładzania (nearest).
- Shader materiału daje pasma cel-shadingu i cienie przesunięte w chłodny fiolet.
- Postproces daje kontur 1 px z bufora głębi (jaśniejszy po stronie światła), kwantyzację do palety przez LUT i dithering tylko w Mgle.

**Paleta:**

- około 40–45 kolorów w rampach po 4 tony na materiał;
- ciepła w świetle (żółć, ochra, rdza), zimna w Mgle (fiolet, sina zieleń, szarość);
- czerwień zarezerwowana dla krwi, mutacji i alarmów;
- kolory kości i jasnych beży przesunięte w róż, bo pod niebieskim konturem robią się zielonkawe.

**Skala:** człowiek ma około 1,75 m, latarnia około 8 m (około 4,5 wzrostu człowieka), drzwi mają wysokość postaci, a mutanty są w skali ludzi. Kratka osady ma 2 m.

**Postacie:**

- Szkielet z kośćmi, części ciała jako zwężane, organiczne bryły.
- Warstwy ubrań odsunięte o 1–3 cm, a czasze na barkach, łokciach i kolanach zakrywają stawy, więc nic nie przenika. Odsłonięte są tylko głowa i dłonie.
- Pancerz i hełm to sztywne modele przypięte do kości. Broń trzymają dłonie przez IK.
- Mutacje to osobne części: trzecie ramię, chityna, świecące oczy.
- Ubiór i barwy pokazują ustrój i frakcję.
- **Wydajność:** przy eksporcie części postaci trzeba scalać w jedną siatkę z atlasem materiałów, a roje rysować instancjami. Dziś postać ma około 50 osobnych siatek, co przy setkach jednostek jest za dużo.

**Mutanty:** czytelne sylwetki o różnych kształtach. Szczurza Fala to rojowisko, Ogar jest niski i wydłużony, Krzykacz wysoki i chudy, a organy Rozrostu świecą jako cele.

**Światło i Mgła (najważniejsze):**

- Latarnia, ogniska, okna, latarki i błyski wystrzałów to prawdziwe źródła światła z cieniami. Postacie dostają dodatkowe światło wypełniające i kontur od tyłu dla czytelności.
- Mgła to animowany efekt objętościowy albo shader na szumie, z gęstością z danych mapy, który cofa się przed światłem.
- Przypływ to fala Mgły napływająca na osadę, a zarośnięte budynki pulsują tkanką.
- Dym i ogień to cząsteczki w Godocie w docelowej jakości.

**Czytelność walki:** delikatne obwódki pod jednostkami w kolorze składu, ikonki ról, dymki rozkazów pikselową czcionką, nakładki sektorów i przydziałów w powtórce, wyraźny znacznik rannych i nieprzytomnych.

**Interfejs diegetyczny:**

- Papier z fakturą, plamami i zagięciami, czcionki maszynowe i odręczne na wolnych licencjach, pieczątki.
- Portrety w aktach to osobne, dokładniejsze głowy 3D renderowane w sepii jak zdjęcie legitymacyjne.
- Mapa świata jako papier z liniami dróg, pinezkami i przypiętymi notatkami.
- Rysunki w Bestiariuszu jako szkice w stylu notatek przyrodnika.

**Czego unikamy:** dużych ilustracji, filmowych przerywników i malowanych portretów. Ich rolę przejmują tekst, papier i światło.

### Animacje postaci (specyfikacja v1)

Animacje działają na tym samym szkielecie co modele. Pozy są zapisane jako obroty kości, a ręce trzyma broń przez IK. Klatki kluczowe mają krzywe: wolny start i koniec, szybkie uderzenie, zatrzymanie na trafieniu. Tempo to 12 kl/s, czyli pikselowy „krok” w stylu klasycznych gier.

| Akcja | Długość | Kluczowe fazy |
| --- | --- | --- |
| Chód | 12 kl., pętla | Kontakt i przejście, podskok bioder, ręce przeciwnie do nóg, kontrrotacja miednicy i klatki |
| Bieg (z bronią) | 8 kl., pętla | Faza lotu, pochylenie 12°, głębokie ugięcie przy kontakcie, broń nisko |
| Strzał | 24 kl. | Gotowość nisko → złożenie → odrzut (unosi się lufa, cofa klatka, błysk 1 kl.) → powrót w 3 kl. |
| Przeładowanie | 24 kl. | Przechył broni, lewa dłoń: magazynek → ładownica → nowy → wciśnięcie i klepnięcie |
| Osłona | 24 kl. | Kucnięcie za workami → wychylenie → 2 strzały → schowanie |
| Dzida z tarczą | 24 kl. | Garda → cofnięcie (antycypacja) → pchnięcie z wypadem w 2 kl. → wolny powrót |
| Młot | 24 kl. | Zamach za głowę → uderzenie z przyspieszeniem → zatrzymanie → powrót |
| Granat | 24 kl. | Zamach z obrotem tułowia → rzut z przeniesieniem ciężaru → osobny lot po paraboli → wybuch |
| Trafienie i śmierć | 24 kl. | Odrzut → ugięcie kolan → upadek z przyspieszeniem → odbicie → bezruch |
| Patrol | 12 kl., pętla | Chód z bronią w gotowości nisko |

Zasady: antycypacja przed każdym ciosem, przesada ruchu o 20–30%, nic nie jest liniowe, a odrzut przyjmuje głównie górna część tułowia. Czytelność zapewniają kontur światła od tyłu, oświetlenie wypełniające tylko na postaciach i nasycone kolory ubrań.

**Dodane w nocy z 5 na 6 listopada:**

- **Przejścia i reakcje:** płynne mieszanie akcji (chód → bieg → stop i strzał), skulenie pod ostrzałem → pad → czołganie, strzał z kolana, zachwianie po trafieniu bez śmierci, bieg do osłony. Prędkość przesuwu miesza się razem z pozą, więc stopy się nie ślizgają.
- **Broń ręczna:** rewolwer z odrzutem, nóż z wypadem, łuk z prawdziwie napinaną cięciwą i lecącą strzałą, sztandar w marszu, wartownik rozglądający się.
- **Życie osady:** kowal przy kowadle (iskry), noszenie skrzyni, kopanie łopatą, mieszanie w kotle, kłótnia dwóch osób, siedzenie przy ognisku.
- **Mutanty:** Ogar (galop obrotowy, skok z ugryzieniem i szarpaniem, śmierć z przewrotką), Krzykacz (powłóczysty chód, krzyk z falą dźwięku, cięcie, śmierć), Nosiciel (chód kaczkowaty, nabrzmienie i wybuch gazu), rój szczurów.
- **Efekty:** łuski, wyrzucany magazynek, dym z lufy, smugi pocisków, iskry i drzazgi przy ciosach, odłamki i chmura po granacie.

Do zrobienia: wynoszenie rannego, animacje bestiariusza poza trzema głównymi mutantami.

**Druga runda (6 listopada, dzień):**

- **Lepsza jakość klipów:**
  - strzał z kolbą przy barku i głową przy przyrządach, z mocniejszym odrzutem i szerszą postawą;
  - przeładowanie z lufą w górę, wyrzuceniem pustego magazynka na ziemię i klepnięciem;
  - wypad przy pchnięciu dzidą i krok w cios młotem;
  - granat z obrotem tułowia i przykucnięciem po rzucie;
  - śmierć z krokiem w tył, przy której broń wypada z rąk;
  - chód z wyraźniejszym ciężarem ciała.
- **Nowe klipy:**
  - cięcie bronią jednoręczną (nóż, rura, maczeta) z gardą;
  - strzał z kolana, czołganie, strzał z ziemi i skok do pozycji leżącej;
  - skręt w biegu z przechyłem i wspinaczka po drabinie (do sprawdzenia w silniku);
  - rąbanie drewna, kopanie, noszenie skrzyni, kucie, siedzenie i rozmowa.
- **Arsenał w rękach:** 14 rodzajów broni od Złomu do Światła, każdy z własnym chwytem i pozą:
  - Złom: rura z gwoździami, maczeta, nóż;
  - Kuźnia: dzida z tarczą, łuk, kusza, młot;
  - Czarny proch: muszkiet, obrzyn;
  - Nabój: rewolwer, karabin;
  - Silnik: pistolet maszynowy, karabin maszynowy;
  - Światło: lanca latarniowa (propozycja).
- **Sceny pokazowe:**
  - osada nocą (latarnia, budynki w kręgu światła, praca, patrol, ognisko, Mgła za krawędzią światła);
  - przejścia i reakcje;
  - mutanty w akcji;
  - bitwa przy bramie.
- **Eksport do Godota:** 10 postaci z pełnymi klipami, do tego 39 budynków, 13 broni, 15 mutantów i 18 jednostek frakcji. Instrukcja dla silnika jest w export/README.md (zdarzenia błysku i magazynka, prędkości przesuwu, mieszanie przejść).

**Trzecia runda (6 listopada, popołudnie):**

- **Znaki 11 frakcji:** pikselowe emblematy na sztandarach chorążych, tarczach, naszywkach i tunikach. Hełmy i tarcze są w barwach frakcji.
- **Jednostki frakcji:** 82 jednostki (po 6–8 na frakcję, zawsze z chorążym), każda w swoim ubiorze i ze swoją bronią, zgodnie z tabelą w sekcji Wrogowie.
- **Zwierzęta:** Ogar jest teraz zwykłym psem z lekkimi objawami (wychudzony, wyłysiałe placki, wrzody, białe oczy, odsłonięte dziąsła). Wataha, Ryjec i Rogacz stoją na tym samym szkielecie czworonoga, więc działają u nich wszystkie animacje Ogara.
- **Stworzenia Mgły:** Zwiewnik, Nici, Kłębowisko, Oddech, Gasiciel i Siewca, z mocnymi objawami (tkanka, gruczoły, zarodniki).
- **Ludzie zmienieni przez Mgłę:** Biegacz, Wydrążony, Fałszywy Latarnik i Kokon.
- **Eksport do Godota:** wszystko powyższe jest w export/frakcje i export/wrogowie.

### Modele (stan na 6 listopada)

| Zestaw | Co jest | Uwagi |
| --- | --- | --- |
| Budynki osady | Wszystkie 39 budynków z listy w wersji z wczesnych epok (Złom i Kuźnia): Światło, Mieszkanie, Żywność i woda, Produkcja, Wojsko, Społeczeństwo, Nauka, Handel | Zestaw budowlany: deski, blacha falista, cegła, kamień, beton, plandeka i około 25 rekwizytów. Budynki późniejszych epok to osobne modele składane z zestawu części post-apo (decyzja z 6 listopada). |
| Broń | 13 sztuk: nóż, dzida, włócznia z tarczą, łuk, młot, kusza, muszkiet, obrzyn, rewolwer, karabin, pistolet maszynowy, karabin maszynowy, miotacz ognia | Każda sztuka ma chwyt w punkcie 0, więc pasuje do dłoni, stojaka i ekwipunku. |
| Frakcje | Po 3 jednostki dla 6 frakcji: Szabrownicy, Synowie Czystości, Chór Mgły, Ludzie Toru, Łowcy Głów, Zakon Latarników | Dodatki do postaci: kaptury, szaty, peleryny, maski (czaszka, żelazna, chusta), rogi, plecaki, butle, latarnia na tyczce, łańcuchy, kolce na barkach. |
| Bestiariusz | 12 nowych mutantów: Odmieniec, Naśladowca, Śpiący, Zrośnięci, Dawny Żołnierz, Opiekunka, Pielgrzym, Echo, Pasterz, Bezoki, Ćma, Kopacz | Na razie jedna poza każdego. Animacje mają Ogar, Krzykacz, Nosiciel i szczury. |

**Ustalone 6 listopada:**

- Broń epoki Światła: lanca latarniowa zostaje.
- Barwy i znaki frakcji są docelowe (tabela w sekcji Wrogowie).
- Ogar to zwykły pies lekko zmieniony przez Mgłę. Mocne objawy infekcji mają tylko Stworzenia Mgły, a zwierzęta i ludzie są zmienieni słabiej.
- Budynki późniejszych epok to osobne modele z własnymi animacjami i efektami, żeby rozwój było widać.
- Dym i ogień będą cząsteczkami w Godocie, ale w docelowej jakości: kłęby dymu z cieniowaniem i ogień z językami, żarem i iskrami.
- Wojownicy dzielą się na klasy w kompleksie szkoleniowym (osobna sekcja). Przeciwników przybywa we wszystkich grupach.

**Nadal czeka:** prototyp w Godocie, do którego potrzebny jest podłączony folder na Twoim komputerze.

**Uwagi do makiet interfejsu (6 listopada, do wykonania):** makiety są tu: https://claude.ai/artifact/Uvf3BjLQNRKzXmKaCA9R5F (też w folderze projektu).

- **Styl:** więcej stylizacji i detalu w całym interfejsie: faktura papieru, zszywki, plamy, pismo odręczne, tasiemki, przybory na biurku. Liczby zostają czytelną czcionką.
- **Układ:** okienka nie mogą na siebie nachodzić. Na ekranie Osady zakładki ekranów zasłaniają pasek dnia i czasu. Trzeba stałego układu stref ekranu, który pilnuje, żeby panele nie wchodziły na siebie.
- **Osada:** klik w budynek na mapie osady otwiera jego panel, gdzie się rozbudowuje, przydziela ludzi i ustawia produkcję.
- **Kompleks szkoleniowy:** to budynki w osadzie. Panel szkoły pokazuje drzewko klas, opis każdej klasy i specjalizacji, listę kursantów pasujących do kursu (reszta wyszarzona z powodem) i koszt przeszkolenia.
- **Ludzie:** zakładka bardzo dobra, zostaje. Trzeba dodać:
  - przy każdej broni informację, czy to sprzęt klasy, pokrewny czy obcy;
  - drugą kwalifikację z jej sprzętem;
  - ukryte cechy jako puste rubryki „?” do odkrycia.
- **Mapa:** znacznie większa, z przewijaniem i przybliżaniem. Więcej węzłów i dróg oraz więcej możliwości ruchu: kilka składów naraz, karawany, obejścia, rzeki i przeprawy.
- **Ratusz:** dobry, ale bardziej stylizowany. Trzeba dodać dekret „Rozstrzeliwanie na miejscu” dla komisarzy.

**Budynki: kierunek graficzny (uwaga z 6 listopada):** mniej prostych domków z prostokąta i trójkątnego dachu, bo mają być naprawdę post-apo. Wczesne epoki to sklecone z wraków, kontenerów, blachy, opon i plandek, z własną sylwetką. Z kolejnymi epokami budynki zbliżają się do klasycznych, np. ceglane domy, ale zawsze trochę zrujnowane i niedokończone, żeby klimat post-apo został. Każda epoka ma osobny model z animacjami i efektami.

**Budynki z zestawu części:** każdy budynek to obrys, szkielet funkcji (piec, komin, wieża, brama) i części epoki. Budynki późniejszych epok dalej są osobnymi modelami, ale składa się je z tego samego zestawu, więc styl zostaje spójny.

- **Złom:**
  - kontenery, blacha falista, palety, opony, plandeki, beczki;
  - wraki aut i autobusów, rury, siatka, deski z rozbiórki.
- **Kuźnia:** kute okucia, ciosane belki, łaty z blachy nitowanej.
- **Czarny proch i Para:** cegła z odzysku, mur na zaprawie, rury i kotły, dachówka łatana blachą.
- **Nabój i Silnik:** beton, stal, szkło zbrojone, kable, worki z piaskiem.
- **Światło:** reflektory, soczewki, przewody, elementy z chityny.
- **Zawsze:** łaty, gruz, sadza, ślady Mgły i zarastania, nigdy czysty nowy budynek.

**Ustalone 6 listopada wieczorem:**

- Jedna wspólna lista umiejętności dla cywilów i wojowników.
- Sprzęt każdej szkoły zostaje osobny w każdej z 7 epok.
- Komisarz zależny od ustroju.
- Około 7 ekranów zamiast 12.
- Mapa w dwóch warstwach z wieloma sposobami ruchu.
- Budynki z zestawu części post-apo.
- Poprawione sprzeczności: widok 3D jako pixel art, gra na komputer, Akademia jako skrzydło, jeden Plac ćwiczeń, stopień I, pistolet skałkowy.

## Skala i decyzje projektowe

Na początku gracz patrzy na ludzi, pod koniec na system. Ekran zarządzania zmienia się razem ze skalą.

| Etap | Ludzie | Zarządzanie | Walka |
| --- | --- | --- | --- |
| Osada | 10–25 | każdy wojownik osobno, broń z imieniem, ręczne ubieranie | 1–2 składy |
| Miasteczko | 40–100 | Wzory, kilka składów, produkcja na zamówienie | kilka składów, karawany |
| Mikropaństwo | 150–400 | tabele Wzorów i ich stanu, fabryki, kwatermistrzowie | kolumny 2–4 składów pod oficerem |

**Przyjęte decyzje:**

| Temat | Decyzja |
| --- | --- |
| Rdzeń | zarządzanie osadą + autobitwy składów |
| Oprawa | 3D low-poly renderowane jako pixel art, kamera po przekątnej z przybliżaniem, dowolny obrót postaci, render na żywo w Godot |
| Interfejs | diegetyczny; osada jako ekran główny; panele i tabele |
| Silnik | Godot 4 z C#, symulacja w czystym C# oddzielona od silnika, desktop |
| Czas | rzeczywisty z pauzą, zatrzymany podczas oglądania walki |
| Kampania | czysty sandbox bez wątku głównego, około 80 godzin (7 epok, każda około roku gry, bez limitów czasu) |
| Lore | Mgła jako skutek eksperymentu, przedwojenna sieć latarni; ton: ludzki dramat |
| Świat | fikcyjna Europa Środkowa, dziwny post-apo z mutantami |
| Mapa świata | sieć lokacji; stałe miejsca fabuły + proceduralnie z modułów |
| Dyplomacja | średnia: relacje, umowy, sojusze, ultimata, wojny |
| Budowa osady | swobodna siatka w promieniu światła |
| Ekonomia | średnia, około 15 surowców, krótkie łańcuchy |
| Składy | 3–8 osób, później kolumny pod oficerem |
| Ustroje | sześć od początku, system hybrydowy (drzewo praw + osie) |
| Prawa | można uchylać, ale drogo; część nieodwracalna |
| Technologie | pięć gałęzi, siedem epok, progi z zależnościami między gałęziami |
| Badania | punkty nauki + surowce + znaleziska i inżynieria wsteczna |
| Walki | trwają tyle, ile potrzeba; realistyczna śmiertelność |
| Bratobójczy ogień | pełny |
| Dowódca | poziom ogólny + trzy specjalizacje |
| Sztandar | przedmiot, nie rola |
| Ekwipunek | pas i plecak połączone w jeden slot |
| Standaryzacja | bez premii do statystyk |

## Do rozpisania i plan MVP

**Tematy otwarte:**

- [ ] Osada i ekonomia: pierwsze 10 dni policzone i sprawdzone symulacją, do policzenia dalsze dni, zima i budynki kolejnych epok
- [ ] Drzewo praw: zarys z liczbami gotowy, do testów balansu
- [ ] Drzewko technologii: zarys siedmiu epok gotowy, do ustalenia czasy badań
- [ ] Mapa świata i wyprawy: zarys gotowy, do rozpisania katalog modułów i zdarzeń
- [ ] Lore i kampania: zarys gotowy, pula wydarzeń rozpisana, do rozpisania postacie z historiami
- [ ] Interfejs: zarys ekranów gotowy, makiety 7 ekranów zrobione, do poprawek według uwag z 6 listopada

**Pierwszy grywalny prototyp** ma sprawdzić, czy oglądanie walk z dowódcą wciąga, bo na tym stoi cała gra:

1. Mała osada z latarnią i paliwem, około 10 ludzi.
2. Epoka Złomu i początek Kuźni: 6 broni, 3 dodatki, sloty, ręczne ubieranie.
3. Cztery mutanty: Szczurza Fala, Ogar, Krzykacz, Nosiciel.
4. Dowódcy poziomów 1–3 z przydziałem, specjalizacjami, dymkami i kilkoma formacjami (klin, rząd, mur tarcz, jeż).
5. Raport po walce.
6. Jeden typ wyprawy i nocna obrona.

Polityka, pełne technologie, frakcje i standaryzacja wchodzą dopiero wtedy, gdy ten rdzeń działa.

**Produkcja:** grafikę i kod robi Claude, kod powstaje w folderze na komputerze użytkownika, a pierwszy grywalny kawałek to sama walka: skład z dowódcą kontra mutanty na jednej mapie, z dymkami rozkazów i raportem. Przed startem prototypu dopracowujemy jeszcze koncept.

**Źródła do formacji i drili:**

- [Fire team and squad formations, movement techniques – GlobalSecurity](https://www.globalsecurity.org/military/library/policy/army/accp/in0202/le2c.htm)
- [Infantry Formations – Joint Task Force Fatal](https://joint-task-force-fatal.readthedocs.io/en/latest/infantry/formations.html)
- [Military Operations in Urban Terrain – UNITAF](https://unitedtaskforce.net/handbook/group/107/military-operations-in-urban-terrain-mout)
- [Team Movement Techniques – ShackTac TTP2](https://wikiwiki.jp/arma2/ShackTac%20ArmA2%20TTP2%20Guide/Tactics/Team%20Movement%20Techniques)
- [FM 7-8 Battle Drills – GlobalSecurity](https://www.globalsecurity.org/military/library/policy/army/fm/7-8/ch4.htm)
- [US Army battle drills – Task & Purpose](https://taskandpurpose.com/military-life/us-army-battle-drills/)
- [FM 3-06.11 Urban Combat Skills – GlobalSecurity](https://www.globalsecurity.org/military/library/policy/army/fm/3-06-11/ch3.htm)
- [FM 21-75 Appendix D – GlobalSecurity](https://www.globalsecurity.org/military/library/policy/army/fm/21-75/Appd.htm)
- [Shield wall – Wikipedia](https://www.Wikipedia.org/wiki/Shieldwall)
- [Acies triplex – Imperium Romanum](https://imperiumromanum.pl/en/roman-army/military-formations-of-ancient-romans/acies-triplex/amp/)
- [Pike & Shot Tactics – BCW Project](https://bcw-project.org.uk/military/tactics)
- [Volley fire in Europe in the mid-16th century – CEEOL](https://ceeol.com/search/article-detail?id=1019627)
