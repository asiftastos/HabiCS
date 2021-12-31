using System;
using OpenTK.Mathematics;

namespace HabiCS
{
    class Camera
    {
        public enum BehaviorType
        {
            FIRST_PERSON,
            SPECTATOR,
            FLIGHT,
            ORBIT
        }

        private Vector3 position;
        private Vector3 savedPosition;
        private Vector3 target;

        private Vector3 right;
        private Vector3 up;
        private Vector3 forward;
        private Vector3 viewDir;
        private Vector3 targetYAxes;

        private Quaternion orientation;
        private Quaternion savedOrientation;

        private float yaw;
        private float pitch;
        private float roll;
        private float orbitMinZoom;
        private float orbitMaxZoom;
        private float orbitOffsetDistance;
        private float fpsYOffset;

        private Matrix4 view;

        private BehaviorType behavior;

        public Vector3 Position { get { return position; } set { position = value; UpdateView(); } }
        public Vector3 Target { get { return target; } set { target = value; } }

        public Vector3 Right { get { return right; } set { right = value; } }
        public Vector3 Up { get { return up; } set { up = value; } }
        public Vector3 Forward { get { return forward; } set { forward = value; } }
        public Quaternion Orientation { get { return orientation; } set { orientation = value; } }
        public Matrix4 View { get { return view; } }

        public BehaviorType Behavior { get { return behavior; } set { UpdateBehavior(value); } }


        public float Yaw { get { return yaw; } set { yaw = value; } }
        public float Pitch { get { return pitch; } set { pitch = value; } }
        public float Roll { get { return roll; } set { roll = value; } }
        public float OrbitMinZoom { get { return orbitMinZoom; } set { orbitMinZoom = value; } }
        public float OrbitMaxZoom { get { return orbitMaxZoom; } set { orbitMaxZoom = value; } }
        public float OrbitOffsetDistance { get { return orbitOffsetDistance; } set { orbitOffsetDistance = value; } }
        public float FPSYOffset { get { return fpsYOffset; } set { fpsYOffset = value; } }



        public Camera(Vector3 pos, Vector3 target)
        {
            this.behavior = BehaviorType.SPECTATOR;
            
            this.position = pos;
            this.target = target;
            this.savedPosition = new Vector3(0.0f, 0.0f, 0.0f);
            this.targetYAxes = Vector3.UnitY;
            
            this.pitch = 0;
            this.roll = 0;
            this.yaw = 0;

            this.orbitMinZoom = 1.5f;
            this.orbitMaxZoom = 100.0f;
            this.orbitOffsetDistance = 50.0f;

            this.forward = Vector3.UnitZ;
            this.right = Vector3.UnitX;
            this.up = Vector3.UnitY;
            this.targetYAxes = Vector3.UnitY;
            this.viewDir = -Vector3.UnitY;

            this.orientation = Quaternion.Identity;
            this.savedOrientation = Quaternion.Identity;
            this.view = Matrix4.Identity;
        }

        public void LookAt(Vector3 at)
        {
            LookAt(position, at, Up);
        }

        public void LookAt(Vector3 eye, Vector3 at, Vector3 Yup)
        {
            position = eye;
            target = at;

            forward = Vector3.Normalize(eye - at);
            viewDir = -forward;
            
            right = Vector3.Normalize(Vector3.Cross(Yup, forward));

            up = Vector3.Normalize(Vector3.Cross(forward, right));

            view.M11 = right.X;
            view.M21 = right.Y;
            view.M31 = right.Z;
            view.M41 = -Vector3.Dot(right, eye);

            view.M12 = up.X;
            view.M22 = up.Y;
            view.M32 = up.Z;
            view.M42 = -Vector3.Dot(up, eye);

            view.M13 = forward.X;
            view.M23 = forward.Y;
            view.M33 = forward.Z;
            view.M43 = -Vector3.Dot(forward, eye);

            Pitch = MathHelper.RadiansToDegrees(view.M23);

            orientation = view.ExtractRotation();

            UpdateView();
        }

        public void Update()
        {
            UpdateView();
        }
        
        public void Zoom(float zoom, float minzoom, float maxzoom)
        {
            if (behavior == BehaviorType.ORBIT)
            {
                OrbitMaxZoom = maxzoom;
                OrbitMinZoom = minzoom;

                Vector3 offset = position - target;

                orbitOffsetDistance = offset.Length;
                offset.Normalize();
                orbitOffsetDistance += zoom;
                orbitOffsetDistance = MathHelper.Min(MathHelper.Max(orbitOffsetDistance, minzoom), maxzoom);

                offset *= orbitOffsetDistance;
                position = offset + target;
            }
        }

        public void Move(Vector3 direction, float speed)
        {
            if (behavior == BehaviorType.ORBIT)
            {
                target += direction * speed;
                return;
            }

            position += direction * speed;
        }

        public void Rotate(float xfactor, float yfactor)
        {
            switch(behavior)
            {
                case BehaviorType.ORBIT:
                    RotateOrbit(xfactor, yfactor);
                    break;
                case BehaviorType.FLIGHT:
                    RotateFlight(xfactor, yfactor);
                    break;
                case BehaviorType.FIRST_PERSON:
                case BehaviorType.SPECTATOR:
                    RotateFPS(xfactor, yfactor);
                    break;
                default:
                    break;
            }
        }
        private void UpdateBehavior(BehaviorType type)
        {
            if(behavior == type)
                return;

            BehaviorType prev = behavior;
            behavior = type;

            switch(type)
            {
                case BehaviorType.FIRST_PERSON:
                    switch(prev)
                    {
                        case BehaviorType.FLIGHT:
                            position.Y = fpsYOffset;
                            UpdateView();
                            break;
                        case BehaviorType.SPECTATOR:
                            position.Y = fpsYOffset;
                            UpdateView();
                            break;
                        case BehaviorType.ORBIT:
                            position.Y = fpsYOffset;
                            position.X = savedPosition.X;
                            position.Y = savedPosition.Y;
                            orientation = savedOrientation;
                            UpdateView();
                            break;
                    }
                    UndoRoll();
                    break;
                case BehaviorType.SPECTATOR:
                    switch(prev)
                    {
                        case BehaviorType.FLIGHT:
                            UpdateView();
                            break;
                        case BehaviorType.ORBIT:
                            position = savedPosition;
                            orientation = savedOrientation;
                            UpdateView();
                            break;
                    }
                    UndoRoll();
                    break;
                case BehaviorType.FLIGHT:
                    if(prev == BehaviorType.ORBIT)
                    {
                        position = savedPosition;
                        orientation = savedOrientation;
                        UpdateView();
                    }
                    else
                    {
                        savedPosition = position;
                        UpdateView();
                    }
                    break;
                case BehaviorType.ORBIT:
                    if(prev == BehaviorType.FIRST_PERSON)
                        fpsYOffset = position.Y;

                    savedPosition = position;
                    savedOrientation = orientation;

                    targetYAxes = up;

                    Vector3 eye = position + (forward * orbitOffsetDistance);
                    Vector3 at = position;
                    LookAt(eye, at, targetYAxes);
                    break;
            }
        }

        private void UndoRoll()
        {
            if(Behavior == BehaviorType.ORBIT)
            {
                LookAt(position, target, targetYAxes);
            }
            else
            {
                LookAt(position, position + viewDir, Vector3.UnitY);
            }
        }

        private void UpdateView()
        {
            view = Matrix4.CreateFromQuaternion(orientation);
            right = new Vector3(view.M11, view.M21, view.M31);
            up = new Vector3(view.M12, view.M22, view.M32);
            forward = new Vector3(view.M13, view.M23, view.M33);

            if(behavior == BehaviorType.ORBIT)
            {
                position = target + (Forward * orbitOffsetDistance);
            }

            view.M41 = -Vector3.Dot(right, position);
            view.M42 = -Vector3.Dot(up, position);
            view.M43 = -Vector3.Dot(forward, position);
        }

        private void RotateOrbit(float xfactor, float yfactor)
        {
            Quaternion rot = Quaternion.FromAxisAngle(targetYAxes, xfactor);
            orientation = rot * orientation;
            rot = Quaternion.FromAxisAngle(Vector3.UnitX, yfactor);
            orientation = orientation * rot;
        }

        private void RotateFlight(float xFactor, float yFactor)
        {
            Yaw += xFactor;
            Pitch += yFactor;

            Quaternion rot = Quaternion.FromEulerAngles(Yaw, Pitch, Roll);
            orientation *= rot;
        }

        private void RotateFPS(float xFactor, float yFactor)
        {
            Yaw += xFactor;

            Pitch += yFactor;

            if (Pitch > 90.0f)
                Pitch = 89.0f;
            if (Pitch < -90.0f)
                Pitch = -89.0f;

            Quaternion rot = Quaternion.FromAxisAngle(Vector3.UnitY, Yaw);
            orientation = rot * orientation;
            rot = Quaternion.FromAxisAngle(Vector3.UnitX, Pitch);
            orientation = orientation * rot;
        }
    }
}
