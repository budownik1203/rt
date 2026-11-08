using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Latarnia.Sim.Combat;
using Latarnia.Sim.Data;
using Latarnia.Sim.Units;
using Latarnia.Sim.World;
using V2 = Latarnia.Sim.Core.Vec2;

namespace Symulator;

/// <summary>Bitwa 3D: symulacja na żywo (krok 50 ms), modele z animacjami, pixel art (SubViewport ×1/3 + kontur, Mgła, paleta),
/// dymki rozkazów, panel składów, sterowanie kamerą i czasem.</summary>
public partial class BattleView : Control
{
    public Battle B;
    public BattleSpec Spec;
    public Action<Battle> OnFinished;
    public Action OnBack;
    public bool AutoShots;           // tryb bez okna: zrzuty ekranu do out/
    public string ShotPrefix = "bitwa";
    public double ShotInterval = 1.6;  // 0 = każda klatka (nagrywanie z --fixed-fps)
    public bool Follow;                // kamera za środkiem składu A
    public double RecordFrom = 0, RecordTo = 1e9;
    public bool QuitAfterRecord;

    SubViewport _vp; SubViewportContainer _cont;
    Camera3D _cam; ShaderMaterial _post;
    Node3D _world;
    readonly Dictionary<int, UnitView> _views = new();
    readonly Dictionary<Projectile, Node3D> _proj = new();
    readonly List<(Node3D node, double start, double dur, Action<Node3D, double> anim)> _fx = new();
    readonly Dictionary<Flare, Node3D> _flares = new();
    V2 _origin;
    double _acc; float _speed = 1f; bool _paused;
    float _yaw = -30f, _pitch = 52f, _ortho = 13f; Vector3 _target = new(0, 0.9f, 1.0f);
    const int Shrink = 3;
    Control _overlay; Label _clock; VBoxContainer _listA, _listB; RichTextLabel _log;
    readonly Dictionary<int, (Label name, ProgressBar blood, ProgressBar morale, Label intent)> _rows = new();
    UnitView _selected;
    bool _finishedSignalled;
    double _time;
    int _shotIdx; double _nextShot = 2.0;

    public override void _Ready()
    {
        SetAnchorsPreset(LayoutPreset.FullRect);
        _origin = B.Map.LampPos;
        _cont = new SubViewportContainer { Stretch = true, StretchShrink = Shrink, TextureFilter = TextureFilterEnum.Nearest };
        _cont.SetAnchorsPreset(LayoutPreset.FullRect);
        _cont.MouseFilter = MouseFilterEnum.Ignore;
        AddChild(_cont);
        _vp = new SubViewport { Msaa3D = Viewport.Msaa.Disabled, ScreenSpaceAA = Viewport.ScreenSpaceAAEnum.Disabled };
        _cont.AddChild(_vp);
        _world = new Node3D(); _vp.AddChild(_world);
        BuildEnvironment();
        BuildMap();
        BuildCamera();
        foreach (var f in B.All) AddView(f);
        _overlay = new Control { MouseFilter = MouseFilterEnum.Ignore }; _overlay.SetAnchorsPreset(LayoutPreset.FullRect); AddChild(_overlay);
        BuildHud();
    }

    // ================================================================== świat
    void BuildEnvironment()
    {
        var env = new Godot.Environment
        {
            BackgroundMode = Godot.Environment.BGMode.Color, BackgroundColor = new Color(0.07f, 0.065f, 0.09f),
            AmbientLightSource = Godot.Environment.AmbientSource.Color, AmbientLightColor = new Color(0.20f, 0.20f, 0.30f), AmbientLightEnergy = 0.35f,
            TonemapMode = Godot.Environment.ToneMapper.Linear
        };
        _world.AddChild(new WorldEnvironment { Environment = env });
        var lampTop = new Vector3(0, 6.1f, 0);
        var lamp = new OmniLight3D { Position = lampTop, LightColor = new Color(1.0f, 0.78f, 0.45f), LightEnergy = 7.5f, OmniRange = B.Map.LampRadius * 1.35f, OmniAttenuation = 1.1f, ShadowEnabled = true };
        _world.AddChild(lamp);
        var moon = new DirectionalLight3D { RotationDegrees = new Vector3(-55, 130, 0), LightColor = new Color(0.55f, 0.6f, 0.9f), LightEnergy = 0.3f, ShadowEnabled = true };
        _world.AddChild(moon);
        // wypełnienie i kontur tylko na postaciach (warstwa 2): czytelność w ciemności
        _world.AddChild(new DirectionalLight3D { RotationDegrees = new Vector3(-35, -40, 0), LightColor = new Color(1.0f, 0.85f, 0.65f), LightEnergy = 0.35f, LightCullMask = 2 });
        _world.AddChild(new DirectionalLight3D { RotationDegrees = new Vector3(-20, 150, 0), LightColor = new Color(0.5f, 0.65f, 1.0f), LightEnergy = 0.55f, LightCullMask = 2 });
    }

    Vector3 W(V2 p) => new(p.X - _origin.X, 0, p.Y - _origin.Y);

    void BuildMap()
    {
        var g = B.Map;
        // ziemia z szumem
        var ground = new MeshInstance3D { Mesh = new PlaneMesh { Size = new Vector2(g.WidthM + 30, g.HeightM + 30) } };
        var gm = Art.Toon(new Color(0.24f, 0.27f, 0.16f));
        var nt = new NoiseTexture2D { Seamless = true, Width = 256, Height = 256, Noise = new FastNoiseLite { Frequency = 0.02f } };
        gm.SetShaderParameter("noise_tex", nt); gm.SetShaderParameter("noise_amt", 1.0f);
        ground.MaterialOverride = gm;
        ground.Position = new Vector3(g.WidthM / 2 - _origin.X, 0, g.HeightM / 2 - _origin.Y);
        _world.AddChild(ground);
        // latarnia: smukła wieża z cegły i blachy, galeria, laterna ze światłem, daszek (8 m)
        var lh = new Node3D { Name = "latarnia" };
        var brick = new Color(0.62f, 0.36f, 0.26f); var whitewash = new Color(0.86f, 0.82f, 0.74f); var iron = new Color(0.20f, 0.20f, 0.23f);
        lh.AddChild(Art.Cyl(0.95f, 0.5f, new Color(0.40f, 0.38f, 0.36f), new Vector3(0, 0.25f, 0), 8, 0.85f));                  // podmurówka
        for (int i = 0; i < 5; i++)                                                                                                // pasy: cegła i bielone
        {
            float y0 = 0.5f + i * 1.0f; float r0 = 0.70f - i * 0.045f, r1 = 0.70f - (i + 1) * 0.045f;
            lh.AddChild(Art.Cyl(r0, 1.0f, i % 2 == 0 ? brick : whitewash, new Vector3(0, y0 + 0.5f, 0), 10, r1));
        }
        lh.AddChild(Art.Box(new Vector3(0.05f, 0.5f, 0.32f), new Color(0.25f, 0.18f, 0.12f), new Vector3(0.68f, 1.0f, 0)));       // drzwi
        lh.AddChild(Art.Box(new Vector3(0.05f, 0.3f, 0.2f), new Color(0.95f, 0.75f, 0.4f), new Vector3(0.56f, 3.4f, 0)));          // okienko
        lh.AddChild(Art.Cyl(0.95f, 0.12f, iron, new Vector3(0, 5.56f, 0), 12));                                                   // galeria
        for (int i = 0; i < 12; i++) { float a = i * MathF.Tau / 12; lh.AddChild(Art.Cyl(0.02f, 0.5f, iron, new Vector3(0.9f * MathF.Cos(a), 5.85f, 0.9f * MathF.Sin(a)), 4)); }
        lh.AddChild(Art.Cyl(0.95f, 0.04f, iron, new Vector3(0, 6.1f, 0), 12, 0.95f));                                            // poręcz
        var glass = new MeshInstance3D { Mesh = new CylinderMesh { TopRadius = 0.42f, BottomRadius = 0.42f, Height = 0.9f, RadialSegments = 10, Rings = 1 }, MaterialOverride = Art.Toon(new Color(1f, 0.9f, 0.6f), false, new Color(2.4f, 1.8f, 0.9f)), Position = new Vector3(0, 6.08f, 0) };
        lh.AddChild(glass);
        lh.AddChild(Art.Cyl(0.6f, 0.55f, iron, new Vector3(0, 6.8f, 0), 10, 0.08f));                                             // daszek
        lh.AddChild(Art.Cyl(0.05f, 0.4f, iron, new Vector3(0, 7.2f, 0), 4));
        _world.AddChild(lh); NoShadow(lh); SetLayers(lh, 1 | 2);
        // budynki według prostokątów mapy (Maps.SmallSettlement)
        Place("szalas", new V2(-8.5f, -6.5f), 15, 1.6f);
        Place("kuchnia", new V2(7f, -9f), 180, 1.3f);
        Place("szalas", new V2(-10f, 5.5f), -80, 1.6f);
        Place("wedzarnia", new V2(9f, 5.5f), 100, 1.15f);
        Place("studnia", new V2(-3.5f, -9.5f), 0, 1f, solid: false);
        Place("sklad_paliwa", new V2(4.5f, 13.6f), 0, 0.55f, solid: false);
        // skrzynie i beczki (niskie osłony)
        var crates = new (float x0, float y0, float x1, float y1)[] { (-3.5f, 9.5f, -1.5f, 10.5f), (2f, 11f, 3.5f, 12f), (11f, -4f, 12f, -1f), (-4f, -5f, -2.5f, -3.5f), (3f, 2f, 4.5f, 3f) };
        int k = 0;
        var wood = new Color(0.46f, 0.34f, 0.22f); var woodD = new Color(0.30f, 0.21f, 0.14f); var barrel = new Color(0.33f, 0.30f, 0.29f);
        foreach (var (x0, y0, x1, y1) in crates)
        {
            float w = x1 - x0, h = y1 - y0; bool alongX = w >= h; float len = MathF.Max(w, h);
            int n = Math.Max(1, (int)MathF.Round(len / 0.75f));
            for (int i = 0; i < n; i++)
            {
                float t = (i + 0.5f) / n * len;
                var p = new Vector3(x0 + (alongX ? t : w / 2), 0, y0 + (alongX ? h / 2 : t));
                if ((i + k) % 3 == 2)
                {
                    var b = Art.Cyl(0.3f, 0.85f, barrel, p + new Vector3(0, 0.43f, 0), 10); _world.AddChild(b);
                    _world.AddChild(Art.Cyl(0.315f, 0.06f, woodD, p + new Vector3(0, 0.62f, 0), 10)); _world.AddChild(Art.Cyl(0.315f, 0.06f, woodD, p + new Vector3(0, 0.22f, 0), 10));
                }
                else
                {
                    float rot = (i * 37 + k * 11) % 20 - 10;
                    _world.AddChild(Art.Box(new Vector3(0.66f, 0.62f, 0.66f), wood, p + new Vector3(0, 0.31f, 0), rot));
                    _world.AddChild(Art.Box(new Vector3(0.70f, 0.08f, 0.70f), woodD, p + new Vector3(0, 0.6f, 0), rot));
                    _world.AddChild(Art.Box(new Vector3(0.70f, 0.08f, 0.70f), woodD, p + new Vector3(0, 0.04f, 0), rot));
                    if ((i + k) % 2 == 0) _world.AddChild(Art.Box(new Vector3(0.5f, 0.42f, 0.5f), wood.Lightened(0.08f), p + new Vector3(0.03f, 0.85f, -0.02f), rot + 15));
                }
            }
            k++;
        }
        // palisada: ośmiokąt z pali, brama od południa, wyłom od wschodu
        var cc = V2.Zero; float R = 17f;
        var stake = new Color(0.36f, 0.27f, 0.18f);
        var gate = new V2(0, R * MathF.Cos(MathF.PI / 8)); var breach = new V2(R * MathF.Cos(MathF.PI / 8), -2.5f);
        var rnd = new Random(7);
        for (int i = 0; i < 8; i++)
        {
            float a0 = (i + 0.5f) * MathF.Tau / 8, a1 = (i + 1.5f) * MathF.Tau / 8;
            var p0 = V2.FromAngle(a0) * R; var p1 = V2.FromAngle(a1) * R;
            float len = V2.Dist(p0, p1); int n = (int)(len / 0.32f);
            for (int s = 0; s <= n; s++)
            {
                var p = V2.Lerp(p0, p1, s / (float)n);
                if (V2.Dist(p, gate) < 1.7f || V2.Dist(p, breach) < 1.3f) continue;
                float hgt = 2.3f + (float)rnd.NextDouble() * 0.5f;
                var pole = Art.Cyl(0.15f, hgt, stake, new Vector3(p.X, hgt / 2, p.Y), 6, 0.1f);
                pole.RotationDegrees = new Vector3((float)rnd.NextDouble() * 4 - 2, 0, (float)rnd.NextDouble() * 4 - 2);
                _world.AddChild(pole);
            }
            // poprzeczki
            var mid = (p0 + p1) * 0.5f; var dir = (p1 - p0);
            foreach (float hh in new[] { 0.7f, 1.8f })
            {
                var beam = Art.Box(new Vector3(len, 0.12f, 0.12f), stake.Darkened(0.2f), new Vector3(mid.X, hh, mid.Y) + new Vector3(-MathF.Cos(dir.Angle + MathF.PI / 2), 0, -MathF.Sin(dir.Angle + MathF.PI / 2)) * 0.18f);
                beam.Rotation = new Vector3(0, -dir.Angle, 0);
                if (V2.Dist(mid, gate) > 2.5f) _world.AddChild(beam);
            }
        }
        var br = Art.Building("brama"); br.Position = new Vector3(gate.X, 0, gate.Y); br.Scale = new Vector3(0.75f, 0.75f, 0.75f); _world.AddChild(br);
        for (int s = 0; s < 4; s++) { var fl = Art.Cyl(0.13f, 2.2f, stake, new Vector3(breach.X + 1.2f + s * 0.25f, 0.15f, breach.Y - 0.8f + s * 0.45f), 6); fl.RotationDegrees = new Vector3(85, 20 * s, 0); _world.AddChild(fl); }
        // zarośla i wraki za palisadą
        for (int y = 0; y < g.H; y += 2)
            for (int x = 0; x < g.W; x += 2)
            {
                var cell = g.Get(x, y);
                var p = g.CellCenter(x, y);
                if ((cell & Cell.Rough) != 0 && rnd.NextDouble() < 0.6)
                    _world.AddChild(Art.Blob(new Vector3(1.1f, 0.7f, 1.1f) * (0.7f + (float)rnd.NextDouble() * 0.6f), new Color(0.13f, 0.20f, 0.11f), new Vector3(p.X - _origin.X, 0.25f, p.Y - _origin.Y)));
            }
        Wreck(new V2(-7.5f, 22.75f), 0);
        Wreck(new V2(22f, -6.5f), 90);
    }

    void Wreck(V2 c, float rot)
    {
        var rust = new Color(0.38f, 0.22f, 0.14f);
        var n = new Node3D { Position = new Vector3(c.X, 0, c.Y), RotationDegrees = new Vector3(0, rot, 4) };
        n.AddChild(Art.Box(new Vector3(3.2f, 0.7f, 1.5f), rust, new Vector3(0, 0.55f, 0)));
        n.AddChild(Art.Box(new Vector3(1.6f, 0.6f, 1.4f), rust.Darkened(0.15f), new Vector3(-0.3f, 1.15f, 0)));
        foreach (var (x, z) in new[] { (-1.1f, 0.75f), (1.1f, 0.75f), (-1.1f, -0.75f) }) { var wh = Art.Cyl(0.32f, 0.2f, new Color(0.12f, 0.11f, 0.12f), new Vector3(x, 0.3f, z), 8); wh.RotationDegrees = new Vector3(90, 0, 0); n.AddChild(wh); }
        _world.AddChild(n);
    }

    void Place(string model, V2 rel, float rotY, float scale, bool solid = true)
    {
        var bld = Art.Building(model);
        bld.Position = new Vector3(rel.X, 0, rel.Y); bld.RotationDegrees = new Vector3(0, rotY, 0); bld.Scale = Vector3.One * scale;
        _world.AddChild(bld);
    }

    static void SetLayers(Node n, uint l)
    {
        if (n is VisualInstance3D vi) vi.Layers = l;
        foreach (var c in n.GetChildren()) SetLayers(c, l);
    }

    static void NoShadow(Node n)
    {
        if (n is GeometryInstance3D gi) gi.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;
        foreach (var c in n.GetChildren()) NoShadow(c);
    }

    void AddView(Fighter f)
    {
        var v = new UnitView();
        _world.AddChild(v);
        var col = f.Team == Team.Squad ? new Color(0.35f, 0.65f, 1f) : f.IsHuman ? new Color(0.95f, 0.45f, 0.25f) : new Color(0.85f, 0.18f, 0.15f);
        v.Init(f, _origin, col);
        _views[f.Id] = v;
    }

    // ================================================================== kamera
    void BuildCamera()
    {
        _cam = new Camera3D { Projection = Camera3D.ProjectionType.Orthogonal, Size = _ortho, Near = 0.5f, Far = 160f };
        _world.AddChild(_cam);
        var q = new MeshInstance3D { Mesh = new QuadMesh { Size = new Vector2(2, 2) }, ExtraCullMargin = 16384f };
        _post = new ShaderMaterial { Shader = GD.Load<Shader>("res://shaders/post.gdshader") };
        _post.SetShaderParameter("lamp_pos", Vector3.Zero);
        _post.SetShaderParameter("fog_r0", B.Map.LampRadius * 0.85f);
        _post.SetShaderParameter("fog_r1", B.Map.LampRadius * 1.55f);
        q.MaterialOverride = _post;
        _cam.AddChild(q); q.Position = new Vector3(0, 0, -1);
        UpdateCamera();
    }
    void UpdateCamera()
    {
        var basis = Basis.FromEuler(new Vector3(Mathf.DegToRad(-(90f - _pitch)), Mathf.DegToRad(_yaw), 0), EulerOrder.Yxz);
        var pos = _target + basis.Z * 50f;
        float texel = _ortho / Mathf.Max(1, _vp.Size.Y);
        var local = basis.Inverse() * pos;
        local.X = Mathf.Round(local.X / texel) * texel; local.Y = Mathf.Round(local.Y / texel) * texel;
        _cam.GlobalTransform = new Transform3D(basis, basis * local);
        _cam.Size = _ortho;
    }

    // ================================================================== HUD
    void BuildHud()
    {
        var top = Ui.Paper(); top.SetAnchorsPreset(LayoutPreset.TopWide); top.OffsetBottom = 46; top.OffsetLeft = 8; top.OffsetRight = -8; top.OffsetTop = 6;
        AddChild(top);
        var hb = new HBoxContainer(); hb.AddThemeConstantOverride("separation", 10); top.AddChild(hb);
        hb.AddChild(Ui.Button("← Edytor", () => OnBack?.Invoke()));
        hb.AddChild(Ui.Title(Spec.Title, 20));
        _clock = Ui.Label("0:00", 18); hb.AddChild(_clock);
        hb.AddChild(new Control { SizeFlagsHorizontal = SizeFlags.ExpandFill });
        hb.AddChild(Ui.Button("❚❚", () => _paused = !_paused));
        foreach (var s in new[] { 0.5f, 1f, 2f, 4f }) { float sp = s; hb.AddChild(Ui.Button($"×{s:0.#}", () => { _speed = sp; _paused = false; })); }
        hb.AddChild(Ui.Button("Zakończ i raport", () => { B.Over = true; B.EndReason = "przerwane przez gracza"; Finish(); }));
        hb.AddChild(Ui.Label("Q/E obrót · kółko zoom · WASD przesuw · klik: zaznacz", 13, Ui.InkLight));

        var left = Ui.Paper(); left.SetAnchorsPreset(LayoutPreset.LeftWide); left.OffsetTop = 58; left.OffsetBottom = -150; left.OffsetLeft = 8; left.OffsetRight = 268;
        AddChild(left);
        var lv = new VBoxContainer(); left.AddChild(lv);
        lv.AddChild(Ui.Title(B.Squads[0].Name, 17));
        _listA = new VBoxContainer(); lv.AddChild(_listA);
        foreach (var f in B.Squads[0].Members) AddRow(_listA, f);
        var enemyHumans = B.All.Where(f => f.Team == Team.Enemy && f.IsHuman).ToList();
        if (enemyHumans.Count > 0)
        {
            var right = Ui.Paper(); right.SetAnchorsPreset(LayoutPreset.RightWide); right.OffsetTop = 58; right.OffsetBottom = -150; right.OffsetLeft = -268; right.OffsetRight = -8;
            AddChild(right);
            var rv = new VBoxContainer(); right.AddChild(rv);
            rv.AddChild(Ui.Title(B.Squads[1].Name, 17));
            _listB = new VBoxContainer(); rv.AddChild(_listB);
            foreach (var f in enemyHumans) AddRow(_listB, f);
        }
        var bottom = Ui.Paper(); bottom.SetAnchorsPreset(LayoutPreset.BottomWide); bottom.OffsetTop = -140; bottom.OffsetBottom = -8; bottom.OffsetLeft = 8; bottom.OffsetRight = -8;
        AddChild(bottom);
        _log = new RichTextLabel { ScrollFollowing = true, BbcodeEnabled = true, FitContent = false, SizeFlagsVertical = SizeFlags.ExpandFill };
        _log.AddThemeColorOverride("default_color", Ui.Ink); _log.AddThemeFontSizeOverride("normal_font_size", 14);
        bottom.AddChild(_log);
    }

    void AddRow(VBoxContainer list, Fighter f)
    {
        var box = new VBoxContainer(); box.AddThemeConstantOverride("separation", 1);
        var name = Ui.Label($"{f.Name} · {ClassName(f)}", 14);
        var bars = new HBoxContainer();
        var blood = Ui.Bar(new Color(0.62f, 0.12f, 0.10f)); var mor = Ui.Bar(new Color(0.75f, 0.60f, 0.15f));
        bars.AddChild(blood); bars.AddChild(mor);
        var intent = Ui.Label("", 12, Ui.InkLight);
        box.AddChild(name); box.AddChild(bars); box.AddChild(intent);
        list.AddChild(box);
        _rows[f.Id] = (name, blood, mor, intent);
    }
    static string ClassName(Fighter f) => f.Role is Role.Herszt or Role.Lucznik or Role.Obdartus or Role.Nozownik or Role.Rozbijacz ? Latarnia.Sim.Report.ReportBuilder.RolePl(f.Role) : Classes.Get(f.ClassId).NamePl;

    // ================================================================== pętla
    public override void _Process(double delta)
    {
        _time += delta;
        HandleCameraKeys((float)delta);
        if (!_paused && !B.Over)
        {
            _acc += delta * _speed;
            int steps = 0;
            while (_acc >= Battle.Dt && steps < 40 && !B.Over)
            {
                B.Step(); _acc -= Battle.Dt; steps++;
                foreach (var e in B.Fx) OnFx(e);
                foreach (var v in _views.Values) v.AfterStep();
                if (B.All.Count > _views.Count) foreach (var f in B.All) if (!_views.ContainsKey(f.Id)) { AddView(f); _views[f.Id].AfterStep(); }
            }
        }
        float alpha = (float)Mathf.Clamp(_acc / Battle.Dt, 0, 1);
        foreach (var v in _views.Values) v.Tick(delta * (_paused ? 0 : _speed), alpha, B, id => _views.TryGetValue(id, out var u) ? u : null);
        UpdateProjectiles(alpha);
        UpdateFlares();
        UpdateFx(delta);
        UpdateHud();
        DrawBubbles();
        if (B.Over && !_finishedSignalled) { _finishedSignalled = true; GetTree().CreateTimer(2.5).Timeout += Finish; }
        if (Follow)
        {
            var act = B.Squads[0].Members.Where(m => m.Status != Status.Dead).ToList();
            if (act.Count > 0)
            {
                var c = new Vector3(act.Average(m => m.Pos.X) - _origin.X, 0.9f, act.Average(m => m.Pos.Y) - _origin.Y);
                _target = _target.Lerp(c, (float)Math.Min(1, delta * 2)); UpdateCamera();
            }
        }
        if (AutoShots) AutoShot(delta);
    }

    void Finish()
    {
        if (!IsInsideTree()) return;
        OnFinished?.Invoke(B);
    }

    void OnFx(FxEvent e)
    {
        if (_views.TryGetValue(e.A, out var v)) v.OnFx(e, B);
        switch (e.Kind)
        {
            case "explode": Burst(W(e.Pos), new Color(0.75f, 0.85f, 0.25f), 4f, 0.6f, true); break;
            case "scream": Ring(W(e.Pos), new Color(0.9f, 0.4f, 0.85f), 18f, 0.9f); break;
            case "block": Spark(W(e.Pos) + new Vector3(0, 1.1f, 0), new Color(1f, 0.9f, 0.6f)); break;
            case "hit": Spark(W(e.Pos) + new Vector3(0, B.TryGet(e.A, out var t) && t.Kind == Kind.Rat ? 0.15f : 0.9f, 0), new Color(0.75f, 0.1f, 0.1f)); break;
            case "flare": break;
        }
        if (e.Kind is "shout" or "down" or "death" or "flare" or "scream" or "explode" or "ability")
        {
            string who = B.TryGet(e.A, out var f) ? f.Name : "?";
            string txt = e.Kind switch
            {
                "shout" => $"[b]{who}:[/b] {e.Info}", "down" => $"[color=#7a1d16]{who} pada[/color]", "death" => $"[color=#7a1d16]{who} ginie[/color]",
                "flare" => $"[b]{who}[/b] odpala flarę", "scream" => "[color=#6b2a66]Krzykacz krzyczy[/color]", "explode" => "[color=#4a5a12]Nosiciel wybucha[/color]",
                "ability" => $"{who}: [i]{e.Info}[/i]", _ => ""
            };
            if ((e.Kind == "down" || e.Kind == "death") && f != null && !f.IsHuman && f.Kind == Kind.Rat) txt = "";
            if (txt != "") _log.AppendText($"[color=#6b5b4b]{B.Time,5:0.0}[/color]  {txt}\n");
        }
    }

    // ================================================================== pociski, flary, efekty
    void UpdateProjectiles(float alpha)
    {
        foreach (var p in B.Projectiles)
        {
            if (p.Done) continue;
            if (!_proj.TryGetValue(p, out var n))
            {
                n = new Node3D();
                bool bolt = p.Weapon.Id == "crossbow";
                var shaft = new MeshInstance3D { Mesh = new BoxMesh { Size = new Vector3(bolt ? 0.45f : 0.75f, 0.025f, 0.025f) }, MaterialOverride = Art.Toon(new Color(0.55f, 0.42f, 0.26f)) };
                n.AddChild(shaft);
                var tip = new MeshInstance3D { Mesh = new BoxMesh { Size = new Vector3(0.08f, 0.04f, 0.04f) }, MaterialOverride = Art.Toon(new Color(0.7f, 0.7f, 0.72f)), Position = new Vector3(bolt ? 0.24f : 0.39f, 0, 0) };
                n.AddChild(tip);
                _world.AddChild(n); _proj[p] = n;
            }
            var d = (p.To - p.From).Normalized();
            float h = 1.35f - 0.4f * (p.Travelled / MathF.Max(1, p.Total)) + 0.5f * MathF.Sin(MathF.PI * p.Travelled / MathF.Max(1, p.Total)) * MathF.Min(1, p.Total / 25f);
            n.Position = W(p.Pos) + new Vector3(0, h, 0);
            n.Rotation = new Vector3(0, -d.Angle, 0);
        }
        foreach (var kv in _proj.Where(kv => kv.Key.Done || !B.Projectiles.Contains(kv.Key)).ToList())
        {
            kv.Value.QueueFree(); _proj.Remove(kv.Key);
        }
    }

    void UpdateFlares()
    {
        foreach (var fl in B.Flares)
        {
            if (!_flares.TryGetValue(fl, out var n))
            {
                n = new Node3D { Position = W(fl.Pos) };
                var l = new OmniLight3D { LightColor = new Color(1f, 0.35f, 0.25f), LightEnergy = 4f, OmniRange = fl.Radius * 1.3f, Position = new Vector3(0, 3.5f, 0) };
                n.AddChild(l);
                n.AddChild(new MeshInstance3D { Mesh = new SphereMesh { Radius = 0.12f, Height = 0.24f, RadialSegments = 6, Rings = 3 }, MaterialOverride = Art.Toon(new Color(1, 0.4f, 0.3f), false, new Color(3f, 1.0f, 0.6f)), Position = new Vector3(0, 3.5f, 0) });
                _world.AddChild(n); _flares[fl] = n;
            }
            float left = fl.Until - B.Time;
            var light = n.GetChild<OmniLight3D>(0);
            light.LightEnergy = 4f * Mathf.Clamp(left / 4f, 0, 1) * (0.85f + 0.15f * MathF.Sin((float)_time * 23f));
            float hgt = 0.2f + 3.3f * Mathf.Clamp(left / 20f, 0, 1);   // flara opada
            light.Position = new Vector3(0, hgt, 0); n.GetChild<Node3D>(1).Position = new Vector3(0, hgt, 0);
            n.Visible = left > 0;
        }
    }

    void Spark(Vector3 at, Color c)
    {
        var n = new MeshInstance3D { Mesh = new SphereMesh { Radius = 0.05f, Height = 0.1f, RadialSegments = 5, Rings = 2 }, MaterialOverride = Art.Toon(c, false, c * 1.5f), Position = at, CastShadow = GeometryInstance3D.ShadowCastingSetting.Off };
        _world.AddChild(n);
        _fx.Add((n, _time, 0.18, (node, t) => node.Scale = Vector3.One * (float)(0.6 + t * 0.8)));
    }
    void Burst(Vector3 at, Color c, float r, float dur, bool light)
    {
        var n = new MeshInstance3D { Mesh = new SphereMesh { Radius = 1f, Height = 2f, RadialSegments = 10, Rings = 5 }, MaterialOverride = Art.Toon(c, false, c * 1.2f), Position = at + new Vector3(0, 0.6f, 0), CastShadow = GeometryInstance3D.ShadowCastingSetting.Off };
        _world.AddChild(n);
        _fx.Add((n, _time, dur, (node, t) => { node.Scale = Vector3.One * (float)(0.3 + t * r); node.Visible = t < 0.85; }));
        if (light) { var l = new OmniLight3D { LightColor = c, LightEnergy = 6f, OmniRange = r * 3, Position = at + Vector3.Up }; _world.AddChild(l); _fx.Add((l, _time, dur, (node, t) => ((OmniLight3D)node).LightEnergy = (float)(6 * (1 - t)))); }
    }
    void Ring(Vector3 at, Color c, float r, float dur)
    {
        var n = new MeshInstance3D { Mesh = new TorusMesh { InnerRadius = 0.95f, OuterRadius = 1f, Rings = 32, RingSegments = 3 }, MaterialOverride = new StandardMaterial3D { ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded, AlbedoColor = c }, Position = at + new Vector3(0, 0.1f, 0), CastShadow = GeometryInstance3D.ShadowCastingSetting.Off };
        _world.AddChild(n);
        _fx.Add((n, _time, dur, (node, t) => node.Scale = new Vector3((float)(1 + t * r), 0.05f, (float)(1 + t * r))));
    }
    void UpdateFx(double delta)
    {
        for (int i = _fx.Count - 1; i >= 0; i--)
        {
            var (node, start, dur, anim) = _fx[i];
            double t = (_time - start) / dur;
            if (t >= 1) { node.QueueFree(); _fx.RemoveAt(i); continue; }
            anim(node, t);
        }
    }

    // ================================================================== HUD i dymki
    void UpdateHud()
    {
        int mm = (int)(B.Time / 60), ss = (int)(B.Time % 60);
        int enemies = B.All.Count(f => f.Team == Team.Enemy && f.Active);
        _clock.Text = $"{mm}:{ss:00}   wrogów: {enemies}{(B.PendingSpawns > 0 ? $" (+{B.PendingSpawns} fale)" : "")}   {(_paused ? "PAUZA" : $"×{_speed:0.#}")}";
        foreach (var (id, row) in _rows)
        {
            var f = B.Get(id);
            row.blood.Value = f.Blood; row.morale.Value = f.Morale;
            string st = f.Status switch { Status.Dead => "nie żyje", Status.Down => f.Stabilized ? "ranny, opatrzony" : "ranny, krwawi", Status.Fled => "uciekł", _ => f.Intent };
            row.intent.Text = st;
            row.name.AddThemeColorOverride("font_color", f.Status == Status.Ok ? Ui.Ink : f.Status == Status.Dead ? new Color(0.45f, 0.1f, 0.1f) : new Color(0.55f, 0.25f, 0.15f));
        }
    }

    readonly List<Control> _bubbles = new();
    void DrawBubbles()
    {
        foreach (var b in _bubbles) b.QueueFree();
        _bubbles.Clear();
        var placed = new List<Rect2>();
        var items = new List<(Vector2 at, string text, bool squad)>();
        foreach (var v in _views.Values)
        {
            var f = v.F;
            if (f.Status == Status.Dead || f.Status == Status.Fled) continue;
            bool show = B.Time < f.BubbleUntil && f.Bubble != "";
            if (!show && v != _selected) continue;
            if (_cam.IsPositionBehind(v.HeadPos)) continue;
            var sp = _cam.UnprojectPosition(v.HeadPos) * Shrink;
            string txt = show ? f.Bubble : $"{f.Name}: {f.Intent}";
            if (show && f.IsHuman) txt = $"{f.Name}: {f.Bubble}";
            items.Add((sp, txt, f.Team == Team.Squad));
        }
        foreach (var (at, text, squad) in items.OrderBy(i => i.at.Y))
        {
            var pan = Ui.Bubble(text, squad);
            _overlay.AddChild(pan);
            var size = pan.GetCombinedMinimumSize();
            var r = new Rect2(at - new Vector2(size.X / 2, size.Y + 6), size);
            for (int guard = 0; guard < 12 && placed.Any(p => p.Intersects(r.Grow(2))); guard++) r.Position -= new Vector2(0, size.Y + 3);
            r.Position = new Vector2(Mathf.Clamp(r.Position.X, 275, Size.X - 280 - size.X), Mathf.Clamp(r.Position.Y, 52, Size.Y - 150 - size.Y));
            pan.Position = r.Position; placed.Add(r);
            _bubbles.Add(pan);
        }
    }

    void HandleCameraKeys(float dt)
    {
        bool ch = false;
        if (Input.IsKeyPressed(Key.Q)) { _yaw -= 70 * dt; ch = true; }
        if (Input.IsKeyPressed(Key.E)) { _yaw += 70 * dt; ch = true; }
        var basis = Basis.FromEuler(new Vector3(0, Mathf.DegToRad(_yaw), 0));
        var mv = Vector3.Zero;
        if (Input.IsKeyPressed(Key.W)) mv -= basis.Z; if (Input.IsKeyPressed(Key.S)) mv += basis.Z;
        if (Input.IsKeyPressed(Key.A)) mv -= basis.X; if (Input.IsKeyPressed(Key.D)) mv += basis.X;
        if (mv != Vector3.Zero) { _target += mv.Normalized() * _ortho * 0.6f * dt; ch = true; }
        if (_selected != null && Input.IsKeyPressed(Key.F)) { _target = _selected.Position + new Vector3(0, 0.9f, 0); ch = true; }
        if (ch) UpdateCamera();
    }

    public override void _UnhandledInput(InputEvent ev)
    {
        if (ev is InputEventMouseButton mb && mb.Pressed)
        {
            if (mb.ButtonIndex == MouseButton.WheelUp) { _ortho = Mathf.Max(5f, _ortho - 1.2f); UpdateCamera(); }
            else if (mb.ButtonIndex == MouseButton.WheelDown) { _ortho = Mathf.Min(40f, _ortho + 1.2f); UpdateCamera(); }
            else if (mb.ButtonIndex == MouseButton.Left)
            {
                UnitView best = null; float bd = 30;
                foreach (var v in _views.Values)
                {
                    if (v.F.Status == Status.Dead || _cam.IsPositionBehind(v.Position)) continue;
                    var sp = _cam.UnprojectPosition(v.Position + new Vector3(0, 0.8f, 0)) * Shrink;
                    float d = sp.DistanceTo(mb.Position);
                    if (d < bd) { bd = d; best = v; }
                }
                if (_selected != null) _selected.Selected = false;
                _selected = best; if (best != null) best.Selected = true;
            }
        }
        if (ev is InputEventKey k && k.Pressed && k.Keycode == Key.Space) _paused = !_paused;
    }

    // ================================================================== zrzuty (tryb automatyczny)
    void AutoShot(double delta)
    {
        if (B.Time > RecordTo && QuitAfterRecord) { GetTree().Quit(); return; }
        if (B.Time < RecordFrom || B.Time > RecordTo) return;
        if (_time < _nextShot) return;
        _nextShot = _time + ShotInterval;
        var img = GetViewport().GetTexture().GetImage();
        var dir = ProjectSettings.GlobalizePath("res://out");
        System.IO.Directory.CreateDirectory(dir);
        img.SavePng($"{dir}/{ShotPrefix}_{_shotIdx++:000}.png");
        GD.Print($"ZRZUT {ShotPrefix}_{_shotIdx - 1:00} t={B.Time:0.0}");
    }
    public void SetCamera(float yaw, float ortho, Vector3 target) { _yaw = yaw; _ortho = ortho; _target = target; UpdateCamera(); }
    public void SetSpeed(float s) => _speed = s;
}
