using System;
using System.Collections.Generic;
using System.Linq;
using Latarnia.Sim.Core;
using Latarnia.Sim.Data;
using Latarnia.Sim.Units;

namespace Latarnia.Sim.AI;

/// <summary>Dymki: krótkie, imienne, z punktem odniesienia. Po polsku, jak na polu walki.</summary>
public static class Phrases
{
    public static readonly string[] LeaderDown = { "Mróz padł!", "Dowódca leży!", "Kto dowodzi?!", "Szef dostał!" };
    public static readonly string[] TakeCommand = { "Słuchać mnie!", "Ja dowodzę!", "Do mnie, trzymać się!" };
    public static readonly string[] CantHold = { "Nie utrzymam!", "Za dużo ich!", "Potrzebuję wsparcia!" };
    public static readonly string[] Flank = { "Z boku!", "Za nami!", "Zachodzą od tyłu!" };
    public static readonly string[] Panic = { "Uciekam!", "Nie dam rady!", "Ja stąd spadam!" };
    public static readonly string[] Medic = { "Trzymaj się!", "Mam cię!", "Uciskam, nie ruszaj się!" };
    public static readonly string[] NoAmmo = { "Koniec bełtów!", "Nie mam strzał!", "Pusto!" };
    public static readonly string[] Bomb = { "Nosiciel! Odsuń się!", "Bomba idzie!", "Nie podchodź do niego!" };

    public static string Pick(string[] a, Rng r) => a[r.Int(a.Length)];

    public static string KindPl(Kind k) => k switch { Kind.Rat => "szczury", Kind.Ogar => "Ogary", Kind.Krzykacz => "Krzykacz", Kind.Nosiciel => "Nosiciel", _ => "ludzie" };

    static string Who(List<Fighter> f) => f.Count switch { 1 => f[0].Name, 2 => $"{f[0].Name}, {f[1].Name}", _ => string.Join(", ", f.Take(f.Count - 1).Select(x => x.Name)) + " i " + f[^1].Name };

    static string Where(CTask t) => t.Threat?.Label switch
    {
        null => "", "brama" => "bramę", "wyłom" => "wyłom",
        var l when l.Contains("za palisadą") => l.Replace(" (za palisadą)", "") switch { var s => "na " + Loc(s) },
        var l => "na " + Loc(l)
    };
    static string At(CTask t) => t.Threat?.Label switch
    {
        null => "", "brama" => "przy bramie", "wyłom" => "przy wyłomie",
        var l => "na " + Loc(l.Replace(" (za palisadą)", ""))
    };
    static string Loc(string side) => side switch
    {
        "północ" => "północy", "południe" => "południu", "wschód" => "wschodzie", "zachód" => "zachodzie",
        "północny wschód" => "północnym wschodzie", "północny zachód" => "północnym zachodzie",
        "południowy wschód" => "południowym wschodzie", "południowy zachód" => "południowym zachodzie", _ => side
    };

    public static string Order(CTask t, List<Fighter> who, Rng r)
    {
        string w = Who(who);
        string where = Where(t);
        string tl = t.Threat?.Label ?? "";
        bool place = tl == "brama" || tl == "wyłom";
        if (t.Key == "krag") return who.Count >= 3 ? $"{w}, w krąg przy latarni, twarzą na zewnątrz!" : $"{w}, do kręgu!";
        return t.Type switch
        {
            TaskType.Hold => place ? $"{w}, trzymać {where}!" : $"{w}, zatrzymać ich {At(t)}!",
            TaskType.Assault => t.Threat == null ? $"{w}, naprzód!" : $"{w}, bierzemy ich {At(t)}!",
            TaskType.FireSupport => t.PriorityKind == Kind.Krzykacz ? $"{w}, strzelać w Krzykacza!" : t.PriorityKind == Kind.Nosiciel ? $"{w}, zdjąć Nosiciela z daleka!" : place ? $"{w}, ogień na {where}!" : $"{w}, ogień {At(t)}!",
            TaskType.Eliminate => $"{w}, zdjąć Krzykacza {At(t)}!",
            TaskType.KillAtRange => $"{w}, Nosiciel {At(t)}, strzelać z daleka!",
            TaskType.Rescue => t.TargetId >= 0 ? $"{w}, do rannego!" : $"{w}, ratuj!",
            TaskType.Watch => $"{w}, pilnuj tyłów!",
            TaskType.Reserve => $"{w}, do mnie, w odwodzie!",
            _ => $"{w}!"
        };
    }
}
