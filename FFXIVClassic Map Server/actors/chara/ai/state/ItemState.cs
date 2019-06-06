using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.dataobjects;

namespace FFXIVClassic_Map_Server.actors.chara.ai.state
{
    class ItemState : State
    {
        ItemData item;
        new Player owner;
        public ItemState(Player owner, Character target, ushort slot, uint itemId) :
            base(owner, target)
        {
            this.owner = owner;
            this.target = target;
        }
    }
}
