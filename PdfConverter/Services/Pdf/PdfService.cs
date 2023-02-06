using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfConverter.Services
{
  public class PdfService : IPdfService
  {

    public async Task<FileResult> PickFile()
    {
      var options = new PickOptions() {
        FileTypes = FilePickerFileType.Pdf,
        PickerTitle="Pick pdf file"
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


  }
}
