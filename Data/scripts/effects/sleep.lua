require("modifiers")

--Set magnitude to milliseconds that HF will reduce delay by
function onGain(owner, effect, actionContainer)
end;

function onLose(owner, effect, actionContainer)
end;

function onDamageTaken(effect, attacker, defender, skill, action, actionContainer)
    defender.statusEffects.RemoveStatusEffect(effect, actionContainer)
end;