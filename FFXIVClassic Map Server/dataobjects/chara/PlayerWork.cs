using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.dataobjects.chara
{
    class PlayerWork
    {
        public int tribe;
        public int guardian;
        public int birthdayMonth;
        public int birthdayDay;
        public int initialTown;

        public int restBonusExpRate;

        public int[] questScenario = new int[16];
        public int[] questGuildLeve = new int[8];

        public int questScenarioComplete;
        public int questGuildleveComplete;

        public bool isContentsCommand;

        public int castCommandClient;
        public int castEndClient;

        public int[] comboNextCommandId = new int[2];
        public int comboCostBonusRate;

        public bool isRemainBonusPoint;

        public int[] npcLinkshellChatCalling = new int[64];
        public int[] npcLinkshellChatExtra = new int[64];

        public int variableCommandConfirmWarp;
        public int variableCommandConfirmWarpSender;
        public int variableCommandConfirmWarpSenderByID;
        public int variableCommandConfirmWarpSenderSex;
        public int variableCommandConfirmWarpPlace;

        public int variableCommandConfirmRaise;
        public int variableCommandConfirmRaiseSender;
        public int variableCommandConfirmRaiseSenderByID;
        public int variableCommandConfirmRaiseSenderSex;
        public int variableCommandConfirmRaisePlace;
        
    }
}
