using System;
using System.Collections.Generic;
using Latarnia.Sim.Core;
using Latarnia.Sim.Data;

namespace Latarnia.Sim.Units;

public enum Team { Squad = 0, Enemy = 1 }
public enum Status { Ok, Down, Dead, Fled }
public enum Zone { Head, Torso, Arms, Legs }
public enum Severity { Light, Heavy, Fatal }

public sealed record Wound(Zone Zone, Severity Severity, float Time, string Source);

public sealed class Stats
{
    public int Accuracy = 5, Melee = 5, Strength = 5, Agility = 5, Perception = 5, Nerve = 5, Endurance = 5;
}

/// <summary>Rozkaz od dowódcy. Instynkt wojownika waży go według dyscypliny i morale.</summary>
public enum OrderType { None, Hold, Engage, FireSupport, Eliminate, Rescue, Guard, Avoid, Advance, Retreat, Regroup }

public sealed class Order
{
    public OrderType Type;
    public Vec2 Pos;            // pozycja do zajęcia
    public float Facing;        // kierunek obserwacji
    public int TargetId = -1;   // wróg albo ranny
    public int TaskId = -1;     // zadanie dowódcy (do raportu i różnicowania rozkazów)
    public float IssuedAt;
    public float Tolerance = 1f;   // jak dokładnie trzymać pozycję (Formacje)
    public Kind? PriorityKind;     // preferowany typ celu (np. Krzykacz)
    public Vec2 Anchor; public float Leash;   // krąg: wolno wyjść do wroga, który atakuje kogokolwiek z kręgu
}

public sealed class Fighter
{
    public int Id;
    public string Name = "";
    public Team Team;
    public Kind Kind;
    public Role Role;
    public CreatureDef? Creature;   // nie-ludzie
    public int SquadIndex;
    public Stats S = new();
    public int Tactical = 2;        // wyszkolenie taktyczne 1..5
    public string ClassId = "rekrut";
    public readonly HashSet<Ability> Abilities = new();
    public readonly Dictionary<Ability, float> ReadyAt = new();
    public float StillSince;        // od kiedy stoi (Ogień celowany)
    public float NextStrikeBonus, NextDamageMul = 1f;
    public string Model = "";       // model 3D do widoku
    public bool Has(Ability a) => Abilities.Contains(a);
    public bool Ready(Ability a, float now) => Abilities.Contains(a) && (!ReadyAt.TryGetValue(a, out var t) || now >= t);
    public void Use(Ability a, float now, float cd) => ReadyAt[a] = now + cd;
    public bool Brave, Coward, Merciful;

    // stan fizyczny
    public Vec2 Pos, Vel;
    public float Facing;            // radiany
    public bool Crouch;
    public Status Status = Status.Ok;
    public float Blood = 100f, Bleed;   // krew i utrata na sekundę
    public bool Stabilized;
    public readonly List<Wound> Wounds = new();
    public int HitsTaken;          // dla nie-ludzi
    public float Morale = 70f, Suppression, Fatigue, Odmiennosc;
    public float StunUntil, DownAt = -1, DiedAt = -1, RevealedUntil = -1;
    public bool LegsDisabled, ArmDisabled;

    // uzbrojenie
    public WeaponDef Main = Weapons.Knife;
    public WeaponDef? Side;
    public WeaponDef Current = Weapons.Knife;
    public int Ammo;
    public int ArmorHead, ArmorTorso;
    public float Cooldown, Aim;     // czas do następnego ataku, celowanie
    public int Medkits;

    // AI
    public Order Order = new();
    public float OrderReceivedAt = -999;
    public Order? PendingOrder; public float PendingAt;
    public readonly HashSet<int> Seen = new();        // kogo widzi teraz
    public readonly Dictionary<int, (Vec2 pos, float t)> Heard = new();
    public int TargetId = -1;
    public Vec2 MoveTarget; public bool HasMoveTarget;
    public List<Vec2>? Path; public int PathIndex; public Vec2 PathGoal; public float PathAt = -99;
    public float ReactionReadyAt;   // od kiedy reaguje na nowy kontakt (wyszkolenie)
    public float LastSeenEnemyAt = -99;
    public string Bubble = ""; public float BubbleUntil;
    public int CarryingId = -1, CarriedById = -1;
    public object? Brain;          // mózg wroga (nie-ludzie)
    public float NextThinkAt;      // instynkt nie musi myśleć co tick
    public string Intent = "";     // do podglądu: co teraz robi

    // statystyki do raportu
    public int Kills, Rescues, FriendlyHits, Shots; public float DamageDealt, TimeInCombat, TimeHolding;

    public bool Alive => Status != Status.Dead;
    public bool Active => Status == Status.Ok;
    public bool IsHuman => Kind == Kind.Human;
    public float Size => Creature?.Size ?? 0.9f;
    public float Radius => Kind == Kind.Rat ? 0.2f : Kind == Kind.Ogar ? 0.35f : 0.3f;

    public float Speed(bool run)
    {
        if (Creature != null) return run ? Creature.RunSpeed : Creature.WalkSpeed;
        float s = (run ? 3.4f : 1.6f) * (0.85f + S.Agility * 0.03f);
        if (LegsDisabled) s *= 0.3f;
        if (Crouch) s *= 0.55f;
        if (CarryingId >= 0) s *= Has(Ability.Wyciagniecie) ? 0.9f : 0.55f;
        s *= 1f - MathX.Clamp01(Fatigue / 100f) * 0.35f;
        return s;
    }
    public void Say(string text, float now, float dur = 2.2f) { Bubble = text; BubbleUntil = now + dur; }
    public override string ToString() => $"{Name}#{Id}";
}
