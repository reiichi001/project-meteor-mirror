using FFXIVClassic_Map_Server.actors.director;

namespace FFXIVClassic_Map_Server.actors.group
{
    class GLContentGroup : ContentGroup
    {
        public GLContentGroup(ulong groupIndex, Director director, uint[] initialMembers)
            : base(groupIndex, director, initialMembers)
        {
        }

        public override uint GetTypeId()
        {
            return Group.ContentGroup_GuildleveGroup;
        }
    }
}
