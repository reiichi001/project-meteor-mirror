require("modifiers")
require("battleutils")

--Untraited reduces cooldown by 50%
--Traited reduces cooldown by 100%
function onCommandStart(effect, owner, skill, actionContainer)
    --Does this apply to auto attacks?
    if skill.commandType == CommandType.Weaponskill or skill.commandType == CommandType.Ability or skill.commandType == CommandType.Magic then
        if skill.actionType == ActionType.Physical or skill.actionType == ActionType.Magic then
            --No idea what the enmity effect is
            skill.enmityModifier = skill.enmityModifier * 0.5;

            owner.AddTP(effect.GetMagnitude());
        end
    end
end;