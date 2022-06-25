using System;
using System.Collections.Generic;
using System.Xml;

namespace ByPass
{
    public static class Config
    {
        private const string CONFIG_FILE_NAME = @"C:\Recycler.xml";

        public static int NoteCountLength { set; get; } = 2;
        //public static int LastTxStatusMaxLength { set; get; } = 26;

        public static IDictionary<string, XmlNode> ConfigurationParameters { set; get; } = new Dictionary<string, XmlNode>();
        public static IDictionary<string, XmlNode> StateDefinitions { set; get; } = new Dictionary<string, XmlNode>();
        public static IDictionary<string, XmlNode> NoteMappings { set; get; } = new Dictionary<string, XmlNode>();
        public static IDictionary<string, XmlNode> Devices { set; get; } = new Dictionary<string, XmlNode>();

        static Config()
        {
            try
            {
                var doc = new XmlDocument();

                doc.Load(CONFIG_FILE_NAME);

                NoteCountLength = ReadIntegerValue(doc, "/Config/NoteCountLength", NoteCountLength);
                //LastTxStatusMaxLength = ReadIntegerValue(doc, "/Config/LastTxStatusMaxLength", LastTxStatusMaxLength);

                ConfigurationParameters = ReadDictionaryValue(doc, "/Config/Parameters/Option", "Number");
                StateDefinitions = ReadDictionaryValue(doc, "/Config/StateDefinitions/State", "Number");
                NoteMappings = ReadDictionaryValue(doc, "/Config/NoteMappings/Note", "ID");
                Devices = ReadDictionaryValue(doc, "/Config/Devices/Message", "ID");
            }
            catch (Exception ex)
            {
                Logger.Log($"Error reading configuration: {ex.Message}");
            }
        }

        private static int ReadIntegerValue(XmlDocument doc, string xpath, int defaultValue)
        {
            Logger.Log("Start reading integer value");

            var node = doc.SelectSingleNode(xpath);

            if (node != null)
            {
                Logger.Log($"Found '{xpath}' node");

                if (int.TryParse(node.InnerText, out int val))
                {
                    Logger.Log($"Reading integer value: {val}");

                    return val;
                }
                else
                {
                    Logger.Log("Unable to parse integer value");
                }
            }
            else
            {
                Logger.Log($"Specified node '{xpath}' not found");
            }

            return defaultValue;
        }

        private static IDictionary<string, XmlNode> ReadDictionaryValue(XmlDocument doc, string xpath, string key)
        {
            Logger.Log("Start reading dictionary");

            var dict = new Dictionary<string, XmlNode>();
            var nodes = doc.SelectNodes(xpath);

            if (nodes != null)
            {
                foreach (XmlNode node in nodes)
                {
                    var keyAttr = node.Attributes[key];
                    
                    if (keyAttr != null)
                    {
                        Logger.Log($"Added new node for key '{keyAttr.Value}'");

                        dict.Add(keyAttr.Value, node);
                    }
                    else
                    {
                        Logger.Log("Key attribute not found");
                    }
                }
            }
            else
            {
                Logger.Log("Nodes for dictionary not found");
            }

            return dict;
        }
    }
}
