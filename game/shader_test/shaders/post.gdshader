// Postproces Latarni na pełnoekranowym kwadracie: kontur 1 px z głębi, Mgła z ditheringiem Bayera, paleta.
shader_type spatial;
render_mode unshaded, depth_test_disabled, depth_draw_never, cull_disabled, fog_disabled;
uniform sampler2D screen_tex : hint_screen_texture, filter_nearest;
uniform sampler2D depth_tex : hint_depth_texture, filter_nearest;
uniform bool use_palette = true;
uniform bool use_outline = true;
uniform bool use_fog = true;
uniform vec3 lamp_pos = vec3(0.0);
uniform float fog_r0 = 9.0;
uniform float fog_r1 = 17.0;
uniform vec3 fog_col : source_color = vec3(0.16, 0.15, 0.22);
uniform float edge_thr = 0.18;
const int PAL_N = 48;
const vec3 PAL[48] = vec3[](
vec3(0.056,0.053,0.070),
vec3(0.107,0.101,0.130),
vec3(0.169,0.164,0.200),
vec3(0.261,0.258,0.300),
vec3(0.079,0.120,0.078),
vec3(0.165,0.220,0.132),
vec3(0.294,0.340,0.197),
vec3(0.476,0.480,0.288),
vec3(0.106,0.140,0.098),
vec3(0.220,0.260,0.169),
vec3(0.370,0.400,0.248),
vec3(0.560,0.552,0.364),
vec3(0.140,0.094,0.084),
vec3(0.260,0.185,0.143),
vec3(0.400,0.306,0.220),
vec3(0.580,0.487,0.348),
vec3(0.220,0.110,0.110),
vec3(0.380,0.207,0.152),
vec3(0.560,0.349,0.196),
vec3(0.740,0.577,0.333),
vec3(0.200,0.110,0.121),
vec3(0.360,0.174,0.162),
vec3(0.540,0.296,0.243),
vec3(0.720,0.532,0.396),
vec3(0.300,0.195,0.208),
vec3(0.500,0.336,0.300),
vec3(0.680,0.515,0.422),
vec3(0.840,0.718,0.605),
vec3(0.144,0.148,0.180),
vec3(0.256,0.274,0.320),
vec3(0.394,0.439,0.480),
vec3(0.568,0.632,0.660),
vec3(0.182,0.180,0.200),
vec3(0.346,0.351,0.380),
vec3(0.539,0.556,0.580),
vec3(0.800,0.792,0.720),
vec3(0.420,0.344,0.367),
vec3(0.600,0.504,0.516),
vec3(0.780,0.684,0.663),
vec3(0.920,0.854,0.754),
vec3(0.600,0.432,0.180),
vec3(0.800,0.654,0.280),
vec3(0.940,0.847,0.517),
vec3(1.000,0.976,0.850),
vec3(0.250,0.075,0.096),
vec3(0.420,0.105,0.124),
vec3(0.600,0.168,0.168),
vec3(0.780,0.377,0.351));

vec3 to_srgb(vec3 c){ return mix(c*12.92, 1.055*pow(c, vec3(1.0/2.4)) - 0.055, step(vec3(0.0031308), c)); }
vec3 to_lin(vec3 c){ return mix(c/12.92, pow((c+0.055)/1.055, vec3(2.4)), step(vec3(0.04045), c)); }
void vertex() { POSITION = vec4(VERTEX.xy, 1.0, 1.0); }
vec3 view_pos(vec2 uv, mat4 inv_proj) {
	float d = texture(depth_tex, uv).r;
	vec4 v = inv_proj * vec4(uv * 2.0 - 1.0, d, 1.0);
	return v.xyz / v.w;
}
float bayer4(vec2 p) {
	int x = int(mod(p.x, 4.0)), y = int(mod(p.y, 4.0));
	int m[16] = int[](0,8,2,10,12,4,14,6,3,11,1,9,15,7,13,5);
	return (float(m[y * 4 + x]) + 0.5) / 16.0;
}
void fragment() {
	vec2 px = 1.0 / VIEWPORT_SIZE;
	vec3 col = texture(screen_tex, SCREEN_UV).rgb;
	vec3 vp = view_pos(SCREEN_UV, INV_PROJECTION_MATRIX);
	float dc = -vp.z;
	if (use_fog) {   // Mgła promieniowo od latarni, rozdrobniona ditheringiem tylko w Mgle
		vec3 wp = (INV_VIEW_MATRIX * vec4(vp, 1.0)).xyz;
		float f = smoothstep(fog_r0, fog_r1, length(wp.xz - lamp_pos.xz));
		float fq = floor(f * 4.0 + bayer4(FRAGCOORD.xy)) / 4.0;
		col = mix(col, fog_col, clamp(fq, 0.0, 0.85));
	}
	if (use_outline) {   // kontur po stronie bliższego obiektu
		float m = 0.0;
		vec2 o[4] = vec2[](vec2(px.x,0.0), vec2(-px.x,0.0), vec2(0.0,px.y), vec2(0.0,-px.y));
		for (int i = 0; i < 4; i++) {
			float dn = -view_pos(SCREEN_UV + o[i], INV_PROJECTION_MATRIX).z;
			m = max(m, dn - dc);
		}
		if (m > edge_thr) col = col * vec3(0.30, 0.28, 0.42);
	}
	if (use_palette) {   // paleta jest w sRGB (z renderów Blendera), obraz w Godocie jest liniowy
		vec3 s = to_srgb(clamp(col, 0.0, 1.0));
		float best = 1e9; vec3 bc = s;
		for (int i = 0; i < PAL_N; i++) { vec3 d = PAL[i] - s; d *= vec3(0.30, 0.59, 0.11) * 3.0; float dd = dot(d, d); if (dd < best) { best = dd; bc = PAL[i]; } }
		col = to_lin(bc);
	}
	ALBEDO = col;
}
