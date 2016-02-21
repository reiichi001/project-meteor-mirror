using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.dataobjects
{
    class Item
    {
        //Basic
        public readonly string id;
        public readonly string name;
        
        //_item sheet
        public readonly string category;
        public readonly int maxStack;
        public readonly bool isRare;
        public readonly bool isExclusive;

        //itemData sheet
        public readonly int durability;
        public readonly int icon;
        public readonly int king;
        public readonly int color;
        public readonly int material;
        public readonly int decoration;
        public readonly int use;
        public readonly int mainSkill;
        public readonly int unknown1;
        public readonly int level;
        public readonly int compatibility;
        public readonly float effectMagnitude;
        public readonly float effectRate;
        public readonly float shieldBlocking;
        public readonly float effectDuration;
        public readonly float recastTime;
        public readonly float unknown2;
        public readonly byte recastGroup;
        public readonly int repairSkill;
        public readonly int repairItem;
        public readonly int repairItemNum;
        public readonly int repairLevel;
        public readonly int repairLicense;

    }
    class EquipmentItem : Item
    {       
        //equipment sheet
        public readonly int equipPoint;
        public readonly short equipTribe1;
        public readonly ushort unknown1;
        public readonly short equipTribe2;
        public readonly ushort unknown2;
        public readonly short equipTribe3;
        public readonly ushort unknown3;
        public readonly short equipTribe4;
        public readonly ushort unknown4;

        public readonly int paramBonusType1;
        public readonly short paramBonusValue1;
        public readonly int paramBonusType2;
        public readonly short paramBonusValue2;
        public readonly int paramBonusType3;
        public readonly short paramBonusValue3;
        public readonly int paramBonusType4;
        public readonly short paramBonusValue4;

        public readonly int paramBonusAtSlotType;
        public readonly short paramBonusAtSlotValue;

        public readonly int elementalBonusType;
        public readonly short elementalBonusValue;
    }

    class WeaponItem : EquipmentItem
    {
        //graphics
        public readonly int graphicsWeaponId;
        public readonly int graphicsEquipId;
        public readonly int graphicsVariantId;
        public readonly int graphicsColorId;

        //weapon sheet
        public readonly short attack;
        public readonly short magicAttack;
        public readonly short craftProcessing;
        public readonly short craftMagicProcessing;
        public readonly short harvestPotency;
        public readonly short harvestLimit;
        public readonly byte frequency;
        public readonly short rate;
        public readonly short magicRate;
        public readonly short craftProcessControl;
        public readonly short harvestRate;
        public readonly short critical;
        public readonly short magicCritical;
        public readonly short parry;

        public readonly int damageAttributeType1;
        public readonly float damageAttributeValue1;
        public readonly int damageAttributeType2;
        public readonly float damageAttributeValue2;
        public readonly int damageAttributeType3;
        public readonly float damageAttributeValue3;
    }

    class ArmorItem : EquipmentItem
    {
        //graphics
        public readonly int graphicsArmorId;
        public readonly int graphicsEquipId;
        public readonly int graphicsVariantId;
        public readonly int graphicsColorId;

        //armor sheet
        public readonly short defence;   
        public readonly short magicDefence;
        public readonly short criticalDefense;
        public readonly short evasion;
        public readonly short magicResistance;

        public readonly int damageDefenseType1;
        public readonly short damageDefenseValue1;
        public readonly int damageDefenseType2;
        public readonly short damageDefenseValue2;
        public readonly int damageDefenseType3;
        public readonly short damageDefenseValue3;
        public readonly int damageDefenseType4;
        public readonly short damageDefenseValue4;
    }

    class AccessoryItem : EquipmentItem
    {
        //graphics
        public readonly int graphicsAccessoryId;
        public readonly int graphicsEquipId;
        public readonly int graphicsVariantId;
        public readonly int graphicsColorId;

        //accessory sheet
        public readonly byte power;
        public readonly byte size;
    }

    
}
