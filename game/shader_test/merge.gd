# Scalanie siatek Latarni: sztywne części na BoneAttachment3D -> jedna siatka ze szkieletem (1 wywołanie rysowania).
# Kolory materiałów trafiają do kolorów wierzchołków, więc cała postać ma jeden materiał.
# Części sterowane zdarzeniami (błysk, magazynek w dłoni, granat) zostają osobno.
class_name Merge

static func is_event_part(n: Node) -> bool:
	var low := String(n.name).to_lower()
	return low.contains("flash") or low.contains("maghand") or low.contains("gren")

static func collect(n: Node, out: Array) -> void:
	if n is MeshInstance3D and not is_event_part(n): out.append(n)
	for c in n.get_children(): collect(c, out)

static func find_skeleton(n: Node) -> Skeleton3D:
	if n is Skeleton3D: return n
	for c in n.get_children():
		var r := find_skeleton(c)
		if r: return r
	return null

static func mat_color(m: Material) -> Color:
	if m is BaseMaterial3D: return (m as BaseMaterial3D).albedo_color
	return Color(0.7, 0.7, 0.7)

static func mat_emission(m: Material) -> Color:
	if m is BaseMaterial3D and (m as BaseMaterial3D).emission_enabled:
		var b := m as BaseMaterial3D
		return b.emission * b.emission_energy_multiplier
	return Color.BLACK

# Postać: zwraca liczbę scalonych części.
static func merge_character(root: Node3D, material: Material) -> int:
	var skel := find_skeleton(root)
	if skel == null: return 0
	var parts: Array = []
	collect(root, parts)
	var st := SurfaceTool.new()
	st.set_skin_weight_count(SurfaceTool.SKIN_4_WEIGHTS)
	st.begin(Mesh.PRIMITIVE_TRIANGLES)
	var to_skel := skel.global_transform.affine_inverse()
	var count := 0
	for mi: MeshInstance3D in parts:
		var bone := -1
		var p := mi.get_parent()
		if p is BoneAttachment3D: bone = skel.find_bone((p as BoneAttachment3D).bone_name)
		if bone < 0: continue
		var xf := to_skel * mi.global_transform
		for s in mi.mesh.get_surface_count():
			var arr := mi.mesh.surface_get_arrays(s)
			var mat := mi.get_active_material(s)
			var col := mat_color(mat)
			var verts: PackedVector3Array = arr[Mesh.ARRAY_VERTEX]
			var norms: PackedVector3Array = arr[Mesh.ARRAY_NORMAL]
			var idx = arr[Mesh.ARRAY_INDEX]
			var order: PackedInt32Array = idx if idx != null and idx.size() > 0 else PackedInt32Array(range(verts.size()))
			for i in order:
				st.set_color(col)
				st.set_normal((xf.basis * norms[i]).normalized() if norms.size() > i else Vector3.UP)
				st.set_bones(PackedInt32Array([bone, 0, 0, 0]))
				st.set_weights(PackedFloat32Array([1.0, 0.0, 0.0, 0.0]))
				st.add_vertex(xf * verts[i])
		mi.visible = false
		mi.queue_free()
		count += 1
	var mesh := st.commit()
	var skin := Skin.new()
	for b in skel.get_bone_count():
		skin.add_named_bind(skel.get_bone_name(b), skel.get_bone_global_pose(b).affine_inverse())
	var out := MeshInstance3D.new()
	out.name = "scalona_postac"
	out.mesh = mesh
	out.skin = skin
	out.material_override = material
	skel.add_child(out)
	out.skeleton = out.get_path_to(skel)
	return count

# Budynek: wszystko w jedną statyczną siatkę.
static func merge_static(root: Node3D, material: Material) -> int:
	var parts: Array = []
	collect(root, parts)
	var st := SurfaceTool.new()
	st.begin(Mesh.PRIMITIVE_TRIANGLES)
	var to_root := root.global_transform.affine_inverse()
	var count := 0
	for mi: MeshInstance3D in parts:
		var xf := to_root * mi.global_transform
		for s in mi.mesh.get_surface_count():
			var arr := mi.mesh.surface_get_arrays(s)
			var col := mat_color(mi.get_active_material(s))
			var e := mat_emission(mi.get_active_material(s))
			if e.get_luminance() > 0.05: col = Color(e.r, e.g, e.b, 0.0)   # alfa 0 = świeci (okna, płomienie)
			var verts: PackedVector3Array = arr[Mesh.ARRAY_VERTEX]
			var norms: PackedVector3Array = arr[Mesh.ARRAY_NORMAL]
			var idx = arr[Mesh.ARRAY_INDEX]
			var order: PackedInt32Array = idx if idx != null and idx.size() > 0 else PackedInt32Array(range(verts.size()))
			for i in order:
				st.set_color(col)
				st.set_normal((xf.basis * norms[i]).normalized() if norms.size() > i else Vector3.UP)
				st.add_vertex(xf * verts[i])
		mi.queue_free()
		count += 1
	var out := MeshInstance3D.new()
	out.name = "scalony_budynek"
	out.mesh = st.commit()
	out.material_override = material
	root.add_child(out)
	return count
