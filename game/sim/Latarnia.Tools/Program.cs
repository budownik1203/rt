using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Latarnia.Sim.AI;
using Latarnia.Sim.Combat;
using Latarnia.Sim.Report;
using Latarnia.Sim.Scenarios;
using Latarnia.Sim.Units;
using Latarnia.Sim.World;

// Narzędzia prototypu walki:
//   bench [N] [scenariusz|all]       – N walk na poziom dowódcy (1, 2, 3), tabela wyników
//   replay <scen> <poziom> <seed> <plik.json> – zapis klatek do podglądu z góry (narzedzia/replay_gif.py)
//   report <scen> <poziom> <seed>    – raport po walce
//   diag [N]                         – test diagnozy: słaba jedna specjalizacja, czy raport ją wskaże
var cmd = args.Length > 0 ? args[0] : "bench";
switch (cmd)
{
    case "bench": Bench(args.Length > 1 ? int.Parse(args[1]) : 200, args.Length > 2 ? args[2] : "all"); break;
    case "replay": Replay(args[1], int.Parse(args[2]), ulong.Parse(args[3]), args[4]); break;
    case "report": { var (b, s) = RunOne(args[1], Prof(args[2]), ulong.Parse(args[3]), false); Console.WriteLine(ReportBuilder.Build(b, s)); PrintOrders(b, s, 25); break; }
    case "stuck":
    {
        var (b, s) = RunOne(args[1], Prof(args[2]), ulong.Parse(args[3]), false);
        Console.WriteLine($"{b.EndReason} t={b.Time:0}");
        foreach (var f in b.All.Where(f => f.Active)) Console.WriteLine($"  {f.Name,-10} {f.Team} pos={f.Pos} intent={f.Intent} light={b.Map.Light(f.Pos):0.00} order={f.Order.Type} target={f.TargetId} ammo={f.Ammo} hits={f.HitsTaken} morale={f.Morale:0}");
        break;
    }
    case "why":
    {
        int n = int.Parse(args[3]); var cnt = new Dictionary<string, int>();
        for (int i = 0; i < n; i++)
        {
            var (b, s) = RunOne(args[1], Prof(args[2]), (ulong)(1000 + i), false);
            foreach (var e in b.Log.Events)
            {
                if (e.Target < 0 || b.Get(e.Target).Team != Team.Squad) continue;
                string who = e.Actor >= 0 ? b.Get(e.Actor).Name : "?";
                if (e.Actor >= 0 && b.Get(e.Actor).Team == Team.Squad) who = "SWÓJ";
                string k = e.Kind switch { LogKind.Down => "pada od " + who, LogKind.Wound => "rana od " + who + (e.Text.Contains("wybuch") ? " (wybuch)" : ""), LogKind.Kill => "ginie: " + (e.Text.Contains("(") ? e.Text[(e.Text.LastIndexOf('(') + 1)..^1] : e.Text), _ => "" };
                if (k != "") cnt[k] = cnt.GetValueOrDefault(k) + 1;
            }
        }
        foreach (var kv in cnt.OrderByDescending(kv => kv.Value)) Console.WriteLine($"  {kv.Value / (float)n,6:0.00}  {kv.Key}");
        break;
    }
    case "ff":
        for (int i = 0; i < int.Parse(args[3]); i++)
        {
            var (b, s) = RunOne(args[1], Prof(args[2]), (ulong)(1000 + i), false);
            foreach (var e in b.Log.Events.Where(e => e.Kind == LogKind.FriendlyFire && b.Get(e.Actor).Team == Team.Squad)) Console.WriteLine($"{i,3} {e.T,6:0.0} {e.Text}");
        }
        break;
    case "dec":
    {
        var (b, s) = RunOne(args[1], Prof(args[2]), ulong.Parse(args[3]), false);
        var cs = args.Length > 5 ? b.Squads[int.Parse(args[5])] : s;
        foreach (var d in cs.Commander!.Decisions.Take(int.Parse(args[4]))) Console.WriteLine($"{d.T,6:0.0} znani={d.Known} {(d.Replaced ? "NOWY(" + d.Why + ")" : "")}\n   Z: {d.Threats}\n   P: {d.Tasks}");
        break;
    }
    case "ablate":
    {
        int n = int.Parse(args[1]);
        var profs = new (string name, CommanderProfile p)[]
        {
            ("L2 K2 T2 F2", CommanderProfile.Of(2)), ("L3 K3 T3 F3", CommanderProfile.Of(3)),
            ("L3 K2 T3 F3", new CommanderProfile { Level = 3, Kadry = 2, Teren = 3, Formacje = 3 }),
            ("L3 K3 T2 F3", new CommanderProfile { Level = 3, Kadry = 3, Teren = 2, Formacje = 3 }),
            ("L3 K3 T3 F2", new CommanderProfile { Level = 3, Kadry = 3, Teren = 3, Formacje = 2 }),
            ("L2 K3 T3 F3", new CommanderProfile { Level = 2, Kadry = 3, Teren = 3, Formacje = 3 }),
        };
        foreach (var sc in args[2].Split(','))
            foreach (var (name, p) in profs)
            {
                var reps = new System.Collections.Concurrent.ConcurrentBag<BattleReport>();
                Parallel.For(0, n, i => { var (b, s) = RunOne(sc, p, (ulong)(1000 + i), false); reps.Add(ReportBuilder.Build(b, s)); });
                Console.WriteLine($"{sc,-12} {name}  wygr {reps.Average(r => r.Won ? 100 : 0),5:0}%  zabici {reps.Average(r => r.Dead):0.00}  padli {reps.Average(r => r.Dead + r.Downed):0.00}  ranni {reps.Average(r => r.Wounded):0.00}");
            }
        break;
    }
    case "roles":
    {
        int n = int.Parse(args[3]); var acc = new Dictionary<string, float[]>();
        for (int i = 0; i < n; i++)
        {
            var (b, s) = RunOne(args[1], Prof(args[2]), (ulong)(1000 + i), false);
            if (args.Length > 4) s = b.Squads[int.Parse(args[4])];
            foreach (var m in s.Members)
            {
                if (!acc.TryGetValue(m.Name, out var a)) acc[m.Name] = a = new float[6];
                a[0] += m.Shots; a[1] += m.Kills; a[2] += m.Status is Status.Down or Status.Dead ? 1 : 0; a[3] += m.TimeHolding; a[4] += m.TimeInCombat; a[5] += m.Rescues;
            }
        }
        foreach (var (k, a) in acc) Console.WriteLine($"  {k,-7} strzały {a[0] / n,5:0.0} zabici {a[1] / n,4:0.0} padł {a[2] / n,4:0.00} trzymał {a[3] / n,5:0}s walka {a[4] / n,5:0}s pomoc {a[5] / n,4:0.0}");
        break;
    }
    case "cmp":
    {
        int n = int.Parse(args[1]);
        foreach (var sc in args[2].Split(';'))
        {
            var line = new StringBuilder($"{sc,-12}");
            foreach (var ps in args[3].Split(';'))
            {
                var v = ps.Split(',').Select(int.Parse).ToArray();
                var p = new CommanderProfile { Level = v[0], Kadry = v[1], Teren = v[2], Formacje = v[3] };
                var reps = new System.Collections.Concurrent.ConcurrentBag<BattleReport>();
                Parallel.For(0, n, i => { var (b, s) = RunOne(sc, p, (ulong)(1000 + i), false); reps.Add(ReportBuilder.Build(b, s)); });
                line.Append($" | {ps}: straty {reps.Average(r => r.Dead * 3 + r.Downed + (r.Won ? 0 : 3)):0.00} zab {reps.Average(r => r.Dead):0.00}");
            }
            Console.WriteLine(line);
        }
        break;
    }
    case "diag": Diag(args.Length > 1 ? int.Parse(args[1]) : 100); break;
    default: Console.WriteLine("bench|replay|report|diag"); break;
}

static CommanderProfile Prof(string a) { if (!a.Contains(',')) return CommanderProfile.Of(int.Parse(a)); var v = a.Split(',').Select(int.Parse).ToArray(); return new CommanderProfile { Level = v[0], Kadry = v[1], Teren = v[2], Formacje = v[3] }; }

static (Battle, Squad) RunOne(string scen, CommanderProfile p, ulong seed, bool record)
{
    var b = Scenarios.Create(scen, p, seed, record);
    b.Run();
    return (b, b.Squads[0]);
}

static void PrintOrders(Battle b, Squad s, int max)
{
    Console.WriteLine("Dziennik rozkazów i meldunków:");
    foreach (var e in b.Log.Events.Where(e => e.Kind is LogKind.Order or LogKind.Reply or LogKind.Chaos or LogKind.Scream or LogKind.Explosion or LogKind.Down or LogKind.Rescue).Take(max * 2))
        Console.WriteLine($"  {e.T,6:0.0}  {e.Text}");
}

static void Bench(int n, string which)
{
    var scens = which == "all" ? Scenarios.All.Select(s => s.Id).ToArray() : which.Split(";");
    var sw = Stopwatch.StartNew();
    Console.WriteLine($"{"scenariusz",-12} {"poz",3} {"wygr%",6} {"zabici",7} {"padli",6} {"uratow",7} {"ranni",6} {"czas",6} {"FF",5} {"trafne",7} {"spózn",6} {"brak",5} {"zaskocz",7}");
    var csv = new StringBuilder("scen,level,win,dead,down,rescued,wounded,time,ff,acc,late,missed,surprise\n");
    foreach (var sc in scens)
        foreach (int lvl in new[] { 1, 2, 3 })
        {
            var reps = new List<BattleReport>();
            var lockObj = new object();
            Parallel.For(0, n, i =>
            {
                var (b, s) = RunOne(sc, CommanderProfile.Of(lvl), (ulong)(1000 + i), false);
                var r = ReportBuilder.Build(b, s);
                lock (lockObj) reps.Add(r);
            });
            double win = reps.Average(r => r.Won ? 1 : 0) * 100, dead = reps.Average(r => r.Dead), down = reps.Average(r => r.Downed + r.Dead), resc = reps.Average(r => r.Rescued),
                   wnd = reps.Average(r => r.Wounded), time = reps.Average(r => r.Time), ff = reps.Average(r => r.FriendlyHits);
            double acc = reps.Average(r => r.Accurate), late = reps.Average(r => r.Late), miss = reps.Average(r => r.Missed), sur = reps.Average(r => r.Surprise);
            Console.WriteLine($"{sc,-12} {lvl,3} {win,6:0.0} {dead,7:0.00} {down,6:0.00} {resc,7:0.00} {wnd,6:0.00} {time,6:0} {ff,5:0.00} {acc,7:0.0} {late,6:0.0} {miss,5:0.0} {sur,7:0.0}");
            csv.AppendLine(FormattableString.Invariant($"{sc},{lvl},{win:0.0},{dead:0.000},{down:0.000},{resc:0.000},{wnd:0.000},{time:0.0},{ff:0.000},{acc:0.00},{late:0.00},{miss:0.00},{sur:0.00}"));
        }
    File.WriteAllText("bench.csv", csv.ToString());
    Console.WriteLine($"czas: {sw.Elapsed.TotalSeconds:0.0} s");
}

static void Diag(int n)
{
    // poziom 3, jedna specjalizacja na 1, reszta na 4: czy raport wskaże właśnie ją częściej niż inne?
    var specs = new Spec?[] { null, Spec.Kadry, Spec.Teren, Spec.Formacje };
    foreach (var weak in specs)
    {
        var tally = new Dictionary<string, int>(); float cas = 0; int runs = 0;
        foreach (var sc in new[] { "ogary", "krzykacz", "mieszany", "szczury" })
            for (int i = 0; i < n; i++)
            {
                var p = new CommanderProfile { Level = 3, Kadry = weak == Spec.Kadry ? 1 : 4, Teren = weak == Spec.Teren ? 1 : 4, Formacje = weak == Spec.Formacje ? 1 : 4 };
                var (b, s) = RunOne(sc, p, (ulong)(5000 + i), false);
                var r = ReportBuilder.Build(b, s); cas += r.Dead * 3 + r.Downed + (r.Won ? 0 : 3); runs++;
                var top = r.Blame.Where(kv => kv.Key != Spec.Wyszkolenie && kv.Key != Spec.Poziom).OrderByDescending(kv => kv.Value).FirstOrDefault();
                string k = top.Value < 0.5f ? "brak" : top.Key.ToString();
                tally[k] = tally.GetValueOrDefault(k) + 1;
            }
        Console.WriteLine($"słaba {(weak?.ToString() ?? "żadna"),-9}: straty {cas / runs:0.00}; raport wskazuje → {string.Join(", ", tally.OrderByDescending(kv => kv.Value).Select(kv => $"{kv.Key} {kv.Value}"))}");
    }
}

static void Replay(string scen, int lvl, ulong seed, string path)
{
    var (b, s) = RunOne(scen, CommanderProfile.Of(lvl), seed, true);
    var g = b.Map;
    var rows = new List<string>();
    for (int y = 0; y < g.H; y++)
    {
        var sb = new StringBuilder();
        for (int x = 0; x < g.W; x++)
        {
            var c = g.Get(x, y);
            sb.Append((c & Cell.Blocked) != 0 ? '#' : (c & Cell.LowCover) != 0 ? 'c' : (c & Cell.Rough) != 0 ? 'r' : '.');
        }
        rows.Add(sb.ToString());
    }
    var light = new List<string>();
    for (int y = 0; y < g.H; y++) { var sb = new StringBuilder(); for (int x = 0; x < g.W; x++) sb.Append((char)('a' + (int)(Math.Clamp(g.Light(g.CellCenter(x, y)), 0, 1) * 15))); light.Add(sb.ToString()); }
    var report = ReportBuilder.Build(b, s);
    var obj = new
    {
        scenario = scen, level = lvl, seed, cell = Grid.CellSize, w = g.W, h = g.H, lamp = new[] { g.LampPos.X, g.LampPos.Y }, radius = g.LampRadius,
        map = rows, light, frames = b.Log.Frames,
        events = b.Log.Events.Select(e => new { t = e.T, k = e.Kind.ToString(), e.Text }),
        report = report.ToString()
    };
    File.WriteAllText(path, JsonSerializer.Serialize(obj, new JsonSerializerOptions { IncludeFields = true }));
    Console.WriteLine(report);
    PrintOrders(b, s, 30);
}
