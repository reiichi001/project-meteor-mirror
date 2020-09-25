require("modifiers")
require("battleutils")

--Forces crit of a single WS action from rear.
function onPreAction(effect, caster, target, skill, action, actionContainer)
    --If action hit from the rear and is a weaponskill ation
    if (action.param == HitDirection.Rear and skill.GetCommandType() == CommandType.WeaponSkill) then
        --Set action's crit rate to 100%
        action.critRate = 100.0;
    end

    --Remove status and add message 
    target.statusEffects.RemoveStatusEffect(effect, actionContainer);
end;

