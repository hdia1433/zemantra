shader_type spatial;
render_mode depth_test_disabled;

uniform sampler2D spriteTexture;

uniform sampler2D depthTexture: hint_depth_texture;
uniform vec4 shadowColour: source_color = vec4(.25, .25, .25, .9);

void vertex() 
{
	MODELVIEW_MATRIX = VIEW_MATRIX * mat4(
		INV_VIEW_MATRIX[0],
		MODEL_MATRIX[1],
		INV_VIEW_MATRIX[2],
		MODEL_MATRIX[3]
	);
}

void fragment() {
	vec4 texColour = texture(spriteTexture, UV);
	
	if(texColour.a < .01)
	{
		discard;
	}
	
	vec2 screenUv = SCREEN_UV;
	
	float sceneDepth = texture(depthTexture, screenUv).r;
	
	float myDepth = FRAGCOORD.z;
	
	if (myDepth >= sceneDepth)
	{
		discard;
	}
	
	ALBEDO = shadowColour.rgb;
	ALPHA = texColour.a * shadowColour.a;
}
