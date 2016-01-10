using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.dataobjects.chara
{
    class PlayerWork
    {
        public byte tribe;
        public byte guardian;
        public byte birthdayMonth;
        public byte birthdayDay;
        public byte initialTown;

        public int restBonusExpRate;

        public uint[] questScenario = new uint[16];
        public uint[] questGuildLeve = new uint[8];

        public int questScenarioComplete;
        public int questGuildleveComplete;

        public bool isContentsCommand;

        public int castCommandClient;
        public int castEndClient;

        public int[] comboNextCommandId = new int[2];
        public int comboCostBonusRate;

        public bool isRemainBonusPoint;

        public bool[] npcLinkshellChatCalling = new bool[64];
        public bool[] npcLinkshellChatExtra = new bool[64];

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
