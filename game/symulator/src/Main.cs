using System;
using Godot;
using Latarnia.Sim.Combat;
using Latarnia.Sim.Data;
using Latarnia.Sim.Scenarios;

namespace Symulator;

/// <summary>Przełącza ekrany: edytor → bitwa 3D → raport. Argumenty: -- demo [preset] (bitwa od razu i zrzuty), -- edytor (zrzut edytora).</summary>
public partial class Main : Node
{
    Control _screen;
    EditorScreen _editor;
    BattleSpec _last;
    string[] _args;

    public override void _Ready()
    {
        _args = OS.GetCmdlineUserArgs();
        var layer = new CanvasLayer(); AddChild(layer);
        _root = new Control { Theme = Ui.MakeTheme() }; layer.AddChild(_root); _root.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        ShowEditor();
        if (_args.Length > 0 && _args[0] == "demo")
        {
            var spec = Presets.Default();
            if (_args.Length > 1) spec = _args[1] switch
            {
                "szabrownicy" => new BattleSpec { Title = "Szabrownicy atakują", Squad = Presets.Mroz(), Opponent = OpponentType.Sklad, Enemy = Presets.Szabrownicy(2) },
                "starcie" => new BattleSpec { Title = "Starcie w polu", Squad = Presets.Mroz(), Opponent = OpponentType.Sklad, Enemy = Presets.Szabrownicy(2), Mode = ClashMode.Starcie },
                "ogary" => new BattleSpec { Title = "Ogary", Squad = Presets.Mroz(), Waves = Presets.Waves("ogary") },
                "szczury" => new BattleSpec { Title = "Szczurza Fala", Squad = Presets.Mroz(), Waves = Presets.Waves("szczury") },
                _ => spec
            };
            if (_args.Length > 2) spec.Seed = ulong.Parse(_args[2]);
            StartBattle(spec, auto: true);
            var bv = (BattleView)_screen;
            // -- demo preset ziarno [zoom] [nagraj od do]
            if (_args.Length > 3) { bv.Follow = true; bv.SetCamera(-30, float.Parse(_args[3], System.Globalization.CultureInfo.InvariantCulture), new Vector3(0, 0.9f, 1)); }
            if (_args.Length > 5) { bv.ShotInterval = 0; bv.RecordFrom = double.Parse(_args[4], System.Globalization.CultureInfo.InvariantCulture); bv.RecordTo = double.Parse(_args[5], System.Globalization.CultureInfo.InvariantCulture); bv.SetSpeed(1f); bv.QuitAfterRecord = true; }
        }
        if (_args.Length > 0 && _args[0] == "przelicz")
        {
            _editor.Bench();
            var t = GetTree().CreateTimer(60); t.Timeout += () => { GD.Print("PRZELICZ: ", _editor.BenchText); GetTree().Quit(); };
            GetTree().ProcessFrame += () => { if (_editor.BenchText != null && !_editor.BenchText.StartsWith("Liczę")) { GD.Print("PRZELICZ: ", _editor.BenchText); Shot("edytor_przelicz"); GetTree().Quit(); } };
        }
        if (_args.Length > 0 && _args[0] == "edytor")
            GetTree().CreateTimer(1.5).Timeout += () => { Shot("edytor"); GetTree().Quit(); };
    }
    Control _root;

    void Swap(Control c) { _screen?.QueueFree(); _screen = c; _root.AddChild(c); c.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect); }

    void ShowEditor()
    {
        _editor = new EditorScreen();
        if (_last != null) _editor.Spec = _last;
        _editor.OnFight = s => StartBattle(s, false);
        Swap(_editor);
    }

    void StartBattle(BattleSpec spec, bool auto)
    {
        _last = spec.Clone();
        var b = Scenarios.Build(spec);
        var v = new BattleView { B = b, Spec = spec, AutoShots = auto, ShotPrefix = "bitwa_" + (auto && _args.Length > 1 ? _args[1] : "noc") };
        v.OnBack = ShowEditor;
        v.OnFinished = battle => { if (auto) { GetTree().CreateTimer(0.6).Timeout += () => { ShowReport(battle); GetTree().CreateTimer(1.0).Timeout += () => { Shot("raport"); GetTree().Quit(); }; }; } else ShowReport(battle); };
        Swap(v);
        if (auto) { v.SetSpeed(2f); }
    }

    void ShowReport(Battle b)
    {
        var r = new ReportScreen { B = b };
        r.OnEditor = ShowEditor;
        r.OnReplay = () => StartBattle(_last.Clone(), false);
        r.OnNewSeed = () => { var s = _last.Clone(); s.Seed = (ulong)(Random.Shared.Next(1, 999999)); StartBattle(s, false); };
        Swap(r);
    }

    void Shot(string name)
    {
        var dir = ProjectSettings.GlobalizePath("res://out"); System.IO.Directory.CreateDirectory(dir);
        GetViewport().GetTexture().GetImage().SavePng($"{dir}/{name}.png");
        GD.Print("ZRZUT ", name);
    }
}
