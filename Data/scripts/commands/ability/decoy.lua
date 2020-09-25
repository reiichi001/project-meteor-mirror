require("global");
require("ability");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)
    --27244: Enhanced Decoy: Renders Decoy capable of evading melee attacks
    if caster.HasTrait(27244) then
        ability.statusId = 223238;
    end
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;