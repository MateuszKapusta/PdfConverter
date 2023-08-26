using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PdfConverter.Model;
using PdfConverter.Services;
using PdfSharp.Pdf.IO;
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
                        Width = page.Width,
                        Height = page.Height,
                        NewWidth = page.Width,
                        NewHeight = page.Height
                    });
                }

                ImageData?.Dispose();
                ImageData = imageData;
            }

            SaveClickedCommand.NotifyCanExecuteChanged();
        }




        [RelayCommand(CanExecute = nameof(IsValid))]
        private async Task SaveClicked() => 
            await pdfService.SaveFile(StartPage, LastPage, FileData, PageDataList);


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
                CurrentPageHeight = PageDataList[newValue - 1].Height;
                CurrentPageWidth = PageDataList[newValue - 1].Width;
                CurrentPageNewHeight = PageDataList[newValue - 1].NewHeight;
                CurrentPageNewWidth = PageDataList[newValue - 1].NewWidth;
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

            PageDataList[CurrentPageNumber - 1].NewHeight = newValue;
        }

        partial void OnCurrentPageNewWidthChanged(double oldValue, double newValue)
        {
            if (PageDataList == null 
                || PageDataList.Count() == 0 
                || newValue < 1)
            {
                return;
            }

            PageDataList[CurrentPageNumber - 1].NewWidth = newValue;
        }


    }
}
