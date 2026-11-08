# Symulacja ekonomii pierwszych 10 dni gry Latarnia (wartości robocze).
import random

SHIFT = 10
DAY_H, NIGHT_H = 14, 10
LAMP = {1: (7, 0.5), 2: (10, 1.0), 3: (13, 2.0), 4: (16, 3.5)}
DAY_FACTOR = 0.5
TAR_IN, TAR_MAX = 2, 4          # smolarnia: 2 drewna -> 1 paliwa, do 4 paliwa/os/dzień
WOOD_PER = 10
FORAGE = [2.5, 2.5, 2, 1.5, 1.5, 1.5, 1, 1, 1, 1]   # zbieractwo/os, okolica się wyczerpuje
MUSH_PER, MUSH_GROW = 3.5, 4
KITCHEN_BONUS = 1.25
HEAT_PER_HUT = 1
EATERS = 9.5
COSTS = {'kuchnia': (8, 4), 'smolarnia': (6, 6), 'piwnica': (10, 0), 'szalas': (6, 2), 'wieza': (10, 4)}
RUINS = {'stodola': (25, 2, 0), 'dom': (15, 10, 0), 'wrak': (0, 10, 2)}
# noce: (siła ataku, tłuszcz po wygranej obronie, szansa rannego)
NIGHTS = {2: ('szczury', 1, 0.2), 4: ('2 ogary', 6, 0.5), 7: ('szczurza fala', 3, 0.4), 9: ('3 ogary + krzykacz', 8, 0.7)}
WORKERS_FULL = 8                # 3 wojowników + 5 dorosłych; starzec bada, dziecko nie pracuje

def lamp_cost(power):
    _, ph = LAMP[power]
    return DAY_H * ph * DAY_FACTOR + NIGHT_H * ph

def run(plan, seed=1, verbose=True, name=''):
    rnd = random.Random(seed)
    s = dict(fuel=20.0, rations=25.0, wood=20, scrap=10)
    built = set(); mush_day = None; huts = 3
    wounded = []      # dni powrotu rannych
    morale = 70; hunger_days = 0; dark_hours = 0; out = []
    for day in range(1, 11):
        wounded = [d for d in wounded if d > day]
        avail = WORKERS_FULL - len(wounded) - 1   # warta nocna = 2 osoby na pół dnia
        p = plan(day, s, built)
        jobs = {k: p.get(k, 0) for k in ('drwale', 'smolarze', 'zbieracze', 'grzybiarze', 'kucharz', 'budowa')}
        used = sum(jobs.values()) + (3 if p.get('wyprawa') else 0)
        if used > avail + 1e-9:
            # nadmiarowe przydziały przepadają: obcinamy od zbieraczy, potem drwali
            over = used - avail
            for k in ('zbieracze', 'drwale', 'grzybiarze', 'smolarze'):
                cut = min(over, jobs[k]); jobs[k] -= cut; over -= cut
        rbh = jobs['budowa'] * SHIFT
        for b in p.get('build', []):
            need = {'kuchnia': 12, 'smolarnia': 14, 'piwnica': 15, 'szalas': 10, 'wieza': 12}[b]
            if rbh >= need and s['wood'] >= COSTS[b][0] and s['scrap'] >= COSTS[b][1]:
                rbh -= need; s['wood'] -= COSTS[b][0]; s['scrap'] -= COSTS[b][1]; built.add(b)
                if b == 'piwnica': mush_day = day
                if b == 'szalas': huts += 1
        for r in p.get('demolish', []):
            need = {'stodola': 20, 'dom': 12, 'wrak': 6}[r]
            if rbh >= need:
                rbh -= need; g = RUINS[r]; s['wood'] += g[0]; s['scrap'] += g[1]; s['fuel'] += g[2]
        s['wood'] += jobs['drwale'] * WOOD_PER
        if 'smolarnia' in built:
            made = min(jobs['smolarze'] * TAR_MAX, s['wood'] // TAR_IN)
            s['wood'] -= made * TAR_IN; s['fuel'] += made
        food = jobs['zbieracze'] * FORAGE[day - 1]
        if mush_day and day >= mush_day + MUSH_GROW: food += jobs['grzybiarze'] * MUSH_PER
        cook = 'kuchnia' in built and jobs['kucharz'] >= 1
        s['rations'] += food * (KITCHEN_BONUS if cook else 1.0)
        if p.get('wyprawa'):
            m = 1.0 if p.get('nowe_miejsce', True) else 0.5
            s['fuel'] += round(rnd.randint(5, 11) * m); s['rations'] += round(rnd.randint(4, 10) * m)
            s['scrap'] += round(rnd.randint(4, 12) * m)
            if rnd.random() < 0.35: wounded.append(day + 3)
        # zużycie
        s['fuel'] -= lamp_cost(p.get('moc', 1))
        portion = p.get('racje', 1.0)
        s['rations'] -= EATERS * portion
        s['wood'] -= huts * HEAT_PER_HUT
        dm = 0
        if portion < 1: dm -= 5 if portion >= 0.75 else 12
        if s['rations'] < 0: dm -= 15; s['rations'] = 0; hunger_days += 1
        if s['fuel'] < 0: dm -= 20; dark_hours += 10; s['fuel'] = 0
        if s['wood'] < 0: dm -= 8; s['wood'] = 0   # zimne noce
        # noc
        if day in NIGHTS:
            n, fat, wr = NIGHTS[day]
            s['fuel'] += fat / 2
            if rnd.random() < wr: wounded.append(day + 3)
        if dm == 0: dm = 2
        morale = max(0, min(100, morale + dm))
        out.append((day, round(s['fuel'], 1), round(s['rations'], 1), s['wood'], s['scrap'], morale, len(wounded)))
    if verbose:
        print(f"== {name} ==\ndzień paliwo racje drewno złom morale ranni")
        for l in out: print(*l)
        print("dni głodu:", hunger_days, "godziny ciemności:", dark_hours)
    return out, hunger_days, dark_hours

def good(day, s, built):
    p = {'moc': 1, 'racje': 1.0, 'kucharz': 1 if 'kuchnia' in built else 0}
    if day == 1:
        p.update(budowa=3, build=['smolarnia', 'kuchnia'], demolish=['wrak'], zbieracze=2, drwale=2)
    elif day == 2:
        p.update(budowa=2, demolish=['wrak', 'wrak'], smolarze=1, drwale=1, zbieracze=2, kucharz=1)
    elif day == 3:
        p.update(budowa=2, build=['piwnica'], smolarze=1, drwale=1, kucharz=1, wyprawa=True)
    elif day % 2 == 0:
        p.update(smolarze=1, drwale=1, grzybiarze=2, kucharz=1, zbieracze=2)
    else:
        p.update(smolarze=1, drwale=1, grzybiarze=1, kucharz=1, wyprawa=True, nowe_miejsce=day <= 7)
    if 3 <= day <= 7: p['racje'] = 0.75
    if day == 6: p.update(budowa=2, demolish=['stodola'], zbieracze=0)
    return p

def bad(day, s, built):
    p = {'moc': 2, 'racje': 1.0, 'drwale': 2, 'zbieracze': 4}
    if day == 5: p.update(budowa=2, build=['smolarnia'])
    if day >= 6: p['smolarze'] = 2
    return p

def mid(day, s, built):   # średni gracz: wyprawy tak, ale smolarnia dzień później i bez racjonowania
    p = good(day, s, built); p['racje'] = 1.0
    if day == 1: p.update(build=['kuchnia'], drwale=1, zbieracze=3)
    if day == 2: p.update(build=['smolarnia'], budowa=3, smolarze=0)
    return p

if __name__ == '__main__':
    print("koszt latarni na dzień:", {k: lamp_cost(k) for k in LAMP})
    for n, pl in (('dobry', good), ('średni', mid), ('zły', bad)):
        run(pl, name=n)
    # odporność dobrego planu na los
    res = [run(good, seed=i, verbose=False) for i in range(200)]
    print("\ndobry plan, 200 losowań: głód >0 dni:", sum(1 for r in res if r[1] > 0),
          " ciemność:", sum(1 for r in res if r[2] > 0),
          " min paliwo:", min(min(x[1] for x in r[0]) for r in res),
          " min racje:", min(min(x[2] for x in r[0]) for r in res))

if __name__ == '__main__':
    for n, pl in (('dobry', good), ('średni', mid), ('zły', bad)):
        res = [run(pl, seed=i, verbose=False) for i in range(500)]
        print(f"{n}: z głodem {sum(1 for r in res if r[1]>0)/5:.0f}% gier, z ciemnością {sum(1 for r in res if r[2]>0)/5:.0f}%, "
              f"śr. min paliwa {sum(min(x[1] for x in r[0]) for r in res)/500:.1f}, śr. min racji {sum(min(x[2] for x in r[0]) for r in res)/500:.1f}, "
              f"śr. morale 10. dnia {sum(r[0][-1][5] for r in res)/500:.0f}")
