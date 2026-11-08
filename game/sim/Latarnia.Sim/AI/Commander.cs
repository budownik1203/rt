using System;
using System.Collections.Generic;
using System.Linq;
using Latarnia.Sim.Combat;
using Latarnia.Sim.Core;
using Latarnia.Sim.Data;
using Latarnia.Sim.Units;
using Latarnia.Sim.World;

namespace Latarnia.Sim.AI;

public sealed class KnownEnemy
{
    public int Id; public Kind Kind; public Role Role; public Vec2 Pos, Vel; public float LastSeen, FirstSeen; public bool OnlyHeard;
    public float Value => Kind switch
    {
        Kind.Rat => 0.35f, Kind.Ogar => 3f, Kind.Krzykacz => 2.5f, Kind.Nosiciel => 2f,
        _ => Role == Role.Herszt ? 3.5f : Role == Role.Lucznik ? 2.2f : 2.5f
    };
}

public sealed class Threat
{
    public int Id; public readonly List<KnownEnemy> Members = new();
    public Vec2 Center, Vel; public float Radius, Strength, Eta, Distance; public string Label = "";
    public bool Swarm, Fast, Support, Explosive, Ranged, Human;
    public Vec2 Dir;   // od rdzenia składu do zagrożenia
    public float Priority;
    public bool Handled;
}

public enum TaskType { Hold, FireSupport, Eliminate, KillAtRange, Rescue, Watch, Reserve, Assault, Avoid }

public sealed class CTask
{
    public int Id; public TaskType Type; public Threat? Threat; public Vec2 Pos; public float Facing; public float Priority, Required;
    public readonly List<Fighter> Assigned = new(); public int TargetId = -1; public Kind? PriorityKind; public string Formation = "";
    public string Key = "";   // stabilny klucz zadania między cyklami (np. "hold:brama")
    public float Covered;     // ile siły przydzielono
    public readonly Dictionary<int, Vec2> Slots = new();
    public readonly Dictionary<int, float> SlotFacing = new();
}

public sealed class Plan
{
    public readonly List<CTask> Tasks = new(); public float Score; public bool Emergency; public float Created; public string Why = "";
}

public sealed record DecisionRecord(float T, int Known, string Threats, string Tasks, bool Replaced, bool Emergency, string Why, List<(int threatId, string label, float eta, bool handled, Kind kind, Vec2 pos)> ThreatList, List<(string key, TaskType type, int[] who, float cover)> TaskList);

/// <summary>AI dowódcy w pięciu krokach (TECHNIKA_AI_I_MECHANIKI.md, rozdział 6).</summary>
public sealed class Commander
{
    public CommanderProfile P;
    public Fighter Leader;
    public readonly List<Fighter> Deputies = new();
    public readonly Dictionary<int, KnownEnemy> Known = new();
    public List<Threat> LastThreats = new();
    public Plan? Current; public float PlanAge;
    public readonly List<DecisionRecord> Decisions = new();
    public float NextThink = 0.5f;
    public float ChaosUntil = -1;
    public bool ReplanRequested;
    public int ScreamsHeard;
    public bool Surprised;   // ktoś oberwał od wroga, o którym dowódca nie wiedział
    public int FlaresLeft = 2; float _nextFlare;
    readonly Queue<string> _shouts = new(); float _nextShout;
    int _taskSeq, _threatSeq;
    float _lastPlanAt = -99;
    readonly Dictionary<string, int> _taskIds = new();
    public readonly CommanderProfile Original;
    // do raportu: kiedy dowódca dowiedział się o wrogu, kiedy zareagował, kiedy wysłał kogoś do rannego
    public readonly Dictionary<int, float> FirstKnown = new(), FirstHandled = new(), RescueSent = new();
    public readonly Dictionary<int, float> RangedInLine = new();   // czas, który strzelcy spędzili w linii wręcz

    public Commander(Fighter leader, CommanderProfile p) { Leader = leader; P = p; Original = p; }

    public void Update(Battle b, Squad s)
    {
        // łańcuch dowodzenia: po upadku lidera 3–5 s chaosu, potem zastępca (słabszy profil)
        if (!Leader.Active)
        {
            if (ChaosUntil < 0)
            {
                ChaosUntil = b.Time + b.Rng.Range(3f, 5f);
                b.Log.Event(b.Time, LogKind.Chaos, Leader.Id, -1, $"{s.Name}: dowódca {Leader.Name} wyłączony, chaos");
                foreach (var m in s.Active) { m.Order = new Order(); m.Say(Phrases.Pick(Phrases.LeaderDown, b.Rng), b.Time); }
                Current = null;
            }
            if (b.Time < ChaosUntil) return;
            var dep = Deputies.FirstOrDefault(d => d.Active);
            if (dep == null) { P = CommanderProfile.Of(1); return; }
            Leader = dep; ChaosUntil = -1;
            P = CommanderProfile.Of(Math.Max(1, Original.Level - 1));
            Leader.Say(Phrases.Pick(Phrases.TakeCommand, b.Rng), b.Time);
            b.Log.Event(b.Time, LogKind.Chaos, Leader.Id, -1, $"{s.Name}: dowodzenie przejmuje {Leader.Name} ({P})");
            NextThink = b.Time + 0.3f;
        }
        if (P.Level <= 1) { Ingest(b, s); return; }   // poziom 1: brak planowania, sam instynkt
        PlanAge += Battle.Dt;
        // zdarzenia nagłe przyspieszają myślenie (ale nie szybciej niż reakcja)
        bool early = b.Time - _lastPlanAt > MathF.Max(1.2f, P.Cycle * 0.6f) && (ReplanRequested || EmergencyNow(b, s));
        if (b.Time >= NextThink || early)
        {
            Think(b, s); _lastPlanAt = b.Time;
            NextThink = b.Time + P.Cycle;
            ReplanRequested = false;
        }
        FlushShouts(b);
    }

    // ---------------- krok 1: obraz sytuacji ----------------
    void Ingest(Battle b, Squad s)
    {
        foreach (var m in s.Members)
        {
            if (!m.Active) continue;
            // meldunki: kto jest daleko od dowódcy i poza zasięgiem głosu, melduje z opóźnieniem (pomijamy go w tym cyklu)
            if (m != Leader && Vec2.Dist(m.Pos, Leader.Pos) > 30f) continue;
            foreach (var id in m.Seen)
            {
                var e = b.Get(id); if (!e.Active) continue;
                if (!Known.TryGetValue(id, out var k)) { k = new KnownEnemy { Id = id, Kind = e.Kind, Role = e.Role, FirstSeen = b.Time }; Known[id] = k; }
                FirstKnown.TryAdd(id, b.Time);
                k.Vel = k.LastSeen > 0 && b.Time - k.LastSeen < 1f ? (e.Pos - k.Pos) / MathF.Max(0.05f, b.Time - k.LastSeen) : e.Vel;
                k.Pos = e.Pos; k.LastSeen = b.Time; k.OnlyHeard = false;
            }
            foreach (var (id, (pos, t)) in m.Heard)
            {
                if (id < 0 || !b.TryGet(id, out var e) || !e.Active) continue;
                if (Known.TryGetValue(id, out var k) && b.Time - k.LastSeen < 0.5f) continue;
                if (k == null) { k = new KnownEnemy { Id = id, Kind = e.Kind, Role = e.Role, FirstSeen = b.Time, OnlyHeard = true }; Known[id] = k; }
                k.Pos = pos; k.LastSeen = t;
            }
        }
        var forget = Known.Values.Where(k => b.Time - k.LastSeen > MathF.Max(1.5f, P.Memory) || (b.TryGet(k.Id, out var e) && !e.Active && b.Time - k.LastSeen > 0.3f)).Select(k => k.Id).ToList();
        foreach (var id in forget) Known.Remove(id);
    }

    bool EmergencyNow(Battle b, Squad s)
    {
        if (Current == null) return Known.Count > 0 || s.Members.Any(m => m.Seen.Count > 0);
        // wróg blisko, którego obecny plan nie obejmuje
        foreach (var m in s.Members)
        {
            if (!m.Active) continue;
            foreach (var id in m.Seen)
            {
                var e = b.Get(id);
                if (Vec2.Dist(e.Pos, m.Pos) < 8f && !Current.Tasks.Any(t => t.Threat != null && t.Threat.Members.Any(x => x.Id == id))) return true;
            }
        }
        return s.Members.Any(m => m.Status == Status.Down && b.Time - m.DownAt < 0.2f);
    }

    public Vec2 Core(Battle b, Squad s)
    {
        // rdzeń do obrony: przy obronie latarnia i ranni, przy ataku środek składu
        if (s.Attacking) { var act = s.Active.ToList(); return act.Count == 0 ? s.Anchor : new Vec2(act.Average(m => m.Pos.X), act.Average(m => m.Pos.Y)); }
        var downs = s.Members.Where(m => m.Status == Status.Down).Select(m => m.Pos).ToList();
        var c = s.Anchor;
        if (downs.Count > 0) c = Vec2.Lerp(c, new Vec2(downs.Average(p => p.X), downs.Average(p => p.Y)), 0.3f);
        return c;
    }

    List<Threat> Cluster(Battle b, Squad s, Vec2 core)
    {
        var pts = Known.Values.ToList();
        var threats = new List<Threat>();
        var used = new HashSet<int>();
        float r = P.ClusterRadius;
        foreach (var k in pts.OrderBy(k => Vec2.DistSq(k.Pos, core)))
        {
            if (used.Contains(k.Id)) continue;
            var th = new Threat { Id = threats.Count };
            var stack = new Stack<KnownEnemy>(); stack.Push(k); used.Add(k.Id);
            while (stack.Count > 0)
            {
                var c = stack.Pop(); th.Members.Add(c);
                foreach (var o in pts) if (!used.Contains(o.Id) && Vec2.Dist(o.Pos, c.Pos) < r) { used.Add(o.Id); stack.Push(o); }
            }
            threats.Add(th);
        }
        // słabszy dowódca widzi mniej grup: scala najmniejsze z najbliższymi
        while (threats.Count > P.MaxThreats)
        {
            var small = threats.OrderBy(t => t.Members.Sum(m => m.Value)).First(); threats.Remove(small);
            Describe(small, core, b, s);
            var near = threats.OrderBy(t => Vec2.Dist(Center(t), small.Center)).First(); near.Members.AddRange(small.Members);
        }
        foreach (var t in threats) Describe(t, core, b, s);
        // tożsamość zagrożeń między cyklami: dopasowanie po wspólnych członkach
        var taken = new HashSet<int>();
        foreach (var t in threats.OrderByDescending(t => t.Members.Count))
        {
            var ids = t.Members.Select(m => m.Id).ToHashSet();
            var prev = LastThreats.Where(p => !taken.Contains(p.Id)).Select(p => (p, n: p.Members.Count(m => ids.Contains(m.Id)))).Where(x => x.n > 0).OrderByDescending(x => x.n).FirstOrDefault();
            t.Id = prev.p != null ? prev.p.Id : ++_threatSeq;
            taken.Add(t.Id);
        }
        return threats;
    }

    static Vec2 Center(Threat t) => new(t.Members.Average(m => m.Pos.X), t.Members.Average(m => m.Pos.Y));

    void Describe(Threat t, Vec2 core, Battle b, Squad s)
    {
        t.Center = Center(t);
        t.Vel = new Vec2(t.Members.Average(m => m.Vel.X), t.Members.Average(m => m.Vel.Y));
        var pred = t.Center + t.Vel * P.Predict;   // przewidywanie (poziom 3+)
        t.Radius = MathF.Max(1f, t.Members.Max(m => Vec2.Dist(m.Pos, t.Center)));
        t.Strength = t.Members.Sum(m => m.Value);
        t.Swarm = t.Members.Count(m => m.Kind == Kind.Rat) >= 4;
        t.Fast = t.Members.Any(m => m.Kind == Kind.Ogar);
        t.Support = t.Members.Any(m => m.Kind == Kind.Krzykacz);
        t.Explosive = t.Members.Any(m => m.Kind == Kind.Nosiciel);
        t.Ranged = t.Members.Any(m => m.Role == Role.Lucznik);
        if (t.Ranged) t.Label = t.Label;
        t.Human = t.Members.Any(m => m.Kind == Kind.Human);
        var act = s.Active.ToList();
        float nearest = act.Count == 0 ? 99 : act.Min(m => Vec2.Dist(m.Pos, pred));
        t.Distance = nearest;
        float speed = MathF.Max(t.Vel.Length, t.Fast ? 6f : t.Swarm ? 3.5f : 1.5f);
        t.Eta = nearest / speed;
        t.Dir = (pred - core).Normalized();
        t.Label = Landmarks.Name(b.Map, pred, core);
    }

    // ---------------- krok 2–5 ----------------
    void Think(Battle b, Squad s)
    {
        Ingest(b, s);
        var core = Core(b, s);
        var threats = Cluster(b, s, core);
        LastThreats = threats;
        MaybeFlare(b, s, core, threats);
        var plan = BuildPlan(b, s, core, threats);
        float curScore = Current == null ? 0 : Rescore(Current, plan, out _);
        bool collapsing = Current != null && Rescore(Current, plan, out float worst) >= 0 && worst < 0.45f && PlanAge > 1.2f;
        if (collapsing) plan.Emergency = true;
        bool replace = Current == null || (plan.Emergency && PlanAge > 1.0f) || (ReplanRequested && PlanAge > 1.5f) ||
                       (PlanAge >= P.MinPlan && plan.Score > curScore * (1 + P.ReplaceThreshold) + 0.01f) ||
                       (PlanAge >= P.MinPlan && StructureChanged(Current, plan));
        string why = Current == null ? "pierwszy plan" : plan.Emergency ? "zdarzenie nagłe" : ReplanRequested ? "meldunek z linii" : replace ? "lepszy plan" : "plan bez zmian";
        if (replace)
        {
            Current = plan; PlanAge = 0; Current.Why = why;
            IssueOrders(b, s, plan, full: true);
        }
        else if (Current != null)
        {
            // ten sam przydział, ale pozycje śledzą zagrożenia (bez nowych okrzyków)
            RefreshPositions(b, s, Current, plan);
        }
        var cur = Current ?? plan;
        foreach (var t in cur.Tasks)
        {
            if (t.Threat != null && t.Assigned.Count > 0) foreach (var m in t.Threat.Members) FirstHandled.TryAdd(m.Id, b.Time);
            if (t.Type == TaskType.Rescue && t.Assigned.Count > 0) RescueSent.TryAdd(t.TargetId, b.Time);
            if (t.Type == TaskType.Hold && t.Key != "krag") foreach (var a in t.Assigned) if (a.Main.IsRanged) RangedInLine[a.Id] = RangedInLine.GetValueOrDefault(a.Id) + P.Cycle;
        }
        Decisions.Add(new DecisionRecord(b.Time, Known.Count,
            string.Join("; ", threats.Select(t => $"{t.Label}: {t.Members.Count}×{Phrases.KindPl(t.Members[0].Kind)} (siła {t.Strength:0.0}, za {t.Eta:0.0}s)")),
            string.Join("; ", (Current ?? plan).Tasks.Select(t => $"{t.Type} {t.Key} ← {string.Join(",", t.Assigned.Select(a => a.Name))}")),
            replace, plan.Emergency, why,
            threats.Select(t => (t.Id, t.Label, t.Eta, (Current ?? plan).Tasks.Any(k => k.Threat != null && k.Threat.Members.Intersect(t.Members).Any()), t.Members[0].Kind, t.Center)).ToList(),
            (Current ?? plan).Tasks.Select(t => (t.Key, t.Type, t.Assigned.Select(a => a.Id).ToArray(), t.Type == TaskType.Hold || t.Type == TaskType.FireSupport ? b.Map.CoverFrom(t.Pos, t.Threat?.Center ?? t.Pos) : 0f)).ToList()));
    }

    /// <summary>Ile wart jest obecny plan wobec dzisiejszego obrazu sytuacji (zagrożenia rosną, ludzie padają).</summary>
    static float Rescore(Plan cur, Plan fresh, out float worstHold)
    {
        float score = 0; worstHold = 1f;
        foreach (var t in cur.Tasks)
        {
            if (t.Type == TaskType.Reserve) continue;
            var nt = fresh.Tasks.FirstOrDefault(x => x.Key == t.Key);
            if (nt == null) continue;
            float cov = t.Assigned.Where(a => a.Active).Sum(a => Power(a, nt));
            float ratio = MathX.Clamp01(cov / MathF.Max(0.1f, MathF.Min(nt.Required, 6f)));
            score += nt.Priority * ratio;
            if (t.Type is TaskType.Hold or TaskType.Assault) worstHold = MathF.Min(worstHold, ratio);
        }
        return score;
    }

    /// <summary>Flara: dowódca oświetla zagrożenie w ciemności, żeby strzelcy je widzieli (poziom 3 także na sam hałas).</summary>
    void MaybeFlare(Battle b, Squad s, Vec2 core, List<Threat> threats)
    {
        if (FlaresLeft <= 0 || b.Time < _nextFlare || s.Attacking || !Leader.Active) return;
        Vec2? where = null; string why = "";
        foreach (var t in threats.OrderBy(t => t.Eta))
        {
            var p = t.Center + t.Vel * 2f;
            if (b.LightAt(p) < 0.3f && t.Distance < 26f && t.Distance > 5f && (t.Strength >= 2f || t.Explosive || t.Support)) { where = p; why = t.Label; break; }
        }
        if (where == null && P.Level >= 3)
        {
            // słychać coś w ciemności, a nic nie widać: flara na hałas
            var heard = s.Active.SelectMany(m => m.Heard.Values).Where(h => b.Time - h.t < 1f && b.LightAt(h.pos) < 0.25f && Vec2.Dist(h.pos, core) < 24f).Select(h => h.pos).ToList();
            if (heard.Count >= 3) { where = new Vec2(heard.Average(h => h.X), heard.Average(h => h.Y)); why = Landmarks.Name(b.Map, where.Value, core); }
        }
        if (where == null) return;
        var target = where.Value;
        if (Vec2.Dist(target, Leader.Pos) > 25f) target = Leader.Pos + (target - Leader.Pos).Normalized() * 25f;
        b.Flares.Add(new Flare(target, 10f, b.Time + 20f, Leader.Id));
        FlaresLeft--; _nextFlare = b.Time + 12f;
        _shouts.Enqueue($"Flara na {why.Replace(" (za palisadą)", "")}!");
        b.Log.Event(b.Time, LogKind.Order, Leader.Id, -1, $"{Leader.Name}: flara ({why})");
    }

    static bool StructureChanged(Plan a, Plan b)
    {
        var ka = a.Tasks.Where(t => t.Type != TaskType.Reserve).Select(t => t.Key).ToHashSet();
        var kb = b.Tasks.Where(t => t.Type != TaskType.Reserve).Select(t => t.Key).ToHashSet();
        return !ka.SetEquals(kb);
    }

    int TaskId(string key) { if (!_taskIds.TryGetValue(key, out var id)) { id = ++_taskSeq; _taskIds[key] = id; } return id; }

    Plan BuildPlan(Battle b, Squad s, Vec2 core, List<Threat> threats)
    {
        var plan = new Plan { Created = b.Time };
        var pool = s.Active.ToList();
        if (pool.Count == 0) return plan;
        var downs = s.Members.Where(m => m.Status == Status.Down && m.CarriedById < 0).ToList();

        // priorytet zagrożeń: siła × bliskość × wrażliwość celu
        foreach (var t in threats)
        {
            float urgency = 1f + 8f / MathF.Max(1f, t.Eta);
            float vuln = 1f;
            if (downs.Any(d => Vec2.Dist(d.Pos, t.Center) < 8)) vuln += 0.6f;
            if (pool.Any(m => m.Current.IsRanged && Vec2.Dist(m.Pos, t.Center) < 6)) vuln += 0.4f;
            float special = 1f;
            if (t.Support) special *= ScreamsHeard > 0 || P.KnowsBestiary ? 2.2f : 1.2f;   // po pierwszym krzyku dowódca rozumie zagrożenie
            if (t.Explosive && t.Distance < 10) special *= 1.8f;
            t.Priority = t.Strength * urgency * vuln * special * P.RiskWeight;
            if (s.Attacking && P.Kadry >= 3) t.Priority = (8f / MathF.Max(1f, t.Strength)) * urgency;   // atakujący dobry dowódca bije w słabszą grupę
        }
        var ordered = threats.OrderByDescending(t => t.Priority).ToList();
        int capacity = P.Subgroups;   // poziom 2: jedno zagrożenie naraz, poziom 3: dwa
        if (s.Attacking) capacity = 1;   // szturm: koncentracja sił w jednym punkcie, nie rozdrabniać
        var handled = ordered.Take(capacity).ToList();
        foreach (var t in handled) t.Handled = true;

        var tasks = plan.Tasks;
        foreach (var t in handled)
        {
            string key = $"z{t.Id}";
            if (t.Explosive && !t.Swarm && !t.Fast && !t.Human)
            {
                tasks.Add(MakeTask(TaskType.KillAtRange, t, $"bomba:{key}", t.Priority, 1.5f, b, s, core, Kind.Nosiciel));
                tasks.Add(MakeTask(TaskType.Avoid, t, $"odsun:{key}", t.Priority * 0.5f, 0, b, s, core));
                continue;
            }
            if (t.Support && t.Members.Count <= 2 && !t.Fast && !t.Swarm)
            {
                tasks.Add(MakeTask(TaskType.Eliminate, t, $"elim:{key}", t.Priority, 2f, b, s, core, Kind.Krzykacz));
                continue;
            }
            if (t.Explosive)
            {
                // Nosiciel wśród innych: zdjąć go z daleka, linia trzyma się z dala od niego
                tasks.Add(MakeTask(TaskType.KillAtRange, t, $"bomba:{key}", t.Priority * 1.1f, 1.5f, b, s, core, Kind.Nosiciel));
            }
            bool rushArchers = !s.Attacking && t.Ranged && P.Level >= 3 && b.LightAt(t.Center) < 0.35f && t.Members.Count(m => m.Role != Role.Lucznik) <= 1;
            if (s.Attacking || rushArchers) tasks.Add(MakeTask(TaskType.Assault, t, $"szturm:{key}", t.Priority, s.Attacking ? 99f : t.Strength * 1.1f, b, s, core));   // szturm: wszyscy, odwód nic nie daje
            else tasks.Add(MakeTask(TaskType.Hold, t, $"hold:{key}", t.Priority, t.Strength * 1.15f, b, s, core));
            var fs = MakeTask(TaskType.FireSupport, t, $"ogien:{key}", t.Priority * 0.8f, 99, b, s, core, t.Support ? Kind.Krzykacz : t.Explosive ? Kind.Nosiciel : null);
            // strzelcy obok linii, nie za nią: zza pleców swoich nie ma czystej linii strzału (od Formacji 2)
            var line = tasks.LastOrDefault(x => x.Threat == t && x.Type is TaskType.Hold or TaskType.Assault);
            if (line != null && P.Formacje >= 2 && !s.Attacking)
            {
                var fwd = Vec2.FromAngle(line.Facing); var side = fwd.Perp;
                Vec2 Best(float sgn) => Terrain.Walkable(b.Map, line.Pos + side * (sgn * 3.4f) - fwd * 0.8f);
                var a1 = Best(1); var a2 = Best(-1);
                float S(Vec2 p) => (LineOfSight.Clear(b.Map, p, t.Center) ? 2f : 0) + b.LightAt(p) + b.Map.CoverFrom(p, t.Center);
                fs.Pos = S(a1) >= S(a2) ? a1 : a2;
            }
            tasks.Add(fs);
        }
        // szybkie zagrożenia (Ogary) albo atak z wielu stron: krąg wokół rdzenia zamiast rozdzielania ludzi
        bool fast = handled.Any(t => t.Fast) || threats.Count(t => t.Fast) > 0;
        bool many = threats.Count >= 2 && threats.Max(a => threats.Max(c2 => MathF.Abs(MathX.AngleDiff(a.Dir.Angle, c2.Dir.Angle)))) > 1.6f;
        if (!s.Attacking && ((fast && P.Formacje >= 2) || (many && P.Formacje >= 3)))
        {
            tasks.RemoveAll(t => t.Type == TaskType.Hold);
            var main = handled.FirstOrDefault();
            var ring = new CTask { Type = TaskType.Hold, Threat = main, Pos = core, Facing = main?.Dir.Angle ?? 0, Priority = handled.Sum(t => t.Priority) + 1, Required = 99, Key = "krag", Formation = "krąg" };
            ring.Id = TaskId(ring.Key); tasks.Add(ring);
        }
        // ranni
        // atakujący ratują dopiero, gdy wokół spokojnie (szturmu nie przerywa się dla rannego)
        foreach (var d in downs.Where(d => (!d.Stabilized || EnemiesNear(b, d.Pos, 6) > 0) && (!s.Attacking || EnemiesNear(b, d.Pos, 12) == 0)))
        {
            float danger = EnemiesNear(b, d.Pos, 8);
            var rt = new CTask { Type = TaskType.Rescue, TargetId = d.Id, Pos = d.Pos, Priority = 3f + danger * 2f, Required = 1, Key = $"ratuj:{d.Name}" };
            rt.Id = TaskId(rt.Key); tasks.Add(rt);
        }
        // obserwacja tyłów (sektory): od Formacji 3
        // obserwacja tyłów ma sens, gdy wróg może przyjść z innej strony (Ogary, kilka kierunków, wcześniejsze zaskoczenie)
        bool flankRisk = threats.Any(t => t.Fast) || threats.Count >= 2 || Surprised || threats.Count == 0 || P.Formacje >= 4;
        if (P.UsesSectors && pool.Count >= 4 && !s.Attacking && flankRisk)
        {
            var main = handled.FirstOrDefault();
            var watchDir = main != null ? -main.Dir : Vec2.FromAngle(b.Time * 0.1f);
            var wt = new CTask { Type = TaskType.Watch, Pos = core + watchDir * 3f, Facing = watchDir.Angle, Priority = 1.2f, Required = 1, Key = "obserwuj:tyl" };
            wt.Id = TaskId(wt.Key); tasks.Add(wt);
        }
        // bez zagrożeń: atakujący idą do celu, obrońcy czekają w gotowości przy rdzeniu
        if (handled.Count == 0 && s.Attacking)
        {
            var adv = new CTask { Type = TaskType.Assault, Pos = s.Anchor, Facing = (s.Anchor - core).Angle, Priority = 1, Required = 99, Key = "naprzod" };
            adv.Id = TaskId(adv.Key); tasks.Add(adv);
        }
        var reserve = new CTask { Type = TaskType.Reserve, Pos = core, Priority = 0.1f, Required = 99, Key = "odwod" };
        reserve.Id = TaskId(reserve.Key); tasks.Add(reserve);
        plan.Emergency = threats.Any(t => t.Distance < 6f && t.Handled && !(Current?.Tasks.Any(k => k.Key.EndsWith($":z{t.Id}")) ?? false))
                         || downs.Any(d => b.Time - d.DownAt < 1f && Current != null && !Current.Tasks.Any(k => k.TargetId == d.Id));
        if (plan.Tasks.Any(t => t.Key == "krag"))
            foreach (var t in plan.Tasks.Where(t => t.Type is TaskType.FireSupport or TaskType.Reserve)) t.Pos = Terrain.Walkable(b.Map, core - (t.Threat?.Dir ?? Vec2.Zero) * 0.5f);
        Assign(b, s, plan, pool, core);
        Formations.Arrange(b, this, plan, core);
        plan.Score = plan.Tasks.Sum(t => t.Type == TaskType.Reserve ? 0 : t.Priority * MathX.Clamp01(t.Covered / MathF.Max(0.1f, MathF.Min(t.Required, 6f))));
        return plan;
    }

    static int EnemiesNear(Battle b, Vec2 p, float r) => b.All.Count(e => e.Active && e.Team == Team.Enemy && Vec2.Dist(e.Pos, p) < r);

    CTask MakeTask(TaskType type, Threat t, string key, float prio, float req, Battle b, Squad s, Vec2 core, Kind? pk = null)
    {
        var task = new CTask { Type = type, Threat = t, Key = key, Priority = prio, Required = req, PriorityKind = pk, Facing = t.Dir.Angle };
        task.Id = TaskId(key);
        task.Pos = Terrain.PickPosition(b, this, task, core);
        return task;
    }

    // ---------------- krok 3: przydział ----------------
    void Assign(Battle b, Squad s, Plan plan, List<Fighter> pool, Vec2 core)
    {
        var free = new List<Fighter>(pool);
        var prevTask = new Dictionary<int, string>();
        if (Current != null) foreach (var t in Current.Tasks) foreach (var a in t.Assigned) prevTask[a.Id] = t.Key;

        void Give(CTask task, Fighter f) { task.Assigned.Add(f); free.Remove(f); task.Covered += Power(f, task); }
        IEnumerable<Fighter> Ranked(CTask task) => free.Where(f => Eligible(f, task)).OrderByDescending(f => Score(b, f, task, prevTask)).ToList();

        // 1. ratowanie: jedna osoba na rannego (najlepiej sanitariusz); przy walce wręcz nie więcej niż co trzeci człowiek
        int maxRescuers = plan.Tasks.Any(t => t.Threat != null && t.Threat.Eta < 6f) ? Math.Max(1, pool.Count / 3) : pool.Count;
        foreach (var task in plan.Tasks.Where(t => t.Type == TaskType.Rescue).OrderByDescending(t => t.Priority))
        {
            if (plan.Tasks.Where(t => t.Type == TaskType.Rescue).Sum(t => t.Assigned.Count) >= maxRescuers) break;
            var f = Ranked(task).FirstOrDefault();
            if (f == null) continue;
            // nie-sanitariusz idzie tylko, gdy ranny krwawi mocno albo jest spokojnie (Kadry ≥ 3 to rozumie)
            if (P.Kadry >= 3 && f.Role != Role.Sanitariusz && b.TryGet(task.TargetId, out var w) && w.Bleed < 1.0f && maxRescuers < pool.Count) continue;
            Give(task, f);
        }
        // 2. zadania specjalne strzelców (Krzykacz, Nosiciel): do 2 osób
        foreach (var task in plan.Tasks.Where(t => t.Type is TaskType.Eliminate or TaskType.KillAtRange).OrderByDescending(t => t.Priority))
            foreach (var f in Ranked(task).Take(2)) Give(task, f);
        // 3. linia: walczący wręcz dzieleni między zagrożenia według priorytetu
        var holds = plan.Tasks.Where(t => t.Type is TaskType.Hold or TaskType.Assault).OrderByDescending(t => t.Priority).ToList();
        if (holds.Count > 0)
        {
            float total = holds.Sum(h => h.Priority);
            int melee = free.Count(f => Eligible(f, holds[0]));
            for (int i = 0; i < holds.Count; i++)
            {
                var task = holds[i];
                int left = holds.Count - i - 1;
                int share = i == holds.Count - 1 ? 99 : Math.Max(1, (int)MathF.Round(melee * task.Priority / total));
                int avail = free.Count(f => Eligible(f, task));
                share = Math.Min(share, Math.Max(1, avail - left));
                foreach (var f in Ranked(task))
                {
                    if (task.Assigned.Count >= share || (task.Covered >= task.Required && task.Assigned.Count >= 1)) break;
                    Give(task, f);
                }
            }
        }
        // 3b. krąg: wszyscy wolni w krąg (strzelcy też, na obwodzie mają czyste linie na zewnątrz)
        var ringTask = plan.Tasks.FirstOrDefault(t => t.Key == "krag");
        if (ringTask != null) foreach (var f in free.Where(f => f.Role != Role.Sanitariusz || P.Kadry <= 2).ToList()) Give(ringTask, f);
        // 4. obserwacja tyłów: jedna osoba
        foreach (var task in plan.Tasks.Where(t => t.Type == TaskType.Watch))
        { var f = Ranked(task).FirstOrDefault(); if (f != null && free.Count > 1) Give(task, f); }
        // 5. wsparcie ogniowe: wszyscy pozostali strzelcy, rozdzieleni między zagrożenia
        var fs = plan.Tasks.Where(t => t.Type == TaskType.FireSupport).OrderByDescending(t => t.Priority).ToList();
        for (int k = 0; fs.Count > 0 && k < 10; k++)
        {
            bool any = false;
            foreach (var task in fs)
            {
                var f = Ranked(task).FirstOrDefault();
                if (f != null) { Give(task, f); any = true; }
            }
            if (!any) break;
        }
        // 6. linia bez nikogo (np. sami strzelcy): słabszy dowódca dobierze kogokolwiek
        foreach (var task in holds.Where(h => h.Assigned.Count == 0))
        {
            var f = free.Where(x => x.Role != Role.Sanitariusz || P.Kadry <= 2).OrderByDescending(x => Score(b, x, task, prevTask)).FirstOrDefault()
                    ?? (P.Kadry <= 2 ? null : fs.SelectMany(t => t.Assigned).Where(x => !x.Main.IsRanged).FirstOrDefault());
            if (f != null) { foreach (var t in fs) if (t.Assigned.Remove(f)) free.Add(f); Give(task, f); }
        }
        var res = plan.Tasks.First(t => t.Type == TaskType.Reserve);
        res.Assigned.AddRange(free);
    }

    bool Eligible(Fighter f, CTask t)
    {
        // Kadry 1–2: nie odróżnia dobrze strzelców od walczących wręcz (strzelec może trafić do linii)
        bool ranged = f.Main.IsRanged && f.Ammo > 0;
        return t.Type switch
        {
            TaskType.FireSupport or TaskType.Eliminate or TaskType.KillAtRange => ranged || (P.Kadry <= 1),
            TaskType.Hold or TaskType.Assault => (!ranged || P.Kadry <= 2 || f.Role == Role.Zwiadowca && P.Kadry <= 2) && (f.Role != Role.Sanitariusz || P.Kadry <= 2) && (f != Leader || !ranged),
            TaskType.Rescue => f.Role != Role.Herszt,
            _ => true
        };
    }

    float Score(Battle b, Fighter f, CTask t, Dictionary<int, string> prev)
    {
        float s = -Vec2.Dist(f.Pos, t.Pos) / 8f;
        if (P.Kadry >= 2) s += RoleMatch(f, t) * (P.Kadry >= 3 ? 3f : 1.5f);
        if (P.Kadry >= 3)
        {
            if (t.Type is TaskType.FireSupport or TaskType.Eliminate or TaskType.KillAtRange) s += f.Ammo > 3 ? 1 : -3;
            s -= f.Fatigue / 40f + f.Wounds.Count * 0.6f;
        }
        if (P.Kadry >= 4 && f.Morale < 40) s -= 1.5f;
        if (prev.TryGetValue(f.Id, out var k) && k == t.Key) s += 1.2f;   // koszt porzucenia pozycji
        // dowódca ze strzelbą/łukiem stoi z tyłu; Herszt z maczetą prowadzi szturm
        if (f == Leader) s -= t.Type is TaskType.Hold or TaskType.Assault ? (f.Main.IsRanged ? 2.5f : -0.5f) : 0.5f;
        return s;
    }

    public static float RoleMatch(Fighter f, CTask t)
    {
        var th = t.Threat;
        return t.Type switch
        {
            TaskType.Hold or TaskType.Assault => f.Role switch
            {
                Role.Tarczownik => th != null && (th.Swarm || th.Fast) ? 1.0f : 0.8f,
                Role.Rebacz or Role.Rozbijacz => th != null && th.Fast ? 0.95f : 0.8f,
                Role.Obdartus or Role.Nozownik or Role.Herszt => 0.6f,
                Role.Sanitariusz => -0.4f, Role.Strzelec or Role.Lucznik => -0.6f, Role.Zwiadowca => 0.1f, _ => 0
            },
            TaskType.FireSupport or TaskType.Eliminate or TaskType.KillAtRange => f.Role is Role.Strzelec or Role.Lucznik ? 1f : f.Role == Role.Zwiadowca ? 0.8f : f.Main.IsRanged ? 0.5f : -1f,
            TaskType.Rescue => f.Role == Role.Sanitariusz ? 1.5f : f.Merciful ? 0.4f : 0f,
            TaskType.Watch => f.Role == Role.Zwiadowca ? 1f : f.Main.IsRanged ? 0.4f : f.Role == Role.Sanitariusz ? -1f : 0f,
            _ => 0
        };
    }

    public static float Power(Fighter f, CTask t)
    {
        float p = t.Type is TaskType.Hold or TaskType.Assault ? f.Role switch
        {
            Role.Tarczownik => t.Threat?.Swarm == true ? 3.2f : 2.6f,
            Role.Rebacz or Role.Rozbijacz => t.Threat?.Fast == true ? 2.6f : 2.2f,
            Role.Herszt => 2.4f, Role.Obdartus or Role.Nozownik => 1.8f,
            _ => f.Main.IsRanged ? 0.9f : 1.3f
        } : t.Type is TaskType.FireSupport or TaskType.Eliminate or TaskType.KillAtRange ? (f.Main.IsRanged ? 1.6f : 0.3f) : 1f;
        if (f.Wounds.Count > 0) p *= 0.7f;
        if (f.Morale < 40) p *= 0.7f;
        return p;
    }

    // ---------------- krok 4–5: rozkazy ----------------
    void IssueOrders(Battle b, Squad s, Plan plan, bool full)
    {
        var shouts = new List<string>();
        foreach (var task in plan.Tasks)
        {
            var changed = new List<Fighter>();
            foreach (var f in task.Assigned)
            {
                var o = BuildOrder(task, f, b);
                bool same = f.Order.TaskId == task.Id && f.Order.Type == o.Type && Vec2.Dist(f.Order.Pos, o.Pos) < 2f;
                if (same) continue;
                float delay = P.OrderDelay * (f == Leader ? 0.2f : 1f) * (1f + b.Rng.Range(-0.2f, 0.3f));
                if (Vec2.Dist(f.Pos, Leader.Pos) > 20f) delay += 0.6f;
                bool sameTask = f.Order.TaskId == task.Id && f.Order.Type == o.Type;
                if (sameTask) { f.Order.Pos = o.Pos; f.Order.Facing = o.Facing; f.Order.Anchor = o.Anchor; f.Order.Leash = o.Leash; continue; }   // przesunięcie w szyku: bez okrzyku
                f.PendingOrder = o; f.PendingAt = b.Time + delay;
                if (f != Leader) changed.Add(f);
            }
            if (changed.Count > 0 && task.Type != TaskType.Reserve) shouts.Add(Phrases.Order(task, changed, b.Rng));
            else if (changed.Count > 0 && task.Type == TaskType.Reserve && plan.Tasks.Count > 1 && changed.Count <= 2) shouts.Add(Phrases.Order(task, changed, b.Rng));
        }
        foreach (var sh in shouts.Take(3)) _shouts.Enqueue(sh);
        if (shouts.Count > 0)
        {
            b.MakeNoise(Leader.Pos, 20f, Leader.Team, "rozkaz");
            b.Log.Event(b.Time, LogKind.Order, Leader.Id, -1, $"{Leader.Name}: {string.Join(" | ", shouts)}");
        }
    }

    void RefreshPositions(Battle b, Squad s, Plan cur, Plan fresh)
    {
        foreach (var t in cur.Tasks)
        {
            var nt = fresh.Tasks.FirstOrDefault(x => x.Key == t.Key);
            if (nt == null) continue;
            if (Vec2.Dist(nt.Pos, t.Pos) > 2.5f) { t.Pos = nt.Pos; t.Facing = nt.Facing; t.Threat = nt.Threat; Formations.Arrange(b, this, cur, Core(b, s), only: t); }
            else t.Threat = nt.Threat;
            foreach (var f in t.Assigned)
            {
                if (!f.Active) continue;
                var o = BuildOrder(t, f, b);
                if (f.Order.TaskId == t.Id && Vec2.Dist(f.Order.Pos, o.Pos) > 1.2f) { f.Order.Pos = o.Pos; f.Order.Facing = o.Facing; f.Order.TargetId = o.TargetId; f.Order.Anchor = o.Anchor; f.Order.Leash = o.Leash; }
            }
        }
    }

    Order BuildOrder(CTask t, Fighter f, Battle b)
    {
        var pos = t.Slots.TryGetValue(f.Id, out var sp) ? sp : t.Pos;
        float facing = t.SlotFacing.TryGetValue(f.Id, out var sf) ? sf : t.Facing;
        return new Order
        {
            Type = t.Type switch
            {
                TaskType.Hold => OrderType.Hold, TaskType.FireSupport => OrderType.FireSupport, TaskType.Eliminate => OrderType.Eliminate,
                TaskType.KillAtRange => OrderType.Eliminate, TaskType.Rescue => OrderType.Rescue, TaskType.Watch => OrderType.Guard,
                TaskType.Assault => OrderType.Advance, TaskType.Avoid => OrderType.Avoid, _ => OrderType.Regroup
            },
            Pos = pos, Facing = facing, TargetId = t.TargetId, TaskId = t.Id, IssuedAt = b.Time, Tolerance = P.SlotTolerance, PriorityKind = t.PriorityKind,
            Anchor = t.Pos, Leash = t.Key == "krag" ? Vec2.Dist(pos, t.Pos) + 2.2f : 0
        };
    }

    void FlushShouts(Battle b)
    {
        if (_shouts.Count == 0 || b.Time < _nextShout || !Leader.Active) return;
        var s = _shouts.Dequeue();
        Leader.Say(s, b.Time, 2.0f);
        _nextShout = b.Time + 1.6f;
    }

    public void Report(Battle b, Fighter f, string text, bool replan)
    {
        if (f == Leader) { if (replan && P.Level >= 2) ReplanRequested = true; return; }   // dowódca nie melduje sam sobie
        f.Say(text, b.Time, 1.8f);
        b.Log.Event(b.Time, LogKind.Reply, f.Id, -1, $"{f.Name}: {text}");
        if (replan && P.Level >= 2) ReplanRequested = true;
    }
}

public static class Landmarks
{
    public static string Name(Grid g, Vec2 p, Vec2 core)
    {
        foreach (var c in g.Chokepoints)
            if (Vec2.Dist(c, p) < 7f) return c.Y > g.LampPos.Y + 5 ? "brama" : "wyłom";
        var d = p - g.LampPos;
        float a = MathF.Atan2(d.Y, d.X) * 180f / MathF.PI;   // +Y = południe
        string side = a switch { >= -22.5f and < 22.5f => "wschód", >= 22.5f and < 67.5f => "południowy wschód", >= 67.5f and < 112.5f => "południe", >= 112.5f and < 157.5f => "południowy zachód", >= -67.5f and < -22.5f => "północny wschód", >= -112.5f and < -67.5f => "północ", >= -157.5f and < -112.5f => "północny zachód", _ => "zachód" };
        return d.Length > 17f ? $"{side} (za palisadą)" : side;
    }
}
