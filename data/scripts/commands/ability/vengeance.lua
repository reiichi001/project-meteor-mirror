require("global");
require("ability");
require("battleutils")

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)

    --8032703: Fighter's Cuirass: Enhances Vengeance
    if caster.HasItemEquippedInSlot(8032703, 10) then
        skill.statusTier = 2;
    end

    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;