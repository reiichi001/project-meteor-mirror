require("global");
require("modifiers");
--require("ability");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)

    return 0;
end;

--http://forum.square-enix.com/ffxiv/threads/51208-2nd-wind-modifier
--The primary modifier for SW is class level.

--There are three other factors that contribute to SW:
-- PGL's SW trait, which increases potency by 25%.
-- A bonus from INT (2INT=1HP)
-- An additional random integer (580 at level 50. +/- 3%)
function onSkillFinish(caster, target, skill, action, actionContainer)
    --Base formula isn't quit known yet
    local amount = 100;
    --Heals can vary by up to ~3.5% in either direction
    amount = math.Clamp(amount * (0.965 + (math.rand() * 7.0)), 0, 9999);

    --PGL gets an INT bonus for Second Wind
    if caster.GetClass() == 2 then
        amount = amount + caster.GetMod(modifiersGlobal.Intelligence) / 2;
    end;

    --27120: Enhanced Second Wind
    if caster.HasTrait(27120) then
        amount = amount * 1.25;
    end;
    
    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;