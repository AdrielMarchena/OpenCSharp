#version 330 core

layout(location = 0) out vec4 o_Color;

in vec3 v_Color;
in vec2 v_TexCoord;
in float v_TexIndex;

uniform sampler2D u_Textures[16];

void main()
{
	int index = int(v_TexIndex);
	vec4 sampled = vec4(1.0, 1.0, 1.0, texture(u_Textures[index], v_TexCoord).r);
	o_Color = vec4(v_Color, 1.0) * sampled;
}