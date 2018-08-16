using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace joselima.dicom.viewer.wpf.ViewModels {

    public class DataSetViewModel {

        public List<AttributeViewModel> Items { get; set; }
    }

    public class AttributeViewModel {

        public string Tag { get; set; }
        public string Value { get; set; }
    }
}
