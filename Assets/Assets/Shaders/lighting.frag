#version 330 core

in vec3 fNorm;
in vec3 fPos;

out vec4 FragColor;
  
uniform vec4 objectColor;
uniform vec4 lightColor;
uniform vec3 lightPos;

uniform float ambientStrenth;

void main()
{
    vec3 norm = normalize(fNorm);
    vec3 lightDir = normalize(lightPos - fPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec4 diffuse = diff * lightColor;

    vec4 ambient = ambientStrenth * lightColor;

    FragColor = (ambient + diffuse) * objectColor;
}
