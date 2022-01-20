using OpenTK.Mathematics;

namespace Lighting
{
    public struct Material
    {
        public Vector3 ambient;
        public Vector3 diffuse;
        public Vector3 specular;
        public float shininess;

        public Material(Vector3 ambient, Vector3 diffuse, Vector3 specular, float shininess)
        {
            this.ambient = ambient;
            this.diffuse = diffuse;
            this.specular = specular;
            this.shininess = shininess;
        }
    }
}