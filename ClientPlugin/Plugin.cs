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

namespace ClientPlugin
{

    // ReSharper disable once UnusedType.Global
    public class Plugin : IPlugin, IDisposable
    {
        public const string Name = "KeybindSwitcher";
        public static Plugin Instance { get; private set; }

        private CommandLine cmd = null;
        private string memory;
        private bool block = false;

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
            if (controlled is MyCubeBlock)
            {
                MyCubeGrid controlledGrid = ((MyCubeBlock)controlled).CubeGrid;
                if (!block | MyInput.Static.IsNewKeyPressed(MyKeys.OemTilde))
                {
                    startCMD(controlledGrid, (MyShipController)controlled);
                    block = true;
                }
                if (cmd != null)
                {
                    if (cmd.State == MyGuiScreenState.OPENED)
                    {
                        updateCMD((MyShipController)controlled);
                    }
                }
                
            }
            else
            {
                block = false;
            }
        }

        public void updateCMD(MyShipController controlled)
        {
            if (controlled.CustomData != memory)
            {
                cmd.ChatTextbox.Text = controlled.CustomData;
                memory = controlled.CustomData;
            }
            else
            {
                controlled.CustomData = cmd.ChatTextbox.Text;
                memory = cmd.ChatTextbox.Text;
            }
        }
        public void startCMD(MyCubeGrid grid,MyShipController controlled)
        {
            
            //// Check if the 'capslock' key has been pressed since the last frame
            //if (MyInput.Static.IsNewKeyPressed(MyKeys.OemTilde))
            //{
            //    // creates Commandline window for use with PB (Debug)
            //    cmd = new CommandLine();
            //    MyGuiSandbox.AddScreen(cmd);
            //}
            if (controlled.DisplayNameText.Contains("[CMDInput]"))
            {
                // creates a transparent commandline window
                cmd = new CommandLine();
                cmd.m_chatTextbox.Alpha = 0f;
                cmd.m_chatTextbox.CanPlaySoundOnMouseOver = false;
                MyGuiSandbox.AddScreen(cmd);
            }
            else if (controlled.DisplayNameText.Contains("[CMDTerminal]"))
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

    class CommandLine : MyGuiScreenBase
    {
        public readonly MyGuiControlTextbox m_chatTextbox;

        public MyGuiControlTextbox ChatTextbox
        {
            get { return m_chatTextbox; }
        }

        public static CommandLine Static = null;

        private const int MESSAGE_HISTORY_SIZE = 20;

        private static StringBuilder[] m_messageHistory = new StringBuilder[MESSAGE_HISTORY_SIZE];

        static CommandLine()
        {
            for (int i = 0; i < MESSAGE_HISTORY_SIZE; ++i)
            {
                m_messageHistory[i] = new StringBuilder();
            }
        }

        public CommandLine()
            : base(new Vector2(0.5f, 0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, null)
        {
            MySandboxGame.Log.WriteLine("MyGuiScreenChat.ctor START");

            EnabledBackgroundFade = false;
            m_isTopMostScreen = true;
            CanHideOthers = false;
            DrawMouseCursor = true;
            m_closeOnEsc = true;

            m_chatTextbox = new MyGuiControlTextbox(
                Vector2.Zero,
                null);
            m_chatTextbox.OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP;
            m_chatTextbox.Size = new Vector2(0.4f, 0.05f);
            m_chatTextbox.TextScale = 0.8f;
            m_chatTextbox.VisualStyle = MyGuiControlTextboxStyleEnum.Debug;

            Controls.Add(m_chatTextbox);

            MySandboxGame.Log.WriteLine("MyGuiScreenChat.ctor END");
        }

        public override void HandleInput(bool receivedFocusInThisUpdate)
        {
            base.HandleInput(receivedFocusInThisUpdate);
            if (MyInput.Static.IsNewKeyPressed(MyKeys.CapsLock))
            {
                CloseScreen();
            }
            // CloseScreen();
        }

        public override bool Update(bool hasFocus)
        {
            if (!base.Update(hasFocus)) return false;

            var normPos = m_position;
            normPos = MyGuiScreenHudBase.ConvertHudToNormalizedGuiPosition(ref normPos);
            m_chatTextbox.Position = new Vector2(0.15f, 0);
            return true;
        }

        public override bool Draw()
        {
            return base.Draw();
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenChat";
        }

        public override void LoadContent()
        {
            base.LoadContent();
            Static = this;
        }

        public override void UnloadContent()
        {
            Static = null;
            base.UnloadContent();
        }

        public override bool HideScreen()
        {
            UnloadContent();
            return base.HideScreen();
        }
    }
}