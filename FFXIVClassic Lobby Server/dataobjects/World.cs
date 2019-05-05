namespace FFXIVClassic_Lobby_Server.dataobjects
{
    class World
    {
        public readonly ushort id;
        public readonly string address;
        public readonly ushort port;
        public readonly ushort listPosition;
        public readonly ushort population;
        public readonly string name;
        public readonly bool isActive;

        public World(
            ushort id,
            string address,
            ushort port,
            ushort listPosition,
            ushort population,
            string name,
            bool isActive)
        {
            this.id = id;
            this.address = address;
            this.port = port;
            this.listPosition = listPosition;
            this.population = population;
            this.name = name;
            this.isActive = isActive;
        }
    }
}
