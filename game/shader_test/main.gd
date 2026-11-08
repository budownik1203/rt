# Test shadera pixel art Latarni: scena -> SubViewport 640x360 -> toon + kontur + Mgła + paleta -> x3 nearest.
# Uruchom: godot --rendering-driver opengl3 -- still     (zrzuty: pełny efekt i warianty bez części efektów)
#          godot ... -- rotate snap|nosnap               (sekwencja obrotu kamery)
extends Node

const OUT := "res://out/"
var vp: SubViewport
var cam: Camera3D
var post_mat: ShaderMaterial
var target := Vector3(0, 0.9, 0)
var yaw := -30.0
var pitch_from_vertical := 52.0
var ortho := 15.0
var snap := true
var mode := "view"   # bez argumentów: podgląd interaktywny (Q/E obrót, kółko myszy przybliżenie, 1-4 efekty)
var frame := 0
var shots: Array = []
var anim_players: Array = []
var hud: Label

func _ready() -> void:
	var args := OS.get_cmdline_user_args()
	if args.size() > 0: mode = args[0]
	if args.size() > 1: snap = args[1] != "nosnap"
	DirAccess.make_dir_recursive_absolute(ProjectSettings.globalize_path(OUT))
	get_window().size = Vector2i(1920, 1080)
	var cont := SubViewportContainer.new()
	cont.stretch = true
	cont.stretch_shrink = 3
	cont.texture_filter = CanvasItem.TEXTURE_FILTER_NEAREST
	cont.size = Vector2(1920, 1080)
	add_child(cont)
	vp = SubViewport.new()
	vp.msaa_3d = Viewport.MSAA_DISABLED
	vp.screen_space_aa = Viewport.SCREEN_SPACE_AA_DISABLED
	cont.add_child(vp)
	build_world()
	build_camera()
	if mode == "view":
		hud = Label.new(); hud.position = Vector2(12, 8); hud.add_theme_font_size_override("font_size", 22); add_child(hud)
	if mode == "still":
		shots = [["pelny", true, true, true, 15.0], ["bez_palety", false, true, true, 15.0], ["surowy", false, false, false, 15.0], ["zblizenie", true, true, true, 7.0]]

func build_world() -> void:
	var env := WorldEnvironment.new()
	var e := Environment.new()
	e.background_mode = Environment.BG_COLOR
	e.background_color = Color(0.10, 0.09, 0.13)
	e.ambient_light_source = Environment.AMBIENT_SOURCE_COLOR
	e.ambient_light_color = Color(0.20, 0.20, 0.27)
	e.ambient_light_energy = 0.4
	e.tonemap_mode = Environment.TONE_MAPPER_LINEAR
	env.environment = e
	vp.add_child(env)
	# ziemia
	var g := MeshInstance3D.new()
	var pm := PlaneMesh.new(); pm.size = Vector2(60, 60)
	g.mesh = pm
	var gm := toon(Color(0.26, 0.30, 0.16))
	var nt := NoiseTexture2D.new(); nt.seamless = true; nt.width = 256; nt.height = 256
	var fn := FastNoiseLite.new(); fn.frequency = 0.02; nt.noise = fn
	gm.set_shader_parameter("noise_tex", nt); gm.set_shader_parameter("noise_amt", 1.0)
	g.material_override = gm
	vp.add_child(g)
	# budynki i postacie
	place("res://models/szalas.glb", Vector3(-4.5, 0, -3.0), 20.0, false)
	place("res://models/kuchnia.glb", Vector3(4.0, 0, -4.5), -10.0, false)
	place("res://models/wieza_straznicza.glb", Vector3(-6.5, 0, 3.5), 0.0, false)
	place("res://models/palisada_i_mur.glb", Vector3(5.5, 0, 4.0), 90.0, false)
	place("res://models/lampa_pomocnicza.glb", Vector3(0.5, 0, -1.5), 0.0, false)
	place("res://models/zolnierz_karabin.glb", Vector3(0.0, 0, 1.2), 200.0, true, "shoot")
	place("res://models/zolnierz_karabin.glb", Vector3(-1.6, 0, 0.4), 160.0, true, "walk")
	place("res://models/ogar.glb", Vector3(2.6, 0, 3.4), 220.0, true, "gallop")
	# światło latarni (ciepłe, z cieniem), księżyc (chłodny, słaby), wypełnienie i kontur tylko na postaciach (warstwa 2)
	var lamp := OmniLight3D.new()
	lamp.position = Vector3(0.5, 4.2, -1.5)
	lamp.light_color = Color(1.0, 0.78, 0.45)
	lamp.light_energy = 6.0
	lamp.omni_range = 16.0
	lamp.omni_attenuation = 1.2
	lamp.shadow_enabled = true
	vp.add_child(lamp)
	var moon := DirectionalLight3D.new()
	moon.rotation_degrees = Vector3(-55, 130, 0)
	moon.light_color = Color(0.55, 0.6, 0.9)
	moon.light_energy = 0.35
	moon.shadow_enabled = true
	vp.add_child(moon)
	var fill := DirectionalLight3D.new()
	fill.rotation_degrees = Vector3(-35, -40, 0)
	fill.light_color = Color(1.0, 0.85, 0.65)
	fill.light_energy = 0.45
	fill.light_cull_mask = 2
	vp.add_child(fill)
	var rim := DirectionalLight3D.new()
	rim.rotation_degrees = Vector3(-20, 150, 0)
	rim.light_color = Color(0.5, 0.65, 1.0)
	rim.light_energy = 0.6
	rim.light_cull_mask = 2
	vp.add_child(rim)

func place(path: String, pos: Vector3, rot_y: float, is_char: bool, anim := "") -> void:
	var inst: Node3D = load(path).instantiate()
	inst.position = pos
	inst.rotation_degrees.y = rot_y
	vp.add_child(inst)
	convert_materials(inst, is_char)
	if path.contains("lampa"): no_shadow(inst)   # lampa nie zasłania własnego światła (jak visible_shadow=False w Blenderze)
	if anim != "":
		var ap := find_anim(inst)
		if ap:
			var names := ap.get_animation_list()
			var pick := anim if ap.has_animation(anim) else (names[0] if names.size() > 0 else "")
			if pick != "":
				ap.get_animation(pick).loop_mode = Animation.LOOP_LINEAR
				ap.play(pick)
				anim_players.append(ap)
			print(path, " animacje: ", names, " -> ", pick)

func no_shadow(n: Node) -> void:
	if n is GeometryInstance3D: (n as GeometryInstance3D).cast_shadow = GeometryInstance3D.SHADOW_CASTING_SETTING_OFF
	for c in n.get_children(): no_shadow(c)

func find_anim(n: Node) -> AnimationPlayer:
	if n is AnimationPlayer: return n
	for c in n.get_children():
		var r := find_anim(c)
		if r: return r
	return null

func toon(c: Color, tex: Texture2D = null, emis := Color.BLACK) -> ShaderMaterial:
	var m := ShaderMaterial.new()
	m.shader = load("res://shaders/toon.gdshader")
	m.set_shader_parameter("albedo", c)
	m.set_shader_parameter("emission", Vector3(emis.r, emis.g, emis.b))
	if tex:
		m.set_shader_parameter("albedo_tex", tex)
		m.set_shader_parameter("use_tex", true)
	return m

func convert_materials(n: Node, is_char: bool) -> void:
	var low := String(n.name).to_lower()
	if low.contains("flash") or low.contains("maghand") or low.contains("gren"):
		if n is Node3D: (n as Node3D).visible = false   # błysk, magazynek w dłoni i granat włącza silnik zdarzeniami
	if n is MeshInstance3D:
		var mi := n as MeshInstance3D
		if is_char: mi.layers = 1 | 2
		for s in mi.mesh.get_surface_count():
			var src := mi.get_active_material(s)
			var col := Color(0.7, 0.7, 0.7); var tex: Texture2D = null; var em := Color.BLACK
			if src is BaseMaterial3D:
				var b := src as BaseMaterial3D
				col = b.albedo_color; tex = b.albedo_texture
				if b.emission_enabled: em = b.emission * b.emission_energy_multiplier
			mi.set_surface_override_material(s, toon(col, tex, em))
	for c in n.get_children(): convert_materials(c, is_char)

func build_camera() -> void:
	cam = Camera3D.new()
	cam.projection = Camera3D.PROJECTION_ORTHOGONAL
	cam.size = ortho
	cam.near = 0.5
	cam.far = 120.0
	vp.add_child(cam)
	var q := MeshInstance3D.new()
	var qm := QuadMesh.new(); qm.size = Vector2(2, 2)
	q.mesh = qm
	q.extra_cull_margin = 16384.0
	post_mat = ShaderMaterial.new()
	post_mat.shader = load("res://shaders/post.gdshader")
	post_mat.set_shader_parameter("lamp_pos", Vector3(0.5, 0, -1.5))
	q.material_override = post_mat
	cam.add_child(q)
	q.position = Vector3(0, 0, -1)
	update_camera()

func update_camera() -> void:
	var basis := Basis.from_euler(Vector3(deg_to_rad(-(90.0 - pitch_from_vertical)), deg_to_rad(yaw), 0), EULER_ORDER_YXZ)
	var pos := target + basis.z * 40.0
	if snap:   # przyciągnij pozycję kamery do siatki tekseli w jej płaszczyźnie (mniej „pływania” pikseli)
		var texel := ortho / 360.0
		var local := basis.inverse() * pos
		local.x = round(local.x / texel) * texel
		local.y = round(local.y / texel) * texel
		pos = basis * local
	cam.global_transform = Transform3D(basis, pos)

func _unhandled_input(ev: InputEvent) -> void:
	if mode != "view": return
	if ev is InputEventMouseButton and ev.pressed:
		if ev.button_index == MOUSE_BUTTON_WHEEL_UP: ortho = max(5.0, ortho - 1.0)
		if ev.button_index == MOUSE_BUTTON_WHEEL_DOWN: ortho = min(30.0, ortho + 1.0)
		cam.size = ortho; update_camera()
	if ev is InputEventKey and ev.pressed:
		match ev.keycode:
			KEY_1: post_mat.set_shader_parameter("use_palette", not post_mat.get_shader_parameter("use_palette"))
			KEY_2: post_mat.set_shader_parameter("use_outline", not post_mat.get_shader_parameter("use_outline"))
			KEY_3: post_mat.set_shader_parameter("use_fog", not post_mat.get_shader_parameter("use_fog"))
			KEY_4: snap = not snap

func _process(dt: float) -> void:
	frame += 1
	if mode == "view":
		if Input.is_key_pressed(KEY_Q): yaw -= 60.0 * dt; update_camera()
		if Input.is_key_pressed(KEY_E): yaw += 60.0 * dt; update_camera()
		hud.text = "FPS: %d   wywołania rysowania: %d   ortho: %.0f m   [Q/E obrót, kółko: zoom, 1 paleta, 2 kontur, 3 Mgła, 4 snap=%s]" % [Engine.get_frames_per_second(), Performance.get_monitor(Performance.RENDER_TOTAL_DRAW_CALLS_IN_FRAME), ortho, str(snap)]
		return
	if mode == "still":
		if frame < 20: return
		var i := (frame - 20) / 4
		if (frame - 20) % 4 != 0: return
		if i >= shots.size():
			get_tree().quit(); return
		var s: Array = shots[i]
		post_mat.set_shader_parameter("use_palette", s[1])
		post_mat.set_shader_parameter("use_outline", s[2])
		post_mat.set_shader_parameter("use_fog", s[3])
		ortho = s[4]; cam.size = ortho; target = Vector3(0.6, 0.9, 1.8) if ortho < 10.0 else Vector3(0, 0.9, 0); update_camera()
		await RenderingServer.frame_post_draw
		await RenderingServer.frame_post_draw
		get_viewport().get_texture().get_image().save_png(OUT + "%s.png" % s[0])
		print("zapisano ", s[0], " | wywołania rysowania: ", Performance.get_monitor(Performance.RENDER_TOTAL_DRAW_CALLS_IN_FRAME), " | obiekty: ", Performance.get_monitor(Performance.RENDER_TOTAL_OBJECTS_IN_FRAME), " | trójkąty: ", Performance.get_monitor(Performance.RENDER_TOTAL_PRIMITIVES_IN_FRAME))
	elif mode == "rotate":
		if frame < 15: return
		var k := frame - 15
		if k >= 48:
			get_tree().quit(); return
		for ap in anim_players: ap.pause()   # zatrzymane animacje: widać tylko efekt obrotu
		yaw = -30.0 + k * 1.25
		update_camera()
		await RenderingServer.frame_post_draw
		get_viewport().get_texture().get_image().save_png(OUT + "rot_%s_%02d.png" % ["snap" if snap else "nosnap", k])
