// --------------------------------------------------------------------------------------------------------------------
// <summary>
//    Defines the BaseController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using MahApps.Metro.Controls;

namespace NinjaCoder.MvvmCross.Controllers
{
    using Constants;
    using EnvDTE;
    using EnvDTE80;
    using MahApps.Metro;
    using Scorchio.Infrastructure.Extensions;
    using Scorchio.Infrastructure.Services;
    using Scorchio.Infrastructure.Wpf.Views;
    using Scorchio.VisualStudio.Services;
    using Scorchio.VisualStudio.Services.Interfaces;
    using Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;

    /// <summary>
    ///  Defines the BaseController type.
    /// </summary>
    public abstract class BaseController
    {
        /// <summary>
        /// The readme lines
        /// </summary>
        private IList<string> readmeLines;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseController" /> class.
        /// </summary>
        /// <param name="visualStudioService">The visual studio service.</param>
        /// <param name="settingsService">The settings service.</param>
        /// <param name="messageBoxService">The message box service.</param>
        /// <param name="resolverService">The resolver service.</param>
        /// <param name="readMeService">The read me service.</param>
        protected BaseController(
            IVisualStudioService visualStudioService,
            ISettingsService settingsService,
            IMessageBoxService messageBoxService,
            IResolverService resolverService,
            IReadMeService readMeService)
        {
            //// init the tracing service first!
            TraceService.Initialize(
                settingsService.LogToTrace,
                false, //// log to console.
                settingsService.LogToFile,
                settingsService.ExtendedLogging,
                settingsService.LogFilePath,
                settingsService.DisplayErrors,
                settingsService.ErrorFilePath);

            TraceService.WriteLine("BaseController::Constructor");

            this.VisualStudioService = visualStudioService;
            this.SettingsService = settingsService;
            this.MessageBoxService = messageBoxService;
            this.ResolverService = resolverService;
            this.ReadMeService = readMeService;
        }

        /// <summary>
        /// Gets the visual studio service.
        /// </summary>
        protected IVisualStudioService VisualStudioService { get; }

        /// <summary>
        /// Gets the settings service.
        /// </summary>
        protected ISettingsService SettingsService { get; }

        /// <summary>
        /// Gets the message box service.
        /// </summary>
        protected IMessageBoxService MessageBoxService { get; }

        /// <summary>
        /// Gets the resolver service.
        /// </summary>
        protected IResolverService ResolverService { get; }

        /// <summary>
        /// Gets the readme service.
        /// </summary>
        protected IReadMeService ReadMeService { get; }

        /// <summary>
        /// Gets the read me lines.
        /// </summary>
        protected IList<string> ReadMeLines
        {
            get { return this.readmeLines ?? (this.readmeLines = new List<string>()); }
        }

        /// <summary>
        /// Gets the current theme.
        /// </summary>
        protected Theme CurrentTheme
        {
            get { return this.SettingsService.Theme == "Dark" ? Theme.Dark : Theme.Light; }
        }

        /// <summary>
        /// Sets the visual studio instance.
        /// </summary>
        /// <param name="dte2">The dte2.</param>
        public void SetDte2(DTE2 dte2)
        {
            this.VisualStudioService.DTE2 = dte2;
        }

        /// <summary>
        /// Project Item added event handler.
        /// </summary>
        /// <param name="projectItem">The project item.</param>
        internal void ProjectItemsEventsItemAdded(ProjectItem projectItem)
        {
            string message = string.Format(
                "BaseController::ProjectItemsEventsItemAdded file={0}",
                projectItem.Name);

            TraceService.WriteLine(message);

            ProjectItemService projectItemService = new ProjectItemService(projectItem);

            this.ProjectItemAdded(projectItemService);
        }

        /// <summary>
        /// Projects the item added.
        /// </summary>
        /// <param name="projectItemService">The project item service.</param>
        internal void ProjectItemAdded(IProjectItemService projectItemService)
        {
            TraceService.WriteLine("BaseController::ProjectItemAdded");

            bool saveFile = false;

            if (projectItemService.IsCSharpFile())
            {
                if (this.SettingsService.RemoveDefaultComments)
                {
                    projectItemService.RemoveComments();
                    saveFile = true;
                }

                if (this.SettingsService.RemoveDefaultFileHeaders)
                {
                    projectItemService.RemoveHeader();
                    saveFile = true;
                }
            }

            if (saveFile)
            {
                this.VisualStudioService.DTEService.SaveAll();
            }
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="dialog">The dialog.</param>
        /// <returns>
        /// The view model.
        /// </returns>
        protected TViewModel ShowDialog<TViewModel>(IDialog dialog) 
            where TViewModel : class
        {
            TraceService.WriteLine("BaseController::ShowDialog");

            //// set the visual studio version number
            this.SettingsService.VisualStudioVersion = this.VisualStudioService.DTE2.Version;

            TViewModel viewModel = this.ResolverService.Resolve<TViewModel>();

            dialog.DataContext = viewModel;

            dialog.ShowDialog();

            return viewModel;
        }
        
        /// <summary>
        /// Gets the read me path.
        /// </summary>
        /// <returns>The path of the ReadMe file.</returns>
        protected string GetReadMePath()
        {
            TraceService.WriteLine("BaseController::GetReadMePath");

            string directoryName;

            //// this could fail if the solution hasnt actually been created yet!
            try
            {
                directoryName = this.VisualStudioService.SolutionService.GetDirectoryName();
            }
            catch (Exception)
            {
                // set to temp path.
                directoryName = Path.GetTempPath();
            }

            string path = directoryName + Settings.NinjaReadMeFile;

            TraceService.WriteLine("BaseController::GetReadMePath path=" + path);
            return path;
        }

        /// <summary>
        /// Shows the not MVVM cross solution message.
        /// </summary>
        protected void ShowNotMvvmCrossSolutionMessage()
        {
            TraceService.WriteLine("BaseController::.ShowNotMvvmCrossSolutionMessage");

            this.MessageBoxService.Show(
                Settings.NonMvvmCrossSolution, 
                Settings.ApplicationName);
        }

        /// <summary>
        /// Shows the not MVVM cross solution message.
        /// </summary>
        protected void ShowNotXamarinFormsSolutionMessage()
        {
            TraceService.WriteLine("BaseController::.ShowNotMvvmCrossSolutionMessage");

            this.MessageBoxService.Show(
                Settings.NonXamarinFormsSolution,
                Settings.ApplicationName);
        }

        /// <summary>
        /// Shows the not MVVM cross or xamarin forms solution message.
        /// </summary>
        protected void ShowNotMvvmCrossOrXamarinFormsSolutionMessage()
        {
            TraceService.WriteLine("BaseController::.ShowNotMvvmCrossOrXamarinFormsSolutionMessage");

            this.MessageBoxService.Show(
                Settings.NonMvvmCrossOrXamarinFormsSolution,
                Settings.ApplicationName);
        }

        /// <summary>
        /// Shows the read me.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="messages">The messages.</param>
        /// <param name="closeDocuments">if set to <c>true</c> [close documents].</param>
        /// <param name="collapseSolution">if set to <c>true</c> [collapse solution].</param>
        protected void ShowReadMe(
            string function,
            IEnumerable<string> messages,
            bool closeDocuments = true,
            bool collapseSolution = true)
        {
            TraceService.WriteLine("BaseController::ShowReadMe " + function);

            //// never quite got to the bottom this but sometimes closing documents/collapsing the solution fails.
            //// this isnt that important - but we need to show the readme file - so catch the error.
            
            try
            {
                //// close any open documents.
                if (closeDocuments)
                {
                    this.VisualStudioService.DTEService.CloseDocuments();
                }

                //// now collapse the solution!
                if (collapseSolution)
                {
                    this.VisualStudioService.DTEService.CollapseSolution();
                }
            }
            catch (Exception exception)
            {
                TraceService.WriteError("Error closing documents/solution exception=" + exception.Message);
            }

            try
            {
                string readMePath = this.GetReadMePath();

                TraceService.WriteLine("BaseController::ShowReadMe path=" + readMePath);

                //// now construct the ReadMe.txt
                this.ReadMeLines.AddRange(messages);
                
                this.ReadMeService.AddLines(
                    readMePath, 
                    function,
                    this.ReadMeLines, 
                    TraceService.ErrorMessages, 
                    this.GetTraceMessages(false));

                this.VisualStudioService.DTEService.OpenFile(readMePath);

                //// reset the messages - if we don't do this we get the previous messages!
                this.readmeLines = new List<string>();
            }
            catch (Exception exception)
            {
                TraceService.WriteError("BaseController::ShowReadMe Showing ReadMe Error :-" + exception.Message);
            }
        }

        /// <summary>
        /// Gets the language dictionary.
        /// </summary>
        protected ResourceDictionary GetLanguageDictionary()
        {
            TraceService.WriteLine("BaseController::GetLanguageDictionary");

            ResourceDictionary resourceDictionary = new ResourceDictionary();

            const string ResourcesBaseUrl = "/NinjaCoder.MvvmCross;component/Resources/";
            
            resourceDictionary.Source = new Uri(ResourcesBaseUrl + "StringResources.xaml", UriKind.RelativeOrAbsolute);

            return resourceDictionary;
        }

        /// <summary>
        /// Traces the messages.
        /// </summary>
        /// <param name="exceptionRaised">if set to <c>true</c> [exception raised].</param>
        /// <returns>
        /// The Trace Messages.
        /// </returns>
        internal IEnumerable<string> GetTraceMessages(bool exceptionRaised)
        {
            if (this.SettingsService.OutputLogsToReadMe || 
                exceptionRaised)
            {
                return TraceService.Messages;
            }

            return new List<string>();
        }
    }
}
