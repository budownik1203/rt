using System;
using Godot;

namespace Symulator;

/// <summary>Wygląd interfejsu: „papier na krawędziach” — jasny papier, ciemny tusz, ciemne tło gry. Czytelny, nic nie nachodzi.</summary>
public static class Ui
{
    public static readonly Color PaperC = new(0.87f, 0.83f, 0.72f);
    public static readonly Color PaperDark = new(0.78f, 0.73f, 0.61f);
    public static readonly Color Ink = new(0.16f, 0.12f, 0.10f);
    public static readonly Color InkLight = new(0.38f, 0.31f, 0.26f);
    public static readonly Color Edge = new(0.36f, 0.28f, 0.20f);
    public static readonly Color Accent = new(0.55f, 0.18f, 0.12f);

    static FontFile _sans, _bold, _serif;
    public static FontFile Sans => _sans ??= GD.Load<FontFile>("res://fonts/DejaVuSans.ttf");
    public static FontFile Bold => _bold ??= GD.Load<FontFile>("res://fonts/DejaVuSans-Bold.ttf");
    public static FontFile Serif => _serif ??= GD.Load<FontFile>("res://fonts/DejaVuSerif-Bold.ttf");

    public static Theme MakeTheme()
    {
        var t = new Theme { DefaultFont = Sans, DefaultFontSize = 15 };
        foreach (var cls in new[] { "Label", "Button", "OptionButton", "CheckBox", "LineEdit", "SpinBox", "ItemList", "RichTextLabel", "PopupMenu" })
            t.SetColor("font_color", cls, Ink);
        t.SetColor("font_hover_color", "Button", Accent); t.SetColor("font_pressed_color", "Button", Accent); t.SetColor("font_focus_color", "Button", Ink);
        t.SetColor("font_hover_color", "OptionButton", Accent); t.SetColor("font_focus_color", "OptionButton", Ink);
        t.SetColor("font_color", "PopupMenu", Ink); t.SetColor("font_hover_color", "PopupMenu", Accent);
        t.SetColor("font_selected_color", "ItemList", PaperC);
        t.SetColor("font_color", "CheckBox", Ink); t.SetColor("font_hover_color", "CheckBox", Accent); t.SetColor("font_pressed_color", "CheckBox", Ink); t.SetColor("font_focus_color", "CheckBox", Ink);
        var btn = Box(PaperDark, Edge, 1, 4); var btnH = Box(new Color(0.83f, 0.77f, 0.63f), Accent, 1, 4); var btnP = Box(new Color(0.72f, 0.66f, 0.52f), Accent, 2, 4);
        foreach (var cls in new[] { "Button", "OptionButton" })
        {
            t.SetStylebox("normal", cls, btn); t.SetStylebox("hover", cls, btnH); t.SetStylebox("pressed", cls, btnP); t.SetStylebox("focus", cls, Box(new Color(0, 0, 0, 0), Accent, 1, 4));
            t.SetStylebox("disabled", cls, Box(PaperC, InkLight, 1, 4));
        }
        var field = Box(new Color(0.94f, 0.91f, 0.83f), Edge, 1, 3);
        t.SetStylebox("normal", "LineEdit", field); t.SetStylebox("focus", "LineEdit", Box(new Color(0.96f, 0.93f, 0.86f), Accent, 1, 3));
        t.SetColor("caret_color", "LineEdit", Ink); t.SetColor("selection_color", "LineEdit", new Color(0.8f, 0.6f, 0.4f));
        t.SetStylebox("panel", "PopupMenu", Box(PaperC, Edge, 1, 3));
        t.SetStylebox("panel", "ItemList", Box(new Color(0.92f, 0.88f, 0.79f), Edge, 1, 3));
        t.SetStylebox("selected", "ItemList", Box(Edge, Edge, 0, 2)); t.SetStylebox("selected_focus", "ItemList", Box(Edge, Edge, 0, 2));
        t.SetStylebox("panel", "PanelContainer", Box(PaperC, Edge, 2, 6, 10));
        t.SetStylebox("panel", "TooltipPanel", Box(new Color(0.95f, 0.92f, 0.84f), Edge, 1, 3, 6));
        t.SetColor("font_color", "TooltipLabel", Ink);
        t.SetStylebox("background", "ProgressBar", Box(new Color(0.55f, 0.50f, 0.42f), Edge, 1, 2, 0));
        t.SetStylebox("grabber_area", "HSlider", Box(Edge, Edge, 0, 2, 0));
        t.SetStylebox("slider", "HSlider", Box(new Color(0.65f, 0.6f, 0.5f), Edge, 1, 2, 0));
        t.SetColor("font_color", "TabBar", InkLight); t.SetColor("font_selected_color", "TabBar", Ink); t.SetColor("font_hovered_color", "TabBar", Accent);
        t.SetStylebox("tab_selected", "TabBar", Box(PaperC, Edge, 1, 4, 6)); t.SetStylebox("tab_unselected", "TabBar", Box(PaperDark, Edge, 1, 4, 6)); t.SetStylebox("tab_hovered", "TabBar", Box(new Color(0.83f, 0.77f, 0.63f), Edge, 1, 4, 6));
        return t;
    }

    public static StyleBoxFlat Box(Color bg, Color border, int bw, int radius, int margin = 6)
    {
        var s = new StyleBoxFlat { BgColor = bg, BorderColor = border, CornerRadiusBottomLeft = radius, CornerRadiusBottomRight = radius, CornerRadiusTopLeft = radius, CornerRadiusTopRight = radius };
        s.SetBorderWidthAll(bw); s.SetContentMarginAll(margin);
        s.ShadowColor = new Color(0, 0, 0, 0.25f); s.ShadowSize = bw > 1 ? 3 : 0;
        return s;
    }

    public static PanelContainer Paper() => new();

    public static Label Label(string text, int size = 15, Color? col = null)
    {
        var l = new Label { Text = text };
        l.AddThemeFontSizeOverride("font_size", size);
        l.AddThemeColorOverride("font_color", col ?? Ink);
        return l;
    }
    public static Label Title(string text, int size = 20)
    {
        var l = Label(text, size); l.AddThemeFontOverride("font", Serif); return l;
    }
    public static Button Button(string text, Action onClick, string tip = null)
    {
        var b = new Button { Text = text, TooltipText = tip ?? "" };
        b.Pressed += () => onClick();
        return b;
    }
    public static ProgressBar Bar(Color fill)
    {
        var p = new ProgressBar { MinValue = 0, MaxValue = 100, ShowPercentage = false, CustomMinimumSize = new Vector2(110, 7), SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
        p.AddThemeStyleboxOverride("fill", Box(fill, fill, 0, 2, 0));
        return p;
    }
    public static SpinBox Spin(double min, double max, double val, Action<double> onChange, double step = 1)
    {
        var s = new SpinBox { MinValue = min, MaxValue = max, Step = step, Value = val, CustomMinimumSize = new Vector2(78, 0) };
        s.GetLineEdit().AddThemeColorOverride("font_color", Ink);
        s.ValueChanged += v => onChange(v);
        return s;
    }
    public static OptionButton Option(string[] items, int selected, Action<int> onSel)
    {
        var o = new OptionButton();
        foreach (var it in items) o.AddItem(it);
        o.Selected = Math.Clamp(selected, 0, Math.Max(0, items.Length - 1));
        o.ItemSelected += i => onSel((int)i);
        return o;
    }
    public static HBoxContainer Row(string label, Control c, int labelW = 130, string tip = null)
    {
        var h = new HBoxContainer();
        var l = Label(label, 14); l.CustomMinimumSize = new Vector2(labelW, 0); l.TooltipText = tip ?? ""; l.MouseFilter = Control.MouseFilterEnum.Pass;
        h.AddChild(l); c.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill; h.AddChild(c);
        return h;
    }

    /// <summary>Dymek rozkazu: skrawek papieru (swoi) albo ciemna kartka (wróg).</summary>
    public static PanelContainer Bubble(string text, bool squad)
    {
        var p = new PanelContainer { MouseFilter = Control.MouseFilterEnum.Ignore };
        p.AddThemeStyleboxOverride("panel", Box(squad ? new Color(0.96f, 0.93f, 0.84f) : new Color(0.30f, 0.12f, 0.10f), squad ? Edge : new Color(0.15f, 0.05f, 0.04f), 1, 3, 4));
        var l = Label(text, 13, squad ? Ink : new Color(1f, 0.85f, 0.78f));
        l.MouseFilter = Control.MouseFilterEnum.Ignore;
        p.AddChild(l);
        return p;
    }
}
