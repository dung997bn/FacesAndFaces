using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Faces.WebMvc.Models
{
    public class OrderDetailViewModel
    {
        public int OrderDetailId { get; set; }
        public Guid OrderId { get; set; }

        public byte[] FaceData { get; set; }
        public string ImageString { get; set; }
    }
}
