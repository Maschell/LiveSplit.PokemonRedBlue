namespace LiveSplit.PokemonRedBlue
{
    public enum GymEvent : byte
    {
        None = 0x0,
        FoughtGymLeader2 = 0x02,
        FoughtTrainer1 = 0x04,
        FoughtTrainer2 = 0x08,
        FoughtTrainer3 = 0x10,
        GymItem = 0x40,
        FoughtGymLeader = 0x80
        
    }
}