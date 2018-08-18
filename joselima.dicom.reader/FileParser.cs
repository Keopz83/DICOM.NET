using System;
using System.IO;
using System.Text;

namespace joselima.dicom {

    public class FileParser {

        static readonly int PREAMBLE_SIZE_BYTES = 128;
        static readonly int PREFIX_SIZE_BYTES = 4;
        static readonly int GROUP_SIZE_BYTES = 2;
        static readonly int ELEMENT_SIZE_BYTES = 2;
        static readonly int LENGTH_SIZE_BYTES_EXPLICIT_VR = 4;
        static readonly int LENGTH_SIZE_BYTES_IMPLICIT_VR = 2;
        static readonly int VR_SIZE_BYTES = 2;
        static readonly string DICM_PREFIX = "DICM";

        static readonly UInt32 LAST_TAG_BEFORE_PIXEL_DATA = 0x7FE0000F;

        private UInt32 _lastTagId;

        private bool _isExplicitVr = true;


        public File ParseMetadata(string fileAbsolutePath) {
            return Parse(fileAbsolutePath, LAST_TAG_BEFORE_PIXEL_DATA);
        }


        public File Parse(string fileAbsolutePath, UInt32 lastTagId = 0) {

            _lastTagId = lastTagId;

            var streamPos = 0;
            var attributeSet = new AttributeSet();
            using(var file = System.IO.File.OpenRead(fileAbsolutePath)) {

                //Preamble
                var preamble = new byte[PREAMBLE_SIZE_BYTES];
                streamPos += file.Read(preamble, streamPos, preamble.Length);

                //'DICM' Prefix
                var prefixRaw = new byte[PREFIX_SIZE_BYTES];
                streamPos += file.Read(prefixRaw, 0, prefixRaw.Length);

                string prefixUTF8 = Encoding.UTF8.GetString(prefixRaw, 0, prefixRaw.Length);
                if (prefixUTF8.Equals(DICM_PREFIX, StringComparison.InvariantCultureIgnoreCase)) {
                    Console.WriteLine($"Found '{DICM_PREFIX}' prefix.");
                }


                while (true) {

                    Tag tag = ParseNextTag(file);
                    Console.Write($"{tag}");
                    if (_lastTagId > 0 && tag.ID > _lastTagId) break;

                    VR vr = ParseVr(file, _isExplicitVr);
                    tag.VR = vr;
                    Console.Write($"\t{vr.ToString()}");

                    int valueLength = ParseValueLength(file, _isExplicitVr, vr);
                    Console.Write($"\t{valueLength}");

                    var value = ParseNextValue(file, vr, valueLength);
                    Console.Write($"\t{value.ToString()}");

                    var newAttribute = new Attribute(tag, value);
                    attributeSet.Add(newAttribute.Tag.ID, newAttribute);
                    Console.WriteLine();
                }
                

                return new File() {
                    IsVRExplicit = _isExplicitVr,
                    Attributes = attributeSet
                };
            }

        }

        private object ParseNextValue(FileStream file, VR vr, int valueLength) {

            if (valueLength == 0) {
                return null;
            }

            //Value
            var valueRaw = new byte[valueLength];
            file.Read(valueRaw, 0, valueRaw.Length);
            object value = ValueParser.ParseValue(vr, valueRaw);

            return value;
        }


        private static VR ParseVr(FileStream file, bool isExplicitVr) {
            var vrRaw = new byte[VR_SIZE_BYTES];
            file.Read(vrRaw, 0, vrRaw.Length);

            string vrUTF8 = Encoding.UTF8.GetString(vrRaw, 0, vrRaw.Length);
            var result = Enum.TryParse(vrUTF8, out VR vr);

            //2 bytes empty after VR for VRs: OB, OW, OF, SQ and UN
            switch (vr) {
                case VR.OB:
                case VR.OW:
                case VR.OF:
                case VR.SQ:
                case VR.UN:
                    file.Read(new byte[2], 0, 2);
                    break;
                default:
                    break;
            }

            return vr;
        }


        private static int ParseValueLength(FileStream file, bool isExplicitVr, VR vr) {

            var valueLengthByteLength = 2;

            //4 bytes if Explicit & VR for VRs: OB, OW, OF, SQ and UN, or Implict
            //otherwise 2 bytes
            if (!isExplicitVr) {
                valueLengthByteLength = 4;
            }
            else {
                switch (vr) {
                    case VR.OB:
                    case VR.OW:
                    case VR.OF:
                    case VR.SQ:
                    case VR.UN:
                        valueLengthByteLength = 4;
                        break;
                    default:
                        break;
                }
            }

            var lengthRaw = new byte[valueLengthByteLength];
            file.Read(lengthRaw, 0, lengthRaw.Length);
            var valueLength = valueLengthByteLength == 2 ? BitConverter.ToInt16(lengthRaw, 0) : BitConverter.ToInt32(lengthRaw, 0);
            
            return valueLength;
        }


        private static Tag ParseNextTag(FileStream file) {

            //Tag group number
            var groupRaw = new byte[GROUP_SIZE_BYTES];
            file.Read(groupRaw, 0, groupRaw.Length);

            //Tag element number
            var elementRaw = new byte[ELEMENT_SIZE_BYTES];
            file.Read(elementRaw, 0, elementRaw.Length);

            int tagId = (BitConverter.ToInt16(groupRaw, 0) << 16) + BitConverter.ToInt16(elementRaw, 0);
            var tag = TagsDictionary.Get((UInt32)tagId);

            return tag;
        }


        
    }
}
