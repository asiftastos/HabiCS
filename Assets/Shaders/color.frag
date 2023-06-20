#version 430 core

in vec3 oCol;

out vec4 FragColor;

uniform vec4 color;

void main()
{
    FragColor = vec4(oCol, 1.0f) * color;
}
