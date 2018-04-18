require("global");
require("ability");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)
    --27204: Enhanced Rampage
    if caster.HasTrait(27204) then
        ability.statusTier = 2;
    end
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;