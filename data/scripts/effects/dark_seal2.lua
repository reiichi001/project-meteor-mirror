require("modifiers")
require("battleutils")

--Increases accuracy of next cast.
--There isn't really any information on this, but due to the fact it falls off BEFORE the target is hit, 
--I'm assuming it increases a spell's accuracy modifier instead of giving actual magic accuracy
function onCommandStart(effect, owner, skill, actionContainer)
    if skill.GetActionType() == ActionType.Magic then
        --50 is random guess.
        skill.accuracyModifier = skill.accuracyModifier + 50;
        owner.statusEffects.RemoveStatusEffect(effect, actionContainer, 30331, false);
    end
end