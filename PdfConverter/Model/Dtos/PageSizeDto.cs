using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfConverter.Model
{
    public class PageSizeDto
    {
        public double Width { get; set; }
        public double Height { get; set; }


        public bool SameInteger(PageSizeDto item) 
            => ((int)Width) == ((int)item.Width) && ((int)Height) == ((int)item.Height);    
    }
}
