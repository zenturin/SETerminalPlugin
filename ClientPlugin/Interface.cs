using EmptyKeys.UserInterface.Generated;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Blocks;
using Sandbox.Graphics.GUI;
using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Input;
using VRageMath;

namespace ClientPlugin
{
    struct TextBoxStruct
    {
        public string Text;
        public int CarriageIndex;
    }
    struct UIConfig
    {
        public string CMDMode;
        public float Alpha;
        public bool CanPlaySoundOnMouseOver;
        public float TextScale;
        public MyGuiControlTextboxStyleEnum VisualStyle;
        public float PositionX;
        public float PositionY;
        public Vector2 Size;
        public Vector4 ColorMask;
        public Vector4 BorderColor;

    }
    class Interface
    {
        // Required Blocks
        MyProgrammableBlock PB;
        MyShipController controller;
        // Randomizer for Session ID generation
        static Random randomizer = new Random();
        int SessionID = randomizer.Next(0, 1000000);
        string User = "NA";
        public TextBoxStruct textbox;
        private TextBoxStruct lasttextbox;
        public UIConfig UI;
        private bool isFirstRun = true;


        public Interface(MyProgrammableBlock PB,MyShipController controller) 
        {
            this.controller = controller;
            this.PB = PB;
            User = controller.Pilot.Name;
            textbox = new TextBoxStruct();
            lasttextbox = new TextBoxStruct();
        }

        public void Update()
        {
            MyIni temp = new MyIni();
            temp.TryParse(PB.CustomData);
            ResolveTextBox(temp);
        }

        public void Update(string TextBox,int CarriageIndex,List<MyKeys> PressedKeys,List<MyKeys> ReleasedKeys)
        {
            if (controller.CustomData.Contains("#terminal"))
            {
                //Generate New Config (Mainly for testing for now)
            }
            MyIni temp = new MyIni();
            temp.TryParse(PB.CustomData);
            ResolveTextBox(temp);
            SendArguments(PressedKeys,ReleasedKeys);
        }
        private void SendArguments(List<MyKeys> PressedKeys,List<MyKeys> ReleasedKeys) 
        {
            if (isFirstRun)
            {
                PB.Run("Startup" + " " + SessionID + " " + User,UpdateType.Mod);
                isFirstRun = false;
            }

            if (PressedKeys.Count != 0)
            {
                string keyString = "";
                foreach (var key in PressedKeys)
                {
                    keyString += key.ToString() + " ";
                }
                PB.Run("KeyPressed" + " " + keyString,UpdateType.Mod);
            }

            if (ReleasedKeys.Count != 0)
            {
                string keyString = "";
                foreach (var key in ReleasedKeys)
                {
                    keyString += key.ToString() + " ";
                }
                PB.Run("KeyReleased" + " " + keyString, UpdateType.Mod);
            }
        }

        private void ResolveTextBox(MyIni temp)
        {
            var TextBoxData = new MyIni();
            TextBoxData.TryParse(PB.CustomData);
            TextBoxStruct PBTextBox = new TextBoxStruct();
            PBTextBox.Text = TextBoxData.Get("TextBox", "Text").ToString();
            PBTextBox.CarriageIndex = TextBoxData.Get("TextBox", "CarriageIndex").ToInt32();

            if (PBTextBox.Text != lasttextbox.Text || PBTextBox.CarriageIndex != lasttextbox.CarriageIndex)
            {
                temp.Set("TextBox", "Text", PBTextBox.Text);
                temp.Set("TextBox", "CarriageIndex", PBTextBox.CarriageIndex);
                lasttextbox = PBTextBox;
                textbox.Text = PBTextBox.Text;
                textbox.CarriageIndex = PBTextBox.CarriageIndex;
            }
            else if (textbox.Text != lasttextbox.Text || textbox.CarriageIndex != lasttextbox.CarriageIndex)
            {
                temp.Set("TextBox", "Text", textbox.Text);
                temp.Set("TextBox", "CarriageIndex", textbox.CarriageIndex);
                lasttextbox = textbox;
                PB.CustomData = temp.ToString();
            }
        }

        private MyIni lastUIData = new MyIni();
        public void RetrieveUI()
        {
            var UIData = new MyIni();
            UIData.TryParse(PB.CustomData);

            if (UIData == lastUIData)
            {
                return;
            }
            lastUIData = UIData;
            List<MyIniKey> Values = new List<MyIniKey>();

            Dictionary<string, string> TempUI = new Dictionary<string, string>();

            UIData.GetKeys("UI", Values);
            foreach (var value in Values)
            {
                TempUI.Add(value.Name, UIData.Get(value.Section,value.Name).ToString());
            }
            
            UI.CMDMode = TempUI["CMDMode"];

            UI.Alpha = float.Parse(TempUI["Alpha"]);

            UI.CanPlaySoundOnMouseOver = bool.Parse(TempUI["CanPlaySoundOnMouseOver"]);

            UI.TextScale = float.Parse(TempUI["TextScale"]);

            switch (TempUI["VisualStyle"])
            {
                case "Debug":
                    UI.VisualStyle = MyGuiControlTextboxStyleEnum.Debug;
                    break;
                case "Default":
                    UI.VisualStyle = MyGuiControlTextboxStyleEnum.Default;
                    break;
                case "Custom":
                    UI.VisualStyle = MyGuiControlTextboxStyleEnum.Custom;
                    break;
                case "NoHighlight":
                    UI.VisualStyle = MyGuiControlTextboxStyleEnum.NoHighlight;
                    break;
                default:
                    UI.VisualStyle = MyGuiControlTextboxStyleEnum.Default;
                    break;
            }

            UI.PositionX = float.Parse(TempUI["PositionX"]);

            UI.PositionY = float.Parse(TempUI["PositionY"]);

            UI.Size = new Vector2();
            UI.Size.X = float.Parse(TempUI["SizeX"]);
            UI.Size.Y = float.Parse(TempUI["SizeY"]);

            UI.ColorMask = new Vector4();
            UI.ColorMask.X = float.Parse(TempUI["ColorMaskRed"]);
            UI.ColorMask.Y = float.Parse(TempUI["ColorMaskGreen"]);
            UI.ColorMask.Z = float.Parse(TempUI["ColorMaskBlue"]);
            UI.ColorMask.W = float.Parse(TempUI["ColorMaskAlpha"]);


        }

        public void Reset()
        {
            MyIni data = new MyIni();
            //TextBox
            data.Set("TextBox", "Text", "");
            data.Set("TextBox", "CarriageIndex", "0");
            // UI Configuration
            data.Set("UI", "CMDMode", "CMDTerminal");
            data.Set("UI", "Alpha", "1");// Float
            data.Set("UI", "CanPlaySoundOnMouseOver", "False"); // Bool
            data.Set("UI", "TextScale", "0.8"); // Float
            data.Set("UI", "VisualStyle", "Debug");
            data.Set("UI", "PositionX", "0.15"); // Float
            data.Set("UI", "PositionY", "0"); // Float
            data.Set("UI", "SizeX", "0.15"); // Vector2
            data.Set("UI", "SizeY", "0.15"); // Vector2
            data.Set("UI", "ColorMaskRed", "1"); // Vector4
            data.Set("UI", "ColorMaskGreen", "1"); // Vector4
            data.Set("UI", "ColorMaskBlue", "1"); // Vector4
            data.Set("UI", "ColorMaskAlpha", "1"); // Vector4

            // Debug
            data.Set("Debug", "ErrorCount", "0"); // Int
            data.Set("Debug", "Last Error", "None");
            PB.CustomData = data.ToString();
        }
    }
}
