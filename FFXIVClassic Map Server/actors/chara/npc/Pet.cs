using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FFXIVClassic_Map_Server.actors.chara.ai;
using FFXIVClassic_Map_Server.actors.chara.ai.controllers;
using FFXIVClassic_Map_Server.actors.chara.npc;
using FFXIVClassic_Map_Server.packets.send.actor;

namespace FFXIVClassic_Map_Server.Actors
{
    class Pet : BattleNpc
    {
        public Pet(int actorNumber, ActorClass actorClass, string uniqueId, Area spawnedArea, float posX, float posY, float posZ, float rot,
                    ushort actorState, uint animationId, string customDisplayName)
            : base(actorNumber, actorClass, uniqueId, spawnedArea, posX, posY, posZ, rot, actorState, animationId, customDisplayName)  
        {
            this.aiContainer = new AIContainer(this, new PetController(this), new PathFind(this), new TargetFind(this));
            this.currentSubState = SetActorStatePacket.SUB_STATE_MONSTER;
            this.hateContainer = new HateContainer(this);
        }
    }
}
