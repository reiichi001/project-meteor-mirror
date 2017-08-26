require("global")
messageId = MESSAGE_TYPE_SYSTEM_ERROR;
sender = "regen";

function onGain(target, effect)
    messageId = MESSAGE_TYPE_SYSTEM_ERROR;
    sender = "regen";

end;

function onTick(target, effect)
    messageId = MESSAGE_TYPE_SYSTEM_ERROR;
    sender = "regen";

    -- todo: actual regen effect thing
    local ability = GetWorldManager().GetAbility(27346);
    local anim = bit32.bxor(bit32.lshift(ability.animationType, 24), bit32.lshift(tonumber(1), 12) , 101); 
    local addHp = effect.GetMagnitude();
    
    target.AddHP(addHp);
    --   target.SendBattleActionX01Packet(anim, 101, 0, 0, addHp);
end;

function onLose(target, effect)
    messageId = MESSAGE_TYPE_SYSTEM_ERROR;
    sender = "regen";
end;