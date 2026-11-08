# Potok grafiki Latarni

Grafika powstaje jako **3D low-poly w Blenderze**, a potem jest przerabiana na pixel art własnym postprocesem w Pythonie. Docelowo ten sam efekt ma dawać shader w Godocie przy renderze na żywo.

## Środowisko

- Blender jako moduł Pythona: `pip install bpy` (wersja 5.x), skrypty uruchamiane przez `python3 skrypt.py -- argumenty`.
- Cycles na CPU, 16–24 próbki, odszumianie OIDN, `filter_width = 0.01` (ostre piksele).
- Wynik to wielowarstwowy EXR: Combined, Depth.Z, Normal, Diffuse Color, Emission, Object Index. Czyta go pakiet `OpenEXR`. Uwaga: najpierw trzeba ustawić `media_type='MULTI_LAYER_IMAGE'`, dopiero potem `file_format='OPEN_EXR_MULTILAYER'`.
- Postproces w numpy i PIL (`post.py`).

## Kamera i światło

- Kamera ortograficzna: nachylenie 50–52° od pionu, obrót wokół osi pionowej −25°…−32°. Skala ortho 13–30 m zależnie od ujęcia.
- Postacie mają około 1,75 m, latarnia około 8 m (około 4,5 wzrostu człowieka).
- Światło kluczowe ciepłe, a słońce wypełniające i chłodny kontur od tyłu świecą **tylko na postacie** (light linking na kolekcję postaci). To daje czytelność postaci w ciemnych scenach.
- Latarnia to punktowe światło o mocy 9–16 kW na wysokości około 8,5 m. Jej szkło, galeria i dach mają `visible_shadow = False`, bo inaczej zasłaniają własne światło.

## Postproces do pixel artu (`post.py: stylize`)

1. **Światło:** (Combined − Emission) / Diffuse Color.
2. **Pasma cel-shadingu:** progi [0,04; 0,12; 0,30; 0,62] dają wartości [0,06; 0,16; 0,36; 0,66; 1,0].
3. **Cienie:** przesunięte w chłodny fiolet.
4. **Emisja:** dodawana bez zmian (lampy, błyski, gruczoły).
5. **Kontur 1 px** z nieciągłości głębi:
   - próg 0,07 dla postaci (Object Index ≥ 3) i 0,22 dla otoczenia;
   - jaśniejszy kontur po stronie oświetlonej (selective outline).
6. **Mgła:** odtworzona w przestrzeni świata z kamery ortograficznej. Wersja liniowa po osi X albo promieniowa od latarni (krąg światła). Dithering Bayera działa tylko w Mgle.
7. **Paleta:** kwantyzacja do około 45 kolorów.
8. **Skalowanie:** powiększenie ×2–×3 bez wygładzania (nearest).

Uwaga: jasne beże i kość pod niebieskim konturem robią się zielonkawe, dlatego kolory kości są przesunięte w róż.

## Postacie (`chars.py`)

- **Szkielet** (armatura): hips, spine, chest, neck, head, shoulder/upperarm/forearm/hand/thigh/shin/foot z sufiksami .L i .R, plus kość `weapon` (i `shield`) zależna od pozy broni, dziecko `chest`.
- **Części sztywne** podpięte do kości, bez skinningu. Ubiór warstwowy:
  - powłoki 1–3 cm nad ciałem i nakładki na barki, łokcie i kolana, żeby nic nie przechodziło przez siebie;
  - odsłonięte tylko głowa i dłonie.
- **Broń** na kości `weapon`. Dłonie trzymają ją przez IK (łańcuch 2) do celów `ikR` i `ikL` przypiętych do broni. Bazowe położenia celów są w `rig.tg_base`.
- **Pozy broni:**
  - rifle, musket, shotgun, smg, mg, crossbow, lance (wspólny chwyt karabinowy);
  - pistol;
  - knife, club, machete (jednoręczne);
  - spear (z tarczą), hammer, axe, bow, banner, grenade;
  - shovel, carry, smith (praca);
  - idle (bez broni).
- `human(name, outfit, pose)`. Outfit: jacket, pants (klucze z `CLOTH`), helmet (pot/mil), armor (vest/plate), mask (gas), beard, hair (hair/hair_l), skin (skin/pale), glow_eyes, third_arm, strap, pouches.
- **Konwencje znaków obrotu** (`rig.rot(bone, axis, deg)`, w osiach armatury):
  - kość skierowana w dół (ręka, udo): +ry odchyla do tyłu, −ry do przodu;
  - kość skierowana w górę (kręgosłup): +ry pochyla do przodu;
  - kość skierowana w +X (karabin): +ry opuszcza lufę;
  - rz dodatnie (CCW z góry) wysuwa prawy bark do przodu;
  - rx ujemne odwodzi prawe ramię.
- **Postać patrzy w +X**, a lewa strona to +Y.

## System animacji (`anim.py`)

- **Poza** to słownik `P(rot, loc, tgt, ik, root, vis)`:
  - `root = (dx, dy, dz, pochylenie, przechył)`;
  - `vis` przełącza obiekty: flash, mag, maghand, gren.
- `merge` dodaje pozy, `lerp_pose` je miesza, a `Clip(length, keys)` interpoluje z krzywymi `io/in/out/snap/hold/lin`. Błysk z lufy trwa tylko w klatce kluczowej.
- `Seq` składa segmenty z przejściami i miesza też prędkość przesuwu, więc stopy się nie ślizgają (chód 0,137 m/kl, patrol 0,12, bieg 0,30 przy 12 kl/s).
- Cykle i klipy:
  - cykle: `walk`, `run`, `run_rifle`;
  - strzelanie: `clip_shoot`, `clip_reload`, `clip_cover`;
  - wręcz i rzut: `clip_spear`, `clip_hammer`, `clip_grenade`, `clip_slash`;
  - śmierć i reakcje: `clip_death` (broń wypada z rąk), `stagger`, `flinch`;
  - pozycje: `kneel`, `prone`, `crawl`, `dive_mid`;
  - ruch specjalny: `run_turn`, `climb`;
  - praca: `clip_chop`, `clip_dig`, `carry_walk`, `clip_smith`, `talk`, `sit`.
- `anim_mut.py`: mutanty (galop, skok Ogara, krzyk i bieg Krzykacza, chód i puchnięcie Nosiciela). `mutanim.py` to starsza wersja z nocy.
- `fx.py`: łuski, dym z lufy, wypadający magazynek, iskry i drzazgi, wybuch, gaz i szczątki, fala dźwięku.

## Najważniejsze skrypty

| Plik | Co robi |
|---|---|
| `lat3d.py` | pomocnicze: materiały, loft, ellipsoid, box, cyl, Rig, światła, kamera, render |
| `chars.py` | ludzie, broń 7 epok, narzędzia, mutanty (Ogar to pies z `mutants2.quadruped`) |
| `factions.py`, `factions2.py` | dodatki frakcji; 11 frakcji × 6–8 jednostek ze znakami (tekstury z `emblems.py`) |
| `mutants2.py` | czworonogi (pies, wilk, dzik, jeleń), Stworzenia Mgły, ludzie zmienieni |
| `bestiary.py`, `buildings.py`, `weapons.py` | 12 mutantów z nocy, 39 budynków, 13 broni |
| `showcase*.py`, `osada.py`, `battle.py`, `arsenal.py`, `bestiary2.py` | sceny pokazowe; `make_*.py` składa z nich GIF-y |
| `export_all.py`, `export_new.py`, `export_anims.py` | eksport .glb (IK wypiekane do FK, klipy jako ścieżki NLA) |
| `post.py` | postproces do pixel artu |
| `emblems.py` | 11 znaków frakcji 48×48 i paleta barw frakcji |

## Do Godota

- Modele `.glb` i README z konwencjami są w `latarnia_modele_godot.zip`.
- Widoczność błysku, magazynka i granatu steruje silnik zdarzeniami.
- Przesuw postaci przy chodzie i biegu robi silnik, z prędkościami podanymi wyżej.
- Postproces trzeba odtworzyć shaderem:
  - pasma oświetlenia w shaderze materiału (toon);
  - kontur z bufora głębi w postprocesie;
  - kwantyzacja do palety przez LUT;
  - render do SubViewport w niskiej rozdzielczości (np. 640×360) i skalowanie nearest.
