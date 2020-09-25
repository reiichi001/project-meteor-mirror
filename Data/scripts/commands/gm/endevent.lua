require("global");

properties = {
    permissions = 0,
    parameters = "ss",
    description =
[[
Passes endEvent() to player or <targetname> to close a script.
!endevent |
!endevent <targetname> |
]],
}

function onTrigger(player, argc, name, lastName)
    local sender = "[endevent] ";
    
    if name then
        if lastName then
            player = GetWorldManager():GetPCInWorld(name.." "..lastName) or nil;
        else
            player = GetWorldManager():GetPCInWorld(name) or nil;
        end;
    end;
       
        local messageID = MESSAGE_TYPE_SYSTEM_ERROR;
        local message = "Sending endEvent()";
    
    if player then
    	player:endEvent();
        player:SendMessage(messageID, sender, message);
        print(message);
    else
        print(sender.."Sending Event.");
    end;
end;