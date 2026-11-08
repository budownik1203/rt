using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Latarnia.Sim.AI;
using Latarnia.Sim.Data;
using Latarnia.Sim.Report;
using Latarnia.Sim.Scenarios;

namespace Symulator;

/// <summary>Edytor bitwy: skład (ludzie, klasa, umiejętności, sprzęt, cechy), dowódca, przeciwnik (fale mutantów albo drugi skład),
/// ziarno losowania; start bitwy 3D i szybki przelicznik wielu walk.</summary>
public partial class EditorScreen : Control
{
    public BattleSpec Spec = Presets.Default();
    public Action<BattleSpec> OnFight;
    int _side;          // 0 = skład A, 1 = skład B
    int _sel;           // wybrany wojownik
    VBoxContainer _col1, _col2, _col3;
    ItemList _list;
    Label _benchOut;
    bool _benchRunning;

    static readonly string[] KindNames = { "szczury", "Ogary", "Krzykacz", "Nosiciel" };
    static readonly Kind[] KindVals = { Kind.Rat, Kind.Ogar, Kind.Krzykacz, Kind.Nosiciel };
    static readonly string[] Dirs = { "wschód", "południowy wschód", "południe (brama)", "południowy zachód", "zachód", "północny zachód", "północ", "północny wschód" };
    static readonly string[] ArmorH = { "brak", "garnek", "hełm" };
    static readonly string[] ArmorT = { "brak", "skóra", "kamizelka", "blacha" };
    static readonly string[] Chars = { "spokojny", "ostrożny", "brawurowy", "uparty", "opiekuńczy" };
    static readonly string[] Models = { "(według broni)", "tarczownik", "rebacz", "strzelec_luk", "strzelec_kusza", "sanitariusz", "zwiadowca", "nozownik_sklad", "maczeta_sklad",
                                        "sz_herszt", "sz_rozbijacz", "sz_nozownik", "sz_lucznik", "sz_obdartus", "sz_psiarz" };

    SquadSpec Cur => _side == 0 ? Spec.Squad : (Spec.Enemy ??= Presets.Szabrownicy());

    public override void _Ready()
    {
        SetAnchorsPreset(LayoutPreset.FullRect);
        var bg = new ColorRect { Color = new Color(0.10f, 0.09f, 0.11f) }; AddChild(bg); bg.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        var root = new VBoxContainer(); AddChild(root); root.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect); root.OffsetLeft = 10; root.OffsetRight = -10; root.OffsetTop = 8; root.OffsetBottom = -10;
        root.AddThemeConstantOverride("separation", 8);
        root.AddChild(TopBar());
        var cols = new HBoxContainer { SizeFlagsVertical = SizeFlags.ExpandFill }; cols.AddThemeConstantOverride("separation", 8); root.AddChild(cols);
        _col1 = Column(cols, 340, false); _col2 = Column(cols, 0, true); _col3 = Column(cols, 400, false);
        RebuildAll();
    }

    VBoxContainer Column(HBoxContainer parent, float w, bool expand)
    {
        var p = Ui.Paper(); p.CustomMinimumSize = new Vector2(w, 0); if (expand) p.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        p.SizeFlagsVertical = SizeFlags.ExpandFill;
        parent.AddChild(p);
        var sc = new ScrollContainer { HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled, SizeFlagsVertical = SizeFlags.ExpandFill, SizeFlagsHorizontal = SizeFlags.ExpandFill }; p.AddChild(sc);
        var v = new VBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill }; v.AddThemeConstantOverride("separation", 6); sc.AddChild(v);
        return v;
    }

    Control TopBar()
    {
        var p = Ui.Paper();
        var h = new HBoxContainer(); h.AddThemeConstantOverride("separation", 10); p.AddChild(h);
        h.AddChild(Ui.Title("Latarnia — symulator bitew", 22));
        h.AddChild(new Control { SizeFlagsHorizontal = SizeFlags.ExpandFill });
        var presets = new[] { "Noc Mgły", "Ogary", "Szczurza Fala", "Krzykacz z Ogarami", "Nosiciele", "Szabrownicy atakują osadę", "Starcie w polu z Szabrownikami", "Lustro: ten sam skład, dowódca 3 kontra 1" };
        h.AddChild(Ui.Label("Gotowe:", 14));
        h.AddChild(Ui.Option(presets, -1, i => { LoadPreset(i); RebuildAll(); }));
        h.AddChild(Ui.Label("Ziarno:", 14));
        h.AddChild(Ui.Spin(1, 999999, Spec.Seed, v => Spec.Seed = (ulong)v));
        h.AddChild(Ui.Button("Zapisz", Save, "Zapis do pliku w katalogu użytkownika (bitwy/)."));
        h.AddChild(Ui.Button("Wczytaj", Load));
        h.AddChild(Ui.Button("Przelicz 100 walk", Bench, "Szybko: ta sama bitwa na 100 ziarnach, bez grafiki."));
        var fight = Ui.Button("  Walcz  ", () => OnFight?.Invoke(Spec.Clone()));
        fight.AddThemeFontOverride("font", Ui.Bold); fight.AddThemeFontSizeOverride("font_size", 18);
        h.AddChild(fight);
        return p;
    }

    void LoadPreset(int i)
    {
        ulong seed = Spec.Seed;
        Spec = i switch
        {
            1 => new BattleSpec { Title = "Ogary", Squad = Presets.Mroz(), Waves = Presets.Waves("ogary") },
            2 => new BattleSpec { Title = "Szczurza Fala", Squad = Presets.Mroz(), Waves = Presets.Waves("szczury") },
            3 => new BattleSpec { Title = "Krzykacz z Ogarami", Squad = Presets.Mroz(), Waves = Presets.Waves("krzykacz") },
            4 => new BattleSpec { Title = "Nosiciele", Squad = Presets.Mroz(), Waves = Presets.Waves("nosiciel") },
            5 => new BattleSpec { Title = "Szabrownicy atakują", Squad = Presets.Mroz(), Opponent = OpponentType.Sklad, Enemy = Presets.Szabrownicy(2), Mode = ClashMode.ObronaOsady },
            6 => new BattleSpec { Title = "Starcie w polu", Squad = Presets.Mroz(), Opponent = OpponentType.Sklad, Enemy = Presets.Szabrownicy(2), Mode = ClashMode.Starcie },
            7 => Mirror(),
            _ => Presets.Default(),
        };
        Spec.Seed = seed; _side = 0; _sel = 0;
    }
    static BattleSpec Mirror()
    {
        var b = new BattleSpec { Title = "Lustro", Squad = Presets.Mroz(), Opponent = OpponentType.Sklad, Mode = ClashMode.Starcie, Enemy = Presets.Mroz() };
        b.Enemy.Name = "Skład Lustra"; b.Enemy.Commander.Level = 1; b.Enemy.Commander.Kadry = 1; b.Enemy.Commander.Teren = 1; b.Enemy.Commander.Formacje = 1;
        foreach (var f in b.Enemy.Fighters) f.Name += " II";
        return b;
    }

    void RebuildAll() { BuildCol1(); BuildCol2(); BuildCol3(); }

    static void Clear(Container c) { foreach (var ch in c.GetChildren()) ch.QueueFree(); }

    // ---------------------------------------------------------------- kolumna 1: skład i dowódca
    void BuildCol1()
    {
        Clear(_col1);
        if (Spec.Opponent == OpponentType.Sklad)
        {
            var tabs = new TabBar(); tabs.AddTab("Skład A (gracz)"); tabs.AddTab("Skład B (wróg)"); tabs.CurrentTab = _side;
            tabs.TabChanged += t => { _side = (int)t; _sel = 0; RebuildAll(); };
            _col1.AddChild(tabs);
        }
        var sq = Cur;
        var name = new LineEdit { Text = sq.Name }; name.TextChanged += t => sq.Name = t;
        _col1.AddChild(Ui.Row("Nazwa składu", name, 110));
        _col1.AddChild(Ui.Title("Dowódca", 16));
        var cm = sq.Commander;
        _col1.AddChild(Ui.Row("Kto dowodzi", Ui.Option(sq.Fighters.Select(f => f.Name).ToArray(), cm.Leader, i => cm.Leader = i), 110));
        _col1.AddChild(Ui.Row("Poziom 1–5", Ui.Spin(1, 5, cm.Level, v => cm.Level = (int)v), 110, "1: bez planu, sam instynkt. 2: plan co 3 s, jedna podgrupa. 3: co 2 s, dwie podgrupy, przewidywanie."));
        _col1.AddChild(Ui.Row("Zasoby ludzkie", Ui.Spin(1, 5, cm.Kadry, v => cm.Kadry = (int)v), 110, "Kadry: kto do czego (strzelcy z boku, sanitariusz przy rannych)."));
        _col1.AddChild(Ui.Row("Orientacja w ter.", Ui.Spin(1, 5, cm.Teren, v => cm.Teren = (int)v), 110, "Teren: wybór pozycji, przesmyki, światło."));
        _col1.AddChild(Ui.Row("Taktyka i formacje", Ui.Spin(1, 5, cm.Formacje, v => cm.Formacje = (int)v), 110, "Formacje: szyk, krąg, mur tarcz, obserwacja tyłów."));
        _col1.AddChild(Ui.Row("Charakter", Ui.Option(Chars, (int)cm.Character, i => cm.Character = (Character)i), 110));
        _col1.AddChild(Ui.Row("Flary", Ui.Spin(0, 6, cm.Flares, v => cm.Flares = (int)v), 110));
        _col1.AddChild(Ui.Title("Ludzie", 16));
        _list = new ItemList { CustomMinimumSize = new Vector2(0, 230), SizeFlagsHorizontal = SizeFlags.ExpandFill };
        foreach (var f in sq.Fighters) _list.AddItem($"{f.Name} — {Classes.Get(f.ClassId).NamePl}, {WName(f.Weapon)}");
        if (sq.Fighters.Count > 0) { _sel = Math.Clamp(_sel, 0, sq.Fighters.Count - 1); _list.Select(_sel); }
        _list.ItemSelected += i => { _sel = (int)i; BuildCol2(); };
        _col1.AddChild(_list);
        var hb = new HBoxContainer();
        hb.AddChild(Ui.Button("Dodaj", () => { if (sq.Fighters.Count < 10) { sq.Fighters.Add(new FighterSpec { Name = NewName(sq), ClassId = "rekrut", Weapon = "machete" }); _sel = sq.Fighters.Count - 1; RebuildAll(); } }));
        hb.AddChild(Ui.Button("Kopiuj", () => { if (sq.Fighters.Count is > 0 and < 10) { var c = sq.Fighters[_sel].Clone(); c.Name += "'"; sq.Fighters.Add(c); _sel = sq.Fighters.Count - 1; RebuildAll(); } }));
        hb.AddChild(Ui.Button("Usuń", () => { if (sq.Fighters.Count > 1) { sq.Fighters.RemoveAt(_sel); cm.Leader = Math.Clamp(cm.Leader, 0, sq.Fighters.Count - 1); _sel = Math.Max(0, _sel - 1); RebuildAll(); } }));
        _col1.AddChild(hb);
        var hb2 = new HBoxContainer();
        hb2.AddChild(Ui.Label("Wstaw skład:", 13));
        hb2.AddChild(Ui.Button("Mróz", () => { ReplaceSquad(Presets.Mroz()); }));
        hb2.AddChild(Ui.Button("Szabrownicy", () => { ReplaceSquad(Presets.Szabrownicy()); }));
        _col1.AddChild(hb2);
    }
    void ReplaceSquad(SquadSpec s) { if (_side == 0) Spec.Squad = s; else Spec.Enemy = s; _sel = 0; RebuildAll(); }
    static string NewName(SquadSpec s) { var pool = new[] { "Wiktor", "Gośka", "Staszek", "Irka", "Bolek", "Lena", "Tadek", "Wanda" }; return pool.FirstOrDefault(n => s.Fighters.All(f => f.Name != n)) ?? $"Nowy {s.Fighters.Count + 1}"; }
    static string WName(string id) => Weapons.ById(id)?.NamePl ?? "-";

    void RefreshListItem()
    {
        var f = Cur.Fighters[_sel];
        _list.SetItemText(_sel, $"{f.Name} — {Classes.Get(f.ClassId).NamePl}, {WName(f.Weapon)}");
    }

    // ---------------------------------------------------------------- kolumna 2: wojownik
    void BuildCol2()
    {
        Clear(_col2);
        if (Cur.Fighters.Count == 0) return;
        var f = Cur.Fighters[Math.Clamp(_sel, 0, Cur.Fighters.Count - 1)];
        _col2.AddChild(Ui.Title($"Wojownik: {f.Name}", 18));
        var name = new LineEdit { Text = f.Name }; name.TextChanged += t => { f.Name = t; RefreshListItem(); };
        _col2.AddChild(Ui.Row("Imię", name));
        var classes = Classes.All;
        var cls = Ui.Option(classes.Select(c => $"{Roman(c.Tier)} · {c.NamePl}" + (c.From != null ? $"  (z: {Classes.Get(c.From).NamePl})" : "")).ToArray(), Array.FindIndex(classes, c => c.Id == f.ClassId),
            i => { f.ClassId = classes[i].Id; RefreshListItem(); BuildCol2(); });
        _col2.AddChild(Ui.Row("Klasa", cls));
        var cdef = Classes.Get(f.ClassId);
        var desc = string.Join("\n", Classes.Chain(f.ClassId).Where(c => c.Ability != Ability.None).Select(c => $"• {c.NamePl}: {AbilityPl(c.Ability)}{(c.Cooldown > 0 ? $" (odnowienie {c.Cooldown:0} s)" : " (ciągła)")}"));
        var dl = Ui.Label(cdef.Description + (desc != "" ? "\n" + desc : "\nBez zdolności."), 13, Ui.InkLight); dl.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        _col2.AddChild(dl);
        if (_side == 1 || f.RoleOverride != null)
        {
            var roles = new[] { "(z klasy)", "Herszt", "Łucznik", "Obdartus", "Nożownik", "Rozbijacz" };
            var rv = new Role?[] { null, Role.Herszt, Role.Lucznik, Role.Obdartus, Role.Nozownik, Role.Rozbijacz };
            _col2.AddChild(Ui.Row("Rola w bandzie", Ui.Option(roles, Array.IndexOf(rv, f.RoleOverride), i => f.RoleOverride = rv[i])));
        }
        _col2.AddChild(Ui.Title("Wyposażenie", 16));
        var weps = Weapons.Human;
        _col2.AddChild(Ui.Row("Broń główna", Ui.Option(weps.Select(w => WeaponLine(w)).ToArray(), Array.FindIndex(weps, w => w.Id == f.Weapon), i => { f.Weapon = weps[i].Id; RefreshListItem(); })));
        var sides = new[] { "(brak)" }.Concat(weps.Where(w => !w.IsRanged && !w.TwoHanded && w.Block == 0).Select(w => w.NamePl)).ToArray();
        var sideIds = new string[] { null }.Concat(weps.Where(w => !w.IsRanged && !w.TwoHanded && w.Block == 0).Select(w => w.Id)).ToArray();
        _col2.AddChild(Ui.Row("Broń boczna", Ui.Option(sides, Math.Max(0, Array.IndexOf(sideIds, f.Side)), i => f.Side = sideIds[i]), 130, "Strzelec w zwarciu przechodzi na broń boczną."));
        _col2.AddChild(Ui.Row("Hełm", Ui.Option(ArmorH, f.ArmorHead, i => f.ArmorHead = i)));
        _col2.AddChild(Ui.Row("Pancerz", Ui.Option(ArmorT, f.ArmorTorso, i => f.ArmorTorso = i)));
        _col2.AddChild(Ui.Row("Apteczki", Ui.Spin(0, 6, f.Medkits, v => f.Medkits = (int)v)));
        _col2.AddChild(Ui.Row("Model", Ui.Option(Models, Math.Max(0, Array.IndexOf(Models, f.Model ?? "")), i => f.Model = i == 0 ? null : Models[i])));
        _col2.AddChild(Ui.Title("Umiejętności i statystyki (1–10)", 16));
        var grid = new GridContainer { Columns = 4 }; grid.AddThemeConstantOverride("h_separation", 12);
        void Stat(string label, Func<int> get, Action<int> set, string tip)
        {
            var l = Ui.Label(label, 14); l.TooltipText = tip; l.MouseFilter = MouseFilterEnum.Pass; grid.AddChild(l);
            grid.AddChild(Ui.Spin(1, 10, get(), v => set((int)v)));
        }
        Stat("Celność", () => f.Accuracy, v => f.Accuracy = v, "Trafienie z łuku i kuszy.");
        Stat("Walka wręcz", () => f.Melee, v => f.Melee = v, "Trafienie i obrona wręcz.");
        Stat("Siła", () => f.Strength, v => f.Strength = v, "");
        Stat("Zręczność", () => f.Agility, v => f.Agility = v, "Szybkość ruchu.");
        Stat("Percepcja", () => f.Perception, v => f.Perception = v, "Zasięg wzroku i słuchu, szybkość reakcji.");
        Stat("Nerwy", () => f.Nerve, v => f.Nerve = v, "Odporność na strach, powrót morale.");
        Stat("Wytrzymałość", () => f.Endurance, v => f.Endurance = v, "Zmęczenie w biegu.");
        Stat("Morale (0–100)", () => (int)f.Morale / 10, v => f.Morale = v * 10, "Morale na starcie ×10.");
        _col2.AddChild(grid);
        var bonus = string.Join(", ", new[] { ("Celność", Classes.Chain(f.ClassId).Sum(c => c.Acc)), ("Walka wręcz", Classes.Chain(f.ClassId).Sum(c => c.Melee)), ("Siła", Classes.Chain(f.ClassId).Sum(c => c.Str)),
            ("Zręczność", Classes.Chain(f.ClassId).Sum(c => c.Agi)), ("Percepcja", Classes.Chain(f.ClassId).Sum(c => c.Per)), ("Nerwy", Classes.Chain(f.ClassId).Sum(c => c.Nerve)), ("Wytrzymałość", Classes.Chain(f.ClassId).Sum(c => c.End)) }
            .Where(x => x.Item2 != 0).Select(x => $"{x.Item1} +{x.Item2}"));
        if (bonus != "") _col2.AddChild(Ui.Label($"Premia z klasy: {bonus}", 13, Ui.InkLight));
        _col2.AddChild(Ui.Row("Wyszkolenie taktyczne", Ui.Spin(1, 5, f.Tactical, v => f.Tactical = (int)v), 180, "Dyscyplina, szybkość reakcji, kontrola ognia, reakcja na hałas z boku."));
        _col2.AddChild(Ui.Title("Cechy", 16));
        var tr = new HBoxContainer();
        var b1 = new CheckBox { Text = "Odważny", ButtonPressed = f.Brave }; b1.Toggled += v => f.Brave = v;
        var b2 = new CheckBox { Text = "Tchórz", ButtonPressed = f.Coward }; b2.Toggled += v => f.Coward = v;
        var b3 = new CheckBox { Text = "Litościwy", ButtonPressed = f.Merciful }; b3.Toggled += v => f.Merciful = v;
        tr.AddChild(b1); tr.AddChild(b2); tr.AddChild(b3);
        _col2.AddChild(tr);
    }

    static string WeaponLine(WeaponDef w) => w.IsRanged ? $"{w.NamePl} (zasięg {w.EffRange:0}/{w.MaxRange:0} m, co {w.Interval:0.#} s, amunicja {w.Ammo})"
                                                       : $"{w.NamePl} (zasięg {w.Reach:0.#} m, co {w.Interval:0.##} s, przebicie {w.Penetration}{(w.Block > 0 ? ", tarcza" : "")})";
    static string Roman(int t) => t switch { 1 => "I", 2 => "II", 3 => "III", _ => "IV" };
    static string AbilityPl(Ability a) => a switch
    {
        Ability.Zaslona => "Zasłona: sąsiedzi w szyku dostają mniej ciosów z przodu",
        Ability.TwardySzyk => "Twardy szyk: nie przewraca się, mocniejszy mur tarcz",
        Ability.ScianaGrotow => "Ściana grotów: cios wyprzedzający we wroga wchodzącego w zasięg",
        Ability.Wypad => "Wypad: skok do 3,5 m i cios +30%",
        Ability.Rozbicie => "Rozbicie: cios zdejmuje pancerz, potworowi dodatkowe trafienie",
        Ability.SeriaCiec => "Seria cięć: trzy szybkie ciosy z krwawieniem",
        Ability.OgienCelowany => "Ogień celowany: +15% celności po 2 s bez ruchu",
        Ability.CelnyStrzal => "Celny strzał: +40% celności, +1 przebicia",
        Ability.Wypatrywanie => "Wypatrywanie: +25% zasięgu wzroku",
        Ability.Trop => "Trop: słyszy dalej (×1,5)",
        Ability.CicheZabicie => "Ciche zabicie: od tyłu wyłącza nieświadomego wroga",
        Ability.Stabilizacja => "Stabilizacja: opatruje w 4 s",
        Ability.LeczenieWPolu => "Leczenie w polu: opatrzony krwawi trzy razy wolniej",
        Ability.Wyciagniecie => "Wyciągnięcie: ciągnie rannego prawie bez spowolnienia, połowa trafień chybia",
        _ => ""
    };

    // ---------------------------------------------------------------- kolumna 3: przeciwnik
    void BuildCol3()
    {
        Clear(_col3);
        _col3.AddChild(Ui.Title("Przeciwnik", 18));
        _col3.AddChild(Ui.Row("Rodzaj", Ui.Option(new[] { "Mutanty (fale)", "Skład ludzi" }, (int)Spec.Opponent, i =>
        {
            Spec.Opponent = (OpponentType)i;
            if (Spec.Opponent == OpponentType.Sklad && Spec.Enemy == null) Spec.Enemy = Presets.Szabrownicy();
            if (Spec.Opponent == OpponentType.Mutanty && Spec.Waves.Count == 0) Spec.Waves = Presets.Waves("mieszany");
            _side = 0; _sel = 0; RebuildAll();
        }), 90));
        if (Spec.Opponent == OpponentType.Mutanty)
        {
            var hb = new HBoxContainer();
            hb.AddChild(Ui.Label("Wstaw:", 13));
            foreach (var (lbl, id) in new[] { ("Ogary", "ogary"), ("Szczury", "szczury"), ("Krzykacz", "krzykacz"), ("Nosiciele", "nosiciel"), ("Noc", "mieszany") })
                hb.AddChild(Ui.Button(lbl, () => { Spec.Waves = Presets.Waves(id); BuildCol3(); }));
            _col3.AddChild(hb);
            for (int i = 0; i < Spec.Waves.Count; i++) _col3.AddChild(WaveRow(i));
            _col3.AddChild(Ui.Button("+ fala", () => { Spec.Waves.Add(new WaveSpec { Kind = Kind.Rat, Count = 8, AngleDeg = 90, Delay = 10 }); BuildCol3(); }));
        }
        else
        {
            _col3.AddChild(Ui.Row("Tryb", Ui.Option(new[] { "Skład B atakuje osadę A", "Starcie w polu" }, (int)Spec.Mode, i => Spec.Mode = (ClashMode)i), 90));
            var l = Ui.Label("Skład B edytujesz w zakładce po lewej. Ten sam AI dowódcy co u gracza.", 13, Ui.InkLight); l.AutowrapMode = TextServer.AutowrapMode.WordSmart;
            _col3.AddChild(l);
        }
        _col3.AddChild(Ui.Title("Warunki", 16));
        _col3.AddChild(Ui.Row("Limit czasu (s)", Ui.Spin(60, 900, Spec.MaxTime, v => Spec.MaxTime = (float)v, 30), 130));
        var t = new LineEdit { Text = Spec.Title }; t.TextChanged += s => Spec.Title = s;
        _col3.AddChild(Ui.Row("Tytuł", t, 130));
        _col3.AddChild(Ui.Title("Przelicznik", 16));
        _benchOut = Ui.Label(_benchText ?? "Przelicz 100 walk: wynik pojawi się tutaj.", 13, Ui.InkLight);
        _benchOut.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        _col3.AddChild(_benchOut);
    }
    string _benchText;

    Control WaveRow(int i)
    {
        var w = Spec.Waves[i];
        var box = new VBoxContainer();
        var h1 = new HBoxContainer();
        h1.AddChild(Ui.Option(KindNames, Array.IndexOf(KindVals, w.Kind), k => w.Kind = KindVals[k]));
        h1.AddChild(Ui.Label("×", 14));
        h1.AddChild(Ui.Spin(1, 60, w.Count, v => w.Count = (int)v));
        h1.AddChild(Ui.Button("usuń", () => { Spec.Waves.RemoveAt(i); BuildCol3(); }));
        var h2 = new HBoxContainer();
        int dir = (int)MathF.Round(((w.AngleDeg % 360 + 360) % 360) / 45f) % 8;
        h2.AddChild(Ui.Option(Dirs, dir, d => w.AngleDeg = d * 45f));
        h2.AddChild(Ui.Label("po", 13)); h2.AddChild(Ui.Spin(0, 300, w.Delay, v => w.Delay = (float)v)); h2.AddChild(Ui.Label("s", 13));
        box.AddChild(h1); box.AddChild(h2);
        box.AddChild(new HSeparator());
        return box;
    }

    // ---------------------------------------------------------------- przelicznik, zapis
    public string BenchText => _benchText;
    public void Bench()
    {
        if (_benchRunning) return;
        _benchRunning = true; _benchText = "Liczę 100 walk…"; if (_benchOut != null) _benchOut.Text = _benchText;
        var spec = Spec.Clone();
        Task.Run(() =>
        {
            var reps = new System.Collections.Concurrent.ConcurrentBag<BattleReport>();
            Parallel.For(0, 100, i => { var s = spec.Clone(); s.Seed = spec.Seed + (ulong)i; var b = Scenarios.Build(s); b.Run(); reps.Add(ReportBuilder.Build(b, b.Squads[0])); });
            var r = reps.ToList();
            string top = string.Join("; ", r.SelectMany(x => x.Findings).GroupBy(x => x.Spec).Select(g => (g.Key, g.Sum(x => x.Weight))).OrderByDescending(x => x.Item2).Take(2).Select(x => BattleReport.SpecPl(x.Key)));
            return $"100 walk (ziarna {spec.Seed}–{spec.Seed + 99}):\nwygrane {r.Average(x => x.Won ? 100 : 0):0}%, zabici {r.Average(x => x.Dead):0.00}, wyłączeni {r.Average(x => x.Dead + x.Downed):0.00}, uratowani {r.Average(x => x.Rescued):0.00}, ranni {r.Average(x => x.Wounded):0.00}\n" +
                   $"średni czas {r.Average(x => x.Time):0} s, ogień bratobójczy {r.Average(x => x.FriendlyHits):0.00}\nNajczęstsze wnioski raportów: {top}";
        }).ContinueWith(t => Callable.From(() => { _benchRunning = false; _benchText = t.Result; if (IsInstanceValid(_benchOut)) _benchOut.Text = _benchText; }).CallDeferred());
    }

    void Save()
    {
        var dir = ProjectSettings.GlobalizePath("user://bitwy"); System.IO.Directory.CreateDirectory(dir);
        var safe = string.Concat(Spec.Title.Select(c => char.IsLetterOrDigit(c) ? c : '_'));
        System.IO.File.WriteAllText($"{dir}/{safe}.json", Spec.ToJson());
        _benchText = $"Zapisano: {dir}/{safe}.json"; if (_benchOut != null) _benchOut.Text = _benchText;
    }
    void Load()
    {
        var dir = ProjectSettings.GlobalizePath("user://bitwy"); System.IO.Directory.CreateDirectory(dir);
        var fd = new FileDialog { FileMode = FileDialog.FileModeEnum.OpenFile, Access = FileDialog.AccessEnum.Filesystem, CurrentDir = dir, Filters = new[] { "*.json ; Bitwa" }, Size = new Vector2I(800, 500) };
        fd.FileSelected += p => { try { Spec = BattleSpec.FromJson(System.IO.File.ReadAllText(p)); _side = 0; _sel = 0; RebuildAll(); } catch (Exception e) { _benchText = "Błąd wczytywania: " + e.Message; BuildCol3(); } };
        AddChild(fd); fd.PopupCentered();
    }
}
