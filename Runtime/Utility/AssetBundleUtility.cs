using System.Xml;
using System.Collections.Generic;

namespace Litchi
{
    public static class AssetBundleUtility
    {
        public static void CreateAssetBundleMap(Dictionary<string, List<string>> assetAndBundlesMap, string outputPath)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement rootElement = doc.CreateElement("", "Assets", "");
            doc.AppendChild(rootElement);
            foreach (var pair in assetAndBundlesMap)
            {
                XmlElement element = doc.CreateElement("Asset");
                element.SetAttribute("path", pair.Key);

                foreach (var bundleName in pair.Value)
                {
                    XmlElement ele = doc.CreateElement("AssetBundle");
                    ele.InnerText = bundleName;
                    element.AppendChild(ele);
                }
                rootElement.AppendChild(element);
            }
            doc.Save(outputPath);
        }

        public static Dictionary<string, string> LoadAssetBundleMap(string path)
        {
            Dictionary<string, string> assetBundleMap = new Dictionary<string, string>();
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNode rootNode = doc.SelectSingleNode("Assets");
            foreach(XmlNode assetNode in rootNode.ChildNodes)
            {
                if(assetNode.Attributes == null)
                {
                    continue;
                }
                var assetElement = assetNode as XmlElement; 
                string assetPath = assetElement.GetAttribute("path");
                XmlNode bundleNode = assetElement.FirstChild;
                if(bundleNode.Attributes == null)
                {
                    continue;
                }
                var bundleElement = bundleNode as XmlElement;
                assetBundleMap.Add(assetPath, bundleElement.InnerText);
            }
            return assetBundleMap;
        }
    }
}