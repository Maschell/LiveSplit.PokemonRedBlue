#define GAME_TIME

using LiveSplit.ASL;
using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using LiveSplit.PokemonRedBlue;
namespace LiveSplit.UI.Components
{
    class Component : IComponent
    {
        //public ComponentSettings Settings { get; set; }

        public string ComponentName
        {
            get { return "Pokemon Red/Blue Auto Splitter"; }
        }
        public GraphicsCache Cache { get; set; }


        public float PaddingBottom { get { return 0; } }
        public float PaddingTop { get { return 0; } }
        public float PaddingLeft { get { return 0; } }
        public float PaddingRight { get { return 0; } }

        public bool Refresh { get; set; }

        public IDictionary<string, Action> ContextMenuControls { get; protected set; }

        public PokemonRedBlueScript Script { get; set; }

        protected SimpleLabel Label1 = new SimpleLabel();

        protected Font TitleFont { get; set; }

        protected string InfoStringPokemon { get; set; }
        protected string InfoStringEncounter { get; set; }
        public Component()
        {
            //Settings = new ComponentSettings();
            Script = new PokemonRedBlueScript();
            Label1 = new SimpleLabel();
            Cache = new GraphicsCache();
           
            TitleFont = new Font("Segoe UI", 20, FontStyle.Regular, GraphicsUnit.Pixel);
        }

        public static UInt16 ReverseBytes(UInt16 value)
        {
            return (UInt16)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            Script.Update(state);

          
            if (state.CurrentPhase == TimerPhase.Running || state.CurrentPhase == TimerPhase.Paused)
            {
                TeamData Team = getPokemon(Script.State.Data);
                int  encounter = getEncounter(Script.State.Data);
                InfoStringEncounter = "Encounter: " + encounter.ToString();
                InfoStringPokemon = "";
                for (int i = 0; i < 6; i++)
                {
                    PokemonData Pokemon = new PokemonData();
                    switch (i)
                    {
                        case 0:
                            Pokemon = Team.Pokemon1;
                            break;
                        case 1:
                            Pokemon = Team.Pokemon2;
                            break;
                        case 2:
                            Pokemon = Team.Pokemon3;
                            break;
                        case 3:
                            Pokemon = Team.Pokemon4;
                            break;
                        case 4:
                            Pokemon = Team.Pokemon5;
                            break;
                        case 5:
                            Pokemon = Team.Pokemon6;
                            break;
                        default:                            
                            break;
                    }
                    if (Pokemon.Pokemon != 0) { 
                        ushort currentHP = ReverseBytes((ushort)Pokemon.CurrentHP);
                        ushort maxHP = ReverseBytes((ushort)Pokemon.MaxHP);
                        double test = (currentHP * 1.0 / maxHP * 1.0) * 100;
                        int test1 = (int)test;
                        InfoStringPokemon += Pokemon.Pokemon.ToString() + " " + Pokemon.Level.ToString()  +  " " + test1.ToString() + "%";
                    }
                    InfoStringPokemon += "\r\n"; 
                }
            }

            Cache.Restart();

            Cache["Encounter"] = InfoStringEncounter;
            Cache["Team"] = InfoStringPokemon;
            

            if (invalidator != null && Cache.HasChanged )
            {
                invalidator.Invalidate(0, 0, width, height);
            }
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {        
           
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            // Create solid brush.
            SolidBrush blueBrush = new SolidBrush(Color.Black);

            // Create rectangle.
            Rectangle rect = new Rectangle(0, 0, 150, 30);
            g.FillRectangle(blueBrush, rect);

            Label1.Text = InfoStringEncounter;// +"\r\n\r\n" + InfoStringPokemon;
            Label1.Brush = new SolidBrush(Color.White);
            Label1.X = 0;
            Label1.Y = 0;
            Label1.Width = 150;
            Label1.Height = 30;
            Label1.Font = TitleFont;
            Label1.Draw(g);
            // Fill rectangle to screen.
        }
        private TeamData getPokemon(dynamic current)
        {
            return (TeamData)current.TeamData;
        }
        private int getEncounter(dynamic current)
        {
            return (int)current.Encounter;
        }

        public float VerticalHeight
        {
            get { return 30; }
        }

        public float MinimumWidth
        {
            get { return 150; }
        }

        public float HorizontalWidth
        {
            get { return 200; }
        }

        public float MinimumHeight
        {
            get { return 30; }
        }

        public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
        {
            return document.CreateElement("x");
        }

        public System.Windows.Forms.Control GetSettingsControl(UI.LayoutMode mode)
        {
            return null;
        }

        public void SetSettings(System.Xml.XmlNode settings)
        {
        }

        public void RenameComparison(string oldName, string newName)
        {
        }

        public void Dispose()
        {
        }
    }
}
