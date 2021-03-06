﻿// --------------------------------------------- -----------------------------------------------------------------------
// <summary>
//    Defines the OptionsViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace NinjaCoder.MvvmCross.ViewModels.Options
{
    using Scorchio.VisualStudio.Services;
    using Services.Interfaces;
    using System.Windows;
    using BaseViewModel = NinjaBaseViewModel;

    /// <summary>
    ///  Defines the OptionsViewModel type.
    /// </summary>
    public class OptionsViewModel : BaseViewModel
    {
        /// <summary>
        /// The language dictionary.
        /// </summary>
        private ResourceDictionary languageDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsViewModel" /> class.
        /// </summary>
        /// <param name="resolverService">The resolver service.</param>
        /// <param name="settingsService">The settings service.</param>
        public OptionsViewModel(
            IResolverService resolverService,
            ISettingsService settingsService)
            : base(settingsService)
        {
            TraceService.WriteLine("OptionsViewModel::Constructor Start");
            
            this.TracingViewModel = resolverService.Resolve<TracingViewModel>();
            this.BuildViewModel = resolverService.Resolve<BuildViewModel>();
            this.ProjectsViewModel = resolverService.Resolve<ProjectsViewModel>();
            this.ProjectsSuffixesViewModel = resolverService.Resolve<ProjectsSuffixesViewModel>();
            this.CodingStyleViewModel = resolverService.Resolve<CodingStyleViewModel>();
        }

        /// <summary>
        /// Gets the tracing view model.
        /// </summary>
        public TracingViewModel TracingViewModel { get; private set; }

        /// <summary>
        /// Gets the build view model.
        /// </summary>
        public BuildViewModel BuildViewModel { get; private set; }

        /// <summary>
        /// Gets the projects view model.
        /// </summary>
        public ProjectsViewModel ProjectsViewModel { get; private set; }

        /// <summary>
        /// Gets the coding style view model.
        /// </summary>
        public CodingStyleViewModel CodingStyleViewModel { get; private set; }

        /// <summary>
        /// Gets the projects suffixes view model.
        /// </summary>
        public ProjectsSuffixesViewModel ProjectsSuffixesViewModel { get; private set; }

        /// <summary>
        /// Gets or sets the language dictionary.
        /// </summary>
        public ResourceDictionary LanguageDictionary
        {
            get
            {
                return this.languageDictionary;
            }

            set
            {
                this.languageDictionary = value;
                this.TracingViewModel.LanguageDictionary = value;
                this.ProjectsViewModel.LanguageDictionary = value;
                this.CodingStyleViewModel.LanguageDictionary = value;
            }
        }
       
        /// <summary>
        /// Updates the settings.
        /// </summary>
        internal void UpdateSettings()
        {
            this.TracingViewModel.Save();
            this.BuildViewModel.Save();
            this.ProjectsViewModel.Save();
            this.ProjectsSuffixesViewModel.Save();
            this.CodingStyleViewModel.Save();
        }

        /// <summary>
        /// Called when ok button pressed.
        /// </summary>
        protected override void OnOk()
        {
            this.UpdateSettings();
            base.OnOk();
        }
    }
}
