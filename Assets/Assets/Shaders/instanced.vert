#version 330 core

layout (location = 0) in vec2 aPos;
layout (location = 1) in vec4 aColor;
layout (location = 2) in vec2 aOffset;

uniform mat4 ortho;

out vec4 fColor;

void main()
{
    gl_Position = ortho * vec4(aPos + aOffset, -1.0, 1.0);
    fColor = aColor;
}
