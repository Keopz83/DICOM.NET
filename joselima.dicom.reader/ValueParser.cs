using joselima.dicom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace joselima.dicom {

    public class ValueParser {

        public static object ParseValue(VR vr, byte[] rawValue) {

            switch (vr) {
                case VR.AE:
                    return ParseString(rawValue);

                case VR.AS:
                    return ParseString(rawValue);

                case VR.AT: //Attribute Tag
                    return rawValue; //TODO

                case VR.CS:
                    return ParseString(rawValue);

                case VR.DA:
                    return ParseString(rawValue);

                case VR.DS:
                    return ParseString(rawValue);

                case VR.DT:
                    return ParseString(rawValue);

                case VR.FL: //Floating Point Single
                    return rawValue; //TODO

                case VR.FD: //Floating Point Double
                    return rawValue; //TODO
                case VR.IS:
                    return ParseString(rawValue);

                case VR.LO:
                    return ParseString(rawValue);

                case VR.LT:
                    return ParseString(rawValue);

                case VR.OB:
                    return rawValue;
                case VR.OD:
                    return rawValue; //TODO
                case VR.OF:
                    return rawValue; //TODO
                case VR.OW:
                    return rawValue; //TODO
                case VR.PN:
                    return ParseString(rawValue);

                case VR.SH:
                    return ParseString(rawValue);

                case VR.SL:
                    return rawValue; //TODO

                case VR.SQ:
                    return rawValue; //TODO

                case VR.SS:
                    return rawValue; //TODO

                case VR.ST:
                    return ParseString(rawValue);

                case VR.TM:
                    return ParseString(rawValue);

                case VR.UI:
                    return ParseString(rawValue);

                case VR.UL:
                    return BitConverter.ToUInt32(rawValue, 0);

                case VR.UN:
                    return rawValue;

                case VR.US:
                    return BitConverter.ToUInt16(rawValue, 0);

                case VR.UT:
                    return BitConverter.ToUInt32(rawValue, 0);

                default:
                    return rawValue;
            }
        }

        private static string ParseString(byte[] rawValue) {
            return Encoding.UTF8.GetString(rawValue, 0, rawValue.Length).Trim().Trim('\0');
        }
    }
}
