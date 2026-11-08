# Latarnia: symulator bitew (Godot 4.7 + C#)

Edytor składu i przeciwnika, bitwa 3D w stylu pixel art (modele i animacje z Blendera) i raport po walce.
Pod spodem jest ta sama deterministyczna symulacja co w `game/sim` (AI dowódcy w 5 krokach).

## Uruchomienie

1. Zainstaluj **Godot 4.7.2 .NET** (wersja „mono”) i **.NET 8 SDK**.
2. W Godocie wybierz „Importuj” i wskaż `game/symulator/project.godot`.
3. Przy pierwszym otwarciu poczekaj na import modeli, potem kliknij „Build” (młotek) i „Uruchom” (F5).

Z wiersza poleceń (Linux, bez monitora, zrzuty do `out/`):

```
godot --rendering-driver vulkan -- edytor                         # zrzut edytora
godot --rendering-driver vulkan -- demo noc 1001                  # bitwa od razu, zrzuty co 1,6 s, raport
godot --rendering-driver vulkan --fixed-fps 12 -- demo noc 1001 8 6 15   # kamera za składem (ortho 8 m), nagranie klatek od 6 do 15 s
godot --rendering-driver vulkan -- przelicz                       # przelicznik 100 walk z edytora
```

Gotowe bitwy dla `demo`: `noc`, `ogary`, `szczury`, `szabrownicy`, `starcie`.

## Sterowanie w bitwie

- **Kamera:** Q/E obrót, kółko myszy przybliżenie, WASD przesuw.
- **Zaznaczenie:** klik zaznacza jednostkę (dymek z imieniem i zadaniem), a F przenosi do niej kamerę.
- **Czas:** spacja to pauza, przyciski ×0,5 / ×1 / ×2 / ×4 zmieniają tempo.
- **Koniec:** „Zakończ i raport” przerywa walkę i pokazuje raport.

## Edytor

- **Skład A** i (przy przeciwniku „Skład ludzi”) **skład B:**
  - ludzie (do 10): imię, klasa, broń główna i boczna, hełm, pancerz, apteczki, model;
  - 7 statystyk, wyszkolenie taktyczne, cechy (Odważny, Tchórz, Litościwy);
  - dowódca: kto dowodzi, poziom 1–5, Zasoby ludzkie, Orientacja w terenie, Taktyka i formacje, charakter, flary.
- **Przeciwnik:**
  - fale mutantów: gatunek, liczba, kierunek, opóźnienie;
  - albo skład ludzi w trybie „atakuje osadę” lub „starcie w polu”.
- **Ziarno losowania i zapis:** ziarno ustawiasz w polu u góry, a „Zapisz” i „Wczytaj” trzymają bitwy w JSON w katalogu użytkownika Godota (`bitwy/`).
- **Przelicz 100 walk:** ta sama bitwa na 100 ziarnach, bez grafiki. Wynik pojawia się w prawej kolumnie.

## Pliki

| Plik | Co robi |
|---|---|
| `src/Main.cs` | przełączanie ekranów i tryby z wiersza poleceń |
| `src/EditorScreen.cs` | edytor bitwy |
| `src/BattleView.cs` | świat 3D, kamera, pociski, flary, efekty, panel, dymki |
| `src/UnitView.cs` | jednostka: animacje ze stanu symulacji i zdarzeń, interpolacja |
| `src/Art.cs` | materiał toon, scalanie siatek (1 wywołanie rysowania na model), rekwizyty |
| `src/ReportScreen.cs` | raport po walce |
| `src/Ui.cs` | wygląd interfejsu |
| `models/units/` | 18 modeli z pełnymi zestawami klipów |
| `models/buildings/` | budynki z eksportu Blendera |

Skrypty, z których powstają modele, są w `assets/latarnia_zrodla_grafiki.zip`: `anim_combat.py`, `export_combat.py`, `export_mutants.py`, `preview_combat.py`.
