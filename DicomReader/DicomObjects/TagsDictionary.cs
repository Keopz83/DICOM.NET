using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomReader.DicomObjects {

    public static class TagsDictionary {

        public static readonly Tag FileMetaInformationGroupLength = new Tag(0x2, 0x0, VR.UL, "File Meta Information Group Length");
        public static readonly Tag PatientsName = new Tag(0x10, 0x10, VR.PN, "Patient's Name");
        public static readonly Tag PatientID = new Tag(0x10, 0x20, VR.LO, "Patient ID");
        public static readonly Tag PixelData = new Tag(0x7FE0, 0x10, VR.OB, "Pixel data");

        private static readonly Dictionary<UInt32, Tag> Index = new Dictionary<UInt32, Tag>() {
            { FileMetaInformationGroupLength.ID, FileMetaInformationGroupLength},
            { PatientsName.ID, PatientsName},
            { PatientID.ID, PatientID},
        };

        public static Tag Get(UInt32 tagId) {

            if (Index.ContainsKey(tagId)) {
                return Index[tagId];
            }

            return new Tag(tagId, VR.UN, "Unknown");
        }
    }
}
