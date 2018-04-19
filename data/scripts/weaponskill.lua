-- todo: add enums for status effects in global.lua
--require("global")
require("battleutils")
--[[
     statId - see BattleTemp.cs
     modifier - Modifier.Intelligence, Modifier.Mind (see Modifier.cs)
     multiplier - 
  ]]


function CalculateDamage(caster, target, skill, action)
        --http://forum.square-enix.com/ffxiv/threads/36412-STR-PIE-ATK-Testing/page2
        --DRG numbers
        --Against Level 52 Halberdiers:
        --0.8 damage/STR. Caps at 350
        --0.67=0.69 damage/PIE. Hard cap at 310
        --0.35-0.37 damage/ATK for both AA and WS.
        
        --^ Old?
        --http://prestigexiv.guildwork.com/forum/threads/4fecdc94205cb248b5000526-dragoon-and-other-dd-dpsbase-damage-study?page=1#4fecdc94205cb248b5000525
        --10/09/2012 http://forum.square-enix.com/ffxiv/threads/55291-DPS-Testing/page4
        -- 1 point prim: 0.8 damage
        -- ATK: .1% damage? .38 damage?

        --Possible formula for melee?:
        --local strCap = CalculateCapOfWeapon(caster.getweapon)<- not sure how this is calculated yet, just an example
        --local secondaryCap = CalculateSecondaryCapOfWeapon(caster.getweapon)
        --local cappedStr = math.min(caster.GetMod(modifiersGlobal.Strength), strCap);
        --local cappedSec = math.min(caster.GetMod(caster.secondaryStat), secCap);
        --local damageBase = skill.basePotency + (0.85 * cappedStr) + (0.65 * cappedSec);

        --The maximum deviation for weaponskills is ~8%.
        --local dev = 0.96 + (math.random() * 8);
        --damageBase = math.Clamp(damageBase * dev, 1, 9999);
        --return damageBase;
        return 100;
end

function HandleHealingSkill(caster, target, skill, action, statId, modifierId, multiplier, baseAmount)
    potency = potency or 1.0;
    healAmount = baseAmount;
    
    -- todo: shit based on mnd
    local mind = caster.GetMod(Modifier.Mind);
end;

function HandleAttackSkill(caster, target, skill, action, statId, modifierId, multiplier, baseAmount)
    -- todo: actually handle this
    damage = baseAmount or math.random(1,10) * 10;
    
    return damage;
end;

function HandleStoneskin(caster, target, skill, action, statId, modifierId, damage)
    --[[
    if target.statusEffects.HasStatusEffect(StatusEffect.Stoneskin) then
        -- todo: damage reduction
        return true;
    end;
    ]]
    return false;
end;