using System.Collections.Generic;
using System.Linq;
using Latarnia.Sim.Units;

namespace Latarnia.Sim.Combat;

public enum LogKind { Wound, Down, Kill, Fled, FriendlyFire, Order, Reply, Rescue, Scream, Explosion, Contact, Decision, Chaos }

public sealed record LogEvent(float T, LogKind Kind, int Actor, int Target, string Text);

public sealed class FrameUnit
{
    public int Id; public float X, Y, F; public int Team; public string Kind = ""; public string Role = ""; public string Name = "";
    public string St = ""; public string Say = ""; public string Do = ""; public int Task; public float Blood, Morale; public bool Carried;
}
public sealed class FrameThreat { public float X, Y, R; public int N; public string Label = ""; public int Squad; }
public sealed class FrameTask { public float X, Y; public string Type = ""; public int[] Who = System.Array.Empty<int>(); public int Squad; public int Id; }
public sealed class Frame
{
    public float T;
    public List<FrameUnit> U = new();
    public List<float[]> P = new();     // pociski: x, y, dx, dy
    public List<FrameThreat> Th = new();
    public List<FrameTask> Tk = new();
    public List<float[]> Fl = new();    // flary: x, y, promień, siła
}

public sealed class BattleLog
{
    public readonly List<LogEvent> Events = new();
    public readonly List<Frame> Frames = new();
    public bool RecordFrames = true;

    public void Event(float t, LogKind k, int actor, int target, string text) => Events.Add(new LogEvent(t, k, actor, target, text));

    public void Snapshot(Battle b)
    {
        if (!RecordFrames) return;
        var fr = new Frame { T = b.Time };
        foreach (var f in b.All)
        {
            if (f.Status == Status.Fled) continue;
            fr.U.Add(new FrameUnit
            {
                Id = f.Id, X = f.Pos.X, Y = f.Pos.Y, F = f.Facing, Team = (int)f.Team, Kind = f.Kind.ToString(), Role = f.Role.ToString(), Name = f.Name,
                St = f.Status.ToString(), Say = b.Time < f.BubbleUntil ? f.Bubble : "", Do = f.Intent, Task = f.Order.TaskId, Blood = f.Blood, Morale = f.Morale, Carried = f.CarriedById >= 0
            });
        }
        foreach (var p in b.Projectiles) { var d = (p.To - p.From).Normalized(); fr.P.Add(new[] { p.Pos.X, p.Pos.Y, d.X, d.Y }); }
        foreach (var fl in b.Flares) if (b.Time < fl.Until) fr.Fl.Add(new[] { fl.Pos.X, fl.Pos.Y, fl.Radius, Math.Clamp((fl.Until - b.Time) / 4f, 0, 1) });
        foreach (var s in b.Squads)
        {
            var c = s.Commander; if (c == null) continue;
            foreach (var th in c.LastThreats) fr.Th.Add(new FrameThreat { X = th.Center.X, Y = th.Center.Y, R = th.Radius, N = th.Members.Count, Label = th.Label, Squad = s.Index });
            if (c.Current != null)
                foreach (var tk in c.Current.Tasks) fr.Tk.Add(new FrameTask { X = tk.Pos.X, Y = tk.Pos.Y, Type = tk.Type.ToString(), Who = tk.Assigned.Select(a => a.Id).ToArray(), Squad = s.Index, Id = tk.Id });
        }
        Frames.Add(fr);
    }
}
