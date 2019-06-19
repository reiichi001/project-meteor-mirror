/*
===========================================================================
Copyright (C) 2015-2019 Project Meteor Dev Team

This file is part of Project Meteor Server.

Project Meteor Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Project Meteor Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with Project Meteor Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/

using System;

namespace Meteor.Common
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

        public static Vector3 operator /(Vector3 lhs, float scalar)
        {
            return new Vector3(lhs.X / scalar, lhs.Y / scalar, lhs.Z / scalar);
        }

        public static bool operator !=(Vector3 lhs, Vector3 rhs)
        {
            return !(lhs?.X == rhs?.X && lhs?.Y == rhs?.Y && lhs?.Z == rhs?.Z);
        }

        public static bool operator ==(Vector3 lhs, Vector3 rhs)
        {
            return (lhs?.X == rhs?.X && lhs?.Y == rhs?.Y && lhs?.Z == rhs?.Z);
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

        public static float GetAngle(Vector3 lhs, Vector3 rhs)
        {
            return GetAngle(lhs.X, lhs.Z, rhs.X, rhs.Z); 
        }

        public static float GetAngle(float x, float z, float x2, float z2)
        {
            if (x == x2)
                return 0.0f;

            var angle = (float)(Math.Atan((z2 - z) / (x2 - x)));
            return (float)(x > x2 ? angle + Math.PI : angle);
        }

        public Vector3 NewHorizontalVector(float angle, float extents)
        {
            var newVec = new Vector3();
            newVec.Y = this.Y;
            newVec.X = this.X + (float)Math.Cos(angle) * extents;
            newVec.Z = this.Z + (float)Math.Sin(angle) * extents;

            return newVec;
        }

        public bool IsWithinCircle(Vector3 center, float maxRadius, float minRadius)
        {
            if (this.X == center.X && this.Z == center.Z)
                return true;

            float diffX = center.X - this.X;
            float diffZ = center.Z - this.Z;

            float distance = Utils.XZDistance(center.X, center.Z, X, Z);

            return distance <= maxRadius && distance >= minRadius;
        }

        public bool IsWithinBox(Vector3 upperLeftCorner, Vector3 lowerRightCorner)
        {
            return upperLeftCorner.X <= this.X &&
                upperLeftCorner.Y <= this.Y &&
                upperLeftCorner.Z <= this.Z &&
                lowerRightCorner.X >= this.X &&
                lowerRightCorner.Y >= this.Y &&
                lowerRightCorner.Z >= this.Z;
        }

        //Checks if this vector is in a cone, note it doesn't check for distance
        public bool IsWithinCone(Vector3 coneCenter, float coneRotation, float coneAngle)
        {
            float angleToTarget = GetAngle(coneCenter, this);
            float halfAngleOfAoe = (float) (coneAngle * Math.PI / 2);
            float rotationToAdd = coneRotation + halfAngleOfAoe;

            //This is the angle relative to the lower angle of the cone
            angleToTarget = (angleToTarget + rotationToAdd - (0.5f * (float)Math.PI)) % (2 * (float) Math.PI);

            //If the relative angle is less than the total angle of the cone, the target is inside the cone
            return angleToTarget >= 0 && angleToTarget <= (coneAngle * Math.PI);
        }

        public override bool Equals(object obj)
        {
            var vector = obj as Vector3;
            return vector != null &&
                   X == vector.X &&
                   Y == vector.Y &&
                   Z == vector.Z;
        }

        public override int GetHashCode()
        {
            var hashCode = -307843816;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Z.GetHashCode();
            return hashCode;
        }
    }
}
