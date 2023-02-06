using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfConverter.Services
{
  public interface IPdfService
  {
    Task<FileResult> PickFile();
  }
}
