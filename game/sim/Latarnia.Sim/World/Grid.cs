using System;
using System.Collections.Generic;
using Latarnia.Sim.Core;

namespace Latarnia.Sim.World;

[Flags]
public enum Cell : byte { Open = 0, Blocked = 1, LowCover = 2, Door = 4, Rough = 8 }

/// <summary>Siatka 0,5 m: przeszkody, niskie osłony, światło i gęstość Mgły. Kluczowy teren liczony raz przy budowie mapy.</summary>
public sealed class Grid
{
    public const float CellSize = 0.5f;
    public readonly int W, H;
    readonly Cell[] _cells;
    readonly float[] _light;   // 0..1, 1 = pełne światło latarni
    public Vec2 LampPos;
    public float LampRadius = 14f;   // moc 1: 7 kratek po 2 m
    public float Ambient = 0.06f;     // noc
    public readonly List<Vec2> Chokepoints = new();
    public string Name = "";

    public Grid(int w, int h) { W = w; H = h; _cells = new Cell[w * h]; _light = new float[w * h]; }
    public float WidthM => W * CellSize;
    public float HeightM => H * CellSize;
    public bool InBounds(int x, int y) => x >= 0 && y >= 0 && x < W && y < H;
    public (int x, int y) ToCell(Vec2 p) => ((int)MathF.Floor(p.X / CellSize), (int)MathF.Floor(p.Y / CellSize));
    public Vec2 CellCenter(int x, int y) => new((x + 0.5f) * CellSize, (y + 0.5f) * CellSize);
    public Cell Get(int x, int y) => InBounds(x, y) ? _cells[y * W + x] : Cell.Blocked;
    public Cell Get(Vec2 p) { var (x, y) = ToCell(p); return Get(x, y); }
    public void Set(int x, int y, Cell c) { if (InBounds(x, y)) _cells[y * W + x] = c; }
    public bool Walkable(int x, int y) => (Get(x, y) & Cell.Blocked) == 0;
    public bool Walkable(Vec2 p) { var (x, y) = ToCell(p); return Walkable(x, y); }
    public bool BlocksSight(int x, int y) => (Get(x, y) & Cell.Blocked) != 0;

    public void FillRect(float x0, float y0, float x1, float y1, Cell c)
    {
        var (ax, ay) = ToCell(new Vec2(MathF.Min(x0, x1), MathF.Min(y0, y1)));
        var (bx, by) = ToCell(new Vec2(MathF.Max(x0, x1), MathF.Max(y0, y1)));
        for (int y = ay; y <= by; y++) for (int x = ax; x <= bx; x++) Set(x, y, c);
    }
    public void Line(Vec2 a, Vec2 b, Cell c, float thick = 0.5f)
    {
        int n = (int)MathF.Ceiling(Vec2.Dist(a, b) / (CellSize * 0.5f)) + 1;
        for (int i = 0; i <= n; i++)
        {
            var p = Vec2.Lerp(a, b, i / (float)n);
            FillRect(p.X - thick / 2, p.Y - thick / 2, p.X + thick / 2, p.Y + thick / 2, c);
        }
    }

    public void BakeLight()
    {
        for (int y = 0; y < H; y++) for (int x = 0; x < W; x++)
        {
            var d = Vec2.Dist(CellCenter(x, y), LampPos);
            // pełne światło do 70% promienia, potem spadek do zera na krawędzi kręgu
            float l = d < LampRadius * 0.7f ? 1f : MathX.Clamp01(1f - (d - LampRadius * 0.7f) / (LampRadius * 0.45f));
            if (l > 0.01f && d > 1.2f && !LineOfSight.Clear(this, LampPos + (CellCenter(x, y) - LampPos).Normalized() * 1.0f, CellCenter(x, y))) l *= 0.25f;   // cień za budynkiem
            _light[y * W + x] = MathF.Max(Ambient, l);
        }
    }
    float[]? _flow;
    /// <summary>Pole przepływu do latarni (Dijkstra raz na mapę): mutanty z daleka idą nim przez bramę albo wyłom.</summary>
    public void BakeFlow()
    {
        _flow = new float[W * H]; Array.Fill(_flow, float.MaxValue);
        var (lx, ly) = ToCell(LampPos);
        var q = new PriorityQueue<int, float>();
        for (int dy = -3; dy <= 3; dy++) for (int dx = -3; dx <= 3; dx++) if (Walkable(lx + dx, ly + dy)) { int i = (ly + dy) * W + lx + dx; _flow[i] = 0; q.Enqueue(i, 0); }
        while (q.TryDequeue(out int c, out float d))
        {
            if (d > _flow[c]) continue;
            int cx = c % W, cy = c / W;
            for (int dy = -1; dy <= 1; dy++) for (int dx = -1; dx <= 1; dx++)
            {
                if (dx == 0 && dy == 0) continue;
                int nx = cx + dx, ny = cy + dy;
                if (!Walkable(nx, ny)) continue;
                if (dx != 0 && dy != 0 && (!Walkable(cx + dx, cy) || !Walkable(cx, cy + dy))) continue;
                float nd = d + (dx != 0 && dy != 0 ? 1.4142f : 1f) + ((Get(nx, ny) & Cell.LowCover) != 0 ? 2.5f : 0) + ((Get(nx, ny) & Cell.Rough) != 0 ? 0.8f : 0);
                int n = ny * W + nx;
                if (nd < _flow[n]) { _flow[n] = nd; q.Enqueue(n, nd); }
            }
        }
    }
    public Vec2 FlowStep(Vec2 p)
    {
        if (_flow == null) BakeFlow();
        var (x, y) = ToCell(p); Vec2 best = LampPos; float bd = InBounds(x, y) ? _flow![y * W + x] : float.MaxValue;
        // patrzy 2 kratki dalej, żeby ruch był płynny
        for (int dy = -2; dy <= 2; dy++) for (int dx = -2; dx <= 2; dx++)
        {
            int nx = x + dx, ny = y + dy; if (!InBounds(nx, ny)) continue;
            float v = _flow![ny * W + nx];
            if (v < bd && LineOfSight.Clear(this, p, CellCenter(nx, ny)) && Walkable(nx, ny) && (dx == 0 || dy == 0 || (Walkable(x + Math.Sign(dx), y) && Walkable(x, y + Math.Sign(dy))))) { bd = v; best = CellCenter(nx, ny); }
        }
        return best;
    }

    public float Light(Vec2 p) { var (x, y) = ToCell(p); return InBounds(x, y) ? _light[y * W + x] : Ambient; }

    /// <summary>Osłona przed zagrożeniem z kierunku dir: 0 brak, 0,5 niska, 1 pełna (sprawdza komórki między pozycją a zagrożeniem).</summary>
    public float CoverFrom(Vec2 pos, Vec2 threatPos)
    {
        var dir = (threatPos - pos).Normalized();
        float best = 0;
        for (float s = 0.5f; s <= 1.25f; s += 0.25f)
        {
            var c = Get(pos + dir * s);
            if ((c & Cell.Blocked) != 0) best = 1f;
            else if ((c & Cell.LowCover) != 0) best = MathF.Max(best, 0.5f);
        }
        return best;
    }
}

public static class LineOfSight
{
    /// <summary>Bresenham po siatce: czy nic pełnego nie zasłania. Niska osłona nie blokuje wzroku.</summary>
    public static bool Clear(Grid g, Vec2 a, Vec2 b)
    {
        var (x0, y0) = g.ToCell(a); var (x1, y1) = g.ToCell(b);
        int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1, dy = -Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1, err = dx + dy;
        int startX = x0, startY = y0;
        while (true)
        {
            if (!(x0 == startX && y0 == startY) && !(x0 == x1 && y0 == y1) && g.BlocksSight(x0, y0)) return false;
            if (x0 == x1 && y0 == y1) return true;
            int e2 = 2 * err;
            if (e2 >= dy) { err += dy; x0 += sx; }
            if (e2 <= dx) { err += dx; y0 += sy; }
        }
    }
}

/// <summary>A* po siatce z 8 sąsiadami i dodatkowym kosztem z funkcji (np. strefa ognia, odsłonięcie).</summary>
public sealed class Pathfinder
{
    readonly Grid _g;
    readonly float[] _gCost; readonly int[] _parent; readonly int[] _stamp; int _cur;
    public Pathfinder(Grid g) { _g = g; _gCost = new float[g.W * g.H]; _parent = new int[g.W * g.H]; _stamp = new int[g.W * g.H]; }

    public List<Vec2>? Find(Vec2 from, Vec2 to, Func<int, int, float>? extraCost = null, int maxNodes = 6000)
    {
        var (sx, sy) = _g.ToCell(from); var (tx, ty) = _g.ToCell(to);
        if (!_g.Walkable(tx, ty)) { var alt = NearestWalkable(tx, ty); if (alt == null) return null; (tx, ty) = alt.Value; }
        if (!_g.Walkable(sx, sy)) { var alt = NearestWalkable(sx, sy); if (alt == null) return null; (sx, sy) = alt.Value; }
        _cur++;
        var open = new PriorityQueue<int, float>();
        int s = sy * _g.W + sx, t = ty * _g.W + tx;
        _stamp[s] = _cur; _gCost[s] = 0; _parent[s] = -1; open.Enqueue(s, 0);
        int expanded = 0;
        while (open.Count > 0 && expanded++ < maxNodes)
        {
            int c = open.Dequeue();
            if (c == t) break;
            int cx = c % _g.W, cy = c / _g.W;
            for (int dy = -1; dy <= 1; dy++) for (int dx = -1; dx <= 1; dx++)
            {
                if (dx == 0 && dy == 0) continue;
                int nx = cx + dx, ny = cy + dy;
                if (!_g.Walkable(nx, ny)) continue;
                if (dx != 0 && dy != 0 && (!_g.Walkable(cx + dx, cy) || !_g.Walkable(cx, cy + dy))) continue;   // bez ścinania rogów
                float step = (dx != 0 && dy != 0) ? 1.4142f : 1f;
                var cell = _g.Get(nx, ny);
                if ((cell & Cell.LowCover) != 0) step += 2.5f;   // przełażenie przez skrzynie
                if ((cell & Cell.Rough) != 0) step += 0.8f;
                if (extraCost != null) step += extraCost(nx, ny);
                int n = ny * _g.W + nx; float ng = _gCost[c] + step;
                if (_stamp[n] != _cur || ng < _gCost[n])
                {
                    _stamp[n] = _cur; _gCost[n] = ng; _parent[n] = c;
                    float h = MathF.Sqrt((nx - tx) * (nx - tx) + (ny - ty) * (ny - ty));
                    open.Enqueue(n, ng + h);
                }
            }
        }
        if (_stamp[t] != _cur) return null;
        var path = new List<Vec2>();
        for (int c = t; c != -1; c = _parent[c]) path.Add(_g.CellCenter(c % _g.W, c / _g.W));
        path.Reverse();
        return Smooth(path);
    }

    List<Vec2> Smooth(List<Vec2> p)
    {
        if (p.Count <= 2) return p;
        var r = new List<Vec2> { p[0] }; int i = 0;
        while (i < p.Count - 1)
        {
            int j = p.Count - 1;
            while (j > i + 1 && !WalkLine(p[i], p[j])) j--;
            r.Add(p[j]); i = j;
        }
        return r;
    }
    bool WalkLine(Vec2 a, Vec2 b)
    {
        int n = (int)(Vec2.Dist(a, b) / 0.25f) + 1;
        for (int k = 0; k <= n; k++)
        {
            var c = _g.Get(Vec2.Lerp(a, b, k / (float)n));
            if ((c & (Cell.Blocked | Cell.LowCover)) != 0) return false;
        }
        return true;
    }
    (int, int)? NearestWalkable(int x, int y)
    {
        for (int r = 1; r < 8; r++) for (int dy = -r; dy <= r; dy++) for (int dx = -r; dx <= r; dx++)
            if (_g.Walkable(x + dx, y + dy)) return (x + dx, y + dy);
        return null;
    }
}
