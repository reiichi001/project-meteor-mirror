using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server;
using FFXIVClassic_Map_Server.utils;
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors.area;

namespace FFXIVClassic_Map_Server.actors.chara.ai
{
    class PathFind
    {
        private Character owner;

        public PathFind(Character owner)
        {
            this.owner = owner;
        }

        // todo: is this class even needed?
        public void PathTo(float x, float y, float z, float stepSize = 0.70f, int maxPath = 40, float polyRadius = 0.0f)
        {
            var pos = new Vector3(owner.positionX, owner.positionY, owner.positionZ);
            var dest = new Vector3(x, y, z);
            var zone = (Zone)owner.GetZone();
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            var path = NavmeshUtils.GetPath(zone, pos, dest, stepSize, maxPath, polyRadius);

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

                owner.positionUpdates = path;

                owner.hasMoved = true;
                owner.isAtSpawn = false;

                sw.Stop();
                zone.pathCalls++;
                zone.pathCallTime += sw.ElapsedMilliseconds;

                if (path.Count == 1)
                    Program.Log.Info($"mypos: {owner.positionX} {owner.positionY} {owner.positionZ} | targetPos: {x} {y} {z} | step {stepSize} | maxPath {maxPath} | polyRadius {polyRadius}");

                Program.Log.Error("[{0}][{1}] Created {2} points in {3} milliseconds", owner.actorId, owner.actorName, path.Count, sw.ElapsedMilliseconds);
            }
        }
    }
}
