using System.Runtime.InteropServices;
namespace LiveSplit.PokemonRedBlue
{
    [StructLayout(LayoutKind.Explicit)]
    public struct PokemonData
    {      
        [FieldOffset(0x00)]
        public PokemonList Pokemon;
        [FieldOffset(0x04)]
        public PokemonStatus Status;        
        [FieldOffset(0x08)]
        public Moves Move1;
        [FieldOffset(0x09)]
        public Moves Move2;
        [FieldOffset(0x0A)]
        public Moves Move3;
        [FieldOffset(0x0B)]
        public Moves Move4;             
    }    
}