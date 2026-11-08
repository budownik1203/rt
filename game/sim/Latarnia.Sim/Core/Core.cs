using System;
using System.Runtime.CompilerServices;

namespace Latarnia.Sim.Core;

/// <summary>Wektor 2D w metrach (płaszczyzna ziemi: X w prawo, Y w głąb).</summary>
public readonly record struct Vec2(float X, float Y)
{
    public static readonly Vec2 Zero = new(0, 0);
    public static Vec2 operator +(Vec2 a, Vec2 b) => new(a.X + b.X, a.Y + b.Y);
    public static Vec2 operator -(Vec2 a, Vec2 b) => new(a.X - b.X, a.Y - b.Y);
    public static Vec2 operator *(Vec2 a, float s) => new(a.X * s, a.Y * s);
    public static Vec2 operator /(Vec2 a, float s) => new(a.X / s, a.Y / s);
    public static Vec2 operator -(Vec2 a) => new(-a.X, -a.Y);
    public float Length => MathF.Sqrt(X * X + Y * Y);
    public float LengthSq => X * X + Y * Y;
    public Vec2 Normalized() { var l = Length; return l > 1e-5f ? this / l : Zero; }
    public static float Dot(Vec2 a, Vec2 b) => a.X * b.X + a.Y * b.Y;
    public static float Cross(Vec2 a, Vec2 b) => a.X * b.Y - a.Y * b.X;
    public static float Dist(Vec2 a, Vec2 b) => (a - b).Length;
    public static float DistSq(Vec2 a, Vec2 b) => (a - b).LengthSq;
    public Vec2 Perp => new(-Y, X);
    public float Angle => MathF.Atan2(Y, X);
    public static Vec2 FromAngle(float a) => new(MathF.Cos(a), MathF.Sin(a));
    public Vec2 Rotated(float a) { float c = MathF.Cos(a), s = MathF.Sin(a); return new(X * c - Y * s, X * s + Y * c); }
    public static Vec2 Lerp(Vec2 a, Vec2 b, float t) => a + (b - a) * t;
    public override string ToString() => $"({X:0.0},{Y:0.0})";
}

/// <summary>Deterministyczny generator (xorshift128+). Ta sama walka z tym samym ziarnem daje ten sam wynik.</summary>
public sealed class Rng
{
    ulong _a, _b;
    public Rng(ulong seed)
    {
        _a = seed * 0x9E3779B97F4A7C15UL + 1; _b = (seed ^ 0xD1B54A32D192ED03UL) * 0xBF58476D1CE4E5B9UL + 7;
        for (int i = 0; i < 8; i++) NextU();
    }
    public ulong NextU() { var x = _a; var y = _b; _a = y; x ^= x << 23; _b = x ^ y ^ (x >> 17) ^ (y >> 26); return _b + y; }
    public float Next() => (NextU() >> 40) / (float)(1UL << 24);
    public float Range(float a, float b) => a + (b - a) * Next();
    public int Int(int maxExclusive) => (int)(Next() * maxExclusive) % Math.Max(1, maxExclusive);
    public bool Chance(float p) => Next() < p;
    public float Gauss() { float u = Math.Max(1e-6f, Next()), v = Next(); return MathF.Sqrt(-2f * MathF.Log(u)) * MathF.Cos(2f * MathF.PI * v); }
}

public static class MathX
{
    public static float Clamp01(float v) => v < 0 ? 0 : v > 1 ? 1 : v;
    public static float Clamp(float v, float a, float b) => v < a ? a : v > b ? b : v;
    public static float AngleDiff(float a, float b) { var d = (b - a) % (2 * MathF.PI); if (d > MathF.PI) d -= 2 * MathF.PI; if (d < -MathF.PI) d += 2 * MathF.PI; return d; }
    public static float MoveTowardsAngle(float a, float target, float maxStep) { var d = AngleDiff(a, target); return a + MathX.Clamp(d, -maxStep, maxStep); }
}
