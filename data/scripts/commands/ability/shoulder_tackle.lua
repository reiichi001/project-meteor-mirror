require("global");
require("ability");

function onSkillPrepare(caster, target, skill)
    return 0;
end;

function onSkillStart(caster, target, skill)
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --chance to influct stun only when target has no enmity towards you
    if not (target.hateContainer.HasHateForTarget(caster)) then
        skill.statusChance = 0.50;
    end

    --calculate ws damage
    action.amount = skill.basePotency;

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);

    --Try to apply status effect
    action.TryStatus(caster, target, skill, actionContainer, true);
end;