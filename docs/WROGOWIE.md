# Wrogowie: bestiariusz w liczbach, scalone duble, nowi wrogowie

Uzupełnienie sekcji „Wrogowie” z konceptu. Zmiany względem konceptu są oznaczone **[zatwierdzone 8.11]** i czekają na zgodę użytkownika (pytanie 120 w `PYTANIA.md`). Liczby są robocze.

**Jak czytać liczby:**

- **Prędkość** w m/s. Dla porównania człowiek idzie z prędkością 1,5, a biegnie 4.
- **Pancerz** w tej samej skali co przebicie broni (0–6).
- **Wytrzymałość** u ludzi to rany według systemu stref. U zwierząt i stworzeń to liczba trafień, które ich wyłączają, przy przebiciu równym albo większym od pancerza. Słabsze trafienia liczą się proporcjonalnie (przebicie o 1 mniejsze daje 0,8 trafienia).
- **Siła zagrożenia** to wartość, z której AI dowódcy liczy zadania. Wojownik w stopniu II ma 10.

---

## 1. Zwierzęta zmienione przez Mgłę (lekkie objawy)

| Wróg | Epoka | Wielkość | Prędkość | Pancerz | Trafienia | Atak | Zmysły | Siła | Łup | Uwagi |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Szczurza Fala | Złom | rój 20–60 | 3 | 0 | 1 na szczura | ugryzienia: lekkie rany, choroby | węch, światło | 1 na szczura | brak | zalewa szyk, przechodzi przez szpary |
| Ogar | Złom | pies | 7 | 0 | 1–2 | ugryzienie: ciężka rana nóg, przewraca | węch, słuch | 8 | skóra, tłuszcz | **[zatwierdzone 8.11]** bez chityny, bo to zwykły pies; obchodzi i atakuje strzelców |
| Wataha | Kuźnia | 4–8 wilków | 8 | 0 | 2 | jak Ogar, taktyka okrążania | węch, wycie | 9 na wilka | skóra | dwa wilki odciągają, reszta zachodzi od tyłu |
| Ryjec | Kuźnia | dzik | 6 | 1 (kark) | 3 | szarża na nogi, przewraca | węch | 14 | mięso, tłuszcz, skóra | ryje pod palisadą |
| Rogacz | Czarny proch | jeleń albo tur | 9 (szarża) | 1 | 4 | szarża rogami, przewraca tarczowników | wzrok | 20 | mięso, rogi, skóra | jeż i wbite dzidy zatrzymują |
| Padlinniki | Złom | stado 10–30 ptaków | 6 (lot) | 0 | 1 | dziobanie rannych, kradzież łupu | wzrok | 0,5 na ptaka | pióra | latają nad osłonami |
| Ćma Latarniowa | Kuźnia | rozpiętość 1,5 m | 5 (lot) | 0 | 1 | wybuch zarodników: Odmienność +5 w promieniu 3 m | światło | 6 | pył zarodników | leci na najjaśniejsze światło |
| Pajęczarz | Kuźnia | wielkość psa | 4 | 1 | 2 | sieć unieruchamia na 4 s, jad osłabia | drgania | 9 | jedwab, jad | zasadzki w ruinach |
| **Gawra [nowy]** | Kuźnia | niedźwiedź | 6 | 2 | 6 | cios łapą: ciężkie rany, rozbija tarcze | węch | 35 | futro, tłuszcz, dużo mięsa | mini-boss lasu, broni gawry; Dzieci Lasu go czczą |
| **Wodnik [nowy]** | Czarny proch | sum 3 m | 2 (na lądzie), 6 (w wodzie) | 1 | 5 | wciąga pod wodę z brzegu i grobli | drgania wody | 25 | tłuszcz, mięso | w Rozlewiskach i przy przeprawach |
| **Szerszenie [nowy]** | Złom | gniazdo, rój 30 | 6 (lot) | 0 | gniazdo 3 | użądlenia: panika, opuchlizna | wibracje | 10 za gniazdo | wosk, jad | gniazda w ruinach na wyprawach; dym i ogień je płoszą |

## 2. Stworzenia Mgły (najsilniejsze objawy, bez pierwowzoru)

| Wróg | Epoka | Wielkość | Prędkość | Pancerz | Trafienia | Atak | Zmysły | Siła | Łup | Uwagi |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Bezoki | Kuźnia | człowiek, bez oczu | 5 | 2 (chityna) | 2 | szpony: ciężkie rany | tylko słuch | 12 | chityna, tkanka | strzały z broni palnej go ściągają, cisza go myli |
| Kopacz | Para | robak 4 m | 3 pod ziemią | 2 | 4 | wynurzenie pod szykiem: przewraca, rani | drgania | 22 | chityna, tkanka | ciężkie pancerze go przyciągają |
| Pijawiec | Czarny proch | 30 cm | 3 | 0 | 1 | przyczepia się i wysysa: krwawienie, Odmienność +1 na minutę | ciepło | 3 | tkanka | zostaje na ciele po walce, trzeba go wypalić |
| Zwiewnik | Nabój | kształt z mgły | 4 | brak (nie da się trafić w ciemności) | 2 w świetle | dotyk: zimno, panika, przechodzi przez tarcze | ruch | 15 | brak | widoczny i ranny tylko w świetle |
| Oddech | Silnik | chmura 6 m | 1 (z wiatrem) | nie dotyczy | rozwiewa go wiatr i ogień | duszenie: ignoruje pancerz | — | 18 | brak | **[zatwierdzone 8.11]** ruchoma anomalia; maska chroni |
| Gasiciel | Silnik | wielkość psa, płaski | 6 | 1 | 2 | gasi światła, rozbija lampy | światło | 10 | gruczoł | najpierw celuje w latarników |
| Kłębowisko → Rozrost | Para | Kłębowisko 2 m, Rozrost 6 m | 2 / 1,5 | 2 / 3 | Kłębowisko 6; Rozrost: 3–5 organów po 4 trafienia | taranuje, wchłania trupy i rośnie | węch trupów | 25 / 80 | tkanka, chityna, gruczoły | **[zatwierdzone 8.11]** jeden wróg w dwóch stadiach: Kłębowisko, które zje 10 trupów albo przeżyje Przypływ, staje się Rozrostem |
| Siewca → Matka Roju | Para / Światło | wieża 4 m / 8 m | 0 | 3 / 4 | rdzeń 8 / 20 | zarodniki co 10 s, rodzi Bezokich (Matka: fale co 30 s) | — | 30 / 120 | rdzeń, tkanka | **[zatwierdzone 8.11]** Siewca wyrasta na Zarośli; ten, który przetrwa Przypływ, staje się Matką Roju (boss) |
| **Tkacz [nowy]** | Nabój | pajęczy kształt z tkanki 2 m | 3 | 2 | 4 | tka ściany i mosty z tkanki: zamyka przejścia, odcina skład | drgania | 16 | jedwab tkanki | zmienia teren walki, ogień pali jego ściany |
| **Gruczolak [nowy]** | Światło | worek 1,5 m | 0 | 0 | 2 | wybucha płonącym paliwem przy trafieniu | — | 5 | gruczoł paliwowy (dużo paliwa) | źródło surowca: trzeba go zebrać nożem, nie strzałem |

## 3. Ludzie zmienieni przez Mgłę

**[zatwierdzone 8.11] Linia przemiany:** Kokon → Biegacz → Odmieniec → Wydrążony. To kolejne stadia tego samego człowieka. Śpiący to Odmieńcy w uśpieniu w gęstej Mgle, którzy budzą się na hałas i światło.

| Wróg | Epoka | Prędkość | Pancerz | Wytrzymałość | Atak | Siła | Łup | Uwagi |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Kokon | Czarny proch | 0 | 1 | 3 | brak | 0 | — | wybór: uratować (felczer i ogień w ciągu 2 minut) albo zabić |
| Biegacz | Czarny proch | 6 | 0 | jak człowiek, ale ignoruje ból nóg | grupy po 4–8, szpony | 8 | tkanka | świeżo przemieniony |
| Odmieniec | Nabój | 2,5 | 1–2 (tkanka) | tylko głowa albo kręgosłup | rozrywa pancerz i ubiór | 15 | tkanka, sprzęt | czasem ktoś z twojej osady (z imieniem) |
| Wydrążony | Nabój | 3 | pancerz poległego +1 | 4 trafienia w tkankę pod pancerzem | walczy bronią, którą ma na sobie | 14 | twój sprzęt | często nosi rzeczy poległych z osady |
| Krzykacz | Złom | 2 | 0 | jak człowiek | krzyk co 12 s: Odmienność +3 i panika w promieniu 15 m | 10 | tkanka | pierwszy cel strzelca |
| Nosiciel | Złom | 2 | 0 | 1 | wybuch przy śmierci: zarodniki 4 m, Odmienność +8 | 6 | — | zabić z dystansu |
| Naśladowca | Nabój | 4 | 0 | jak człowiek | wabi głosem rannych | 12 | — | Fałszywy Latarnik to jego odmiana, która wabi światłem **[zatwierdzone 8.11]** |
| Zrośnięci | Silnik | 3 | 1 | rozpadają się na 2–3 części po 3 trafienia | wiele kończyn | 24 | tkanka | broń obszarowa i łamacz |
| Dawny Żołnierz | Silnik | 4 | 3 (stary hełm i kamizelka) | jak człowiek | strzela, kryje się, rzuca granaty | 18 | broń, amunicja | używa tego samego instynktu co twoi wojownicy |
| Opiekunka | Czarny proch | 5 | 1 | 6 | porywa rannego w Mgłę | 12 | — | po porwaniu można wysłać wyprawę ratunkową (Kokon) |
| Echo | Kuźnia | — | — | nie da się zabić, gaśnie w świetle | morale −10 w promieniu, tylko w ciemności | 0 | — | widmo poległego z imieniem |
| Pasterz | Nabój | 3 | 2 | 6 | dowodzi mutantami w promieniu 40 m | 30 | rdzeń Pasterza | po jego śmierci rój się rozpierzcha |
| Pielgrzymi | Para | 1 | — | jak człowiek | neutralni | 0 | — | idą ku Sercu Mgły; można iść za nimi do ukrytych miejsc |

## 4. Anomalie (teren, nie wrogowie)

**[zatwierdzone 8.11]** Nici przechodzą z wrogów do anomalii.

| Anomalia | Działanie |
| --- | --- |
| Cisza | rozkazy i meldunki nie działają, pomagają Sygnały ręczne |
| Pętla | zakrzywiona przestrzeń: trasy prowadzą w kółko |
| Czerwony Deszcz | niszczy ubiór |
| Martwe Światło | gasną latarki i lampy |
| Rozlewisko | ciężki pancerz wciąga na dno |
| Zarośl | nieruchoma tkanka rodząca mutanty; na niej rośnie Siewca |
| Nici | macki z mgły wciągają ludzi stojących poza kręgiem światła |
| Oddech | ruchoma, dusząca chmura (opis w tabeli Stworzeń) |

## 5. Bossowie

| Boss | Gdzie | Fazy |
| --- | --- | --- |
| Topielec | atakuje latarnię podczas Przypływu | 1. wynurza się z rozlewiska i rzuca mułem w światło; 2. wspina się na latarnię, trzeba trafiać w gruczoły na grzbiecie; 3. ranny cofa się i ciągnie za sobą Pijawce |
| Wędrujący Las | Zarośl idąca na osadę | 1. Zarośl rośnie i rodzi Bezokich; 2. pień-serce wychodzi na powierzchnię; 3. ogień i saperzy niszczą korzenie |
| Matka Roju | dojrzały Siewca | fale co 30 s, rdzeń odsłania się po każdej fali |
| Wielki Pasterz | kieruje regionem | walka z jego rojem na mapie regionu, potem w gnieździe |
| Prorok Chóru | siedziba Chóru Mgły | pieśń wzmacnia mutanty; trzeba przerwać pieśń, zabijając Śpiewaków |
| Wielki Inkwizytor | siedziba Synów Czystości | ciężka zbroja, płomieniarze, zapala osadę |
| Serce Mgły | finał | zależne od drogi zakończenia |

## 6. Mgła się uczy: licznik adaptacji [zmiana: dodane liczby]

Każda kategoria ma licznik 0–100, który rośnie od tego, jak gra gracz, i maleje o 5 na dzień, gdy gracz zmieni styl. Przy 60 zwiad zapowiada zmianę, przy 80 adaptacja wchodzi na 15 dni. Najwyżej 2 adaptacje naraz.

| Kategoria | Co podnosi licznik | Adaptacja |
| --- | --- | --- |
| Światło | latarki, reflektory, flary | więcej Ćm i Gasicieli |
| Pancerz | ciężkie pancerze w składach | więcej Kopaczy, Rozlewiska na trasach |
| Broń palna | dużo strzałów | więcej Bezokich i Śpiących |
| Ogień | miotacze, koktajle, Ognie zaporowe | Ogary i Biegacze odporni na ogień (−50% obrażeń od ognia) |
| Wręcz | mur tarcz, łamacze | Kłębowiska i Zrośnięci |
| Cisza | tłumiki, skradanie | Naśladowcy i Pajęczarze |

## 7. Jednostki frakcji: rola w walce

Wszystkie jednostki frakcji mają klasy i stopnie z tego samego systemu co gracz. Każda frakcja ma jeden kluczowy pomysł na walkę:

| Frakcja | Pomysł na walkę | Słabość |
| --- | --- | --- |
| Szabrownicy | zasadzki i ucieczka, koktajle zapalające | rozpadają się po śmierci Herszta |
| Synowie Czystości | ogień i fanatyzm, nie cofają się | wolni, mało strzelców |
| Chór Mgły | walczą razem z mutantami, Niosący Zarodnik wybuchają | słaby pancerz |
| Ludzie Toru | ogień z wagonów, szybki odwrót pociągiem | daleko od torów są słabi |
| Łowcy Głów | sieci i usypiacze: biorą jeńców zamiast zabijać | mała siła ognia |
| Zakon Latarników | wystandaryzowani, dowódcy 4–5, światło jako broń | mało ludzi, każda strata boli |
| Dezerterzy | znają słabe punkty osady i atakują tam | niskie morale |
| Bractwo Bunkra | broń z epoki Naboju wcześnie, pancerni | mało amunicji, nie wychodzą daleko |
| Gildia Solna | najemnicy za pieniądze, kusznicy | uciekają, gdy przestaje się opłacać |
| Dzieci Lasu | pułapki, łuki, walka w lesie | słabi na otwartym terenie |
| Republika Tamy | elektryczność, reflektory, dyscyplina | zależni od prądu z tamy |
