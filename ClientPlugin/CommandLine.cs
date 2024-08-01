using EmptyKeys.UserInterface.Controls;
using Sandbox.Game.Gui;
using Sandbox.Graphics.GUI;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Input;
using VRage.Utils;
using VRageMath;

namespace ClientPlugin
{
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
            m_chatTextbox.ForceHighlight = true;

            Controls.Add(m_chatTextbox);

            MySandboxGame.Log.WriteLine("MyGuiScreenChat.ctor END");
        }

        public override void HandleInput(bool receivedFocusInThisUpdate)
        {
            base.HandleInput(receivedFocusInThisUpdate);
            /*
            if (MyInput.Static.IsNewKeyPressed(MyKeys.CapsLock))
            {
                CloseScreen();
            }
            */
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
