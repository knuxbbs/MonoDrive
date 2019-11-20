using System;
using System.IO;
using Gtk;
using ODrive.Sharp.Application;
using ODrive.Sharp.Application.Interfaces;
using UI = Gtk.Builder.ObjectAttribute;

namespace ODrive.Sharp.Gtk
{
    internal class MainWindow : Window
    {
        [UI] private readonly Label _userLabel = null;
        [UI] private readonly Label _folderLabel = null;
        [UI] private readonly Button _loginButton = null;
        [UI] private readonly FileChooserButton _fileChooser = null;
        [UI] private readonly Button _syncButton = null;
        [UI] private readonly ProgressBar _progressBar = null;

        private readonly IGoogleDriveAppService _driveAppService;
        private IProgress<FolderStructureDownloadProgressChangedEventArgs> _progressReporter;

        public MainWindow(IGoogleDriveAppService driveAppService) : this(new Builder("MainWindow.glade"))
        {
            _driveAppService = driveAppService;
        }

        private MainWindow(Builder builder) : base(builder.GetObject("MainWindow").Handle)
        {
            builder.Autoconnect(this);

            _loginButton.Clicked += LoginButton_Clicked;

            _fileChooser.Action = FileChooserAction.SelectFolder;
            _fileChooser.SelectionChanged += FileChooser_SelectionChanged;

            _syncButton.Sensitive = false;
            _syncButton.Clicked += SyncButton_Clicked;

            _progressReporter = new Progress<FolderStructureDownloadProgressChangedEventArgs>(args =>
            {
                _progressBar.Text = $"{args.CompletedFolders} de {args.TotalFolders} diretórios baixados.";
                _progressBar.Fraction = args.CompletedFolders / (double) args.TotalFolders;
            });

            DeleteEvent += Window_DeleteEvent;
        }

        private async void LoginButton_Clicked(object sender, EventArgs a)
        {
            await _driveAppService.SignIn();
            //_userLabel.Text = _driveAppService.Email;
        }

        private void FileChooser_SelectionChanged(object sender, EventArgs a)
        {
            var fileChooser = (FileChooserButton) sender;

            _folderLabel.Text = fileChooser.Filename;
            _syncButton.Sensitive = Directory.Exists(fileChooser.Filename);
        }

        private async void SyncButton_Clicked(object sender, EventArgs a)
        {
            await _driveAppService.Sync(_fileChooser.Filename);
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            global::Gtk.Application.Quit();
        }
    }
}