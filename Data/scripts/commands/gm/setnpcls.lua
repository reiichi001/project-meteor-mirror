require("global");

properties = {
    permissions = 0,
    parameters = "is",
    description = "Sets a NPC LS' state. Args: npcLSId, <gone|inactive|active|alert>",
}

function onTrigger(player, argc, lsId, state)
    local id = tonumber(lsId) or 0;
	
	if (state == "alert") then	
		player:SetNpcLS(id, NPCLS_ALERT);
	elseif (state == "active") then
		player:SetNpcLS(id, NPCLS_ACTIVE);
	elseif (state == "inactive") then
		player:SetNpcLS(id, NPCLS_INACTIVE);
	elseif (state == "gone") then
		player:SetNpcLS(id, NPCLS_GONE);
	else
		player:SendMessage(0x20, "", "Invalid state argument");
		return;
	end	
	
	player:SendMessage(0x20, "", string.format("NPC LS %d set to %s", id, state));
	
end