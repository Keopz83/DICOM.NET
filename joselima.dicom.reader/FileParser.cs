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

        public event EventHandler<string> OnInfo;
        public event EventHandler<string> OnWarning;

        public File ParseMetadata(string fileAbsolutePath) {
            return Parse(fileAbsolutePath, LAST_TAG_BEFORE_PIXEL_DATA);
        }


        public File Parse(string fileAbsolutePath, UInt32 lastTagId = 0) {

            _lastTagId = lastTagId;

            var attributeSet = new AttributeSet();
            using(var file = System.IO.File.OpenRead(fileAbsolutePath)) {

                //Preamble
                var preamble = new byte[PREAMBLE_SIZE_BYTES];
                file.Read(preamble, 0, preamble.Length);

                //'DICM' Prefix
                var prefixRaw = new byte[PREFIX_SIZE_BYTES];
                file.Read(prefixRaw, 0, prefixRaw.Length);

                string prefixUTF8 = Encoding.UTF8.GetString(prefixRaw, 0, prefixRaw.Length);
                if (prefixUTF8.Equals(DICM_PREFIX, StringComparison.InvariantCultureIgnoreCase)) {
                    OnInfo?.Invoke(this, $"Found '{DICM_PREFIX}' prefix.");
                } else {
                    OnWarning?.Invoke(this, $"Could not find '{DICM_PREFIX}' prefix.");
                }


                while (true) {
                    try {
                        var pos = file.Position;
                        var newAttribute = ParseAttribute(file);
                        if (newAttribute == null) {
                            OnWarning?.Invoke(this, $"Could not parse attribute at position: {pos}.");
                            continue;
                        }
                        attributeSet.Add(newAttribute.Tag.ID, newAttribute);
                        OnInfo?.Invoke(this, attributeSet.ToString());

                        if (_lastTagId > 0 && newAttribute.Tag.ID >= _lastTagId)
                            break;
                    }
                    catch (EndOfStreamException) {
                        break;
                    }
                }
                

                return new File() {
                    IsVRExplicit = _isExplicitVr,
                    Attributes = attributeSet
                };
            }

        }

        public static Attribute ParseAttribute(Stream stream, bool isExplicitVr = true) {

            //Tag
            Tag tag = ParseTag(stream);

            //VR
            VR vr = ParseVr(stream, isExplicitVr);
            tag.VR = vr;

            //Value length
            int valueLength = ParseValueLength(stream, vr, isExplicitVr);

            //Value
            object value = null;
            if (valueLength > 0) {
                value = ValueParser.ParseValue(stream, vr, valueLength);
            }

            //Attribute
            var newAttribute = new Attribute(tag, value);
            return newAttribute;
        }

        public static VR ParseVr(Stream stream, bool isExplicitVr = true) {
            var vrRaw = new byte[VR_SIZE_BYTES];
            stream.Read(vrRaw, 0, vrRaw.Length);

            string vrUTF8 = Encoding.UTF8.GetString(vrRaw, 0, vrRaw.Length);
            var result = Enum.TryParse(vrUTF8, out VR vr);

            //2 bytes empty after VR for VRs: OB, OW, OF, SQ and UN
            switch (vr) {
                case VR.OB:
                case VR.OW:
                case VR.OF:
                case VR.SQ:
                case VR.UN:
                    stream.Read(new byte[2], 0, 2);
                    break;
                default:
                    break;
            }

            return vr;
        }


        public static int ParseValueLength(Stream stream, VR vr, bool isExplicitVr = true) {

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
            stream.Read(lengthRaw, 0, lengthRaw.Length);
            var valueLength = valueLengthByteLength == 2 ? BitConverter.ToInt16(lengthRaw, 0) : BitConverter.ToInt32(lengthRaw, 0);
            
            return valueLength;
        }


        public static Tag ParseTag(Stream stream) {

            //Tag group number
            var groupRaw = new byte[GROUP_SIZE_BYTES];
            stream.Read(groupRaw, 0, groupRaw.Length);

            //Tag element number
            var elementRaw = new byte[ELEMENT_SIZE_BYTES];
            stream.Read(elementRaw, 0, elementRaw.Length);

            int tagId = (BitConverter.ToInt16(groupRaw, 0) << 16) + BitConverter.ToInt16(elementRaw, 0);
            var tag = TagsDictionary.Get((UInt32)tagId);

            return tag;
        }


        
    }
}
