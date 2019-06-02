require("global");
require("modifiers");
properties = {
    permissions = 0,
    parameters = "ssss",
    description = 
[[
yolo
]],
}

local quests = 
{
    [111807] = { level = 25, weight = 4, rewardexp = 1080 },
    [110868] = { level = 50, weight = 4, rewardexp = 4400 },
    [111603] = { level = 22, weight = 5, rewardexp = 1100 },
    [111602] = { level = 22, weight = 5, rewardexp = 1100 },
    [111420] = { level = 45, weight = 5, rewardexp = 4450 },
    [110811] = { level = 18, weight = 6, rewardexp = 780 },
    [110814] = { level = 18, weight = 6, rewardexp = 780 },
    [110707] = { level = 25, weight = 6, rewardexp = 1620 },
    [110682] = { level = 34, weight = 6, rewardexp = 3180 },
    [111202] = { level = 35, weight = 6, rewardexp = 3360 },
    [111222] = { level = 35, weight = 6, rewardexp = 3360 },
    [111302] = { level = 35, weight = 6, rewardexp = 3360 },
    [111223] = { level = 40, weight = 6, rewardexp = 4260 },
    [110819] = { level = 45, weight = 6, rewardexp = 5340 },
    [111224] = { level = 45, weight = 6, rewardexp = 5340 },
    [111225] = { level = 45, weight = 6, rewardexp = 5340 },
    [110867] = { level = 45, weight = 6, rewardexp = 5340 },
    [110869] = { level = 45, weight = 6, rewardexp = 5340 },
    [110708] = { level = 45, weight = 6, rewardexp = 5340 },
    [110627] = { level = 45, weight = 6, rewardexp = 5340 },
    [111434] = { level = 50, weight = 6, rewardexp = 6600 },
    [110850] = { level = 1, weight = 7, rewardexp = 40 },
    [110851] = { level = 1, weight = 7, rewardexp = 40 },
    [110841] = { level = 20, weight = 7, rewardexp = 1120 },
    [110642] = { level = 20, weight = 7, rewardexp = 1120 },
    [110840] = { level = 20, weight = 7, rewardexp = 1120 },
    [110727] = { level = 21, weight = 7, rewardexp = 1401 },
    [111221] = { level = 30, weight = 7, rewardexp = 2661 },
    [111241] = { level = 30, weight = 7, rewardexp = 2661 },
    [110687] = { level = 28, weight = 9, rewardexp = 2970 },
    [110016] = { level = 34, weight = 50, rewardexp = 26500 },
    [110017] = { level = 38, weight = 50, rewardexp = 32500 },
    [110019] = { level = 46, weight = 50, rewardexp = 46000 }
};

local expTable = {
    570, -- 1
    700,
    880,
    1100,
    1500,
    1800,
    2300,
    3200,
    4300,
    5000, -- 10
    5900,
    6800,
    7700,
    8700,
    9700,
    11000,
    12000,
    13000,
    15000,
    16000, -- 20
    20000,
    22000,
    23000,
    25000,
    27000,
    29000,
    31000,
    33000,
    35000,
    38000, -- 30
    45000,
    47000,
    50000,
    53000,
    56000,
    59000,
    62000,
    65000,
    68000,
    71000, -- 40
    74000,
    78000,
    81000,
    85000,
    89000,
    92000,
    96000,
    100000,
    100000,
    110000 -- 50
};

local commandCost = {
    ["raise"] = 150,
    ["cure"] = 40,
    ["cura"] = 100,
    ["curaga"] = 150,
};
-- stone:   (1, 9) (5, 12) (10, )
-- cure:    (1, 5) (5, 6)  (10, )
-- aero:    (1, 9) (5, 12) (10, )
-- protect: (1, 9) (5, 12) (10, )
--[[
function onTrigger(player, argc, id, level, weight)
    id = tonumber(id) or 111807;
    level = tonumber(level) or quests[id].level;
    weight = tonumber(weight) or quests[id].weight;
    local messageId = MESSAGE_TYPE_SYSTEM_ERROR;
    local sender = "yolo";

    if id == 1 then
	    return
    end
    local message = calcSkillPoint(player, level, weight);
    if player then
        player.SendMessage(messageId, sender, string.format("calculated %s | expected %s", message, quests[id].rewardexp));
    end;
    printf("calculated %s | expected %s", message, quests[id].rewardexp);
end;
]]



function onTrigger(player, argc, width, height, blockCount)
    local messageId = MESSAGE_TYPE_SYSTEM_ERROR;
    local sender = "yolo";
    
    if player then
        if false then
            local effectId = 223004;
            
            player.statusEffects.RemoveStatusEffect(effectId);
            player.statusEffects.AddStatusEffect(effectId, 1, 0, 5);
            return;
        end;
        
        local pos = player:GetPos();
        local x = tonumber(pos[0]);
        local y = tonumber(pos[1]);
        local z = tonumber(pos[2]);
        local rot = tonumber(pos[3]);
        local zone = pos[4];
        local w = tonumber(width) or 0;

        local h = tonumber(height) or 0;
        local blocks = tonumber(blockCount) or 0;

        printf("%f %f %f", x, y, z);
        --local x, y, z = player.GetPos();
        for b = 0, blocks do
            for i = 0, w do
                for j = 0, h do
                    local actor = player.GetZone().SpawnActor(2104001, 'ass', x + (i * 1), y, z + (j * 1), rot, 0, 0, true);
                    actor.ChangeNpcAppearance(2200905);
                    actor.SetMaxHP(5000);
                    actor.SetHP(5000);
                    actor.SetMod(modifiersGlobal.CanBlock, 1);
                    actor.SetMod(modifiersGlobal.AttackRange, 3);
                    actor.SetMod(modifiersGlobal.MovementSpeed, 5);
                    actor.SetMobMod(mobModifiersGlobal.Roams, 1);
					actor.SetMobMod(mobModifiersGlobal.RoamDelay, 10);
					actor.charaWork.parameterSave.state_mainSkillLevel = 52;
                    actor.moveState = 3;
                end 
            end

            x = x + 500
        end
        return;
    end
end;

function calculateCommandCost(player, skillName, level)
  if skillName and level and commandCost[skillName] then
    return math.ceil((8000 + (level - 70) * 500) * (commandCost[skillName] * 0.001));
  end;
  return 1;
end

function calcSkillPoint(player, lvl, weight)
  weight = weight / 100

  return math.ceil(expTable[lvl] * weight)
end