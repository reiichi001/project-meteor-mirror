using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SharpNav;
using SharpNav.Pathfinding;
using SharpNav.Crowds;
using SharpNav.IO;

namespace FFXIVClassic_Map_Server.utils
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

        public Vector3(SharpNav.Geometry.Vector3 vec)
        {
            X = vec.X;
            Y = vec.Y;
            Z = vec.Z;
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

    class NavmeshUtils
    {

        // navmesh
        public static bool CanSee(float x1, float y1, float z1, float x2, float y2, float z2)
        {

            return false;
        }

        public static SharpNav.TiledNavMesh LoadNavmesh(TiledNavMesh navmesh, string filePath)
        {
            var serialiser = new SharpNav.IO.Json.NavMeshJsonSerializer();
            return serialiser.Deserialize(System.IO.Path.Combine("../../navmesh/", filePath));
            //return navmesh = new SharpNav.IO.Json.NavMeshJsonSerializer().Deserialize(filePath);
        }

        #region sharpnav stuff
        // Copyright (c) 2013-2016 Robert Rouhani <robert.rouhani@gmail.com> and other contributors (see CONTRIBUTORS file).
        // Licensed under the MIT License - https://raw.github.com/Robmaister/SharpNav/master/LICENSE

        public static List<Vector3> GetPath(FFXIVClassic_Map_Server.actors.area.Zone zone, Vector3 startVec, Vector3 endVec, float stepSize = 0.70f, int pathSize = 45, float polyRadius = 0.0f)
        {
            var navMesh = zone.tiledNavMesh;
            var navMeshQuery = zone.navMeshQuery;
            
            // no navmesh loaded, run straight to player
            if (navMesh == null)
            {
                return new List<Vector3>() { endVec };
            }

            // no need to waste cycles finding path to same point
            if (startVec.X == endVec.X && startVec.Y == endVec.Y && startVec.Z == endVec.Z && polyRadius == 0.0f)
            {
                return null;
            }

            // we dont care about distance if picking random point
            float distanceSquared = polyRadius == 0.0f ? FFXIVClassic.Common.Utils.DistanceSquared(startVec.X, startVec.Y, startVec.Z, endVec.X, endVec.Y, endVec.Z) : 100;
            
            // no point pathing if in range
            if (distanceSquared < 4 && Math.Abs(startVec.Y - endVec.Y) < 1.1f)
            {
                return null;
            }

            var smoothPath = new List<Vector3>(pathSize) { };

            NavQueryFilter filter = new NavQueryFilter();

            NavPoint startPt, endPt;

            try
            {
                SharpNav.Geometry.Vector3 c = new SharpNav.Geometry.Vector3(startVec.X, startVec.Y, startVec.Z);
                SharpNav.Geometry.Vector3 ep = new SharpNav.Geometry.Vector3(endVec.X, endVec.Y, endVec.Z);

                SharpNav.Geometry.Vector3 e = new SharpNav.Geometry.Vector3(5, 5, 5);
                navMeshQuery.FindNearestPoly(ref c, ref e, out startPt);
                navMeshQuery.FindNearestPoly(ref ep, ref e, out endPt);
                
                //calculate the overall path, which contains an array of polygon references
                int MAX_POLYS = 256;
                var path = new SharpNav.Pathfinding.Path();

                navMeshQuery.FindPath(ref startPt, ref endPt, filter, path);

                //find a smooth path over the mesh surface
                int npolys = path.Count;
                SharpNav.Geometry.Vector3 iterPos = new SharpNav.Geometry.Vector3();
                SharpNav.Geometry.Vector3 targetPos = new SharpNav.Geometry.Vector3();
                navMeshQuery.ClosestPointOnPoly(startPt.Polygon, startPt.Position, ref iterPos);
                navMeshQuery.ClosestPointOnPoly(path[npolys - 1], endPt.Position, ref targetPos);

                // set target to random point at end of path
                if (polyRadius != 0.0f)
                {
                    var randPoly = navMeshQuery.FindRandomPointAroundCircle(endPt, polyRadius);
                    targetPos = randPoly.Position;
                }

                smoothPath.Add(new Vector3(iterPos));

                //float STEP_SIZE = 0.70f;
                float SLOP = 0.15f;
                while (npolys > 0 && smoothPath.Count < smoothPath.Capacity)
                {
                    //find location to steer towards
                    SharpNav.Geometry.Vector3 steerPos = new SharpNav.Geometry.Vector3();
                    StraightPathFlags steerPosFlag = 0;
                    NavPolyId steerPosRef = NavPolyId.Null;

                    if (!GetSteerTarget(navMeshQuery, iterPos, targetPos, SLOP, path, ref steerPos, ref steerPosFlag, ref steerPosRef))
                        break;

                    bool endOfPath = (steerPosFlag & StraightPathFlags.End) != 0 ? true : false;
                    bool offMeshConnection = (steerPosFlag & StraightPathFlags.OffMeshConnection) != 0 ? true : false;

                    //find movement delta
                    SharpNav.Geometry.Vector3 delta = steerPos - iterPos;
                    float len = (float)Math.Sqrt(SharpNav.Geometry.Vector3.Dot(delta, delta));

                    //if steer target is at end of path or off-mesh link
                    //don't move past location
                    if ((endOfPath || offMeshConnection) && len < stepSize)
                        len = 1;
                    else
                        len = stepSize / len;

                    SharpNav.Geometry.Vector3 moveTgt = new SharpNav.Geometry.Vector3();
                    VMad(ref moveTgt, iterPos, delta, len);

                    //move
                    SharpNav.Geometry.Vector3 result = new SharpNav.Geometry.Vector3();
                    List<NavPolyId> visited = new List<NavPolyId>(pathSize);
                    NavPoint startPoint = new NavPoint(path[0], iterPos);
                    navMeshQuery.MoveAlongSurface(ref startPoint, ref moveTgt, out result, visited);
                    path.FixupCorridor(visited);
                    npolys = path.Count;
                    float h = 0;
                    navMeshQuery.GetPolyHeight(path[0], result, ref h);
                    result.Y = h;
                    iterPos = result;

                    //handle end of path when close enough
                    if (endOfPath && InRange(iterPos, steerPos, SLOP, 1000.0f))
                    {
                        //reached end of path
                        iterPos = targetPos;
                        if (smoothPath.Count < smoothPath.Capacity)
                        {
                            smoothPath.Add(new Vector3(iterPos));
                        }
                        break;
                    }

                    //store results
                    if (smoothPath.Count < smoothPath.Capacity)
                    {
                        smoothPath.Add(new Vector3(iterPos));
                    }
                }
            }
            catch(Exception e)
            {
                Program.Log.Error(e.Message);
                Program.Log.Error("Start pos {0} {1} {2} end pos {3} {4} {5}", startVec.X, startVec.Y, startVec.Z, endVec.X, endVec.Y, endVec.Z);
                // todo: probably log this
                return new List<Vector3>() { endVec };
            }
            return smoothPath;
        }

        /// <summary>
		/// Scaled vector addition
		/// </summary>
		/// <param name="dest">Result</param>
		/// <param name="v1">Vector 1</param>
		/// <param name="v2">Vector 2</param>
		/// <param name="s">Scalar</param>
		private static void VMad(ref SharpNav.Geometry.Vector3 dest, SharpNav.Geometry.Vector3 v1, SharpNav.Geometry.Vector3 v2, float s)
        {
            dest.X = v1.X + v2.X * s;
            dest.Y = v1.Y + v2.Y * s;
            dest.Z = v1.Z + v2.Z * s;
        }

        private static bool GetSteerTarget(NavMeshQuery navMeshQuery, SharpNav.Geometry.Vector3 startPos, SharpNav.Geometry.Vector3 endPos, float minTargetDist, SharpNav.Pathfinding.Path path,
            ref SharpNav.Geometry.Vector3 steerPos, ref StraightPathFlags steerPosFlag, ref NavPolyId steerPosRef)
        {
            StraightPath steerPath = new StraightPath();
            navMeshQuery.FindStraightPath(startPos, endPos, path, steerPath, 0);
            int nsteerPath = steerPath.Count;
            if (nsteerPath == 0)
                return false;

            //find vertex far enough to steer to
            int ns = 0;
            while (ns < nsteerPath)
            {
                if ((steerPath[ns].Flags & StraightPathFlags.OffMeshConnection) != 0 ||
                    !InRange(steerPath[ns].Point.Position, startPos, minTargetDist, 1000.0f))
                    break;

                ns++;
            }

            //failed to find good point to steer to
            if (ns >= nsteerPath)
                return false;

            steerPos = steerPath[ns].Point.Position;
            steerPos.Y = startPos.Y;
            steerPosFlag = steerPath[ns].Flags;
            if (steerPosFlag == StraightPathFlags.None && ns == (nsteerPath - 1))
                steerPosFlag = StraightPathFlags.End; // otherwise seeks path infinitely!!!
            steerPosRef = steerPath[ns].Point.Polygon;

            return true;
        }

        private static bool InRange(SharpNav.Geometry.Vector3 v1, SharpNav.Geometry.Vector3 v2, float r, float h)
        {
            float dx = v2.X - v1.X;
            float dy = v2.Y - v1.Y;
            float dz = v2.Z - v1.Z;
            return (dx * dx + dz * dz) < (r * r) && Math.Abs(dy) < h;
        }
        #endregion

        public static Vector3 GamePosToNavmeshPos(float x, float y, float z)
        {
            return new Vector3(x, -z, y);
        }

        public static Vector3 NavmeshPosToGamePos(float x, float y, float z)
        {
            return new Vector3(x, z, -y);
        }
        
    }
}
