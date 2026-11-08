using System;
using System.Collections.Generic;
using System.Linq;
using Latarnia.Sim.Combat;
using Latarnia.Sim.Core;
using Latarnia.Sim.Data;
using Latarnia.Sim.Units;
using Latarnia.Sim.World;

namespace Latarnia.Sim.AI;

public enum BeastState { Approach, Stalk, Circle, Charge, Bite, Retreat, Scream, Flee }

public sealed class BeastBrain
{
    public BeastState State = BeastState.Approach;
    public float StateUntil; public int Prey = -1; public Vec2 Goal; public float FrenzyUntil; public float NextScream; public bool Exploded;
    public int Pack = -1; public float Side = 1;
}

/// <summary>Mózgi mutantów. Każdy gatunek ma własny pomysł na walkę, który dowódca musi rozpoznać.</summary>
public static class EnemyBrains
{
    public static readonly WeaponDef Blast = new("blast", "wybuch Nosiciela", WeaponKind.Melee, 3, 4f, 0, 1.15f, Noise: 30);

    public static void Think(Battle b, Fighter f)
    {
        var br = f.Brain as BeastBrain ?? (BeastBrain)(f.Brain = new BeastBrain { Side = f.Id % 2 == 0 ? 1 : -1 });
        if (br.State == BeastState.Flee) { Flee(b, f); return; }
        switch (f.Kind)
        {
            case Kind.Rat: Rat(b, f, br); break;
            case Kind.Ogar: Ogar(b, f, br); break;
            case Kind.Krzykacz: Krzykacz(b, f, br); break;
            case Kind.Nosiciel: Nosiciel(b, f, br); break;
        }
    }

    static IEnumerable<Fighter> Humans(Battle b, Fighter f) => b.All.Where(h => h.Team != f.Team && h.Status != Status.Dead && h.Status != Status.Fled && h.CarriedById < 0);

    static Fighter? Closest(Battle b, Fighter f, Func<Fighter, float>? bias = null)
    {
        Fighter? best = null; float bs = float.MaxValue;
        foreach (var h in Humans(b, f))
        {
            float d = Vec2.Dist(h.Pos, f.Pos);
            bool known = f.Seen.Contains(h.Id) || f.Heard.ContainsKey(h.Id) || d < 30f;   // mutanty czują ludzi w Mgle
            if (!known) continue;
            float s = d + (bias?.Invoke(h) ?? 0);
            if (s < bs) { bs = s; best = h; }
        }
        return best;
    }

    static void Go(Fighter f, Vec2 p, string intent) { f.MoveTarget = p; f.HasMoveTarget = true; f.Intent = intent; }

    static void Melee(Battle b, Fighter f, Fighter t)
    {
        f.TargetId = t.Id;
        if (b.CanStrike(f, t)) b.Strike(f, t);
    }

    // ---------------- szczury: rój ----------------
    static void Rat(Battle b, Fighter f, BeastBrain br)
    {
        // rój: najbliższy człowiek, leżący kuszą najbardziej; ucieka, gdy roju ubyło
        var t = Closest(b, f, h => h.Status == Status.Down ? -3f : 0f);
        int left = b.All.Count(e => e.Active && e.Kind == Kind.Rat && e.SquadIndex == f.SquadIndex);
        int total = b.All.Count(e => e.Kind == Kind.Rat && e.SquadIndex == f.SquadIndex);
        if (total > 4 && left <= total / 5) { br.State = BeastState.Flee; br.StateUntil = b.Time + 5f; return; }
        if (t == null) { Go(f, b.Map.LampPos, "bieg do światła"); return; }
        var sideStep = (t.Pos - f.Pos).Normalized().Perp * (((f.Id * 7) % 5 - 2) * 0.35f);   // rój rozlewa się na boki
        Go(f, Vec2.Dist(t.Pos, f.Pos) > 3 ? t.Pos + sideStep : t.Pos, "bieg");
        Melee(b, f, t);
    }

    // ---------------- Ogar: drapieżnik ----------------
    static void Ogar(Battle b, Fighter f, BeastBrain br)
    {
        float now = b.Time;
        // wybór ofiary: samotni, strzelcy, ranni; omija tarcze
        Fighter? prey = br.Prey >= 0 && b.TryGet(br.Prey, out var p0) && p0.Status != Status.Dead && p0.CarriedById < 0 ? p0 : null;
        if (prey == null || (br.State is BeastState.Stalk or BeastState.Approach && now > br.StateUntil))
        {
            prey = Closest(b, f, h =>
            {
                int friends = b.All.Count(o => o.Team == h.Team && o.Active && o != h && Vec2.Dist(o.Pos, h.Pos) < 3f);
                float s = friends * 4f;
                if (h.Current.IsRanged) s -= 4f;
                if (h.Current.Block > 0) s += 6f;
                if (h.Status == Status.Down) s -= 3f;
                if (h.Wounds.Count > 0) s -= 2f;
                return s;
            });
            br.Prey = prey?.Id ?? -1;
        }
        if (prey == null) { Go(f, b.Map.LampPos, "węszy"); return; }
        float d = Vec2.Dist(prey.Pos, f.Pos);
        int pack = f.SquadIndex;
        switch (br.State)
        {
            case BeastState.Approach:
            case BeastState.Stalk:
            {
                // podchodzi skrajem ciemności na dystans skoku, po stronie, gdzie ofiara nie patrzy
                var away = (f.Pos - prey.Pos).Normalized();
                var behind = Vec2.FromAngle(prey.Facing + MathF.PI);
                var dir = (away * 0.5f + behind * 0.5f).Normalized();
                if (dir.LengthSq < 0.1f) dir = away;
                dir = dir.Rotated(br.Side * 0.4f);
                var goal = prey.Pos + dir * 10f;
                // trzyma się ciemności
                for (int i = 0; i < 6 && b.LightAt(goal) > 0.35f; i++) goal = prey.Pos + dir * (10f + i * 2f);
                goal = Terrain.Walkable(b.Map, goal);
                Go(f, goal, "skrada się");
                bool inPlace = Vec2.Dist(f.Pos, goal) < 2.5f || (d < 11f && b.LightAt(f.Pos) < 0.4f);
                b.Shared.TryGetValue($"pack{pack}", out var chargeAt);
                if (inPlace && br.State == BeastState.Approach) { br.State = BeastState.Stalk; br.StateUntil = now + 2.5f; }
                if ((br.State == BeastState.Stalk && now >= br.StateUntil) || (chargeAt > 0 && now >= chargeAt && now - chargeAt < 2f) || d < 4f)
                {
                    br.State = BeastState.Charge; br.StateUntil = now + 4f;
                    // stado rusza razem
                    if (chargeAt <= 0 || now - chargeAt > 3f) b.Shared[$"pack{pack}"] = now + 0.4f;
                }
                if (now - br.StateUntil > 10f && br.State == BeastState.Approach) { br.State = BeastState.Charge; br.StateUntil = now + 4f; }
                f.TargetId = -1;
                if (d < 1.6f) Melee(b, f, prey);
                break;
            }
            case BeastState.Charge:
                Go(f, prey.Pos + prey.Vel * 0.3f, "szarża");
                if (b.CanStrike(f, prey) && f.Cooldown <= 0)
                {
                    b.Strike(f, prey);
                    br.State = BeastState.Bite; br.StateUntil = now + b.Rng.Range(0.8f, 1.8f);
                }
                if (now > br.StateUntil) { br.State = BeastState.Retreat; br.StateUntil = now + 2.5f; }
                break;
            case BeastState.Bite:
                Go(f, prey.Pos, "gryzie"); Melee(b, f, prey);
                if (now > br.StateUntil) { br.State = BeastState.Retreat; br.StateUntil = now + b.Rng.Range(2f, 3.5f); br.Goal = Terrain.Walkable(b.Map, f.Pos + (f.Pos - prey.Pos).Normalized() * 8f); }
                break;
            case BeastState.Retreat:
                Go(f, br.Goal == default ? f.Pos + (f.Pos - prey.Pos).Normalized() * 8f : br.Goal, "odskok");
                f.TargetId = -1;
                if (now > br.StateUntil) { br.State = BeastState.Approach; br.StateUntil = now + 8f; br.Prey = -1; br.Side = -br.Side; }
                break;
        }
    }

    // ---------------- Krzykacz: wsparcie ----------------
    static void Krzykacz(Battle b, Fighter f, BeastBrain br)
    {
        var t = Closest(b, f);
        if (t == null) { Go(f, b.Map.LampPos, "idzie"); return; }
        float d = Vec2.Dist(t.Pos, f.Pos);
        float now = b.Time;
        if (br.State == BeastState.Retreat) { if (now < br.StateUntil) { Go(f, br.Goal, "cofa się"); return; } br.State = BeastState.Approach; }
        // trzyma 12–17 m od ofiary, w ciemności, ale z widokiem na nią (musi widzieć, kogo straszy)
        bool seesNow = d < 17.5f && d > 11f && LineOfSight.Clear(b.Map, f.Pos, t.Pos);
        if (!seesNow && now >= br.StateUntil)
        {
            Vec2 best = b.Map.FlowStep(f.Pos); float bs = float.MinValue;
            for (int i = 0; i < 16; i++)
            {
                var p = t.Pos + Vec2.FromAngle(i * MathF.Tau / 16) * (12f + (i % 3) * 2.5f);
                if (!b.Map.Walkable(p) || !LineOfSight.Clear(b.Map, p, t.Pos)) continue;
                float sc = -b.LightAt(p) * 4f - Vec2.Dist(p, f.Pos) * 0.08f;
                if (sc > bs) { bs = sc; best = p; }
            }
            br.Goal = best; br.StateUntil = now + 2f;
        }
        Go(f, seesNow ? f.Pos : br.Goal, d < 18.5f ? "wyje" : "zbliża się");
        if (d < 1.5f) Melee(b, f, t);
        if (now >= br.NextScream && d < 18f) Scream(b, f, br);
    }

    static void Scream(Battle b, Fighter f, BeastBrain br)
    {
        br.NextScream = b.Time + 12f;
        f.Say("AAAAA!", b.Time, 1.2f);
        b.MakeNoise(f.Pos, 40f, f.Team, "krzyk");
        b.Log.Event(b.Time, LogKind.Scream, f.Id, -1, "Krzykacz krzyczy"); b.Emit("scream", f.Id, -1, f.Pos);
        f.RevealedUntil = b.Time + 3.5f;
        foreach (var h in b.All)
        {
            if (!h.Active) continue;
            float d = Vec2.Dist(h.Pos, f.Pos);
            if (h.Team != f.Team && h.IsHuman && d < 40f) h.Heard[f.Id] = (f.Pos + new Vec2(b.Rng.Range(-1.5f, 1.5f), b.Rng.Range(-1.5f, 1.5f)), b.Time);
            if (h.Team != f.Team && h.IsHuman && d < 18f) { h.Morale -= h.Brave ? 6f : 12f; h.Odmiennosc += 3f; h.Suppression = MathF.Min(100, h.Suppression + 20); }
            if (h.Team == f.Team && !h.IsHuman && d < 22f && h.Brain is BeastBrain hb) hb.FrenzyUntil = b.Time + 5f;
        }
        foreach (var s in b.Squads) if (s.Team != f.Team && s.Commander != null && s.Members.Any(m => m.Active && Vec2.Dist(m.Pos, f.Pos) < 30)) s.Commander.ScreamsHeard++;
    }

    // ---------------- Nosiciel: bomba ----------------
    static void Nosiciel(Battle b, Fighter f, BeastBrain br)
    {
        // idzie w największe skupisko ludzi
        Vec2? best = null; int bestN = 0;
        foreach (var h in Humans(b, f))
        {
            int n = Humans(b, f).Count(o => Vec2.Dist(o.Pos, h.Pos) < 4f);
            if (n > bestN || (n == bestN && best != null && Vec2.Dist(h.Pos, f.Pos) < Vec2.Dist(best.Value, f.Pos))) { bestN = n; best = h.Pos; }
        }
        if (best == null) { Go(f, b.Map.LampPos, "idzie"); return; }
        Go(f, best.Value, "idzie na ludzi");
        if (Humans(b, f).Any(h => h.Status == Status.Ok && Vec2.Dist(h.Pos, f.Pos) < 1.8f)) b.Kill(f, null, "pęka");
    }

    public static void Explode(Battle b, Fighter f)
    {
        var br = f.Brain as BeastBrain; if (br != null && br.Exploded) return;
        if (br != null) br.Exploded = true;
        b.MakeNoise(f.Pos, 40f, f.Team, "wybuch"); b.Emit("explode", f.Id, -1, f.Pos);
        b.Log.Event(b.Time, LogKind.Explosion, f.Id, -1, $"Nosiciel wybucha ({b.LightAt(f.Pos):0.00} światła)");
        foreach (var h in b.All.ToList())
        {
            if (h == f || h.Status == Status.Dead) continue;
            float d = Vec2.Dist(h.Pos, f.Pos);
            if (d > 4f) continue;
            if (h.IsHuman)
            {
                h.StunUntil = b.Time + 1.5f; h.Morale -= 15; h.Odmiennosc += 8;
                if (d < 2.2f || b.Rng.Chance(0.5f)) b.Hit(f, h, Blast, melee: false);
            }
            else if (h.Kind == Kind.Rat) b.Kill(h, null, "wybuch");
        }
        foreach (var o in b.All) if (o.IsHuman && o.Active && Vec2.Dist(o.Pos, f.Pos) < 12f) o.Morale -= 6;
    }

    public static void OnWounded(Battle b, Fighter f, Fighter by)
    {
        if (f.Brain is not BeastBrain br) return;
        if (f.Kind == Kind.Krzykacz)
        {
            br.State = BeastState.Retreat; br.StateUntil = b.Time + 3.5f;
            br.Goal = Terrain.Walkable(b.Map, f.Pos + (f.Pos - by.Pos).Normalized() * 7f);
        }
        else if (f.Kind == Kind.Ogar && br.State != BeastState.Retreat)
        {
            br.State = BeastState.Retreat; br.StateUntil = b.Time + 3f;
            br.Goal = Terrain.Walkable(b.Map, f.Pos + (f.Pos - by.Pos).Normalized() * 9f);
        }
    }

    static void Flee(Battle b, Fighter f)
    {
        var br = (BeastBrain)f.Brain!;
        if (br.StateUntil < b.Time - 100) br.StateUntil = b.Time + 5f;   // pierwszy raz
        if (b.Time > br.StateUntil && b.LightAt(f.Pos) < 0.3f) { f.Status = Status.Fled; b.Log.Event(b.Time, LogKind.Fled, f.Id, -1, $"{f.Name} znika w Mgle"); return; }
        var away = (f.Pos - b.Map.LampPos).Normalized();
        Go(f, Terrain.Walkable(b.Map, b.Map.LampPos + away * 50f), "ucieka");
        if (Vec2.Dist(f.Pos, b.Map.LampPos) > 30f) { f.Status = Status.Fled; b.Log.Event(b.Time, LogKind.Fled, f.Id, -1, $"{f.Name} ucieka"); }
    }
}
