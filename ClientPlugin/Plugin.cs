using System;
using System.Reflection;
using HarmonyLib;
using VRage.Plugins;
using System.IO;
using VRage.FileSystem;
using static Sandbox.Engine.Utils.MyConfigBase;
using SpaceEngineers;
using ClientPlugin;
using Sandbox;
using Sandbox.Graphics.GUI;
using VRage.Library.Utils;
using VRageRender.ExternalApp;
using VRage.Game.Components;
using VRage.Input;
using Sandbox.Game.Gui;
using Sandbox.Game.World;
using System.Text;
using VRage.Game;
using VRage.Utils;
using VRageMath;
using DirectShowLib.Dvd;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using static VRage.Game.MyObjectBuilder_ControllerSchemaDefinition;
using System.Collections.Generic;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace ClientPlugin
{

    // ReSharper disable once UnusedType.Global
    public class Plugin : IPlugin, IDisposable
    {
        public const string Name = "Terminal";

        public static Plugin Instance { get; private set; }

        private CommandLine cmd = null;
        private Comms comms = null;
        private MyIni checker = new MyIni();
        private List<string> AllowedKeys = new List<string>
        {
            "Enter",
            "Back",
            "LeftAlt",
            "RightAlt",
            "LeftControl",
            "RightControl",
            "LeftShift",
            "RightShift",
            "Tab",
            "Insert",
            "Delete",
            "Home",
            "End",
            "PageUp",
            "PageDown",
            "Up",
            "Down",
            "Left",
            "Right",
            "Clear",
            "NumLock",
            "ScrollLock",
            "Pause",
            "Oem8"
        };
        private List<string> SpecialKeys = new List<string>();
        private List<string> GeneralKeys = new List<string>();

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        public void Init(object gameInstance)
        {
            Instance = this;
            // TODO: Put your one time initialization code here.
            Harmony harmony = new Harmony(Name);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            // My crap

            
        }

        public void Dispose()
        {
            // TODO: Save state and close resources here, called when the game exits (not guaranteed!)
            // IMPORTANT: Do NOT call harmony.UnpatchAll() here! It may break other plugins.

            Instance = null;
        }
        //DEBUG
        private string log = "";
        private List<MyKeys> HeldKeys = new List<MyKeys>();
        public void Update()
        {
            var controlled = MyAPIGateway.Session?.ControlledObject?.Entity;
            if (controlled is MyShipController)
            {
                if (((MyShipController)controlled).CustomData != "" &&
                (((MyShipController)controlled).CustomData.ToLower() == "#terminal")
                || checker.TryParse(((MyShipController)controlled).CustomData))
                {
                    // DEBUG
                    var x = (MyShipController)controlled;
                    if (x.CustomData != log)
                    {
                        log = x.CustomData;
                        Debug.WriteLine(log);
                    }

                    // DEBUG

                    bool KeyUpdate = false;
                    List<MyKeys> MyKeys = new List<MyKeys>();
                    SpecialKeys.Clear();
                    GeneralKeys.Clear();
                    MyInput.Static.GetListOfPressedKeys(MyKeys);
                    //MyInput.Static.GetPressedKeys(MyKeys);
                    if (MyInput.Static.IsAnyKeyPress() || HeldKeys.Count != 0)
                    {
                        foreach (var key in MyKeys)
                        {
                            var k = key.ToString();
                            if (AllowedKeys.Contains(k))
                            {
                                SpecialKeys.Add(k);
                            }
                            else
                            {
                                GeneralKeys.Add(k);
                            }
                        }
                        HeldKeys.AddList(MyKeys);
                        KeyUpdate = true;

                        List<MyKeys> tempKeys = new List<MyKeys>(HeldKeys);
                        foreach (var key in tempKeys)
                        {
                            if (MyInput.Static.IsNewKeyReleased(key))
                            {
                                HeldKeys.Remove(key);
                            }
                        }
                    }






                    //MyCubeGrid controlledGrid = ((MyCubeBlock)controlled).CubeGrid;

                    if (cmd == null)
                    {
                        Main((MyShipController)controlled, KeyUpdate);
                    }
                    else if (cmd.State == MyGuiScreenState.OPENED)
                    {
                        Main((MyShipController)controlled, KeyUpdate);
                    }
                    else if (MyInput.Static.IsNewKeyPressed(VRage.Input.MyKeys.Oem8) | MyInput.Static.IsNewKeyPressed(VRage.Input.MyKeys.OemTilde))
                    {
                        cmd = null;
                        Main((MyShipController)controlled, KeyUpdate);
                    }


                }
                else
                {
                    CloseMain();
                }
            }
        }

        public void Main(MyShipController controlled,bool KeyUpdate)
        {
            if (comms == null)
            {
                comms = new Comms(controlled);
            }

            string newText = comms.update();
            if (cmd == null)
            {
                comms.Controlled = controlled;
                comms.Data.SessionID = "NA";
                startCMD(controlled);
                cmd.ChatTextbox.Text = comms.Data.Text;
                cmd.ChatTextbox.MoveCarriageToEnd();
            }


            newText = comms.update(cmd.m_chatTextbox.Text,cmd.m_chatTextbox.CarriagePositionIndex,SpecialKeys,GeneralKeys);
            if (newText != null)
            {
                if (cmd.m_chatTextbox.Text != comms.Data.Text)
                {
                    cmd.m_chatTextbox.Text = comms.Data.Text;
                    cmd.m_chatTextbox.MoveCarriageToEnd();
                }

            }
            //Moved Temporarily
        }

        public void CloseMain()
        {
            cmd = null;
        }


        public void startCMD(MyShipController controlled)
        {
            
            //// Check if the 'capslock' key has been pressed since the last frame
            //if (MyInput.Static.IsNewKeyPressed(MyKeys.OemTilde))
            //{
            //    // creates Commandline window for use with PB (Debug)
            //    cmd = new CommandLine();
            //    MyGuiSandbox.AddScreen(cmd);
            //}
            if (comms.Data.CMDMode == "CMDInput")
            {
                // creates a transparent commandline window
                cmd = new CommandLine();
                cmd.m_chatTextbox.Alpha = 0f;
                cmd.m_chatTextbox.CanPlaySoundOnMouseOver = false;
                MyGuiSandbox.AddScreen(cmd);
            }
            else if (comms.Data.CMDMode == "CMDTerminal")
            {
                // creates a transparent commandline window
                cmd = new CommandLine();
                cmd.m_chatTextbox.Alpha = 1f;
                cmd.m_chatTextbox.CanPlaySoundOnMouseOver = false;
                cmd.m_chatTextbox.Text = "";
                cmd.m_chatTextbox.MoveCarriageToEnd();
                MyGuiSandbox.AddScreen(cmd);
            }
            else if (comms.Data.CMDMode == "Custom")
            {
                cmd = new CommandLine();
                cmd.m_chatTextbox.Alpha = comms.Data.Alpha;
                cmd.m_chatTextbox.CanPlaySoundOnMouseOver = comms.Data.CanPlaySoundOnMouseOver;
                cmd.m_chatTextbox.Text = comms.Data.Text;
                cmd.m_chatTextbox.MoveCarriageToEnd();
                cmd.ChatTextbox.VisualStyle = comms.Data.VisualStyle;
                cmd.ChatTextbox.TextScale = comms.Data.TextScale;
                cmd.ChatTextbox.PositionX = comms.Data.PositionX;
                cmd.ChatTextbox.PositionY = comms.Data.PositionY;
                cmd.ChatTextbox.Size = comms.Data.Size;
                cmd.ChatTextbox.ColorMask = comms.Data.ColorMask;
                cmd.ChatTextbox.BorderColor = comms.Data.BorderColor;
                MyGuiSandbox.AddScreen(cmd);
            }
        }


        // TODO: Uncomment and use this method to create a plugin configuration dialog
        // ReSharper disable once UnusedMember.Global
        /*public void OpenConfigDialog()
        {
            MyGuiSandbox.AddScreen(new MyPluginConfigDialog());
        }*/

        //TODO: Uncomment and use this method to load asset files
        /*public void LoadAssets(string folder)
        {

        }*/
    }
}