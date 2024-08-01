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

namespace ClientPlugin
{

    // ReSharper disable once UnusedType.Global
    public class Plugin : IPlugin, IDisposable
    {
        public const string Name = "Terminal";

        public static Plugin Instance { get; private set; }

        private CommandLine cmd = null;
        private Comms comms = null;

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

        public void Update()
        {
            var controlled = MyAPIGateway.Session?.ControlledObject?.Entity;
            if (controlled is MyShipController)
            {
                //MyCubeGrid controlledGrid = ((MyCubeBlock)controlled).CubeGrid;
                Main((MyShipController) controlled);
            }
            else
            {
                CloseMain();
            }

        }

        public void Main(MyShipController controlled)
        {
            if (comms == null)
            {
                comms = new Comms(controlled);
            }
            if (cmd == null)
            {
                comms.Controlled = controlled;
                comms.Data.randomizeSessionID();
                startCMD(controlled);
                cmd.ChatTextbox.Text = comms.Data.Text;
                cmd.ChatTextbox.MoveCarriageToEnd();
            }

            string newText = comms.update(cmd.m_chatTextbox.Text,cmd.m_chatTextbox.CarriagePositionIndex);
            if (newText != null)
            {
                cmd.m_chatTextbox.Text = newText;
                cmd.m_chatTextbox.MoveCarriageToEnd();
            }
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