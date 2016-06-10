using System;
using Improbable.Assets;
using Improbable.Core;

namespace Improbable.Unity.Core
{
    internal class LocalDeployment : IDeployment
    {
        private readonly string ip;
        private readonly string assemblyName;

        internal LocalDeployment()
        {
            ReceptionistUrl = string.Format("http://{0}:{1}/login", EngineConfiguration.Instance.Ip, EngineConfiguration.Instance.Port);
            if (!string.IsNullOrEmpty(EngineConfiguration.Instance.AppName) && !string.IsNullOrEmpty(EngineConfiguration.Instance.AssemblyName))
            {
                AssetDatabaseStrategy = AssetDatabaseStrategy.Streaming;
            }
            else
            {
                AssetDatabaseStrategy = AssetDatabaseStrategy.Local;
            }

            if (!string.IsNullOrEmpty(EngineConfiguration.Instance.AppName))
            {
                AppName = EngineConfiguration.Instance.AppName;
            }
            else
            {
                AppName = "local_app";
            }

            assemblyName = EngineConfiguration.Instance.AssemblyName;
        }

        public string Name
        {
            get { return "local"; }
        }

        public string AppName { get; private set; }

        public string AssemblyName
        {
            get
            {
                if (AssetDatabaseStrategy == AssetDatabaseStrategy.Local)
                {
                    throw new NotImplementedException("This method should not be called on a local fake deployment");
                }
                return assemblyName;
            }
        }

        public string ReceptionistUrl { get; private set; }

        public string QueueingUrl
        {
            get { throw new NotImplementedException("This method should not be called on a local fake deployment"); }
        }

        public AssetDatabaseStrategy AssetDatabaseStrategy { get; private set; }
    }
}