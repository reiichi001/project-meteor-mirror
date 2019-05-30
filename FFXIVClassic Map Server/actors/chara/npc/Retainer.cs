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
        public const int MAXSIZE_INVENTORY_NORMAL = 150;
        public const int MAXSIZE_INVENTORY_CURRANCY = 320;
        public const int MAXSIZE_INVENTORY_BAZAAR = 10;

        private uint retainerId;
        private Player ownerPlayer;
        private Dictionary<ushort, Inventory> inventories = new Dictionary<ushort, Inventory>();

        public Retainer(uint retainerId, ActorClass actorClass, Player player, float posX, float posY, float posZ, float rot)
            : base(0, actorClass, "myretainer", player.GetZone(), posX, posY, posZ, rot, 0, 0, null)
        {
            this.retainerId = retainerId;
            this.ownerPlayer = player;
            this.actorName = String.Format("_rtnre{0:x7}", actorId);

            inventories[Inventory.NORMAL] = new Inventory(this, MAXSIZE_INVENTORY_NORMAL, Inventory.NORMAL);
            inventories[Inventory.CURRENCY_CRYSTALS] = new Inventory(this, MAXSIZE_INVENTORY_CURRANCY, Inventory.CURRENCY_CRYSTALS);
            inventories[Inventory.BAZAAR] = new Inventory(this, MAXSIZE_INVENTORY_BAZAAR, Inventory.BAZAAR);

            inventories[Inventory.NORMAL].InitList(Database.GetInventory(this, Inventory.NORMAL));
            inventories[Inventory.CURRENCY_CRYSTALS].InitList(Database.GetInventory(this, Inventory.CURRENCY_CRYSTALS));
            inventories[Inventory.BAZAAR].InitList(Database.GetInventory(this, Inventory.BAZAAR));
        }

        public Inventory GetInventory(ushort type)
        {
            if (inventories.ContainsKey(type))
                return inventories[type];
            else
                return null;
        }

        public void SendFullRetainerInventory(Player player)
        {         
            player.QueuePacket(InventoryBeginChangePacket.BuildPacket(actorId));
            inventories[Inventory.NORMAL].SendFullInventory(player);
            inventories[Inventory.CURRENCY_CRYSTALS].SendFullInventory(player);
            inventories[Inventory.BAZAAR].SendFullInventory(player);
            player.QueuePacket(InventoryEndChangePacket.BuildPacket(actorId));
        }

        public uint getRetainerId()
        {
            return retainerId;
        }
    }
}
