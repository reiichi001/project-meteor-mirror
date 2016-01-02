using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.dataobjects.chara
{
    class CharaWork
    {
        public ParameterSave    parameterSave = new ParameterSave();
        public ParameterTemp    parameterTemp = new ParameterTemp();
        public BattleSave       battleSave = new BattleSave();
        public BattleTemp       battleTemp = new BattleTemp();
        public EventSave        eventSave = new EventSave();
        public EventTemp        eventTemp = new EventTemp();

        public byte[] property = new byte[32];

        public uint[]   statusShownTime = new uint[20];

        public int[]    command = new int[64];
        public int[]    commandCategory = new int[64];
        public int      commandBorder = 0x20;
        public bool     commandAcquired = false;
        public bool[]   additionalCommandAcquired = new bool[1];

        public uint depictionJudge = 0xa0f50911;
    }
}
