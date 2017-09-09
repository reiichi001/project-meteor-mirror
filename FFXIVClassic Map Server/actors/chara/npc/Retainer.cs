using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors.chara.player;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.packets.send.actor.inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors.chara.npc
{
    class Retainer : Npc
    {
        Player player;

        public Retainer(string retainerName, ActorClass actorClass, Player player, float posX, float posY, float posZ, float rot)
            : base(0, actorClass, "myretainer", player.GetZone(), posX, posY, posZ, rot, 0, 0, retainerName)
        {
            this.player = player;
            this.actorName = String.Format("_rtnre{0:x7}", actorId);
        }

        public void SendBazaarItems(Player player)
        {
                Inventory bazaar = new Inventory(this, 150, (ushort)0);
                bazaar.AddItem(1000001);
                bazaar.AddItem(3020517);
                player.QueuePacket(InventoryBeginChangePacket.BuildPacket(actorId));
                bazaar.SendFullInventory(player);
                player.QueuePacket(InventoryEndChangePacket.BuildPacket(actorId));
            
        }

        public void SendStorageItems(Player player)
        {
            Inventory storage = new Inventory(this, 10, Inventory.CURRENCY);
            storage.AddItem(1000001);
            storage.AddItem(3020519);
            player.QueuePacket(InventoryBeginChangePacket.BuildPacket(actorId));
            storage.SendFullInventory(player);
            player.QueuePacket(InventoryEndChangePacket.BuildPacket(actorId));
        }

        public void SendHuhItems(Player player)
        {
            Inventory storage = new Inventory(this, 20, 7);
            storage.AddItem(1000003);
            storage.AddItem(1000016);
            player.QueuePacket(InventoryBeginChangePacket.BuildPacket(actorId));
            storage.SendFullInventory(player);
            player.QueuePacket(InventoryEndChangePacket.BuildPacket(actorId));
        }

    }
}
