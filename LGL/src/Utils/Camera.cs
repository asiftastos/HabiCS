using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace LGL.Utils
{
    public class Camera
    {
        private Vector3 _position;
        private Vector3 _target;
        private Vector3 _up;
        private float _fieldOfView;
        private CameraProjectionType _projectionType;
        private CameraMode _mode;

        public Vector3 Position => _position;

        public Vector3 Target => _target;

        public Vector3 Forward => Vector3.Normalize(_target - _position);

        public Vector3 Up => Vector3.Normalize(_up);

        public Vector3 Right => Vector3.Normalize(Vector3.Cross(Forward, Up));

        public Matrix4 View => Matrix4.LookAt(_position, _target, Up);

        public float FOV { get { return _fieldOfView; } set { _fieldOfView = value; } }

        public CameraProjectionType ProjectionType { get { return _projectionType; } set { _projectionType = value; } }

        public CameraMode Mode { get { return _mode; } set { _mode = value; } }

        public float MoveSpeed { get; set; } = 0.09f;
        
        public float RotationSpeed { get; set; } = 0.03f;

        public float OrbitalSpeed { get; set; } = 0.5f;

        public float MouseSensitivity { get; set; } = 0.003f;

        public Camera()
        {
            this._position = Vector3.Zero;
            this._target = Vector3.Zero;
            this._up = Vector3.UnitY;
            this._fieldOfView = 45.0f;
            this._projectionType = CameraProjectionType.PERSPECTIVE;
            this._mode = CameraMode.FREE;
        }

        public Camera(Vector3 position,
            Vector3 target,
            Vector3 up,
            float fieldOfView = 45.0f, 
            CameraProjectionType projectionType = CameraProjectionType.PERSPECTIVE, 
            CameraMode mode = CameraMode.FREE)
        {
            this._position = position;
            this._target = target;
            this._up = up;
            this._fieldOfView = fieldOfView;
            this._projectionType = projectionType;
            this._mode = mode;
        }

        // moveInWorldPlane: move parallel to x/z plane
        public void MoveForward(float distance, bool moveInWorldPlane)
        {
            Vector3 forward = Forward;

            if (moveInWorldPlane)
            {
                forward.Y = 0.0f;
                forward.Normalize();
            }

            forward *= distance;
            _position += forward;
            _target += forward;
        }

        public void MoveUp(float distance)
        {
            Vector3 up = Up;
            up *= distance;
            _position += up;
            _target += up;
        }

        // moveInWorldPlane: move parallel to x/z plane
        public void MoveRight(float distance, bool moveInWorldPlane)
        {
            Vector3 right = Right;
            if (moveInWorldPlane)
            {
                right.Y = 0.0f;
                right.Normalize();
            }

            right *= distance;
            _position += right;
            _target += right;
        }

        public void MoveToTarget(float delta)
        {
            float distance = Vector3.Distance(_position, _target);

            distance += delta;

            if(distance <= 0.0f)
            {
                distance = 0.001f;
            }

            _position = _target - (Forward * distance);
        }

        //rotates camera looking left/right
        //rotateAroundTarget: if true camera rotates around target (position is moving), else around it's position (target is moving)
        //angle: in radians
        public void Yaw(float angle, bool rotateAroundTarget)
        {
            Vector3 up = Up;

            Vector3 targetPosition = _position - _target;

            if(_mode == CameraMode.FREE || _mode == CameraMode.FIRST)
                angle = -angle;

            Quaternion quat = Quaternion.FromAxisAngle(up, angle);
            targetPosition = Vector3.Transform(targetPosition, quat);

            if(rotateAroundTarget)
            {
                _position = _target + targetPosition;
            }else
            {
                _target = _position - targetPosition;
            }
        }

        /*
         *  rotate looking up/down
         *  lockView: prevents overrotation
         *  rotateAroundTarget: if true camera rotates around target (position is moving), else around it's position (target is moving)
         *  rotateUp: rotate the up vector too (Usefull in FREE mode camera)
         *  angle: in radians
         */
        public void Pitch(float angle, bool lovkView, bool rotateAroundTarget, bool rotateUp)
        {
            Vector3 up = Up;

            Vector3 targetPosition = _position - _target;

            if(lovkView)
            {
                float angleMaxUp = Vector3.CalculateAngle(up, targetPosition);
                angleMaxUp -= 0.001f;
                if(angle > angleMaxUp) { angle = angleMaxUp; }

                float angleMaxDown = Vector3.CalculateAngle(-up, targetPosition);
                angleMaxDown *= -1.0f;
                angleMaxDown += 0.001f;
                if(angle < angleMaxDown) { angle = angleMaxDown; }
            }

            if(_mode == CameraMode.FREE || _mode == CameraMode.FIRST)
                angle = -angle;

            Quaternion quat = Quaternion.FromAxisAngle(Right, angle);
            targetPosition = Vector3.Transform(targetPosition, quat);

            if(rotateAroundTarget)
            {
                _position = _target + targetPosition;
            }else
            {
                _target = _position - targetPosition;
            }

            if(rotateUp)
            {
                _up = Vector3.Transform(up, quat);
            }
        }

        /*
         * rotate around forward ventor, tilt head
         * angle: in radians
         */
        public void Roll(float angle)
        {
            Quaternion quat = Quaternion.FromAxisAngle(Forward, angle);
            _up = Vector3.Transform(_up, quat);
        }

        public Matrix4 Projection(float aspect)
        {
            switch (_projectionType)
            {
                case CameraProjectionType.PERSPECTIVE:
                    return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(_fieldOfView), aspect, 0.1f, 1000.0f);
                case CameraProjectionType.ORTHOGRAPHIC:
                    float top = _fieldOfView / 2.0f;
                    float right = top * aspect;
                    return Matrix4.CreateOrthographicOffCenter(-right, right, -top, top, 0.1f, 1000.0f);
                default:
                    return Matrix4.Identity;
            }
        }

        /*
         * Update provided by the user the movement/rotation/zoom
         * 
         * Required:
         *  movement.x - move forward/backward
         *  movement.y - move left/right
         *  movement.z - up/down
         *  rotation.x - yaw
         *  rotation.y - pitch
         *  rotation.z - roll
         *  zoom - move towards target
         */
        public void UpdatePro(Vector3 movement, Vector3 rotation, float zoom)
        {
            bool lockView = true;
            bool rotateAroundTarget = false;
            bool rotateUp = false;
            bool moveInWorldPlane = true;

            Pitch(MathHelper.DegreesToRadians(-rotation.Y), lockView, rotateAroundTarget, rotateUp);
            Yaw(MathHelper.DegreesToRadians(-rotation.X), rotateAroundTarget);
            Roll(MathHelper.DegreesToRadians(rotation.Z));

            MoveForward(movement.X, moveInWorldPlane);
            MoveRight(movement.Y, moveInWorldPlane);
            MoveUp(movement.Z);

            MoveToTarget(zoom);
        }

        public void Update(GameWindow game)
        {
            switch (_mode)
            {
                case CameraMode.CUSTOM:
                    break;
                case CameraMode.FREE:
                    UpdateRotation(game, false, false, true);
                    UpdateMove(game, false, false, false, true);
                    break;
                case CameraMode.ORBITAL:
                    UpdateOrbital(game);
                    UpdateZoom(game);
                    break;
                case CameraMode.FIRST:
                    UpdateRotation(game, true, false, false);
                    UpdateMove(game, true, false, true, false);
                    break;
                case CameraMode.THIRD:
                    UpdateRotation(game, true, true, false);
                    UpdateMove(game, true, true, true, false);
                    UpdateZoom(game);
                    break;
                default:
                    break;
            }
        }

        private void UpdateOrbital(GameWindow game)
        {
            Matrix4 rot = Matrix4.CreateFromAxisAngle(Up, OrbitalSpeed * (float)game.RenderTime);
            Vector3 targetPosition = _target - _position;
            targetPosition = Vector3.TransformVector(targetPosition, rot);
            _position = _target - targetPosition;
        }

        private void UpdateRotation(GameWindow game, bool lockView, bool rotateAroundTarget, bool rotateUp)
        {
            if (game.IsKeyDown(Keys.Down)) Pitch(RotationSpeed, lockView, rotateAroundTarget, rotateUp);
            if (game.IsKeyDown(Keys.Up)) Pitch(-RotationSpeed, lockView, rotateAroundTarget, rotateUp);
            if (game.IsKeyDown(Keys.Right)) Yaw(RotationSpeed, rotateAroundTarget);
            if(game.IsKeyDown(Keys.Left)) Yaw(-RotationSpeed, rotateAroundTarget);
            if (game.IsKeyDown(Keys.Q)) Roll(-RotationSpeed);
            if (game.IsKeyDown(Keys.E)) Roll(RotationSpeed);
        }

        private void UpdateMove(GameWindow game, bool lockView, bool rotateAroundTarget, bool moveInWorldPlane, bool rotateUp)
        {
            Yaw(game.MouseState.Delta.X * MouseSensitivity, rotateAroundTarget);
            Pitch(game.MouseState.Delta.Y * MouseSensitivity, lockView, rotateAroundTarget, rotateUp);

            if(game.IsKeyDown(Keys.W)) MoveForward(MoveSpeed, moveInWorldPlane);
            if(game.IsKeyDown(Keys.S)) MoveForward(-MoveSpeed, moveInWorldPlane);
            if(game.IsKeyDown(Keys.A)) MoveRight(-MoveSpeed, moveInWorldPlane);
            if(game.IsKeyDown(Keys.D)) MoveRight(MoveSpeed, moveInWorldPlane);
            if(game.IsKeyDown(Keys.Space)) MoveUp(MoveSpeed);
            if(game.IsKeyDown(Keys.LeftControl)) MoveUp(-MoveSpeed);
        }

        private void UpdateZoom(GameWindow game)
        {
            MoveToTarget(-game.MouseState.ScrollDelta.Y);
            if (game.IsKeyPressed(Keys.KeyPadSubtract)) MoveToTarget(2.0f);
            if (game.IsKeyPressed(Keys.KeyPadAdd)) MoveToTarget(-2.0f);
        }

        public enum CameraProjectionType
        {
            PERSPECTIVE,
            ORTHOGRAPHIC
        }

        public enum CameraMode
        {
            CUSTOM,
            FREE,
            ORBITAL,
            FIRST,
            THIRD
        }
    }
}
