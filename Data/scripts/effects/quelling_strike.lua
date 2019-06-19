require("modifiers")
require("battleutils")

--Untraited reduces cooldown by 50%
--Traited reduces cooldown by 100%
function onCommandStart(effect, owner, skill, actionContainer)
    --Does this apply to auto attacks?
    if skill.GetCommandType() == CommandType.Weaponskill or skill.GetCommandType() == CommandType.Ability or skill.GetCommandType() == CommandType.Magic then
        if skill.GetActionType() == ActionType.Physical or skill.GetActionType() == ActionType.Magic then
            skill.enmityModifier = skill.enmityModifier * 0.8;
            skill.tpCost = skill.tpCost - effect.GetMagnitude();
        end
    end
end;