using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomReader.DicomObjects {

    public class Attribute {

        public Tag Tag { get; }

        public object Value { get; set; }

        public Attribute(Tag tag, object value) {
            Tag = tag;
            Value = value;
        }

        public override string ToString() {
            return $"{Tag.ToString()}: {Value.ToString()}";
        }
    }
}
