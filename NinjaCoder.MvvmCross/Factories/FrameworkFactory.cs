﻿// --------------------------------------------------------------------------------------------------------------------
// <summary>
//    Defines the FrameworkFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace NinjaCoder.MvvmCross.Factories
{
    using Constants;
    using Entities;
    using Extensions;
    using Interfaces;
    using Scorchio.Infrastructure.Entities;
    using Scorchio.Infrastructure.Extensions;
    using Scorchio.VisualStudio.Services;
    using Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines the FrameworkFactory type.
    /// </summary>
    public class FrameworkFactory : IFrameworkFactory
    {
        /// <summary>
        /// The settings service.
        /// </summary>
        private readonly ISettingsService settingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameworkFactory"/> class.
        /// </summary>
        /// <param name="settingsService">The settings service.</param>
        public FrameworkFactory(ISettingsService settingsService)
        {
            TraceService.WriteLine("FrameworkFactory::Constructor");

            this.settingsService = settingsService;
        }

        /// <summary>
        /// Gets the frameworks.
        /// </summary>
        public IEnumerable<string> Frameworks
        {
            get { return FrameworkType.MvvmCross.GetDescriptions(); }
        }

        /// <summary>
        /// Gets the allowed frameworks.
        /// </summary>
        public IEnumerable<ImageItemWithDescription> AllowedFrameworks
        {
            get
            {
                List<ImageItemWithDescription> frameworks = new List<ImageItemWithDescription>();

                if (this.settingsService.FrameworkType.IsMvvmCrossSolutionType())
                {
                    frameworks.Add(new ImageItemWithDescription
                        {
                            ImageUrl = this.GetUrlPath("MvvmCross.png"),
                            Name = FrameworkType.MvvmCross.GetDescription(),
                            Selected = true
                        });
                }

                if (this.settingsService.FrameworkType.IsXamarinFormsSolutionType())
                {
                    frameworks.Add(new ImageItemWithDescription
                        {
                            ImageUrl = this.GetUrlPath("Xamarin.png"),
                            Name = FrameworkType.XamarinForms.GetDescription()
                        });
                }

                return frameworks.OrderBy(x => x.Name);
            }
        }

        /// <summary>
        /// Gets the URL path.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>the url of the image.</returns>
        internal string GetUrlPath(string image)
        {
            return string.Format("{0}/{1}", Settings.ResourcePath, image);
        }
    }
}
