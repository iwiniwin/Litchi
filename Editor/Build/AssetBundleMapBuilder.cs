#if ASSETGRAPH_1_7_OR_NEWER

using UnityEditor;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using V1=AssetBundleGraph;
using Model=UnityEngine.AssetGraph.DataModel.Version2;
using UnityEngine;
using UnityEngine.AssetGraph;
using FileUtil = UnityEngine.AssetGraph.FileUtility;

namespace Litchi {

	[CustomNode("Build/Build Asset Bundle Map", 90)]
	public class AssetBundleMapBuilder : Node, Model.NodeDataImporter {

		private static readonly string key = "0";

        [SerializeField] private SerializableMultiTargetString m_mapName;

		public override string ActiveStyle {
			get {
				return "node 5 on";
			}
		}

		public override string InactiveStyle {
			get {
				return "node 5";
			}
		}

		public override string Category {
			get {
				return "Build";
			}
		}

		public override Model.NodeOutputSemantics NodeInputType {
			get {
				return Model.NodeOutputSemantics.AssetBundles;
			}
		}

		public override Model.NodeOutputSemantics NodeOutputType {
			get {
				return Model.NodeOutputSemantics.AssetBundles;
			}
		}

		public override void Initialize(Model.NodeData data) {
            m_mapName = new SerializableMultiTargetString();

			data.AddDefaultInputPoint();
			data.AddDefaultOutputPoint();
		}

		public void Import(V1.NodeData v1, Model.NodeData v2) {
            m_mapName = new SerializableMultiTargetString();
		}
			
		public override Node Clone(Model.NodeData newData) {
			var newNode = new AssetBundleMapBuilder();
            newNode.m_mapName = new SerializableMultiTargetString (m_mapName);

			newData.AddDefaultInputPoint();
			newData.AddDefaultOutputPoint();

			return newNode;
		}

		public override void OnInspectorGUI(NodeGUI node, AssetReferenceStreamManager streamManager, NodeGUIEditor editor, Action onValueChanged) {

			EditorGUILayout.HelpBox("Build Asset Bundle Map: Build asset bundles with given asset bundle settings.", MessageType.Info);
			editor.UpdateNodeName(node);

			GUILayout.Space(8f);

			EditorGUILayout.HelpBox("Build Asset Bundle Map: Build asset bundles with given asset bundle settings.", MessageType.Info);

			var mapName = m_mapName[editor.CurrentEditingGroup];
			var newMapName = EditorGUILayout.TextField("Map Name", mapName);
			if(newMapName != mapName) {
				using(new RecordUndoScope("Change Map Name", node, true)){
					m_mapName[editor.CurrentEditingGroup] = newMapName;
					onValueChanged();
				}
			}
		}

		public override void Prepare (BuildTarget target, 
			Model.NodeData node, 
			IEnumerable<PerformGraph.AssetGroups> incoming, 
			IEnumerable<Model.ConnectionData> connectionsToOutput, 
			PerformGraph.Output Output) 
		{
			// BundleBuilder do nothing without incoming connections
			if(incoming == null) {
				return;
			}

			if(connectionsToOutput != null && Output != null) {
				UnityEngine.Assertions.Assert.IsTrue(connectionsToOutput.Any());
				var dst = (connectionsToOutput == null || !connectionsToOutput.Any())? 
					null : connectionsToOutput.First();

				var ag = incoming.First();
				var output = new Dictionary<string, List<AssetReference>>();
				output[key] = new List<AssetReference>();
				output[key].AddRange(ag.assetGroups[key]);

				var bundleOutputDir = GetOutputDirectory(ag.assetGroups[key]);
				var mapName = GetMapName(target, node, bundleOutputDir);
				var outputPath = FileUtil.PathCombine(bundleOutputDir, mapName);
				output[key].Add(AssetReferenceDatabase.GetReferenceWithType(outputPath, typeof(TextAsset)));

				Output(dst, output);
			}
		}
		
		public override void Build (BuildTarget target, 
			Model.NodeData node, 
			IEnumerable<PerformGraph.AssetGroups> incoming, 
			IEnumerable<Model.ConnectionData> connectionsToOutput, 
			PerformGraph.Output Output,
			Action<Model.NodeData, string, float> progressFunc) 
		{
			if(incoming == null) {
				return;
			}

			var bundleNames = new HashSet<string>();

			if(progressFunc != null) progressFunc(node, "Collecting all inputs...", 0f);

			var ag = incoming.First();
			foreach(var name in ag.assetGroups.Keys) {
				bundleNames.UnionWith(ag.assetGroups[name].Select(v => v.fileName));
			}


            var bundleOutputDir = GetOutputDirectory(ag.assetGroups[key]);
			var mapName = GetMapName(target, node, bundleOutputDir);
			var outputPath = Path.Combine(bundleOutputDir, mapName);

			if(progressFunc != null) progressFunc(node, "Building Asset Bundle Map...", 0.7f);

			BuildAssetBundleMap(outputPath, bundleNames);

			if(Output != null) {
				var dst = (connectionsToOutput == null || !connectionsToOutput.Any())? 
					null : connectionsToOutput.First();
				var output = new Dictionary<string, List<AssetReference>>();
				output[key] = new List<AssetReference>();
				output[key].AddRange(ag.assetGroups[key]);
				output[key].Add(AssetReferenceDatabase.GetReferenceWithType(outputPath, typeof(TextAsset)));
				Output(dst, output);
			}
		}

		private void BuildAssetBundleMap(string outputPath, HashSet<string> bundleNames)
		{
			AssetBundleBuildMap map = AssetBundleBuildMap.GetBuildMap();
			Dictionary<string, List<string>> assetAndBundlesMap = new Dictionary<string, List<string>>();
			foreach(var bundleName in bundleNames)
			{
				string[] assetPaths = map.GetAssetPathsFromAssetBundle(bundleName);
				foreach(var assetPath in assetPaths)
				{
					List<string> bundles;
					if(!assetAndBundlesMap.TryGetValue(assetPath, out bundles))
					{
						bundles = new List<string>();
						assetAndBundlesMap[assetPath] = bundles;
					}
					bundles.Add(bundleName);
				}
			}
			AssetBundleUtility.CreateAssetBundleMap(assetAndBundlesMap, outputPath);
		}

		private string GetOutputDirectory(List<AssetReference> list)
		{
			return Path.GetDirectoryName(list.First().path);
		}

        private string GetMapName(BuildTarget target, Model.NodeData node, string outputDir) {
            if (!string.IsNullOrEmpty (m_mapName [target])) {
                return m_mapName [target];
            } else {
                return Path.GetFileName(outputDir) + ".xml"; 
            }
        }

        private string PrepareOutputDirectory(BuildTarget target, Model.NodeData node, bool autoCreate, bool throwException) {

            return "";
        }
	}
}
#endif