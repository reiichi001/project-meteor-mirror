require("modifiers")

--Add 30 raw block rate. No idea how much block it actually gives.
function onGain(owner, effect, actionContainer)
    owner.AddMod(modifiersGlobal.RawBlockRate, 30);
end

function onLose(owner, effect, actionContainer)
    owner.SubtractMod(modifiersGlobal.RawBlockRate, 30);
end

--Gives 200 TP on block. Traited: Gives 10% of the amount blocked back as MP
function onBlock(effect, attacker, defender, skill, action, actionContainer)
    --200 TP on block
    defender.AddTP(200);

    --If traited, add 10% of damage taken as MP
    if(effect.GetTier() == 2) then
        local mpToReturn = math.ceil(0.10 * action.amount);
        defender.AddMP(math.ceil(mpToReturn));
        --33009: You recover x MP from Outmaneuver
        actionContainer.AddMPAction(defender.actorId, 33009, mpToReturn);
    end
end;