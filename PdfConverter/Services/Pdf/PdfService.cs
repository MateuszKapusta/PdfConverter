﻿using PdfConverter.Model;
using PdfConverter.Services.FolderPicker;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using Syncfusion.Maui.DataSource.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfConverter.Services
{
    public class PdfService : IPdfService
    {
        readonly IFolderPicker folderPicker;
        public PdfService(IFolderPicker folderPicker)
        {
            this.folderPicker = folderPicker;
        }

        public async Task<FileResult> PickFile()
        {
            var options = new PickOptions()
            {
                FileTypes = FilePickerFileType.Pdf,
                PickerTitle = "Pick pdf file"
            };
            try
            {
                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    if (result.FileName.EndsWith("pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return null;
        }

        public async Task SaveFile(int startNumber, int lastNumber, FileResult fileData, List<PageDataDto> pdfData = null)
        {
            try
            {
                using var document = PdfReader.Open(await fileData.OpenReadAsync());

                if(pdfData!= null)
                {
                    int index = 0;
                    foreach (var pageData in pdfData)
                    {
                        //Add width at right of the page
                        var page = document.Pages[index];
                        page.Width = pageData.NewPageSize.Width;

                        //Add height at top of the page
                        //page.Height = pageData.NewHeight;

                        // Add height at bottom of the page
                        XRect rect = new XRect(0, page.Height - pageData.NewPageSize.Height, page.Width, pageData.NewPageSize.Height);
                        page.MediaBox = new PdfRectangle(rect);

                        index++;
                    }
                }

                for (int i = document.Pages.Count - 1; i >= lastNumber; i--)
                {
                    document.Pages.RemoveAt(i);
                }


                for (int i = startNumber - 1; i > 0; i--)
                {
                    document.Pages.RemoveAt(i);
                }

                var folderPath = await folderPicker.PickFolder();
                var file = Path.Combine(folderPath, fileData.FileName);

                document.Save(file);
                document.Close();

                await App.Current.MainPage.DisplayAlert("Success", "Pdf was successfully created.", "OK");
            }
            catch (Exception ex)
            {

            }
        }


    }
}
