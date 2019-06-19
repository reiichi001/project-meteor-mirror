require("global");
require("magic");

function onMagicPrepare(caster, target, spell)
    return 0;
end;

function onMagicStart(caster, target, spell)
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    action.amount = skill.basePotency;

    --8071401: Gallant Gauntlets: Enhances Holy Succor
    if caster.HasItemEquippedInSlot(8071401, 13) then
        action.amount = action.amount * 1.10;
    end

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);

    --When cast on another player you also heal 50% of the amount restored.
    if caster != target then
        caster.AddHP(action.amount / 2)
        --33012: You recover [amount] HP.
        actionContainer.AddHPAbsorbAction(caster.actorId, 33012, (action.amount / 2));
    end
end;