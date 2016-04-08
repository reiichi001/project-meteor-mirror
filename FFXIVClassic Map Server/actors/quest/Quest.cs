using FFXIVClassic_Lobby_Server.common;
using System;
using System.Collections.Generic;

namespace FFXIVClassic_Map_Server.Actors
{
    class Quest : Actor
    {
        private int currentPhase = 0;
        private uint questFlags = 0;
        private Dictionary<string, Object> questData = new Dictionary<string, object>();

        public Quest(uint actorID, string name)
            : base(actorID)
        {
            actorName = name;
        }

        public void InitQuestData(string dataName, object initialValue)
        {
            questData[dataName] = initialValue;
        }

        public void UpdateQuestData(string dataName, object data)
        {
            if (questData.ContainsKey(dataName))
                questData[dataName] = data;

            //Inform update
        }

        public object GetQuestData(string dataName)
        {
            if (questData.ContainsKey(dataName))
                return questData[dataName];
            else
                return null;
        }

        public uint GetQuestId()
        {
            return actorId;
        }

        public void SetQuestFlag(int bitIndex, bool value)
        {
            if (bitIndex >= 32)
            {
                Log.error(String.Format("Tried to access bit flag >= 32 for questId: {0}", actorId));
                return;
            }
            
            int mask = 1 << bitIndex;

            if (value)
                questFlags |= (uint)(1 << bitIndex);
            else
                questFlags &= (uint)~(1 << bitIndex);

            //Inform update
        }

        public bool GetQuestFlag(int bitIndex)
        {
            if (bitIndex >= 32)
            {
                Log.error(String.Format("Tried to access bit flag >= 32 for questId: {0}", actorId));
                return false;
            }
            else
                return (questFlags & (1 << bitIndex)) == (1 << bitIndex);
        }

        public int GetPhase()
        {
            return currentPhase;
        }

        public void NextPhase()
        {
            currentPhase++;
        }

    }
}
