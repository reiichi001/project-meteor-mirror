using FFXIVClassic_Map_Server.actors.chara.player;
using FFXIVClassic_Map_Server.Actors;
using System;

namespace FFXIVClassic_Map_Server.actors.chara.npc
{
    class Retainer : Npc
    {
        public const int MAXSIZE_INVENTORY_NORMAL = 150;
        public const int MAXSIZE_INVENTORY_CURRANCY = 320;
        public const int MAXSIZE_INVENTORY_BAZAAR = 10;

        private uint retainerId;
        private Player ownerPlayer;

        public Retainer(uint retainerId, ActorClass actorClass, Player player, float posX, float posY, float posZ, float rot)
            : base(0, actorClass, "myretainer", player.GetZone(), posX, posY, posZ, rot, 0, 0, null)
        {
            this.retainerId = retainerId;
            this.ownerPlayer = player;
            this.actorName = String.Format("_rtnre{0:x7}", actorId);

            itemPackages[ItemPackage.NORMAL] = new ItemPackage(this, MAXSIZE_INVENTORY_NORMAL, ItemPackage.NORMAL);
            itemPackages[ItemPackage.CURRENCY_CRYSTALS] = new ItemPackage(this, MAXSIZE_INVENTORY_CURRANCY, ItemPackage.CURRENCY_CRYSTALS);
            itemPackages[ItemPackage.BAZAAR] = new ItemPackage(this, MAXSIZE_INVENTORY_BAZAAR, ItemPackage.BAZAAR);

            itemPackages[ItemPackage.NORMAL].InitList(Database.GetInventory(this, ItemPackage.NORMAL));
            itemPackages[ItemPackage.CURRENCY_CRYSTALS].InitList(Database.GetInventory(this, ItemPackage.CURRENCY_CRYSTALS));
            itemPackages[ItemPackage.BAZAAR].InitList(Database.GetInventory(this, ItemPackage.BAZAAR));
        }

        public uint GetRetainerId()
        {
            return retainerId;
        }
    }
}
