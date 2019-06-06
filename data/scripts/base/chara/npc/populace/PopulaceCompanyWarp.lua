--[[

PopulaceCompanyWarp Script

Functions:

eventTalkWelcome(player) - Start Text
eventAskMainMenu(player, index) - Shows teleport menu, hides the teleport location at index value to prevent warping to the spot you're at
eventAfterWarpOtherZone(player) - Fades out for warp
eventTalkStepBreak() - Ends talk
--]]

require ("global")

warpNpc =
{ --[actorId] = {warpIndex, cityId}   -- ()s around name indicate missing NPC + Aethernet
    [1500321] = {1, 1}, -- (Storm Private Gardner)      
    [1500331] = {2, 1}, -- (Storm Private Rich)
    [1500323] = {3, 1}, -- (Storm Private Potter)
    [1500330] = {4, 1}, -- (Storm Private Hunt)
    [1500322] = {5, 1}, -- (Storm Private Abel)
    [1500332] = {6, 1}, -- (Storm Private Stone)
    [1500339] = {7, 1}, -- (Storm Private Holt)
    [1500324] = {1, 2}, -- serpent_private_white
    [1500334] = {2, 2}, -- serpent_private_hill  
    [1500326] = {3, 2}, -- serpent_private_carver    
    [1500333] = {4, 2}, -- serpent_private_stone
    [1500325] = {5, 2}, -- serpent_private_holmes
    [1500335] = {6, 2}, -- serpent_private_kirk
    [1500327] = {1, 3}, -- flame_private_newton
    [1500337] = {2, 3}, -- (Flame Private Tanner)
    [1500329] = {3, 3}, -- (Flame Private Morning)
    [1500336] = {4, 3}, -- (Flame Private Covey)
    [1500328] = {5, 3}, -- flame_private_allen    
    [1500338] = {6, 3}, -- (Flame Private Yar)
}

aethernet = 
{
    {   -- 1: Limsa
        {zone = 230, x = -424.140, y = 42.000, z = 371.988, r = -2.472},        -- 1 - Aetheryte Plaza
        {zone = 133, x = -439.744, y = 40.000, z = 234.376, r = 0.287},         -- 2 - Drowning Wench
        {zone = 230, x = -498.131, y = 43.622, z = 60.818,  r = 0.254},         -- 3 - The Bismarck
        {zone = 230, x = -759.331, y = 12.000, z = 239.413, r = -0.869},        -- 4 - Ferry Docks  
        {zone = 230, x = -623.582, y = 4.000,  z = 369.318, r = 1.736},         -- 5 - Fisherman's Bottom        
        {zone = 230, x = -525.536, y = 18.000, z = 173.735, r = 3.082},         -- 6 - The Octant
        {zone = 133, x = -231.711, y = 12.000, z = 193.573, r = -0.786},        -- 7 - Procession of Terns
        {zone = 128, x = -20.783,  y = 42.214, z = 146.946, r = 2.046},         -- 8 - Zephyr Gate
    },
    {   -- 2: Gridania
        {zone = 206, x = -107.878,  y = 17.524, z = -1343.871, r = 0.657},      -- 1 - Aetheryte Plaza
        {zone = 155, x =  96.868,   y = 3.480,  z = -1211.040, r = 2.582},      -- 2 - Carline Canopy
        {zone = 206, x =  86.942,   y = 19.789, z = -1420.891, r = 2.965},      -- 3 - Atelier Fen-Yil
        {zone = 206, x =  -84.621,  y = 19.061, z = -1502.665, r = 0.756},      -- 4 - Whistling Miller
        {zone = 206, x =  205.101,  y = 9.526,  z = -1245.405, r = -1.749},     -- 5 - Quiver's Hold
        {zone = 206, x =  160.578,  y = 25.061, z = -1556.662, r = 1.896},      -- 6 - Wailing Barracks
        {zone = 150, x = 318.838,   y = 4.036,  z = -992.071,  r = -0.307},     -- 7 - Mistalle Bridges
        {zone = 206, x = -192.167,  y = 4.466,  z = -1061.777, r = -0.026},     -- 8 - Berlends Bridges
    },
    {   -- 3: Ul'dah
        {zone = 175, x = -190.574,  y = 190.000, z = 18.086,  r = 2.190},       -- 1 - Aetheryte Plaza
        {zone = 175, x = -36.513,   y = 192.000, z = 37.130,  r = -0.490},      -- 2 - Quicksand
        {zone = 209, x = -192.971,  y = 230.000, z = 209.348, r = 2.860},       -- 3 - Frondale's Phrontistery
        {zone = 209, x = -60.243,   y = 200.000, z = 257.718, r = -1.276},      -- 4 - Onyx Lane
        {zone = 209, x = -147.633,  y = 198.000, z = 160.064, r = -1.600},      -- 5 - Gold Court
        {zone = 209, x = -263.776,  y = 202.000, z = 206.699, r = -3.135},      -- 6 - Arrzaneth Ossuary
        {zone = 170, x = -29.721,   y = 182.635, z = -76.313, r = 2.625},       -- 7 - Gate of Nald
        {zone = 170, x = 129.957,   y = 183.862, z = 220.719, r = 1.515},       -- 8 - Gate of Thal
    }     
}

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
    local passLimsa = 2001014;
    local passGrid  = 2001015;
    local passUldah = 2001016;
    passCheck = 1;  -- 0 = Check player for Aetherpass keyitem.  1 = Ignore it.
    
    npcId = npc:GetActorClassId();
    city = warpNpc[npcId][2];
    
    
    if city == 1 then
        if player:GetItemPackage(INVENTORY_KEYITEMS):HasItem(passLimsa) then
            passCheck = 1;
        else
            if passCheck == 0 then callClientFunction(player, "eventTalkWelcome", player); end
        end;
    elseif city == 2 then
        if player:GetItemPackage(INVENTORY_KEYITEMS):HasItem(passGrid) then
            passCheck = 1;
        else
           if passCheck == 0 then callClientFunction(player, "eventTalkWelcome", player); end
        end;
    elseif city == 3 then
        if player:GetItemPackage(INVENTORY_KEYITEMS):HasItem(passUldah) then
            passCheck = 1;
        else
            if passCheck == 0 then callClientFunction(player, "eventTalkWelcome", player); end
        end
    end
    
    if passCheck == 1 then
        choice = callClientFunction(player, "eventAskMainMenu", player, warpNpc[npcId][1]);
    
        if choice == 0 then
            --callClientFunction(player, "playereventTalkStepBreak");
            player:EndEvent();
        else
         --   callClientFunction(player, "eventAfterWarpOtherZone", player);   -- Commented out for now to prevent double fade-to-black for warp
           player:EndEvent();
           GetWorldManager():DoZoneChange(player, aethernet[city][choice].zone, nil, 0, 15, aethernet[city][choice].x, aethernet[city][choice].y, aethernet[city][choice].z, aethernet[city][choice].r);    
        end
    end
    
    player:EndEvent();
end
