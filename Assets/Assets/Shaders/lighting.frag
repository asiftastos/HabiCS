#version 330 core

const float ambientStrenth = 0.3;
const float diffuseStrength = 0.5;
const float specularStrength = 0.8;
const float shiningness = 32;

in vec3 fNorm;
in vec3 fPos;

out vec4 FragColor;
  
uniform vec4 objectColor;
uniform vec4 lightColor;
uniform vec3 lightPos;
uniform vec3 viewPos;

void main()
{
    vec3 norm = normalize(fNorm);
    vec3 viewDir = normalize(viewPos - fPos);
    vec3 lightDir = normalize(lightPos - fPos);

    vec3 reflectDir = reflect(-lightDir, norm);

    float spec = pow(max(dot(viewDir, reflectDir), 0.0), shiningness);
    vec4 specular = specularStrength * spec * lightColor;

    float diff = max(dot(norm, lightDir), 0.0);
    vec4 diffuse = diffuseStrength * diff * lightColor;

    vec4 ambient = ambientStrenth * lightColor;

    FragColor = (ambient + diffuse + specular) * objectColor;
}
