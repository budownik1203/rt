# Budynki: cykl epok i wygląd (gotowe prompty)

Katalog wszystkich budynków: od kiedy są, jak zmieniają się z epokami, co je zastępuje, oraz opis wyglądu każdej wersji gotowy jako prompt do zrobienia modelu. Koszty i liczby są w `EPOKI.md`. Ta sama tabela cyklu życia jest w zakładce „Budynki” dokumentu online.

## Zasady

- **Przebudowa:**
  - Budynek, który żyje przez wiele epok, można przebudować do wersji nowej epoki za około 60% ceny nowego. Ludzie i zapasy zostają na miejscu.
  - W czasie przebudowy budynek pracuje na pół gwizdka.
  - Nieprzebudowany działa dalej z dawnymi liczbami i wygląda staro.
- **Następca:** niektóre budynki mają następcę (Dymarka → Kuźnia → Odlewnia → Stalownia). Stary budynek zostaje do drobnych prac.
- **Utrata sensu:** budynek, który się już nie opłaca (np. Beczki po Pompowni), można rozebrać i odzyskać około 40% materiałów.
- **Ustrój w wygląd:** Ratusz, świątynia, pomnik i domy dostają nakładki ustroju (sztandary, barwy, napisy, naszywki, tkanka w Kościele Mgły) bez zmiany modelu.
- **Spoza zakładki Epoki:** w Złomie są też Magazyn, Złomowisko, Brama, Zbrojownia, Prosektorium i Kaplica, a w Silniku Blok (dom dla 30 osób).

## Wspólny początek promptu

Każdy prompt modelu zaczyna się od tego tekstu:

> Zrób model budynku do gry Latarnia w potoku opisanym w `docs/PIPELINE_GRAFIKI.md`, czyli low-poly w Blenderze (bpy), wzorując się na funkcjach `b_*` z `buildings.py` w paczce źródeł.
>
> - **Skala:** człowiek 1,75 m, latarnia 8 m. Wymiary podane niżej są w metrach.
> - **Render i postproces:** Cycles, `post.py`: pasma cel-shadingu, fiolet w cieniach, kontur 1 px, paleta około 45 kolorów. Kamera ortograficzna, nachylenie 50°, obrót −25°.
> - **Styl:** post-apo, złożony z zestawu części swojej epoki (lista niżej). Zawsze łaty, gruz, sadza, rdza i ślady Mgły (sinawe zacieki u podstawy). Nigdy czysty, nowy budynek i nigdy prosty domek z prostokąta z trójkątnym dachem.
> - **Funkcja:** budynek ma być czytelny z daleka po jednym znaku (komin, piec, maszt, szyld z symbolem).
> - **Noc:** ciepłe światło z okien i ognia, zimny błękit Mgły na krawędziach.
> - **Animacje i efekty** (dym, ogień, para, maszyny) jako osobne obiekty lub ścieżki NLA.
> - **Eksport:** `.glb` jak w `export_rest.py`, do `export/budynki/<nazwa>_<epoka>.glb`.
>
> Opis budynku: …

**Zestaw części epok (skrót):**

| Epoka | Części |
| --- | --- |
| Złom | kontenery, blacha falista, palety, opony, plandeki, beczki, wraki aut i autobusów, rury, siatka, deski z rozbiórki, taśma, liny |
| Kuźnia | kute okucia i zawiasy, ciosane belki, łaty z nitowanej blachy, pierwsza cegła, dachy z gontu i blachy |
| Czarny proch | cegła z odzysku, mur na zaprawie, otwory strzelnicze, dachówka łatana blachą, beczki prochu, słupy z lampami |
| Para | nitowane blachy, rury i kotły, koła zębate, kominy z cegły, szkło w żelaznych ramach, manometry |
| Nabój | beton pierwszy raz, stalowe belki, worki z piaskiem, drut kolczasty, druty telegrafu, maszty z flagami |
| Silnik | beton, prefabrykaty, stal, kable i słupy, reflektory, blacha falista na stalowych ramach, opony, pojazdy |
| Światło | soczewki i kryształy, pierścienie światła, chityna jako płyty, żywa tkanka (Przemiana) albo wypalona, biała powierzchnia (Wypalenie) |

---

## Światło

**Latarnia** (cała gra, 7 wersji)

| Epoka | Opis |
| --- | --- |
| Złom | Stara ceglana wieża 8 m na planie ośmioboku, osmalona, z ubytkami łatanymi blachą drogową. W szczycie za porysowanym szkłem beczka z płonącą smołą. Drabina z rur i lina z bloczkiem do wciągania paliwa. U podstawy stos beczek i kanistrów. Animacja: płomień i dym ze szczytu, słabe żółte światło. |
| Kuźnia | Wieża obwiązana kutymi obręczami, a w szczycie wieniec szlifowanych soczewek w kutej ramie (Soczewki I). Latarnia murowana dostaje nową, murowaną podstawę z łukowymi drzwiami. Animacja: obracający się wieniec soczewek. |
| Czarny proch | Galeria z blankami wokół szczytu, paraboliczne lustra z polerowanej blachy (Zwierciadła), wysięgniki z sygnałowymi lampami, prochownia pod schodami. Światło jaśniejsze i bardziej skupione. |
| Para | U stóp kocioł z nitowanej blachy i rura biegnąca po ścianie do szczytu. Lampa węglowo-gazowa w żeliwnej klatce, manometry, para z zaworu. Animacja: para, drgająca wskazówka, pulsujące światło. |
| Nabój | Na galerii stanowiska strzeleckie z workami z piaskiem, lampa sygnalizacji świetlnej (żaluzje) i maszty z flagami ustroju. Animacja: migająca sygnalizacja. |
| Silnik | Dolna część obudowana betonem, a do budynku prądnicy biegną kable. W szczycie elektryczny reflektor łukowy, wokół mniejsze reflektory. Animacja: snop obrotowy przez Mgłę. |
| Światło | Kryształowa Soczewka latarni morskiej, białe-błękitne światło, pierścienie światła unoszące się wokół szczytu. Trzy warianty drogi: Wypalenie (biała, wypolerowana wieża), Przemiana (wieża oplątana żywą tkanką, światło sinofioletowe), Sieć (wieża z promieniami łączącymi się z innymi światłami). |

**Lampa oliwna** (Złom): tyczka z rury 2,5 m z lampą oliwną w osłonie z puszki i daszkiem z blachy, przywiązana drutem. Mały krąg ciepłego światła. Animacja: migotanie.

**Ognie strażnicze** (Złom): kosze z siatki na słupach palisady z płonącym drewnem. Animacja: ogień i iskry.

**Lampa pomocnicza** (od Prochu)

| Epoka | Opis |
| --- | --- |
| Czarny proch | Ceglany słup 4 m z latarnią w żelaznej klatce, beczka paliwa u podstawy i daszek nad drzwiczkami. |
| Para | Gazowa latarnia z kloszem i rurą w ziemi. |
| Silnik | Elektryczna lampa na betonowym słupie z kablem. |
| Światło | Kryształ w pierścieniu (węzeł Sieci). |

**Posterunek świetlny** (od Prochu): mała ceglana wieżyczka 6 m z lampą, izba dla 2–3 ludzi z piecykiem, palisada wokół, wiata na zapasy. Od Naboju dochodzi maszt telegrafu, od Silnika antena radiowa i reflektor.

**Skład paliwa**

| Epoka | Opis |
| --- | --- |
| Złom | Ogrodzony siatką plac z beczkami pod plandeką, ziemny nasyp przeciwpożarowy, tablica „Nie palić” namalowana farbą. |
| Para | Węglarnia: drewniane zasieki z hałdami węgla, taczki, wózek na szynach. |
| Nabój | Dwie zardzewiałe cysterny kolejowe na podkładach, rury i zawory. |

---

## Mieszkania

**Szałas** (Złom, potem slumsy): 3×3 m, ściany z palet i blachy falistej, dach z plandeki przyciśniętej oponami, piecyk z beczki z rurą przez dach, drzwi z drzwi samochodu. Warianty: szałas na wraku busa i szałas przy kontenerze. Animacja: dym z rury.

**Barak** (Złom środkowa): 8×4 m, dwa kontenery postawione obok siebie i połączone daszkiem z blachy, ocieplone matami z trzciny i workami, dwa kominki z rur, ganek z palet, pranie na sznurku.

**Dom murowany** (Kuźnia): 6×5 m, parter z cegły z odzysku w różnych kolorach, piętro z belek i desek, dach z blachy i gontu łatany, ceglany komin, okiennice z kutymi zawiasami. Ściany nierówne, cegła wypełnia ubytki starego budynku.

**Kamienica** (Para): 10×8 m, trzy kondygnacje, przedwojenna kamienica odbudowana: ubytki zamurowane inną cegłą, nitowane balkony, rury spustowe, gazowa lampa nad bramą, pranie w podwórku.

**Koszary** (Nabój): 14×6 m, długi ceglany budynek z betonowym cokołem, rząd okien z kratami, maszt z flagą, plac apelowy, worki z piaskiem przy wejściu.

**Blok** (Silnik): 16×10 m, cztery piętra z wielkiej płyty z ubytkami łatanymi blachą, balkony obudowane deskami i plandeką, anteny, kable między blokami, światło w części okien.

---

## Obrona

**Palisada i mur**

| Epoka | Opis |
| --- | --- |
| Złom | Palisada z wbitych desek, drzwi samochodów, blach i znaków drogowych, łączona drutem, z podestem dla obrońców. Częstokół dodaje zaostrzone pale i kolce z prętów, a Rowy z palami dają rów z wbitymi palami przed ogrodzeniem. |
| Kuźnia | Mur z cegły z odzysku, nierówny, z blachą na koronie. |
| Czarny proch | Mur z blankami i otworami strzelniczymi, ceglane baszty co kilkanaście metrów. |
| Para | Mur obłożony nitowanymi płytami, wentylatory zaporowe w basztach (jeśli zbadane). |
| Nabój | Przed murem zasieki z drutu kolczastego na kozłach i okopy z workami. |
| Silnik | Betonowe bloki, zapory przeciw pojazdom, reflektory na basztach. |
| Światło | Pierścień kryształowych słupów świetlnych (Sieć) albo wypalona, biała strefa przed murem (Wypalenie). |

**Brama**

| Epoka | Opis |
| --- | --- |
| Złom | Burta ciężarówki zawieszona na łańcuchach na dwóch słupach z rur, z przeciwwagą z beczek z gruzem. |
| Kuźnia | Drewniana brama dwuskrzydłowa z kutymi okuciami i daszkiem. |
| Czarny proch | Brama w ceglanej bramie-baszcie z kratą. |
| Para | Przesuwna stalowa brama na kołach, napędzana kołem zębatym. |
| Nabój | Do tego budka wartownicza ze szlabanem, reflektor ręczny i worki. |
| Silnik | Pancerna brama z silnikiem elektrycznym i lampą ostrzegawczą. |
| Światło | Śluza świetlna: brama w łuku z soczewek. |

**Wieża strażnicza**

| Epoka | Opis |
| --- | --- |
| Złom | Rusztowanie 5 m z palet i rur, platforma z blachy, osłona z opon i drzwi, drabina, dzwon z butli gazowej. |
| Kuźnia | Wieża z ciosanych belek z daszkiem z gontu, dzwon. |
| Czarny proch | Ceglana wieża z otworami strzelniczymi. |
| Para | Wieża z nitowanych blach z dzwonem parowym. |
| Nabój | Stanowisko ciężkiego karabinu w workach i lampa bojowa. |
| Silnik | Wieża reflektora: betonowa wieża 7 m z dużym reflektorem i kablem. |
| Światło | Działo świetlne albo lanca na szczycie. |

**Bunkier** (Silnik): niski betonowy schron wkopany w ziemię, z wejściem po schodach, wywietrznikiem, peryskopem i drzwiami pancernymi.

---

## Żywność i woda

**Kuchnia i Stołówka**

| Epoka | Opis |
| --- | --- |
| Złom | Kuchnia: zadaszenie z blachy na słupach, palenisko z cegieł i felg, wielki gar z kotła, stół z drzwi, beczka na wodę. Animacja: para z garnka i dym. |
| Kuźnia | Murowane palenisko z kapą. |
| Para | Stołówka: hala z cegły z kotłami parowymi i długimi stołami, kolejka ludzi. |
| Silnik | Stołówka z kuchnią elektryczną i wentylatorami na dachu. |

**Piwnica grzybowa**

| Epoka | Opis |
| --- | --- |
| Złom | Wejście w ziemię obłożone oponami i blachą, zejście po schodach z palet, nad ziemią wywietrzniki z rur i kopiec ziemi z darnią. Wnętrze widoczne tylko w przekroju: półki z workami. |
| Kuźnia | Murowane wejście z łukiem i drewnianymi drzwiami. |
| Silnik | Wentylatory elektryczne i lampy. |

**Szklarnia** (Para): długa szklarnia z żelaznych ram i szkła, część szyb zastąpiona folią i blachą, kocioł grzewczy na końcu, zielone światło roślin w środku.

**Farma w Mgle** (Światło): pola pod kopułami z kryształowych prętów. Wypalenie: białe, jałowe pola z jasnymi roślinami. Przemiana: pola żywej tkanki z owocującymi naroślami.

**Wyrąb**

| Epoka | Opis |
| --- | --- |
| Złom | Stanowisko na skraju światła: kozioł do piłowania, stos drewna, siekiery w pniaku, latarka na tyczce. |
| Silnik | Piła mechaniczna na stole i ciągnik do zrywki. |

**Suszarnia** (Złom): drewniane ruszty pod daszkiem z blachy, sznury z grzybami i mięsem, dymiące palenisko pod spodem.

**Wędzarnia** (Kuźnia): ceglana komora z kominem, drewniane drzwiczki, wieszaki z mięsem, gęsty dym.

**Konserwownia** (Nabój): hala z cegły z kotłem do pasteryzacji, prasa do puszek, stosy puszek, taśma.

**Pole przy latarni**

| Epoka | Opis |
| --- | --- |
| Kuźnia | Zagony ziemniaków, lnu i rzepaku ogrodzone płotkiem z palet, strach na wróble z lampą. |
| Silnik | Ciągnik i rowy melioracyjne. |

**Beczki na deszczówkę** (Złom): trzy niebieskie beczki pod blaszanym okapem z rynną z przeciętej rury.

**Studnia, Filtr wody, Pompownia**

| Epoka | Budynek | Opis |
| --- | --- | --- |
| Złom | Studnia | Krąg z opon i cegły, kołowrót z rury, wiadro, daszek z blachy. |
| Kuźnia | Filtr wody | Beczki z piaskiem i węglem drzewnym połączone rurkami, kranik. |
| Para | Pompownia z wieżą ciśnień | Ceglany budynek z pompą parową i nitowanym zbiornikiem na stalowej wieży, rury do domów. |

---

## Produkcja

**Złomowisko**

| Epoka | Opis |
| --- | --- |
| Złom | Ogrodzony plac z hałdami złomu posortowanego po rodzaju, waga z belką, dźwignia z liną. |
| Para | Prasa parowa do złomu. |

**Smolarnia** (Złom): retorta z beczki obłożonej gliną nad paleniskiem, rura skraplacza do wiadra, kopiec drewna. Animacja: gęsty czarny dym i kapiąca smoła.

**Mielerz** (Złom): kopiec drewna przykryty darnią i ziemią, dymiący z otworów, obok drągi i szufla.

**Warsztat złomiarza** (Złom): kontener z otwartą ścianą, stół z imadłem z koła samochodowego, kowadło z szyny, blachy, nożyce i młotki, tarcze w budowie. Animacja: iskry przy klepaniu.

**Dymarka, Kuźnia, Odlewnia, Stalownia** (linia metalu)

| Epoka | Budynek | Opis |
| --- | --- | --- |
| przełom Złomu | Dymarka | Gliniany piec szybowy 2 m z miechem z worka, glina popękana, żar w otworze. |
| Kuźnia | Kuźnia | Ceglane palenisko z okapem pod wiatą z belek, kowadło, miech na drągu, beczka z wodą, narzędzia na ścianie. Animacja: żar i iskry. |
| Czarny proch | Odlewnia | Ceglana hala z piecem kopulakowym, formy w piasku, kadź z płynnym żeliwem, wysoki komin. |
| Nabój | Stalownia | Wielka hala z pieca martenowskiego, stalowe belki, dźwig, łuna przez okna. |

**Garbarnia** (Kuźnia): wiata z kadziami z garbnikiem, skóry rozpięte na ramach, smród zaznaczony brunatnym dymem. Od Pary dochodzi bęben napędzany pasem.

**Szwalnia** (Kuźnia): dom z belek z dużymi oknami, krosna, bele lnu, w środku stół krawiecki. Od Pary maszyny do szycia napędzane wałem.

**Topielnia albo Olejarnia** (Kuźnia)

| Wariant | Opis |
| --- | --- |
| Topielnia | Kotły nad ogniem, hak z tuszą, beczki tłuszczu, sinawa para. |
| Olejarnia | Prasa śrubowa z belek, worki z ziarnem, beczki oleju. |

**Cegielnia** (Kuźnia): suszarnia cegieł pod daszkiem, piec polowy z cegły, glinianka obok, stosy cegieł.

**Wytwórnia prochu i Prochownia** (Czarny proch)

| Budynek | Opis |
| --- | --- |
| Wytwórnia prochu | Ceglany budynek z grubymi ścianami, daleko od domów, z lekkim dachem, żeby wybuch szedł w górę, i z kołem młyńskim. |
| Prochownia | Niska ziemianka z ceglanym sklepieniem i nasypem, drzwi obite blachą, tablica z czaszką. |

**Saletrzarnia albo Kopalnia saletry** (Czarny proch)

| Wariant | Opis |
| --- | --- |
| Saletrzarnia | Pryzmy obornika i ziemi pod daszkiem, kadzie do ługowania. |
| Kopalnia | Wejście do jaskini wzmocnione belkami, wózek na szynach i lampa. |

**Ludwisarnia** (Czarny proch): mała odlewnia z formami kul i dzwonem w budowie.

**Kotłownia** (Para): ceglany budynek z wysokim kominem, okno kotła z łuną, hałda węgla, rury i pasy napędowe wychodzące do sąsiednich budynków.

**Kotłownia miejska albo kotły w zakładach** (Para)

| Wariant | Opis |
| --- | --- |
| Kotłownia miejska | Duża kotłownia z rurami ciepłowniczymi biegnącymi nad ziemią do domów. |
| Kotły w zakładach | Małe kotły przy każdym warsztacie. |

**Warsztat maszynowy** (Para): hala z cegły i blachy z wałem napędowym pod sufitem, pasy do tokarek i wiertarek, suwnica, okna świetlikowe.

**Huta szkła** (Para): stożkowy ceglany piec, dmuchacze z piszczałkami, szyby oparte o ścianę, zielonkawa łuna.

**Gazownia** (Para): dwa okrągłe zbiorniki gazu z nitowanej blachy, retorty, rury do miasta.

**Rusznikarnia** (przełom Pary): hala z cegły ze strzelnicą z tyłu, prasy do łusek, skrzynki amunicji, stół z karabinami.

**Rafineria** (Nabój): kolumny destylacyjne z blachy, rury, cysterny, płonąca pochodnia gazowa.

**Betoniarnia** (Silnik): betoniarka bębnowa, silos z blachy, formy prefabrykatów, hałdy gruzu.

**Elektrownia** (Silnik): betonowy budynek z silnikami spalinowymi albo kotłem i turbiną, transformator za siatką, słupy i kable do miasta, buczenie i iskry. Animacja: migotanie przy przeciążeniu.

**Warsztat silnikowy z prądnicą** (przełom Naboju): garaż z kanałem, silnik na stojaku, prądnica pasowa, pierwsze żarówki.

**Garaż albo zajezdnia drezyn** (Silnik)

| Wariant | Opis |
| --- | --- |
| Garaż | Hangar z blachy falistej z ciężarówką na podnośniku i beczkami benzyny. |
| Zajezdnia | Hala na torach z drezyną pancerną i obrotnicą. |

**Kuźnia chityny** (Światło): hala z betonu z prasami, płyty chityny suszące się na stojakach, sinawy połysk.

**Gruczolarnia** (Światło): kadzie z hodowanymi gruczołami, które pulsują i świecą, rury z paliwem. W Wypaleniu gruczoły są w szklanych słojach, w Przemianie hodowla wrasta w ściany.

**Reaktor Rdzenia** (Światło): betonowy pierścień wokół rdzenia Mgły w kryształowej klatce, mgła sącząca się z rdzenia, kable.

---

## Wojsko i szkoły

**Plac ćwiczeń**

| Epoka | Opis |
| --- | --- |
| Złom | Ubite klepisko otoczone oponami, manekiny z worków na palach, tablica z kredą, tor z palet do przeskakiwania. |
| Kuźnia | Ogrodzenie z belek i stojaki na broń. |
| Czarny proch | Strzelnica z wałem ziemnym. |
| Nabój | Plac apelowy z masztem. |
| Silnik | Tor przeszkód z betonu i opon, głośniki. |

**Skrzydło szkoły:** budynek ze stylu epoki plus znak szkoły, czyli jeden wyróżnik przed wejściem.

| Szkoła | Wyróżnik |
| --- | --- |
| Walki wręcz | manekiny z opon i stojak z tarczami |
| Strzelecka | tarcze strzeleckie na wale |
| Ciężka broń | podest ze stanowiskiem broni i skrzynie |
| Szturmowa | atrapa drzwi i ścianki do wejść |
| Zwiadu | siatka maskująca i wieżyczka obserwacyjna |
| Saperska | doły, belki i drut do ćwiczeń |
| Sanitarna | nosze i biała opaska z czerwonym pasem na drzwiach (nie czerwony krzyż) |
| Łączności | maszt z chorągiewkami, potem anteną |
| Komisarzy | mównica z tubą i sztandar ustroju |
| Latarników | lampa na wysokim drągu i lustra |
| Tresury | kojce z psami i tor przeszkód |

**Akademia (skrzydło dowódców)**

| Epoka | Opis |
| --- | --- |
| Kuźnia | Dom z belek z dużą salą i piaskownicą-makietą terenu pod dachem. |
| Czarny proch | Ceglany budynek z mapami. |
| Para | Sala z tablicami i maszyną do liczenia. |
| Nabój | Budynek sztabowy z masztem i telegrafem. |
| Silnik | Betonowy sztab z radiostacją i stołem operacyjnym. |

**Zbrojownia**

| Epoka | Opis |
| --- | --- |
| Złom | Kontener z kłódką, stojaki na dzidy i tarcze przy ścianie, kartka z ewidencją na drzwiach. |
| Kuźnia | Murowana izba z drzwiami okutymi. |
| Para | Regały z oznaczonymi Wzorami i wózek. |
| Nabój | Arsenał, czyli ceglany magazyn z rampą i wartownikiem. |
| Silnik | Kwatermistrzostwo: betonowa hala z regałami i ciężarówką. |

**Strzelnica** (Nabój): wał ziemny, tarcze na szynach, stanowiska z workami i budka instruktora.

---

## Władza i społeczeństwo

**Ratusz**

| Epoka | Opis |
| --- | --- |
| Złom | Wrak autobusu z doklejonym blaszanym gankiem, tablica ogłoszeń z kartkami, dzwon z butli gazowej, ławka z palet. |
| Kuźnia | Dom z belek z wieżyczką i dzwonem. |
| Czarny proch | Ceglany budynek z balkonem do przemów i zegarem z odzysku. |
| Para | Tuba parowa na dachu. |
| Nabój | Maszty z flagami ustroju. |
| Silnik | Radiowęzeł i głośniki. |
| Światło | Soczewka nad wejściem. |

Nakładki ustroju na Ratusz:

| Ustrój | Nakładka |
| --- | --- |
| Junta | mundurowa straż i orzeł |
| Komuna | czerwone płótna i hasła |
| Rada | tablica wyników głosowania |
| Reżim Latarnika | portret Latarnika i naszywki |
| Kościół Mgły | tkanka i spirale |
| Wolne Miasto | szyldy i kantor |

**Sąd albo Trybunał** (Kuźnia): izba z belek z ławą sędziowską i aresztem w piwnicy z kratą. W Trybunale zamiast ławy jest jedno krzesło z wysokim oparciem. Od Naboju obok stoi Komisariat Straży: ceglany, z kratami i lampą.

**Szkoła** (Kuźnia): dom z belek z tablicą, ławkami z desek i dzwonkiem. W Parze ceglana szkoła z zegarem.

**Kaplica** (cała gra)

| Wersja | Opis |
| --- | --- |
| Złom | Kapliczka z rur i blachy z lampką wotywną i obrazkami. |
| Kult Latarni (Czystość) | Wieżyczka z lampą i białe płótna. |
| Kościół Mgły (Przemiana) | Przybytek oplątany tkanką, mgła w środku i fioletowe światło. |
| Neutralna, późniejsze epoki | Ceglany kościółek z dobudówkami. |

**Cmentarz albo Stos**

| Wariant | Opis |
| --- | --- |
| Cmentarz | Krzyże z rur i tabliczki z imionami, płotek, lampki. W Przypływie groby zarastają tkanką. |
| Stos | Kamienny krąg z popiołem i tablica z imionami. |

**Pomnik** (Para): postument z cegły z figurą z odlewu w stylu ustroju, np. Latarnik z lampą, robotnik, Wódz, spirala Mgły.

**Urząd ewidencji** (Para): ceglany budynek z szafami kartotek widocznymi w oknach i tablicą ludności.

**Drukarnia** (Czarny proch): izba z prasą drukarską, bele papieru, plakaty na ścianie.

**Radiostacja** (Silnik): betonowy budynek z wysokim masztem z odciągami, czerwona lampa na szczycie.

**Stacja telegrafu** (Nabój): budka z belek z aparatem i drutami na słupach.

**Ambasada** (u frakcji, Nabój): dom w stylu frakcji z twoim sztandarem i strażą.

---

## Zdrowie i nauka

**Zielarnia** (Złom): szałas z suszącymi się ziołami pod okapem, moździerz i słoiki.

**Izba chorych, Szpital, Lazaret**

| Epoka | Budynek | Opis |
| --- | --- | --- |
| Kuźnia | Izba chorych | Dom murowany z łóżkami z desek i białymi płachtami, beczka z wrzątkiem. |
| Czarny proch | Szpital | Ceglany pawilon z oknami, ogródek ziół. |
| Czarny proch | Lazaret polowy | Namioty z płótna i wozy sanitarne. |
| Silnik | Szpital z salą operacyjną | Betonowy, z lampą operacyjną w oknie i generatorem. |

**Kwarantanna albo Izba przemiany** (Kuźnia)

| Wariant | Opis |
| --- | --- |
| Kwarantanna | Ogrodzony siatką barak z wieżyczką strażnika i bramą z dwoma zamkami. |
| Izba przemiany | Barak z miękkim światłem, ludzie z mutacjami przy pracy, ozdoby z tkanki. |

**Apteka** (Czarny proch): ceglana izba z półkami słoików i alembikiem, szyld z moździerzem.

**Łaźnia** (Czarny proch): ceglany budynek z kominem i parą z okien, beczki z wodą.

**Pracownia, Prosektorium, Laboratorium**

| Epoka | Budynek | Opis |
| --- | --- | --- |
| Złom | Pracownia | Stół badacza w izbie latarni, książki z wypraw, lampa. |
| Złom | Prosektorium | Wiata z kamiennym stołem, haki, słoje z próbkami, rynna do krwi. |
| Kuźnia | Pracownia | Własny dom. |
| Silnik | Laboratorium | Betonowe, z mikroskopami. |
| Światło | Laboratorium Światła | Kopuła z kryształowymi soczewkami i pierścieniem światła. |

---

## Handel i transport

**Magazyn**

| Epoka | Opis |
| --- | --- |
| Złom | Kontenery w podkowę pod wspólną plandeką. |
| Kuźnia | Szopa z belek z okuciami. |
| Czarny proch | Ceglany spichlerz. |
| Para | Magazyn z dźwigiem parowym. |
| Nabój | Hala z rampą i wózkami. |
| Silnik | Hangar z blachy falistej na betonie. |

**Targ** (Kuźnia): plac z budami z palet i plandek w kolorach, waga, beczki soli. W Parze zadaszona hala targowa.

**Stacja karawan albo Twierdza** (Para)

| Wariant | Opis |
| --- | --- |
| Stacja karawan | Zajazd z wiatami dla wozów, poidłem i magazynem, latarnia sygnałowa. |
| Twierdza | Bastiony z cegły i blachy, fosa i bramy w zakolu. |

**Bocznica** (Nabój): tory z podkładami, rozjazd z dźwignią, rampa i wagon.

**Wieża sieci** (Światło): smukła wieża z kryształem na szczycie, promień światła łączący ją z sąsiednią wieżą.

**Budowla finału** (Światło, trzy drogi)

| Droga | Opis |
| --- | --- |
| Wypalenie | Wielki Płomień: gigantyczne palenisko-soczewka na wieży z betonu i chityny. |
| Przemiana | Komunia z Mgłą: żywa katedra z tkanki rosnąca z latarni. |
| Sieć | Pierścień Świateł: wieża sterująca z mapą regionu i promieniami do wszystkich świateł. |
