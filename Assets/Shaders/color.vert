#version 430 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aCol;

out vec3 oCol;

uniform mat4 viewproj;
uniform mat4 model;

void main()
{
    oCol = aCol;
    gl_Position = viewproj * model * vec4(aPos, 1.0);
}
