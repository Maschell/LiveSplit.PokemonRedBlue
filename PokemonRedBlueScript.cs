using LiveSplit.Model;
using LiveSplit.PokemonRedBlue;
using System;
using System.Diagnostics; // 1

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
                   
                    State.RefreshValues();
                    ReBuild();
                    OldState = State;
                    
                }
            }
        }

        private void ReBuild()
        {
            State.ValueDefinitions.Clear();
            var gameVersion = GetGameVersion();
            switch (gameVersion)
            {
                case GameVersion.Unknown: RebuildFoo(); break;
                default: Emulator = null; break;
            }
           
        }
        private void RebuildFoo()
        {
            //Base ist 0xD000
            AddPointer<Badges>("Badge", 0x0356);
            AddPointer<int>("inGame", 0x071A);
            AddPointer<EndGame>("EndGame", 0x0358); // if Map==0x76 and Endgame == 0x02
            AddPointer<Maps>("Map", 0x035E);
        }
        private GameVersion GetGameVersion()
        {            
            return GameVersion.Unknown;
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
           
            current.Badge = Badges.None;
            current.EndGame = EndGame.none;
            current.Map = Maps.None;
            current.HasFirstBadge =
            current.HasSecondBadge =
            current.HasThirdBadge =
            current.HasFourthBadge =
            current.HasFifthBadge =
            current.HasSixthBadge =
            current.HasSeventhBadge = 
            current.HasEighthBadge =
            current.GotIntoHallOfFame = false;
            //Check for Timer Start
            WriteDynobj(current);
            if (current.inGame == 0xFF00)
            {
                return true;
            }
            else return false;
        }


       

        public bool Split(LiveSplitState timer, dynamic old, dynamic current)
        {
            //Functions
            WriteDynobj(current);
           var segment = timer.CurrentSplit.Name.ToLower();
           if (segment == "1st gym")
            {               
                current.HasFirstBadge = current.Badge.HasFlag(Badges.One);               
                return !old.HasFirstBadge && current.HasFirstBadge;
            }
           else if (segment == "2nd gym")
           {
               current.HasSecondBadge = current.Badge.HasFlag(Badges.Two);
               return !old.HasSecondBadge && current.HasSecondBadge;
           }
           else if (segment == "3rd gym")
           {
               current.HasThirdBadge = current.Badge.HasFlag(Badges.Three);
               return !old.HasThirdBadge && current.HasThirdBadge;
           }
           else if (segment == "4th gym")
           {
               current.HasFourthBadge = current.Badge.HasFlag(Badges.Four);
               return !old.HasFourthBadge && current.HasFourthBadge;
           }
           else if (segment == "5th gym")
           {
               current.HasFifthBadge = current.Badge.HasFlag(Badges.Five);
               return !old.HasFifthBadge && current.HasFifthBadge;
           }
            else if (segment == "6th gym")
           {
               current.HasSixthBadge = current.Badge.HasFlag(Badges.Six);
               return !old.HasSixthBadge && current.HasSixthBadge;
           }
             else if (segment == "7th gym")
           {
               current.HasSeventhBadge = current.Badge.HasFlag(Badges.Seven);
               return !old.HasSeventhBadge && current.HasSeventhBadge;
           }
           else if (segment == "8th gym")
           {
               current.HasEighthBadge = current.Badge.HasFlag(Badges.Eight);
               return !old.HasEighthBadge && current.HasEighthBadge;
           }
           else if (segment == "elite four")
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

        public bool IsPaused(LiveSplitState timer, dynamic old, dynamic current)
        {
            return true;
        }

        public TimeSpan? GameTime(LiveSplitState timer, dynamic old, dynamic current)
        {
            var delta = (current.GameFrames >= old.GameFrames)
                ? current.GameFrames - old.GameFrames
                : current.GameFrames;

            switch ((int)current.FPSDenominator)
            {
                case 1: current.AccumulatedFrames60 += delta; break;
                case 2: current.AccumulatedFrames30 += delta; break;
                case 3: current.AccumulatedFrames20 += delta; break;
            }

            var seconds = current.AccumulatedFrames20 / 20.0f 
                + current.AccumulatedFrames30 / 30.0f 
                + current.AccumulatedFrames60 / 60.0f;

            return TimeSpan.FromSeconds(seconds);
        }
    
        private static void WriteDynobj(dynamic person)
        {
            System.Collections.Generic.IDictionary<string, object> p = person;

            foreach (var itm in p)
            {
                Debug.WriteLine(string.Format("{0} ist vom Typ {1} und hat als Wert {2}", itm.Key, itm.Value.GetType(), itm.Value));
            }
        }
    }
}
