using System.Runtime.InteropServices;
namespace LiveSplit.PokemonRedBlue
{
    [StructLayout(LayoutKind.Explicit)]
    public struct EventFlagData
    {
      
        [FieldOffset(0x065)]
        public byte FoughtRivalSSAnne;
        [FieldOffset(0x151)]
        public GymEvent FoughtGiovanni;
        [FieldOffset(0x155)]
        public GymEvent FoughtBrock;
        [FieldOffset(0x15A)]
        public byte FoughtRivalNugget;
        [FieldOffset(0x15E)]
        public GymEvent FoughtMisty;
        [FieldOffset(0x15F)]
        public bool HasBike;

        [FieldOffset(0x164)]
        public byte FoughtRivalGhost; // 0x80
        [FieldOffset(0x16C)]
        public PokeFluteEvent PokeFlute;
        [FieldOffset(0x173)]
        public GymEvent FoughtLtSurge;
        [FieldOffset(0x17C)]
        public GymEvent FoughtErika;
        [FieldOffset(0x182)]
        public byte FoughtArticuno;
        [FieldOffset(0x192)]
        public GymEvent FoughtKoga;
        [FieldOffset(0x19A)]
        public GymEvent FoughtBlaine;
        [FieldOffset(0x1B3)]
        public GymEvent FoughtSabrina;
        [FieldOffset(0x1D4)]
        public byte FoughtZapdos;
        [FieldOffset(0x1D8)]
        public byte FoughtSnorlaxV;
        [FieldOffset(0x1E0)]
        public CeladonEvent CeladonEvent;
        [FieldOffset(0x1EB)]
        public byte FoughtRivalAla;
        [FieldOffset(0x1EE)]        
        public byte FoughtMoltres;
        [FieldOffset(0x1F2)]
        public BillEvent Bill;        
        [FieldOffset(0x203)]
        public SSAnneEvent SSAnne;
        [FieldOffset(0x25F)]
        public byte FoughtMewtwo;


        
    }
    
    
}