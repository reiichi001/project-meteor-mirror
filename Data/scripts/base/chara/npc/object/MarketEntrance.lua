--[[

MarketEntrance Script

Functions:

Parameters mostly rely on the xtx_placeName sheet for its strings.

eventPushChoiceAreaOrQuest(
    exitPlaceName[Fronds, etc],     - Retail only showed it when inside a Market Ward/Office  Set to 0 to hide the menu.
    showMarketWards/Houses          - If > 0, client script adds nation-specific Mercentile Houses as well.
    gcHQPlaceName,                  - Set to the placeName id for the Grand Company office of that city
    questAreaName,                  - Set to the placeName id of applicable quest instance, ex. Sailors Ward.
    showItemSearchCounter,          - If true, shows the Item Search menu
    itemSearchId                    - If > 0 & showItemSearchCounter = true, displays the item name with a "Stop Searching"
)
eventPushStepPrvMarket(
    staringWard,                    - Sets the starting placeName id
    wardCount,                      - Valid number 1-20. Sets the amount of market ward entries. Client continues sequentially from startingWard id.
    excludeWard                     - Hides the ward in the list that matches the id.  Use on the ward you're currently in.
) 
  
  
MarketEntrance City TriggerBox details
Limsa    - !warp 230 -416.5 40 446              ActorClass Id = 1090238
    bgObj  Id - [0xB3B] 2875
    Layout Id - [0x79 ] 121  (0x29d90001)
    Condition - in
    reactName - dwti    - Not a typo compared to the other cities
Gridania - !warp 206 -192.57 23.48 -1407.58     ActorClass Id = 1090264

    bgObj  Id - [0xCFA] 3322
    Layout Id - [0x141]  321 (0x29b00001)
    Condition - in
    reactName - dtwi
Ul'dah   - !warp 175 -235 189 50.5              ActorClass Id = 1500394
    bgObj  Id - [0x102F] 4143
    Layout Id - [0x1A5] 421  (0x615a0001)
    Condition - in
    reactName - dtwi

--]]

require ("global")

function init(npc)
	return false, false, 0, 0;
end


CITY_INFO = { -- wardPlaceName, exitPlaceName, gcHQPlaceName, questAreaName, wardListStart, wardListCount
    {1093, 1087, 1512, 1091, 1261, 20}, -- Limsa
    {2099, 2091, 2526, 2095, 2261, 20}, -- Gridania
    {3098, 3091, 3514, 3095, 3261, 20}, -- Ul'dah
}

-- TO-DO:  Add some X/Z pos jitter to Entrances/Exits when called
MARKETWARD_ENTRANCE = {
    {134, 160, 0, 135}, -- Limsa Market
    {160, 160, 0, 138}, -- Gridania Market
    {180, 160, 0, 185}  -- Ul'dah Market
}

MARKETWARD_EXIT = {
    {230, -420, 41, 435, -3.14},  -- Educated guess for Limsa, need video reference to confirm
    {206, -180, 22, -1408, 1.5},
    {175, -210, 190, 25, 0.65}
}

GC_ENTRANCE = { 
    [1512] = {232, 160, 0, -155}, -- Maelstrom Command
    [2526] = {234, 160, 0, -155}, -- Adders' Nest
    [3514] = {233, 160, 0, -155}  -- Hall of Flames
}

city = {
    [1090238] = 1, -- Limsa Market Ward Entrance
    [1090264] = 2, -- Gridania Market Ward Entrance
    [1090265] = 3, -- Ul'dah Market Ward Entrance
    [1500392] = 1, -- Limsa     : M'septha
    [1500393] = 2, -- Gridania  : Torsefers
    [1500394] = 3, -- Ul'dah    : Edine
}



function onEventStarted(player, npc, triggerName)	

    local npcCity = city[npc:GetActorClassId()] or 1;
    local wardPlaceName = CITY_INFO[npcCity][1];        -- Market Wards category name. Identical in all languages except Japanese
    local exitPlaceName = CITY_INFO[npcCity][2];        -- Central Limsa Lominsa / Heartstream / The Fronds
    local gcHQPlaceName = CITY_INFO[npcCity][3];        -- Maelstrom Command / Adders' Nest / Hall of Flames
    local questAreaName = 0; --CITY_INFO[npcCity][4];   -- Sailors Ward / Peasants Ward / Merchants Ward
    local wardListStart = CITY_INFO[npcCity][5];        -- Starting id for the market wards
    local wardListCount = CITY_INFO[npcCity][6];        -- Amount of wards in the list
    local showItemSearchCounter = false;
    local itemSearchId = 11000125;   
    
    local worldMaster = GetWorldMaster(); 
    local pos = player:GetPos();
    local currZone = pos[4];
    
    if (currZone == 133 or currZone == 230 or currZone == 155 or currZone == 206 or currZone == 175 or currZone == 209) then 
        exitPlaceName = 0;  -- If in city, hide city menu option
    elseif (currZone == 232 or currZone == 234 or currZone == 233) then 
        gcHQPlaceName = 0;  -- If in GC Office, hide office menu option
    end

    choice = callClientFunction(player, "eventPushChoiceAreaOrQuest", exitPlaceName, wardPlaceName, gcHQPlaceName, questAreaName, showItemSearchCounter, itemSearchId);
        
    while (true) do
        
        if choice == wardPlaceName then -- Market Wards
            wardSelect = callClientFunction(player, "eventPushStepPrvMarket", wardListStart, wardListCount, 0);
            
            if wardSelect and (wardSelect >= wardListStart and wardSelect <= (wardListStart+wardListCount)) then
                player:SendGameMessage(player, worldMaster, 60004, 0x20, wardSelect);
                warp = MARKETWARD_ENTRANCE[npcCity];
                playerRot = math.random(-3.14, 3.14);
                wait(1);
                GetWorldManager():DoZoneChange(player, warp[1], nil, 0, 0x02, warp[2], warp[3], warp[4], playerRot);
                player:SendDataPacket("attention", worldMaster, "", 60003, wardSelect);
                -- Temp: Pop-up display after Ward zone-in.  Client should automate this with PrivateArea's properly setup

                break;
            end
            
        elseif (choice == 1519 or choice == 2534 or choice == 3533) then -- Mercentile Wards
                player:SendMessage(0x20, "", "[MarketEntrance] DEBUG: "..choice);
        elseif (choice == 1512 or choice == 2526 or choice == 3514) then -- GC Office
                warp = GC_ENTRANCE[choice];
                player:SendGameMessage(player, worldMaster, 60004, 0x20, choice);
                wait(1);
                GetWorldManager():DoZoneChange(player, warp[1], nil, 0, 0x02, warp[2], warp[3], warp[4], math.pi);
                break;
        elseif (choice == 1087 or choice == 2091 or choice == 3091) then -- Exiting to City
                player:SendGameMessage(player, worldMaster, 60004, 0x20, choice);
                warp = MARKETWARD_EXIT[npcCity];
                wait(1);
                GetWorldManager():DoZoneChange(player, warp[1], nil, 0, 0x02, warp[2], warp[3], warp[4], warp[5]);          
                break;
        elseif (choice == 0 or choice == -3) then -- Menu Closed
            break;  
        end 
        
        choice = callClientFunction(player, "eventPushChoiceAreaOrQuest", exitPlaceName, wardPlaceName, gcHQPlaceName, questAreaName, showItemSearchCounter, itemSearchId);
          
    end
    
	player:EndEvent();	
    
end