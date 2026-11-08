using System;
using System.Linq;
using Godot;
using Latarnia.Sim.Combat;
using Latarnia.Sim.Report;
using Latarnia.Sim.Units;

namespace Symulator;

/// <summary>Raport po walce: wynik, ludzie, wnioski według specjalizacji dowódcy, kluczowe chwile, dziennik rozkazów.</summary>
public partial class ReportScreen : Control
{
    public Battle B;
    public Action OnEditor, OnReplay, OnNewSeed;

    public override void _Ready()
    {
        SetAnchorsPreset(LayoutPreset.FullRect);
        var bg = new ColorRect { Color = new Color(0.10f, 0.09f, 0.11f) }; AddChild(bg); bg.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        var paper = Ui.Paper(); AddChild(paper); paper.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        paper.OffsetLeft = 60; paper.OffsetRight = -60; paper.OffsetTop = 24; paper.OffsetBottom = -24;
        var v = new VBoxContainer(); v.AddThemeConstantOverride("separation", 8); paper.AddChild(v);
        var r = ReportBuilder.Build(B, B.Squads[0]);
        var head = new HBoxContainer(); v.AddChild(head);
        head.AddChild(Ui.Title($"Raport: {r.Outcome} po {r.Time:0} s", 24));
        head.AddChild(new Control { SizeFlagsHorizontal = SizeFlags.ExpandFill });
        head.AddChild(Ui.Button("Powtórz (to samo ziarno)", () => OnReplay?.Invoke()));
        head.AddChild(Ui.Button("Nowe ziarno", () => OnNewSeed?.Invoke()));
        head.AddChild(Ui.Button("Edytor", () => OnEditor?.Invoke()));
        var cols = new HBoxContainer { SizeFlagsVertical = SizeFlags.ExpandFill }; cols.AddThemeConstantOverride("separation", 16); v.AddChild(cols);
        var left = new RichTextLabel { BbcodeEnabled = true, SizeFlagsHorizontal = SizeFlags.ExpandFill, SizeFlagsVertical = SizeFlags.ExpandFill };
        var right = new RichTextLabel { BbcodeEnabled = true, SizeFlagsHorizontal = SizeFlags.ExpandFill, SizeFlagsVertical = SizeFlags.ExpandFill };
        foreach (var t in new[] { left, right }) { t.AddThemeColorOverride("default_color", Ui.Ink); t.AddThemeFontSizeOverride("normal_font_size", 15); t.AddThemeFontOverride("bold_font", Ui.Bold); }
        cols.AddChild(left); cols.AddChild(right);
        left.AppendText($"[b]Skład:[/b] {r.Unhurt} cali, {r.Wounded} ranni, {r.Rescued} opatrzeni lub wyniesieni, {r.Dead} zabici{(r.Fled > 0 ? $", {r.Fled} uciekło" : "")}\n");
        left.AppendText($"[b]Wrogowie:[/b] {r.EnemiesKilled}/{r.EnemiesTotal} pokonani ({string.Join(", ", r.KilledByKind.Select(kv => $"{kv.Key} {kv.Value}"))})\n");
        left.AppendText($"[b]Reakcje dowódcy[/b] na zagrożenia, które raniły ludzi: {r.Accurate} trafne, {r.Late} spóźnione, {r.Missed} bez reakcji, {r.Surprise} zaskoczenia\n");
        if (r.FriendlyHits > 0) left.AppendText($"[b]Ogień bratobójczy:[/b] {r.FriendlyHits}\n");
        left.AppendText("\n[b]Ludzie[/b]\n");
        foreach (var p in r.People) left.AppendText("  " + p + "\n");
        if (B.Squads.Count > 1 && B.Squads[1].Commander != null && B.Squads[1].Members.Any(m => m.IsHuman))
        {
            left.AppendText($"\n[b]{B.Squads[1].Name}[/b]\n");
            foreach (var m in B.Squads[1].Members)
                left.AppendText($"  {m.Name}: {(m.Status switch { Status.Dead => "zginął", Status.Down => "ciężko ranny", Status.Fled => "uciekł", _ => m.Wounds.Count > 0 ? "ranny" : "cały" })}; wyłączył {m.Kills}\n");
        }
        right.AppendText("[b]Wnioski[/b]\n");
        foreach (var f in r.Findings.OrderByDescending(f => f.Weight).Take(6)) right.AppendText($"  [color=#7a2a1a]{BattleReport.SpecPl(f.Spec)}[/color]: {f.Text}\n");
        if (r.Findings.Count == 0) right.AppendText("  Bez zastrzeżeń.\n");
        if (r.Moments.Count > 0) { right.AppendText("\n[b]Kluczowe chwile[/b]\n"); foreach (var m in r.Moments) right.AppendText("  " + m + "\n"); }
        right.AppendText("\n[b]Dziennik rozkazów[/b]\n");
        foreach (var e in B.Log.Events.Where(e => e.Kind is LogKind.Order or LogKind.Reply or LogKind.Chaos or LogKind.Down or LogKind.Rescue).Where(e => e.Actor < 0 || B.Get(e.Actor).Team == Team.Squad || e.Kind == LogKind.Down).Take(60))
            right.AppendText($"  [color=#6b5b4b]{e.T,5:0.0}[/color]  {e.Text}\n");
    }
}
