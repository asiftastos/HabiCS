#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 aTex;

uniform mat4 ortho;
uniform mat4 model;

out vec2 oTex;

void main()
{
    gl_Position = ortho * model * vec4(aPos, 1.0);
    oTex = aTex;
}
