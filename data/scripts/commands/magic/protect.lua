require("global");
require("magic");
require("modifiers");

function onMagicPrepare(caster, target, spell)
    return 0;
end;

function onMagicStart(caster, target, spell)
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --Actual amount of def/mdef will be calculated in OnGain
    skill.statusMagnitude = caster.GetMod(modifiersGlobal.EnhancementMagicPotency);
    
    --27365: Enhanced Protect: Increases magic defense gained from Protect.
    if caster.HasTrait(27365) then
        skill.statusId = 223129
    end
    
    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;