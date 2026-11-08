// Cel-shading Latarni: pasma światła jak w post.py (Blender), cienie przesunięte w chłodny fiolet.
// use_vcol: kolor z wierzchołków (scalone siatki); alfa wierzchołka 0 = świeci.
shader_type spatial;
render_mode diffuse_lambert, specular_disabled;
uniform vec4 albedo : source_color = vec4(1.0);
uniform sampler2D albedo_tex : source_color, filter_nearest;
uniform bool use_tex = false;
uniform bool use_vcol = false;
uniform vec3 emission : source_color = vec3(0.0);
uniform vec3 shadow_tint : source_color = vec3(0.72, 0.64, 0.95);
uniform sampler2D noise_tex : filter_linear, repeat_enable;
uniform float noise_amt = 0.0;
varying vec3 wpos;
varying float jitter;
vec3 srgb_to_lin(vec3 c){ return mix(c/12.92, pow((c+0.055)/1.055, vec3(2.4)), step(vec3(0.04045), c)); }
void vertex() { wpos = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz; }
void fragment() {
	vec4 c = albedo;
	vec3 em = emission;
	if (use_vcol) {
		vec3 vc = srgb_to_lin(COLOR.rgb);
		if (COLOR.a < 0.5) { em = vc * 2.0; c.rgb = vc; } else { c.rgb *= vc; }
	}
	if (use_tex) c *= texture(albedo_tex, UV);
	jitter = 0.0;
	if (noise_amt > 0.0) {
		float n = texture(noise_tex, wpos.xz * 0.35).r;
		float n2 = texture(noise_tex, wpos.xz * 1.3 + 17.0).r;
		c.rgb *= mix(1.0, 0.75 + 0.5 * (n * 0.6 + n2 * 0.4), noise_amt);
		jitter = (n2 - 0.5) * 0.10 * noise_amt;   // rozmywa równe pierścienie pasm światła
	}
	ALBEDO = c.rgb;
	EMISSION = em;
}
float band(float x) {   // progi [0,04; 0,12; 0,30; 0,62] -> [0,06; 0,16; 0,36; 0,66; 1,0]
	return x < 0.04 ? 0.06 : (x < 0.12 ? 0.16 : (x < 0.30 ? 0.36 : (x < 0.62 ? 0.66 : 1.0)));
}
void light() {
	float e = max(max(LIGHT_COLOR.r, LIGHT_COLOR.g), LIGHT_COLOR.b) / PI;
	float ndl = clamp(dot(NORMAL, LIGHT), 0.0, 1.0) * ATTENUATION * e;
	float b = band(ndl * (1.0 + jitter * 3.0));
	vec3 tint = mix(shadow_tint, vec3(1.0), smoothstep(0.1, 0.7, b));
	DIFFUSE_LIGHT += b * tint * (LIGHT_COLOR / max(e * PI, 0.001));
}
