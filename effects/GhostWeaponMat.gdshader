shader_type spatial;
render_mode depth_draw_always;

instance uniform float FADE : hint_range(0.0, 1.05) = 0.0;
instance uniform vec3 VELOCITY = vec3(0.0, 0.0, 0.0);

uniform vec4 FADE_COLOR : source_color;
uniform sampler2D ALBEDO_SAMPLER;
uniform sampler2D FADE_NOISE;

void vertex() {
	VERTEX.z *= min(3.0, length(VELOCITY) + 1.0);
}

void fragment() {
	ALBEDO = texture(ALBEDO_SAMPLER, UV).xyz;
	vec3 world = (INV_VIEW_MATRIX * vec4(VERTEX, 1.0)).xyz;
	vec3 pos = (inverse(MODEL_MATRIX) * vec4((world), 1.0)).xyz;
	float fadeNoiseValue = texture(FADE_NOISE, (pos.xy + pos.z) * 30.0).r;
	if (fadeNoiseValue + FADE > 1.05)
		ALPHA = 0.0;
	
	vec3 glowValue = max(0.0, (fadeNoiseValue + FADE - 1.00) * 50.0) * FADE_COLOR.rgb;
	ALBEDO += glowValue;
	EMISSION = glowValue * 2.0;
}
