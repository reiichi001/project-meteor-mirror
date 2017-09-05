using FFXIVClassic_Map_Server.actors.chara.player;
using FFXIVClassic_Map_Server.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors.chara.npc
{
    class Retainer : Npc
    {
        public Retainer(uint id, string retainerName, ActorClass actorClass, Player player, float posX, float posY, float posZ, float rot)
            : base(0, actorClass, String.Format("_rtnre{0:x7}", id), player.GetZone(), posX, posY, posZ, rot, 0, 0, retainerName)
        {
            this.actorId = 0xD0000000 | id;
        }

        public void SendBazaarItems(Player player)
        {
            Inventory bazaar = new Inventory(this, 4, Inventory.RETAINER_BAZAAR);
            bazaar.SendFullInventory(player);
        }

        public void SendStorageItems(Player player)
        {
            Inventory storage = new Inventory(this, 4, 1);
            storage.SendFullInventory(player);
        }
    }
}
