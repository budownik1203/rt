# Test shadera pixel art w Godocie (8 listopada 2026)

Projekt testu: `game/shader_test/` (Godot 4.7.2, Forward+, GDScript tylko w teście). Obrazy: `docs/test_shadera/`.

## Co zbudowano

1. **Scena** z modeli `.glb` z paczki: 2 żołnierze (klipy `shoot` i `walk`), Ogar (`gallop`), szałas, kuchnia, wieża strażnicza, palisada, lampa pomocnicza.
2. **Render** do SubViewport 640×360 i powiększenie ×3 do 1920×1080 bez wygładzania.
3. **Shader materiału** (`shaders/toon.gdshader`):
   - pasma światła z tymi samymi progami co `post.py` w Blenderze;
   - cienie przesunięte w chłodny fiolet;
   - faktura ziemi z szumu.
4. **Postproces** (`shaders/post.gdshader`, pełnoekranowy kwadrat):
   - kontur 1 px z bufora głębi;
   - Mgła promieniowo od latarni z ditheringiem Bayera tylko w Mgle;
   - kwantyzacja do palety 48 kolorów (12 ramp materiałów po 4 tony).
5. **Światła:**
   - ciepła lampa z cieniem, której własna geometria nie rzuca cienia;
   - słaby chłodny księżyc;
   - wypełnienie i kontur od tyłu tylko na postaciach (maska świateł, odpowiednik light linking z Blendera).
6. **Ukrywanie błysku z lufy, magazynka i granatu** przy wczytaniu modelu.

## Wyniki

- **Styl da się odtworzyć na żywo.** Pasma światła, kontur, Mgła z ditheringiem i paleta działają w Godocie. Broń jest w dłoniach, a animacje grają z plików `.glb`.
- **Czytelność:**
  - w zbliżeniu (ortho 7 m) postacie są czytelne: hełmy, twarze, broń, cienie;
  - przy domyślnym oddaleniu (ortho 15 m) postać ma około 15 px wysokości w 640×360, więc jest mała, zgodnie z uwagą z analizy;
  - użytkownik nie chce domyślnego przybliżenia, więc czytelność muszą dać obwódki pod jednostkami i mocniejsze barwy składu. W tym teście ich jeszcze nie ma.
- **Obrót kamery:**
  - płynny obrót działa, ale przy każdym kroku sylwetki zmieniają piksele (typowe migotanie 3D renderowanego jako pixel art);
  - przyciąganie kamery do siatki tekseli nie pomaga przy obrocie (zmiana pikseli między klatkami jest taka sama, 2,58 w obu wariantach), bo pomaga tylko przy przesuwaniu kamery;
  - zostaje do przesuwania.
- **Wydajność:**
  - około **860 wywołań rysowania** dla 3 postaci i 5 budynków, około 10 tysięcy trójkątów;
  - trójkątów jest mało, problemem są wywołania;
  - skalując do docelowej sceny (kilkunastu ludzi, kilkadziesiąt mutantów, kilkadziesiąt budynków) wychodzi kilka do kilkunastu tysięcy wywołań, za dużo dla GTX 1660 Super;
  - **scalanie siatek przy eksporcie jest konieczne.**
- **Test robiony na procesorze** (programowy Vulkan, llvmpipe), więc nie mierzy klatek na sekundę na prawdziwej karcie. Prawdziwy pomiar musi być u użytkownika.

## Co wyszło słabo (szczerze)

1. **Paleta przeskakuje między rampami.**
   - **Problem:** kolejne pasma światła na ziemi zmieniają barwę (zielony, potem brąz, potem szary), zamiast ciemnieć w jednej barwie, bo kwantyzacja szuka najbliższego koloru w całej palecie.
   - **Rozwiązanie:** materiał zna swoją rampę (indeks rampy zapisany w buforze albo w kanale alfa), a postproces wybiera ton tylko z tej rampy. Ewentualnie ręcznie zaprojektowana LUT 3D.
2. **Plamy szumu na ziemi są za duże i wyglądają jak stempel.** Potrzebna jest faktura z kępkami trawy, kamykami i ścieżkami, jak w renderach z Blendera.
3. **Pasma światła lampy tworzą równe koncentryczne pierścienie.** Faktura łagodzi je częściowo. Do rozważenia: zaburzenie progów szumem.
4. **Kontur tylko z głębi.** Brak jaśniejszego konturu po stronie światła (selective outline) i konturu z normalnych na załamaniach brył. Oba da się dodać (Godot daje bufor normalnych w Forward+).
5. **Brak obwódek pod jednostkami i barw składu**, czyli tego, co ma zastąpić przybliżenie.
6. **Ogar** jest słabo oświetlony i zlewa się z ziemią.
7. **Dachy jasnych budynków** wychodzą szaro-liliowe. To kwestia kolorów materiałów w modelach, nie shadera.

## Następne kroki

1. Paleta z rampami przypisanymi do materiałów.
2. Obwódki pod jednostkami, barwy składu, jaśniejszy kontur po stronie światła.
3. Scalanie siatek postaci w eksporcie (jedna siatka ze szkieletem i atlasem kolorów) oraz instancjonowanie rojów.
4. Faktura ziemi.
5. Ten sam test uruchomiony u użytkownika na GTX 1660 Super: pomiar klatek na sekundę.
6. Decyzja o obrocie kamery (patrz niżej).

## Do decyzji

- **Obrót kamery:** zostaje w pełni płynny (z migotaniem w trakcie obrotu), czy hybryda: płynny podczas obracania, a po puszczeniu przycisku dociąga do najbliższego z 8 kątów, w których pixel art jest stabilny.
