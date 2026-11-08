using System;
using System.Collections.Generic;
using System.Linq;
using Latarnia.Sim.AI;
using Latarnia.Sim.Core;
using Latarnia.Sim.Data;
using Latarnia.Sim.Units;
using Latarnia.Sim.World;

namespace Latarnia.Sim.Combat;

public sealed class Squad
{
    public int Index; public Team Team; public string Name = "";
    public readonly List<Fighter> Members = new();
    public Commander? Commander;
    public Vec2 Anchor;          // punkt do obrony (latarnia) albo cel ataku
    public bool Attacking;       // skład atakujący (np. Szabrownicy)
    public IEnumerable<Fighter> Active => Members.Where(m => m.Active);
}

public sealed class Projectile
{
    public int ShooterId; public Team Team; public Vec2 From, To, Pos; public float Speed, Travelled, Total;
    public int TargetId; public bool WillHit; public WeaponDef Weapon = Weapons.PipeBow; public bool Done;
}

public sealed record Flare(Vec2 Pos, float Radius, float Until, int By);
public sealed record Noise(Vec2 Pos, float Radius, float Time, Team Team, string What);

public sealed class Battle
{
    public const float Dt = 0.05f;
    public readonly Grid Map;
    public readonly Pathfinder Paths;
    public readonly Rng Rng;
    public readonly List<Fighter> All = new();
    public readonly List<Squad> Squads = new();
    public readonly List<Projectile> Projectiles = new();
    public readonly List<Noise> Noises = new();
    public readonly BattleLog Log = new();
    public float Time;
    public int Tick;
    public bool Over; public Team? Winner; public string EndReason = "";
    public float MaxTime = 360f;
    public int PendingSpawns => Schedule.Count;     // fale, które jeszcze nadejdą
    public readonly List<(float t, Action<Battle> a)> Schedule = new();
    public readonly Dictionary<string, float> Shared = new();
    public readonly List<Flare> Flares = new();
    /// <summary>Światło latarni plus flary (flara: 10 m, 20 s, gaśnie pod koniec).</summary>
    public float LightAt(Vec2 p)
    {
        float l = Map.Light(p);
        foreach (var f in Flares)
        {
            if (Time > f.Until) continue;
            float d = Vec2.Dist(p, f.Pos); if (d > f.Radius) continue;
            float fade = MathX.Clamp01((f.Until - Time) / 4f);
            l = MathF.Max(l, (1f - d / f.Radius * 0.6f) * 0.85f * fade);
        }
        return l;
    }   // wspólne ustalenia mózgów (np. atak stada)
    readonly Dictionary<int, Fighter> _byId = new();

    public Battle(Grid map, ulong seed) { Map = map; Paths = new Pathfinder(map); Rng = new Rng(seed); }

    public Fighter Get(int id) => _byId[id];
    public bool TryGet(int id, out Fighter f) => _byId.TryGetValue(id, out f!);
    public Fighter Add(Fighter f) { f.Id = All.Count; All.Add(f); _byId[f.Id] = f; return f; }
    public Squad Squad(Fighter f) => Squads[f.SquadIndex];

    public void Run(Action<Battle>? perTick = null)
    {
        while (!Over) { Step(); perTick?.Invoke(this); }
    }

    public void Step()
    {
        if (Over) return;
        Time += Dt; Tick++;
        for (int i = Schedule.Count - 1; i >= 0; i--) if (Time >= Schedule[i].t) { var a = Schedule[i].a; Schedule.RemoveAt(i); a(this); }
        // 1. percepcja: każdy co 0,25 s, rozłożone na ticki
        foreach (var f in All) if (f.Active && (f.Id + Tick) % 5 == 0) Perceive(f);
        Noises.RemoveAll(n => Time - n.Time > 0.6f);
        // 2. dowódcy
        foreach (var s in Squads) s.Commander?.Update(this, s);
        // 3. instynkt i mózgi wrogów
        foreach (var f in All)
        {
            if (!f.Active || Time < f.StunUntil) continue;
            if (f.PendingOrder != null && Time >= f.PendingAt) { f.Order = f.PendingOrder; f.PendingOrder = null; f.OrderReceivedAt = Time; }
            if (f.IsHuman) Instinct.Think(this, f); else EnemyBrains.Think(this, f);
        }
        // 4. ruch
        foreach (var f in All) if (f.Active && Time >= f.StunUntil) Move(f);
        Separate();
        // 5. pociski
        UpdateProjectiles();
        // 6. stany
        foreach (var f in All) UpdateState(f);
        // 7. dziennik
        if (Tick % 5 == 0) Log.Snapshot(this);
        CheckEnd();
    }

    // ---------------- percepcja ----------------
    public float SightRange(Fighter f, Fighter target)
    {
        float light = LightAt(target.Pos);
        float baseDark, baseLit;
        if (f.Creature != null) { baseDark = f.Creature.SightDark; baseLit = f.Creature.SightLit; }
        else { baseDark = 4f + f.S.Perception * 0.5f; baseLit = 35f + f.S.Perception * 2f; }
        float r = baseDark + (baseLit - baseDark) * MathX.Clamp01((light - 0.1f) / 0.6f);
        if (target.Kind == Kind.Krzykacz) r = MathF.Max(r, 14f);   // świecące gruczoły i hałas
        if (target.Kind == Kind.Nosiciel) r = MathF.Max(r, 16f);   // fosforyzujące wrzody: widać go z daleka
        if (Time < target.RevealedUntil) r = MathF.Max(r, 26f);   // po krzyku gruczoły świecą
        if (target.Kind == Kind.Rat) r *= 0.8f;
        if (target.Crouch) r *= 0.85f;
        return r;
    }

    void Perceive(Fighter f)
    {
        f.Seen.Clear();
        float half = f.IsHuman ? MathF.PI * (0.55f + 0.03f * f.S.Perception) : MathF.PI;   // ludzie ~110° w każdą stronę
        foreach (var t in All)
        {
            if (t.Team == f.Team || !t.Active) continue;
            var d = t.Pos - f.Pos; float dist = d.Length;
            if (dist > 75f) continue;
            float ang = MathF.Abs(MathX.AngleDiff(f.Facing, d.Angle));
            bool inCone = ang <= half || dist < 2.0f;
            if (!inCone || dist > SightRange(f, t)) continue;
            if (!LineOfSight.Clear(Map, f.Pos, t.Pos)) continue;
            if (!f.Seen.Contains(t.Id)) { if (f.LastSeenEnemyAt < Time - 3f) f.ReactionReadyAt = Time + ReactionDelay(f); }
            f.Seen.Add(t.Id); f.LastSeenEnemyAt = Time;
        }
        // słuch: hałasy wrogów w zasięgu (pozycja przybliżona)
        foreach (var n in Noises)
        {
            if (n.Team == f.Team) continue;
            float hearR = n.Radius * (0.7f + f.S.Perception * 0.06f);
            if (Vec2.Dist(n.Pos, f.Pos) < hearR)
                f.Heard[-1 - (int)(n.Pos.X * 10) - (int)(n.Pos.Y * 1000)] = (n.Pos + new Vec2(Rng.Range(-2, 2), Rng.Range(-2, 2)), Time);
        }
        // bliskie kroki: tak działa „usłyszał coś za plecami”
        foreach (var t in All)
        {
            if (t.Team == f.Team || !t.Active || f.Seen.Contains(t.Id)) continue;
            float dist = Vec2.Dist(t.Pos, f.Pos);
            float stepR = t.Kind == Kind.Rat ? 4f : t.Kind == Kind.Ogar && t.Vel.Length > 5 ? 9f : 5f;
            if (dist < stepR * (0.6f + f.S.Perception * 0.08f)) f.Heard[t.Id] = (t.Pos, Time);
        }
        var old = f.Heard.Where(kv => Time - kv.Value.t > 2.5f).Select(kv => kv.Key).ToList();
        foreach (var k in old) f.Heard.Remove(k);
    }

    public float ReactionDelay(Fighter f) => f.IsHuman ? MathX.Clamp(1.0f - f.Tactical * 0.15f - f.S.Perception * 0.03f, 0.15f, 1.2f) : 0.2f;

    public void MakeNoise(Vec2 p, float radius, Team team, string what) => Noises.Add(new Noise(p, radius, Time, team, what));

    // ---------------- ruch ----------------
    void Move(Fighter f)
    {
        if (f.CarriedById >= 0 || f.Status != Status.Ok) return;
        if (!f.HasMoveTarget) { f.Vel *= 0.5f; return; }
        var goal = f.MoveTarget;
        if (Vec2.Dist(goal, f.Pos) < 0.25f) { f.Vel = Vec2.Zero; return; }
        // ścieżka A* przeliczana, gdy cel się przesunął albo co 1,5 s
        bool direct = Vec2.Dist(goal, f.Pos) < 3f && LineOfSight.Clear(Map, f.Pos, goal) && Map.Walkable(goal);
        Vec2 next;
        if (direct) { next = goal; f.Path = null; }
        else
        {
            bool flow = !f.IsHuman && Vec2.Dist(goal, Map.LampPos) < 17.5f && Vec2.Dist(f.Pos, Map.LampPos) > 15.2f;
            if (flow) { next = Map.FlowStep(f.Pos); f.Path = null; }
            else
            {
            if (f.Path == null || Vec2.Dist(f.PathGoal, goal) > MathF.Max(1.0f, Vec2.Dist(f.Pos, goal) * 0.25f) || Time - f.PathAt > 1.5f)
            {
                f.Path = Paths.Find(f.Pos, goal, f.IsHuman ? DangerCost(f) : null, 9000); f.PathIndex = 1; f.PathGoal = goal; f.PathAt = Time;
            }
            if (f.Path == null || f.Path.Count < 2) { next = goal; }
            else
            {
                while (f.PathIndex < f.Path.Count - 1 && Vec2.Dist(f.Path[f.PathIndex], f.Pos) < 0.45f) f.PathIndex++;
                next = f.Path[Math.Min(f.PathIndex, f.Path.Count - 1)];
            }
            }
        }
        bool run = f.Intent.StartsWith("bieg") || f.Intent.StartsWith("ucieka") || f.Intent.StartsWith("szarża") || Vec2.Dist(goal, f.Pos) > 4f;
        float speed = f.Speed(run);
        if (f.Brain is Latarnia.Sim.AI.BeastBrain bb && Time < bb.FrenzyUntil) speed *= 1.25f;
        var dir = (next - f.Pos).Normalized();
        var step = dir * MathF.Min(speed * Dt, Vec2.Dist(next, f.Pos));
        var np = f.Pos + step;
        if (!Map.Walkable(np)) { np = f.Pos + new Vec2(step.X, 0); if (!Map.Walkable(np)) np = f.Pos + new Vec2(0, step.Y); if (!Map.Walkable(np)) np = f.Pos; }
        if ((Map.Get(np) & Cell.LowCover) != 0) np = f.Pos + step * 0.35f;   // przełażenie
        f.Vel = (np - f.Pos) / Dt; f.Pos = np;
        if (f.Vel.LengthSq > 0.05f && f.TargetId < 0) f.Facing = MathX.MoveTowardsAngle(f.Facing, f.Vel.Angle, 6f * Dt);
        if (run && f.IsHuman) f.Fatigue += Dt * (1.2f - f.S.Endurance * 0.06f);
    }

    /// <summary>Koszt ścieżki dla ludzi: nie wchodzić na linię strzału swoich (ogień bratobójczy) i w znane strefy wybuchu.</summary>
    Func<int, int, float>? DangerCost(Fighter f)
    {
        var shooters = All.Where(o => o.Team == f.Team && o.Active && o.Id != f.Id && o.Current.IsRanged && o.TargetId >= 0).ToList();
        var bombs = All.Where(o => o.Active && o.Kind == Kind.Nosiciel && o.Team != f.Team).Select(o => o.Pos).ToList();
        if (shooters.Count == 0 && bombs.Count == 0) return null;
        var lines = new List<(Vec2 a, Vec2 b)>();
        foreach (var s in shooters) if (TryGet(s.TargetId, out var t)) lines.Add((s.Pos, t.Pos));
        return (x, y) =>
        {
            var p = Map.CellCenter(x, y); float c = 0;
            foreach (var (a, b) in lines) if (DistToSegment(p, a, b) < 0.8f) c += 3f;
            foreach (var b in bombs) if (Vec2.Dist(p, b) < 5f) c += 6f;
            return c;
        };
    }

    static float Mass(Fighter f) => f.Kind switch { Kind.Rat => 0.12f, Kind.Ogar => 0.8f, Kind.Krzykacz => 1.2f, Kind.Nosiciel => 1.5f, _ => f.Order.Type == OrderType.Hold && f.Vel.LengthSq < 0.3f ? 2.5f : 1f };

    void Separate()
    {
        for (int i = 0; i < All.Count; i++)
        {
            var a = All[i]; if (a.Status == Status.Dead || a.CarriedById >= 0) continue;
            for (int j = i + 1; j < All.Count; j++)
            {
                var b = All[j]; if (b.Status == Status.Dead || b.CarriedById >= 0) continue;
                float min = a.Radius + b.Radius; var d = b.Pos - a.Pos; float dl = d.Length;
                if (dl < min && dl > 1e-4f)
                {
                    // masa: człowiek stoi jak ściana wobec szczura, Ogar go przepycha tylko trochę
                    float ma = Mass(a), mb = Mass(b);
                    bool aFixed = a.Status != Status.Ok, bFixed = b.Status != Status.Ok;
                    if (aFixed) ma = 50; if (bFixed) mb = 50;
                    var push = d / dl * (min - dl);
                    float ka = mb / (ma + mb), kb = ma / (ma + mb);
                    if (Map.Walkable(a.Pos - push * ka)) a.Pos -= push * ka;
                    if (Map.Walkable(b.Pos + push * kb)) b.Pos += push * kb;
                }
            }
        }
    }

    // ---------------- ataki ----------------
    public bool CanStrike(Fighter a, Fighter t) => Vec2.Dist(a.Pos, t.Pos) <= a.Current.Reach + t.Radius + a.Radius * 0.5f;

    public void Strike(Fighter a, Fighter t)
    {
        if (a.Cooldown > 0) return;
        var w = a.Current;
        a.Cooldown = w.Interval * (1f + MathX.Clamp01(a.Fatigue / 100f) * 0.4f) * (a.ArmDisabled ? 1.6f : 1f);
        a.Facing = (t.Pos - a.Pos).Angle;
        MakeNoise(a.Pos, w.Noise, a.Team, "cios");
        int atk = a.Creature?.MeleeSkill ?? a.S.Melee, def = t.Creature?.MeleeSkill ?? t.S.Melee;
        float p = 0.46f + (atk - def) * 0.05f;
        // tarcza działa, gdy obrońca jest zwrócony do napastnika
        if (t.Current.Block > 0 && MathF.Abs(MathX.AngleDiff(t.Facing, (a.Pos - t.Pos).Angle)) < 1.2f) p -= t.Current.Block * 0.11f + ShieldWallBonus(t);
        // dzida zatrzymuje szarżę: pierwszy cios w nacierające zwierzę z premią
        if (w.StopsCharge && !t.IsHuman && t.Vel.Length > 4f) p += 0.22f;
        if (t.Status != Status.Ok) p += 0.3f;
        // cios z boku i od tyłu: tu działa szyk (krąg i linia nie mają odsłoniętych pleców)
        if (t.Status == Status.Ok)
        {
            float back = MathF.Abs(MathX.AngleDiff(t.Facing, (a.Pos - t.Pos).Angle));
            if (back > 2.0f) p += 0.22f; else if (back > 1.3f) p += 0.1f;
            int ganging = 0;
            foreach (var o in All) if (o != a && o.Active && o.Team == a.Team && Vec2.Dist(o.Pos, t.Pos) < 1.8f) ganging++;
            p += MathF.Min(ganging, 3) * 0.05f;
        }
        if (t.Crouch) p -= 0.05f;
        p -= MathX.Clamp01(a.Fatigue / 100f) * 0.15f;
        if (!Rng.Chance(MathX.Clamp(p, 0.08f, 0.95f))) return;
        Hit(a, t, w, melee: true);
        if (w.Stuns && t.IsHuman && Rng.Chance(0.25f)) t.StunUntil = Time + 1.2f;
        if (w.Stuns && !t.IsHuman && t.Kind != Kind.Rat) t.StunUntil = Time + 0.5f;
    }

    float ShieldWallBonus(Fighter t)
    {
        // mur tarcz: sąsiad z tarczą w odległości do 1,3 m daje dodatkowy blok
        int n = 0;
        foreach (var o in All) if (o != t && o.Active && o.Team == t.Team && o.Current.Block > 0 && Vec2.Dist(o.Pos, t.Pos) < 1.3f) n++;
        return n * 0.06f;
    }

    public void Shoot(Fighter a, Fighter t)
    {
        if (a.Cooldown > 0 || a.Ammo <= 0) return;
        var w = a.Current; float dist = Vec2.Dist(a.Pos, t.Pos);
        if (dist > w.MaxRange) return;
        a.Cooldown = w.Interval; a.Ammo--; a.Shots++;
        a.Facing = (t.Pos - a.Pos).Angle;
        MakeNoise(a.Pos, w.Noise, a.Team, "strzał");
        float p = HitChance(a, t, dist);
        bool hit = Rng.Chance(p);
        float flight = dist / w.ProjSpeed;
        var aim = t.Pos + t.Vel * flight * MathX.Clamp01(0.4f + a.S.Accuracy * 0.07f);
        Vec2 to;
        if (hit) to = aim;
        else
        {
            var dir = (aim - a.Pos).Normalized();
            float side = Rng.Range(0.7f, 2.2f) * (Rng.Chance(0.5f) ? 1 : -1);
            to = aim + dir.Perp * side + dir * Rng.Range(0f, 6f);
        }
        var len = Vec2.Dist(a.Pos, to);
        Projectiles.Add(new Projectile { ShooterId = a.Id, Team = a.Team, From = a.Pos, To = to, Pos = a.Pos, Speed = w.ProjSpeed, Total = len, TargetId = t.Id, WillHit = hit, Weapon = w });
    }

    public float HitChance(Fighter a, Fighter t, float dist)
    {
        var w = a.Current;
        float baseP = 0.22f + a.S.Accuracy * 0.045f;   // improwizowana broń, noc, stres
        float rangeF = dist <= w.EffRange * 0.3f ? 1f : dist <= w.EffRange ? 1f - (dist - w.EffRange * 0.3f) / (w.EffRange * 0.7f) * 0.45f : 0.55f - (dist - w.EffRange) / (w.MaxRange - w.EffRange) * 0.4f;
        float sizeF = 0.55f + 0.5f * t.Size;
        float moveF = 1f - MathX.Clamp01(t.Vel.Length / 10f) * 0.45f - MathX.Clamp01(a.Vel.Length / 4f) * 0.4f;
        float lightF = 0.5f + 0.5f * MathX.Clamp01(LightAt(t.Pos) / 0.6f);
        float coverF = 1f - Map.CoverFrom(t.Pos, a.Pos) * 0.55f;
        float stress = 1f - a.Suppression / 200f - MathX.Clamp01((60f - a.Morale) / 100f) * 0.4f;
        return MathX.Clamp(baseP * rangeF * sizeF * moveF * lightF * coverF * stress, 0.02f, 0.95f);
    }

    void UpdateProjectiles()
    {
        foreach (var pr in Projectiles)
        {
            if (pr.Done) continue;
            float step = pr.Speed * Dt; var dir = (pr.To - pr.From).Normalized();
            var prev = pr.Pos; pr.Pos += dir * step; pr.Travelled += step;
            // przeszkoda
            if (!Map.Walkable(pr.Pos) && (Map.Get(pr.Pos) & Cell.Blocked) != 0) { pr.Done = true; continue; }
            // kto stoi na torze (swój albo inny wróg) — pierwsza bryła na drodze
            foreach (var f in All)
            {
                if (f.Id == pr.ShooterId || f.Status == Status.Dead || f.CarriedById >= 0) continue;
                if (f.Id == pr.TargetId && pr.WillHit) continue;
                if (pr.Travelled < 1.2f) continue;
                float d = DistToSegment(f.Pos, prev, pr.Pos);
                if (d < f.Radius * 0.8f && Rng.Chance(f.Crouch ? 0.35f : 0.6f))
                {
                    var shooter = Get(pr.ShooterId);
                    Hit(shooter, f, pr.Weapon, melee: false);
                    if (f.Team != shooter.Team && f.IsHuman) f.Heard[shooter.Id] = (pr.From + new Vec2(Rng.Range(-3f, 3f), Rng.Range(-3f, 3f)), Time);
                    if (f.Team == shooter.Team) { shooter.FriendlyHits++; Log.Event(Time, LogKind.FriendlyFire, shooter.Id, f.Id, $"{shooter.Name} trafił swojego: {f.Name} [{shooter.Intent} → {f.Intent}, cel {(TryGet(pr.TargetId, out var tt) ? tt.Name : "?")} {(TryGet(pr.TargetId, out var t2) ? Vec2.Dist(t2.Pos, f.Pos) : 0):0.0} m]"); }
                    pr.Done = true; break;
                }
                else if (f.Team != pr.Team && d < 1.5f && f.IsHuman)
                {
                    f.Suppression = MathF.Min(100, f.Suppression + 4f);
                    // świst strzały zdradza, skąd strzelają (w przybliżeniu)
                    f.Heard[pr.ShooterId] = (pr.From + new Vec2(Rng.Range(-3f, 3f), Rng.Range(-3f, 3f)), Time);
                }
            }
            if (pr.Done) continue;
            if (pr.Travelled >= pr.Total)
            {
                pr.Done = true;
                if (pr.WillHit && TryGet(pr.TargetId, out var t) && t.Status != Status.Dead && Vec2.Dist(t.Pos, pr.To) < 0.9f + t.Radius)
                {
                    Hit(Get(pr.ShooterId), t, pr.Weapon, melee: false);
                    if (t.IsHuman && t.Team != pr.Team) t.Heard[pr.ShooterId] = (pr.From + new Vec2(Rng.Range(-3f, 3f), Rng.Range(-3f, 3f)), Time);
                }
            }
        }
        Projectiles.RemoveAll(p => p.Done && p.Travelled >= p.Total || p.Done);
    }

    public static float DistToSegment(Vec2 p, Vec2 a, Vec2 b)
    {
        var ab = b - a; float l2 = ab.LengthSq; if (l2 < 1e-6f) return Vec2.Dist(p, a);
        float t = MathX.Clamp01(Vec2.Dot(p - a, ab) / l2);
        return Vec2.Dist(p, a + ab * t);
    }

    // ---------------- rany ----------------
    public void Hit(Fighter a, Fighter t, WeaponDef w, bool melee)
    {
        if (t.Status == Status.Dead) return;
        a.DamageDealt += w.Damage;
        if (!t.IsHuman)
        {
            int dmg = w.Damage >= 1.1f || (w.Penetration >= 4) ? 2 : (Rng.Chance(w.Damage * 0.6f) ? 2 : 1);
            if (t.Kind == Kind.Rat) dmg = 1;
            t.HitsTaken += dmg;
            if (t.HitsTaken >= t.Creature!.Hp) Kill(t, a, w.NamePl);
            else if (t.Kind == Kind.Krzykacz || t.Kind == Kind.Ogar) EnemyBrains.OnWounded(this, t, a);
            return;
        }
        Zone z = RollZone(t, melee, a.Kind);
        int armor = z == Zone.Head ? t.ArmorHead : z == Zone.Torso ? t.ArmorTorso : 0;
        float sev = w.Damage * (1f - 0.2f * Math.Max(0, armor - w.Penetration)) * Rng.Range(0.6f, 1.4f) * (z == Zone.Head ? 1.5f : z == Zone.Torso ? 1.1f : 0.8f);
        var s = sev < 0.8f ? Severity.Light : sev < 1.45f ? Severity.Heavy : Severity.Fatal;
        t.Wounds.Add(new Wound(z, s, Time, w.NamePl));
        bool small = w == Weapons.RatBite;   // ugryzienie szczura: dużo drobnych ran, nie jedna wielka
        t.Morale -= small ? 2.5f : s == Severity.Light ? 8 : 18;
        switch (s)
        {
            case Severity.Light: t.Bleed += small ? 0.035f : 0.12f; if (z == Zone.Arms && Rng.Chance(0.2f)) t.ArmDisabled = true; break;
            case Severity.Heavy:
                t.Bleed += 0.45f;
                if (z == Zone.Legs) t.LegsDisabled = true;
                if (z == Zone.Arms) t.ArmDisabled = true;
                if ((z == Zone.Head && Rng.Chance(0.8f)) || (z == Zone.Torso && Rng.Chance(0.45f + (t.Brave ? -0.1f : 0))) || (z == Zone.Legs && Rng.Chance(0.3f)) || t.Wounds.Count(w => w.Severity != Severity.Light) >= 2) Incapacitate(t, a);
                break;
            case Severity.Fatal: t.Bleed += 2.6f; Incapacitate(t, a); break;
        }
        if (a.Kind == Kind.Ogar && z == Zone.Legs && t.Status == Status.Ok && Rng.Chance(0.35f)) t.StunUntil = Time + 1.0f;   // przewraca
        Log.Event(Time, LogKind.Wound, a.Id, t.Id, $"{a.Name} → {t.Name}: {ZonePl(z)}, {SevPl(s)} ({w.NamePl})");
        var cmd = t.Squad(this).Commander;
        if (cmd != null && !cmd.FirstKnown.ContainsKey(a.Id)) cmd.Surprised = true;
    }

    Zone RollZone(Fighter t, bool melee, Kind attacker)
    {
        float r = Rng.Next();
        if (attacker == Kind.Rat || attacker == Kind.Ogar) return r < 0.6f ? Zone.Legs : r < 0.85f ? Zone.Arms : Zone.Torso;
        if (t.Crouch && !melee) return r < 0.2f ? Zone.Head : r < 0.6f ? Zone.Torso : r < 0.9f ? Zone.Arms : Zone.Legs;
        return r < 0.1f ? Zone.Head : r < 0.55f ? Zone.Torso : r < 0.75f ? Zone.Arms : Zone.Legs;
    }

    public void Incapacitate(Fighter t, Fighter? by)
    {
        if (t.Status != Status.Ok) return;
        t.Status = Status.Down; t.DownAt = Time; t.Order = new Order(); t.HasMoveTarget = false; t.TargetId = -1;
        if (t.CarryingId >= 0 && TryGet(t.CarryingId, out var c)) { c.CarriedById = -1; t.CarryingId = -1; }
        if (by != null && by.Team != t.Team) by.Kills++;
        Log.Event(Time, LogKind.Down, by?.Id ?? -1, t.Id, $"{t.Name} pada ({(by != null ? by.Name : "?")})");
        foreach (var o in All)
            if (o.Team == t.Team && o.Active && o.IsHuman)
            {
                float d = Vec2.Dist(o.Pos, t.Pos);
                o.Morale -= d < 10 ? 12 : 5;
                if (t.Squad(this).Commander?.Leader == t) o.Morale -= 15;
            }
    }

    public void Kill(Fighter t, Fighter? by, string how)
    {
        if (t.Status == Status.Dead) return;
        var was = t.Status;
        t.Status = Status.Dead; t.DiedAt = Time; t.HasMoveTarget = false;
        if (t.CarriedById >= 0 && TryGet(t.CarriedById, out var c)) { c.CarryingId = -1; t.CarriedById = -1; }
        if (by != null && by.Team != t.Team) { if (was != Status.Down) by.Kills++; if (by.IsHuman) by.Morale = MathF.Min(100, by.Morale + (t.Kind == Kind.Rat ? 1 : 5)); }
        Log.Event(Time, LogKind.Kill, by?.Id ?? -1, t.Id, $"{(by != null ? by.Name : "?")} zabija: {t.Name} ({how})");
        if (t.Kind == Kind.Nosiciel) EnemyBrains.Explode(this, t);
        if (t.IsHuman && was == Status.Ok)
            foreach (var o in All) if (o.Team == t.Team && o.Active && o.IsHuman && Vec2.Dist(o.Pos, t.Pos) < 12) o.Morale -= 10;
        if (t.Role == Role.Herszt) foreach (var o in All) if (o.Team == t.Team && o.Active) o.Morale -= 45;   // banda ucieka po śmierci Herszta
    }

    void UpdateState(Fighter f)
    {
        if (f.Cooldown > 0) f.Cooldown -= Dt;
        if (f.Status == Status.Dead) return;
        if (f.IsHuman)
        {
            if (f.Bleed > 0 && f.Status != Status.Fled)
            {
                f.Blood -= f.Bleed * Dt * (f.Stabilized ? 0.08f : 1f);
                if (f.Blood <= 0) { Kill(f, null, "wykrwawienie"); return; }
                if (f.Blood < 35 && f.Status == Status.Ok) Incapacitate(f, null);
            }
            if (f.Status == Status.Ok)
            {
                f.Suppression = MathF.Max(0, f.Suppression - Dt * (f.Crouch ? 14f : 8f));
                f.Fatigue = MathF.Max(0, f.Fatigue - Dt * 0.6f);
                // morale wraca powoli przy dowódcy i bez wroga w pobliżu
                var leader = f.Squad(this).Commander?.Leader;
                float regen = (leader != null && leader.Active && Vec2.Dist(leader.Pos, f.Pos) < 12 ? 0.9f : 0.3f) * (0.6f + f.S.Nerve * 0.08f);
                if (Time - f.LastSeenEnemyAt > 4) regen *= 2f;
                if (LightAt(f.Pos) < 0.2f) regen -= 0.4f;   // strach przed ciemnością
                f.Morale = MathX.Clamp(f.Morale + regen * Dt, 0, 100);
                if (f.LastSeenEnemyAt > Time - 1) f.TimeInCombat += Dt;
                if (f.Morale < 8 && f.Team == Team.Enemy && Time > 5) { f.Status = Status.Fled; Log.Event(Time, LogKind.Fled, f.Id, -1, $"{f.Name} ucieka z pola walki"); }
            }
        }
        if (f.Status == Status.Ok && f.Team == Team.Enemy && Vec2.Dist(f.Pos, Map.LampPos) > 45f && f.Intent.StartsWith("ucieka")) f.Status = Status.Fled;
        if (f.Status == Status.Ok && f.Team == Team.Squad && f.Intent.StartsWith("ucieka") && Vec2.Dist(f.Pos, Map.LampPos) > 40f) f.Status = Status.Fled;
        if (f.CarriedById >= 0 && TryGet(f.CarriedById, out var carrier)) f.Pos = carrier.Pos + Vec2.FromAngle(carrier.Facing + MathF.PI) * 0.4f;
    }

    void CheckEnd()
    {
        bool squadUp = All.Any(f => f.Team == Team.Squad && f.Active);
        bool enemyUp = All.Any(f => f.Team == Team.Enemy && f.Active);
        if (!enemyUp && PendingSpawns <= 0) { Over = true; Winner = Team.Squad; EndReason = "wrogowie pokonani"; }
        else if (!squadUp) { Over = true; Winner = Team.Enemy; EndReason = "skład wyłączony z walki"; }
        else if (Time >= MaxTime) { Over = true; Winner = null; EndReason = "koniec czasu"; }
        if (Over)
        {
            // po walce: ranni bez opieki mają ostatnią szansę (ranni stabilizowani przeżywają)
            foreach (var f in All) if (f.IsHuman && f.Status == Status.Down && !f.Stabilized && f.Blood < 50 && Winner != f.Team) Kill(f, null, "pozostawiony");
            Log.Snapshot(this);
        }
    }

    public static string ZonePl(Zone z) => z switch { Zone.Head => "głowa", Zone.Torso => "tors", Zone.Arms => "ręka", _ => "noga" };
    public static string SevPl(Severity s) => s switch { Severity.Light => "lekka rana", Severity.Heavy => "ciężka rana", _ => "rana śmiertelna" };
}

public static class FighterExt
{
    public static Squad Squad(this Fighter f, Battle b) => b.Squads[f.SquadIndex];
}
