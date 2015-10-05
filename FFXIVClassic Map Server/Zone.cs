using FFXIVClassic_Map_Server.dataobjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server
{
    class Zone
    {
        public uint mapId;
        public int boundingGridSize = 200;

        private Dictionary<Tuple<int, int>, List<Actor>> actorBlock = new Dictionary<Tuple<int, int>, List<Actor>>();

        public void addActorToZone(Actor actor)
        {
            int gridX = (int)actor.positionX / boundingGridSize;
            int gridY = (int)actor.positionY / boundingGridSize;

            lock (actorBlock)
                actorBlock[Tuple.Create(gridX, gridY)].Add(actor);
        }

        public void removeActorToZone(Actor actor)
        {
            int gridX = (int)actor.positionX / boundingGridSize;
            int gridY = (int)actor.positionY / boundingGridSize;

            lock (actorBlock)
                actorBlock[Tuple.Create(gridX, gridY)].Remove(actor);
        }

        public void updateActorPosition(Actor actor)
        {
            int gridX = (int)actor.positionX / boundingGridSize;
            int gridY = (int)actor.positionY / boundingGridSize;

            int gridOldX = (int)actor.oldPositionX / boundingGridSize;
            int gridOldY = (int)actor.oldPositionY / boundingGridSize;

            //Still in same block
            if (gridX == gridOldX && gridY == gridOldY)
                return;

            lock (actorBlock)
                actorBlock[Tuple.Create(gridOldX, gridOldY)].Remove(actor);
            lock (actorBlock)
                actorBlock[Tuple.Create(gridX, gridY)].Add(actor);
        }

        public List<Actor> getActorsAroundPoint(float x, float y, int checkDistance)
        {
            int gridX = (int)x/boundingGridSize;
            int gridY = (int)y/boundingGridSize;

            List<Actor> result = new List<Actor>();

            for (int gx = gridX - checkDistance; gx <= gridX + checkDistance; gx++)
            {
                for (int gy = gridY - checkDistance; gy <= gridY + checkDistance; gy++)
                {
                    result.AddRange(actorBlock[Tuple.Create(gx, gy)]);
                }
            }

            return result;
        }
        
    }
}
