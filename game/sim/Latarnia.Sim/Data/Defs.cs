using System.Collections.Generic;

namespace Latarnia.Sim.Data;

public enum WeaponKind { Melee, Ranged }

/// <summary>Broń. Zasięgi w metrach (decyzja z 8.11: stare kratki ×5). Wartości robocze do balansu.</summary>
public sealed record WeaponDef(string Id, string NamePl, WeaponKind Kind, int Penetration, float Reach,
    float Interval, float Damage, float EffRange = 0, float MaxRange = 0, float ProjSpeed = 0, int Ammo = 0,
    float Block = 0, bool StopsCharge = false, bool Stuns = false, float Noise = 6f, bool TwoHanded = false)
{
    public bool IsRanged => Kind == WeaponKind.Ranged;
}

public static class Weapons
{
    public static readonly WeaponDef Knife   = new("knife", "nóż", WeaponKind.Melee, 1, 1.0f, 0.7f, 0.55f, Noise: 3);
    public static readonly WeaponDef Pipe    = new("pipe", "rura", WeaponKind.Melee, 2, 1.3f, 1.1f, 0.75f, Stuns: true, Noise: 8);
    public static readonly WeaponDef Machete = new("machete", "maczeta", WeaponKind.Melee, 2, 1.2f, 0.85f, 0.8f, Noise: 5);
    public static readonly WeaponDef Spear   = new("spear", "dzida", WeaponKind.Melee, 3, 2.1f, 1.2f, 0.95f, StopsCharge: true, TwoHanded: true, Noise: 4);
    public static readonly WeaponDef ShieldSpear = new("shield_spear", "włócznia z tarczą", WeaponKind.Melee, 2, 2.0f, 1.1f, 0.8f, Block: 2, StopsCharge: true, Noise: 4);
    public static readonly WeaponDef Hammer  = new("hammer", "młot", WeaponKind.Melee, 6, 1.4f, 1.8f, 1.35f, Stuns: true, TwoHanded: true, Noise: 10);
    public static readonly WeaponDef PipeBow = new("pipe_bow", "łuk z rurek", WeaponKind.Ranged, 2, 0, 1.9f, 0.85f, 40, 70, 38, 20, TwoHanded: true, Noise: 3);
    public static readonly WeaponDef Crossbow = new("crossbow", "kusza", WeaponKind.Ranged, 4, 0, 4.0f, 1.15f, 50, 75, 55, 15, TwoHanded: true, Noise: 4);
    // broń mutantów i zwierząt
    public static readonly WeaponDef RatBite  = new("rat_bite", "ugryzienie szczura", WeaponKind.Melee, 0, 0.6f, 0.9f, 0.25f, Noise: 2);
    public static readonly WeaponDef OgarBite = new("ogar_bite", "ugryzienie Ogara", WeaponKind.Melee, 2, 1.0f, 1.0f, 1.05f, Noise: 6);
    public static readonly WeaponDef Claw     = new("claw", "pazury", WeaponKind.Melee, 1, 1.0f, 1.3f, 0.6f, Noise: 4);

    /// <summary>Broń ludzi do wyboru w edytorze (epoka Złomu).</summary>
    public static readonly WeaponDef[] Human = { ShieldSpear, Spear, Hammer, Machete, Pipe, Knife, PipeBow, Crossbow };
    static readonly Dictionary<string, WeaponDef> _by = new();
    static Weapons() { foreach (var w in new[] { Knife, Pipe, Machete, Spear, ShieldSpear, Hammer, PipeBow, Crossbow, RatBite, OgarBite, Claw }) _by[w.Id] = w; }
    public static WeaponDef? ById(string? id) => id != null && _by.TryGetValue(id, out var w) ? w : null;
}

public enum Kind { Human, Rat, Ogar, Krzykacz, Nosiciel }
public enum Role { None, Tarczownik, Rebacz, Strzelec, Sanitariusz, Zwiadowca, Obdartus, Nozownik, Rozbijacz, Lucznik, Herszt }

/// <summary>Parametry istot (WROGOWIE.md, przeliczone na metry i sekundy). Hp = trafienia do wyłączenia dla nie-ludzi.</summary>
public sealed record CreatureDef(Kind Kind, string NamePl, int Hp, float WalkSpeed, float RunSpeed, float Size,
    float ThreatValue, float SightDark, float SightLit, int MeleeSkill, WeaponDef Attack);

public static class Creatures
{
    public static readonly CreatureDef Rat      = new(Kind.Rat, "szczur", 1, 3.0f, 4.2f, 0.35f, 0.35f, 6, 25, 3, Weapons.RatBite);
    public static readonly CreatureDef Ogar     = new(Kind.Ogar, "Ogar", 2, 3.0f, 8.5f, 0.8f, 3.0f, 14, 40, 6, Weapons.OgarBite);
    public static readonly CreatureDef Krzykacz = new(Kind.Krzykacz, "Krzykacz", 3, 1.4f, 2.2f, 1.0f, 2.5f, 12, 40, 3, Weapons.Claw);
    public static readonly CreatureDef Nosiciel = new(Kind.Nosiciel, "Nosiciel", 1, 1.1f, 1.3f, 1.1f, 2.0f, 10, 35, 2, Weapons.Claw);
}

public static class Names
{
    public static readonly string[] Squad = { "Mróz", "Bartek", "Ola", "Zenek", "Hania", "Kuba", "Wiktor", "Gośka" };
    public static readonly string[] Bandits = { "Kiler", "Szczerba", "Gruby", "Łysy", "Kopyto", "Bury", "Szpak", "Młody", "Kulas" };
}
