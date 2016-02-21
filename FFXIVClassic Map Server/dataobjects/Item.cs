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
        public readonly uint catalogID;
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

        #region Utility Functions
        public bool IsMoney()
        {
            return catalogID >= 1000000 && catalogID <= 1999999;
        }

        public bool IsImportant()
        {
            return catalogID >= 2000001 && catalogID <= 2002048;
        }

        public bool IsFood()
        {
            return catalogID >= 3010000 && catalogID <= 3019999;
        }

        public bool IsDrink()
        {
            return catalogID >= 3010600 && catalogID <= 3010699;
        }

        public bool IsPotion()
        {
            return catalogID >= 3020000 && catalogID <= 3029999;
        }

        public bool IsEquipment()
        {
            return catalogID >= 3900000 && catalogID <= 9999999;
        }

        public bool IsWeapon()
        {
            return catalogID >= 3900000 && catalogID <= 7999999;
        }

        public bool IsBattleWeapon()
        {
            return catalogID >= 3900000 && catalogID <= 5999999;
        }

        public bool IsAttackWeapon()
        {
            return catalogID >= 4020000 && catalogID <= 4999999;
        }

        public bool IsNailWeapon()
        {
            return catalogID >= 4020000 && catalogID <= 4029999;
        }

        public bool IsSwordWeapon()
        {
            return catalogID >= 4030000 && catalogID <= 4039999;
        }

        public bool IsAxeWeapon()
        {
            return catalogID >= 4040000 && catalogID <= 4049999;
        }

        public bool IsRapierWeapon()
        {
            return catalogID >= 4050000 && catalogID <= 4059999;
        }

        public bool IsMaceWeapon()
        {
            return catalogID >= 4060000 && catalogID <= 4069999;
        }

        public bool IsBowWeapon()
        {
            return catalogID >= 4070000 && catalogID <= 4079999;
        }

        public bool IsLanceWeapon()
        {
            return catalogID >= 4080000 && catalogID <= 4089999;
        }

        public bool IsGunWeapon()
        {
            return catalogID >= 4090000 && catalogID <= 4099999;
        }

        public bool IsLongRangeWeapon()
        {
            return catalogID >= 4050000 && catalogID <= 4059999;
        }

        public bool IsShotWeapon()
        {
            return !IsBowWeapon() ? IsGunWeapon() : false;
        }

        public bool IsAmmoWeapon()
        {
            return !IsThrowWeapon() && !IsArrowWeapon();
        }

        public bool IsThrowWeapon()
        {
            return catalogID >= 3910000 && catalogID <= 3919999;
        }

        public bool IsArrowWeapon()
        {
            return catalogID >= 3920000 && catalogID <= 3929999;
        }

        public bool IsBulletWeapon()
        {
            return catalogID >= 3930000 && catalogID <= 3939999;
        }

        public bool IsShieldWeapon()
        {
            return catalogID >= 4100000 && catalogID <= 4109999;
        }

        public bool IsManualGuardShieldWeapon()
        {
            return IsShieldWeapon() && GetShieldGuardTime() != -1;
        }

        public bool IsMagicWeapon()
        {
            return catalogID >= 5000000 && catalogID <= 5999999;
        }

        public bool IsMysticWeapon()
        {
            return catalogID >= 5010000 && catalogID <= 5019999;
        }

        public bool IsThaumaturgeWeapon()
        {
            return catalogID >= 5020000 && catalogID <= 5029999;
        }

        public bool IsConjurerWeapon()
        {
            return catalogID >= 5030000 && catalogID <= 5039999;
        }

        public bool IsArchanistWeapon()
        {
            return catalogID >= 5040000 && catalogID <= 5049999;
        }

        public bool IsCraftWeapon()
        {
            return catalogID >= 6000000 && catalogID <= 6999999;
        }

        public bool IsCarpenterWeapon()
        {
            return catalogID >= 6010000 && catalogID <= 6019999;
        }

        public bool IsBlackSmithWeapon()
        {
            return catalogID >= 6020000 && catalogID <= 6029999;
        }

        public bool IsArmorerWeapon()
        {
            return catalogID >= 6030000 && catalogID <= 6039999;
        }

        public bool IsGoldSmithWeapon()
        {
            return catalogID >= 6040000 && catalogID <= 6049999;
        }

        public bool IsTannerWeapon()
        {
            return catalogID >= 6050000 && catalogID <= 6059999;
        }

        public bool IsWeaverWeapon()
        {
            return catalogID >= 6060000 && catalogID <= 6069999;
        }

        public bool IsAlchemistWeapon()
        {
            return catalogID >= 6070000 && catalogID <= 6079999;
        }

        public bool IsCulinarianWeapon()
        {
            return catalogID >= 6080000 && catalogID <= 6089999;
        }

        public bool IsHarvestWeapon()
        {
            return catalogID >= 7000000 && catalogID <= 7999999;
        }

        public bool IsMinerWeapon()
        {
            return catalogID >= 7010000 && catalogID <= 7019999;
        }

        public bool IsBotanistWeapon()
        {
            return catalogID >= 7020000 && catalogID <= 7029999;
        }

        public bool IsFishingWeapon()
        {
            return catalogID >= 7030000 && catalogID <= 7039999;
        }

        public bool IsShepherdWeapon()
        {
            return catalogID >= 7040000 && catalogID <= 7049999;
        }

        public bool IsFishingBaitWeapon()
        {
            return catalogID >= 3940000 && catalogID <= 3949999;
        }

        public bool IsFishingLureWeapon()
        {
            return catalogID >= 3940100 && catalogID <= 3940199;
        }

        public bool IsArmor()
        {
            return catalogID >= 8000000 && catalogID <= 8999999;
        }

        public bool IsAccessory()
        {
            return catalogID >= 9000000 && catalogID <= 9079999;
        }

        public bool IsAmulet()
        {
            return catalogID >= 9080000 && catalogID <= 9089999;
        }

        public bool IsEnchantMateria()
        {
            return catalogID >= 10100000 && catalogID <= 10199999;
        }

        public bool IsMaterial()
        {
            return catalogID >= 10000000 && catalogID <= 10999999;
        }

        public bool IsEventItem()
        {
            return catalogID >= 11000000 && catalogID <= 15000000;
        }

        public bool IsUseForBattle()
        {
            return false;
        }

        public bool IsHostilityItem()
        {
            return true;
        }

        public bool IsUsable()
        {
            return use != 0;
        }

        public bool IsUseFree()
        {
            return use == -1;
        }

        public bool IsLostAfterUsed()
        {
            return !IsEquipment();
        }

        public int GetShieldGuardTime()
        {
            return -1;
        }

        #endregion

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
