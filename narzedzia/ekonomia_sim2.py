# Latarnia: symulacja ekonomii dni 1-36 (jesień, pierwsza zima, pierwszy Przypływ). Wersja 2 (8.11.2026).
# Zastępuje ekonomia_sim.py i ekonomia_zima_sim.py. Jeden zestaw liczb = tabela LICZBY niżej.
# Uruchom: python3 ekonomia_sim2.py            (statystyki 500 gier na styl)
#          python3 ekonomia_sim2.py dobry 7    (jeden przebieg ze szczegółami)
import random, sys, math

# ---------------- LICZBY (jedyne źródło prawdy dla konceptu) ----------------
L = dict(
    pora=lambda d: 'jesien' if d <= 15 else ('zima' if d <= 30 else 'wiosna'),
    godziny={'jesien': (14, 10), 'zima': (10, 14), 'wiosna': (12, 12)},   # dzień, noc
    latarnia_h={1: 0.5, 2: 1.0, 3: 2.0, 4: 3.5},     # paliwo na godzinę nocy
    latarnia_dzien=0.25,                             # w dzień moc kosztuje 25%
    promien={1: 7, 2: 10, 3: 13, 4: 16},             # kratki po 2 m
    drwal={'jesien': 12, 'zima': 8, 'wiosna': 10},
    smolarz=(3, 2, 6),            # 3 drewna -> 2 paliwa, max 6 paliwa na smolarza
    zbieranie=[2.5, 2.5, 2, 1.5, 1.5, 1.5, 1, 1, 1, 1, 1, 1, 1, 1, 1],  # jesień, okolica się wyczerpuje
    zbieranie_zima=0.3, sidla_zima=1.5, zbieranie_wiosna=1.0,
    grzyby=3.5, grzyby_start=4, piwnica_miejsc=3,
    kuchnia=1.25,
    ogrzewanie={'szalas': {'jesien': 1, 'zima': 3, 'wiosna': 1}, 'barak': {'jesien': 1, 'zima': 2, 'wiosna': 1}},
    miejsca={'szalas': 4, 'barak': 8},
    woda_os=1.0, pompa=12, beczki=4, studnia=20,
    budynki={  # drewno, złom, roboczogodziny
        'smolarnia': (6, 6, 14), 'kuchnia': (8, 4, 12), 'piwnica': (10, 0, 15), 'szalas': (6, 2, 10),
        'barak': (16, 6, 24), 'beczki': (0, 3, 3), 'pompa': (0, 4, 4), 'studnia': (4, 0, 20),
        'przytulek': (6, 2, 10), 'komisja': (8, 2, 10), 'pracownia': (8, 4, 12)},
    ruiny={'wrak': (6, dict(zlom=10, paliwo=2)), 'dom': (12, dict(drewno=15, zlom=10)),
           'stodola': (20, dict(drewno=25, zlom=2))},
    ruiny_start=['wrak', 'wrak', 'wrak', 'dom', 'stodola'],
    badania=[('Studnia', 50), ('Sidla', 45), ('Barak', 75), ('Lampy', 45)],   # punkty; starzec daje 10/dzień
    wyprawa=dict(ludzi=3, paliwo=(5, 11), racje=(4, 10), zlom=(4, 12), ranny=0.35, ocalaly=0.15, miejsc=6),
    noce={2: 1, 4: 2, 7: 2, 9: 3, 12: 2, 14: 3, 18: 3, 21: 4, 24: 3, 27: 4, 29: 3, 33: 3, 35: 4},  # siła ataku
    przybysze={'jesien': 0.35, 'zima': 0.08, 'wiosna': 0.30},   # szansa dziennie przy mocy 1 i świetle w nocy
)

# ---------------- OŚ OPIEKA <-> SELEKCJA ----------------
OS = {
    'opieka':   dict(jedzenie=1.10, wydajnosc=1.00, morale_dzien=+0.5, smierc_w_walce=0.7, ucieczki=0.7),
    'srodek':   dict(jedzenie=1.00, wydajnosc=1.00, morale_dzien=0,  smierc_w_walce=1.0, ucieczki=1.0),
    'selekcja': dict(jedzenie=0.95, wydajnosc=1.10, morale_dzien=-0.5, smierc_w_walce=1.2, ucieczki=1.5),
}

STYLE = {
    # dobry: smolarnia i kuchnia od razu, racjonowanie przy niskich zapasach, drewno na zimę, wyprawy z głową
    'dobry':  dict(opoznienie=0, racjonuje=True, zapas_drewna=110, zapas_dni=3, wyprawy=True, moc_max=1, os='srodek'),
    'opieka': dict(opoznienie=0, racjonuje=True, zapas_drewna=110, zapas_dni=3, wyprawy=True, moc_max=1, os='opieka'),
    'selekcja': dict(opoznienie=0, racjonuje=True, zapas_drewna=110, zapas_dni=3, wyprawy=True, moc_max=1, os='selekcja'),
    # średni: wszystko o 2 dni później, bez racjonowania, mniejszy zapas drewna
    'sredni': dict(opoznienie=2, racjonuje=False, zapas_drewna=50, zapas_dni=1, wyprawy=True, moc_max=1, os='srodek'),
    # zły: latarnia na mocy 2 dla miejsca, smolarnia 5. dnia, bez wypraw, bez zapasów na zimę
    'zly':    dict(opoznienie=4, racjonuje=False, zapas_drewna=0, zapas_dni=0, wyprawy=False, moc_max=2, os='srodek'),
}

def run(styl, seed=0, verbose=False):
    st = STYLE[styl]; ax = OS[st['os']]; R = random.Random(seed)
    s = dict(paliwo=20.0, racje=25.0, drewno=20.0, zlom=10.0, woda=15.0)
    dorosli, dzieci = 9, 1                      # w tym starzec-badacz
    domy = {'szalas': 3, 'barak': 0}
    zbud = set(); piwnice = []; smolarnie = 0
    ruiny = list(L['ruiny_start']); znane = set(); postep = 0; kolejka = list(L['badania'])
    ranni = []; zgony = 0; ucieczki = 0; dni_glodu = 0; godz_ciemn = 0; morale = 70; odwiedzone = 0
    ciemno_wczoraj = False; przyplyw = R.randint(31, 36); straty_przyplyw = 0
    log = []
    for d in range(1, 37):
        pora = L['pora'](d)
        ranni = [x for x in ranni if x > d]
        ludzie = dorosli + dzieci
        je = (dorosli + 0.5 * dzieci) * ax['jedzenie']
        # praca: starzec bada, warta = 1 osoba (2 osoby po pół zmiany), ranni nie pracują
        prac = max(0, dorosli - 1 - 1 - len(ranni))
        if morale < 25: prac = int(prac * 0.8)                 # strajk / ociąganie się
        J = dict(wyprawa=0, kucharz=0, grzyby=0, smolarz=0, drwal=0, zbier=0, budowa=0)
        wolni = prac
        moc = 1 if st['moc_max'] == 1 else 2
        lampa = lambda m: (L['godziny'][pora][0] * L['latarnia_h'][m] * L['latarnia_dzien'] + L['godziny'][pora][1] * L['latarnia_h'][m])
        potrzeba_paliwa = lampa(moc)
        ogrz = sum(n * L['ogrzewanie'][h][pora] for h, n in domy.items())
        if pora == 'jesien': rate = L['zbieranie'][d - 1]
        elif pora == 'zima': rate = L['sidla_zima'] if 'Sidla' in znane else L['zbieranie_zima']
        else: rate = L['zbieranie_wiosna']
        gotowe = sum(1 for p in piwnice if d >= p + L['grzyby_start'])
        # --- budowa: kolejka według stylu ---
        do_budowy = []
        op = st['opoznienie']
        if not smolarnie and d >= 1 + op: do_budowy.append('smolarnia')
        if 'kuchnia' not in zbud and d >= 1 + op // 2: do_budowy.append('kuchnia')
        if 'pompa' not in zbud: do_budowy.append('pompa')
        if len(piwnice) < 1 and d >= 2 + op: do_budowy.append('piwnica')
        if len(piwnice) < 2 and d >= 6 + op: do_budowy.append('piwnica')
        if st['os'] == 'opieka' and 'przytulek' not in zbud and d >= 3: do_budowy.append('przytulek')
        if st['os'] == 'selekcja' and 'komisja' not in zbud and d >= 3: do_budowy.append('komisja')
        miejsc = sum(L['miejsca'][h] * n for h, n in domy.items())
        if 'Barak' in znane and op == 0 and domy['szalas'] >= 2 and domy['barak'] < 2 and pora != 'wiosna': do_budowy.append('barak')   # dobry gracz zamienia szałasy na baraki przed zimą
        elif ludzie + 1 > miejsc: do_budowy.append('barak' if 'Barak' in znane else 'szalas')
        if 'Studnia' in znane and 'studnia' not in zbud: do_budowy.append('studnia')
        if 'beczki' not in zbud and d >= 4: do_budowy.append('beczki')
        if smolarnie == 1 and ludzie >= 14 and op == 0: do_budowy.append('smolarnia')
        # --- przydział: zawsze tam, gdzie zapasu starczy na najkrócej (względem celu) ---
        cel = 1.0 + st['zapas_dni']
        zima_blisko = pora == 'jesien' and d >= 6
        cel_paliwa = cel + (st['zapas_drewna'] / 20 if zima_blisko else 0)   # dobry gracz gromadzi paliwo i drewno na zimę
        cel_drewna = cel + (st['zapas_drewna'] / 15 if zima_blisko else 0)
        if d >= 24 and st['zapas_dni'] >= 3: cel_paliwa = max(cel_paliwa, 55 / potrzeba_paliwa)   # dobry gracz odkłada paliwo na noc Przypływu
        if st['wyprawy'] and d >= 3 and d % (3 if pora != 'zima' else 4) == 0 and wolni >= 6 and s['racje'] > je:
            J['wyprawa'] = 3; wolni -= 3
        if 'kuchnia' in zbud and wolni: J['kucharz'] = 1; wolni -= 1
        if do_budowy or ruiny:
            b = min(wolni, 3 if (op == 0 or d > 3) else 2) if (s['racje'] > je * 1.5 and s['paliwo'] > potrzeba_paliwa) else min(wolni, 1)
            J['budowa'] = b; wolni -= b
        kuch = L['kuchnia'] if J['kucharz'] else 1.0
        for _ in range(wolni):
            dz = lambda v, potrz: v / max(potrz, 0.1)
            smola_drewno = J['smolarz'] * L['smolarz'][2] * L['smolarz'][0] / L['smolarz'][1]
            prog_jedz = s['racje'] + (J['grzyby'] * L['grzyby'] + J['zbier'] * rate) * kuch
            prog_pal = s['paliwo'] + J['smolarz'] * L['smolarz'][2]
            prog_drw = s['drewno'] + J['drwal'] * L['drwal'][pora] - smola_drewno
            opcje = []
            opcje.append((dz(prog_jedz, je) / cel, 'jedzenie'))
            if smolarnie and J['smolarz'] < 2 * smolarnie: opcje.append((dz(prog_pal, potrzeba_paliwa) / cel_paliwa, 'smolarz'))
            opcje.append((dz(prog_drw, ogrz + smola_drewno + L['smolarz'][2] * 1.5 * (J['smolarz'] < 2 * smolarnie and smolarnie > 0)) / cel_drewna, 'drwal'))
            _, wyb = min(opcje)
            if wyb == 'jedzenie':
                if J['grzyby'] < gotowe * L['piwnica_miejsc']: J['grzyby'] += 1
                else: J['zbier'] += 1
            else: J[wyb] += 1
        wyd = ax['wydajnosc']
        # --- budowa i rozbiórka ---
        rbh = J['budowa'] * 10 * wyd
        for b in do_budowy:
            w, z, h = L['budynki'][b]
            if rbh >= h and s['drewno'] >= w and s['zlom'] >= z:
                rbh -= h; s['drewno'] -= w; s['zlom'] -= z
                if b == 'piwnica': piwnice.append(d)
                elif b == 'smolarnia': smolarnie += 1
                elif b == 'barak':
                    domy['barak'] += 1
                    zostaw = max(0, math.ceil((ludzie + 1 - sum(L['miejsca'][h] * n for h, n in domy.items() if h == 'barak')) / 4))
                    while domy['szalas'] > zostaw: domy['szalas'] -= 1; s['drewno'] += 3   # rozbiórka szałasu
                elif b == 'szalas': domy['szalas'] += 1
                else: zbud.add(b)
        while ruiny and rbh >= L['ruiny'][ruiny[0]][0]:      # limit ruin: tylko to, co stoi w promieniu
            r = ruiny.pop(0); rbh -= L['ruiny'][r][0]
            for k, v in L['ruiny'][r][1].items(): s[k] += v
        # --- badania ---
        if kolejka:
            postep += 10
            if postep >= kolejka[0][1]: postep -= kolejka[0][1]; znane.add(kolejka.pop(0)[0])
        # --- produkcja ---
        s['drewno'] += J['drwal'] * L['drwal'][pora] * wyd
        mozna = min(J['smolarz'] * L['smolarz'][2] * wyd, s['drewno'] // L['smolarz'][0] * L['smolarz'][1])
        s['drewno'] -= mozna * L['smolarz'][0] / L['smolarz'][1]; s['paliwo'] += mozna
        jedz = (J['zbier'] * rate + J['grzyby'] * L['grzyby']) * wyd
        s['racje'] += jedz * (L['kuchnia'] if J['kucharz'] else 1.0)
        s['woda'] += (L['pompa'] if 'pompa' in zbud else 0) + (L['beczki'] if 'beczki' in zbud else 0) + (L['studnia'] if 'studnia' in zbud else 0)
        if J['wyprawa']:
            w = L['wyprawa']; m = (1.0 if odwiedzone < w['miejsc'] else 0.5) * (0.6 if pora == 'zima' else 1.0); odwiedzone += 1
            s['paliwo'] += round(R.randint(*w['paliwo']) * m); s['racje'] += round(R.randint(*w['racje']) * m); s['zlom'] += round(R.randint(*w['zlom']) * m)
            if R.random() < w['ranny'] * (1.4 if pora == 'zima' else 1): ranni.append(d + 3)
            if R.random() < w['ocalaly']: dorosli += 1
        # --- przybysze: tylko do światła, nie po nocy w ciemności ---
        if not ciemno_wczoraj and ludzie < 20:
            p = L['przybysze'][pora] * (1 + 0.3 * (moc - 1))
            if R.random() < p: dorosli += 1
        # --- zużycie ---
        dm = 0
        if d == przyplyw:
            for m in (4, 3, moc):
                if s['paliwo'] >= lampa(m): break
            straty_przyplyw = {4: 0, 3: 2}.get(m, 5); s['paliwo'] -= lampa(m) if s['paliwo'] >= lampa(m) else s['paliwo']
            if straty_przyplyw: dm -= 10
        else:
            s['paliwo'] -= potrzeba_paliwa
        porcja = 0.75 if (st['racjonuje'] and s['racje'] < je * st['zapas_dni']) else 1.0
        s['racje'] -= je * porcja
        if porcja < 1: dm -= 2
        s['woda'] -= ludzie * L['woda_os']
        s['drewno'] -= ogrz
        ciemno_wczoraj = False
        if s['racje'] < 0:
            s['racje'] = 0; dni_glodu += 1; dm -= 15
            if dni_glodu >= 2 and dorosli > 1: dorosli -= 1; zgony += 1; dm -= 10
        else: dni_glodu = 0
        if s['woda'] < 0: s['woda'] = 0; dm -= 10
        if s['drewno'] < 0:
            s['drewno'] = 0; dm -= 8
            if pora == 'zima' and R.random() < 0.4 and dorosli > 1: dorosli -= 1; zgony += 1; dm -= 10
        if s['paliwo'] < 0:
            s['paliwo'] = 0; godz_ciemn += 10; dm -= 20; ciemno_wczoraj = True
        if ludzie > sum(L['miejsca'][h] * n for h, n in domy.items()): dm -= 4
        # --- noc: atak (więcej wrogów przy mocniejszym świetle i w ciemności) ---
        if d in L['noce']:
            sila = L['noce'][d] + (moc - 1) + (2 if ciemno_wczoraj else 0)
            s['paliwo'] += sila                          # tłuszcz z mutantów, wytopiony w kuchni
            if R.random() < 0.08 * sila: ranni.append(d + 3)
            if R.random() < 0.025 * sila * ax['smierc_w_walce'] and dorosli > 1: dorosli -= 1; zgony += 1; dm -= 10
        # --- morale i ucieczki ---
        dm += 1 if dm == 0 else 0
        dm += ax['morale_dzien']
        morale = max(0, min(100, morale + dm))
        if morale < 10 and R.random() < 0.15 * ax['ucieczki'] and dorosli > 1: dorosli -= 1; ucieczki += 1
        log.append((d, pora, dorosli + dzieci, round(s['paliwo'], 1), round(s['racje'], 1), round(s['drewno']), round(s['woda']), morale,
                    ' '.join(f"{k[:3]}{v}" for k, v in J.items() if v)))
    if verbose:
        print(f"== {styl}, ziarno {seed}, Przypływ dnia {przyplyw} ==\ndzień pora ludzie paliwo racje drewno woda morale | praca")
        for r in log: print(*r[:8], '|', r[8])
        print(f"zgony {zgony}, ucieczki {ucieczki}, godziny ciemności {godz_ciemn}, straty w Przypływie {straty_przyplyw}")
    zima = [r for r in log if r[1] == 'zima']
    return dict(zgony=zgony, ucieczki=ucieczki, ciemnosc=godz_ciemn > 0, ludzie=dorosli + dzieci, morale=morale,
                przyplyw_ok=straty_przyplyw == 0, zapasy=s['paliwo'] + s['racje'] + s['drewno'] / 2 + s['zlom'], min_drewno_zima=min(r[5] for r in zima))

if __name__ == '__main__':
    if len(sys.argv) >= 2:
        run(sys.argv[1], int(sys.argv[2]) if len(sys.argv) > 2 else 1, True); sys.exit()
    print("styl      zgony śr.  bez zgonów  ucieczki  ciemność  ludzi na końcu  morale  Przypływ bez strat  zapasy 36. dnia")
    for st in ('dobry', 'opieka', 'selekcja', 'sredni', 'zly'):
        rs = [run(st, i) for i in range(500)]; n = len(rs)
        print(f"{st:9s} {sum(r['zgony'] for r in rs)/n:8.2f} {sum(r['zgony']==0 for r in rs)*100//n:9d}% "
              f"{sum(r['ucieczki'] for r in rs)/n:9.2f} {sum(r['ciemnosc'] for r in rs)*100//n:8d}% "
              f"{sum(r['ludzie'] for r in rs)/n:15.1f} {sum(r['morale'] for r in rs)/n:7.0f} {sum(r['przyplyw_ok'] for r in rs)*100//n:12d}% {sum(r['zapasy'] for r in rs)/n:14.0f}")
