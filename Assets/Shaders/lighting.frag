#version 450 core

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
	
	//ambient
	vec3 ambient = material.ambient * light.ambient;

	//diffuse
    vec3 norm = normalize(fNorm);
	vec3 lightDir = normalize(light.position - fPos);
	float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = (material.diffuse * diff) * light.diffuse;
	
	//specular
    vec3 viewDir = normalize(viewPos - fPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = (material.specular * spec) * light.specular;

    
    vec3 result = ambient + diffuse + specular;
    FragColor = vec4(result, 1.0) * objectColor;
}
