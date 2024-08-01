using DirectShowLib.DMO;
using HarmonyLib;
using Sandbox.Game.Entities;
using Sandbox.Game.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace ClientPlugin
{
    public class Comms
    {

        public Packet Data = new Packet();
        public MyShipController Controlled;
        


        public Comms(MyShipController controller)
        {
            Controlled = controller;
        }

        public string update(string CMDText)
        {
            bool ClientUpdate = false;

            if (CMDText != Data.Text)
            {
                Data.Text = CMDText;
                ClientUpdate = true;
            }
            if (Controlled.Pilot.Name.ToString() != Data.User)
            {
                Data.User = Controlled.Pilot.Name.ToString();
                ClientUpdate = true;
            }

            if (ClientUpdate)
            {
                Controlled.CustomData = Data.ToString();
            }

            string x = Data.TryParse(Controlled.CustomData);
            if (x == "Failed")
            {
                Controlled.CustomData = Data.ToString();
                Data.flagError("Ini Structure is incorrect and failed to parse");
            }
            else if (x == "Changed")
            {
                if (Data.Text != CMDText & !ClientUpdate )
                {
                    return Data.Text;
                }
            }
            return null;
        }

    }
}
