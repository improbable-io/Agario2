using System;
using System.Collections;
using System.Collections.Generic;
using Improbable.Core;
using Improbable.Fapi.Receptionist;
using Improbable.Unity.Entity;
using Improbable.Unity.Logging;
using Improbable.Unity.Visualizer;
using IoC;
using log4net;
using Newtonsoft.Json;
using UnityEngine;

namespace Improbable.Unity.Core
{
    /// <summary>
    ///     Manages the tasks required to take an engine through its bootstrap lifecycle.
    ///     Is responsible for calling back into the <code>BootstrapHandler</code> that the
    ///     user has implemented.
    /// </summary>
    public class EngineLifecycleManager : MonoBehaviour
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(EngineLifecycleManager));

        private IBootstrapHandler bootstrapHandler;
        private IDeployment chosenDeployment;
        private string queuingCompleteToken;
        private IContainer container;
        private IEntityTemplateProvider entityTemplateProvider;

        private int pendingTaskCount;

        /// <summary>
        ///     Start the boostrap process which results in connecting to the Fabric.  This will look for the asset loader on the
        ///     boot game object, or will default to the DefaultTemplateProvider.
        /// </summary>
        /// <param name="bootstrapHandler">Code that receives callbacks about the bootstrap process</param>
        /// <param name="gameObject">
        ///     The game object that acts as the point of initialisation for the Improbable Fabric connection
        ///     within the scene.
        /// </param>
        /// <param name="engineConfiguration">Used to provided user specific settings to the bootstrap process</param>
        [Obsolete("EngineConfiguration is now a singleton, you do not have to specify it.")]
        public static void StartGame(IBootstrapHandler bootstrapHandler, GameObject gameObject, EngineConfiguration engineConfiguration)
        {
            StartGame(bootstrapHandler, gameObject);
        }

        [Obsolete("EngineConfiguration is now a singleton, you do not have to specify it.")]
        public static void StartGame(IBootstrapHandler bootstrapHandler, GameObject gameObject, EngineConfiguration engineConfiguration, IEntityTemplateProvider templateProvider)
        {
            StartGame(bootstrapHandler, gameObject, templateProvider);
        }

        /// <summary>
        ///     Start the boostrap process which results in connecting to the Fabric.  This will look for the asset loader on the
        ///     boot game object, or will default to the DefaultTemplateProvider.
        /// </summary>
        /// <param name="bootstrapHandler">Code that receives callbacks about the bootstrap process</param>
        /// <param name="gameObject">
        ///     The game object that acts as the point of initialisation for the Improbable Fabric connection
        ///     within the scene.
        /// </param>
        public static void StartGame(IBootstrapHandler bootstrapHandler, GameObject gameObject)
        {
            var templateProvider = gameObject.GetComponent<IEntityTemplateProvider>() ?? gameObject.AddComponent<DefaultTemplateProvider>();
            StartGame(bootstrapHandler, gameObject, templateProvider);
        }

        public static void StartGame(IBootstrapHandler bootstrapHandler, GameObject gameObject, IEntityTemplateProvider templateProvider)
        {
            SetupDebugLogger();

            // Verify that AppName has been set
            if (!EngineConfiguration.Instance.IsAppNameSet)
            {
                throw new ArgumentException(String.Format("The AppName must be set in your EngineConfiguration, {0}, or via command-line argument +{1}",
                    System.IO.Path.GetFileName(EngineConfiguration.Instance.ProjectDescriptorPath),
                    ConfigNames.APP_NAME));
            }

            var lifecycle = gameObject.AddComponent<EngineLifecycleManager>();
            lifecycle.StartEngine(bootstrapHandler, templateProvider);
        }

        private static void SetupDebugLogger()
        {
            if (EngineConfiguration.Instance.Log4netConfigXml.Length > 0)
            {
                LoggerConfigurationLoader.LoadConfigFile(EngineConfiguration.Instance.Log4netConfigXml);
            }
            else
            {
                throw new Exception("No log4net configuration specified!");
            }
            if (EngineConfiguration.Instance.IsDebugMode)
            {
                System.Diagnostics.Debug.Listeners.Add(new UnityTraceListener());
            }
        }

        /// <summary>
        ///     Starts the bootstrap process with a given configuration.  Should be called when you are
        ///     ready to begin connecting to the Fabric.
        /// </summary>
        private void StartEngine(IBootstrapHandler handler, IEntityTemplateProvider provider)
        {
            entityTemplateProvider = provider;
            bootstrapHandler = handler;

            if (bootstrapHandler == null)
            {
                throw new Exception("No BootstrapHandler component found.");
            }

            Debug.LogFormat("Using {0} to provide templates.", provider.GetType().Name);

            if (!string.IsNullOrEmpty(EngineConfiguration.Instance.SteamToken))
            {
                ExchangeSteamToken();
            }
            else if (!string.IsNullOrEmpty(EngineConfiguration.Instance.LoginToken))
            {
                StartCoroutine(GetDeploymentList(EngineConfiguration.Instance.LoginToken));
            }
            else if (!string.IsNullOrEmpty(EngineConfiguration.Instance.Ip))
            {
                var localDeployment = new LocalDeployment();
                InitiatePreConnection(localDeployment);
            }
            else
            {
                bootstrapHandler.OnBootstrapError(new Exception("Invalid parameters provided"));
            }
        }

        private void ExchangeSteamToken()
        {
            var tokenExchanger = gameObject.AddComponent<SteamTokenExchange>();
            tokenExchanger.ExchangeSteamTokenForLoginToken(
                                                           EngineConfiguration.Instance.SteamToken,
                                                           EngineConfiguration.Instance.InfraLocatorUrl,
                                                           EngineConfiguration.Instance.AppName,
                                                           EngineConfiguration.Instance.DeploymentTag,
                                                           token =>
                                                           {
                                                               EngineConfiguration.Instance.LoginToken = token;
                                                               StartCoroutine(GetDeploymentList(EngineConfiguration.Instance.LoginToken));
                                                           },
                                                           bootstrapHandler.OnBootstrapError);
        }

        private IEnumerator GetDeploymentList(string loginToken)
        {
            Logger.Debug("Getting deployment list");

            var headers = new Dictionary<string, string>
            {
                { "Authorization", "bearer " + loginToken },
                { "Accept", "application/json" }
            };
            var www = new WWW(EngineConfiguration.Instance.InfraLocatorUrl + "/locator/v2/deployments/" + EngineConfiguration.Instance.AppName, null, headers);
            yield return www;

            if (www.error != null)
            {
                bootstrapHandler.OnBootstrapError(new Exception("Failed to retrieve deployment list: " + www.text));
            }
            else
            {
                var deploymentList = JsonConvert.DeserializeObject<CloudDeploymentList>(www.text);
                OnDeploymentListRetrieved(deploymentList.Deployments);
            }
        }

        private void OnDeploymentListRetrieved(IList<IDeployment> deploymentList)
        {
            Logger.Debug("Deployment list retrieved");
            bootstrapHandler.OnDeploymentListRetrieved(deploymentList, OnDeploymentChosen);
        }

        private void OnDeploymentChosen(IDeployment deployment)
        {
            Logger.DebugFormat("OnDeploymentChosen name={0}, appName={1}, assemblyName={2}", deployment.Name, deployment.AppName, deployment.AssemblyName);

            EngineConfiguration.Instance.AssemblyName = deployment.AssemblyName;

            InitiateQueueing(deployment);
            InitiatePreConnection(deployment);
        }

        private void OnPreConnectionStageComplete()
        {
            Logger.Debug("OnPreConnectionStageComplete");
            FinishPendingTask(ConnectToFabric);
        }

        private void OnQueueEntered()
        {
            Logger.Debug("OnQueueEntered");
            bootstrapHandler.OnQueuingStarted();
        }

        private void OnQueueingStatusUpdate(IQueueStatus status)
        {
            Logger.Debug("OnQueueingStatusUpdate status=" + status.Status + " token=" + status.QueueToken);
            bootstrapHandler.OnQueuingUpdate(status);
        }

        private void OnQueueingComplete(IQueueStatus status)
        {
            Logger.Debug("OnQueueingComplete token=" + status.QueueToken);
            queuingCompleteToken = status.QueueToken;
            bootstrapHandler.OnQueuingCompleted(status);
            FinishPendingTask(ConnectToFabric);
        }

        private void InitiateQueueing(IDeployment deployment)
        {
            StartPendingTask();

            Logger.Debug("InitiateQueueing");

            var queueManager = gameObject.AddComponent<QueueManager>();
            queueManager.StartQueueing(EngineConfiguration.Instance.LoginToken,
                                       deployment.QueueingUrl,
                                       OnQueueEntered,
                                       OnQueueingStatusUpdate,
                                       OnQueueingComplete);
        }

        private void InitiatePreConnection(IDeployment deployment)
        {
            StartPendingTask();

            chosenDeployment = deployment;

            container = new UnityContainer(VisualizerMetadataLookup.Instance.StaticInjectionCache);
            container.Bind<IDeployment>().AsSingle(deployment);
            container.Bind<IEntityTemplateProvider>().AsSingle(entityTemplateProvider);

            container.Inject(entityTemplateProvider);

            Logger.Debug("InitiatePreConnection deployment=" + deployment.Name);
            bootstrapHandler.BeginPreconnectionTasks(deployment, container, OnPreConnectionStageComplete);
        }

        private void ConnectToFabric()
        {
            Logger.Debug("ConnectToFabric");

            var gameRoot = new GameRoot(gameObject, container, entityTemplateProvider, chosenDeployment, queuingCompleteToken);
            gameRoot.Start();
        }

        private void StartPendingTask()
        {
            pendingTaskCount++;
        }

        private void FinishPendingTask(Action allTasksFinished)
        {
            pendingTaskCount--;
            if (pendingTaskCount < 0)
            {
                throw new InvalidOperationException("FinishPendingTask has been called too many times.");
            }

            if (pendingTaskCount == 0)
            {
                allTasksFinished();
            }
        }
    }
}
