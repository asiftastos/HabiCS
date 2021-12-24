#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aCol;

uniform mat4 VP;
uniform mat4 M;

out vec3 oCol;

void main()
{
    gl_Position = VP * M * vec4(aPos, 1.0);
    oCol = aCol;
}
