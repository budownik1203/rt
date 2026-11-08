// Cel-shading Latarni: pasma światła jak w post.py (Blender), cienie przesunięte w chłodny fiolet.
shader_type spatial;
render_mode diffuse_lambert, specular_disabled;
uniform vec4 albedo : source_color = vec4(1.0);
uniform sampler2D albedo_tex : source_color, filter_nearest;
uniform bool use_tex = false;
uniform sampler2D noise_tex : filter_linear, repeat_enable;
uniform float noise_amt = 0.0;
varying vec3 wpos;
void vertex() { wpos = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz; }
uniform vec3 emission : source_color = vec3(0.0);
uniform vec3 shadow_tint : source_color = vec3(0.72, 0.64, 0.95);
void fragment() {
	vec4 c = albedo;
	if (use_tex) c *= texture(albedo_tex, UV);
	if (noise_amt > 0.0) { float n = texture(noise_tex, wpos.xz * 0.12).r; float n2 = texture(noise_tex, wpos.xz * 0.5).r; c.rgb *= mix(1.0, 0.65 + 0.7 * (n * 0.7 + n2 * 0.3), noise_amt); }
	ALBEDO = c.rgb;
	EMISSION = emission;
}
float band(float x) {   // progi [0,04; 0,12; 0,30; 0,62] -> [0,06; 0,16; 0,36; 0,66; 1,0]
	return x < 0.04 ? 0.06 : (x < 0.12 ? 0.16 : (x < 0.30 ? 0.36 : (x < 0.62 ? 0.66 : 1.0)));
}
void light() {
	float e = max(max(LIGHT_COLOR.r, LIGHT_COLOR.g), LIGHT_COLOR.b) / PI;
	float ndl = clamp(dot(NORMAL, LIGHT), 0.0, 1.0) * ATTENUATION * e;
	float b = band(ndl);
	vec3 tint = mix(shadow_tint, vec3(1.0), smoothstep(0.1, 0.7, b));
	DIFFUSE_LIGHT += b * tint * (LIGHT_COLOR / max(e * PI, 0.001));
}
