using UnityEditor;
using UnityEditor.Build.Reporting;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Litchi.Editor.Build
{
    public class BuildSystem
    {
        private static List<IModifyPlayerSettings> m_ModifySettingsList = new List<IModifyPlayerSettings>();
        private static List<IBeforeBuild> m_BeforeBuildList = new List<IBeforeBuild>();
        private static List<IAfterBuild> m_AfterBuildList = new List<IAfterBuild>();
        private static List<IFinallyBuild> m_FinallyBuildList = new List<IFinallyBuild>();

        public static void BuildPlayer()
        {
            BuildPlayerOptions options = new BuildPlayerOptions();
            BuildReport buildReport = BuildPipeline.BuildPlayer(options);

            bool buildSuccess = buildReport.summary.result == BuildResult.Succeeded;

        }

        private static void InitBuildSteps()
        {
            m_ModifySettingsList.Clear();
            m_BeforeBuildList.Clear();
            m_AfterBuildList.Clear();
            m_FinallyBuildList.Clear();


        }

        private static void LoadBuildSteps<T>(string rootName, ref List<T> list)
        {

        }

        private static XmlDocument LoadBuildXmlConfig()
        {
            try
            {

            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}