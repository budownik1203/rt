using System.Collections.Generic;
using Godot;

namespace Symulator;

/// <summary>Wygląd: materiał toon, scalanie siatek (1 wywołanie rysowania na postać/budynek) z pamięcią podręczną na typ modelu,
/// proste rekwizyty proceduralne (palisada, skrzynie, krzaki, wraki) w tym samym stylu.</summary>
public static class Art
{
    static Shader _toon;
    public static ShaderMaterial Toon(Color c, bool vcol = false, Color? emis = null)
    {
        _toon ??= GD.Load<Shader>("res://shaders/toon.gdshader");
        var m = new ShaderMaterial { Shader = _toon };
        m.SetShaderParameter("albedo", c);
        m.SetShaderParameter("use_vcol", vcol);
        var e = emis ?? Colors.Black;
        m.SetShaderParameter("emission", new Vector3(e.R, e.G, e.B));
        return m;
    }

    static ShaderMaterial _vcolMat;
    public static ShaderMaterial VCol => _vcolMat ??= Toon(Colors.White, true);

    // ---------------------------------------------------------------- scalanie
    sealed class Merged { public Mesh Mesh; public Skin Skin; }
    static readonly Dictionary<string, Merged> _chars = new();
    static readonly Dictionary<string, Mesh> _static = new();

    static void Collect(Node n, List<MeshInstance3D> outp)
    {
        if (n is MeshInstance3D mi) outp.Add(mi);
        foreach (var c in n.GetChildren()) Collect(c, outp);
    }
    public static Skeleton3D FindSkeleton(Node n)
    {
        if (n is Skeleton3D s) return s;
        foreach (var c in n.GetChildren()) { var r = FindSkeleton(c); if (r != null) return r; }
        return null;
    }
    public static AnimationPlayer FindAnim(Node n)
    {
        if (n is AnimationPlayer a) return a;
        foreach (var c in n.GetChildren()) { var r = FindAnim(c); if (r != null) return r; }
        return null;
    }
    static (Color col, bool glow) MatInfo(Material m)
    {
        if (m is BaseMaterial3D b)
        {
            if (b.EmissionEnabled && (b.Emission * b.EmissionEnergyMultiplier).Luminance > 0.05f) { var e = b.Emission; return (new Color(e.R, e.G, e.B, 0f), true); }
            return (b.AlbedoColor, false);
        }
        return (new Color(0.7f, 0.7f, 0.7f), false);
    }

    /// <summary>Instancja postaci z jedną siatką skinowaną (części sztywne na kościach -> wagi 1 na kość).</summary>
    public static Node3D Character(string model, Node parent)
    {
        var inst = GD.Load<PackedScene>($"res://models/units/{model}.glb").Instantiate<Node3D>();
        parent.AddChild(inst);
        var skel = FindSkeleton(inst);
        if (skel == null) return inst;
        var parts = new List<MeshInstance3D>(); Collect(inst, parts);
        if (!_chars.TryGetValue(model, out var mg))
        {
            var st = new SurfaceTool();
            st.SetSkinWeightCount(SurfaceTool.SkinWeightCount.Skin4Weights);
            st.Begin(Mesh.PrimitiveType.Triangles);
            var toSkel = skel.GlobalTransform.AffineInverse();
            foreach (var mi in parts)
            {
                int bone = -1;
                if (mi.GetParent() is BoneAttachment3D ba) bone = skel.FindBone(ba.BoneName);
                if (bone < 0) continue;
                var xf = toSkel * mi.GlobalTransform;
                for (int s = 0; s < mi.Mesh.GetSurfaceCount(); s++)
                {
                    var arr = mi.Mesh.SurfaceGetArrays(s);
                    var (col, _) = MatInfo(mi.GetActiveMaterial(s));
                    var verts = arr[(int)Mesh.ArrayType.Vertex].AsVector3Array();
                    var norms = arr[(int)Mesh.ArrayType.Normal].AsVector3Array();
                    var idxV = arr[(int)Mesh.ArrayType.Index];
                    int[] order = idxV.VariantType == Variant.Type.Nil ? null : idxV.AsInt32Array();
                    int count = order != null && order.Length > 0 ? order.Length : verts.Length;
                    for (int k = 0; k < count; k++)
                    {
                        int i = order != null && order.Length > 0 ? order[k] : k;
                        st.SetColor(col);
                        st.SetNormal(norms.Length > i ? (xf.Basis * norms[i]).Normalized() : Vector3.Up);
                        st.SetBones(new[] { bone, 0, 0, 0 });
                        st.SetWeights(new[] { 1f, 0f, 0f, 0f });
                        st.AddVertex(xf * verts[i]);
                    }
                }
            }
            var skin = new Skin();
            for (int b = 0; b < skel.GetBoneCount(); b++) skin.AddNamedBind(skel.GetBoneName(b), skel.GetBoneGlobalRest(b).AffineInverse());
            mg = new Merged { Mesh = st.Commit(), Skin = skin };
            _chars[model] = mg;
        }
        foreach (var mi in parts) { mi.Visible = false; mi.QueueFree(); }
        var outMi = new MeshInstance3D { Name = "postac", Mesh = mg.Mesh, Skin = mg.Skin, MaterialOverride = VCol };
        skel.AddChild(outMi);
        outMi.Skeleton = outMi.GetPathTo(skel);
        outMi.Layers = 1 | 2;
        return inst;
    }

    /// <summary>Budynek: jedna statyczna siatka (pamięć podręczna na model).</summary>
    public static Node3D Building(string model)
    {
        if (!_static.TryGetValue(model, out var mesh))
        {
            var inst = GD.Load<PackedScene>($"res://models/buildings/{model}.glb").Instantiate<Node3D>();
            var parts = new List<MeshInstance3D>(); Collect(inst, parts);
            var st = new SurfaceTool(); st.Begin(Mesh.PrimitiveType.Triangles);
            var holder = new Node3D(); holder.AddChild(inst);   // poza drzewem: liczymy transformacje ręcznie
            foreach (var mi in parts)
            {
                var xf = Transform3D.Identity; Node n = mi;
                while (n != null && n != holder) { if (n is Node3D n3) xf = n3.Transform * xf; n = n.GetParent(); }
                for (int s = 0; s < mi.Mesh.GetSurfaceCount(); s++)
                {
                    var arr = mi.Mesh.SurfaceGetArrays(s);
                    var (col, _) = MatInfo(mi.GetActiveMaterial(s));
                    var verts = arr[(int)Mesh.ArrayType.Vertex].AsVector3Array();
                    var norms = arr[(int)Mesh.ArrayType.Normal].AsVector3Array();
                    var idxV = arr[(int)Mesh.ArrayType.Index];
                    int[] order = idxV.VariantType == Variant.Type.Nil ? null : idxV.AsInt32Array();
                    int count = order != null && order.Length > 0 ? order.Length : verts.Length;
                    for (int k = 0; k < count; k++)
                    {
                        int i = order != null && order.Length > 0 ? order[k] : k;
                        st.SetColor(col);
                        st.SetNormal(norms.Length > i ? (xf.Basis * norms[i]).Normalized() : Vector3.Up);
                        st.AddVertex(xf * verts[i]);
                    }
                }
            }
            mesh = st.Commit(); _static[model] = mesh;
            holder.Free();
        }
        var root = new Node3D { Name = model };
        root.AddChild(new MeshInstance3D { Mesh = mesh, MaterialOverride = VCol });
        return root;
    }

    // ---------------------------------------------------------------- rekwizyty proceduralne
    public static MeshInstance3D Box(Vector3 size, Color c, Vector3 pos, float rotY = 0)
    {
        var mi = new MeshInstance3D { Mesh = new BoxMesh { Size = size }, MaterialOverride = Toon(c) };
        mi.Position = pos; mi.RotationDegrees = new Vector3(0, rotY, 0);
        return mi;
    }
    public static MeshInstance3D Cyl(float r, float h, Color c, Vector3 pos, int seg = 6, float r2 = -1)
    {
        var mi = new MeshInstance3D { Mesh = new CylinderMesh { TopRadius = r2 < 0 ? r : r2, BottomRadius = r, Height = h, RadialSegments = seg, Rings = 1 }, MaterialOverride = Toon(c) };
        mi.Position = pos;
        return mi;
    }
    public static MeshInstance3D Blob(Vector3 scale, Color c, Vector3 pos)
    {
        var mi = new MeshInstance3D { Mesh = new SphereMesh { Radius = 0.5f, Height = 1f, RadialSegments = 8, Rings = 4 }, MaterialOverride = Toon(c) };
        mi.Position = pos; mi.Scale = scale;
        return mi;
    }
}
