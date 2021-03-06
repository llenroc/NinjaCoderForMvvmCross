﻿// --------------------------------------------------------------------------------------------------------------------
// <summary>
//    Defines the ProjectFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace NinjaCoder.MvvmCross.Factories
{
    using Entities;
    using Interfaces;
    using Scorchio.Infrastructure.Wpf.ViewModels.Wizard;
    using Scorchio.VisualStudio.Entities;
    using Scorchio.VisualStudio.Services;
    using Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UserControls.AddProjects;
    using UserControls.AddViews;
    using ViewModels.AddNugetPackages;
    using ViewModels.AddProjects;
    using ViewModels.AddViews;
    using PluginsControl = UserControls.AddPlugins.PluginsControl;
    using ProjectsFinishedControl = UserControls.AddProjects.ProjectsFinishedControl;

    /// <summary>
    ///  Defines the ProjectFactory type.
    /// </summary>
    public class ProjectFactory : IProjectFactory
    {
        /// <summary>
        /// The resolver service.
        /// </summary>
        private readonly IResolverService resolverService;

        /// <summary>
        /// The register service
        /// </summary>
        private readonly IRegisterService registerService;

        /// <summary>
        /// The caching service.
        /// </summary>
        private readonly ICachingService cachingService;

        /// <summary>
        /// The settings service.
        /// </summary>
        private readonly ISettingsService settingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectFactory" /> class.
        /// </summary>
        /// <param name="resolverService">The resolver service.</param>
        /// <param name="registerService">The register service.</param>
        /// <param name="cachingService">The caching service.</param>
        /// <param name="settingsService">The settings service.</param>
        public ProjectFactory(
            IResolverService resolverService,
            IRegisterService registerService,
            ICachingService cachingService,
            ISettingsService settingsService)
        {
            TraceService.WriteLine("ProjectFactory::Constructor");

            this.resolverService = resolverService;
            this.registerService = registerService;
            this.cachingService = cachingService;
            this.settingsService = settingsService;
        }

        /// <summary>
        /// Gets the allowed projects.
        /// </summary>
        /// <param name="frameworkType">Type of the framework.</param>
        /// <returns>
        /// The allowed projects.
        /// </returns>
        public IEnumerable<ProjectTemplateInfo> GetAllowedProjects(FrameworkType frameworkType)
        {
            TraceService.WriteLine("ProjectFactory::GetAllowedProjects");

            switch (frameworkType)
            {
                case FrameworkType.NoFramework:
                    return this.resolverService.Resolve<INoFrameworkProjectFactory>().GetAllowedProjects();

                case FrameworkType.MvvmCross:
                    return this.resolverService.Resolve<IMvvmCrossProjectFactory>().GetAllowedProjects();

                case FrameworkType.XamarinForms:
                    return this.resolverService.Resolve<XamarinFormsProjectFactory>().GetAllowedProjects();

                default:
                    return this.resolverService.Resolve<MvvmCrossAndXamarinFormsProjectFactory>().GetAllowedProjects();
            }
        }

        /// <summary>
        /// Gets the wizards steps.
        /// </summary>
        /// <returns>
        /// The wizard steps.
        /// </returns>
        public List<WizardStepViewModel> GetWizardsSteps()
        {
            TraceService.WriteLine("ProjectFactory::GetWizardsSteps");

            List<WizardStepViewModel> wizardSteps = new List<WizardStepViewModel>
            {
                new WizardStepViewModel
                {
                    ViewModel = this.resolverService.Resolve<BuildOptionsViewModel>(),
                    ViewType = typeof(BuildOptionsControl),
                    Name = "Build"
                },
                new WizardStepViewModel
                {
                    ViewModel = this.resolverService.Resolve<FrameworkSelectorViewModel>(),
                    ViewType = typeof(FrameworkSelectorControl),
                    Name = "Framework"
                },
                new WizardStepViewModel
                {
                    ViewModel = this.resolverService.Resolve<ProjectsViewModel>(),
                    ViewType = typeof(ProjectsControl),
                    Name = "Projects"
                },
                new WizardStepViewModel
                {
                    ViewModel = this.resolverService.Resolve<ApplicationOptionsViewModel>(),
                    ViewType = typeof(ApplicationOptionsControl),
                    Name = "ApplicationOptions"
                },
                new WizardStepViewModel
                {
                    ViewModel = this.resolverService.Resolve<NinjaCoderOptionsViewModel>(),
                    ViewType = typeof(NinjaCoderOptionsControl),
                    Name = "NinjaCoderOptions"
                },
                new WizardStepViewModel
                {
                    ViewModel = this.resolverService.Resolve<ApplicationSamplesOptionsViewModel>(),
                    ViewType = typeof(ApplicationSamplesOptionsControl),
                    Name = "ApplicationSamplesOptions"
                },
                new WizardStepViewModel
                {
                    ViewModel = this.resolverService.Resolve<ViewsViewModel>(),
                    ViewType = typeof(ViewsControl),
                    Name = "ViewsAndViewModels"
                },
                new WizardStepViewModel
                {
                    ViewModel = this.resolverService.Resolve<PluginsViewModel>(),
                    ViewType = typeof(PluginsControl),
                    Name = "MvvmCrossPlugins"
                },
                new WizardStepViewModel
                {
                    ViewModel = this.resolverService.Resolve<NugetPackagesViewModel>(),
                    ViewType = typeof(NugetPackagesControl),
                    Name = "NugetPackages"
                },
                new WizardStepViewModel
                {
                    ViewModel = this.resolverService.Resolve<XamarinFormsLabsViewModel>(),
                    ViewType = typeof(XamarinFormsLabsControl),
                    Name = "XamarinFormsLabs"
                },
                new WizardStepViewModel
                {
                    ViewModel = this.resolverService.Resolve<ProjectsFinishedViewModel>(),
                    ViewType = typeof(ProjectsFinishedControl),
                    Name = "Finish"
                }
            };

            return wizardSteps;
        }

        /// <summary>
        /// Registers the wizard data.
        /// </summary>
        public void RegisterWizardData()
        {
            TraceService.WriteLine("ProjectFactory::RegisterWizardData");

            WizardData wizardData = new WizardData
            {
                WindowTitle = "Add Projects",
                WindowHeight = 600,
                WindowWidth = 680,
                WizardSteps = this.GetWizardsSteps()
            };

            this.registerService.Register<IWizardData>(wizardData);
        }

        /// <summary>
        /// Gets the route modifier.
        /// </summary>
        /// <param name="frameworkType">Type of the framework.</param>
        /// <returns>The RouteModifier.</returns>
        public RouteModifier GetRouteModifier(FrameworkType frameworkType)
        {
            RouteModifier routeModifier = new RouteModifier
                                              {
                                                  ExcludeViewTypes = new List<Type>()
                                              };

            //// if no framework we cant setup up the viewmodels and views.
            if (frameworkType == FrameworkType.NoFramework)
            {
                routeModifier.ExcludeViewTypes.Add(typeof(ViewsControl));
                routeModifier.ExcludeViewTypes.Add(typeof(PluginsControl));
            }

            if (frameworkType == FrameworkType.XamarinForms)
            {
                routeModifier.ExcludeViewTypes.Add(typeof(PluginsControl));
            }

            if (this.cachingService.HasNinjaNugetPackages == false && 
                this.cachingService.HasNinjaCommunityNugetPackages == false &&
                this.cachingService.HasLocalNugetPackages == false)
            {
                routeModifier.ExcludeViewTypes.Add(typeof(NinjaCoderOptionsControl));
            }

            IEnumerable<Plugin> samplePlugins = this.cachingService.ApplicationSamplePlugIns;

            if (samplePlugins != null &&
                samplePlugins.Any() == false)
            {
                routeModifier.ExcludeViewTypes.Add(typeof(ApplicationSamplesOptionsControl));
            }

            if (this.cachingService.XamarinFormsLabsNugetPackageRequested == false)
            {
                routeModifier.ExcludeViewTypes.Add(typeof(XamarinFormsLabsControl));
            }

            if (this.settingsService.AddProjectsSkipViewOptions)
            {
                routeModifier.ExcludeViewTypes.Add(typeof(ViewsControl));
            }

            if (this.settingsService.AddProjectsSkipNinjaCoderOptions)
            {
                routeModifier.ExcludeViewTypes.Add(typeof(NinjaCoderOptionsControl));
            }

            if (this.settingsService.AddProjectsSkipApplicationOptions)
            {
                routeModifier.ExcludeViewTypes.Add(typeof(ApplicationOptionsControl));
                routeModifier.ExcludeViewTypes.Add(typeof(ApplicationSamplesOptionsControl));
            }

            if (this.settingsService.AddProjectsSkipMvvmCrossPluginOptions)
            {
                routeModifier.ExcludeViewTypes.Add(typeof(PluginsControl));
            }

            if (this.settingsService.AddProjectsSkipNugetPackageOptions)
            {
                routeModifier.ExcludeViewTypes.Add(typeof(NugetPackagesControl));
            }

            return routeModifier;
        }
    }
}
