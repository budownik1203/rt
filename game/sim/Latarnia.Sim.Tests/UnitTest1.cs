using Latarnia.Sim.AI;
using Latarnia.Sim.Combat;
using Latarnia.Sim.Report;
using Latarnia.Sim.Scenarios;
using Latarnia.Sim.Units;

namespace Latarnia.Sim.Tests;

public class BattleTests
{
    static (Battle b, BattleReport r) Run(string scen, CommanderProfile p, ulong seed)
    {
        var b = Scenarios.Scenarios.Create(scen, p, seed);
        b.Run();
        return (b, ReportBuilder.Build(b, b.Squads[0]));
    }

    [Theory]
    [InlineData("ogary")]
    [InlineData("szabrownicy")]
    [InlineData("mieszany")]
    public void Deterministyczna(string scen)
    {
        var (a, _) = Run(scen, CommanderProfile.Of(3), 42);
        var (b, _) = Run(scen, CommanderProfile.Of(3), 42);
        Assert.Equal(a.Time, b.Time);
        Assert.Equal(a.Log.Events.Count, b.Log.Events.Count);
        Assert.Equal(a.All.Select(f => f.Pos).ToArray(), b.All.Select(f => f.Pos).ToArray());
    }

    [Fact]
    public void WalkaSieKonczy()
    {
        foreach (var s in Scenarios.Scenarios.All)
        {
            var (b, _) = Run(s.Id, CommanderProfile.Of(2), 7);
            Assert.True(b.Over);
        }
    }

    [Theory]
    [InlineData("szczury")]
    [InlineData("mieszany")]
    public void Poziom3LepszyOd1(string scen)
    {
        const int n = 60;
        float Loss(int lvl) => Enumerable.Range(0, n).Select(i => { var (_, r) = Run(scen, CommanderProfile.Of(lvl), (ulong)(900 + i)); return r.Dead * 3f + r.Downed + (r.Won ? 0 : 3); }).Average();
        Assert.True(Loss(3) < Loss(1) * 0.8f, "dowódca poziomu 3 powinien tracić wyraźnie mniej ludzi niż poziom 1");
    }

    [Fact]
    public void RaportWskazujeSlabeKadry()
    {
        var weak = new CommanderProfile { Level = 3, Kadry = 1, Teren = 4, Formacje = 4 };
        int hits = 0, n = 40;
        for (int i = 0; i < n; i++)
        {
            var (_, r) = Run("mieszany", weak, (ulong)(300 + i));
            var top = r.Blame.Where(kv => kv.Key is not (Spec.Wyszkolenie or Spec.Poziom)).OrderByDescending(kv => kv.Value).FirstOrDefault();
            if (top.Key == Spec.Kadry && top.Value > 0.5f) hits++;
        }
        Assert.True(hits > n * 0.7, $"Kadry wskazane w {hits}/{n}");
    }

    [Fact]
    public void DowodcaUzywaFlaryIRozkazow()
    {
        var (b, _) = Run("krzykacz", CommanderProfile.Of(3), 11);
        Assert.Contains(b.Log.Events, e => e.Kind == LogKind.Order && e.Text.Contains("flara"));
        Assert.Contains(b.Log.Events, e => e.Kind == LogKind.Order && e.Text.Contains("Krzykacz"));
    }
}
