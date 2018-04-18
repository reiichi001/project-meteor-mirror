require("global");
require("magic");

function onMagicPrepare(caster, target, spell)
    return 0;
end;

function onMagicStart(caster, target, spell)
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --Freeze generates 0 enmity and removes a flat 720 enmity
    spell.enmityModifier = 0;
    target.hateContainer.UpdateHate(caster, -720);

    --calculate damage
    action.amount = skill.basePotency;

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;