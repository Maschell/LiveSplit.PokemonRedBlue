﻿using LiveSplit.Model;
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
            AddPointer<int>("inGame", 0x071A);
            AddPointer<EndGame>("EndGame", 0x0358); // if Map==0x76 and Endgame == 0x02
            AddPointer<Maps>("Map", 0x035E);
            AddPointer<Byte>("Random", 0x0371);
            AddPointer<GameTime>("Gametimer", 0x0A41);
            AddPointer<EventFlagData>("EventFlag", 0x0600);
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
              current.HasFoughtErika = current.EventFlag.FoughtErika.HasFlag(GymEvent.FoughtGymLeader);
              return !old.HasFoughtErika && current.HasFoughtErika;
          }
          else if (segment == "koga")
          {
              current.HasFoughtKoga = current.EventFlag.FoughtKoga.HasFlag(GymEvent.FoughtGymLeader);
              return !old.HasFoughtKoga && current.HasFoughtKoga;
          }
          else if (segment == "sabrina")
          {
              current.HasFoughtSabrina = current.EventFlag.FoughtSabrina.HasFlag(GymEvent.FoughtGymLeader);
              return !old.HasFoughtSabrina && current.HasFoughtSabrina;
          }
          else if (segment == "blaine")
          {
              current.HasFoughtBlaine = current.EventFlag.FoughtBlaine.HasFlag(GymEvent.FoughtGymLeader);
              return !old.HasFoughtBlaine && current.HasFoughtBlaine;
          }
          else if (segment == "giovanni")
          {
              current.HasFoughtGiovanni = current.EventFlag.FoughtGiovanni.HasFlag(GymEvent.FoughtGymLeader);
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
          else if (segment == "elite four" || segment == "champion" || segment == "hall of fame")
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
              return TimeSpan.FromMilliseconds((((current.Gametimer.Hours * 60) + current.Gametimer.Minutes) * 60 + current.Gametimer.Seconds + current.Gametimer.Frames / 60.0) * 1000);
        }

       
       
    }
}
