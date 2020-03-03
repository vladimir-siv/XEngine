Skybox Shader

#pragma shader vertex
	
	#version 430 core
	
	uniform mat4 project;
	uniform mat4 view;
	uniform float scale;

	in layout(location = 0) vec3 in_position;
	
	out vec3 position;
	
	void main(void)
	{
		position = in_position * scale;
		gl_Position = project * view * vec4(position, 1.0f);
		gl_ClipDistance[0] = 0.0f;
	}

#pragma shader fragment

	#version 430 core
	
	const float LOWER_FOG_L = +00.0f;
	const float UPPER_FOG_L = +30.0f;
	
	uniform float transition;
	uniform vec4 sky1_color;
	uniform samplerCube sky1_map;
	uniform vec4 sky2_color;
	uniform samplerCube sky2_map;
	
	in vec3 position;
	
	out vec4 out_color;
	
	void main(void)
	{
		float transition_f = clamp(transition, 0.0f, 1.0f);
		vec4 fog_color = mix(sky1_color, sky2_color, transition_f);
		vec4 sky_color = mix(texture(sky1_map, position), texture(sky2_map, position), transition_f);
		float fog_factor = clamp((position.y - LOWER_FOG_L) / (UPPER_FOG_L - LOWER_FOG_L), 0.0f, 1.0f);
		out_color = mix(fog_color, sky_color, fog_factor);
	}
