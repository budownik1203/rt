#!/usr/bin/env python3
"""Podgląd walki z góry: JSON z `Latarnia.Tools replay` → GIF z dymkami, zagrożeniami i przydziałami.

Użycie: python3 replay_gif.py walka.json walka.gif [--od 0] [--do 90] [--skala 9]
Legenda: niebieskie kółka = skład (litera = rola), czerwone = wrogowie, pomarańczowe okręgi = zagrożenia
widziane przez dowódcę, cienkie linie = przydział do zadania (kolor typu zadania), dymki = rozkazy i okrzyki.
"""
import json, sys, math, argparse
from PIL import Image, ImageDraw, ImageFont

ap = argparse.ArgumentParser()
ap.add_argument("inp"); ap.add_argument("out")
ap.add_argument("--od", type=float, default=0); ap.add_argument("--do", type=float, default=9999)
ap.add_argument("--skala", type=float, default=10); ap.add_argument("--fps", type=int, default=8)
ap.add_argument("--kadr", type=float, default=50, help="bok wycinka mapy w metrach (wokół latarni)")
a = ap.parse_args()
d = json.load(open(a.inp))
S = a.skala; cell = d["cell"]; W, H = d["w"], d["h"]
lx, ly = d["lamp"]
x0, y0 = lx - a.kadr / 2, ly - a.kadr / 2
PW = int(a.kadr * S); PANEL = 300
font = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSansCondensed.ttf", 11)
fontb = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSansCondensed-Bold.ttf", 11)
fonts = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSansCondensed.ttf", 10)

def P(x, y): return ((x - x0) * S, (y - y0) * S)

# tło: teren i światło
bg = Image.new("RGB", (PW + PANEL, PW), (14, 14, 18))
px = bg.load()
for py in range(PW):
    for pxx in range(PW):
        wx, wy = x0 + pxx / S, y0 + py / S
        cx, cy = int(wx / cell), int(wy / cell)
        if not (0 <= cx < W and 0 <= cy < H): continue
        ch = d["map"][cy][cx]; L = (ord(d["light"][cy][cx]) - 97) / 15
        base = (34, 36, 30)
        if ch == "#": base = (92, 84, 72)
        elif ch == "c": base = (120, 96, 60)
        elif ch == "r": base = (40, 58, 34)
        k = 0.35 + 0.65 * L
        warm = (int(base[0] * k + 30 * L), int(base[1] * k + 22 * L), int(base[2] * k + 6 * L))
        px[pxx, py] = warm
dr = ImageDraw.Draw(bg)
lp = P(lx, ly); dr.ellipse([lp[0] - 4, lp[1] - 4, lp[0] + 4, lp[1] + 4], fill=(255, 230, 140))

TASKC = {"Hold": (90, 160, 255), "Assault": (255, 120, 80), "FireSupport": (120, 230, 120), "Eliminate": (255, 230, 80), "KillAtRange": (255, 200, 60),
         "Rescue": (255, 120, 200), "Watch": (190, 140, 255), "Reserve": (150, 150, 150), "Avoid": (255, 80, 80)}
ROLE = {"Tarczownik": "T", "Rebacz": "R", "Strzelec": "S", "Sanitariusz": "+", "Zwiadowca": "Z", "Herszt": "H", "Lucznik": "Ł", "Rozbijacz": "B", "Nozownik": "N", "Obdartus": "o"}
KINDC = {"Rat": (170, 60, 60), "Ogar": (230, 70, 50), "Krzykacz": (230, 90, 220), "Nosiciel": (140, 220, 60), "Human": (220, 80, 60)}
TASKPL = {"Hold": "trzymać", "Assault": "szturm", "FireSupport": "ogień", "Eliminate": "zdjąć", "KillAtRange": "z daleka", "Rescue": "ratować", "Watch": "tyły", "Reserve": "odwód", "Avoid": "odsuń"}

events = d["events"]
frames = [f for f in d["frames"] if a.od <= f["T"] <= a.do]
imgs = []
for fr in frames:
    im = bg.copy(); g = ImageDraw.Draw(im, "RGBA")
    units = {u["Id"]: u for u in fr["U"]}
    # flary
    for fl in fr.get("Fl", []):
        c = P(fl[0], fl[1]); r = fl[2] * S
        g.ellipse([c[0] - r, c[1] - r, c[0] + r, c[1] + r], fill=(255, 90, 60, int(38 * fl[3])), outline=(255, 120, 80, int(120 * fl[3])))
        g.ellipse([c[0] - 3, c[1] - 3, c[0] + 3, c[1] + 3], fill=(255, 160, 120))
    # zagrożenia (dowódca składu 0)
    for th in fr["Th"]:
        if th["Squad"] != 0: continue
        c = P(th["X"], th["Y"]); r = (th["R"] + 1.2) * S
        g.ellipse([c[0] - r, c[1] - r, c[0] + r, c[1] + r], outline=(255, 150, 40, 200), width=2)
        g.text((c[0] - r, c[1] - r - 12), th["Label"], font=fonts, fill=(255, 170, 70))
    # zadania i przydziały
    for tk in fr["Tk"]:
        col = TASKC.get(tk["Type"], (200, 200, 200)); alpha = 150 if tk["Squad"] == 0 else 70
        tp = P(tk["X"], tk["Y"])
        if tk["Type"] != "Reserve":
            g.rectangle([tp[0] - 3, tp[1] - 3, tp[0] + 3, tp[1] + 3], outline=col + (alpha,))
        for wid in tk["Who"]:
            u = units.get(wid)
            if u and tk["Type"] != "Reserve": g.line([P(u["X"], u["Y"]), tp], fill=col + (alpha // 2,), width=1)
    # pociski
    for p in fr["P"]:
        s = P(p[0], p[1]); g.line([s, (s[0] - p[2] * 8, s[1] - p[3] * 8)], fill=(255, 255, 200), width=2)
    # jednostki
    for u in fr["U"]:
        c = P(u["X"], u["Y"])
        squad = u["Team"] == 0
        if u["St"] == "Dead":
            col = (90, 90, 110) if squad else (90, 50, 50); g.line([c[0] - 3, c[1] - 3, c[0] + 3, c[1] + 3], fill=col, width=2); g.line([c[0] - 3, c[1] + 3, c[0] + 3, c[1] - 3], fill=col, width=2); continue
        if u["Kind"] == "Human":
            r = 5.5
            col = (80, 150, 255) if squad else (230, 90, 60)
            if u["St"] == "Down": col = (255, 120, 200) if squad else (140, 70, 60)
            g.ellipse([c[0] - r, c[1] - r, c[0] + r, c[1] + r], fill=col, outline=(10, 10, 10))
            fx, fy = math.cos(u["F"]), math.sin(u["F"])
            g.line([c, (c[0] + fx * 10, c[1] + fy * 10)], fill=(255, 255, 255, 200), width=2)
            g.text((c[0] - 3, c[1] - 7), ROLE.get(u["Role"], "?"), font=fontb, fill=(0, 0, 0))
            if squad: g.text((c[0] + 7, c[1] + 2), u["Name"], font=fonts, fill=(190, 210, 255))
        else:
            r = {"Rat": 2.5, "Ogar": 4.5, "Krzykacz": 5, "Nosiciel": 6}.get(u["Kind"], 4)
            g.ellipse([c[0] - r, c[1] - r, c[0] + r, c[1] + r], fill=KINDC.get(u["Kind"], (200, 60, 60)), outline=(0, 0, 0))
    # dymki
    for u in fr["U"]:
        if not u["Say"] or u["St"] == "Dead": continue
        c = P(u["X"], u["Y"]); txt = u["Say"]
        tw = g.textlength(txt, font=font)
        bx, by = min(max(4, c[0] - tw / 2), PW - tw - 8), max(2, c[1] - 26)
        squad = u["Team"] == 0
        g.rounded_rectangle([bx - 4, by - 2, bx + tw + 4, by + 13], 4, fill=(245, 238, 220, 235) if squad else (60, 20, 20, 230))
        g.text((bx, by), txt, font=font, fill=(20, 20, 20) if squad else (255, 200, 190))
    # panel
    g.rectangle([PW, 0, PW + PANEL, PW], fill=(22, 22, 26))
    g.text((PW + 10, 8), f"{d['scenario']}  dowódca poziom {d['level']}  t={fr['T']:5.1f} s", font=fontb, fill=(240, 220, 160))
    yy = 30
    for u in fr["U"]:
        if u["Team"] != 0: continue
        st = {"Ok": "", "Down": " LEŻY", "Dead": " NIE ŻYJE"}.get(u["St"], "")
        task = next((TASKPL.get(t["Type"], t["Type"]) for t in fr["Tk"] if t["Squad"] == 0 and u["Id"] in t["Who"]), "-")
        g.text((PW + 10, yy), f"{u['Name']:<7}{st}", font=fontb, fill=(255, 120, 200) if st else (180, 200, 255))
        g.text((PW + 10, yy + 13), f"zadanie: {task} | {u['Do'][:26]}", font=fonts, fill=(200, 200, 200))
        g.rectangle([PW + 200, yy + 3, PW + 200 + 80 * max(0, u["Blood"]) / 100, yy + 8], fill=(190, 40, 40))
        g.rectangle([PW + 200, yy + 11, PW + 200 + 80 * max(0, u["Morale"]) / 100, yy + 15], fill=(220, 200, 80))
        yy += 32
    yy += 6
    g.text((PW + 10, yy), "Dziennik:", font=fontb, fill=(240, 220, 160)); yy += 15
    recent = [e for e in events if e["t"] <= fr["T"] and e["k"] in ("Order", "Reply", "Down", "Kill", "Rescue", "Scream", "Explosion", "Chaos", "FriendlyFire")][-14:]
    for e in recent:
        txt = f"{e['t']:5.1f} {e['Text']}"
        while txt and yy < PW - 12:
            line = txt
            while g.textlength(line, font=fonts) > PANEL - 16: line = line[:-1]
            g.text((PW + 8, yy), line, font=fonts, fill=(210, 210, 210) if e["k"] != "Order" else (255, 235, 180)); yy += 12
            txt = txt[len(line):].strip()
            if len(line) < 6: break
    imgs.append(im.convert("P", palette=Image.ADAPTIVE, colors=128))
imgs[0].save(a.out, save_all=True, append_images=imgs[1:], duration=int(1000 / a.fps), loop=0, optimize=True)
print(f"{a.out}: {len(imgs)} klatek")
