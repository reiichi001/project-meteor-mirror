namespace FFXIVClassic_World_Server.Actor.Group.Work
{
    class LinkshellWork
    {
        public GroupGlobalSave _globalSave = new GroupGlobalSave();
        public GroupMemberSave[] _memberSave = new GroupMemberSave[128];   
     
        public LinkshellWork()
        {
            for (int i = 0; i < _memberSave.Length; i++)
                _memberSave[i] = new GroupMemberSave();
        }
    }
}
