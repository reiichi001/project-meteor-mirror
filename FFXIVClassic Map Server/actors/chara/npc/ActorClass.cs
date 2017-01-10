using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors.chara.npc
{
    class ActorClass
    {
        public readonly uint actorClassId;
        public readonly string classPath;
        public readonly uint displayNameId;
        public readonly uint propertyFlags;
        public readonly string eventConditions;

        public readonly ushort pushCommand;
        public readonly ushort pushCommandSub;
        public readonly byte pushCommandPriority;

        public ActorClass(uint id, string classPath, uint nameId, uint propertyFlags, string eventConditions, ushort pushCommand, ushort pushCommandSub, byte pushCommandPriority)
        {
            this.actorClassId = id;
            this.classPath = classPath;
            this.displayNameId = nameId;
            this.propertyFlags = propertyFlags;
            this.eventConditions = eventConditions;

            this.pushCommand = pushCommand;
            this.pushCommandSub = pushCommandSub;
            this.pushCommandPriority = pushCommandPriority;
        }
    }
}
