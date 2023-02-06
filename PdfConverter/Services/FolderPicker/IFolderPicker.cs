using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfConverter.Services.FolderPicker
{
  public interface IFolderPicker
  {
    Task<string> PickFolder();
  }
}
