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
using System.Collections.Generic;
using Meteor.Map.Actors;
using Meteor.Map.utils;
using Meteor.Common;
using Meteor.Map.actors.area;

// port of https://github.com/DarkstarProject/darkstar/blob/master/src/map/ai/helpers/pathfind.h

namespace Meteor.Map.actors.chara.ai
{
    // todo: check for obstacles, los, etc
    public enum PathFindFlags
    {
        None,
        Scripted = 0x01,
        IgnoreNav = 0x02,
    }
    class PathFind
    {
        private Character owner;
        private List<Vector3> path;
        private bool canFollowPath;
        float distanceFromPoint;

        private PathFindFlags pathFlags;

        public PathFind(Character owner)
        {
            this.owner = owner;
        }

        public void PreparePath(Vector3 dest, float stepSize = 1.25f, int maxPath = 40, float polyRadius = 0.0f)
        {
            PreparePath(dest.X, dest.Y, dest.Z, stepSize, maxPath, polyRadius);
        }

        public void PreparePath(float x, float y, float z, float stepSize = 1.25f, int maxPath = 40, float polyRadius = 0.0f)
        {
            var pos = new Vector3(owner.positionX, owner.positionY, owner.positionZ);
            var dest = new Vector3(x, y, z);

            Zone zone;
            if (owner.GetZone() is PrivateArea || owner.GetZone() is PrivateAreaContent)
                zone = (Zone)((PrivateArea)owner.GetZone()).GetParentZone();
            else
                zone = (Zone)owner.GetZone();

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            if ((pathFlags & PathFindFlags.IgnoreNav) != 0)
                path = new List<Vector3>(1) { new Vector3(x, y, z) };
            else
                path = NavmeshUtils.GetPath(zone, pos, dest, stepSize, maxPath, polyRadius);

            if (path != null)
            {
                if (owner.oldPositionX == 0.0f && owner.oldPositionY == 0.0f && owner.oldPositionZ == 0.0f)
                {
                    owner.oldPositionX = owner.positionX;
                    owner.oldPositionY = owner.positionY;
                    owner.oldPositionZ = owner.positionZ;
                }

                // todo: something went wrong
                if (path.Count == 0)
                {
                    owner.positionX = owner.oldPositionX;
                    owner.positionY = owner.oldPositionY;
                    owner.positionZ = owner.oldPositionZ;
                }

                sw.Stop();
                zone.pathCalls++;
                zone.pathCallTime += sw.ElapsedMilliseconds;

                //if (path.Count == 1)
                //    Program.Log.Info($"mypos: {owner.positionX} {owner.positionY} {owner.positionZ} | targetPos: {x} {y} {z} | step {stepSize} | maxPath {maxPath} | polyRadius {polyRadius}");

                //Program.Log.Error("[{0}][{1}] Created {2} points in {3} milliseconds", owner.actorId, owner.actorName, path.Count, sw.ElapsedMilliseconds);
            }
        }

        public void PathInRange(Vector3 dest, float minRange, float maxRange)
        {
            PathInRange(dest.X, dest.Y, dest.Z, minRange, maxRange);
        }

        public void PathInRange(float x, float y, float z, float minRange, float maxRange = 5.0f)
        {
            var dest = owner.FindRandomPoint(x, y, z, minRange, maxRange);
            // todo: this is dumb..
            distanceFromPoint = owner.GetAttackRange();
            PreparePath(dest.X, dest.Y, dest.Z);
        }


        public void SetPathFlags(PathFindFlags flags)
        {
            this.pathFlags = flags;
        }

        public bool IsFollowingPath()
        {
            return path?.Count > 0;
        }

        public bool IsFollowingScriptedPath()
        {
            return (pathFlags & PathFindFlags.Scripted) != 0;
        }

        public void FollowPath()
        {
            if (path?.Count > 0)
            {
                var point = path[0];

                StepTo(point);

                if (AtPoint(point))
                {
                    path.Remove(point);
                    owner.OnPath(point);
                    //Program.Log.Error($"{owner.actorName} arrived at point {point.X} {point.Y} {point.Z}");
                }

                if (path.Count == 0 && owner.target != null)
                    owner.LookAt(owner.target);
            }
        }

        public bool AtPoint(Vector3 point = null)
        {
            if (point == null && path != null && path.Count > 0)
            {
                point = path[path.Count - 1];
            }
            else
            {
                distanceFromPoint = 0;
                return true;
            }

            if (distanceFromPoint == 0)
                return owner.positionX == point.X && owner.positionZ == point.Z;
            else
                return Utils.Distance(owner.positionX, owner.positionY, owner.positionZ, point.X, point.Y, point.Z) <= (distanceFromPoint + 4.5f);
        }

        public void StepTo(Vector3 point, bool run = false)
        {
            float speed = GetSpeed();

            float stepDistance = speed / 3;
            float distanceTo = Utils.Distance(owner.positionX, owner.positionY, owner.positionZ, point.X, point.Y, point.Z);

            owner.LookAt(point);

            if (distanceTo <= distanceFromPoint + stepDistance)
            {
                if (distanceFromPoint <= owner.GetAttackRange())
                {
                    owner.QueuePositionUpdate(point);
                }
                else
                {
                    float x = owner.positionX - (float)Math.Cos(owner.rotation + (float)(Math.PI / 2)) * (distanceTo - distanceFromPoint);
                    float z = owner.positionZ + (float)Math.Sin(owner.rotation + (float)(Math.PI / 2)) * (distanceTo - distanceFromPoint);

                    owner.QueuePositionUpdate(x, owner.positionY, z);
                }
            }
            else
            {
                float x = owner.positionX - (float)Math.Cos(owner.rotation + (float)(Math.PI / 2)) * (distanceTo - distanceFromPoint);
                float z = owner.positionZ + (float)Math.Sin(owner.rotation + (float)(Math.PI / 2)) * (distanceTo - distanceFromPoint);

                owner.QueuePositionUpdate(x, owner.positionY, z);
            }
        }

        public void Clear()
        {
            path?.Clear();
            pathFlags = PathFindFlags.None;
            distanceFromPoint = 0.0f;
        }

        private float GetSpeed()
        {
            float baseSpeed = owner.GetSpeed();

            if (!(owner is Player))
            {
                if (owner is BattleNpc)
                {
                    //owner.ChangeSpeed(0.0f, SetActorSpeedPacket.DEFAULT_WALK - 2.0f, SetActorSpeedPacket.DEFAULT_RUN - 2.0f, SetActorSpeedPacket.DEFAULT_ACTIVE - 2.0f);
                }
                // baseSpeed += ConfigConstants.NPC_SPEED_MOD;
            }
            return baseSpeed;
        }
    }
}
