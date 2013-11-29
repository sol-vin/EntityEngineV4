using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using EntityEngineV4.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine.Services
{
    public class AssetCollector : Service
    {
        private Dictionary<string,object> _assets = new Dictionary<string,object>();

        public AssetCollector(State state) : base(state, "AssetCollector")
        {
        }

        public void AddAsset<T>(string name, string location)
        {
            //Check if our asset is of XNA data types
            if (typeof (T) == typeof (Texture2D) || typeof (T) == typeof (SoundEffect))
            {
                _assets.Add(name, EntityGame.Self.Content.Load<T>(location));
            }
            else if (typeof (T) == typeof (XDocument)) //Check if the requested type is a XML file
            {
                _assets.Add(name, XDocument.Load(location));
            }
        }

        public bool DeleteAsset(string name)
        {
            return _assets.Remove(name);
        }

        public T GetAsset<T>(string name)
        {
            if (typeof (T) != _assets[name].GetType())
                throw new Exception(String.Format("Asset {0} was found to be of type {1} and not of expected type {2}",
                    name, _assets[name].GetType(), typeof(T)));
            return (T) _assets[name];
        }

        public void LoadXML(string path)
        {
            XDocument root = XDocument.Load(path);
            foreach (var element in root.Descendants())
            {
                switch (element.Name.ToString())
                {
                    case "Texture2D":
                        AddAsset<Texture2D>(element.Attribute("name").Value, element.Value);
                        break;
                    case "SoundEffect":
                        AddAsset<SoundEffect>(element.Attribute("name").Value, element.Value);
                        break;
                    case "XDocument":
                        AddAsset<XDocument>(element.Attribute("name").Value, element.Value);
                        break;
                }
            }
        }
    }
}
