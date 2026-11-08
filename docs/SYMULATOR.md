# Symulator bitew w 3D (stan 8 listopada 2026, wieczór)

Pierwsza wersja „docelowej jakości”: edytor składu i przeciwnika, bitwa 3D z modelami i animacjami z Blendera w stylu pixel art oraz raport. Pod spodem działa ta sama symulacja i AI dowódcy co w `game/sim` (`docs/PROTOTYP_WALKI.md`). Jak uruchomić: `game/symulator/README.md`.

Pokazy: `docs/symulator/pokaz_noc.gif` (Noc Mgły, kamera za składem) i `docs/symulator/pokaz_szabrownicy.gif` (atak bandy), klatki w pełnej rozdzielczości obok.

## Co powstało

### Animacje (Blender, `anim_combat.py`, `export_mutants.py`)

Każdy model dostał pełny zestaw klipów, wypiekany z IK do FK.

**Ludzie** (8 modeli składu i 6 Szabrowników) mają 12 klipów wspólnych:

| Klip | Co pokazuje |
|---|---|
| idle, walk, run | spoczynek, chód, bieg |
| hit | reakcja na trafienie |
| fall | upadek ciężko rannego, z bronią wypadającą obok |
| down | ranny na plecach, trzyma ranę i oddycha |
| death, dead | śmierć i ciało |
| treat | opatrywanie na klęczkach, broń na plecach |
| drag | wyciąganie rannego tyłem |
| dragged | ciągnięty ranny z rękami nad głową |
| cower | kulenie się w panice |

Do tego klipy zależne od broni:

| Broń | Klipy |
|---|---|
| Włócznia z tarczą | pchnięcie zza tarczy, blok tarczą |
| Młot | uderzenie z antycypacją i wyrwaniem |
| Nóż | pchnięcie |
| Rura, maczeta | cięcie |
| Łuk | sięgnięcie do kołczanu, nałożenie strzały, naciąg, puszczenie z odrzutem; w zwarciu uderzenie łukiem |
| Kusza | strzał, potem napinanie przez 3 s (dziób w dół, stopa w strzemieniu, cięciwa do orzecha, bełt) |

Szczegóły techniczne:

- **Cięciwy, strzała i bełt** to części na osobnych kościach skalowanych 0/1, więc animacja sama je pokazuje i chowa, bez kodu w silniku. Naciągnięta cięciwa jest w kształcie litery V, luźna prosto. Dodany kołczan.
- **Broń na plecy:** przy opatrywaniu i ciąganiu każda broń wędruje na plecy. Pozycje są liczone z osi broni, a nie zgadywane kątami.
- **Sanitariusz** ma torbę z czerwonym krzyżem i opaskę.

**Mutanty:**

- **Ogar:**
  - spoczynek: dyszy i rozgląda się;
  - skradanie: nisko, boczna sekwencja łap;
  - galop;
  - ugryzienie w miejscu: przyczajenie, wypad, szarpnięcie głową;
  - skowyt po trafieniu;
  - śmierć.
- **Krzykacz:** spoczynek, chód, bieg, krzyk (paszcza się rozdyma), cięcie pazurami, trafienie, śmierć.
- **Nosiciel:**
  - spoczynek i chód;
  - trafienie: brzuch puchnie;
  - pęknięcie: brzuch na własnej kości.
- **Szczur Mgły:** nowy model z kośćmi (zmutowany szczur ok. 45 cm, łysy placek, wrzód, kolce na grzbiecie, świecące oczy). Klipy: spoczynek z węszeniem, bieg skokami, ugryzienie, trafienie, śmierć na grzbiecie.

### Symulacja

- **Opis bitwy z edytora** (`Data/Spec.cs`, JSON):
  - ludzie: klasa, 7 statystyk, wyszkolenie, broń, pancerz, apteczki, cechy, model;
  - dowódca;
  - fale mutantów albo drugi skład w trybie „atakuje osadę” lub „starcie w polu”.
- **Klasy epoki Złomu** (`Data/Classes.cs`, z `KLASY.md`): 15 klas, z których 14 ma zdolność. Zdolności i premie do statystyk sumują się po drzewku.

  | Klasa | Zdolność |
  |---|---|
  | Tarczownik | Zasłona |
  | Mur | Twardy szyk |
  | Dzidnik | Ściana grotów |
  | Rębacz | Wypad |
  | Łamacz | Rozbicie |
  | Nożownik | Seria cięć |
  | Strzelec | Ogień celowany |
  | Strzelec wyborowy | Celny strzał |
  | Zwiadowca | Wypatrywanie |
  | Tropiciel | Trop |
  | Cichy łowca | Ciche zabicie |
  | Sanitariusz | Stabilizacja |
  | Felczer | Leczenie w polu |
  | Sanitariusz bojowy | Wyciągnięcie |

- **Zdarzenia dla widoku** (`Battle.Fx`): cios z wynikiem (trafienie, chybienie, blok), strzał, trafienie, upadek, śmierć, krzyk, wybuch, flara, okrzyk, zdolność.

### Godot (`game/symulator`)

- **Edytor:** trzy kolumny na papierze.
  - Lewa: skład i dowódca.
  - Środkowa: wojownik.
  - Prawa: przeciwnik i warunki.

  Do tego gotowe bitwy, ziarno, zapis i wczytywanie, „Przelicz 100 walk” w tle oraz podpowiedzi przy statystykach.
- **Bitwa:**
  - render do 1/3 rozdzielczości z konturem, Mgłą od latarni i paletą 48 kolorów (shader z testu);
  - scalanie siatek: jedno wywołanie rysowania na model, z pamięcią podręczną na typ;
  - nowa smukła latarnia, palisada z pali z bramą i wyłomem, budynki z eksportu, skrzynie i beczki, zarośla, wraki;
  - pociski w locie, flary opadające z migotaniem, wybuch Nosiciela, fala krzyku;
  - dymki rozkazów bez nachodzenia na siebie, panel stanu obu składów, dziennik u dołu.
- **Animacje w silniku:**
  - łucznik zaczyna naciąg tak, żeby puścić cięciwę dokładnie w chwili strzału w symulacji;
  - kusznik po strzale napina kuszę w tempie odnowienia;
  - tempo ciosu dopasowane do szybkości broni;
  - ciągnący idzie tyłem, a ranny leży głową przy nim.
- **Raport:** wynik, ludzie obu stron, wnioski według specjalizacji dowódcy, kluczowe chwile, dziennik rozkazów. Przyciski: „Powtórz”, „Nowe ziarno”, „Edytor”.

## Szczera ocena

Dobrze:

- **Pixel art z bliska wygląda jak gra:**
  - postacie mają około 60 px przy przybliżeniu 7–8 m;
  - broń jest w dłoniach, cięciwa się napina;
  - ranni leżą, ciała zostają, latarnia daje ciepły krąg, a Mgła zjada brzegi.
- **Bitwa jest czytelna:** dymki z imionami, kolory obwódek, dziennik.
- **Całość działa od edytora do raportu**, a przelicznik w tle liczy 100 walk w kilka sekund.

Słabiej albo niesprawdzone:

1. **Animacje oceniałem z arkuszy klatek i nagrań, nie na żywo w oknie.** Mogą być drobne kolizje: dłoń chwilami obok drzewca w szybkich przejściach, broń na plecach u pochylonej postaci może lekko wchodzić w plecy. Do Twojej oceny po uruchomieniu.
2. **Brak broni bocznej na modelu:** strzelec w zwarciu uderza łukiem albo kuszą, choć symulacja liczy nóż.
3. **Brak dźwięku.**
4. **Budynki stoją mniej więcej tam, gdzie przeszkody na mapie, ale nie idealnie.** Mapa to wciąż prototypowa „Mała osada”.
5. **Nie znam liczby klatek na sekundę na Twojej GTX 1660 Super.** U mnie renderuje programowy Vulkan bez karty, więc to trzeba zmierzyć u Ciebie.
6. **Skład kontra skład:**
   - w „starciu” obie strony to atakujący, a dowodzony szturm wciąż jest za słaby (ten sam problem co z Szabrownikami w `PROTOTYP_WALKI.md`);
   - w lustrze dowódca poziomu 3 wygrywa tylko 45% walk z poziomem 1 (150 walk, ten sam skład).

   Do poprawy w AI.
7. **Starcia ludzi są bardzo szybkie i krwawe:** 6 na 7 kończy się w 13–30 s. Do decyzji przy balansie śmiertelności.

## Do decyzji

- Czy łuk i kusza mają mieć widoczny nóż u pasa do zwarcia? Wymaga dodatkowego modelu i przełączania broni.
- Czy latarnia proceduralna może zostać do czasu modelu z Blendera (`DO_ZROBIENIA`: model latarni)?
- Długość walk ludzi z ludźmi (patrz punkt 7).
