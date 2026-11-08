using System;
using System.Collections.Generic;
using System.Linq;
using Latarnia.Sim.AI;
using Latarnia.Sim.Combat;
using Latarnia.Sim.Core;
using Latarnia.Sim.Data;
using Latarnia.Sim.Units;
using Latarnia.Sim.World;

namespace Latarnia.Sim.Scenarios;

public sealed record ScenarioDef(string Id, string Name, string Description);

/// <summary>Scenariusze prototypu: obrona małej osady w nocy przed jednym typem zagrożenia albo mieszanką.</summary>
public static class Scenarios
{
    public static readonly ScenarioDef[] All =
    {
        new("ogary", "Ogary", "Stado 5 Ogarów od północnego wschodu, po 12 s trzy kolejne od zachodu (kleszcze)."),
        new("szczury", "Szczurza Fala", "26 szczurów od bramy, po 8 s 18 kolejnych przez wyłom."),
        new("krzykacz", "Krzykacz z Ogarami", "Krzykacz i 4 Ogary od północy, po 12 s 16 szczurów od bramy."),
        new("nosiciel", "Nosiciele", "Trzech Nosicieli z 18 szczurami od bramy, po 10 s czwarty z 2 Ogarami od wschodu."),
        new("szabrownicy", "Szabrownicy", "Banda 7 ludzi z Hersztem atakuje od południa. Ten sam AI dowódcy (poziom 2)."),
        new("mieszany", "Noc Mgły", "Wszystko naraz w trzech falach z różnych stron."),
    };

    public static Battle Create(string id, CommanderProfile profile, ulong seed, bool record = false)
    {
        var map = Maps.SmallSettlement();
        var b = new Battle(map, seed);
        b.Log.RecordFrames = record;
        var squad = AddSquad(b, profile);
        switch (id)
        {
            case "ogary":
                Pack(b, Kind.Ogar, 5, -0.8f, 30f);
                Later(b, 12f, () => Pack(b, Kind.Ogar, 3, MathF.PI * 0.8f, 30f));
                break;
            case "szczury":
                Pack(b, Kind.Rat, 26, MathF.PI / 2, 31f);
                Later(b, 8f, () => Pack(b, Kind.Rat, 18, -0.15f, 30f));
                break;
            case "krzykacz":
            {
                int s = Pack(b, Kind.Krzykacz, 1, -MathF.PI / 2 - 0.2f, 32f);
                AddTo(b, s, Kind.Ogar, 4, -MathF.PI / 2 + 0.25f, 31f);
                Later(b, 12f, () => Pack(b, Kind.Rat, 16, MathF.PI / 2, 31f));
                break;
            }
            case "nosiciel":
            {
                int s = Pack(b, Kind.Nosiciel, 3, MathF.PI / 2, 27f);
                AddTo(b, s, Kind.Rat, 18, MathF.PI / 2 + 0.2f, 31f);
                Later(b, 10f, () => { int q = Pack(b, Kind.Nosiciel, 1, 0.05f, 27f); AddTo(b, q, Kind.Ogar, 2, 0.1f, 30f); });
                break;
            }
            case "szabrownicy": Bandits(b, MathF.PI / 2, 30f); break;
            case "szabrownicy1": Bandits(b, MathF.PI / 2, 30f, 1); break;
            case "szabrownicy3": Bandits(b, MathF.PI / 2, 30f, 3); break;
            case "mieszany":
                Pack(b, Kind.Rat, 20, MathF.PI / 2, 31f);
                Later(b, 8f, () => { int s = Pack(b, Kind.Krzykacz, 1, -MathF.PI / 2, 32f); AddTo(b, s, Kind.Ogar, 3, -MathF.PI / 2 - 0.4f, 31f); });
                Later(b, 20f, () => { Pack(b, Kind.Nosiciel, 1, 0f, 28f); Pack(b, Kind.Ogar, 3, MathF.PI * 0.85f, 30f); });
                break;
            case var x when x.StartsWith("szab:"):
            {
                var v = x[5..].Split(',').Select(int.Parse).ToArray();
                var bs = Bandits(b, MathF.PI / 2, 30f);
                bs.Commander = new Commander(bs.Commander!.Leader, new CommanderProfile { Level = v[0], Kadry = v[1], Teren = v[2], Formacje = v[3] });
                bs.Commander.Deputies.AddRange(bs.Members.Where(m => m != bs.Commander.Leader));
                break;
            }
            default: throw new ArgumentException($"nieznany scenariusz {id}");
        }
        return b;
    }

    static void Later(Battle b, float t, Action a) => b.Schedule.Add((t, _ => a()));

    // ---------------- skład gracza ----------------
    public static Squad AddSquad(Battle b, CommanderProfile profile)
    {
        var s = new Squad { Index = b.Squads.Count, Team = Team.Squad, Name = "Skład Mroza", Anchor = b.Map.LampPos };
        b.Squads.Add(s);
        var c = b.Map.LampPos;
        Fighter H(string name, Role role, WeaponDef main, WeaponDef? side, Action<Fighter>? tune = null)
        {
            var f = new Fighter { Name = name, Team = Team.Squad, Kind = Kind.Human, Role = role, SquadIndex = s.Index, Main = main, Side = side, Current = main, Ammo = main.Ammo, Tactical = 2 };
            int i = s.Members.Count;
            f.Pos = Terrain.Walkable(b.Map, c + Vec2.FromAngle(i * MathF.Tau / 6 + 0.3f) * 3f);
            f.Facing = (f.Pos - c).Angle;
            tune?.Invoke(f);
            b.Add(f); s.Members.Add(f);
            return f;
        }
        var mroz = H("Mróz", Role.Strzelec, Weapons.PipeBow, Weapons.Knife, f => { f.Tactical = 3; f.S.Accuracy = 6; f.S.Nerve = 7; f.Morale = 75; });
        H("Bartek", Role.Tarczownik, Weapons.ShieldSpear, null, f => { f.ArmorTorso = 2; f.ArmorHead = 1; f.S.Melee = 6; f.S.Strength = 7; f.Brave = true; });
        H("Ola", Role.Rebacz, Weapons.Hammer, null, f => { f.S.Melee = 7; f.S.Strength = 7; f.ArmorTorso = 1; });
        H("Zenek", Role.Strzelec, Weapons.Crossbow, Weapons.Knife, f => { f.S.Accuracy = 7; f.S.Nerve = 4; f.Coward = true; });
        H("Hania", Role.Sanitariusz, Weapons.Pipe, null, f => { f.Medkits = 3; f.Merciful = true; f.S.Melee = 4; });
        H("Kuba", Role.Zwiadowca, Weapons.PipeBow, Weapons.Knife, f => { f.S.Perception = 8; f.S.Agility = 7; f.S.Accuracy = 6; f.Tactical = 3; });
        s.Commander = new Commander(mroz, profile);
        s.Commander.Deputies.AddRange(s.Members.Where(m => m != mroz).OrderByDescending(m => m.Tactical).ThenByDescending(m => m.S.Nerve));
        return s;
    }

    // ---------------- mutanty ----------------
    static int Pack(Battle b, Kind kind, int n, float angle, float dist)
    {
        var s = new Squad { Index = b.Squads.Count, Team = Team.Enemy, Name = kind.ToString(), Anchor = b.Map.LampPos, Attacking = true };
        b.Squads.Add(s);
        AddTo(b, s.Index, kind, n, angle, dist);
        return s.Index;
    }

    static void AddTo(Battle b, int squad, Kind kind, int n, float angle, float dist)
    {
        var def = kind switch { Kind.Rat => Creatures.Rat, Kind.Ogar => Creatures.Ogar, Kind.Krzykacz => Creatures.Krzykacz, _ => Creatures.Nosiciel };
        var s = b.Squads[squad];
        var c = b.Map.LampPos;
        for (int i = 0; i < n; i++)
        {
            float a = angle + b.Rng.Range(-0.18f, 0.18f) + (i - n / 2f) * (kind == Kind.Rat ? 0.02f : 0.07f);
            var p = c + Vec2.FromAngle(a) * (dist + b.Rng.Range(-1.5f, 1.5f));
            p = new Vec2(MathX.Clamp(p.X, 1, b.Map.WidthM - 1), MathX.Clamp(p.Y, 1, b.Map.HeightM - 1));
            var f = new Fighter { Name = def.NamePl, Team = Team.Enemy, Kind = kind, Creature = def, SquadIndex = squad, Main = def.Attack, Current = def.Attack, Pos = Terrain.Walkable(b.Map, p), Morale = 100 };
            f.Facing = (c - f.Pos).Angle;
            b.Add(f); s.Members.Add(f);
        }
    }

    // ---------------- Szabrownicy: ludzie z tym samym AI ----------------
    public static Squad Bandits(Battle b, float angle, float dist, int level = 2)
    {
        var s = new Squad { Index = b.Squads.Count, Team = Team.Enemy, Name = "Szabrownicy", Anchor = b.Map.LampPos, Attacking = true };
        b.Squads.Add(s);
        var c = b.Map.LampPos;
        var roles = new (Role r, WeaponDef w, WeaponDef? side)[]
        {
            (Role.Herszt, Weapons.Machete, null), (Role.Rozbijacz, Weapons.Pipe, null), (Role.Nozownik, Weapons.Knife, null),
            (Role.Lucznik, Weapons.PipeBow, Weapons.Knife), (Role.Lucznik, Weapons.PipeBow, Weapons.Knife), (Role.Obdartus, Weapons.Spear, null), (Role.Obdartus, Weapons.Machete, null)
        };
        Fighter? herszt = null;
        for (int i = 0; i < roles.Length; i++)
        {
            var (r, w, side) = roles[i];
            var p = c + Vec2.FromAngle(angle + (i - 3) * 0.09f) * (dist + b.Rng.Range(-1f, 1f));
            var f = new Fighter { Name = Names.Bandits[i], Team = Team.Enemy, Kind = Kind.Human, Role = r, SquadIndex = s.Index, Main = w, Side = side, Current = w, Ammo = w.Ammo, Pos = Terrain.Walkable(b.Map, p), Tactical = r == Role.Herszt ? 3 : 2, Morale = 65 };
            f.Facing = (c - f.Pos).Angle; f.S.Nerve = 4; f.S.Accuracy = 5;
            if (r == Role.Herszt) { f.S.Melee = 7; f.S.Nerve = 7; f.ArmorTorso = 1; herszt = f; }
            if (r == Role.Rozbijacz) f.S.Strength = 7;
            b.Add(f); s.Members.Add(f);
        }
        s.Commander = new Commander(herszt!, CommanderProfile.Of(level));
        s.Commander.Deputies.AddRange(s.Members.Where(m => m != herszt));
        return s;
    }
}
