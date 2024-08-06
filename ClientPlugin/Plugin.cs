using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using EmptyKeys.UserInterface.Controls;
using Epic.OnlineServices;
using HarmonyLib;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NLog.LayoutRenderers;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Blocks;
using Sandbox.Graphics.GUI;
using Sandbox.ModAPI;
using SpaceEngineers.Game.Utils;
using VRage.Input;
using VRage.Plugins;
using static VRage.Game.MyObjectBuilder_ControllerSchemaDefinition;

namespace ClientPlugin
{
    // ReSharper disable once UnusedType.Global
    public class Plugin : IPlugin, IDisposable
    {
        public const string Name = "TerminalPlugin";
        public static Plugin Instance { get; private set; }
        public MyProgrammableBlock PB = null;
        Interface PBInterface;
        CommandLine cmd;
        bool cmdallowed = false;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        public void Init(object gameInstance)
        {
            Instance = this;

            // TODO: Put your one time initialization code here.
            Harmony harmony = new Harmony(Name);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public void Dispose()
        {
            // TODO: Save state and close resources here, called when the game exits (not guaranteed!)
            // IMPORTANT: Do NOT call harmony.UnpatchAll() here! It may break other plugins.

            Instance = null;
        }

        public void Update()
        {
            var controlled = MyAPIGateway.Session?.ControlledObject?.Entity;

            if (controlled is MyShipController controller)
            {
                // CONTROLLER LOGIC
                switch (controller.CustomData.ToLower())
                {
                    case "":
                        break;
                    case "#terminal":
                        controller.CustomData = "[Terminal Controller]\n";
                        controller.CustomData += "[NA] : False";
                        break;
                }
                if (isPBAlive() == false && controller.CustomData != "")
                {
                    string temp = controller.CustomData;
                    string tag = "[NA]";
                    try
                    {
                        tag = temp.Split('\n')
                                          [1].Split(':')
                                          [0].Trim();
                    }
                    catch (Exception)
                    {

                    }

                    var PBOnline = GetPB(controller);
                    controller.CustomData = "[Terminal Controller]\n" +
                        tag + " : " + PBOnline.ToString();
                }
                // PB LOGIC
                if (isPBAlive())
                {
                    if (PBInterface == null)
                    {
                        PBInterface = new Interface(PB, controller);
                    }
                    cmdallowed = true;
                    Main(); // IF PB is alive, run the main loop
                }
                else
                {
                    cmd = null;
                    PBInterface = null;
                    cmdallowed = false;
                }
            }
            else
            {
                cmd = null;
                PBInterface = null;
                cmdallowed = false;
            }
        }

        void Main()
        {
            // Manage Key Lists
            var PressedKeys = new List<MyKeys>();
            var ReleasedKeys = new List<MyKeys>();
            MyInput.Static.GetPressedKeys(PressedKeys);
            var tempkeys = new List<MyKeys>();
            tempkeys = PressedKeys;
            foreach (var key in PressedKeys)
            {
                if (MyInput.Static.IsNewKeyReleased(key))
                {
                    ReleasedKeys.Add(key);
                    tempkeys.Remove(key);
                }
            }
            PressedKeys = tempkeys;
            // End of Key Management

            // Closes CMD if the user has closed it
            if (cmd != null && cmd.State == MyGuiScreenState.CLOSED)
            {
                cmd = null;
            }
            //

            // Start CMD if not already running
            if (cmd == null && (MyInput.Static.IsKeyPress(MyKeys.OemTilde) || MyInput.Static.IsKeyPress(MyKeys.Oem8)) && cmdallowed)
            {
                PBInterface.RetrieveUI();
                startCMD();
                cmd.ChatTextbox.Text = PBInterface.textbox.Text;
                cmd.ChatTextbox.MoveCarriageToEnd();
            }
            else if (cmd != null)
            {
                PBInterface.textbox.Text = cmd.ChatTextbox.Text;
                PBInterface.textbox.CarriageIndex = cmd.ChatTextbox.CarriagePositionIndex;
                PBInterface.Update(cmd.ChatTextbox.Text, cmd.ChatTextbox.CarriagePositionIndex, PressedKeys, ReleasedKeys);
                if (PBInterface.textbox.Text != cmd.ChatTextbox.Text)
                {
                    cmd.ChatTextbox.Text = PBInterface.textbox.Text;
                    cmd.ChatTextbox.MoveCarriageToEnd();
                }
            }
            if (PB.CustomData.ToLower() == "#terminal")
            {
                PBInterface.Reset();
            }
            //
        }

        bool isPBAlive()
        {
            var result = PB != null
                && PB.IsWorking;
            return result;
        }

        bool GetPB(MyShipController controller)
        {
            if (controller.CustomData.Contains("[Terminal Controller]"))
            {
                string temp = controller.CustomData;
                string Tag = temp.Split('\n')
                [1].Split(':')
                    [0].Trim();
                var grid = controller.CubeGrid;
                var ProgrammableBlocks = grid.GetFatBlocks<MyProgrammableBlock>();
                foreach (var block in ProgrammableBlocks)
                {
                    if (block.DisplayNameText.Contains(Tag))
                    {
                        PB = block;
                        return true;
                    }
                }
            }
            return false;
        }

        public void startCMD()
        {

            //// Check if the 'capslock' key has been pressed since the last frame
            //if (MyInput.Static.IsNewKeyPressed(MyKeys.OemTilde))
            //{
            //    // creates Commandline window for use with PB (Debug)
            //    cmd = new CommandLine();
            //    MyGuiSandbox.AddScreen(cmd);
            //}
            if (PBInterface.UI.CMDMode == "CMDInput")
            {
                // creates a transparent commandline window
                cmd = new CommandLine();
                cmd.m_chatTextbox.Alpha = 0f;
                cmd.m_chatTextbox.CanPlaySoundOnMouseOver = false;
                MyGuiSandbox.AddScreen(cmd);
            }
            else if (PBInterface.UI.CMDMode == "CMDTerminal")
            {
                // creates a transparent commandline window
                cmd = new CommandLine();
                cmd.m_chatTextbox.Alpha = 1f;
                cmd.m_chatTextbox.CanPlaySoundOnMouseOver = false;
                cmd.m_chatTextbox.Text = "";
                cmd.m_chatTextbox.MoveCarriageToEnd();
                MyGuiSandbox.AddScreen(cmd);
            }
            else if (PBInterface.UI.CMDMode == "Custom")
            {
                cmd = new CommandLine();
                cmd.m_chatTextbox.Alpha = PBInterface.UI.Alpha;
                cmd.m_chatTextbox.CanPlaySoundOnMouseOver = PBInterface.UI.CanPlaySoundOnMouseOver;
                cmd.m_chatTextbox.Text = "";
                cmd.m_chatTextbox.MoveCarriageToEnd();
                cmd.ChatTextbox.VisualStyle = PBInterface.UI.VisualStyle;
                cmd.ChatTextbox.TextScale = PBInterface.UI.TextScale;
                cmd.ChatTextbox.PositionX = PBInterface.UI.PositionX;
                cmd.ChatTextbox.PositionY = PBInterface.UI.PositionY;
                cmd.ChatTextbox.Size = PBInterface.UI.Size;
                cmd.ChatTextbox.ColorMask = PBInterface.UI.ColorMask;
                cmd.ChatTextbox.BorderColor = PBInterface.UI.BorderColor;
                MyGuiSandbox.AddScreen(cmd);
            }
        }
    }
}