namespace Latarnia.Sim.AI;

public enum Character { Neutral, Ostrozny, Brawurowy, Uparty, Opiekunczy }

/// <summary>Profil dowódcy z konceptu (tabela poziomów) i trzy specjalizacje 1..5.</summary>
public sealed class CommanderProfile
{
    public int Level = 2;
    public int Kadry = 2, Teren = 2, Formacje = 2;   // Zasoby ludzkie, Orientacja w terenie, Taktyka i formacje
    public Character Character = Character.Neutral;

    public float Cycle => Level switch { 1 => 999f, 2 => 3f, 3 => 2f, 4 => 1.5f, _ => 1f };
    public int Subgroups => Level switch { 1 => 0, 2 => 1, 3 => 2, 4 => 3, _ => 4 };
    public float Predict => Level switch { <= 2 => 0f, 3 => 1f, 4 => 2.5f, _ => 4f };
    public float Memory => Level switch { 1 => 0f, 2 => 6f, 3 => 15f, 4 => 40f, _ => 120f };
    public float MinPlan => (Level switch { 2 => 5f, 3 => 3.5f, 4 => 3f, _ => 2.5f }) * (Character == Character.Uparty ? 1.5f : 1f);
    public float ReplaceThreshold => (Level switch { 2 => 0.3f, 3 => 0.2f, _ => 0.15f }) * (Character == Character.Uparty ? 1.6f : 1f);
    public float ClusterRadius => Level <= 2 ? 9f : 5.5f;
    public int MaxThreats => Level <= 2 ? 2 : Level == 3 ? 4 : 6;
    public float OrderDelay => Level switch { 2 => 0.8f, 3 => 0.55f, 4 => 0.45f, _ => 0.35f };
    public int PositionSamples => Teren switch { 1 => 4, 2 => 8, 3 => 14, 4 => 24, _ => 40 };
    public bool UsesChokepoints => Teren >= 3;
    public bool UsesSectors => Formacje >= 3;
    public float SlotTolerance => Formacje switch { 1 => 1.6f, 2 => 1.0f, 3 => 0.5f, _ => 0.35f };
    public bool KnowsBestiary => Level >= 4;
    public float RiskWeight => Character == Character.Brawurowy ? 0.6f : Character == Character.Ostrozny ? 1.4f : 1f;

    public static CommanderProfile Of(int level) => level switch
    {
        1 => new CommanderProfile { Level = 1, Kadry = 1, Teren = 1, Formacje = 1 },
        2 => new CommanderProfile { Level = 2, Kadry = 2, Teren = 2, Formacje = 2 },
        3 => new CommanderProfile { Level = 3, Kadry = 3, Teren = 3, Formacje = 3 },
        4 => new CommanderProfile { Level = 4, Kadry = 4, Teren = 4, Formacje = 4 },
        _ => new CommanderProfile { Level = 5, Kadry = 5, Teren = 5, Formacje = 5 },
    };
    public override string ToString() => $"poziom {Level} (Kadry {Kadry}, Teren {Teren}, Formacje {Formacje}{(Character != Character.Neutral ? ", " + Character : "")})";
}
