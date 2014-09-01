using System.Runtime.InteropServices;
namespace LiveSplit.PokemonRedBlue
{
    [StructLayout(LayoutKind.Explicit)]
    public struct TeamData
    {      
        [FieldOffset(0x0)]
        public PokemonData Pokemon1;
        [FieldOffset(0x2C)]
        public PokemonData Pokemon2;
        [FieldOffset(0x58)]
        public PokemonData Pokemon3;
        [FieldOffset(0x84)]
        public PokemonData Pokemon4;
        [FieldOffset(0xB0)]
        public PokemonData Pokemon5;
        [FieldOffset(0xDC)]
        public PokemonData Pokemon6; 
    }
    
    
}