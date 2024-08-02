using Microsoft.Xml.Serialization.GeneratedAssembly;
using Sandbox.Graphics.GUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.VisualScripting.ScriptBuilder.Nodes;
using VRageMath;

namespace ClientPlugin
{
    public class Packet
    {
        // Definitions

        public MyIni data = new MyIni();
        Random randomizer = new Random();

        // Initialization

        public Packet()
        {
            Reset();
        }
        // Accessors

        public string CMDMode
        {
            get
            {
                return (data.Get("Setup", "CMDMode").ToString());
            }
            set
            {
                data.Set("Setup", "CMDMode", value);
            }
        }

        public string SessionID
        {
            get
            {
                return (data.Get("Setup", "SessionID").ToString());
            }
            set
            {
                data.Set("Setup", "SessionID", value);
            }
        }

        public bool FirstRun
        {
            get
            {
                if (data.Get("States", "FirstRun").ToString().ToLower() == "true")
                {
                    return (true);
                }
                else
                {
                    return (false);
                }
            }
            set
            {
                if (value)
                {
                    data.Set("States", "FirstRun", "True");
                }
                else
                {
                    data.Set("States", "FirstRun", "False");
                }
            }
        }

        public string Text
        {
            get
            {
                return (data.Get("States", "Text").ToString());
            }
            set
            {
                data.Set("States", "Text", value);
            }
        }

        public string User
        {
            get
            {
                return (data.Get("States", "User").ToString());
            }
            set
            {
                data.Set("States", "User", value);
            }
        }

        public int CarriageIndex
        {
            get
            {
                return (int)data.Get("States", "CarriageIndex").ToInt32();
            }
            set
            {
                data.Set("States", "CarriageIndex", value);
            }
        }

        public List<string> SpecialKeys
        {
            get
            {

                List<string> Keys = new List<string>();
                string y = data.Get("Keys", "SpecialKeys").ToString();
                try
                {
                    List<string> z = new List<string>();
                    if (y.Contains(','))
                    {
                        z = y.Split(',').ToList();
                    }
                    else
                    {
                         
                        z.Add(y);
                    }

                    foreach (var item in z)
                    {
                        string temp = item;
                        int index = 0;
                        index = temp.IndexOf("(");
                        if (index != -1)
                        {
                            temp = temp.Remove(index, 1);
                        }
                        index = temp.IndexOf(")");
                        if (index != -1)
                        {
                            temp = temp.Remove(index, 1);
                        }
                        temp.Trim();
                        Keys.Add(temp);
                    }
                    return Keys;
                }
                catch (Exception)
                {
                    flagError("Incorrect SpecialKeys Format");
                    return new List<string>();
                }
            }
            set
            {
                string str = "( " + string.Join(",", value) + " )";
                data.Set("Keys", "SpecialKeys", str);
            }
        }

        public float Alpha
        {
            get
            {
                return ((float)data.Get("UI", "Alpha").ToDecimal());
            }
            set
            {
                data.Set("UI", "Alpha", value.ToString());
            }
        }

        public bool CanPlaySoundOnMouseOver
        {
            get
            {
                if (data.Get("UI", "CanPlaySoundOnMouseOver").ToString().ToLower() == "true")
                {
                    return (true);
                }
                else
                {
                    return (false);
                }
            }
            set
            {
                if (value)
                {
                    data.Set("UI", "CanPlaySoundOnMouseOver", "True");
                }
                else
                {
                    data.Set("UI", "CanPlaySoundOnMouseOver", "False");
                }
            }
        }

        public float TextScale
        {
            get
            {
                return ((float)data.Get("UI", "TextScale").ToDecimal());
            }
            set
            {
                data.Set("UI", "TextScale", value.ToString());
            }
        }

        public MyGuiControlTextboxStyleEnum VisualStyle
        {
            get
            {
                switch (data.Get("UI", "VisualStyle").ToString())
                {
                    case "Debug":
                        return MyGuiControlTextboxStyleEnum.Debug;
                    case "Default":
                        return MyGuiControlTextboxStyleEnum.Default;
                    case "Custom":
                        return MyGuiControlTextboxStyleEnum.Custom;
                    case "NoHighlight":
                        return MyGuiControlTextboxStyleEnum.NoHighlight;
                    default:
                        flagError("Invalid VisualStyle");
                        return MyGuiControlTextboxStyleEnum.Default;
                }
                
            }
            set
            {
                switch (value)
                {
                    case MyGuiControlTextboxStyleEnum.Debug:
                        data.Set("UI", "VisualStyle", "Debug");
                        break;
                    case MyGuiControlTextboxStyleEnum.Default:
                        data.Set("UI", "VisualStyle", "Default");
                        break;
                    case MyGuiControlTextboxStyleEnum.Custom:
                        data.Set("UI", "VisualStyle", "Custom");
                        break;
                    case MyGuiControlTextboxStyleEnum.NoHighlight:
                        data.Set("UI", "VisualStyle", "NoHighlight");
                        break;
                }
                
            }
        }

        public float PositionX
        {
            get
            {
                return ((float)data.Get("UI", "PositionX").ToDecimal());
            }
            set
            {
                data.Set("UI", "PositionX", value.ToString());
            }
        }

        public float PositionY
        {
            get
            {
                return ((float)data.Get("UI", "PositionY").ToDecimal());
            }
            set
            {
               data.Set("UI", "PositionY", value.ToString());
            }
        }

        public Vector2 Size
        {
            get
            {
                Vector2 x = new Vector2();
                string y = data.Get("UI","Size").ToString();
                try
                {
                    var z = y.Split(',');
                    List<float> results = new List<float>();
                    foreach (var item in z)
                    {
                        string temp = item;
                        int index = 0;
                        if ((index = item.IndexOf("(")) != -1)
                        {
                            temp = temp.Remove(index,1);
                        }
                        if ((index = item.IndexOf(")")) != -1)
                        {
                            temp = temp.Remove(index, 1);
                        }
                        temp = temp.Trim();
                        results.Add(float.Parse(temp));
                    }
                    x.X = results[0];
                    x.Y = results[1];
                    return x;
                }
                catch (Exception)
                {
                    flagError("Incorrect Size format");
                    return Size;
                }                
            }
            set
            {
                string str = "( " + value.X + " , " + value.Y + " )";
                data.Set("UI", "Size", str);
            }
        }

        public Vector4D ColorMask
        {
            get
            {
                Vector4D x = new Vector4D();
                string y = data.Get("UI", "ColorMask").ToString();
                try
                {
                    var z = y.Split(',');
                    List<float> results = new List<float>();
                    foreach (var item in z)
                    {
                        string temp = item;
                        int index = 0;
                        if ((index = item.IndexOf("(")) != -1)
                        {
                            temp = temp.Remove(index, 1);
                        }
                        if ((index = item.IndexOf(")")) != -1)
                        {
                            temp = temp.Remove(index, 1);
                        }
                        temp.Trim();
                        results.Add(float.Parse(temp));
                    }
                    x.X = results[0];
                    x.Y = results[1];
                    x.Z = results[2];
                    x.W = results[3];
                    return x;
                }
                catch (Exception)
                {
                    flagError("Incorrect ColorMask format");
                    return ColorMask;
                }
            }
            set
            {
                string str = "( " + value.X + " , " + value.Y + " , " + value.Z +" , "+ value.W + " )";
                data.Set("UI", "ColorMask", str);
            }
        }

        public Vector4D BorderColor
        {
            get
            {
                Vector4D x = new Vector4D();
                string y = data.Get("UI", "BorderColor").ToString();
                try
                {
                    var z = y.Split(',');
                    List<float> results = new List<float>();
                    foreach (var item in z)
                    {
                        string temp = item;
                        int index = 0;
                        if ((index = item.IndexOf("(")) != -1)
                        {
                            temp = temp.Remove(index, 1);
                        }
                        if ((index = item.IndexOf(")")) != -1)
                        {
                            temp = temp.Remove(index, 1);
                        }
                        temp.Trim();
                        results.Add(float.Parse(temp));
                    }
                    x.X = results[0];
                    x.Y = results[1];
                    x.Z = results[2];
                    x.W = results[3];
                    return x;
                }
                catch (Exception)
                {
                    flagError("Incorrect BorderColor format");
                    return BorderColor;
                }
            }
            set
            {
                string str = "( " + value.X + " , " + value.Y + " , " + value.Z + " , " + value.W + " )";
                data.Set("UI", "BorderColor", str);
            }
        }

        public List<string> GeneralKeys
        {
            get
            {

                List<string> Keys = new List<string>();
                string y = data.Get("Keys", "GeneralKeys").ToString();
                try
                {
                    List<string> z = new List<string>();
                    if (y.Contains(','))
                    {
                        z = y.Split(',').ToList();
                    }
                    else
                    {

                        z.Add(y);
                    }


                    foreach (var item in z)
                    {
                        string temp = item;
                        int index = 0;
                        index = temp.IndexOf("(");
                        if (index != -1)
                        {
                            temp = temp.Remove(index, 1);
                        }
                        index = temp.IndexOf(")");
                        if (index != -1)
                        {
                            temp = temp.Remove(index, 1);
                        }
                        temp.Trim();
                        Keys.Add(temp);
                    }
                    return Keys;
                }
                catch (Exception)
                {
                    flagError("Incorrect General Keys Format");
                    return new List<string>();
                }
            }
            set
            {
                string str = "( " + string.Join(",", value) + " )";
                data.Set("Keys", "GeneralKeys", str);
            }
        }
        public int ErrorCount
        {
            get
            {
                return (int)data.Get("Debug", "ErrorCount").ToInt32();
            }
            set
            {
                data.Set("Debug", "ErrorCount", value);
            }
        }

        public string lastError
        {
            get
            {
                return (data.Get("Debug", "Last Error").ToString());
            }
            set
            {
                data.Set("Debug", "Last Error", value);
            }
        }

        // public Methods

        public void Clear()
        {
            data.Clear();
        }

        public void Reset()
        {
            // INI Initialization and default values
            // SETUP
            data.Set("Setup", "CMDMode", "CMDTerminal");
            data.Set("Setup", "SessionID", randomizer.Next(10000));
            // STATES
            data.Set("States", "FirstRun", "True"); // Bool
            data.Set("States", "Text", "");
            data.Set("States", "User", "PLACEHOLDERUSER");
            data.Set("States", "CarriageIndex", "0");
            // Keys
            data.AddSection("Keys");
            // UI Configuration
            data.Set("UI", "Alpha", "1");// Float
            data.Set("UI", "CanPlaySoundOnMouseOver", "False"); // Bool
            data.Set("UI", "TextScale", "0.8"); // Float
            data.Set("UI", "VisualStyle", "Debug");
            data.Set("UI", "PositionX", "0.15"); // Float
            data.Set("UI", "PositionY", "0"); // Float
            data.Set("UI", "Size", "( 0.15 , 0.15 )"); // Vector2
            data.Set("UI", "ColorMask", "( 1 , 1 , 1 , 1 )"); // Vector4
            data.Set("UI", "BorderColor", "( 0 , 0 , 0 , 1 )"); // Vector4

            // Debug
            data.Set("Debug", "ErrorCount", "0"); // Int
            data.Set("Debug", "Last Error", "None");
        }

        public void randomizeSessionID()
        {
            SessionID = randomizer.Next(100000).ToString();
        }

        public string TryParse(String xmlstring)
        {
            if (xmlstring.ToLower() == "#terminal")
            {
                return "Initialise";
            }
            Packet temp = new Packet();
            MyIni copy = new MyIni();
            copy = data;
            if (temp.data.TryParse(xmlstring))
            {
                parser(temp);
                if (copy == data)
                {
                    return "UnChanged";
                }
                else
                {
                    return "Changed";
                }
            }
            else
            {
                return "Failed";
            }

        }

        public void flagError(string error)
        {
            lastError = error;
            ErrorCount++;
        }

        // public overrides

        public override string ToString()
        {
            return data.ToString();
        }

        // private Methods

        private void parser(Packet temp)
        {
            List<string> sections = new List<string>();
            temp.data.GetSections(sections);
            List<MyIniKey> keys = new List<MyIniKey>();
            List<MyIniKey> tempkeys = new List<MyIniKey>();

            foreach (var section in sections)
            {
                data.GetKeys(section, tempkeys);
                keys.AddList(tempkeys);
            }
            foreach (var key in keys)
            {
                string value = temp.data.Get(key.Section, key.Name).ToString();
                if (data.Get(key.Section, key.Name).ToString() != value)
                {
                    data.Set(key.Section, key.Name, value);
                }
            }

        }
    }
}
