using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls.PlatformConfiguration.macOSSpecific;
using Newtonsoft.Json;
using PdfConverter.Model;
using PdfConverter.Services;
using PdfSharp.Pdf.IO;
using System.Collections.ObjectModel;
using System.Text;

namespace PdfConverter
{
    public partial class MainPageViewModel : ObservableObject
    {
        [ObservableProperty]
        Stream imageData;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveClickedCommand))]
        int startPage = 1;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveClickedCommand))]
        int lastPage;

        [ObservableProperty]
        ObservableCollection<PageSizeDto> lastSizes = new ObservableCollection<PageSizeDto>();

        [ObservableProperty]
        PageSizeDto sizePicked;

        [ObservableProperty]
        int currentPageNumber;

        [ObservableProperty]
        double currentPageWidth;

        [ObservableProperty]
        double currentPageHeight;

        [ObservableProperty]
        double currentPageNewWidth;

        [ObservableProperty]
        double currentPageNewHeight;

        FileResult FileData;
        List<PageDataDto> PageDataList = new();

        readonly IPdfService pdfService;

        public MainPageViewModel(
          IPdfService pdfService)
        {
            this.pdfService = pdfService;
            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            InitializeLastSizeLiast();
        }

        private void InitializeLastSizeLiast()
        {
            try
            {
                var lastSizes = Preferences.Default.Get(AppConst.LastSizesKey, "");

                if (!string.IsNullOrEmpty(lastSizes))
                {
                    var savedSizes = JsonConvert.DeserializeObject<ObservableCollection<PageSizeDto>>(lastSizes);
                    LastSizes = savedSizes;
                }
            }
            catch
            {

            }
        }

        [RelayCommand]
        private async Task FileClicked()
        {
            FileData = await pdfService.PickFile();
            if (FileData != null)
            {
                var imageData = await FileData.OpenReadAsync();
                using var document = PdfReader.Open(imageData);

                PageDataList = new List<PageDataDto>();
                StartPage = 1;
                LastPage = document.PageCount;

                foreach (var page in document.Pages)
                {
                    PageDataList.Add(new PageDataDto()
                    {
                        OldPageSize = new PageSizeDto() { Height = page.Height, Width = page.Width },
                        NewPageSize = new PageSizeDto() { Height = page.Height, Width = page.Width }
                    });
                }

                ImageData?.Dispose();
                ImageData = null;
                ImageData = imageData;
            }

            SaveClickedCommand.NotifyCanExecuteChanged();
        }




        [RelayCommand(CanExecute = nameof(IsValid))]
        private async Task SaveClicked()
        {
            if (PageDataList == null || !PageDataList.Any())
            {
                return;
            }

            await pdfService.SaveFile(StartPage, LastPage, FileData, PageDataList);

            List<PageSizeDto> newSizes = new List<PageSizeDto>();
            for (int i = StartPage; i <= LastPage; i++)
            {
                try
                {
                    if (!PageDataList[i - 1].OldPageSize.SameInteger(PageDataList[i - 1].NewPageSize))
                    {
                        newSizes.Add(PageDataList[i - 1].NewPageSize);
                    }
                }
                catch (Exception ex)
                {

                }
            }

            if (newSizes.Any())
            {
                newSizes.ForEach(x => LastSizes.Add(x));

                var sizeLiast = JsonConvert.SerializeObject(LastSizes);
                Preferences.Default.Set(AppConst.LastSizesKey, sizeLiast);
            }

        }

        private bool IsValid() =>
            FileData != null
            && StartPage != 0
            && LastPage != 0;


        partial void OnCurrentPageNumberChanged(int oldValue, int newValue)
        {
            if (PageDataList == null
                || PageDataList.Count() == 0
                || newValue < 1)
            {
                return;
            }

            if (newValue <= PageDataList.Count())
            {
                CurrentPageHeight = PageDataList[newValue - 1].OldPageSize.Height;
                CurrentPageWidth = PageDataList[newValue - 1].OldPageSize.Width;
                CurrentPageNewHeight = PageDataList[newValue - 1].NewPageSize.Height;
                CurrentPageNewWidth = PageDataList[newValue - 1].NewPageSize.Width;
            }
        }


        partial void OnCurrentPageNewHeightChanged(double oldValue, double newValue)
        {
            if (PageDataList == null
                || PageDataList.Count() == 0
                || newValue < 1)
            {
                return;
            }

            PageDataList[CurrentPageNumber - 1].NewPageSize.Width = newValue;
        }

        partial void OnCurrentPageNewWidthChanged(double oldValue, double newValue)
        {
            if (PageDataList == null
                || PageDataList.Count() == 0
                || newValue < 1)
            {
                return;
            }

            PageDataList[CurrentPageNumber - 1].NewPageSize.Width = newValue;
        }

        [RelayCommand]
        private void PickedSize()
        {
            if (SizePicked == null)
            {
                return;
            }

            CurrentPageNewHeight = SizePicked.Height;
            CurrentPageNewWidth = SizePicked.Width;
            SizePicked = null;
        }


    }
}
