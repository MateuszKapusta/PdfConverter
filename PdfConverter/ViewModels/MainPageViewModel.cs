using Microsoft.Maui.Graphics.Text;
using Microsoft.Maui.Storage;
using PdfConverter.Services;
using PdfConverter.Services.FolderPicker;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PdfConverter
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        Stream imageData;
        public Stream ImageData
        {
            get { return imageData; }
            set { SetProperty(ref imageData, value); }
        }

        string startPage = "1";
        public string StartPage
        {
            get { return startPage; }
            set
            {
                SetProperty(ref startPage, value);
                ((Command)OnSaveClickedCommand).ChangeCanExecute();
            }
        }

        string lastPage;
        public string LastPage
        {
            get { return lastPage; }
            set
            {
                SetProperty(ref lastPage, value);
                ((Command)OnSaveClickedCommand).ChangeCanExecute();
            }
        }

        public ICommand OnFileClickedCommand => new Command(async () => await OnFileClicked());

        private ICommand onSaveClickedCommand;
        public ICommand OnSaveClickedCommand => onSaveClickedCommand ??= new Command(async () => await OnSaveClicked(), () => IsValid());

        private bool IsValid()
        {
            return FileData != null
              && int.TryParse(StartPage, out int startIndex)
              && int.TryParse(LastPage, out int lastIndex);
        }

        FileResult FileData;

        readonly IFolderPicker folderPicker;
        readonly IPdfService pdfService;
        public MainPageViewModel(
          IPdfService pdfService
          , IFolderPicker folderPicker)
        {
            this.pdfService = pdfService;
            this.folderPicker = folderPicker;

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        private async Task OnFileClicked()
        {
            FileData = await pdfService.PickFile();
            ImageData = await FileData.OpenReadAsync();

            ((Command)OnSaveClickedCommand).ChangeCanExecute();
        }

        private async Task OnSaveClicked()
        {
            if (int.TryParse(StartPage, out int startNumber)
              && int.TryParse(LastPage, out int lastNumber))
            {
                try
                {
                    var document = PdfReader.Open(await FileData.OpenReadAsync());

                    for (int i = document.Pages.Count - 1; i >= lastNumber; i--)
                    {
                        document.Pages.RemoveAt(i);
                    }


                    for (int i = startNumber - 1; i > 0; i--)
                    {
                        document.Pages.RemoveAt(i);
                    }

                    var folderPath = await folderPicker.PickFolder();
                    var file = Path.Combine(folderPath, FileData.FileName);

                    document.Save(file);
                    document.Close();

                    await App.Current.MainPage.DisplayAlert("Success", "Pdf was successfully created.", "OK");
                }
                catch (Exception ex)
                {

                }
            }
        }

        #region INotifyPropertyChanged
        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
