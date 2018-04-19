require("global");

properties = {
    permissions = 0,
    parameters = "sss",
    description =
[[
Adds experience <qty> to player or <targetname>.
!giveexp <qty> |
!giveexp <qty> <targetname> |
]],
}

function onTrigger(player, argc, jobId)
    local sender = "[setjob] ";
    
    jobId = tonumber(jobId)
    if player then
        player:SetCurrentJob(jobId);
    end;
end;