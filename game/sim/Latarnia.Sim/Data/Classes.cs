using System.Collections.Generic;
using System.Linq;

namespace Latarnia.Sim.Data;

/// <summary>Zdolności klas (KLASY.md, epoka Złomu). Ciągłe działają zawsze, pozostałe mają odnowienie.</summary>
public enum Ability
{
    None,
    Zaslona,        // Tarczownik: sąsiedzi w szyku dostają mniej trafień z przodu (ciągła)
    TwardySzyk,     // Mur: nie przewraca się, mur tarcz silniejszy (ciągła)
    ScianaGrotow,   // Dzidnik: cios wyprzedzający w wroga wchodzącego w 2 m, 6 s, odnowienie 25 s
    Wypad,          // Rębacz: skok 3 m i cios +30% obrażeń, odnowienie 10 s
    Rozbicie,       // Łamacz: cios zdejmuje pancerz / dodatkowe trafienie potworowi, odnowienie 15 s
    SeriaCiec,      // Nożownik: 3 szybkie ciosy z krwawieniem, odnowienie 12 s
    OgienCelowany,  // Strzelec: +15% celności po 2 s bez ruchu (ciągła)
    CelnyStrzal,    // Strzelec wyborowy: +40% celności i +1 przebicia, odnowienie 20 s
    Wypatrywanie,   // Zwiadowca: +25% zasięgu wykrywania (ciągła)
    Trop,           // Tropiciel: zna kierunek najbliższego wroga, słyszy dalej (ciągła)
    CicheZabicie,   // Cichy łowca: od tyłu wyłącza nieświadomego wroga, odnowienie 30 s
    Stabilizacja,   // Sanitariusz: zatrzymuje krwotok w kilka sekund (ciągła)
    LeczenieWPolu,  // Felczer: ciężka rana staje się lekką (rany przestają wyłączać), ciągła przy opatrywaniu
    Wyciagniecie,   // Sanitariusz bojowy: wyciąga rannego 2× szybciej, mniej trafień przy tym (ciągła)
}

public sealed record ClassDef(string Id, string NamePl, int Tier, string? From, Role Role, Ability Ability, float Cooldown, string Description,
    int Acc = 0, int Melee = 0, int Str = 0, int Agi = 0, int Per = 0, int Nerve = 0, int End = 0);

public static class Classes
{
    public static readonly ClassDef[] All =
    {
        new("rekrut", "Rekrut", 1, null, Role.None, Ability.None, 0, "Po Placu ćwiczeń: zna podstawy, bez zdolności."),
        new("tarczownik", "Tarczownik", 2, "rekrut", Role.Tarczownik, Ability.Zaslona, 0, "Trzyma szyk i osłania sąsiadów.", Melee: 1, End: 1),
        new("mur", "Mur", 3, "tarczownik", Role.Tarczownik, Ability.TwardySzyk, 0, "Ciężki tarczownik do muru tarcz i bram.", Melee: 1, Str: 1, End: 2),
        new("dzidnik", "Dzidnik", 3, "tarczownik", Role.Tarczownik, Ability.ScianaGrotow, 25, "Zatrzymuje szarże Ogarów.", Melee: 2, End: 1),
        new("rebacz", "Rębacz", 2, "rekrut", Role.Rebacz, Ability.Wypad, 10, "Agresywny wojownik do kontrataku i dobijania.", Melee: 1, Str: 1),
        new("lamacz", "Łamacz", 3, "rebacz", Role.Rebacz, Ability.Rozbicie, 15, "Rozbija pancerz i chitynę.", Melee: 1, Str: 2),
        new("nozownik", "Nożownik", 3, "rebacz", Role.Rebacz, Ability.SeriaCiec, 12, "Walka w ciasnych miejscach, seria cięć.", Melee: 2, Agi: 1),
        new("strzelec", "Strzelec", 2, "rekrut", Role.Strzelec, Ability.OgienCelowany, 0, "Ogień na średnim dystansie.", Acc: 1),
        new("wyborowy", "Strzelec wyborowy", 3, "strzelec", Role.Strzelec, Ability.CelnyStrzal, 20, "Likwiduje Krzykaczy i dowódców.", Acc: 2, Per: 1),
        new("zwiadowca", "Zwiadowca", 2, "rekrut", Role.Zwiadowca, Ability.Wypatrywanie, 0, "Wykrywa wroga wcześniej.", Per: 1, Agi: 1),
        new("tropiciel", "Tropiciel", 3, "zwiadowca", Role.Zwiadowca, Ability.Trop, 0, "Zna kierunek najbliższego wroga.", Per: 2),
        new("cichy", "Cichy łowca", 3, "zwiadowca", Role.Zwiadowca, Ability.CicheZabicie, 30, "Ciche eliminacje od tyłu.", Agi: 1, Melee: 1),
        new("sanitariusz", "Sanitariusz", 2, "rekrut", Role.Sanitariusz, Ability.Stabilizacja, 0, "Utrzymuje rannych przy życiu."),
        new("felczer", "Felczer", 3, "sanitariusz", Role.Sanitariusz, Ability.LeczenieWPolu, 0, "Leczenie w polu: rany przestają wyłączać.", Nerve: 1),
        new("sanbojowy", "Sanitariusz bojowy", 3, "sanitariusz", Role.Sanitariusz, Ability.Wyciagniecie, 0, "Wyciąga rannych spod ognia.", End: 1, Nerve: 1),
    };

    static readonly Dictionary<string, ClassDef> _by = All.ToDictionary(c => c.Id);
    public static ClassDef Get(string id) => _by.TryGetValue(id, out var c) ? c : _by["rekrut"];

    /// <summary>Wszystkie zdolności klasy i klas, z których wyrosła (sumują się).</summary>
    public static IEnumerable<ClassDef> Chain(string id)
    {
        var c = Get(id);
        while (true) { yield return c; if (c.From == null) break; c = Get(c.From); }
    }
}
