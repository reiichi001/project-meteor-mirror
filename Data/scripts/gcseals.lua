--[[

Grand Company Seal Helper Functions

--]]
require("global");

local companySeal = {1000201, 1000202, 1000203}; -- Storm, Serpent, Flame
local rankSealCap = {
    [0] = 0,         -- None   
    [11] = 10000,    -- Private Third Class   
    [13] = 15000,    -- Private Second Class
    [15] = 20000,    -- Private First Class
    [17] = 25000,    -- Corporal
    [21] = 30000,    -- Sergeant Third Class
    [23] = 35000,    -- Sergeant Second Class
    [25] = 40000,    -- Sergeant First Class
    [27] = 45000,    -- Chief Sergeant
    [31] = 50000,    -- Second Lieutenant
    [33] = 50000,    -- First Lieutenant
    [35] = 50000,    -- Captain
    [41] = 60000,    -- Second Commander
    [43] = 60000,    -- First Commander
    [45] = 60000,    -- High Commander
    [51] = 70000,    -- Rear Marshal
    [53] = 70000,    -- Vice Marshal
    [55] = 70000,    -- Marshal
    [57] = 70000,    -- Grand Marshal
    [100] = 100000,  -- Champion
    [111] = 0,       -- Chief Admiral/Elder Seedseer/General
    [127] = 10000    -- Recruit
}
       
function GetGCSeals(player, company)
    company = tonumber(company);
    
    if company ~= nil and company > 0 and company < 4 then
        return player:GetItemPackage(INVENTORY_CURRENCY):GetItemQuantity(companySeal[tonumber(company)]);
    else
        return -1;
    end
end

function AddGCSeals(player, company, amount)
    amount = tonumber(amount);
    company = tonumber(company);   
    
    local gcRank = {player.gcRankLimsa, player.gcRankGridania, player.gcRankUldah};
    local currentAmount = GetGCSeals(player, company);
    local maxAmount = rankSealCap[gcRank[company]];  

    if currentAmount ~= -1 then
        if amount then
            if currentAmount + amount <= maxAmount then
               invCheck = player:GetItemPackage(INVENTORY_CURRENCY):AddItem(companySeal[company], amount, 1);
                if invCheck == INV_ERROR_SUCCESS then
                    return INV_ERROR_SUCCESS;
                end
            else
               return INV_ERROR_FULL;
            end
        end  
    else
        return INV_ERROR_SYSTEM_ERROR;
    end
end