# Na rano: co sprawdzić i potwierdzić (praca z nocy 6/7 listopada)

## 1. Najpierw obejrzyj

- `ui/interfejs_v2.html`: nowy interfejs, otwórz w przeglądarce.
  - Klikaj budynki na widoku osady.
  - Kliknij „Kompleks szkoleniowy” w rogu widoku osady.
  - Przejdź po zakładkach klawiszami O, L, Z, M, R, N, W.
  - Na dole ekranu przełączniki stanu osady pokazują, jak interfejs reaguje na głód, zagrożenie, Odmienność i światło.
- **Zakładki dokumentu online:** „Epoki” i „Budynki” (tabela cyklu życia). Pliki w folderze są pełniejsze.

## 2. Przeczytaj i zdecyduj

- `docs/ANALIZA.md`: 10 największych ryzyk z propozycjami, tabela dubli i niespójności do poprawy.
- `docs/PYTANIA.md`: 162 pytania z propozycjami odpowiedzi, w tym strategie wszystkich 11 frakcji. Zacznij od ★. Wystarczy odpowiedzieć „zgoda” albo podać swoją wersję.

## 3. Przejrzyj nowe treści

| Plik | Co zawiera |
| --- | --- |
| `docs/TECHNIKA_AI_I_MECHANIKI.md` | jak zbudować grę: architektura, AI dowódcy w 5 krokach z kodem, instynkt, balistyka, osada, mapa, shadery |
| `docs/EPOKI.md` | 7 epok w pełni plus tabela nowych mechanik w każdej epoce |
| `docs/BUDYNKI.md` | wygląd każdego budynku w każdej epoce jako gotowy prompt dla nowego czatu |
| `docs/WROGOWIE.md` | bestiariusz w liczbach, scalone duble, 5 nowych wrogów, licznik „Mgła się uczy”, bossowie z fazami |
| `docs/BRON.md` | broń wszystkich epok z wersjami, dodatkami, amunicją i przeróbkami |
| `docs/UZUPELNIENIA.md` | ukryte cechy, zdolności klas w liczbach, tempo badań, 40 nowych wydarzeń, 12 postaci z historiami |

## 4. Zmiany oznaczone [zmiana], które czekają na zgodę

- Ogar bez chityny (zwykły pies).
- Kłębowisko to młody Rozrost.
- Siewca, który przetrwa Przypływ, staje się Matką Roju.
- Linia przemiany: Kokon → Biegacz → Odmieniec → Wydrążony; Śpiący to stan uśpienia.
- Fałszywy Latarnik to odmiana Naśladowcy.
- Nici i Oddech to anomalie.
- Licznik adaptacji Mgły: próg 60 to zapowiedź, 80 to adaptacja na 15 dni, najwyżej 2 naraz.

## 5. Czego nie zrobiłem

- ~~Zima i dni 11–36~~: zrobione 7 listopada (UZUPELNIENIA.md, sekcja 6).
- ~~Pełne zestawy zdolności klas~~: zrobione (KLASY.md).
- ~~Pula około 150 wydarzeń~~: zrobione (WYDARZENIA.md, razem około 155).
- **Nowe pliki nie trafiły do dokumentu online** (claude.ai). Źródłem prawdy jest folder.

## 6. Jak kontynuować w nowym czacie

Rozpakuj `Latarnia_paczka.zip` do `Pulpit\Gra` i podmień pliki. Folder `assets` z dużymi archiwami grafiki zostaje bez zmian. Potem w nowym czacie napisz:

> „Przeczytaj CLAUDE.md, RANO_DO_SPRAWDZENIA.md i dokumenty z docs, potem zadaj mi pytania ★ z PYTANIA.md w formularzu.”
