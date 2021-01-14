#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 aTex;

out vec2 oTex;

uniform mat4 scale;
uniform mat4 projTrans;

void main()
{
    gl_Position = projTrans * scale * vec4(aPos, 1.0);
	oTex = aTex;
}
