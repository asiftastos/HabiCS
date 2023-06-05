using System;
using OpenTK.Mathematics;

namespace LGL.Utils
{
    public class OrbitCamera
    {
        private Matrix4 _view;
        private Vector3 _position;
        private Vector3 _target;
        private Vector3 _direction;
        private Vector3 _right;
        private Vector3 _up;

        private float _movSpeed = 15.5f;
        private float _rotSpeed = 2.5f;
        private float _radius = 22.0f;

        public Matrix4 View { get { return _view; } }

        public float Radius { get { return _radius; } set { _radius = value; } }

        public double Yaw { get; set; }

        public Vector3 Right { get { return _right; } }

        public Vector3 Up { get { return _up; } }

        public Vector3 Direction { get { return _direction; } }

        public Vector3 Forward { get { return Vector3.Normalize(Vector3.Cross(_right, Vector3.UnitY)); } }

        public Vector3 Position { get { return _position; } }

        public OrbitCamera(Vector3 pos, Vector3 target)
        {
            _position = pos;
            _target = target;

            RecalculateDirections();

            Yaw = 0.0f;

            _view = Matrix4.LookAt(_position, _target, _up);
        }

        public void Rotate(double factor)
        {
            Yaw += factor * _rotSpeed;
        }

        public void Move(double factor, Vector3 direction)
        {
            _target += direction * (float)factor * _movSpeed;
        }

        public void Update(double time)
        {
            _position.X = (float)MathHelper.Sin(Yaw) * _radius + _target.X;
            _position.Z = (float)MathHelper.Cos(Yaw) * _radius + _target.Z;
            RecalculateDirections();
            _view = Matrix4.LookAt(_position, _target, Vector3.UnitY);
        }

        private void RecalculateDirections()
        {
            _direction = Vector3.Normalize(_position - _target);
            _right = Vector3.Normalize(Vector3.Cross(Vector3.UnitY, _direction));
            _up = Vector3.Normalize(Vector3.Cross(_direction, _right));
        }
    }
}
