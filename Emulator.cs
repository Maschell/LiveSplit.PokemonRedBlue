using System.Diagnostics;
using System.Linq;

namespace LiveSplit.PokemonRedBlue
{
    public class Emulator
    {
        public Process Process { get; protected set; }
        public int Offset { get; protected set; }

        protected Emulator(Process process, int offset)
        {
            Process = process;
            Offset = offset;
        }

        public static Emulator TryConnect()
        {
            
            var process = Process.GetProcessesByName("VisualBoyAdvance").FirstOrDefault();
            if (process != null)
            {
                return BuildVBA(process);
            }
            process = Process.GetProcessesByName("vba-v24m-svn480").FirstOrDefault();
            if (process != null)
            {
                return BuildVBAv24m(process);
            }
            process = Process.GetProcessesByName("bgb").FirstOrDefault();
            if (process != null)
            {
                return BuildBGB(process);
            }
            process = Process.GetProcessesByName("EmuHawk").FirstOrDefault();
            if (process != null)
            {
                return BuildBizHawk(process);
            }


            return null;
        }

        private static Emulator Build(Process process, int _base)
        {
            var offset = ~new DeepPointer<int>(process, _base);

            return new Emulator(process, offset);
        }
              

        private static Emulator BuildBizHawk(Process process)
        {
            var offset = ~new DeepPointer<int>(process, (int)EmulatorBase.BizHawk);
            return new Emulator(process, offset + 0x390);
        }
    

        private static Emulator BuildVBA(Process process)
        {
            var version = checkVBAVersion(process);
            var _base = 0;

            ProcessModule module = process.MainModule;
            if (version.Equals("1.7.2"))
                _base = (int)EmulatorBase.VisualBoyAdvance172;
            else { 
            }              
           
            return Build(process, _base);
        }

        private static Emulator BuildBGB(Process process)
        {
            ProcessModule module = process.MainModule;           
            var _base = (int)EmulatorBase.BGB;       
            var newoffset = ~new DeepPointer<int>(process, _base,(int)EmulatorOffsets.BGB);

            return new Emulator(process, newoffset);
        }
        private static Emulator BuildVBAv24m(Process process)
        {
            
            ProcessModule module = process.MainModule;
            
            var _base = (int)EmulatorBase.VisualBoyAdvancev24m;
           

            return Build(process, _base);
        }

        private static string checkVBAVersion(Process process)
        {
            return "1.7.2";
        }

        public DeepPointer<T> CreatePointer<T>(int address)
        {
            return CreatePointer<T>(1, address);
        }

        public DeepPointer<T> CreatePointer<T>(int length, int address)
        {
            return new DeepPointer<T>(length, Process, Offset - (int)Process.MainModule.BaseAddress + address);
        }
    }
}
