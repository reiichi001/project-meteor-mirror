using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.actors.chara.ai;
using FFXIVClassic_Map_Server.actors.chara.ai.controllers;

namespace FFXIVClassic_Map_Server.actors.chara.npc
{
    class Ally : BattleNpc
    {
        // todo: ally class is probably not necessary
        public Ally(int actorNumber, ActorClass actorClass, string uniqueId, Area spawnedArea, float posX, float posY, float posZ, float rot,
            ushort actorState, uint animationId, string customDisplayName)
            : base(actorNumber, actorClass, uniqueId, spawnedArea, posX, posY, posZ, rot, actorState, animationId, customDisplayName)  
        {
            aiContainer = new AIContainer(this, new AllyController(this), new PathFind(this), new TargetFind(this));
            this.allegiance = CharacterTargetingAllegiance.Player;
            this.isAutoAttackEnabled = true;
            this.isMovingToSpawn = false;
        }
    }
}
