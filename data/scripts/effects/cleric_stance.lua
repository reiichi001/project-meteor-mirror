require("modifiers")
require("battleutils")

function onPreAction(caster, target, effect, skill, action, actionContainer)
    if skill.commandType == CommandType.Spell then
        if action.actionType == ActionType.Heal then
            action.amount = action.amount * 0.80;
        elseif action.actionType == ActionType.Magic then
            action.amount = action.amount * 1.20;
        end
    end
end;

