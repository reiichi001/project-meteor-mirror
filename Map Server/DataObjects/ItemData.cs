/*
===========================================================================
Copyright (C) 2015-2019 Project Meteor Dev Team

This file is part of Project Meteor Server.

Project Meteor Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Project Meteor Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with Project Meteor Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/

using MySql.Data.MySqlClient;
using System;

namespace Meteor.Map.dataobjects
{
    class ItemData
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
        public readonly int sellPrice;
        public readonly int icon;
        public readonly int kind;
        public readonly int rarity;
        public readonly int isUseable;
        public readonly int mainSkill;
        public readonly int subSkill;
        public readonly int levelType;
        public readonly int level;
        public readonly int compatibility;
        public readonly float effectMagnitude;
        public readonly float effectRate;
        public readonly float shieldBlocking;
        public readonly float effectDuration;
        public readonly float recastTime;
        public readonly byte recastGroup;
        public readonly int repairSkill;
        public readonly int repairItem;
        public readonly int repairItemNum;
        public readonly int repairLevel;
        public readonly int repairLicense;

        public ItemData(MySqlDataReader reader)
        {
            catalogID = reader.GetUInt32("catalogID");
            name = reader.GetString("name");

            category = reader.GetString("category");
            maxStack = reader.GetInt32("maxStack");
            isRare = reader.GetBoolean("isRare");
            isExclusive = reader.GetBoolean("isExclusive");

            durability = reader.GetInt32("durability");
            sellPrice = reader.GetInt32("sellPrice");

            icon = reader.GetInt32("icon");
            kind = reader.GetInt32("kind");
            rarity = reader.GetInt32("rarity");
            isUseable = reader.GetInt32("isUseable");
            mainSkill = reader.GetInt32("mainSkill");
            subSkill = reader.GetInt32("subSkill");
            levelType = reader.GetInt32("levelType");
            level = reader.GetInt32("level");
            compatibility = reader.GetInt32("compatibility");
            effectMagnitude = reader.GetFloat("effectMagnitude");
            effectRate = reader.GetFloat("effectRate");
            shieldBlocking = reader.GetFloat("shieldBlocking");
            effectDuration = reader.GetFloat("effectDuration");
            recastTime = reader.GetFloat("recastTime");
            recastGroup = reader.GetByte("recastGroup");
            repairSkill = reader.GetInt32("repairSkill");
            repairItem = reader.GetInt32("repairItem");
            repairItemNum = reader.GetInt32("repairItemNum");
            repairLevel = reader.GetInt32("repairLevel");
            repairLicense = reader.GetInt32("repairLicense");            
        }

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

        public static bool IsWeapon(uint catalogID)
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

        public static bool IsArmor(uint catalogID)
        {
            return catalogID >= 8000000 && catalogID <= 8999999;
        }

        public bool IsAccessory()
        {
            return catalogID >= 9000000 && catalogID <= 9079999;
        }

        public static bool IsAccessory(uint catalogID)
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
            return isUseable != 0;
        }

        public bool IsUseFree()
        {
            return isUseable == -1;
        }

        public bool IsLostAfterUsed()
        {
            return !IsEquipment();
        }

        public int GetShieldGuardTime()
        {
            return -1;
        }

        public Double GetItemHQValue(float value1, float value2)
        {
            return Math.Max(value1 + 1, Math.Ceiling(value1 * value2));
        }

        #endregion

    }

    class EquipmentItem : ItemData
    {
        //graphics
        public readonly uint graphicsWeaponId;
        public readonly uint graphicsEquipmentId;
        public readonly uint graphicsVariantId;
        public readonly uint graphicsColorId;
   
        //equipment sheet
        public readonly int equipPoint;
        public readonly short equipTribe;

        public readonly int paramBonusType1;
        public readonly short paramBonusValue1;
        public readonly int paramBonusType2;
        public readonly short paramBonusValue2;
        public readonly int paramBonusType3;
        public readonly short paramBonusValue3;
        public readonly int paramBonusType4;
        public readonly short paramBonusValue4;
        public readonly int paramBonusType5;
        public readonly short paramBonusValue5;
        public readonly int paramBonusType6;
        public readonly short paramBonusValue6;
        public readonly int paramBonusType7;
        public readonly short paramBonusValue7;
        public readonly int paramBonusType8;
        public readonly short paramBonusValue8;
        public readonly int paramBonusType9;
        public readonly short paramBonusValue9;
        public readonly int paramBonusType10;
        public readonly short paramBonusValue10;

        public readonly short AdditionalEffect;
        public readonly bool materialBindPermission;
        public readonly short materializeTable;

        public EquipmentItem(MySqlDataReader reader) 
            : base (reader)
        {
            if (!reader.IsDBNull(reader.GetOrdinal("weaponId")) && !reader.IsDBNull(reader.GetOrdinal("equipmentId")) && !reader.IsDBNull(reader.GetOrdinal("variantId")) && !reader.IsDBNull(reader.GetOrdinal("colorId")))
            {
                graphicsWeaponId = reader.GetUInt32("weaponId");
                graphicsEquipmentId = reader.GetUInt32("equipmentId");
                graphicsVariantId = reader.GetUInt32("variantId");
                graphicsColorId = reader.GetUInt32("colorId");
            }

            equipPoint = reader.GetInt32("equipPoint");
            equipTribe = reader.GetInt16("equipTribe");

            paramBonusType1 = reader.GetInt32("paramBonusType1");
            paramBonusValue1 = reader.GetInt16("paramBonusValue1");
            paramBonusType2 = reader.GetInt32("paramBonusType2");
            paramBonusValue2 = reader.GetInt16("paramBonusValue2");
            paramBonusType3 = reader.GetInt32("paramBonusType3");
            paramBonusValue3 = reader.GetInt16("paramBonusValue3");
            paramBonusType4 = reader.GetInt32("paramBonusType4");
            paramBonusValue4 = reader.GetInt16("paramBonusValue4");
            paramBonusType5 = reader.GetInt32("paramBonusType5");
            paramBonusValue5 = reader.GetInt16("paramBonusValue5");
            paramBonusType6 = reader.GetInt32("paramBonusType6");
            paramBonusValue6 = reader.GetInt16("paramBonusValue6");
            paramBonusType7 = reader.GetInt32("paramBonusType7");
            paramBonusValue7 = reader.GetInt16("paramBonusValue7");
            paramBonusType8 = reader.GetInt32("paramBonusType8");
            paramBonusValue8 = reader.GetInt16("paramBonusValue8");
            paramBonusType9 = reader.GetInt32("paramBonusType9");
            paramBonusValue9 = reader.GetInt16("paramBonusValue9");
            paramBonusType10 = reader.GetInt32("paramBonusType10");
            paramBonusValue10 = reader.GetInt16("paramBonusValue10");

            AdditionalEffect = reader.GetInt16("additionalEffect");
            materialBindPermission = reader.GetBoolean("materiaBindPermission");
            materializeTable = reader.GetInt16("materializeTable");
        }
    }

    class WeaponItem : EquipmentItem
    {
        //extra graphics
        public readonly uint graphicsOffhandWeaponId;
        public readonly uint graphicsOffhandEquipmentId;
        public readonly uint graphicsOffhandVariantId;

        //weapon sheet
        public readonly short attack;
        public readonly short magicAttack;
        public readonly short craftProcessing;
        public readonly short craftMagicProcessing;
        public readonly short harvestPotency;
        public readonly short harvestLimit;
        public readonly byte frequency; // hit count, 2 for h2h weapons
        public readonly short rate;
        public readonly short magicRate;
        public readonly short craftProcessControl;
        public readonly short harvestRate;
        public readonly short critical;
        public readonly short magicCritical;
        public readonly short parry;

        public readonly int damageAttributeType1; // 1 slashing, 2 piercing, 3 blunt, 4 projectile
        public readonly float damageAttributeValue1;
        public readonly int damageAttributeType2;
        public readonly float damageAttributeValue2;
        public readonly int damageAttributeType3;
        public readonly float damageAttributeValue3;

        public readonly short damagePower;
        public readonly float damageInterval;
        public readonly short ammoVirtualDamagePower;
        public readonly float dps;

        public WeaponItem(MySqlDataReader reader)
            : base(reader)
        {
            if (!reader.IsDBNull(reader.GetOrdinal("offHandWeaponId")) && !reader.IsDBNull(reader.GetOrdinal("offHandEquipmentId")) && !reader.IsDBNull(reader.GetOrdinal("offHandVarientId")))
            {
                graphicsOffhandWeaponId = reader.GetUInt32("offHandWeaponId");
                graphicsOffhandEquipmentId = reader.GetUInt32("offHandEquipmentId");
                graphicsOffhandVariantId = reader.GetUInt32("offHandVarientId");
            }

            attack = reader.GetInt16("attack");
            magicAttack = reader.GetInt16("magicAttack");
            craftProcessing = reader.GetInt16("craftProcessing");
            craftMagicProcessing = reader.GetInt16("craftMagicProcessing");
            harvestPotency = reader.GetInt16("harvestPotency");
            harvestLimit = reader.GetInt16("harvestLimit");
            frequency = reader.GetByte("frequency");
            rate = reader.GetInt16("rate");
            magicRate = reader.GetInt16("magicRate");
            craftProcessControl = reader.GetInt16("craftProcessControl");
            harvestRate = reader.GetInt16("harvestRate");
            critical = reader.GetInt16("critical");
            magicCritical = reader.GetInt16("magicCritical");
            parry = reader.GetInt16("parry");

            damageAttributeType1 = reader.GetInt32("damageAttributeType1");
            damageAttributeValue1 = reader.GetFloat("damageAttributeValue1");
            damageAttributeType2 = reader.GetInt32("damageAttributeType2");
            damageAttributeValue2 = reader.GetFloat("damageAttributeValue2");
            damageAttributeType3 = reader.GetInt32("damageAttributeType3");
            damageAttributeValue3 = reader.GetFloat("damageAttributeValue3");

            damagePower = reader.GetInt16("damagePower");
            damageInterval = reader.GetFloat("damageInterval");
            ammoVirtualDamagePower = reader.GetInt16("ammoVirtualDamagePower");
            dps = (damagePower + ammoVirtualDamagePower) / damageInterval;
        }
    }

    class ArmorItem : EquipmentItem
    {
        //armor sheet
        public readonly short defense;
        public readonly short magicDefense;
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

        public ArmorItem(MySqlDataReader reader)
            : base(reader)
        {
            defense = reader.GetInt16("defense");
            magicDefense = reader.GetInt16("magicDefense");
            criticalDefense = reader.GetInt16("criticalDefense");
            evasion = reader.GetInt16("evasion");
            magicResistance = reader.GetInt16("magicResistance");

            damageDefenseType1 = reader.GetInt32("damageDefenseType1");
            damageDefenseValue1 = reader.GetInt16("damageDefenseValue1");
            damageDefenseType2 = reader.GetInt32("damageDefenseType2");
            damageDefenseValue2 = reader.GetInt16("damageDefenseValue2");
            damageDefenseType3 = reader.GetInt32("damageDefenseType3");
            damageDefenseValue3 = reader.GetInt16("damageDefenseValue3");
            damageDefenseType4 = reader.GetInt32("damageDefenseType4");
            damageDefenseValue4 = reader.GetInt16("damageDefenseValue4");
        }
    }

    class AccessoryItem : EquipmentItem
    {
        //accessory sheet
        public readonly byte power;
        public readonly byte size;

        public AccessoryItem(MySqlDataReader reader)
            : base(reader)
        {
            power = reader.GetByte("power");
            size = reader.GetByte("size");
        }
    }
    
}
