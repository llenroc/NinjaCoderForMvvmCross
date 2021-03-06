﻿// --------------------------------------------------------------------------------------------------------------------
// <summary>
//    Defines the ApplicationService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace NinjaCoder.MvvmCross.Services
{
    using Constants;
    using Entities;
    using Interfaces;
    using Scorchio.Infrastructure.Translators;
    using Scorchio.VisualStudio.Entities;
    using Scorchio.VisualStudio.Services;
    using Scorchio.VisualStudio.Services.Interfaces;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    using Microsoft.Win32;

    /// <summary>
    /// Defines the ApplicationService type.
    /// </summary>
    public class ApplicationService : IApplicationService
    {
        /// <summary>
        /// The visual studio service.
        /// </summary>
        private readonly IVisualStudioService visualStudioService;

        /// <summary>
        /// The settings service.
        /// </summary>
        private readonly ISettingsService settingsService;

        /// <summary>
        /// The commands list translator.
        /// </summary>
        private readonly ITranslator<string, CommandsList> commandsListTranslator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationService" /> class.
        /// </summary>
        /// <param name="visualStudioService">The visual studio service.</param>
        /// <param name="settingsService">The settings service.</param>
        /// <param name="commandsListTranslator">The commands list translator.</param>
        public ApplicationService(
            IVisualStudioService visualStudioService,
            ISettingsService settingsService,
            ITranslator<string, CommandsList> commandsListTranslator)
        {
            this.visualStudioService = visualStudioService;
            this.settingsService = settingsService;
            this.commandsListTranslator = commandsListTranslator;
        }

        /// <summary>
        /// Views the log file.
        /// </summary>
        public void ViewLogFile()
        {
            TraceService.WriteLine("ApplicationService::ViewLogFile");

            string logFilePath = this.settingsService.LogFilePath;

            if (File.Exists(logFilePath) == false)
            {
                File.Create(logFilePath);
            }

            Process.Start(logFilePath);
        }

        /// <summary>
        /// Clears the log file.
        /// </summary>
        public void ClearLogFile()
        {
            TraceService.WriteLine("ApplicationService::ClearLogFile");

            if (File.Exists(this.settingsService.LogFilePath))
            {
                File.Delete(this.settingsService.LogFilePath);
            }
        }

        /// <summary>
        /// Views the error log file.
        /// </summary>
        public void ViewErrorLogFile()
        {
            TraceService.WriteLine("ApplicationService::ViewLogFile");

            string filePath = this.settingsService.ErrorFilePath;

            if (File.Exists(filePath) == false)
            {
                File.Create(filePath);
            }

            Process.Start(filePath);
        }

        /// <summary>
        /// Clears the error log file.
        /// </summary>
        public void ClearErrorLogFile()
        {
            TraceService.WriteLine("ApplicationService::ClearErrorLogFile");

            if (File.Exists(this.settingsService.ErrorFilePath))
            {
                File.Delete(this.settingsService.ErrorFilePath);
            }
        }

        /// <summary>
        /// Shows the MVVM cross home page.
        /// </summary>
        public void ShowMvvmCrossHomePage()
        {
            TraceService.WriteLine("ApplicationService::ShowMvvmCrossHomePage");

            Process.Start(this.settingsService.MvvmCrossHomePage);
        }

        /// <summary>
        /// Shows the xamarin forms home page.
        /// </summary>
        public void ShowXamarinFormsHomePage()
        {
            TraceService.WriteLine("ApplicationService::ShowXamarinFormsHomePage");

            Process.Start(this.settingsService.XamarinFormsHomePage);
        }

        /// <summary>
        /// Opens the installation directory.
        /// </summary>
        public void OpenInstallationDirectory()
        {
            TraceService.WriteLine("ApplicationService::OpenWorkingDirectory");

            string installationDirectory = this.GetInstallationDirectory();

            if (installationDirectory != string.Empty)
            {
               Process.Start(installationDirectory);
            }
        }

        /// <summary>
        /// Gets the application framework.
        /// </summary>
        /// <returns>The Framework Type.</returns>
        public FrameworkType GetApplicationFramework()
        {
            FrameworkType frameworkType = this.visualStudioService.GetFrameworkType();

            if (frameworkType == FrameworkType.NotSet)
            {
                //// new solution
                frameworkType = this.settingsService.FrameworkType;
            }

            return frameworkType;
        }

        /// <summary>
        /// Suspends the resharper if requested.
        /// </summary>
        public void SuspendResharperIfRequested()
        {
            if (this.settingsService.SuspendReSharperDuringBuild)
            {
                TraceService.WriteLine("SuspendResharper");

                try
                {
                    //// this could fail so catch exception.
                    this.visualStudioService.DTEService.ExecuteCommand(Settings.SuspendReSharperCommand);
                }
                catch (Exception exception)
                {
                    TraceService.WriteError("Error Suspending ReSharper exception=" + exception.Message);
                }
            }
        }

        /// <summary>
        /// Fixes the information p list.
        /// </summary>
        /// <param name="projectTemplateInfo">The project template information.</param>
        public void FixInfoPList(ProjectTemplateInfo projectTemplateInfo)
        {
            TraceService.WriteLine("ApplicationService::FixInfoPlist");

            IProjectService iosProjectService = this.visualStudioService.iOSProjectService;

            if (iosProjectService != null)
            {
                if (projectTemplateInfo != null)
                {
                    IProjectItemService projectItemService = iosProjectService.GetProjectItem("Info.plist");

                    if (projectItemService != null)
                    {
                        XDocument doc = XDocument.Load(projectItemService.FileName);

                        if (doc.Root != null)
                        {
                            XElement element = doc.Root.Element("dict");

                            if (element != null)
                            {
                                //// first look for the elements

                                XElement childElement = element.Elements("key").FirstOrDefault(x => x.Value == "CFBundleDisplayName");

                                if (childElement == null)
                                {
                                    element.Add(new XElement("key", "CFBundleDisplayName"));
                                    element.Add(new XElement("string", iosProjectService.Name));
                                }

                                childElement = element.Elements("key").FirstOrDefault(x => x.Value == "CFBundleVersion");

                                if (childElement == null)
                                {
                                    element.Add(new XElement("key", "CFBundleVersion"));
                                    element.Add(new XElement("string", "1.0"));
                                }

                                childElement = element.Elements("key").FirstOrDefault(x => x.Value == "CFBundleIdentifier");

                                if (childElement == null)
                                {
                                    element.Add(new XElement("key", "CFBundleIdentifier"));
                                    element.Add(new XElement("string", "1"));
                                }
                            }

                            doc.Save(projectItemService.FileName);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the commands list.
        /// </summary>
        /// <returns>A CommandsList.</returns>
        public CommandsList GetCommandsList()
        {
            return this.commandsListTranslator.Translate(this.settingsService.ApplicationCommandsUri);
        }

        /// <summary>
        /// Sets the working directory.
        /// </summary>
        /// <param name="path">The path.</param>
        public void SetWorkingDirectory(string path)
        {
            this.settingsService.WorkingDirectory = path;
        }

        /// <summary>
        /// Gets the installation directory.
        /// </summary>
        /// <returns>The working directory.</returns>
        public string GetInstallationDirectory()
        {
            RegistryKey softwareKey = Registry.CurrentUser.OpenSubKey("Software");

            RegistryKey microsoftKey = softwareKey?.OpenSubKey("Microsoft");

            RegistryKey vsKey = microsoftKey?.OpenSubKey("VisualStudio");

            RegistryKey versionKey = vsKey?.OpenSubKey("14.0");

            RegistryKey extensionManagerKey = versionKey?.OpenSubKey("ExtensionManager");

            RegistryKey enabledExtensionsKey = extensionManagerKey?.OpenSubKey("EnabledExtensions");

            if (enabledExtensionsKey != null)
            {
                string[] valueNames = enabledExtensionsKey.GetValueNames();

                foreach (string valueName in valueNames)
                {
                    if (valueName.StartsWith("NinjaCoderMvvmCross.vsix"))
                    {
                        return enabledExtensionsKey.GetValue(valueName) as string;
                    }
                }
            }

            return string.Empty;
        }
    }
}
