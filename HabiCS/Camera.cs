using OpenTK.Mathematics;

namespace HabiCS
{
    class Camera
    {
        private Vector3 position;
        private Vector3 direction;
        private Vector3 right;
        private Vector3 up;

        private float yaw;
        private float pitch;

        private Matrix4 view;

        public Matrix4 View { get { return view; } }

        public Camera(Vector3 pos)
        {
            position = pos;
            pitch = -26.0f;
            yaw = -90.0f;

            UpdateDirection();
            right = Vector3.Cross(Vector3.UnitY, direction);
            right.Normalize();
            up = Vector3.Cross(direction, right);
            up.Normalize();

            UpdateView();
        }

        public void Update()
        {
            UpdateView();
        }

        private void UpdateView()
        {
            view = Matrix4.LookAt(position, position + direction, up);
        }

        private void UpdateDirection()
        {
            float yawRadians = MathHelper.DegreesToRadians(yaw);
            float pitchRadians = MathHelper.DegreesToRadians(pitch);
            direction.X = (float)MathHelper.Cos(yawRadians) * (float)MathHelper.Cos(pitchRadians);
            direction.Y = (float)MathHelper.Sin(pitchRadians);
            direction.Z = (float)MathHelper.Sin(yawRadians) * (float)MathHelper.Cos(pitchRadians);
            direction.Normalize();
        }

        public void MoveForward(float speed)
        {
            position += direction * speed;
        }

        public void MoveBack(float speed)
        {
            position -= direction * speed;
        }

        public void MoveRight(float speed)
        {
            position -= right * speed;
        }

        public void MoveLeft(float speed)
        {
            position += right * speed;
        }

        public void MoveUp(float speed)
        {
            position += Vector3.UnitY * speed;
        }

        public void MoveDown(float speed)
        {
            position -= Vector3.UnitY * speed;
        }

        public void Rotate(float xfactor, float yfactor)
        {
            yaw += xfactor;
            pitch += yfactor;
            UpdateDirection();
            right = Vector3.Cross(Vector3.UnitY, direction);
            right.Normalize();
            up = Vector3.Cross(direction, right);
            up.Normalize();
        }
    }
}
