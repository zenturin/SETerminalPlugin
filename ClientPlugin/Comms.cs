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
using System.Diagnostics;

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

        public string update(string CMDText,int carriage)
        {
            

            bool ClientUpdate = false;
            /*
            if (CMDText != Data.Text)
            {
                Data.Text = CMDText;
                ClientUpdate = true;
            }
            if (carriage != Data.CarriageIndex)
            {
                Data.CarriageIndex = carriage;
                ClientUpdate = true;
            }
            if (Controlled.Pilot.Name.ToString() != Data.User)
            {
                Data.User = Controlled.Pilot.Name.ToString();
                ClientUpdate = true;
            }
            */
            ClientUpdate =
                CMDText != Data.Text ||
                carriage != Data.CarriageIndex ||
                Controlled.Pilot.Name.ToString() != Data.User ||
                Data.SessionID == "NA";
            if (Data.SessionID == "NA")
            {
                Data.randomizeSessionID();
            }
                


            if (ClientUpdate)
            {
                Data.Text = CMDText;
                Data.CarriageIndex = carriage;
                Data.User = Controlled.Pilot.Name.ToString();
                Controlled.CustomData = Data.ToString();
            }

            string x = Data.TryParse(Controlled.CustomData);
            Debug.WriteLine("TryParse = " + x);
            if (x == "Failed") // Customdata is corrupted
            {
                Controlled.CustomData = Data.ToString();
                Data.flagError("Ini Structure is incorrect and failed to parse");
            }
            else if (x == "Changed") // Customdata has been updated by the server
            {
                if (Data.Text != CMDText & !ClientUpdate )
                {
                    return Data.Text;
                }
            }
            else if (x == "Initialise") // Customdata contains #Terminal
            {
                Data.Reset();
                Controlled.CustomData = Data.ToString();
            }
            return null;
        }

    }
}
