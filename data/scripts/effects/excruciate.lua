require("modifiers")
require("battleutils")

--Gradually increases critical rate of spells
function onTick(owner, effect, actionContainer)
    --No clue how fast the crit rate increases or how often it ticks
    --Only clue I have to go on is that the strategy seemed to be to use it 
    --before or after fire/thunder and you'd usually get a crit at firaga/thundaga
    --Random guess, going to assume it's 25 crit rating every 3s, 50 crit rating traited
    --That's 4% and 8% every 3 seconds of actual crit
    local ratePerTick = 25;

    if effect.GetTier() == 2 then
        ratePerTick = 50;
    end
    
    effect.SetMagnitude(effect.GetMagnitude() + ratePerTick);
end

--Excruciate seems to have an effect on all hits of aoe spells, so it's changing the crit bonus of the skill itself
--rather than on a hit by hit basis
function onCommandStart(effect, owner, skill, actionContainer)
    skill.bonusCritRate = skill.bonusCritRate + effect.GetMagnitude();
end

function onCrit(effect, attacker, defender, skill, action, actionContainer)
    if skill.GetCommandType() == CommandType.Spell then
        attacker.statusEffects.RemoveStatusEffect(effect, actionContainer, 30331, false);
    end
end