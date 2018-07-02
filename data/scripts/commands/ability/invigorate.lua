require("global");
require("Ability");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)
    --27280: Enhanced Invigorate: Increases duration of Invigorate by 15 seconds
    if caster.HasTrait(27280) then
        ability.statusDuration = ability.statusDuration + 15;
    end

    --Drachen Mail: Increases Invigorate TP tick from 100 to 120.
    local magnitude = 100;

    --8032704: Drachen Mail
    if caster.HasItemEquippedInSlot(8032704, 10) then
        magnitude = 120;
    end

    ability.statusMagnitude = magnitude;
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;