require("global");

properties = {
    permissions = 0,
    parameters = "ssss",
    description = 
[[
Adds currency <qty> to player or <targetname>
Defaults to gil if no item entered
!givecurrency <item> <qty> |
!givecurrency <item> <qty> <targetname> |
]],
}

currencyItems = {
["GIL"] = 1000001,
["FIRE_SHARD"] = 1000003,
["ICE_SHARD"] = 1000004,
["WIND_SHARD"] = 1000005,
["EARTH_SHARD"] = 1000006,
["LIGHTNING_SHARD"] = 1000007,
["WATER_SHARD"] = 1000008,
["FIRE_CRYSTAL"] = 1000009,
["ICE_CRYSTAL"] = 1000010,
["WIND_CRYSTAL"] = 1000011,
["EARTH_CRYSTAL"] = 1000012,
["LIGHTNING_CRYSTAL"] = 1000013,
["WATER_CRYSTAL"] = 1000014,
["FIRE_CLUSTER"] = 1000015,
["ICE_CLUSTER"] = 1000016,
["WIND_CLUSTER"] = 1000017,
["EARTH_CLUSTER"] = 1000018,
["LIGHTNING_CLUSTER"] = 1000019,
["WATER_CLUSTER"] = 1000020,
["PUGILISTS_GUILD_MARK"] = 1000101,
["GLADIATORS_GUILD_MARK"] = 1000102,
["MARAUDERS_GUILD_MARK"] = 1000103,
["ARCHERS_GUILD_MARK"] = 1000106,
["LANCERS_GUILD_MARK"] = 1000107,
["THAUMATURGES_GUILD_MARK"] = 1000110,
["CONJURERS_GUILD_MARK"] = 1000111,
["CARPENTERS_GUILD_MARK"] = 1000113,
["BLACKSMITHS_GUILD_MARK"] = 1000114,
["ARMORERS_GUILD_MARK"] = 1000115,
["GOLDSMITHS_GUILD_MARK"] = 1000116,
["LEATHERWORKERS_GUILD_MARK"] = 1000117,
["WEAVERS_GUILD_MARK"] = 1000118,
["ALCHEMISTS_GUILD_MARK"] = 1000119,
["CULINARIANS_GUILD_MARK"] = 1000120,
["MINERS_GUILD_MARK"] = 1000121,
["BOTANISTS_GUILD_MARK"] = 1000122,
["FISHERMENS_GUILD_MARK"] = 1000123,
["STORM_SEAL"] = 1000201,
["SERPENT_SEAL"] = 1000202,
["FLAME_SEAL"] = 1000203,

["FIRESHARD"] = 1000003,
["ICESHARD"] = 1000004,
["WINDSHARD"] = 1000005,
["EARTHSHARD"] = 1000006,
["LIGHTNINGSHARD"] = 1000007,
["WATERSHARD"] = 1000008,
["FIRECRYSTAL"] = 1000009,
["ICECRYSTAL"] = 1000010,
["WINDCRYSTAL"] = 1000011,
["EARTHCRYSTAL"] = 1000012,
["LIGHTNINGCRYSTAL"] = 1000013,
["WATERCRYSTAL"] = 1000014,
["FIRECLUSTER"] = 1000015,
["ICECLUSTER"] = 1000016,
["WINDCLUSTER"] = 1000017,
["EARTHCLUSTER"] = 1000018,
["LIGHTNINGCLUSTER"] = 1000019,
["WATERCLUSTER"] = 1000020,
["PUGILISTSGUILDMARK"] = 1000101,
["GLADIATORSGUILDMARK"] = 1000102,
["MARAUDERSGUILDMARK"] = 1000103,
["ARCHERSGUILDMARK"] = 1000106,
["LANCERSGUILDMARK"] = 1000107,
["THAUMATURGESGUILDMARK"] = 1000110,
["CONJURERSGUILDMARK"] = 1000111,
["CARPENTERSGUILDMARK"] = 1000113,
["BLACKSMITHSGUILDMARK"] = 1000114,
["ARMORERSGUILDMARK"] = 1000115,
["GOLDSMITHSGUILDMARK"] = 1000116,
["LEATHERWORKERSGUILDMARK"] = 1000117,
["WEAVERSGUILDMARK"] = 1000118,
["ALCHEMISTSGUILDMARK"] = 1000119,
["CULINARIANSGUILDMARK"] = 1000120,
["MINERSGUILDMARK"] = 1000121,
["BOTANISTSGUILDMARK"] = 1000122,
["FISHERMENSGUILDMARK"] = 1000123,
["STORMSEAL"] = 1000201,
["SERPENTSEAL"] = 1000202,
["FLAMESEAL"] = 1000203,
}

function onTrigger(player, argc, item, qty, name, lastName)
    local sender = "[givecurrency] ";
    local messageID = MESSAGE_TYPE_SYSTEM_ERROR;
    local worldMaster = GetWorldMaster(); 
     
    if name then
        if lastName then
            player = GetWorldManager():GetPCInWorld(name.." "..lastName) or nil;
        else
            player = GetWorldManager():GetPCInWorld(name) or nil;
        end;
    end;
    
    if player then
        if not currencyItems[string.upper(item)] then
            player:SendMessage(messageID, sender, "Invalid parameter for item.");
            return;
        else
            item = currencyItems[string.upper(item)];
        end                
        
        qty = tonumber(qty) or 1;
        location = INVENTORY_CURRENCY;

        local invCheck = player:getInventory(location):AddItem(item, qty, 1);
        
        if (invCheck == INV_ERROR_FULL) then
            -- Your inventory is full.
            player:SendGameMessage(player, worldMaster, 60022, messageID);
        elseif (invCheck == INV_ERROR_ALREADY_HAS_UNIQUE) then
            -- You cannot have more than one <itemId> <quality> in your possession at any given time.
            player:SendGameMessage(player, worldMaster, 40279, messageID, item, 1);
        elseif (invCheck == INV_ERROR_SYSTEM_ERROR) then
            player:SendMessage(MESSAGE_TYPE_SYSTEM, "", "[DEBUG] Server Error on adding item.");
        elseif (invCheck == INV_ERROR_SUCCESS) then
            message = string.format("Added item %s to location %s to %s", item, location, player:GetName());
            player:SendMessage(MESSAGE_TYPE_SYSTEM, "", message);
            player:SendGameMessage(player, worldMaster, 25246, messageID, item, qty);
        end
    end
end;