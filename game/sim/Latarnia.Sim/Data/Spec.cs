using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Latarnia.Sim.AI;

namespace Latarnia.Sim.Data;

/// <summary>Opis jednego wojownika w edytorze symulatora bitew. Wszystko, co gracz może ustawić.</summary>
public sealed class FighterSpec
{
    public string Name { get; set; } = "Wojownik";
    public string ClassId { get; set; } = "rekrut";
    public Role? RoleOverride { get; set; }            // Szabrownicy: Herszt, Łucznik itd.
    public int Accuracy { get; set; } = 5;
    public int Melee { get; set; } = 5;
    public int Strength { get; set; } = 5;
    public int Agility { get; set; } = 5;
    public int Perception { get; set; } = 5;
    public int Nerve { get; set; } = 5;
    public int Endurance { get; set; } = 5;
    public int Tactical { get; set; } = 2;             // wyszkolenie taktyczne 1–5
    public string Weapon { get; set; } = "machete";
    public string? Side { get; set; }
    public int ArmorHead { get; set; }                 // 0 brak, 1 garnek, 2 hełm
    public int ArmorTorso { get; set; }                // 0 brak, 1 skóra, 2 kamizelka, 3 blacha
    public int Medkits { get; set; }
    public bool Brave { get; set; }
    public bool Coward { get; set; }
    public bool Merciful { get; set; }
    public float Morale { get; set; } = 70;
    public string? Model { get; set; }                 // model 3D; puste = dobór według broni i klasy

    public FighterSpec Clone() => (FighterSpec)MemberwiseClone();
}

public sealed class CommanderSpec
{
    public int Leader { get; set; }                    // indeks wojownika w składzie
    public int Level { get; set; } = 2;                // 1–5
    public int Kadry { get; set; } = 2;
    public int Teren { get; set; } = 2;
    public int Formacje { get; set; } = 2;
    public Character Character { get; set; } = Character.Neutral;
    public int Flares { get; set; } = 2;
}

public sealed class SquadSpec
{
    public string Name { get; set; } = "Skład";
    public List<FighterSpec> Fighters { get; set; } = new();
    public CommanderSpec Commander { get; set; } = new();
}

/// <summary>Fala mutantów: gatunek, liczba, kierunek (stopnie: 0 wschód, 90 południe), odległość i opóźnienie.</summary>
public sealed class WaveSpec
{
    public Kind Kind { get; set; } = Kind.Rat;
    public int Count { get; set; } = 10;
    public float AngleDeg { get; set; } = 90;
    public float Distance { get; set; } = 30;
    public float Delay { get; set; }
}

public enum OpponentType { Mutanty, Sklad }
public enum ClashMode { ObronaOsady, Starcie }

public sealed class BattleSpec
{
    public string Title { get; set; } = "Bitwa";
    public SquadSpec Squad { get; set; } = new();
    public OpponentType Opponent { get; set; } = OpponentType.Mutanty;
    public List<WaveSpec> Waves { get; set; } = new();
    public SquadSpec? Enemy { get; set; }
    public ClashMode Mode { get; set; } = ClashMode.ObronaOsady;   // skład kontra skład: B atakuje osadę A albo starcie w polu
    public ulong Seed { get; set; } = 1001;
    public float MaxTime { get; set; } = 360;
    public bool Night { get; set; } = true;

    static readonly JsonSerializerOptions Opt = new() { WriteIndented = true, Converters = { new JsonStringEnumConverter() } };
    public string ToJson() => JsonSerializer.Serialize(this, Opt);
    public static BattleSpec FromJson(string json) => JsonSerializer.Deserialize<BattleSpec>(json, Opt)!;
    public BattleSpec Clone() => FromJson(ToJson());
}

/// <summary>Gotowe składy i przeciwnicy (punkt wyjścia w edytorze).</summary>
public static class Presets
{
    public static SquadSpec Mroz() => new()
    {
        Name = "Skład Mroza",
        Fighters =
        {
            new() { Name = "Mróz", ClassId = "strzelec", Weapon = "pipe_bow", Side = "knife", Tactical = 3, Accuracy = 6, Nerve = 7, Morale = 75 },
            new() { Name = "Bartek", ClassId = "tarczownik", Weapon = "shield_spear", ArmorTorso = 3, ArmorHead = 1, Melee = 6, Strength = 7, Brave = true },
            new() { Name = "Ola", ClassId = "rebacz", Weapon = "hammer", Melee = 7, Strength = 7, ArmorTorso = 2 },
            new() { Name = "Zenek", ClassId = "strzelec", Weapon = "crossbow", Side = "knife", Accuracy = 7, Nerve = 4, Coward = true },
            new() { Name = "Hania", ClassId = "sanitariusz", Weapon = "pipe", Medkits = 3, Merciful = true, Melee = 4 },
            new() { Name = "Kuba", ClassId = "zwiadowca", Weapon = "pipe_bow", Side = "knife", Perception = 8, Agility = 7, Accuracy = 6, Tactical = 3 },
        },
        Commander = new() { Leader = 0, Level = 3, Kadry = 3, Teren = 3, Formacje = 3 }
    };

    public static SquadSpec Szabrownicy(int level = 2) => new()
    {
        Name = "Szabrownicy",
        Fighters =
        {
            new() { Name = "Kiler", RoleOverride = Role.Herszt, Weapon = "machete", Melee = 7, Nerve = 7, ArmorTorso = 1, Tactical = 3, Morale = 65, Model = "sz_herszt" },
            new() { Name = "Szczerba", RoleOverride = Role.Rozbijacz, Weapon = "pipe", Strength = 7, Nerve = 4, Morale = 65, Model = "sz_rozbijacz" },
            new() { Name = "Gruby", RoleOverride = Role.Nozownik, Weapon = "knife", Nerve = 4, Morale = 65, Model = "sz_nozownik" },
            new() { Name = "Łysy", RoleOverride = Role.Lucznik, Weapon = "pipe_bow", Side = "knife", Nerve = 4, Morale = 65, Model = "sz_lucznik" },
            new() { Name = "Kopyto", RoleOverride = Role.Lucznik, Weapon = "pipe_bow", Side = "knife", Nerve = 4, Morale = 65, Model = "sz_lucznik" },
            new() { Name = "Bury", RoleOverride = Role.Obdartus, Weapon = "spear", Nerve = 4, Morale = 65, Model = "sz_obdartus" },
            new() { Name = "Szpak", RoleOverride = Role.Obdartus, Weapon = "machete", Nerve = 4, Morale = 65, Model = "sz_psiarz" },
        },
        Commander = new() { Leader = 0, Level = level, Kadry = level, Teren = level, Formacje = level, Flares = 0 }
    };

    public static List<WaveSpec> Waves(string id) => id switch
    {
        "ogary" => new() { new() { Kind = Kind.Ogar, Count = 5, AngleDeg = -46 }, new() { Kind = Kind.Ogar, Count = 3, AngleDeg = 144, Delay = 12 } },
        "szczury" => new() { new() { Kind = Kind.Rat, Count = 26, AngleDeg = 90, Distance = 31 }, new() { Kind = Kind.Rat, Count = 18, AngleDeg = -9, Delay = 8 } },
        "krzykacz" => new() { new() { Kind = Kind.Krzykacz, Count = 1, AngleDeg = -101, Distance = 32 }, new() { Kind = Kind.Ogar, Count = 4, AngleDeg = -76, Distance = 31 }, new() { Kind = Kind.Rat, Count = 16, AngleDeg = 90, Distance = 31, Delay = 12 } },
        "nosiciel" => new() { new() { Kind = Kind.Nosiciel, Count = 3, AngleDeg = 90, Distance = 27 }, new() { Kind = Kind.Rat, Count = 18, AngleDeg = 101, Distance = 31 }, new() { Kind = Kind.Nosiciel, Count = 1, AngleDeg = 3, Distance = 27, Delay = 10 }, new() { Kind = Kind.Ogar, Count = 2, AngleDeg = 6, Distance = 30, Delay = 10 } },
        _ => new() { new() { Kind = Kind.Rat, Count = 20, AngleDeg = 90, Distance = 31 }, new() { Kind = Kind.Krzykacz, Count = 1, AngleDeg = -90, Distance = 32, Delay = 8 }, new() { Kind = Kind.Ogar, Count = 3, AngleDeg = -113, Delay = 8 },
                   new() { Kind = Kind.Nosiciel, Count = 1, AngleDeg = 0, Distance = 28, Delay = 20 }, new() { Kind = Kind.Ogar, Count = 3, AngleDeg = 153, Delay = 20 } },
    };

    public static BattleSpec Default() => new() { Title = "Noc Mgły", Squad = Mroz(), Opponent = OpponentType.Mutanty, Waves = Waves("mieszany") };
}
