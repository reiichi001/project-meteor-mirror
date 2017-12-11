using FFXIVClassic_Map_Server.Actors;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.dataobjects
{
    class GameEvent
    {
        private string eventName;
        private uint ownerActorId;
        private Player playerActor;
        private Actor ownerActor;
        private Coroutine coroutine;
        private uint hashCode;

        public GameEvent(String eventName, Player player, Actor owner)
        {
            this.eventName = eventName;
            this.playerActor = player;
            this.ownerActor = owner;
            this.ownerActorId = owner.actorId;
            hashCode = (uint)new Tuple<uint, uint, string>(player.actorId, owner.actorId, eventName).GetHashCode();
        }

        public string GetEventName()
        {
            return eventName;
        }

        public uint GetOwnerActorId()
        {
            return ownerActorId;
        }

        public Player GetPlayerActor()
        {
            return playerActor;
        }

        public Actor GetOwnerActor()
        {
            return ownerActor;
        }

        public Coroutine GetCoroutine()
        {
            return coroutine;
        }

        public void SetCoroutine(Coroutine coroutine)
        {
            this.coroutine = coroutine;
        }

        public uint GetUniqueEventId()
        {
            return hashCode;
        }

    }
}
