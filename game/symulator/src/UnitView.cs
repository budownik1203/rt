using System;
using Godot;
using Latarnia.Sim.Combat;
using Latarnia.Sim.Data;
using Latarnia.Sim.Units;
using V2 = Latarnia.Sim.Core.Vec2;

namespace Symulator;

/// <summary>Widok jednego wojownika lub mutanta: model, animacje dobierane ze stanu symulacji i zdarzeń, interpolacja ruchu.</summary>
public partial class UnitView : Node3D
{
    public Fighter F;
    public Node3D Model;
    AnimationPlayer _ap;
    string _cur = "";
    double _busyUntil;          // do kiedy trwa akcja jednorazowa (cios, strzał, trafienie)
    bool _busyLocks;            // czy akcja blokuje ruch nóg (np. upadek)
    double _now;
    Vector3 _prev, _next;
    float _yawPrev, _yawNext, _yaw;
    MeshInstance3D _ring;
    public bool Selected;
    Color _teamCol;
    static readonly string[] Loops = { "idle", "walk", "run", "down", "treat", "drag", "dragged", "cower" };
    public V2 Origin;           // sim -> świat: odjęcie położenia latarni

    public void Init(Fighter f, V2 origin, Color teamCol)
    {
        F = f; Origin = origin; _teamCol = teamCol;
        Model = Art.Character(string.IsNullOrEmpty(f.Model) ? "maczeta_sklad" : f.Model, this);
        _ap = Art.FindAnim(Model);
        if (_ap != null)
            foreach (var n in Loops)
                if (_ap.HasAnimation(n)) _ap.GetAnimation(n).LoopMode = Animation.LoopModeEnum.Linear;
        float r = f.Kind == Kind.Rat ? 0.22f : f.Kind == Kind.Ogar ? 0.5f : f.Kind == Kind.Nosiciel ? 0.55f : 0.42f;
        _ring = new MeshInstance3D
        {
            Mesh = new TorusMesh { InnerRadius = r * 0.8f, OuterRadius = r, Rings = 20, RingSegments = 4 },
            Scale = new Vector3(1, 0.04f, 1), Position = new Vector3(0, 0.03f, 0), CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
            MaterialOverride = new StandardMaterial3D { ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded, AlbedoColor = teamCol }
        };
        AddChild(_ring);
        _prev = _next = ToWorld(f.Pos); Position = _prev;
        _yaw = _yawPrev = _yawNext = -f.Facing;
        Rotation = new Vector3(0, _yaw, 0);
        Play(f.Status == Status.Ok ? "idle" : "down", 1, 0);
    }

    public Vector3 ToWorld(V2 p) => new(p.X - Origin.X, 0, p.Y - Origin.Y);

    bool Has(string n) => _ap != null && _ap.HasAnimation(n);

    void Play(string name, float speed = 1f, float blend = 0.12f, float from = -1f)
    {
        if (_ap == null || !Has(name)) return;
        if (_cur == name && from < 0 && _ap.IsPlaying()) { _ap.SpeedScale = speed; return; }
        _ap.Play(name, blend);
        _ap.SpeedScale = speed;
        if (from >= 0) _ap.Seek(from, true);
        _cur = name;
    }
    void OneShot(string name, float duration, float speed = 1f, bool locks = false, float from = 0f)
    {
        if (!Has(name)) return;
        _ap.Play(name, 0.06f); _ap.SpeedScale = speed; _ap.Seek(from, true);
        _cur = name; _busyUntil = _now + duration; _busyLocks = locks;
    }
    float Len(string n) => Has(n) ? (float)_ap.GetAnimation(n).Length : 0.5f;

    /// <summary>Po kroku symulacji: nowy cel interpolacji.</summary>
    public void AfterStep()
    {
        _prev = _next; _yawPrev = _yawNext;
        _next = ToWorld(F.Pos);
        _yawNext = -F.Facing;
    }

    /// <summary>Zdarzenie z symulacji (cios, strzał, trafienie, upadek...).</summary>
    public void OnFx(FxEvent e, Battle b)
    {
        switch (e.Kind)
        {
            case "swing" when e.A == F.Id:
            {
                float iv = F.Current.Interval;
                float len = Len("attack");
                float sp = MathF.Max(1f, len / MathF.Max(0.35f, iv * 0.9f));
                OneShot("attack", len / sp, sp);
                break;
            }
            case "block" when e.A == F.Id && Has("block"):
                OneShot("block", Len("block"), 1.3f); break;
            case "shot" when e.A == F.Id:
                if (Has("reload") && e.Info == "crossbow") { OneShot("shoot", Len("shoot") * 0.5f, 1f, false, 4f / 12f); _pendingReload = true; }
                else if (_cur != "shoot" || _now > _busyUntil) OneShot("shoot", Len("shoot") - 13f / 12f, 1f, false, 13f / 12f);   // strzał bez naciągu: od pełnego naciągu
                break;
            case "hit" when e.A == F.Id && F.Status == Status.Ok:
                if (_now > _busyUntil || _cur is "idle" or "walk" or "run") OneShot("hit", Len("hit"), 1.2f);
                break;
            case "down" when e.A == F.Id:
                OneShot("fall", Len("fall"), 1f, true); break;
            case "death" when e.A == F.Id:
                if (e.Info == "down" && Has("dead")) { Play("dead", 1, 0.3f); _busyUntil = 0; }
                else OneShot("death", Len("death"), 1f, true);
                break;
            case "scream" when e.A == F.Id:
                OneShot("scream", Len("scream"), 1f); break;
            case "lunge" when e.A == F.Id:
                _prev = Position;   // skok: bez interpolacji od starej pozycji
                break;
        }
    }
    bool _pendingReload;

    public void Tick(double dt, float alpha, Battle b, Func<int, UnitView> getView)
    {
        _now += dt;
        // pozycja i obrót: interpolacja między krokami symulacji
        var pos = _prev.Lerp(_next, alpha);
        float yawT = LerpAngle(_yawPrev, _yawNext, alpha);
        // wyciąganie rannego: ciągnący idzie tyłem, przodem do rannego; ranny leży z głową przy nim
        if (F.CarryingId >= 0 && b.TryGet(F.CarryingId, out _))
        {
            var mv = new Vector2(F.Vel.X, F.Vel.Y);
            if (mv.LengthSquared() > 0.05f) yawT = -MathF.Atan2(-mv.Y, -mv.X);
        }
        if (F.CarriedById >= 0 && getView(F.CarriedById) is UnitView carrier)
        {
            float cy = carrier._yaw;
            var fwd = new Vector3(MathF.Cos(-cy), 0, MathF.Sin(-cy));
            pos = carrier.Position + fwd * 1.45f;
            yawT = cy;
        }
        Position = pos;
        _yaw = LerpAngle(_yaw, yawT, MathF.Min(1f, (float)dt * 12f));
        Rotation = new Vector3(0, _yaw, 0);

        _ring.Visible = F.Status != Status.Dead && F.Status != Status.Fled && (F.Kind != Kind.Rat || Selected);
        if (_ring.MaterialOverride is StandardMaterial3D rm)
        {
            var c = _teamCol; if (F.Status == Status.Down) c = c.Darkened(0.55f);
            rm.AlbedoColor = c;
            _ring.Scale = new Vector3(Selected ? 1.25f : 1f, 0.04f, Selected ? 1.25f : 1f);
        }
        Visible = F.Status != Status.Fled;
        PickAnim(b);
    }

    void PickAnim(Battle b)
    {
        if (_ap == null) return;
        bool busy = _now < _busyUntil;
        if (F.Status == Status.Dead)
        {
            if (!busy && _cur != "dead") Play("dead", 1, 0.25f);
            return;
        }
        if (F.Status == Status.Down)
        {
            if (busy) return;
            Play(F.CarriedById >= 0 ? "dragged" : "down", 1, 0.25f);
            return;
        }
        if (_pendingReload && !busy) { _pendingReload = false; OneShot("reload", Len("reload"), MathF.Max(1f, Len("reload") / MathF.Max(1f, F.Cooldown))); return; }
        float spd = F.Vel.Length;
        if (busy)
        {
            if (_busyLocks) return;
            return;   // akcja trwa (cios, strzał, trafienie); nogi nie przełączają się w jej trakcie
        }
        if (F.CarryingId >= 0) { Play("drag", MathF.Max(0.6f, spd / 0.9f)); return; }
        if (F.Intent == "opatruje") { Play("treat"); return; }
        if (F.IsHuman && (F.Intent.StartsWith("ucieka") ? false : (F.Crouch && F.Suppression > 50) || (F.Morale < 15 && spd < 0.3f))) { Play("cower"); return; }
        // łucznik z celem: naciąg zgrany z odnowieniem tak, żeby puścić cięciwę w chwili strzału
        if (Has("shoot") && !Has("reload") && F.Current.IsRanged && F.TargetId >= 0 && spd < 0.5f && F.Cooldown > 0 && F.Cooldown < 1.25f)
        {
            float release = 15f / 12f;
            float sp = release / MathF.Max(0.3f, F.Cooldown);
            OneShot("shoot", F.Cooldown + 0.05f, sp, false, 0f);
            return;
        }
        if (spd > 2.3f) Play("run", F.IsHuman ? spd / 3.6f : F.Kind == Kind.Ogar ? MathF.Max(0.7f, spd / 7f) : F.Kind == Kind.Rat ? spd / 3.5f : spd / 2.4f);
        else if (spd > 0.25f) Play("walk", F.IsHuman ? MathF.Max(0.6f, spd / 1.64f) : F.Kind == Kind.Ogar ? MathF.Max(0.6f, spd / 1.6f) : MathF.Max(0.6f, spd / 1.3f));
        else Play("idle");
    }

    static float LerpAngle(float a, float b, float t)
    {
        float d = (b - a) % MathF.Tau; if (d > MathF.PI) d -= MathF.Tau; if (d < -MathF.PI) d += MathF.Tau;
        return a + d * t;
    }

    public Vector3 HeadPos => Position + new Vector3(0, F.Kind == Kind.Rat ? 0.3f : F.Kind == Kind.Ogar ? 1.0f : F.Status == Status.Ok ? 2.0f : 0.6f, 0);
}
