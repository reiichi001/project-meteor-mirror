require("global");
require("weaponskill");
require("battleutils");
require("hiteffect");

function onSkillPrepare(caster, target, skill)
    return 0;
end;

function onSkillStart(caster, target, skill)
    return 0;
end;

--
function onSkillFinish(caster, target, skill, action, actionContainer)
    --calculate ws damage
    action.amount = skill.basePotency;

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, action);

    --additional effect
    --Restores MP
    --Comboed formula seems to be (0.40 * damage) + 180
    --Uncomboed formula seems to be 0.30 * damage
    --These may be wrong. It seems like max mp might influence the slope

    --1.21: Equation used to calculate amount of MP adjusted.
    --fug
    --This might mean max MP isn't involved and the difference was between patches. need to recheck videos
    if action.ActionLanded() and (action.param == HitDirection.Right or action.param == HitDirection.Left) then
        local mpToReturn = 0;

        if skill.isCombo then
            mpToReturn = (0.40 * action.amount) + 180;
        else
            mpToReturn = (0.30 * action.amount);
        end

        caster.AddMP(mpToReturn);
        --30452: You recover x MP.
        actionContainer.AddMPAbsorbAction(caster.actorId, 30452, mpToReturn);
    end
end;