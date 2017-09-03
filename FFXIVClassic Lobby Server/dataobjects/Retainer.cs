namespace FFXIVClassic_Lobby_Server
{
    class Retainer
    {
        public readonly uint id;
        public readonly uint characterId;        
        public readonly string name;
        public readonly bool doRename;

        public Retainer(uint characterId, uint retainerId, string name, bool doRename)
        {
            this.id = retainerId;
            this.characterId = characterId;
            this.name = name;
            this.doRename = doRename;
        }
    }
}