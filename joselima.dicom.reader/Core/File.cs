using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace joselima.dicom {

    public class File {

        public bool IsVRExplicit { get; internal set; }

        public AttributeSet Attributes { get; internal set; }

    }
}
