shader_type spatial;
render_mode unshaded;

void vertex() {
	float density = INSTANCE_CUSTOM.x;
	COLOR.r = density;
	
	// Rotates the quad so that it always faces the camera
	MODELVIEW_MATRIX = VIEW_MATRIX * mat4(
        INV_VIEW_MATRIX[0],
        INV_VIEW_MATRIX[1],
        INV_VIEW_MATRIX[2],
        MODEL_MATRIX[3]
    );
    MODELVIEW_NORMAL_MATRIX = mat3(MODELVIEW_MATRIX);
}

void fragment() {
	float density = COLOR.r;
	ALBEDO = vec3(density);
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}