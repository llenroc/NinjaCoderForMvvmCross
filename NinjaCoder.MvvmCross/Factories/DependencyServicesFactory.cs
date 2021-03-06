﻿// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Defines the DependencyServicesFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace NinjaCoder.MvvmCross.Factories
{
    using Entities;
    using Interfaces;
    using Scorchio.Infrastructure.Wpf.ViewModels.Wizard;
    using Scorchio.VisualStudio.Entities;
    using Scorchio.VisualStudio.Services;
    using Scorchio.VisualStudio.Services.Interfaces;
    using Services.Interfaces;
    using System.Collections.Generic;
    using UserControls.AddDependencyServices;
    using ViewModels.AddDependencyServices;

    /// <summary>
    /// Defines the DependencyServicesFactory type.
    /// </summary>
    public class DependencyServicesFactory : BaseTextTemplateFactory, IDependencyServicesFactory
    {
        /// <summary>
        /// The visual studio service.
        /// </summary>
        private readonly IVisualStudioService visualStudioService;

        /// <summary>
        /// The resolver service.
        /// </summary>
        private readonly IResolverService resolverService;

        /// <summary>
        /// The register service.
        /// </summary>
        private readonly IRegisterService registerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyServicesFactory"/> class.
        /// </summary>
        /// <param name="visualStudioService">The visual studio service.</param>
        /// <param name="settingsService">The settings service.</param>
        /// <param name="resolverService">The resolver service.</param>
        /// <param name="registerService">The register service.</param>
        public DependencyServicesFactory(
            IVisualStudioService visualStudioService,
            ISettingsService settingsService,
            IResolverService resolverService,
            IRegisterService registerService)
            : base(settingsService)
        {
            TraceService.WriteLine("DependencyServicesFactory::Constructor");

            this.visualStudioService = visualStudioService;
            this.resolverService = resolverService;
            this.registerService = registerService;
        }

        /// <summary>
        /// Gets the wizards steps.
        /// </summary>
        /// <returns>
        /// The wizard steps.
        /// </returns>
        public List<WizardStepViewModel> GetWizardsSteps()
        {
            List<WizardStepViewModel> wizardSteps = new List<WizardStepViewModel>
            {
                new WizardStepViewModel
                {
                    ViewModel = this.resolverService.Resolve<DependencyServiceViewModel>(),
                    ViewType = typeof(DependencyServiceControl)
                },
                new WizardStepViewModel
                    {
                        ViewModel = this.resolverService.Resolve<DependencyServiceFinishedViewModel>(),
                        ViewType = typeof(DependencyServiceFinishedControl)
                    }
            };

            return wizardSteps;
        }

        /// <summary>
        /// Registers the wizard data.
        /// </summary>
        public void RegisterWizardData()
        {
            WizardData wizardData = new WizardData
            {
                WindowTitle = "Add Xamarin Forms Dependency Service",
                WindowHeight = 600,
                WindowWidth = 600,
                WizardSteps = this.GetWizardsSteps()
            };

            this.registerService.Register<IWizardData>(wizardData);
        }

        /// <summary>
        /// Gets the text templates.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="methodComment">The method comment.</param>
        /// <param name="methodReturnType">Type of the method return.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="directory">The directory.</param>
        /// <returns>A list of TextTemplateInfos.</returns>
        public IEnumerable<TextTemplateInfo> GetTextTemplates(
            string name,
            string methodComment,
            string methodReturnType,
            string methodName,
            string directory)
        {
           TraceService.WriteLine("DependencyServicesFactory::GetTextTemplates");

            List<TextTemplateInfo> textTemplates = new List<TextTemplateInfo>();

            Dictionary<string, string> baseDictionary = this.GetBaseDictionary(
                name,
                methodComment,
                methodReturnType,
                methodName,
                directory);

            IProjectService formsProjectService = this.visualStudioService.XamarinFormsProjectService;

            if (formsProjectService != null)
            {
                TraceService.WriteLine("building formsProject textTemplate");

                textTemplates.Add(
                    this.GetTextTemplateInfo(
                        formsProjectService,
                        this.SettingsService.IDependencyTextTemplate,
                        "I" + name,
                        directory,
                        ProjectSuffix.XamarinForms,
                        this.SettingsService.XamarinFormsProjectSuffix,
                        baseDictionary,
                        true));
            }

            // ReSharper disable once InconsistentNaming
            IProjectService iOSProjectService = this.visualStudioService.iOSProjectService;

            if (iOSProjectService != null)
            {
                TraceService.WriteLine("building iosProject textTemplate");

                textTemplates.Add(
                    this.GetTextTemplateInfo(
                        iOSProjectService,
                        this.SettingsService.DependencyTextTemplate,
                        name,
                        directory,
                        ProjectSuffix.iOS,
                        this.SettingsService.iOSProjectSuffix,
                        baseDictionary,
                        true));
            }

            IProjectService windowsPhoneProjectService = this.visualStudioService.WindowsPhoneProjectService;

            if (windowsPhoneProjectService != null)
            {
                TraceService.WriteLine("building windowsPhoneProject textTemplate");

                textTemplates.Add(
                    this.GetTextTemplateInfo(
                        windowsPhoneProjectService,
                        this.SettingsService.DependencyTextTemplate,
                        name,
                        directory,
                        ProjectSuffix.WindowsPhone,
                        this.SettingsService.WindowsPhoneProjectSuffix,
                        baseDictionary,
                        true));
            }

            IProjectService droidProjectService = this.visualStudioService.DroidProjectService;

            if (droidProjectService != null)
            {
                TraceService.WriteLine("building droidProject textTemplate");

                textTemplates.Add(
                    this.GetTextTemplateInfo(
                        droidProjectService,
                        this.SettingsService.DependencyTextTemplate,
                        name,
                        directory,
                        ProjectSuffix.Droid,
                        this.SettingsService.DroidProjectSuffix,
                        baseDictionary,
                        true));
            }

            IProjectService universalProjectService = this.visualStudioService.WindowsUniversalProjectService;

            if (universalProjectService != null)
            {
                TraceService.WriteLine("building universalProject textTemplate");

                textTemplates.Add(
                    this.GetTextTemplateInfo(
                        universalProjectService,
                        this.SettingsService.DependencyTextTemplate,
                        name,
                        directory,
                        ProjectSuffix.WindowsUniversal,
                        this.SettingsService.WindowsUniversalProjectSuffix,
                        baseDictionary,
                        true));
            }

            return textTemplates;
        }

        /// <summary>
        /// Gets the base dictionary.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="methodComment">The method comment.</param>
        /// <param name="methodReturnType">Type of the method return.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="directory">The directory.</param>
        /// <returns>A Dictionary.</returns>
        internal Dictionary<string, string> GetBaseDictionary(
            string name,
            string methodComment,
            string methodReturnType,
            string methodName,
            string directory)
        {
            TraceService.WriteLine("DependencyServicesFactory::GetBaseDictionary");

            Dictionary<string, string> dictionary = new Dictionary<string, string>
                {
                    {
                        "NameSpace", 
                        string.Empty
                    },
                    {
                        "Directory", 
                        directory
                    },
                    {
                        "ClassName", 
                        string.Empty
                    },
                    {
                        "InterfaceName", 
                        "I" + name
                    },
                    {
                        "MethodComment", 
                        methodComment
                    },
                    {
                        "MethodReturnType", 
                        methodReturnType
                    },
                    {
                        "MethodName", 
                        methodName
                    },
                    {
                        "MethodReturnStatement", 
                        "return default(" + methodReturnType + ");"
                    }
                };

            return dictionary; 
        }
    }
}
