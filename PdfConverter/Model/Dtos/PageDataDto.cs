using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfConverter.Model
{
    public class PageDataDto
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public double NewWidth { get; set; }
        public double NewHeight { get; set; }
    }
}
