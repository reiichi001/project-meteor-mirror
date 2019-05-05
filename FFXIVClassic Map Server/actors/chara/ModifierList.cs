using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.actors.chara.npc;

namespace FFXIVClassic_Map_Server.actors.chara
{
    class ModifierListEntry
    {
        public uint id;
        public Int64 value;

        public ModifierListEntry(uint id, Int64 value)
        {
            this.id = id;
            this.value = value;
        }
    }

    class ModifierList
    {
        public Dictionary<uint, ModifierListEntry> modList;
        public Dictionary<uint, ModifierListEntry> mobModList;

        public ModifierList(uint id)
        {
            modList = new Dictionary<uint, ModifierListEntry>();
            mobModList = new Dictionary<uint, ModifierListEntry>();
        }

        public void AddModifier(uint id, Int64 val, bool isMobMod)
        {
            var list = isMobMod ? mobModList : modList;
            list.Add(id, new ModifierListEntry(id, val));
        }

        public void SetModifier(uint id, Int64 val, bool isMobMod)
        {
            var list = isMobMod ? mobModList : modList;
            if (list.ContainsKey(id))
                list[id].value = val;
            else
                list.Add(id, new ModifierListEntry(id, val));
        }

        public Int64 GetModifier(uint id, bool isMobMod)
        {
            ModifierListEntry retVal;
            var list = isMobMod ? mobModList : modList;
            if (!list.TryGetValue(id, out retVal))
                return 0;

            return retVal.value;
        }
    }
}
