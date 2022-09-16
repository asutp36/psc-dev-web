using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Models
{
    public class GraphicsDataModel
    {
        public IEnumerable<DateTime> Labels { get; set; }
        public List<Dataset> Datasets { get; set; }
    }

    public class Dataset
    {
        public List<int> Data { get; set; }
        public string Label { get; set; }
    }
}
