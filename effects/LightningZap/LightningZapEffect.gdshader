shader_type spatial;
render_mode blend_add;

instance uniform float FADE = 1.0;
instance uniform float LENGTH = 0.0;
instance uniform float PROGRESS = 0.0;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	ALBEDO = vec3(1.0, 1.0, 1.0);
	EMISSION = vec3(0.01, 0.03, 1.0) * 32.0 * FADE;
	
	vec3 world = (INV_VIEW_MATRIX * vec4(VERTEX, 1.0)).xyz;
	vec3 pos = (inverse(MODEL_MATRIX) * vec4((world), 1.0)).xyz;
	float dist = distance(pos, vec3(0.0, 0.0, 0.0));
	float currentCenter = LENGTH * PROGRESS;
	if (dist > currentCenter)
		ALPHA = 1.0 - min(1.0, abs(dist - currentCenter) / 1.0);
	else
		ALPHA = 0.25 - min(0.25, abs(dist - currentCenter) / 512.0);
	ALPHA *= FADE;
	
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
