require("global");

properties = {
    permissions = 0,
    parameters = "s",
    description =
[[
Changes appearance for equipment with given parameters.
!graphic <slot> <wID> <eID> <vID> <vID>
]],
}

function onTrigger(player, argc, state)
    local messageID = MESSAGE_TYPE_SYSTEM_ERROR;
    local sender = "[setstate] ";
    
    max = tonumber(state) or 0;
    
    for s = 0, max do
        if player and player.target then
            player.target:ChangeState(s);
			wait(0.8);
			player:SendMessage(0x20, "", "state: "..s);
        end;
    end
           
end;