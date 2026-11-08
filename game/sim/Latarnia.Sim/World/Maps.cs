using System;
using Latarnia.Sim.Core;

namespace Latarnia.Sim.World;

public static class Maps
{
    /// <summary>Mała osada Złomu: latarnia w środku, palisada w ośmiokąt z bramą od południa (+Y), kilka budynków i skrzynie.</summary>
    public static Grid SmallSettlement()
    {
        var g = new Grid(140, 140) { Name = "Mała osada" };   // 70 x 70 m
        var c = new Vec2(35, 33);
        g.LampPos = c; g.LampRadius = 14f;
        // palisada: ośmiokąt o promieniu 17 m, brama 3 m szerokości od południa, wyłom od wschodu (stara dziura)
        const float R = 17f; int N = 8;
        for (int i = 0; i < N; i++)
        {
            float a0 = (i + 0.5f) * MathF.Tau / N, a1 = (i + 1.5f) * MathF.Tau / N;
            var p0 = c + Vec2.FromAngle(a0) * R; var p1 = c + Vec2.FromAngle(a1) * R;
            g.Line(p0, p1, Cell.Blocked, 0.6f);
        }
        float apo = R * MathF.Cos(MathF.PI / N);   // ściany ośmiokąta leżą bliżej niż wierzchołki
        var gate = c + new Vec2(0, apo);
        g.FillRect(gate.X - 1.6f, gate.Y - 1.2f, gate.X + 1.6f, gate.Y + 1.2f, Cell.Open);
        var breach = c + new Vec2(apo, -2.5f);
        g.FillRect(breach.X - 1.2f, breach.Y - 1.0f, breach.X + 1.2f, breach.Y + 1.0f, Cell.Open);
        g.Chokepoints.Add(gate); g.Chokepoints.Add(breach);
        // budynki (pełne bryły)
        g.FillRect(c.X - 11, c.Y - 9, c.X - 6, c.Y - 4, Cell.Blocked);     // szałas NW
        g.FillRect(c.X + 4, c.Y - 11, c.X + 10, c.Y - 7, Cell.Blocked);    // kuchnia NE
        g.FillRect(c.X - 12, c.Y + 3, c.X - 8, c.Y + 8, Cell.Blocked);     // szałas SW
        g.FillRect(c.X + 7, c.Y + 4, c.X + 11, c.Y + 7, Cell.Blocked);     // smolarnia SE
        g.FillRect(c.X - 0.6f, c.Y - 0.6f, c.X + 0.6f, c.Y + 0.6f, Cell.Blocked);   // latarnia
        // niskie osłony: skrzynie, beczki, wóz przy bramie
        g.FillRect(c.X - 3.5f, c.Y + 9.5f, c.X - 1.5f, c.Y + 10.5f, Cell.LowCover);
        g.FillRect(c.X + 2f, c.Y + 11f, c.X + 3.5f, c.Y + 12f, Cell.LowCover);
        g.FillRect(c.X + 11f, c.Y - 4f, c.X + 12f, c.Y - 1f, Cell.LowCover);
        g.FillRect(c.X - 4f, c.Y - 5f, c.X - 2.5f, c.Y - 3.5f, Cell.LowCover);
        g.FillRect(c.X + 3f, c.Y + 2f, c.X + 4.5f, c.Y + 3f, Cell.LowCover);
        // na zewnątrz: zarośla (wolniej) i kilka wraków jako osłona
        for (int i = 0; i < 9; i++)
        {
            float a = i * 0.71f + 0.3f; var p = c + Vec2.FromAngle(a) * (24f + (i % 3) * 3f);
            g.FillRect(p.X - 2f, p.Y - 1.5f, p.X + 2f, p.Y + 1.5f, Cell.Rough);
        }
        g.FillRect(c.X - 9, c.Y + 22, c.X - 6, c.Y + 23.5f, Cell.Blocked);   // wrak auta przed bramą
        g.FillRect(c.X + 21, c.Y - 8, c.X + 23, c.Y - 5, Cell.Blocked);      // wrak na wschodzie
        g.BakeLight(); g.BakeFlow();
        return g;
    }
}
