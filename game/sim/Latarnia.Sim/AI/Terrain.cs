using System;
using System.Collections.Generic;
using System.Linq;
using Latarnia.Sim.Combat;
using Latarnia.Sim.Core;
using Latarnia.Sim.Units;
using Latarnia.Sim.World;

namespace Latarnia.Sim.AI;

/// <summary>Orientacja w terenie: wybór pozycji. Liczba próbek rośnie z Terenem, od 3 dowódca używa przesmyków.</summary>
public static class Terrain
{
    public static Vec2 PickPosition(Battle b, Commander c, CTask t, Vec2 core)
    {
        var g = b.Map; var P = c.P; var th = t.Threat;
        if (th == null) return t.Pos;
        var tp = th.Center + th.Vel * P.Predict;
        float dThreat = Vec2.Dist(tp, core);
        switch (t.Type)
        {
            case TaskType.Hold:
            {
                if (P.UsesChokepoints && !b.Squads[c.Leader.SquadIndex].Attacking && !th.Explosive && !th.Fast)
                {
                    // zagrożenie za palisadą: czy jego droga do rdzenia idzie przez przesmyk?
                    if (Vec2.Dist(tp, g.LampPos) > 15.5f)
                    {
                        var best = g.Chokepoints.OrderBy(cp => Vec2.Dist(cp, tp) + Vec2.Dist(cp, core)).First();
                        float via = Vec2.Dist(best, tp) + Vec2.Dist(best, core);
                        if (via < dThreat * 1.6f + 6f)
                        {
                            var inward = (g.LampPos - best).Normalized();
                            t.Facing = (-inward).Angle;
                            // przesmyk w ciemności to walka na ślepo: stań na skraju światła na drodze od przesmyku
                            for (float k = 1.6f; k < 14f; k += 0.5f)
                            {
                                var p = best + inward * k;
                                if (g.Walkable(p) && b.LightAt(p) >= 0.55f) return Walkable(g, p);
                            }
                            return Walkable(g, best + inward * 1.6f);
                        }
                    }
                }
                // próbki na łuku między rdzeniem a zagrożeniem
                var dir = (tp - core).Normalized();
                float reach = MathF.Min(MathF.Max(2.5f, dThreat - 3f), th.Explosive ? 3f : 7f);
                return Sample(b, c, core, dir, 2.5f, reach, 0.9f, tp, scoreKind: 0);
            }
            case TaskType.Assault:
                return Walkable(g, tp);
            case TaskType.FireSupport when b.Squads[c.Leader.SquadIndex].Attacking:
            {
                // atakujący strzelcy: pierścień wokół celu, z widokiem na niego, najchętniej w cieniu
                Vec2 best = tp; float bs = float.MinValue;
                var from = core;
                for (int i = 0; i < Math.Max(8, P.PositionSamples); i++)
                {
                    float ang = (from - tp).Angle + ((i * 0.618f) % 1f - 0.5f) * 2.4f;
                    float r = 10f + (i % 4) * 2f;
                    var p = tp + Vec2.FromAngle(ang) * r;
                    if (!g.Walkable(p)) continue;
                    float sc = (LineOfSight.Clear(g, p, tp) ? 3f : -4f) - b.LightAt(p) * 1.2f - Vec2.Dist(p, from) * 0.05f + g.CoverFrom(p, tp);
                    if (c.P.Teren <= 2) sc += b.Rng.Range(-1f, 1f) * (3 - c.P.Teren);
                    if (sc > bs) { bs = sc; best = p; }
                }
                return best;
            }
            case TaskType.FireSupport:
            {
                var dir = (tp - core).Normalized();
                // za linią, z widokiem na zagrożenie
                float reach = MathF.Min(MathF.Max(1.5f, dThreat - 9f), 7f);
                return Sample(b, c, core, dir, 0.5f, reach, 1.3f, tp, scoreKind: 1);
            }
            case TaskType.Eliminate:
            case TaskType.KillAtRange:
            {
                var dir = (tp - core).Normalized();
                float want = t.Type == TaskType.KillAtRange ? 12f : 16f;
                float reach = MathF.Max(1f, MathF.Min(dThreat - want, 9f));
                return Sample(b, c, core, dir, 0f, reach, 1.6f, tp, scoreKind: 2);
            }
            case TaskType.Avoid:
                return Walkable(g, core - (tp - core).Normalized() * 3f);
        }
        return t.Pos;
    }

    static Vec2 Sample(Battle b, Commander c, Vec2 core, Vec2 dir, float rMin, float rMax, float spread, Vec2 threat, int scoreKind)
    {
        var g = b.Map; int n = c.P.PositionSamples;
        Vec2 best = Walkable(g, core + dir * ((rMin + rMax) / 2)); float bestS = float.MinValue;
        // słabszy teren: mniej próbek i z szumem (gorsze oko do pozycji)
        for (int i = 0; i < n; i++)
        {
            float r = rMin + (rMax - rMin) * ((i * 0.618f) % 1f);
            float a = ((i * 0.381f) % 1f - 0.5f) * 2f * spread;
            var p = core + dir.Rotated(a) * r;
            if (!g.Walkable(p)) continue;
            float s = 0;
            float light = b.LightAt(p);
            float cover = g.CoverFrom(p, threat);
            bool los = LineOfSight.Clear(g, p, threat);
            s += light * 2f;                                  // w świetle widać, w ciemności giną
            s -= Vec2.Dist(p, core) * 0.08f;
            switch (scoreKind)
            {
                case 0: s += cover * 0.8f + (los ? 0.5f : -0.5f); s -= OpenSides(g, p) * 0.15f; break;   // linia: oparcie o przeszkody
                case 1: s += (los ? 2.5f : -3f) + cover * 1.5f; s -= MathF.Abs(Vec2.Dist(p, threat) - 14f) * 0.05f; break;
                case 2: s += (los ? 3f : -4f) + cover * 1.2f; s -= MathF.Abs(Vec2.Dist(p, threat) - 15f) * 0.1f; break;
            }
            if (c.P.Teren <= 2) s += b.Rng.Range(-1f, 1f) * (3 - c.P.Teren);
            if (s > bestS) { bestS = s; best = p; }
        }
        return best;
    }

    static int OpenSides(Grid g, Vec2 p)
    {
        int n = 0;
        for (int k = 0; k < 8; k++) if (g.Walkable(p + Vec2.FromAngle(k * MathF.PI / 4) * 1.5f)) n++;
        return n;
    }

    public static Vec2 Walkable(Grid g, Vec2 p)
    {
        if (g.Walkable(p)) return p;
        for (float r = 0.5f; r < 4f; r += 0.5f)
            for (int k = 0; k < 12; k++) { var q = p + Vec2.FromAngle(k * MathF.PI / 6) * r; if (g.Walkable(q)) return q; }
        return p;
    }
}

/// <summary>Taktyka i formacje: rozstawienie ludzi w zadaniu (tłum, linia, mur tarcz, jeż).</summary>
public static class Formations
{
    public static void Arrange(Battle b, Commander c, Plan plan, Vec2 core, CTask? only = null)
    {
        var g = b.Map; int F = c.P.Formacje;
        // jeż: zagrożenia z wielu stron i Formacje ≥ 3
        var holds = plan.Tasks.Where(t => t.Type == TaskType.Hold && t.Threat != null).ToList();
        bool hedgehog = F >= 3 && c.LastThreats.Count >= 3 && Spread(c.LastThreats, core) > 2.3f;
        // Nosiciel w pobliżu: rozproszyć szyk (od Formacji 2 dowódca wie, że skupisko to cel)
        bool bomb = F >= 2 && c.LastThreats.Any(t => t.Explosive && t.Distance < 14f);
        float spreadK = bomb ? (F >= 3 ? 2.2f : 1.6f) : 1f;
        foreach (var t in plan.Tasks)
        {
            if (only != null && t != only) continue;
            t.Slots.Clear(); t.SlotFacing.Clear();
            int n = t.Assigned.Count; if (n == 0) continue;
            if (t.Type is TaskType.Rescue || t.Type == TaskType.Watch) { foreach (var f in t.Assigned) t.Slots[f.Id] = t.Pos; continue; }
            if (t.Type == TaskType.Reserve && bomb)
            {
                int j = 0; foreach (var f in t.Assigned) { t.Slots[f.Id] = Terrain.Walkable(g, t.Pos + Vec2.FromAngle(j * 2.4f + 0.5f) * 3.5f); j++; }
                continue;
            }
            if (t.Key == "krag")
            {
                // krąg: walczący wręcz na obwodzie twarzą na zewnątrz, gęściej od strony głównego zagrożenia
                t.Formation = "krąg";
                var mainDir = t.Threat != null ? (t.Threat.Center - core).Angle : 0f;
                float rr = MathF.Max(1.3f, n * (F >= 3 ? 1.5f : 1.9f) / MathF.Tau) * spreadK;
                for (int i = 0; i < n; i++)
                {
                    float off = n == 1 ? 0 : n >= 4 ? i * MathF.Tau / n : (i - (n - 1) / 2f) * MathF.Min(MathF.Tau / n, F >= 3 ? 0.9f : 1.2f);
                    float a = mainDir + off;
                    var f = t.Assigned.OrderByDescending(x => x.Current.Block).ThenBy(x => x.Id).ElementAt(i);
                    t.Slots[f.Id] = Terrain.Walkable(g, core + Vec2.FromAngle(a) * rr); t.SlotFacing[f.Id] = a;
                }
                continue;
            }
            if (hedgehog && (t.Type == TaskType.Hold || t.Type == TaskType.Reserve))
            {
                t.Formation = "jeż";
                int i = 0;
                foreach (var f in t.Assigned)
                {
                    var dir = t.Threat != null ? (t.Threat.Center - core).Normalized() : Vec2.FromAngle(i * 2.1f);
                    t.Slots[f.Id] = Terrain.Walkable(g, core + dir.Rotated((i - (n - 1) / 2f) * 0.35f) * 3.2f); i++;
                }
                continue;
            }
            var facing = Vec2.FromAngle(t.Facing); var side = facing.Perp;
            if (F <= 1)
            {
                // tłum: każdy staje, gdzie mu wygodnie
                t.Formation = "tłum";
                foreach (var f in t.Assigned) t.Slots[f.Id] = Terrain.Walkable(g, t.Pos + new Vec2(b.Rng.Range(-1.5f, 1.5f), b.Rng.Range(-1.5f, 1.5f)));
                continue;
            }
            bool shield = t.Type == TaskType.Hold && t.Assigned.Count(f => f.Current.Block > 0) >= 1 && F >= 3;
            float spacing = (t.Type == TaskType.Hold ? (shield ? 1.05f : 1.35f) : 1.8f) * spreadK;
            if (bomb) shield = false;
            t.Formation = t.Type == TaskType.Hold ? (shield ? "mur tarcz" : "linia") : "rozproszenie";
            // tarczownicy na środku linii, reszta po bokach
            var order = t.Type == TaskType.Hold ? t.Assigned.OrderByDescending(f => f.Current.Block).ThenBy(f => f.Id).ToList() : t.Assigned.OrderBy(f => f.Id).ToList();
            for (int i = 0; i < order.Count; i++)
            {
                int k = i == 0 ? 0 : (i % 2 == 1 ? (i + 1) / 2 : -(i / 2));
                var p = t.Pos + side * (k * spacing);
                if (t.Type != TaskType.Hold && i > 0) p -= facing * 0.6f * (i % 2);
                t.Slots[order[i].Id] = Terrain.Walkable(g, p);
            }
        }
    }

    static float Spread(List<Threat> th, Vec2 core)
    {
        // największy kąt między kierunkami zagrożeń
        float best = 0;
        for (int i = 0; i < th.Count; i++) for (int j = i + 1; j < th.Count; j++)
            best = MathF.Max(best, MathF.Abs(MathX.AngleDiff((th[i].Center - core).Angle, (th[j].Center - core).Angle)));
        return best;
    }
}
