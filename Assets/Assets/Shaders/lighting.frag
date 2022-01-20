#version 330 core

struct Material {
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    float shininess;
};

uniform Material material;

struct Light {
    vec3 position;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

uniform Light light;

in vec3 fNorm;
in vec3 fPos;

out vec4 FragColor;
  
uniform vec4 objectColor;
uniform vec3 viewPos;

void main()
{
    vec3 norm = normalize(fNorm);
    vec3 viewDir = normalize(viewPos - fPos);
    vec3 lightDir = normalize(light.position - fPos);

    vec3 reflectDir = reflect(-lightDir, norm);

    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec4 specular = vec4((material.specular * spec) * light.specular, 1.0);

    float diff = max(dot(norm, lightDir), 0.0);
    vec4 diffuse = vec4((material.diffuse * diff) * light.diffuse, 1.0);

    vec4 ambient = vec4(material.ambient * light.ambient, 1.0);

    FragColor = (ambient + diffuse + specular) * objectColor;
}
