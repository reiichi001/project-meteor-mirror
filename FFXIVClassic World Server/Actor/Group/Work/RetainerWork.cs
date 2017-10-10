
namespace FFXIVClassic_World_Server.Actor.Group.Work
{
    class RetainerWork
    {
        public GroupMemberSave[] _memberSave = new GroupMemberSave[128];  

        public RetainerWork()
        {
            for (int i = 0; i < _memberSave.Length; i++)
                _memberSave[i] = new GroupMemberSave();
        }
    }
}
