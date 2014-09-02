using LiveSplit.Model;
using LiveSplit.PokemonRedBlue;
using System;


namespace LiveSplit.ASL
{
    public class PokemonRedBlueScript
    {
        protected TimerModel Model { get; set; }
        protected Emulator Emulator { get; set; }
        public ASLState OldState { get; set; }
        public ASLState State { get; set; }
   
        public PokemonRedBlueScript()
        {
            State = new ASLState();
            
           
        }

        protected void TryConnect()
        {
            if (Emulator == null || Emulator.Process.HasExited)
            {
                Emulator = Emulator.TryConnect();
                if (Emulator != null)
                {
                    Rebuild();
                    State.RefreshValues();
                    
                    OldState = State;
                    
                }
            }
        }

        private void Rebuild()
        {
            State.ValueDefinitions.Clear();
            var gameVersion = GetGameVersion();
            switch (gameVersion)
            {
                case GameVersion.Unknown: Rebuild(); break;
                case GameVersion.US: RebuildUS(); break;
                case GameVersion.German: RebuildGer(); break;
                default: Emulator = null; break;
            }
           
        }
        private void Rebuildbase(int offset = 0x0)
        {
            //Base ist 0xD000
            AddPointer<int>("inGame", 0x071A + offset);
            AddPointer<EndGame>("EndGame", 0x0358 + offset); // if Map==0x76 and Endgame == 0x02
            AddPointer<Maps>("Map", 0x035E + offset);
            AddPointer<Byte>("Random", 0x0371 + offset);
            AddPointer<GameTime>("GameTimer", 0x0A41 + offset);
            AddPointer<EventFlagData>("EventFlag", 0x0600 + offset);
            AddPointer<TeamData>("TeamData", 0x016B + offset);
            AddPointer<Battle>("Battle", 0x057 + offset);
            
        }
         private void RebuildUS()
        {
             Rebuildbase();            
        }
         private void RebuildGer()
         {
             Rebuildbase(0x05);
         }

        private GameVersion GetGameVersion()
        {
            var gameDataCheck = ~Emulator.CreatePointer<String>(4, 0x134);
            return GameVersion.US;
        }

        private void AddPointer<T>(String name, int address)
        {
            AddPointer<T>(1, name, address);
        }

        private void AddPointer<T>(int length, String name, int address)
        {
            State.ValueDefinitions.Add(new ASLValueDefinition()
                {
                    Identifier = name,
                    Pointer = Emulator.CreatePointer<T>(length, address)
                });
        }

        public void Update(LiveSplitState lsState)
        {
           
            if (Emulator != null && !Emulator.Process.HasExited)
            {
                OldState = State.RefreshValues();

                if (lsState.CurrentPhase == TimerPhase.NotRunning)
                {
                    if (Start(lsState, OldState.Data, State.Data))
                    {
                        Model.Start();
                    }
                }
                else if (lsState.CurrentPhase == TimerPhase.Running || lsState.CurrentPhase == TimerPhase.Paused)
                {
                    if (Reset(lsState, OldState.Data, State.Data))
                    {
                        Model.Reset();
                        return;
                    }
                    else if (Split(lsState, OldState.Data, State.Data))
                    {
                        Model.Split();
                    }
                   
                    var isPaused = IsPaused(lsState, OldState.Data, State.Data);
                    if (isPaused != null)
                        lsState.IsGameTimePaused = isPaused;
                    var encounter = GetEncounter(lsState, OldState.Data, State.Data);

                    var gameTime = GameTime(lsState, OldState.Data, State.Data);
                    if (gameTime != null)
                        lsState.SetGameTime(gameTime);
                   
                }
            }
            else
            {
                if (Model == null)
                {
                    Model = new TimerModel() { CurrentState = lsState };
                }
                TryConnect();
            }
        }

        public bool Start(LiveSplitState timer, dynamic old, dynamic current)
        {      
            current.EndGame = EndGame.none;
            current.Encounter = (int)0;
            //current.Map = Maps.None;
            current.HasFoughtBrock =
            current.HasFoughtMisty =            
            current.HasFoughtLtSurge =
            current.HasFoughtErika =
            current.HasFoughtKoga =
            current.HasFoughtSabrina =
            current.HasFoughtBlaine =
            current.HasFoughtGiovanni =
            current.VisitedMtMoon =
            current.HasFoughtRivalNugget =
            current.HasSSAnneTicket =
            current.HasHM01 = 
            current.GotIntoHallOfFame =
            current.HasHM02 =
            current.HasPokeFlute =
            current.HasBike = false;

            //Check for Timer Start
            if (old.Map.HasFlag(Maps.Intro) && old.Random == 0x00)
            {
                return true;
            }
            else return false;
        }


       

        public bool Split(LiveSplitState timer, dynamic old, dynamic current)
        {  

           var segment = timer.CurrentSplit.Name.ToLower();
        
          if (segment == "brock")
           {                       
               current.HasFoughtBrock = current.EventFlag.FoughtBrock.HasFlag(GymEvent.FoughtGymLeader);
               return !old.HasFoughtBrock && current.HasFoughtBrock;
           }
           else if (segment == "misty")
           {
               current.HasFoughtMisty = current.EventFlag.FoughtMisty.HasFlag(GymEvent.FoughtGymLeader);
               return !old.HasFoughtMisty && current.HasFoughtMisty;
           }
          else if (segment == "lt surge" || segment == "lt. surge" || segment == "surge")
          {
              current.HasFoughtLtSurge = current.EventFlag.FoughtLtSurge.HasFlag(GymEvent.FoughtGymLeader);
              return !old.HasFoughtLtSurge && current.HasFoughtLtSurge;
          }
          else if (segment == "erika")
          {
              current.HasFoughtErika = current.EventFlag.FoughtErika.HasFlag(GymEvent.FoughtGymLeader2);
              return !old.HasFoughtErika && current.HasFoughtErika;
          }
          else if (segment == "koga")
          {
              current.HasFoughtKoga = current.EventFlag.FoughtKoga.HasFlag(GymEvent.FoughtGymLeader2);
              return !old.HasFoughtKoga && current.HasFoughtKoga;
          }
          else if (segment == "sabrina")
          {
              current.HasFoughtSabrina = current.EventFlag.FoughtSabrina.HasFlag(GymEvent.FoughtGymLeader2);
              return !old.HasFoughtSabrina && current.HasFoughtSabrina;
          }
          else if (segment == "blaine")
          {
              current.HasFoughtBlaine = current.EventFlag.FoughtBlaine.HasFlag(GymEvent.FoughtGymLeader2);
              return !old.HasFoughtBlaine && current.HasFoughtBlaine;
          }
          else if (segment == "giovanni")
          {
              current.HasFoughtGiovanni = current.EventFlag.FoughtGiovanni.HasFlag(GymEvent.FoughtGymLeader2);
              return !old.HasFoughtGiovanni && current.HasFoughtGiovanni;
          }
          else if (segment == "mt moon" || segment == "mt. moon")
          {
              current.VisitedMtMoon = (current.Map == Maps.MtMoon1 || current.Map == Maps.MtMoon2 || current.Map == Maps.MtMoon3);
              return !old.VisitedMtMoon && current.VisitedMtMoon;
          }
          else if (segment == "rival nugget")
          {
              current.HasFoughtRivalNugget = current.EventFlag.FoughtRivalNugget > 0x0;
              return !old.HasFoughtRivalNugget && current.HasFoughtRivalNugget;             
          }
           else if (segment == "bill")
           {
               current.HasSSAnneTicket = current.EventFlag.Bill.HasFlag(BillEvent.GotSSAnneTicket);
               return !old.HasSSAnneTicket && current.HasSSAnneTicket;
           }         
          else if (segment == "hm1" || segment == "hm01" || segment == "cut")
           {
               current.HasHM01 = current.EventFlag.SSAnne.HasFlag(SSAnneEvent.GotHM01);
               return !old.HasHM01 && current.HasHM01;
           }
          else if (segment == "bike" || segment == "bicycle" )
          {
              current.HasBike = current.EventFlag.HasBike;
              return !old.HasBike && current.HasBike;
          }
          else if (segment == "hm02" || segment == "hm2" || segment == "fly")
          {
              current.HasHM02 = current.EventFlag.CeladonEvent.HasFlag(CeladonEvent.GotHM02);
              return !old.HasHM02 && current.HasHM02;
          }
          else if (segment == "flute" || segment == "pokeflute" || segment == "poke flute")
          {
              current.HasPokeFlute = current.EventFlag.PokeFlute.HasFlag(PokeFluteEvent.HasPokeFlute);
              return !old.HasPokeFlute && current.HasPokeFlute;
          }
          else if (segment == "elite four" || segment == "champion" || segment == "hall of fame" || segment == "end" || segment == "finish")
           {
               current.GotIntoHallOfFame = (current.EndGame == EndGame.end && current.Map == Maps.HallOfFame);
              return !old.GotIntoHallOfFame && current.GotIntoHallOfFame;              
           }
         
            return false;
        }

        public bool Reset(LiveSplitState timer, dynamic old, dynamic current)
        {
            return false;
        }

        public int GetEncounter(LiveSplitState timer, dynamic old, dynamic current)
        {
            if (!(old.Battle == Battle.Encounter) && (current.Battle == Battle.Encounter))
            {
                current.Encounter++;
            }
            return (int)current.Encounter;
        }
        public bool IsPaused(LiveSplitState timer, dynamic old, dynamic current)
        {
            return true;
        }

        public TimeSpan? GameTime(LiveSplitState timer, dynamic old, dynamic current)
        {
              return TimeSpan.FromMilliseconds((((current.GameTimer.Hours * 60) + current.GameTimer.Minutes) * 60 + current.GameTimer.Seconds + current.GameTimer.Frames / 60.0) * 1000);
        }
    }
}
