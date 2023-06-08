#version 450 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNorm;

uniform mat4 viewproj;
uniform mat4 model;
uniform mat4 invmodel;

out vec3 fNorm;
out vec3 fPos;

void main()
{
    gl_Position = viewproj * model * vec4(aPos, 1.0);
    fNorm = mat3(transpose(invmodel)) * aNorm;
    fPos = vec3(model * vec4(aPos, 1.0));
}
