#version 450 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aCol;

layout (location = 0) out vec3 oCol;

uniform mat4 viewproj;
uniform mat4 model;

out gl_PerVertex
{
    vec4 gl_Position;
};

void main()
{
    oCol = aCol;
    gl_Position = viewproj * model * vec4(aPos, 1.0);
}
