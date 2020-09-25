require("battleutils")

--Heals for 30%? of damage dealt on auto attacks.
--Trait: Increases healing by 20%. Is this the base % or the amount after taking the base percent?
--I'm guessing the way it works is that LSI/II/III have 10/20/30% absorb by default and 30/40/50% traited.
--Seems to match what i can find in videos
function onHit(effect, attacker, defender, skill, action, actionContainer)
    if skill.GetCommandType() == CommandType.AutoAttack then
        local healPercent = 0.30;

        if effect.GetTier() == 2 then
            healPercent = 0.50;
        end

        local amount = math.floor((healPercent * action.amount) + 1);
        attacker.AddHP(amount);
        actionContainer.AddHPAbsorbAction(defender.actorId, 30332, amount);
    end
end;