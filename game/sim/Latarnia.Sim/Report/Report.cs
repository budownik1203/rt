using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Latarnia.Sim.AI;
using Latarnia.Sim.Combat;
using Latarnia.Sim.Data;
using Latarnia.Sim.Units;

namespace Latarnia.Sim.Report;

public enum Spec { Poziom, Kadry, Teren, Formacje, Wyszkolenie }

public sealed class Finding
{
    public Spec Spec; public float Weight; public string Text = "";
}

public sealed class BattleReport
{
    public string Outcome = ""; public float Time;
    public int Dead, Downed, Rescued, Wounded, Unhurt, Fled, FriendlyHits, EnemiesKilled, EnemiesTotal, Screams, BlastHits;
    public readonly Dictionary<string, int> KilledByKind = new();
    public readonly List<string> People = new();
    public readonly List<Finding> Findings = new();
    public readonly Dictionary<Spec, float> Blame = new();
    public readonly List<string> Moments = new();
    public int Accurate, Late, Missed, Surprise;   // ocena reakcji na zagrożenia, które zadały rany
    public bool Won;
    public Spec? TopBlame => Blame.Count == 0 || Blame.Values.Max() < 1f ? null : Blame.OrderByDescending(kv => kv.Value).First().Key;

    public static string SpecPl(Spec s) => s switch
    {
        Spec.Poziom => "poziom dowódcy (tempo decyzji)", Spec.Kadry => "Zasoby ludzkie (kto do czego)", Spec.Teren => "Orientacja w terenie (pozycje)",
        Spec.Formacje => "Taktyka i formacje (szyk i sektory)", _ => "wyszkolenie składu"
    };

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"RAPORT: {Outcome} po {Time:0} s");
        sb.AppendLine($"Skład: {Unhurt} cali, {Wounded} ranni, {Rescued} wyniesieni lub opatrzeni, {Dead} zabici{(Fled > 0 ? $", {Fled} uciekło" : "")}");
        sb.AppendLine($"Wrogowie: {EnemiesKilled}/{EnemiesTotal} pokonani ({string.Join(", ", KilledByKind.Select(kv => $"{kv.Key} {kv.Value}"))})");
        sb.AppendLine($"Reakcje dowódcy na zagrożenia, które raniły ludzi: {Accurate} trafne, {Late} spóźnione, {Missed} bez reakcji, {Surprise} zaskoczenia");
        if (FriendlyHits > 0) sb.AppendLine($"Ogień bratobójczy: {FriendlyHits}");
        sb.AppendLine("Ludzie:");
        foreach (var p in People) sb.AppendLine("  " + p);
        if (Moments.Count > 0) { sb.AppendLine("Kluczowe chwile:"); foreach (var m in Moments) sb.AppendLine("  " + m); }
        sb.AppendLine("Wnioski:");
        foreach (var f in Findings.OrderByDescending(f => f.Weight).Take(4)) sb.AppendLine($"  [{SpecPl(f.Spec)}] {f.Text}");
        if (Findings.Count == 0) sb.AppendLine("  Bez zastrzeżeń.");
        return sb.ToString();
    }
}

/// <summary>Raport po walce: co się stało i dlaczego. Każda rana składu jest przypisana do decyzji dowódcy (albo jej braku).</summary>
public static class ReportBuilder
{
    public static BattleReport Build(Battle b, Squad s)
    {
        var r = new BattleReport { Time = b.Time };
        var c = s.Commander!;
        r.Won = b.Winner == s.Team;
        r.Outcome = b.Winner == s.Team ? "obrona udana" : b.Winner == null ? "walka nierozstrzygnięta" : "skład rozbity";
        var enemies = b.All.Where(f => f.Team != s.Team).ToList();
        r.EnemiesTotal = enemies.Count; r.EnemiesKilled = enemies.Count(e => e.Status == Status.Dead || e.Status == Status.Fled);
        foreach (var g in enemies.Where(e => e.Status is Status.Dead or Status.Fled).GroupBy(e => e.Kind == Kind.Human ? "ludzie" : Phrases.KindPl(e.Kind))) r.KilledByKind[g.Key] = g.Count();
        foreach (var m in s.Members)
        {
            string st = m.Status switch
            {
                Status.Dead => m.DownAt >= 0 ? $"zginął (padł w {m.DownAt:0} s)" : "zginął",
                Status.Down => m.Stabilized ? "ciężko ranny, uratowany" : "ciężko ranny",
                Status.Fled => "uciekł", _ => m.Wounds.Count > 0 ? $"ranny ({m.Wounds.Count})" : "cały"
            };
            if (m.Status == Status.Dead) r.Dead++; else if (m.Status == Status.Down) { r.Downed++; if (m.Stabilized) r.Rescued++; }
            else if (m.Status == Status.Fled) r.Fled++; else if (m.Wounds.Count > 0) r.Wounded++; else r.Unhurt++;
            r.FriendlyHits += m.FriendlyHits;
            r.People.Add($"{m.Name} ({RolePl(m.Role)}): {st}; zabici {m.Kills}, pomoc {m.Rescues}{(m.FriendlyHits > 0 ? $", trafił swoich {m.FriendlyHits}" : "")}");
        }
        r.Screams = b.Log.Events.Count(e => e.Kind == LogKind.Scream);
        void Blame(Spec sp, float w, string text)
        {
            r.Blame[sp] = r.Blame.GetValueOrDefault(sp) + w;
            var ex = r.Findings.FirstOrDefault(f => f.Text == text);
            if (ex != null) ex.Weight += w; else r.Findings.Add(new Finding { Spec = sp, Weight = w, Text = text });
        }

        // 1. każda rana: czy dowódca wiedział i zareagował na czas?
        var counted = new HashSet<int>();
        foreach (var e in b.Log.Events.Where(e => e.Kind is LogKind.Wound or LogKind.Down))
        {
            if (e.Target < 0 || !b.TryGet(e.Target, out var victim) || victim.Team != s.Team || e.Actor < 0) continue;
            if (!b.TryGet(e.Actor, out var atk) || atk.Team == s.Team) continue;
            float w = e.Kind == LogKind.Down ? 3f : e.Text.Contains("ciężka") || e.Text.Contains("śmiertelna") ? 2f : 1f;
            bool first = counted.Add(atk.Id);
            float tKnown = c.FirstKnown.TryGetValue(atk.Id, out var tk) ? tk : float.MaxValue;
            float tHandled = c.FirstHandled.TryGetValue(atk.Id, out var th) ? th : float.MaxValue;
            if (c.Original.Level <= 1)
            {
                if (first) r.Missed++;
                Blame(Spec.Poziom, w, "Nikt nie dowodził: każdy walczył na własną rękę, bez podziału na zadania.");
                continue;
            }
            if (tKnown > e.T - 0.3f)
            {
                if (first) r.Surprise++;
                bool flank = !c.Decisions.Any(d => d.T < e.T && d.TaskList.Any(t => t.type == TaskType.Watch));
                if (flank && victim.Order.Type != OrderType.Guard) Blame(Spec.Formacje, w * 0.9f, $"{Phrases.KindPl(atk.Kind)} zaskoczyli ludzi: nikt nie obserwował tyłów i flank.");
                else Blame(Spec.Wyszkolenie, w * 0.5f, "Ludzie nie zauważyli wroga na czas (percepcja, światło).");
            }
            else if (tHandled > e.T)
            {
                if (first) r.Missed++;
                Blame(Spec.Poziom, w, $"Dowódca wiedział o zagrożeniu ({Phrases.KindPl(atk.Kind)}), ale nikogo do niego nie wyznaczył: za mało podgrup naraz.");
            }
            else if (tHandled - tKnown > 3.5f)
            {
                if (first) r.Late++;
                Blame(Spec.Poziom, w * 0.8f, $"Reakcja na {Phrases.KindPl(atk.Kind)} przyszła po {tHandled - tKnown:0.0} s: za wolny cykl decyzji.");
            }
            else
            {
                if (first) r.Accurate++;
                // reakcja na czas, ale rana mimo to: czy ludzie byli tam, gdzie powinni?
                if (victim.Main.IsRanged && victim.Order.Type == OrderType.Hold && victim.Order.Leash <= 0) Blame(Spec.Kadry, w * 0.9f, $"{victim.Name} (strzelec) stał w linii wręcz.");
                else if (victim.Role == Role.Sanitariusz && victim.Order.Type == OrderType.Hold) Blame(Spec.Kadry, w * 0.9f, $"{victim.Name} (sanitariuszka) walczyła w pierwszej linii zamiast ratować.");
                else if (b.Map.Light(victim.Pos) < 0.25f && victim.Order.Type is OrderType.Hold or OrderType.FireSupport) Blame(Spec.Teren, w * 0.7f, $"{victim.Name} trzymał pozycję w ciemności, gdzie wróg widział lepiej niż on.");
                else Blame(Spec.Wyszkolenie, w * 0.3f, "Wymiana ciosów na właściwych pozycjach: tu decyduje sprzęt i wyszkolenie ludzi.");
            }
        }
        // 2. ogień bratobójczy
        foreach (var e in b.Log.Events.Where(e => e.Kind == LogKind.FriendlyFire))
        {
            if (!b.TryGet(e.Actor, out var sh) || sh.Team != s.Team) continue;
            Blame(Spec.Formacje, 1.2f, "Strzelcy strzelali przez plecy swoich: szyk nie zostawił im czystej linii.");
            Blame(Spec.Wyszkolenie, 0.8f, $"{sh.Name} nie pilnował linii strzału.");
        }
        // 3. strzelcy w linii (Kadry)
        foreach (var (id, t) in c.RangedInLine) if (t >= 6f) Blame(Spec.Kadry, MathF.Min(3f, t / 10f), $"{b.Get(id).Name} (łuk/kusza) dostał rozkaz trzymania linii wręcz ({t:0} s).");
        // 4. ranni
        foreach (var m in s.Members.Where(m => m.DownAt >= 0))
        {
            bool died = m.Status == Status.Dead;
            float sent = c.RescueSent.TryGetValue(m.Id, out var rs) ? rs - m.DownAt : float.MaxValue;
            if (c.Original.Level <= 1) { if (died) Blame(Spec.Poziom, 3, $"{m.Name} wykrwawił się: nikt nie wysłał do niego pomocy."); continue; }
            if (died && sent == float.MaxValue) Blame(Spec.Kadry, 4f, $"{m.Name} padł i nikt nie dostał rozkazu, żeby go ratować.");
            else if (died && sent > 6f) Blame(Spec.Poziom, 2.5f, $"Pomoc do {m.Name} wysłana dopiero po {sent:0} s.");
            else if (!died && sent < 6f) r.Moments.Add($"{m.DownAt:0} s: {m.Name} padł, pomoc wysłana po {sent:0.0} s, przeżył.");
            if (died && b.Log.Events.Any(e => e.Kind == LogKind.Kill && e.Target == m.Id && e.Text.Contains("pozostawiony"))) r.Moments.Add($"{m.Name} został na polu i zmarł po walce.");
        }
        // 5. specjalni wrogowie
        if (r.Screams >= 3) Blame(c.Original.Level >= 3 ? Spec.Kadry : Spec.Poziom, (r.Screams - 2) * 0.8f, $"Krzykacz krzyknął {r.Screams} razy, zanim go zdjęto: strzelcy nie dostali go jako priorytetu.");
        foreach (var e in b.Log.Events.Where(e => e.Kind == LogKind.Explosion))
        {
            var hits = b.Log.Events.Count(x => x.Kind == LogKind.Wound && x.Actor == e.Actor && Math.Abs(x.T - e.T) < 0.1f);
            r.BlastHits += hits;
            if (hits > 0) Blame(Spec.Poziom, hits * 1.5f, $"Nosiciel doszedł do ludzi i ranił {hits}: nikt nie zdjął go z daleka.");
            else r.Moments.Add($"{e.T:0} s: Nosiciel wybuchł z dala od ludzi.");
        }
        // 6. chwile
        foreach (var e in b.Log.Events.Where(e => e.Kind == LogKind.Chaos && e.Actor >= 0 && b.Get(e.Actor).Team == s.Team).Take(2)) r.Moments.Add($"{e.T:0} s: {e.Text}");
        var firstPlan = c.Decisions.FirstOrDefault(d => d.Replaced);
        if (firstPlan != null) r.Moments.Insert(0, $"{firstPlan.T:0} s: pierwszy plan — {PlanPl(b, firstPlan)}");
        var firstReal = c.Decisions.FirstOrDefault(d => d.Replaced && d.ThreatList.Count > 0);
        if (firstReal != null && firstReal != firstPlan) r.Moments.Insert(1, $"{firstReal.T:0} s: pierwszy kontakt — {PlanPl(b, firstReal)}");
        int replans = c.Decisions.Count(d => d.Replaced) - 1;
        if (replans > 0) r.Moments.Add($"Zmian planu: {replans} (w tym nagłych: {c.Decisions.Count(d => d.Replaced && d.Emergency)}).");
        return r;
    }

    static string Short(string s) => s.Length > 140 ? s[..140] + "…" : s;

    static string PlanPl(Battle b, DecisionRecord d) => string.Join("; ", d.TaskList.Where(t => t.who.Length > 0).Select(t =>
        $"{TaskPl(t.type, t.key)}: {string.Join(", ", t.who.Select(id => b.Get(id).Name))}"));
    static string TaskPl(TaskType t, string key) => key == "krag" ? "krąg" : t switch
    {
        TaskType.Hold => "trzymać", TaskType.FireSupport => "ogień", TaskType.Eliminate => "zdjąć Krzykacza", TaskType.KillAtRange => "zdjąć Nosiciela",
        TaskType.Rescue => "ratować", TaskType.Watch => "tyły", TaskType.Reserve => "odwód", TaskType.Assault => "szturm", TaskType.Avoid => "odsunąć się", _ => t.ToString()
    };

    public static string RolePl(Role r) => r switch
    {
        Role.Tarczownik => "Tarczownik", Role.Rebacz => "Rębacz", Role.Strzelec => "Strzelec", Role.Sanitariusz => "Sanitariusz", Role.Zwiadowca => "Zwiadowca",
        Role.Herszt => "Herszt", Role.Lucznik => "Łucznik", Role.Rozbijacz => "Rozbijacz", Role.Nozownik => "Nożownik", Role.Obdartus => "Obdartus", _ => "-"
    };
}
