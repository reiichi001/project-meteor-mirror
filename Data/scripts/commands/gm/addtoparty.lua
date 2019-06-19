require("global");

properties = {
    permissions = 0,
    parameters = "sss",
    description =
[[
Adds target to party
]]
}

function onTrigger(player, argc)
    local sender = "[addtoparty] ";
    
    if player then
        if player.target then
            print("hi")
            local id = player.target.actorId
            print("hi")
            player.currentParty:AddMember(id);
            player.target.currentParty = player.currentParty;
            print("hi")
        else
            print(sender.." no target")
        end
    else
        print(sender.." no player");
    end;
end;