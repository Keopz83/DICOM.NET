using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomReader.DicomObjects {

    public class Tag {

        public UInt32 ID { get; }

        public UInt16 Group { get { return (UInt16)(ID >> 16); } }

        public UInt16 Element { get { return (UInt16)(ID & 0x0000FFFF); } }

        public string Description { get; }

        public VR VR { get; }

        public Tag(UInt16 group, UInt16 element, VR vr, string description): 
            this((UInt32)((group << 16) + element), vr, description) {
        }

        public Tag(UInt32 id, VR vr, string description) {
            ID = id;
            VR = vr;
            Description = description;
        }

        public override string ToString() {
            return $"({Group:x4},{Element:x4}) {VR.ToString()} {Description}";
        }
    }
}
