using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic.Common
{
    public class Vector3
    {
        public float X;
        public float Y;
        public float Z;
        public static Vector3 Zero = new Vector3();

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3()
        {
            X = 0.0f;
            Y = 0.0f;
            Z = 0.0f;
        }

        public static Vector3 operator +(Vector3 lhs, Vector3 rhs)
        {
            Vector3 newVec = new Vector3(lhs.X, lhs.Y, lhs.Z);
            newVec.X += rhs.X;
            newVec.Y += rhs.Y;
            newVec.Z += rhs.Z;
            return newVec;
        }

        public static Vector3 operator -(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);
        }

        public static Vector3 operator *(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(lhs.X * rhs.X, lhs.Y * rhs.Y, lhs.Z * rhs.Z);
        }

        public static Vector3 operator *(float scalar, Vector3 rhs)
        {
            return new Vector3(scalar * rhs.X, scalar * rhs.Y, scalar * rhs.Z);
        }

        public static Vector3 operator /(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);
        }

        public float Length()
        {
            return (float)Math.Sqrt(this.LengthSquared());
        }

        public float LengthSquared()
        {
            return (this.X * this.X) + (this.Y * this.Y) + (this.Z * this.Z);
        }

        public static float Dot(Vector3 lhs, Vector3 rhs)
        {
            return (lhs.X * rhs.X) + (lhs.Y * rhs.Y) + (lhs.Z * rhs.Z);
        }
    }
}
