using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfConverter.Model
{
    public class PageDataDto
    {
        public PageSizeDto OldPageSize { get; set; }
        public PageSizeDto NewPageSize { get; set; }
    }
}
