require("global");
require("ability");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    skill.statusMagnitude = 4;
    
    --27242: Enhanced Barrage: Adds an additional attack to barrage ( 4 -> 5 )
    if caster.HasTrait(27242) then
        skill.statusMagnitude = 5;
    end

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;