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

        public float restBonusExpRate = 1.5f;

        public uint[] questScenario = new uint[16];
        public uint[] questGuildleve = new uint[8];

        public bool[] questScenarioComplete = new bool[2048];
        public bool[] questGuildleveComplete = new bool[2048];

        public bool isContentsCommand;

        public int castCommandClient;
        public int castEndClient;

        public int[] comboNextCommandId = new int[2];
        public float comboCostBonusRate;

        public bool isRemainBonusPoint;

        public bool[] npcLinkshellChatCalling = new bool[64];
        public bool[] npcLinkshellChatExtra = new bool[64];

        public int variableCommandConfirmWarp;
        public string variableCommandConfirmWarpSender;
        public int variableCommandConfirmWarpSenderByID;
        public byte variableCommandConfirmWarpSenderSex;
        public int variableCommandConfirmWarpPlace;

        public int variableCommandConfirmRaise;
        public string variableCommandConfirmRaiseSender;
        public int variableCommandConfirmRaiseSenderByID;
        public byte variableCommandConfirmRaiseSenderSex;
        public int variableCommandConfirmRaisePlace;
                
    }
}
