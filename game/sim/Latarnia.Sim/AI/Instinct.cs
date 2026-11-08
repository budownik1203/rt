using System;
using System.Collections.Generic;
using System.Linq;
using Latarnia.Sim.Combat;
using Latarnia.Sim.Core;
using Latarnia.Sim.Data;
using Latarnia.Sim.Units;
using Latarnia.Sim.World;

namespace Latarnia.Sim.AI;

sealed class InstinctState
{
    public float TreatUntil = -1; public int TreatTarget = -1;
    public float SwapUntil;
    public float SidestepUntil; public Vec2 Sidestep;
    public float LastReport = -99;
    public float BombWarned = -99;
    public bool OutOfAmmoSaid;
    public float FireRollUntil; public bool RiskyShotOk; public int SidestepTries; public float LastShotAt;
}

/// <summary>Instynkt wojownika: utility AI wykonujące rozkaz dowódcy i reagujące samodzielnie.
/// Im wyższe wyszkolenie taktyczne, tym szybsza reakcja, lepsza kontrola ognia i dyscyplina.</summary>
public static class Instinct
{
    public static void Think(Battle b, Fighter f)
    {
        var st = f.Brain as InstinctState ?? (InstinctState)(f.Brain = new InstinctState());
        if (f.CarryingId >= 0) { Carry(b, f, st); return; }
        if (b.Time >= st.TreatUntil && st.TreatTarget >= 0) FinishTreatment(b, f, st);
        if (st.TreatTarget >= 0)
        {
            // opatrywanie: przerywa tylko wróg tuż obok
            var near = NearestSeen(b, f, out float nd);
            if (near != null && nd < 2.2f) { st.TreatTarget = -1; st.TreatUntil = -1; }
            else { f.HasMoveTarget = false; f.Intent = "opatruje"; f.Crouch = true; return; }
        }
        if (b.Time >= f.NextThinkAt)
        {
            Decide(b, f, st);
            f.NextThinkAt = b.Time + 0.2f + (f.Id % 3) * 0.03f;
        }
        Act(b, f, st);
    }

    // ---------------- decyzja ----------------
    static void Decide(Battle b, Fighter f, InstinctState st)
    {
        var sq = f.Squad(b);
        var near = NearestSeen(b, f, out float nd);
        bool reacting = b.Time >= f.ReactionReadyAt;
        f.Crouch = false;

        // 1. morale: panika
        float panic = f.Brave ? 6 : f.Coward ? 25 : 14;
        if (f.Morale < panic && (near != null || f.Morale < panic * 0.6f))
        {
            f.Intent = "ucieka"; f.TargetId = -1;
            var away = near != null ? (f.Pos - near.Pos).Normalized() : (f.Pos - b.Map.LampPos).Normalized();
            if (f.Team == Team.Squad && near != null && Vec2.Dist(f.Pos, b.Map.LampPos) > 4) away = (b.Map.LampPos - f.Pos).Normalized() * 0.4f + away * 0.6f;
            f.MoveTarget = Terrain.Walkable(b.Map, f.Pos + away.Normalized() * 8f); f.HasMoveTarget = true;
            if (b.Rng.Chance(0.05f)) f.Say(Phrases.Pick(Phrases.Panic, b.Rng), b.Time);
            return;
        }
        // przygnieciony ogniem: kuli się
        if (f.Suppression > 60 && f.Tactical >= 2) f.Crouch = true;

        SwapWeapon(b, f, st, near, nd);

        // 2. Nosiciel blisko: odsuń się (wyszkolenie ≥ 2 albo widział wybuch)
        var bomb = b.All.FirstOrDefault(e => e.Active && e.Team != f.Team && e.Kind == Kind.Nosiciel && Vec2.Dist(e.Pos, f.Pos) < 5.5f && f.Seen.Contains(e.Id));
        if (bomb != null && (f.Tactical >= 2 || sq.Commander?.P.Level >= 3) && reacting)
        {
            if (b.Time - st.BombWarned > 6) { f.Say(Phrases.Pick(Phrases.Bomb, b.Rng), b.Time); st.BombWarned = b.Time; }
            var away = (f.Pos - bomb.Pos).Normalized();
            f.MoveTarget = Terrain.Walkable(b.Map, bomb.Pos + away * 7f); f.HasMoveTarget = true; f.Intent = "bieg od Nosiciela";
            if (f.Current.IsRanged && f.Ammo > 0) f.TargetId = bomb.Id;
            return;
        }

        // 3. wróg w zasięgu ręki: walczy (nie da się tego zignorować)
        if (near != null && nd < 2.6f && (reacting || nd < 1.4f))
        {
            f.TargetId = near.Id;
            if (f.Current.IsRanged) { f.Intent = "strzela z bliska"; f.HasMoveTarget = false; }
            else { f.Intent = "walczy"; Hold(f, StepTowards(f, near.Pos, f.Current.Reach * 0.75f)); }
            ReportIfOverwhelmed(b, f, st, sq);
            return;
        }

        // 4. rozkaz
        var o = f.Order;
        float discipline = 0.45f + f.Tactical * 0.1f + (f.Morale - 50) / 200f;
        if (o.Type != OrderType.None && (discipline > 0.4f || b.Rng.Chance(discipline)))
        {
            Execute(b, f, st, o, near, nd, reacting);
            ReportIfOverwhelmed(b, f, st, sq);
            return;
        }

        // 5. bez rozkazu: własna inicjatywa
        Freelance(b, f, st, near, nd, reacting);
    }

    static void Execute(Battle b, Fighter f, InstinctState st, Order o, Fighter? near, float nd, bool reacting)
    {
        switch (o.Type)
        {
            case OrderType.Hold:
            {
                // trzyma pozycję, ale bije wszystko, co podejdzie; słabo wyszkoleni gonią
                float leash = o.Tolerance * 1.5f + (f.Tactical <= 1 ? 6f : f.Tactical == 2 ? 3f : 1.5f);
                var center = o.Pos;
                if (o.Leash > 0) { center = o.Anchor; leash = MathF.Max(leash, o.Leash); }   // krąg: pomagają sobie nawzajem
                var target = BestMeleeTarget(b, f, center, leash + f.Current.Reach + 1f);
                if (target != null && reacting)
                {
                    f.TargetId = target.Id;
                    var tp = target.Pos;
                    if (Vec2.Dist(tp, center) < leash + f.Current.Reach) { Hold(f, StepTowards(f, tp, f.Current.Reach * 0.8f)); f.Intent = "walczy na pozycji"; }
                    else { Hold(f, o.Pos); f.Intent = "trzyma"; }
                }
                else
                {
                    f.TargetId = -1;
                    Hold(f, o.Pos); f.Intent = Vec2.Dist(f.Pos, o.Pos) > o.Tolerance + 0.4f ? "bieg na pozycję" : "trzyma";
                    if (f.Current.IsRanged && near != null && reacting) f.TargetId = near.Id;   // strzelec postawiony w linii strzela, póki może
                    Look(b, f, o.Facing, near);
                }
                if (Vec2.Dist(f.Pos, o.Pos) < o.Tolerance + 0.5f) f.TimeHolding += 0.2f;
                break;
            }
            case OrderType.FireSupport:
            case OrderType.Eliminate:
            case OrderType.Guard:
            {
                if (o.Type == OrderType.Eliminate && (!f.Main.IsRanged || f.Ammo <= 0))
                {
                    var prey = b.All.Where(e => e.Active && e.Team != f.Team && (o.PriorityKind == null || e.Kind == o.PriorityKind) && (f.Seen.Contains(e.Id) || f.Heard.ContainsKey(e.Id)))
                        .OrderBy(e => Vec2.Dist(e.Pos, f.Pos)).FirstOrDefault();
                    if (prey != null) { f.TargetId = prey.Id; Hold(f, prey.Pos); f.Intent = "szarża na cel"; break; }
                }
                if (!f.Main.IsRanged || f.Ammo <= 0)
                {
                    if (f.Ammo <= 0 && f.Main.IsRanged && !st.OutOfAmmoSaid) { st.OutOfAmmoSaid = true; f.Squad(b).Commander?.Report(b, f, Phrases.Pick(Phrases.NoAmmo, b.Rng), true); }
                    // bez amunicji: broń boczna i trzyma pozycję
                    var t2 = BestMeleeTarget(b, f, o.Pos, 4f);
                    f.TargetId = t2?.Id ?? -1; Hold(f, t2 != null ? StepTowards(f, t2.Pos, f.Current.Reach * 0.8f) : o.Pos);
                    f.Intent = t2 != null ? "walczy" : "trzyma"; break;
                }
                var target = BestRangedTarget(b, f, o.PriorityKind, o.Type == OrderType.Guard ? o.Facing : (float?)null);
                Vec2 pos = o.Pos;
                if (b.Time > st.SidestepUntil + 4f) st.SidestepTries = 0;
                if (target != null && reacting)
                {
                    f.TargetId = target.Id;
                    // kontrola ognia: swój na linii strzału
                    var blocker = FriendOnLine(b, f, target);
                    if (blocker != null)
                    {
                        if ((f.Tactical >= 3 || (f.Tactical == 2 && b.Rng.Chance(0.6f))) && b.Time > st.SidestepUntil + 2.5f * 0 && st.SidestepTries < 3)
                        {
                            if (b.Time > st.SidestepUntil) st.SidestepTries++;
                            float stepLen = 1.6f + st.SidestepTries * 1.2f;
                            if (b.Time > st.SidestepUntil)
                            {
                                var dir = (target.Pos - f.Pos).Normalized().Perp * (b.Rng.Chance(0.5f) ? 1 : -1);
                                st.Sidestep = Terrain.Walkable(b.Map, f.Pos + dir * stepLen); st.SidestepUntil = b.Time + 1.2f;
                            }
                            pos = st.Sidestep; f.Intent = "krok w bok (swój na linii)"; f.TargetId = -1;
                            Hold(f, pos); break;
                        }
                    }
                    if (blocker == null) st.SidestepTries = 0;
                    f.Intent = o.Type == OrderType.Eliminate ? "poluje na cel" : "strzela";
                    // nie może trafić z pozycji: podchodzi
                    if (Vec2.Dist(f.Pos, target.Pos) > f.Current.EffRange * 0.9f || !LineOfSight.Clear(b.Map, f.Pos, target.Pos))
                        pos = Vec2.Lerp(f.Pos, target.Pos, 0.3f);
                    else if (Vec2.Dist(f.Pos, o.Pos) < 2.5f || f.Cooldown <= 0.15f) pos = f.Pos;   // strzela z miejsca; w drodze staje do strzału, potem idzie dalej
                    if (f.Tactical >= 2 && f.Vel.LengthSq < 0.1f) f.Crouch = b.Map.CoverFrom(f.Pos, target.Pos) > 0;
                }
                else
                {
                    f.TargetId = -1; f.Intent = o.Type == OrderType.Guard ? "pilnuje" : Vec2.Dist(f.Pos, o.Pos) > 1.5f ? "bieg na pozycję" : "czeka na cel";
                    Look(b, f, o.Facing, null);
                }
                // długo bez strzału, a wróg jest: podejdź na skuteczną odległość z czystą linią
                if (b.Time - st.LastShotAt > 4f && o.Type != OrderType.Guard && b.Time - f.OrderReceivedAt > 3f)
                {
                    var any = NearestSeen(b, f, out float ad) ?? b.All.Where(e => e.Active && e.Team != f.Team && f.Heard.ContainsKey(e.Id)).OrderBy(e => Vec2.Dist(e.Pos, f.Pos)).FirstOrDefault();
                    if (any != null && (ad > 10f || FriendOnLine(b, f, any) != null || !f.Seen.Contains(any.Id)))
                    {
                        var side = (any.Pos - f.Pos).Normalized().Perp * ((f.Id % 2 == 0) ? 1 : -1);
                        pos = Terrain.Walkable(b.Map, Vec2.Lerp(f.Pos, any.Pos, 0.35f) + side * 2.5f);
                        f.Intent = "szuka linii strzału";
                    }
                }
                Hold(f, pos);
                break;
            }
            case OrderType.Rescue:
            {
                if (!b.TryGet(o.TargetId, out var w) || w.Status != Status.Down || w.CarriedById >= 0) { f.Order = new Order(); f.Intent = "ranny zabrany"; break; }
                f.TargetId = -1;
                if (Vec2.Dist(f.Pos, w.Pos) > 1.0f) { Hold(f, w.Pos); f.Intent = "bieg do rannego"; break; }
                int enemiesNear = b.All.Count(e => e.Active && e.Team != f.Team && Vec2.Dist(e.Pos, w.Pos) < 7f);
                if (!w.Stabilized && (enemiesNear == 0 || w.Bleed > 1.5f))
                {
                    st.TreatTarget = w.Id;
                    float dur = (f.Role == Role.Sanitariusz ? 2.5f : 5f) * (f.Medkits > 0 ? 1f : 1.6f);
                    st.TreatUntil = b.Time + dur; f.Say(Phrases.Pick(Phrases.Medic, b.Rng), b.Time, 1.5f);
                    f.HasMoveTarget = false; f.Intent = "opatruje";
                }
                else if (enemiesNear > 0 || Vec2.Dist(w.Pos, b.Map.LampPos) > 8f)
                {
                    f.CarryingId = w.Id; w.CarriedById = f.Id; f.Intent = "niesie rannego";
                }
                else { f.Order = new Order(); f.Intent = "ranny zaopatrzony"; }
                break;
            }
            case OrderType.Avoid:
                Hold(f, o.Pos); f.Intent = "odsuwa się"; f.TargetId = near != null && f.Current.IsRanged ? near.Id : -1; break;
            case OrderType.Advance:
            {
                var t = near != null && nd < 12f ? near : null;
                if (t != null && reacting)
                {
                    f.TargetId = t.Id;
                    if (f.Current.IsRanged && f.Ammo > 0) { f.Intent = "strzela"; Hold(f, nd < 8 ? f.Pos - (t.Pos - f.Pos).Normalized() * 2f : f.Pos); }
                    else { f.Intent = "szarża"; Hold(f, t.Pos); }
                }
                else { f.TargetId = -1; f.Intent = "naprzód"; Hold(f, o.Pos); if (f.Current.IsRanged && near != null && reacting) f.TargetId = near.Id; }
                break;
            }
            default:
            {
                if (f.Main.IsRanged && f.Ammo > 0)
                {
                    // strzelec w odwodzie: strzela z miejsca przy dowódcy, z kontrolą linii
                    Execute(b, f, st, new Order { Type = OrderType.FireSupport, Pos = o.Pos, Facing = o.Facing, TaskId = o.TaskId, Tolerance = o.Tolerance }, near, nd, reacting);
                    f.Intent = "odwód: " + f.Intent;
                    break;
                }
                // odwód: przy dowódcy, reaguje na bliskich wrogów
                if (near != null && nd < (f.Role == Role.Sanitariusz ? 0f : 4.5f) && reacting) { f.TargetId = near.Id; Hold(f, f.Current.IsRanged ? f.Pos : near.Pos); f.Intent = f.Current.IsRanged ? "strzela" : "walczy"; }
                else { f.TargetId = near != null && f.Current.IsRanged && reacting ? near.Id : -1; Hold(f, o.Pos); f.Intent = "odwód"; Look(b, f, o.Facing, near); }
                break;
            }
        }
    }

    /// <summary>Bez rozkazu (dowódca poziomu 1 albo chaos): każdy na własną rękę.</summary>
    static void Freelance(Battle b, Fighter f, InstinctState st, Fighter? near, float nd, bool reacting)
    {
        var sq = f.Squad(b);
        // sanitariusz i litościwi ratują sami, gdy jest w miarę bezpiecznie
        if (f.Role == Role.Sanitariusz || f.Merciful)
        {
            var down = sq.Members.Where(m => m.Status == Status.Down && m.CarriedById < 0 && !m.Stabilized)
                .OrderBy(m => Vec2.Dist(m.Pos, f.Pos)).FirstOrDefault();
            if (down != null && b.All.Count(e => e.Active && e.Team != f.Team && Vec2.Dist(e.Pos, down.Pos) < 4f) == 0)
            {
                Execute(b, f, st, new Order { Type = OrderType.Rescue, TargetId = down.Id, Pos = down.Pos }, near, nd, reacting); return;
            }
        }
        if (near != null && reacting)
        {
            f.TargetId = near.Id;
            if (f.Current.IsRanged && f.Ammo > 0)
            {
                f.Intent = "strzela";
                // bez kontroli ognia przy niskim wyszkoleniu
                if (f.Tactical >= 3 && FriendOnLine(b, f, near) != null) { f.TargetId = -1; Hold(f, Terrain.Walkable(b.Map, f.Pos + (near.Pos - f.Pos).Normalized().Perp * 1.5f)); }
                else Hold(f, f.Pos);
            }
            else if (sq.Attacking || nd < 14f || f.Tactical <= 2)
            {
                f.Intent = "szarża"; Hold(f, near.Pos);
            }
            else { f.Intent = "czeka"; Hold(f, f.Pos); }
            return;
        }
        f.TargetId = -1;
        if (sq.Attacking) { Hold(f, sq.Anchor); f.Intent = "naprzód"; return; }
        // obrońcy bez wroga w polu widzenia: zbijają się przy latarni, odwracają się na hałas
        var heard = f.Heard.Values.OrderByDescending(h => h.t).FirstOrDefault();
        if (heard.t > 0 && b.Time - heard.t < 1.5f && (f.Tactical >= 3 || b.Rng.Chance(0.3f))) f.Facing = MathX.MoveTowardsAngle(f.Facing, (heard.pos - f.Pos).Angle, 1.2f);
        var anchor = sq.Anchor + Vec2.FromAngle(f.Id * 1.3f) * 2.5f;
        Hold(f, Vec2.Dist(f.Pos, anchor) > 3f ? anchor : f.Pos);
        f.Intent = "rozgląda się";
    }

    // ---------------- działanie co tick ----------------
    static void Act(Battle b, Fighter f, InstinctState st)
    {
        if (f.TargetId < 0 || !b.TryGet(f.TargetId, out var t) || !t.Active) { if (f.TargetId >= 0) f.TargetId = -1; return; }
        if (b.Time < f.ReactionReadyAt || b.Time < st.SwapUntil) return;
        float d = Vec2.Dist(f.Pos, t.Pos);
        if (f.Current.IsRanged)
        {
            if (f.Ammo <= 0 || d > f.Current.MaxRange) return;
            if (!f.Seen.Contains(t.Id)) return;
            // strzał wymaga chwili spokoju
            if (f.Vel.LengthSq > 2.5f) return;
            // decyzja o ryzykownym strzale zapada raz na sekundę (nie losujemy co tick)
            if (b.Time >= st.FireRollUntil) { st.FireRollUntil = b.Time + 1f; st.RiskyShotOk = b.Rng.Chance(MathX.Clamp01(0.95f - f.Tactical * 0.2f)); }
            if (d > 2.5f && FriendOnLine(b, f, t) != null && (f.Tactical >= 2 || !st.RiskyShotOk)) return;   // przez plecy swoich nie strzela
            if (d > 4 && DangerClose(b, f, t) && !st.RiskyShotOk && !(f.Tactical >= 3 && d < 8f && b.HitChance(f, t, d) > 0.45f)) return;   // pewny strzał z bliska wolno
            if (f.Cooldown <= 0) { b.Shoot(f, t); st.LastShotAt = b.Time; }
        }
        else if (b.CanStrike(f, t)) { f.Facing = (t.Pos - f.Pos).Angle; b.Strike(f, t); }
        if (f.Vel.LengthSq < 0.5f) f.Facing = MathX.MoveTowardsAngle(f.Facing, (t.Pos - f.Pos).Angle, 0.4f);
    }

    static void Carry(Battle b, Fighter f, InstinctState st)
    {
        if (!b.TryGet(f.CarryingId, out var w) || w.Status != Status.Down) { f.CarryingId = -1; return; }
        var safe = Terrain.Walkable(b.Map, f.Squad(b).Attacking ? f.Pos - Vec2.FromAngle(f.Facing) * 10 : b.Map.LampPos + (f.Pos - b.Map.LampPos).Normalized() * 2.2f);
        f.MoveTarget = safe; f.HasMoveTarget = true; f.Intent = "niesie rannego"; f.TargetId = -1;
        if (Vec2.Dist(f.Pos, safe) < 1.2f)
        {
            w.CarriedById = -1; f.CarryingId = -1; f.Rescues++;
            b.Log.Event(b.Time, LogKind.Rescue, f.Id, w.Id, $"{f.Name} wyniósł {w.Name} pod latarnię");
            if (!w.Stabilized) { st.TreatTarget = w.Id; st.TreatUntil = b.Time + (f.Role == Role.Sanitariusz ? 2.5f : 5f); }
            else f.Order = new Order();
        }
    }

    static void FinishTreatment(Battle b, Fighter f, InstinctState st)
    {
        if (b.TryGet(st.TreatTarget, out var w) && w.Status == Status.Down && Vec2.Dist(w.Pos, f.Pos) < 1.8f)
        {
            if (f.Medkits > 0) f.Medkits--;
            w.Stabilized = true; w.Bleed *= 0.6f;
            f.Rescues++;
            b.Log.Event(b.Time, LogKind.Rescue, f.Id, w.Id, $"{f.Name} opatrzył {w.Name}");
        }
        st.TreatTarget = -1; st.TreatUntil = -1; f.Crouch = false;
        if (f.Order.Type == OrderType.Rescue && f.Order.TargetId == w?.Id)
        {
            // po opatrzeniu: wynieść, jeśli wróg blisko; inaczej rozkaz wykonany
            if (b.All.Any(e => e.Active && e.Team != f.Team && Vec2.Dist(e.Pos, w!.Pos) < 7f)) { f.CarryingId = w!.Id; w.CarriedById = f.Id; }
            else f.Order = new Order();
        }
    }

    // ---------------- pomocnicze ----------------
    static void Hold(Fighter f, Vec2 p) { f.MoveTarget = p; f.HasMoveTarget = true; }

    static Vec2 StepTowards(Fighter f, Vec2 target, float keep)
    {
        var d = target - f.Pos; float l = d.Length;
        return l <= keep ? f.Pos : f.Pos + d / l * (l - keep);
    }

    static void Look(Battle b, Fighter f, float facing, Fighter? near)
    {
        if (f.Vel.LengthSq > 0.5f) return;
        float want = near != null ? (near.Pos - f.Pos).Angle : facing;
        // wyszkoleni odwracają się do hałasu z boku/tyłu
        if (near == null && f.Heard.Count > 0)
        {
            var h = f.Heard.Values.OrderByDescending(x => x.t).First();
            if (b.Time - h.t < 1.2f && (f.Tactical >= 4 || (f.Tactical == 3 && b.Rng.Chance(0.5f)))) want = (h.pos - f.Pos).Angle;
        }
        f.Facing = MathX.MoveTowardsAngle(f.Facing, want, 0.9f);
    }

    public static Fighter? NearestSeen(Battle b, Fighter f, out float dist)
    {
        Fighter? best = null; dist = float.MaxValue;
        foreach (var id in f.Seen)
        {
            var e = b.Get(id); if (!e.Active) continue;
            float d = Vec2.Dist(e.Pos, f.Pos); if (d < dist) { dist = d; best = e; }
        }
        // bardzo bliskie, słyszane (za plecami) też się liczą
        foreach (var (id, h) in f.Heard)
        {
            if (id < 0 || !b.TryGet(id, out var e) || !e.Active) continue;
            float d = Vec2.Dist(e.Pos, f.Pos); if (d < 1.6f && d < dist) { dist = d; best = e; }
        }
        return best;
    }

    static Fighter? BestMeleeTarget(Battle b, Fighter f, Vec2 around, float r)
    {
        Fighter? best = null; float bs = float.MaxValue;
        foreach (var id in f.Seen)
        {
            var e = b.Get(id); if (!e.Active) continue;
            float d = Vec2.Dist(e.Pos, around); if (d > r) continue;
            float s = Vec2.Dist(e.Pos, f.Pos) - (e.Kind == Kind.Ogar ? 1.0f : 0) - (e.Role == Role.Herszt ? 0.8f : 0);
            if (e.Kind == Kind.Nosiciel) s += 10;   // Nosiciela nie bije się wręcz
            if (s < bs) { bs = s; best = e; }
        }
        return best;
    }

    static Fighter? BestRangedTarget(Battle b, Fighter f, Kind? prio, float? sector)
    {
        Fighter? best = null; float bs = float.MinValue;
        var allies = f.Squad(b).Members.Where(m => m.Status != Status.Dead && m != f).ToList();
        foreach (var id in f.Seen)
        {
            var e = b.Get(id); if (!e.Active) continue;
            float d = Vec2.Dist(e.Pos, f.Pos); if (d > f.Current.MaxRange) continue;
            if (sector != null && MathF.Abs(MathX.AngleDiff(sector.Value, (e.Pos - f.Pos).Angle)) > 1.3f && d > 6f) continue;
            float s = b.HitChance(f, e, d) * 3f;
            if (prio != null && e.Kind == prio) s += 4f;
            float toAlly = allies.Count == 0 ? 99 : allies.Min(a => Vec2.Dist(a.Pos, e.Pos));
            if (toAlly < 1.6f) s -= 0.6f * f.Tactical;                         // wróg w zwarciu ze swoim: strzał ryzykowny
            else s += MathX.Clamp01((9f - toAlly) / 7f) * 1.5f;                 // kto zaraz dopadnie swoich
            if (e.Kind == Kind.Krzykacz && f.Tactical >= 3) s += 1.5f;
            if (e.Kind == Kind.Nosiciel)
            {
                float toAny = b.All.Where(h => h.Team == f.Team && h.Status != Status.Dead).Min(h => Vec2.Dist(h.Pos, e.Pos));
                // strzelać, póki jest daleko od ludzi; przy swoich wybuch i tak ich rani
                s += toAny > 5f ? 3f : toAny > 2.5f && f.Tactical <= 2 ? 1.5f : -2f * f.Tactical;
            }
            if (e.Kind == Kind.Rat) s -= 0.8f;   // szkoda bełtu na szczura, chyba że nic innego
            if (e.Role == Role.Lucznik || e.Role == Role.Herszt) s += 0.8f;
            if (e.Id == f.TargetId) s += 0.6f;   // nie skakać między celami
            if (f.Tactical >= 2 && FriendOnLine(b, f, e) != null) s -= 1.2f * f.Tactical;   // wybiera cel z czystą linią
            if (s > bs) { bs = s; best = e; }
        }
        return best;
    }

    static bool DangerClose(Battle b, Fighter f, Fighter t)
    {
        foreach (var m in b.All)
            if (m != f && m.Team == f.Team && m.Status != Status.Dead && m.CarriedById < 0 && Vec2.Dist(m.Pos, t.Pos) < 1.6f && Vec2.Dist(m.Pos, f.Pos) < Vec2.Dist(t.Pos, f.Pos) + 4f) return true;
        return false;
    }

    public static Fighter? FriendOnLine(Battle b, Fighter f, Fighter t)
    {
        float dt = Vec2.Dist(f.Pos, t.Pos);
        foreach (var m in b.All)
        {
            if (m == f || m.Team != f.Team || m.Status == Status.Dead || m.CarriedById >= 0) continue;
            if (Vec2.Dist(m.Pos, f.Pos) > dt + 0.5f) continue;
            if (Battle.DistToSegment(m.Pos, f.Pos, t.Pos) < 0.9f + Vec2.Dist(m.Pos, f.Pos) * 0.04f && Vec2.Dist(m.Pos, f.Pos) > 0.6f) return m;
        }
        return null;
    }

    static void SwapWeapon(Battle b, Fighter f, InstinctState st, Fighter? near, float nd)
    {
        if (f.Side == null) return;
        if (f.Current.IsRanged && near != null && nd < 2.6f && f.Tactical >= 2 || (f.Current.IsRanged && f.Ammo <= 0))
        { f.Current = f.Side; st.SwapUntil = b.Time + 0.6f; }
        else if (!f.Current.IsRanged && f.Main.IsRanged && f.Ammo > 0 && (near == null || nd > 5f))
        { f.Current = f.Main; st.SwapUntil = b.Time + 0.6f; }
    }

    static void ReportIfOverwhelmed(Battle b, Fighter f, InstinctState st, Squad sq)
    {
        if (b.Time - st.LastReport < 6f || sq.Commander == null) return;
        int enemies = 0;
        foreach (var id in f.Seen) { var e = b.Get(id); if (e.Active && Vec2.Dist(e.Pos, f.Pos) < 5f) enemies += e.Kind == Kind.Rat ? 1 : 3; }
        int friends = sq.Active.Count(m => m != f && Vec2.Dist(m.Pos, f.Pos) < 3f);
        if (enemies >= 6 + friends * 3 && f.Tactical >= 2)
        {
            st.LastReport = b.Time;
            sq.Commander.Report(b, f, Phrases.Pick(Phrases.CantHold, b.Rng), true);
        }
        // ktoś zaszedł od tyłu
        foreach (var id in f.Seen)
        {
            var e = b.Get(id);
            if (Vec2.Dist(e.Pos, f.Pos) < 6 && f.Order.Type is OrderType.Hold or OrderType.FireSupport && MathF.Abs(MathX.AngleDiff(f.Order.Facing, (e.Pos - f.Pos).Angle)) > 2.2f && f.Tactical >= 2)
            { st.LastReport = b.Time; sq.Commander.Report(b, f, Phrases.Pick(Phrases.Flank, b.Rng), true); break; }
        }
    }
}
