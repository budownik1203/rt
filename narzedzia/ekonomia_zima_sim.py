# Latarnia: symulacja dni 1-36 (jesień, zima, pierwszy Przypływ). Wartości robocze.
# Uruchom: python3 sim3.py   (pokazuje jeden przebieg i statystyki z 500 losowań dla 3 stylów gry)
import random, math

SEASON = lambda d: 'jesien' if d <= 15 else ('zima' if d <= 30 else 'wiosna')
HOURS = {'jesien': (14, 10), 'zima': (10, 14), 'wiosna': (12, 12)}       # godziny dnia, nocy
LAMP_PH = {1: 0.5, 2: 1.0, 3: 2.0, 4: 3.5}                               # paliwo na godzinę nocy
CAP_MOC1 = 12; PER_LAMP = 3; OIL_LAMP = 1.0                              # lampy oliwne
WOOD = {'jesien': 12, 'zima': 8, 'wiosna': 10}
DAYF = 0.25
FORAGE_AUT = [2.5, 2.5, 2, 1.5, 1.5, 1.5, 1, 1, 1, 1, 1, 1, 1, 1, 1]
SNARE = 1.5; WINTER_FORAGE = 0.3; SPRING_FORAGE = 1.0
MUSH, GROW, PER_CELLAR = 3.5, 4, 3
TAR = 6                                                                  # paliwa na smolarza (8 drewna)
HEAT = {'szalas': {'jesien': 1, 'zima': 2.5, 'wiosna': 1}, 'barak': {'jesien': 1, 'zima': 1.5, 'wiosna': 1}}
CAP = {'szalas': 4, 'barak': 8}
NIGHTS = {2: 1, 4: 6, 7: 3, 9: 8, 12: 4, 14: 6, 18: 6, 21: 8, 24: 5, 27: 10, 29: 6, 33: 6, 35: 8}
TECH = [('Lampy oliwne', 45), ('Ratusz', 60), ('Barak', 75), ('Sidla', 45), ('Suszarnia', 70), ('Studnia', 50)]
B = {'smolarnia': (6, 6, 14), 'kuchnia': (8, 4, 12), 'piwnica': (10, 0, 15), 'szalas': (6, 2, 10),
     'barak': (16, 6, 24), 'ratusz': (20, 10, 30), 'suszarnia': (8, 2, 10), 'lampa': (1, 2, 2)}

def lamp_day(moc, se, lamps):
    d, n = HOURS[se]; ph = LAMP_PH[moc]
    return d * ph * DAYF + n * ph + lamps * OIL_LAMP

def run(style, seed=0, verbose=False):
    R = random.Random(seed)
    good, mid, bad = style == 'dobry', style == 'sredni', style == 'zly'
    s = dict(fuel=20.0, food=25.0, wood=20.0, scrap=10.0)
    adults, children = 9, 1                      # 9 dorosłych (w tym starzec-badacz) + dziecko
    homes = {'szalas': 3, 'barak': 0}
    built = set(); cellars = []; smolarnie = 0; lamps = 0
    known = set(); prog = 0
    order = ['Lampy oliwne', 'Barak', 'Sidla', 'Ratusz', 'Suszarnia', 'Studnia'] if good else [t[0] for t in TECH]
    tq = [t for o in order for t in TECH if t[0] == o]
    ruins = ['wrak', 'wrak', 'wrak', 'stodola', 'dom']
    wounded = []; deaths = 0; zero = 0; dark = 0; morale = 70; visited = 0
    flood = R.randint(31, 36); flood_loss = None
    rows = []
    for day in range(1, 37):
        se = SEASON(day)
        wounded = [d for d in wounded if d > day]
        people = adults + children
        eat = adults + 0.5 * children
        workers = max(0, int(adults - 1 - len(wounded) - 0.5))    # starzec bada, 1 warta (pół dnia dwóch osób)
        res2 = 0
        workers -= res2
        J = dict(kucharz=0, grzyb=0, smolarz=0, drwal=0, zbier=0, budowa=0, wyprawa=0)
        exp_every = 3 if se != 'zima' else 4
        if not bad and day >= 3 and day % exp_every == 0 and workers >= 6:
            J['wyprawa'] = 3
        left = workers - J['wyprawa']
        if 'kuchnia' in built: J['kucharz'] = min(left, 1 + (people > 15)); left -= J['kucharz']
        ready = sum(1 for c in cellars if day >= c + GROW)
        food_days_target = 3
        need_g = math.ceil((eat * 1.05 + max(0, food_days_target * eat - s['food']) / 4) / (MUSH * 1.25))
        J['grzyb'] = min(left, ready * PER_CELLAR, max(0, need_g)); left -= J['grzyb']
        # paliwo
        need_l = max(0, math.ceil((people - CAP_MOC1) / PER_LAMP))
        moc = 2 if (bad or need_l > lamps) else 1
        lamp_need = lamp_day(moc, se, min(lamps, need_l))
        reserve = (65 if (good and day >= 24) else (55 if (mid and day >= 25) else 2.5 * lamp_need)) if not bad else 0
        exp_income = 0 if bad else (3.5 if se == 'jesien' else 2)
        if smolarnie:
            want = lamp_need - exp_income + max(0, reserve - s['fuel']) / 4
            J['smolarz'] = max(0, min(left, 2 * smolarnie, math.ceil(want / TAR)))
            if bad: J['smolarz'] = min(J['smolarz'], 1)
            left -= J['smolarz']
        heat = sum(n * HEAT[h][se] for h, n in homes.items())
        wneed = heat + J['smolarz'] * 1.5 * TAR + max(0, 12 - s['wood']) / 2
        J['drwal'] = min(left, math.ceil(wneed / WOOD[se])) if not bad else min(left, 3)
        left -= J['drwal']
        pending = (not smolarnie and not bad) or 'kuchnia' not in built or len(cellars) < (3 if good else 2) or ('Ratusz' in known and 'ratusz' not in built and not bad) or ('Barak' in known and (homes['barak'] < 2 if good else homes['szalas'] > 3)) or ('Suszarnia' in known and 'suszarnia' not in built) or ('Lampy oliwne' in known and lamps < need_l) or people + 2 > sum(CAP[h] * n for h, n in homes.items())
        J['budowa'] = min(left, 3 if pending else 0); left -= J['budowa']
        rate = FORAGE_AUT[day - 1] if se == 'jesien' else (SNARE if 'Sidla' in known and se == 'zima' else (WINTER_FORAGE if se == 'zima' else SPRING_FORAGE))
        target = 2 if not bad else 0
        if s['food'] / eat < target and rate >= 1:
            gap = (target * eat - s['food']) / 3 + eat - J['grzyb'] * MUSH
            J['zbier'] = min(left, max(0, math.ceil(gap / (rate * 1.25))))
            left -= J['zbier']
        # nadwyżka: jesienią drewno na zimę, potem zbieranie
        if se == 'jesien' and day >= 7 and not bad and s['wood'] < (130 if good else 70):
            extra = min(left, 3 if good else 2); J['drwal'] += extra; left -= extra
        J['zbier'] += max(0, left)
        # budowa
        rbh = J['budowa'] * 10
        want = []
        if not smolarnie and (good or (mid and day >= 2) or (bad and day >= 5)): want.append('smolarnia')
        if 'kuchnia' not in built: want.append('kuchnia')
        if len(cellars) < 1 and day >= 2: want.append('piwnica')
        if good and len(cellars) < 2 and day >= 5: want.append('piwnica')
        if good and len(cellars) < 3 and day >= 11: want.append('piwnica')
        if mid and len(cellars) < 2 and day >= 9: want.append('piwnica')
        if 'Ratusz' in known and 'ratusz' not in built and not bad: want.append('ratusz')
        cap = sum(CAP[h] * n for h, n in homes.items())
        if people + 2 > cap or (good and 'Barak' in known and homes['szalas'] > 1 and day <= 18):
            want.append('barak' if 'Barak' in known else 'szalas')
        if 'Suszarnia' in known and 'suszarnia' not in built: want.append('suszarnia')
        if 'Lampy oliwne' in known and lamps < need_l: want.append('lampa')
        if good and smolarnie == 1 and people >= 14: want.append('smolarnia')
        for b in want:
            w, sc, h = B[b]
            if rbh >= h and s['wood'] >= w and s['scrap'] >= sc:
                rbh -= h; s['wood'] -= w; s['scrap'] -= sc
                if b == 'piwnica': cellars.append(day)
                elif b == 'smolarnia': smolarnie += 1
                elif b == 'szalas': homes['szalas'] += 1
                elif b == 'barak':
                    homes['barak'] += 1
                    if people + 2 <= sum(CAP[k] * n for k, n in homes.items()) - 4 and homes['szalas'] > 0: homes['szalas'] -= 1; s['wood'] += 3
                elif b == 'lampa': lamps += 1
                else: built.add(b)
        while ruins and rbh >= 6:
            r = ruins[0]; need = {'wrak': 6, 'stodola': 20, 'dom': 12}[r]
            if rbh < need: break
            rbh -= need; ruins.pop(0)
            if r == 'wrak': s['scrap'] += 10; s['fuel'] += 2
            elif r == 'stodola': s['wood'] += 25; s['scrap'] += 2
            else: s['wood'] += 15; s['scrap'] += 10
        # badania (starzec 10/dzień, drugi badacz 6/dzień)
        if tq:
            prog += 10 + 6 * res2
            if prog >= tq[0][1]: prog -= tq[0][1]; known.add(tq.pop(0)[0])
        # produkcja
        s['wood'] += J['drwal'] * WOOD[se]
        made = min(J['smolarz'] * TAR, (s['wood'] // 3) * 2); s['wood'] -= made * 1.5; s['fuel'] += made
        food = J['zbier'] * rate + J['grzyb'] * MUSH
        s['food'] += food * (1.25 if J['kucharz'] else 1.0)
        if J['wyprawa']:
            m = (1.0 if visited < 6 else 0.5) * (0.6 if se == 'zima' else 1.0); visited += 1
            s['fuel'] += round(R.randint(5, 11) * m); s['food'] += round(R.randint(4, 10) * m); s['scrap'] += round(R.randint(4, 12) * m)
            if R.random() < (0.5 if se == 'zima' else 0.35): wounded.append(day + 3)
            if R.random() < 0.15: adults += 1
        # przybysze
        p_arr = {'jesien': 0.4 if day >= 6 else 0, 'zima': 0.08, 'wiosna': 0.3}[se]
        if good and day >= 12 and people >= 18 and se != 'wiosna': p_arr = 0   # dobry gracz odmawia przybyszom przed zimą
        if R.random() < p_arr: adults += R.randint(1, 2) if se == 'jesien' else 1
        # latarnia (noc Przypływu)
        if day == flood:
            if s['fuel'] >= lamp_day(4, se, 0): use, flood_loss = 4, 0
            elif s['fuel'] >= lamp_day(3, se, 0): use, flood_loss = 3, 2
            else: use, flood_loss = moc, 5
            s['fuel'] -= lamp_day(use, se, 0)
        else:
            s['fuel'] -= lamp_need
        # jedzenie
        portion = 0.75 if ((good and s['food'] < eat * 2) or (mid and s['food'] < eat)) else 1.0
        s['food'] -= eat * portion
        if se == 'jesien' and 'suszarnia' not in built and s['food'] > 30: s['food'] -= (s['food'] - 30) * 0.03
        s['wood'] -= heat
        dm = 2 if portion == 1 else -5
        if s['food'] < 0:
            s['food'] = 0; zero += 1; dm -= 15
            if zero >= 2: adults -= 1; deaths += 1; dm -= 10
        else: zero = 0
        if s['wood'] < 0:
            s['wood'] = 0; dm -= 8
            if se == 'zima' and R.random() < 0.5: adults -= 1; deaths += 1; dm -= 10
        if s['fuel'] < 0: s['fuel'] = 0; dark += 1; dm -= 20
        if people > sum(CAP[h] * n for h, n in homes.items()): dm -= 4
        if day in NIGHTS:
            s['fuel'] += NIGHTS[day] / 2
            if R.random() < 0.25: wounded.append(day + 3)
            if R.random() < (0.05 if not bad else 0.12): adults -= 1; deaths += 1; dm -= 10
        morale = max(0, min(100, morale + dm))
        rows.append((day, se, adults + children, round(s['fuel'], 1), round(s['food'], 1), round(s['wood']), morale,
                     f"s{homes['szalas']}b{homes['barak']}", len(cellars), smolarnie, lamps, len(known),
                     ' '.join(f"{k[:3]}{v}" for k, v in J.items() if v)))
    if verbose:
        print(f"== {style}, ziarno {seed}, Przypływ dnia {flood} ==")
        print("dzień pora ludzie paliwo racje drewno morale domy piwn smol lampy tech | praca")
        for r in rows: print(*r[:12], '|', r[12])
    return dict(deaths=deaths, dark=dark, people=adults + children, morale=morale, flood_loss=flood_loss,
                min_fuel=min(r[3] for r in rows[15:]), min_food=min(r[4] for r in rows[15:]))

if __name__ == '__main__':
    run('dobry', 1, True)
    for st in ('dobry', 'sredni', 'zly'):
        rs = [run(st, i) for i in range(500)]; n = len(rs)
        print(f"{st}: zgony śr. {sum(r['deaths'] for r in rs)/n:.1f} (gry bez zgonów {sum(r['deaths']==0 for r in rs)*100//n}%), "
              f"ciemność {sum(r['dark']>0 for r in rs)*100//n}%, ludzi na końcu {sum(r['people'] for r in rs)/n:.1f}, "
              f"morale {sum(r['morale'] for r in rs)/n:.0f}, Przypływ bez strat {sum(r['flood_loss']==0 for r in rs)*100//n}%")
