require("global");

properties = {
    permissions = 0,
    parameters = "is",
    description = "Sets a NPC LS' state. Args: npcLSId, <gone|inactive|active|alert>",
}

function onTrigger(player, argc, lsId, state)
    local id = tonumber(lsId) or 0;
	
	if (state == "alert") then	
		player:SetNpcLS(id, true, true);
	elseif (state == "active") then
		player:SetNpcLS(id, true, false);
	elseif (state == "inactive") then
		player:SetNpcLS(id, false, true);
	elseif (state == "gone") then
		player:SetNpcLS(id, false, false);
	else
		player:SendMessage(0x20, "", "Invalid state argument");
		return;
	end	
	
	player:SendMessage(0x20, "", string.format("NPC LS %d set to %s", id, state));
	
end