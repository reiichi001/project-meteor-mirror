require("battleutils")

--Untraited reduces cooldown by 50%
--Traited reduces cooldown by 100%
function onCommandStart(effect, owner, skill, actionContainer)
    if skill.GetCommandType() == CommandType.Weaponskill then
        local reduction = 0.5;

        if effect.GetTier() == 2 then
            reduction = 1.0;
        end

        skill.recastTimeMs = skill.recastTimeMs - (reduction * skill.recastTimeMs);
        owner.statusEffects.RemoveStatusEffect(effect, actionContainer, 30331, false);
    end
end;