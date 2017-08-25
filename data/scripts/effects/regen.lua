require("global")
messageId = MESSAGE_TYPE_SYSTEM_ERROR;
sender = "regen";

function onGain(target, effect)
    messageId = MESSAGE_TYPE_SYSTEM_ERROR;
    sender = "regen";

    target.SendMessage(messageId, sender, "dicks");
end;

function onTick(target, effect)
    messageId = MESSAGE_TYPE_SYSTEM_ERROR;
    sender = "regen";

    local ability = GetWorldManager().GetAbility(27346);
    local anim = bit32.bxor(bit32.lshift(ability.animationType, 24), bit32.lshift(tonumber(1), 12) , 101); 
    local addHp = effect.GetMagnitude();
    
    target.AddHP(addHp);
    target.SendBattleActionX01Packet(anim, 101, 0, 0, addHp);
    target.SendMessage(messageId, sender, string.format("ate %u dicks", addHp));
end;

function onLose(target, effect)
    messageId = MESSAGE_TYPE_SYSTEM_ERROR;
    sender = "regen";

    target.SendMessage(messageId, sender, "dicks gon");
end;