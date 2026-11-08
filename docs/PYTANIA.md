# Pytania do ustalenia

Lista pytań do przejścia z użytkownikiem. Przy każdym jest propozycja Claude (**P:**), więc wystarczy potwierdzić albo zmienić. ★ oznacza pytania pilne, bo od nich zależy prototyp. Nowy Claude powinien zadawać je w formularzu wielokrotnego wyboru, po 4 naraz, zaczynając od ★.

---

## 1. Rdzeń i pętla gry

1. ★ Czy gracz ma mieć plan przed walką (punkt zbiórki, kierunek zagrożenia, strefa do utrzymania)? **P:** tak, dla obrony i zaplanowanych ataków (ANALIZA B1).
2. ★ Ile interwencji ma gracz w walce? **P:** 1 flara w Złomie, 2 od Prochu, 3 od Silnika.
3. ★ Co się dzieje, gdy wybuchają dwie walki naraz? **P:** kolejka walk, każdą można obejrzeć albo rozstrzygnąć w tle.
4. Czy kamera w walce ma się sama przybliżać w kluczowych momentach? **P:** tak, z możliwością wyłączenia.
5. Czy walki w tle mają pokazywać skrócony „film” z najważniejszymi chwilami? **P:** tak, 20–40 s skrótu w raporcie.
6. Jak długo ma trwać typowa nocna obrona? **P:** 1–3 minuty przy tempie x1.
7. Czy noc bez ataku też ma swoje wydarzenia (np. odgłosy, zwiad Mgły)? **P:** tak, krótkie, bez walki.
8. Czy gra ma szybkie przewijanie dnia (x8, x16)? **P:** x1, x2, x4, x8; walka tylko do x4.
9. Czy kampania ma mieć zwycięstwo czasowe albo punktowe obok trzech zakończeń? **P:** nie, tylko zakończenia i porażki.
10. Czy po zakończeniu można grać dalej? **P:** tak, tryb „po końcu świata”.

## 2. Trudność i ustawienia kampanii

11. ★ Jakie poziomy trudności? **P:** Kronika, Latarnia, Mgła (ANALIZA B4).
12. Czy jest tryb żelazny (jeden zapis)? **P:** tak, opcjonalny.
13. Czy gracz wybiera ziarno i wielkość regionu? **P:** tak: mały (100 węzłów), średni (180), duży (250).
14. Czy można wybrać punkt startu (np. więcej ludzi, mniej paliwa)? **P:** 3 starty: Uciekinierzy (10 osób, mało zapasów), Kolejarze (8 osób, wagon z narzędziami, bliżej torów), Ocaleni z bunkra (12 osób, broń palna bez amunicji, wysoka Odmienność wokół).
15. Czy częstość Przypływów da się ustawić? **P:** tak, w ustawieniach kampanii.

## 3. Walka i mechaniki bojowe

16. ★ Jaki odsetek wyłączonych z walki to ranni do uratowania, a jaki zabici? **P:** 70 / 30.
17. ★ Czy pozycje w walce są na siatce, czy swobodne? **P:** siatka 0,5 m dla logiki, płynny ruch w widoku.
18. Czy amunicja liczy się co do sztuki? **P:** tak dla składu na wyprawie, w osadzie w paczkach po 20.
19. Czy broń się psuje i zacina? **P:** tak: zużycie i zacięcia rosną przy złej konserwacji i sprzęcie obcym.
20. Czy wojownicy podnoszą broń poległych? **P:** tak, sami, gdy skończy się amunicja albo stracą broń.
21. Czy jest celowanie w strefy (np. snajper w głowę)? **P:** tylko zdolność Snajpera.
22. Czy pogoda wpływa na walkę? **P:** tak: deszcz (gorsza widoczność, gaśnie ogień), śnieg (wolniejszy ruch), wiatr (rozwiewa Oddech i dym).
23. Czy noc i dzień różnią się w walce? **P:** tak: w nocy działa tylko światło i Oczy Mgły.
24. Czy pancerz się niszczy? **P:** tak, wymaga naprawy w Kuźni lub Warsztacie.
25. Czy jeńcy są w walce z ludźmi? **P:** tak: poddający się wrogowie, a decyzję o nich podejmuje ustrój.
26. Czy zwierzęta juczne i psy walczą? **P:** psy tak (Tresura), juczne nie, ale mogą spanikować.
27. Czy jest walka w budynkach (wnętrza)? **P:** tak, w modułach z piętrami jako osobne poziomy siatki.
28. Czy wysokość daje premię? **P:** tak: +celność i zasięg, lepsza widoczność.
29. Czy ogień rozprzestrzenia się po terenie? **P:** tak, po trawie i drewnie, gaśnie w deszczu.
30. Czy granaty mogą wrócić (odbicie, odrzucenie)? **P:** odbicie tak, odrzucenie nie.

## 4. AI dowódców i wojowników

31. ★ Czy architektura AI z `TECHNIKA_AI_I_MECHANIKI.md` (5 kroków, histereza, ocena użyteczności) jest akceptowana? **P:** tak.
32. Czy dowódca ma klasę i walczy? **P:** tak (ANALIZA B8).
33. Czy charakter dowódcy jest stały, czy zmienia się z przeżyciami? **P:** zmienia się powoli (np. po klęsce ostrożny staje się bardziej ostrożny).
34. Czy wojownicy mogą odmówić rozkazu? **P:** tak, przy niskim morale, złej relacji z dowódcą i cesze Odważny / Tchórzliwy.
35. Czy dymki rozkazów są po polsku z imionami? **P:** tak, z krótkimi frazami ze słownika per sytuacja.
36. Ile fraz na sytuację? **P:** 5–10 wariantów, zależnie od charakteru.
37. Czy dowódca zapamiętuje wrogów z poprzednich walk (osobiście, nie tylko Bestiariusz)? **P:** tak, premia za doświadczenie z danym typem.
38. Czy gracz widzi w trakcie walki, co myśli dowódca? **P:** tylko w powtórce, na żywo same dymki.
39. Czy rywalizacja dowódców jest mechaniką (zazdrość, awanse)? **P:** tak, lekka, przez relacje.
40. Czy wojownik może zostać dowódcą bez Akademii? **P:** tak, do poziomu 2.

## 5. Klasy, szkoły, ludzie

41. ★ Ile klas startowych w prototypie? **P:** 4 szkoły (wręcz, strzelecka, sanitarna, zwiadu), po 1–2 klasy.
42. Czy stopnie mają nazwy (szeregowy, kapral…)? **P:** tak, zależne od ustroju (Junta wojskowe, Komuna „towarzysz”, Rada bez stopni, tylko funkcje).
43. Ile ukrytych cech ma człowiek? **P:** 1–3 ukryte obok 2–3 jawnych.
44. Pełny katalog ukrytych cech: czy Claude ma go rozpisać według propozycji z `UZUPELNIENIA.md`? **P:** tak.
45. Czy dzieci mogą chodzić do szkół klas (np. tresura)? **P:** nie, dopiero od 16 lat w grze.
46. Czy jest starzenie się i śmierć ze starości? **P:** tak, powolne, kampania trwa około 7 lat.
47. Czy związki i dzieci są mechaniką? **P:** tak, w tle: pary, narodziny, żałoba.
48. Czy zmutowani mogą być dowódcami? **P:** tak, ale ustrój wpływa na posłuch.
49. Czy można mieć bohaterów unikalnych (postacie z historią)? **P:** tak, 10–15 w kampanii, z wydarzeniami osobistymi.
50. Jak wyglądają portrety w aktach? **P:** osobne dokładniejsze głowy 3D renderowane jako pixel art 64×64.

## 6. Osada, ekonomia, budynki

51. ★ Czy przydział pracy ma tabelę priorytetów jak w RimWorldzie? **P:** tak (ANALIZA B2).
52. ★ Czy transport surowców w osadzie jest fizyczny (tragarze), czy magazyn jest wspólny? **P:** wspólny magazyn w świetle, tragarze tylko do posterunków i karawan.
53. Czy budynki mogą się palić i zawalać? **P:** tak: ogień, Przypływ, zawalenie przy ataku Rozrostu.
54. Czy budynki mają poziomy w ramach epoki? **P:** nie, zamiast tego przebudowa do kolejnej epoki.
55. Czy przebudowa kosztuje 60% nowego? **P:** tak.
56. Czy ruiny w mieście są losowe? **P:** tak, z modułów, inne w każdej kampanii.
57. Czy jest system chorób i epidemii? **P:** tak: zimą i w ciasnych domach, z Kwarantanną i Szpitalem.
58. Czy jest system przestępczości w osadzie? **P:** lekki: kradzieże racji przy głodzie, bójki, kary z praw.
59. Czy jest pieniądz? **P:** nie w osadzie (handel wymienny); sól Gildii jako waluta handlowa od Kuźni.
60. Czy dzień ma zmiany pracy (dzienna i nocna)? **P:** od Pary możliwa nocna zmiana przy lampach gazowych.
61. Ile surowców docelowo? **P:** około 15 aktywnych naraz, łącznie około 40 w całej grze.
62. Czy gracz układa drogi w osadzie? **P:** nie, ścieżki powstają same od ruchu.
63. Czy wysokość terenu w osadzie ma znaczenie? **P:** tak, wzgórze to lepsze miejsce na wieże.

## 7. Polityka i ustroje

64. ★ Czy podgląd skutków prawa przed uchwaleniem wchodzi? **P:** tak (ANALIZA B7).
65. Czy wybory w Radzie są symulowane (kandydaci, głosowanie)? **P:** tak, co porę roku, kandydaci z frakcji wewnętrznych.
66. Czy w Juncie może dojść do puczu wojska? **P:** tak, gdy dowódcy mają niską relację z Wodzem.
67. Czy gracz ma postać Wodza / przywódcy? **P:** nie, gracz jest „wolą osady”, ale w ustrojach Wodza jest postać przywódcy z cechami, którą można stracić.
68. Czy jest religia jako system (wiara, kapłani)? **P:** tak, lekki: Kult Latarni (Czystość) i Kościół Mgły (Przemiana), Kaplica, potrzeba wiary.
69. Czy prawa mogą być nieodwracalne? **P:** tak, około 10 praw z † (już w koncepcie).
70. Czy frakcje wewnętrzne mogą odejść i założyć własną osadę? **P:** tak, jako Dezerterzy.
71. Czy ustrój można zmienić siłą (zamach gracza)? **P:** tak, wydarzenie z kosztem Legitymacji.
72. Czy komisarze mogą stanąć na czele opozycji? **P:** tak (już w koncepcie).
73. Czy jest propaganda jako zasób (punkty wpływu)? **P:** nie, przez budynki i prawa.

## 8. Frakcje: strategie i zachowanie ★

Dla każdej frakcji: cel strategiczny, jak rośnie, jak reaguje na gracza, w jakiej jest epoce i jak walczy. Poniżej szkic do potwierdzenia.

74. ★ Czy frakcje działają na mapie same, z własnymi planami (GOAP-lite)? **P:** tak.
75. ★ Czy frakcje rosną i giną bez udziału gracza? **P:** tak, ale wolno; frakcja może zniknąć albo wchłonąć inną.
76. Czy frakcje wysyłają własne Przypływowe prośby o pomoc? **P:** tak, sojusznicy proszą o pomoc w Przypływie.

**Szabrownicy**
77. Cel: łup, przetrwanie. Strategia: rajdy na karawany i wysunięte posterunki, unikanie walki z silnym. Epoka: Złom. **P:** potwierdzić.
78. Czy da się ich przekupić lub wcielić? **P:** tak, po śmierci Herszta rozpadają się, część przyłącza się do osady.
79. Czy wracają po klęsce? **P:** tak, co porę roku nowa banda z nowym Hersztem.

**Synowie Czystości**
80. Cel: wytępić Przemianę. Strategia: krucjaty na Chór Mgły i osady z mutantami, żądają wydania zmutowanych. Epoka: Kuźnia–Proch. **P:** potwierdzić.
81. Czy mogą być sojusznikiem gracza w Juncie? **P:** tak, z kosztem: żądają czystek.
82. Czy ich Inkwizytor jest bossem? **P:** tak, Wielki Inkwizytor.

**Chór Mgły**
83. Cel: przyspieszyć Przemianę świata. Strategia: misjonarze, nawracanie Przemienionych w twojej osadzie, ataki razem z mutantami w Przypływie. Epoka: Para. **P:** potwierdzić.
84. Czy Kościół Mgły gracza może się z nimi sprzymierzyć? **P:** tak, to naturalny sojusznik.
85. Prorok Chóru jako boss? **P:** tak.

**Ludzie Toru**
86. Cel: kontrola torów i handlu. Strategia: kursowanie pociągiem po pętli, handel, najemnicy, myto za przejazd. Epoka: Para. **P:** potwierdzić.
87. Czy pociąg jest celem do zdobycia? **P:** tak, ale bardzo trudnym, a bez nich tory pustoszeją.

**Łowcy Głów**
88. Cel: niewolnicy na sprzedaż. Strategia: porwania z karawan i obrzeży, targ ludźmi z Gildią. Epoka: Kuźnia. **P:** potwierdzić.
89. Czy gracz może od nich kupować ludzi? **P:** tak, za koszt Legitymacji (oprócz Wolnego Miasta).

**Zakon Latarników**
90. Cel: monopol na paliwo i światło. Strategia: przejmowanie latarni, pomoc w zamian za wasalstwo, technologia. Epoka: Silnik (zawsze o 2 epoki przed graczem). **P:** potwierdzić.
91. Czy Zakon zna prawdę o Latarniach (lore)? **P:** tak, częściowo; to droga do Teorii Światła.

**Dezerterzy**
92. Cel: zemsta lub własna osada. Strategia: nocne napady w słabe punkty, które znają. Epoka: twoja. **P:** potwierdzić.
93. Czy można ich przywrócić do osady? **P:** tak, wydarzeniem i amnestią.

**Bractwo Bunkra**
94. Cel: przetrwać, odzyskać przedwojenny porządek. Strategia: izolacja, obrona bunkra, wyprawy po sprzęt, rzadki handel. Epoka: Nabój. **P:** potwierdzić.
95. Czy Bractwo przyjmuje obcych? **P:** nie, tylko przez długą przyjaźń.

**Gildia Solna**
96. Cel: zysk i szlaki. Strategia: karawany, kontrakty, kredyt, najemnicy, wpływ ekonomiczny. Epoka: Proch. **P:** potwierdzić.
97. Czy Gildia może zadłużyć gracza? **P:** tak, długi spłacane towarem, niespłacone kończą się najazdem najemników.

**Dzieci Lasu**
98. Cel: żyć z Mgłą w lesie, chronić las. Strategia: obrona terytorium pułapkami, handel ziołami, wrogość do wyrębu. Epoka: Złom–Kuźnia. **P:** potwierdzić.
99. Czy wyrąb lasu psuje relacje? **P:** tak.

**Republika Tamy**
100. Cel: własna sieć światła, rywalizacja o ludzi. Strategia: przyciąganie przybyszów, handel prądem, wojna o ludzi i wpływ. Epoka: Silnik. **P:** potwierdzić.
101. Czy można połączyć sieci (Sieć latarni)? **P:** tak, to droga do zakończenia Sieć.

**Ogólnie o frakcjach**
102. Czy frakcje mają własne dowódcy z imionami i historią? **P:** tak, przywódca i 2–3 dowódców nazwanych.
103. Czy frakcje toczą wojny między sobą i gracz widzi to na mapie? **P:** tak.
104. Czy frakcje mogą przejść w inną epokę w trakcie gry? **P:** tak, wolniej niż gracz (oprócz Zakonu).
105. Ile frakcji w jednej kampanii? **P:** 6–8 z 11, losowanych przy starcie, plus stałe Zakon i Chór.

## 9. Mapa świata

106. ★ Czy mapa dwuwarstwowa (region i lokacje) jest dobra? **P:** tak (już w koncepcie).
107. Ile czasu zajmuje przejście jednej drogi? **P:** 2–8 godzin gry.
108. Czy składy na mapie widać na żywo (pionki się ruszają)? **P:** tak.
109. Czy można przechwycić karawanę frakcji? **P:** tak, jako rajd.
110. Czy na mapie są losowe miejsca jednorazowe (skrytki, wraki)? **P:** tak, 20–30 na region.
111. Czy jest system pogody na mapie? **P:** tak, fronty pogodowe przesuwają się po regionie.
112. Czy Przypływ widać na mapie jako falę? **P:** tak, zapowiedź na 1 dzień (losowo na początku wiosny, już ustalone).

## 10. Epoki i technologie

113. ★ Czy zawartość epok z `EPOKI.md` jest zaakceptowana? **P:** tak, z poprawkami użytkownika.
114. Tempo badań: ile punktów daje badacz w kolejnych epokach? **P:** Złom 2 × Nauka, każda epoka +25% (Pracownia, Laboratorium, Akademia nauk).
115. Czy gracz widzi całą planszę epoki od razu? **P:** tak, ale technologie z przyszłych faz są zamazane.
116. Czy wybory wykluczające są na zawsze? **P:** tak, w danej kampanii.
117. Czy porażka próby przełomu może zabić? **P:** tak, rzadko (10–20% szans na ofiary przy porażce).
118. Czy można cofnąć się epoką (np. utrata wiedzy)? **P:** nie.
119. Czy plany i książki z wypraw mogą być fałszywe lub błędne? **P:** tak, rzadko, wykrywa je Nauka 6+.

## 11. Wrogowie

120. ★ Czy łączymy dublujące się typy (ANALIZA C)? **P:** tak.
121. Czy wrogowie mają poziomy zagrożenia rosnące z czasem? **P:** tak, przez „Mgła się uczy” i Przypływy.
122. Czy bossowie mają własne fazy walki? **P:** tak, 2–3 fazy.
123. Czy mutanty polują same na mapie (wędrujące stada)? **P:** tak, widoczne po zwiadzie.
124. Ilu nowych wrogów dodać? **P:** propozycje w `WROGOWIE.md` (Gawra, Wodnik, Szerszenie, Tkacz, Gruczolak).

## 12. Broń i sprzęt

125. ★ Czy warianty broni i modyfikacje z `BRON.md` są dobre? **P:** tak.
126. Czy broń ma imiona i historię (stary karabin Zenka)? **P:** tak, broń, która zabiła wielu, zyskuje imię i premię do morale właściciela.
127. Czy jest system jakości wykonania? **P:** nie (decyzja z 7 listopada: brak jakości), zamiast tego warianty.
128. Czy amunicja ma typy (zwykła, zapalająca, przeciwpancerna)? **P:** tak, od Naboju.
129. Czy sprzęt da się przerabiać (np. obrzyn ze strzelby)? **P:** tak, w warsztacie.

## 13. Interfejs ★

130. ★ Czy stałe strefy ekranu z ANALIZY B9 są dobre? **P:** tak.
131. ★ Czy interfejs ma reagować na stan osady (brud, plamy tkanki, czerwone pieczątki)? **P:** tak.
132. Jakie animacje interfejsu wchodzą? **P:** wszystkie z listy w ANALIZIE B9.
133. Czy biurko ma przedmioty, które coś znaczą (lampa = paliwo, kubek = morale)? **P:** tak.
134. Czy jest pismo odręczne? **P:** tak, tylko w notatkach i podpisach; liczby zawsze drukiem.
135. Czy skróty klawiszowe do wszystkiego? **P:** tak.
136. Czy ekran Osady ma minimapę? **P:** nie, krąg światła jest mapą, z przybliżaniem.
137. Jak pokazać kurs osady na 3 osiach? **P:** trójkąt na kartce w Ratuszu z pinezką.
138. Czy powiadomienia to kartki przypinane do tablicy? **P:** tak.
139. Czy jest ekran statystyk kampanii? **P:** tak, w Kronice.
140. Brakujące makiety do zrobienia: Zbrojownia, Zeszyty (Badania, Bestiariusz, Kronika), panel budynku, dyplomacja na mapie, Akademia. **P:** zrobić w interfejsie v2.

## 14. Grafika

141. ★ Czy najpierw test shadera w Godocie, zanim powstaną nowe modele? **P:** tak.
142. Rozdzielczość renderu? **P:** 640×360 powiększane ×3 do 1920×1080.
143. Czy obrót kamery jest skokowy (4 kierunki) czy płynny? **P:** skokowy, bo pixel art lepiej wygląda w stałych kątach.
144. Czy budynki z zestawu części post-apo (zakładka Budynki) są dobre? **P:** tak.
145. Czy ludzie mają różne sylwetki (wzrost, tusza)? **P:** tak, 3 sylwetki i 2 wzrosty.
146. Czy kobiety i mężczyźni mają różne modele? **P:** tak, wspólny szkielet.
147. Czy zmutowani wyglądają inaczej w osadzie? **P:** tak, mutacje widoczne na modelu.

## 15. Dźwięk i muzyka (brak w koncepcie)

148. ★ Jaki styl muzyki? **P:** ambient, akordeon, smyczki i drony; w walce perkusja i niskie tony.
149. Czy postacie mówią (głosy)? **P:** krótkie okrzyki nagrane lub syntezowane po polsku, reszta tekstem.
150. Czy Mgła ma swój dźwięk? **P:** tak: szum, szepty, które nasilają się z Odmiennością.
151. Dźwięki interfejsu: maszyna do pisania, papier, pieczątka? **P:** tak.

## 16. Fabuła, wydarzenia, ton

152. Czy pełna pula wydarzeń (około 150) ma być rozpisana teraz? **P:** tak, po potwierdzeniu kategorii.
153. Treści drażliwe (praca dzieci, przetwarzanie zmarłych, egzekucje): jaki ton? **P:** poważny, bez epatowania, z ostrzeżeniem na starcie.
154. Czy jest narrator (głos Kroniki)? **P:** tak, tekstowo: kronikarz osady komentuje dni.
155. Czy jest Archiwum Zasłony jako zbieranie lore? **P:** tak, strzępy wiedzy z wypraw.

## 17. Technika i produkcja

156. ★ Identyfikatory w kodzie po angielsku, dane po polsku? **P:** tak.
157. ★ Zapis w JSON czy MessagePack? **P:** JSON na start (łatwy do debugowania), MessagePack później.
158. Czy gra ma obsługiwać mody (dane w JSON)? **P:** tak, z folderu `mods/`.
159. Czy jest tryb wieloosobowy? **P:** nie.
160. Na jakim sprzęcie testujemy? **P:** do ustalenia: komputer użytkownika (specyfikacja?).
161. Gdzie trzymać kod: Pulpit\Gra\game? **P:** tak, z gitem.
162. Czy używać Godot 4.3, czy nowszego? **P:** najnowszy stabilny z C#.
