using DicomReader.DicomObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomReader
{
    public class Parser
    {
        static readonly int PREAMBLE_SIZE_BYTES = 128;
        static readonly int PREFIX_SIZE_BYTES = 4;
        static readonly int TAG_ID_SIZE_BYTES = 4;
        static readonly int GROUP_SIZE_BYTES = 2;
        static readonly int ELEMENT_SIZE_BYTES = 2;
        static readonly int LENGTH_SIZE_BYTES = 2;
        static readonly int VR_SIZE_BYTES = 2;
        static readonly string DICM_PREFIX = "DICM";

        public static Dictionary<Tag, DicomObjects.Attribute> ReadFile(string absolutePath) {

            var isVrExplicit = false;
            var streamPos = 0;
            using(var file = File.OpenRead(absolutePath)) {

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


                //Tag group number
                var groupRaw = new byte[GROUP_SIZE_BYTES];
                streamPos += file.Read(groupRaw, 0, groupRaw.Length);

                //Tag element number
                var elementRaw = new byte[ELEMENT_SIZE_BYTES];
                streamPos += file.Read(elementRaw, 0, elementRaw.Length);

                int tagId = BitConverter.ToInt16(groupRaw, 0) << 16 + BitConverter.ToInt16(elementRaw, 0);
                var tag = TagsDictionary.Index[(UInt32)tagId];

                //Value Representation (VR)
                var vrRaw = new byte[ELEMENT_SIZE_BYTES];
                streamPos += file.Read(vrRaw, 0, vrRaw.Length);

                string vrUTF8 = Encoding.UTF8.GetString(vrRaw, 0, vrRaw.Length);
                var result = Enum.TryParse(vrUTF8, out VR vr);

                if (result) {
                    isVrExplicit = true;
                    Console.WriteLine("Explicit VR.");
                }

                //Value length
                var lengthRaw = new byte[LENGTH_SIZE_BYTES];
                streamPos += file.Read(lengthRaw, 0, lengthRaw.Length);
                var valueLength = BitConverter.ToInt16(lengthRaw, 0);

                //Value
                var valueRaw = new byte[valueLength];
                streamPos += file.Read(valueRaw, 0, valueRaw.Length);
                object value = ParseValue(vr, valueRaw);

                return new Dictionary<Tag, DicomObjects.Attribute>() {
                    {tag, new DicomObjects.Attribute(tag, value) }
                };

            }


            return null;
        }

        private static object ParseValue(VR vr, byte[] rawValue) {

            switch (vr) {

                case VR.UL: return BitConverter.ToUInt32(rawValue, 0);

                default: return rawValue;
            }
        }
    }
}
