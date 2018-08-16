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
                    return Encoding.UTF8.GetString(rawValue, 0, rawValue.Length).Trim();

                case VR.AS:
                    return Encoding.UTF8.GetString(rawValue, 0, rawValue.Length).Trim();

                case VR.AT: //Attribute Tag
                    return rawValue; //TODO

                case VR.CS:
                    return Encoding.UTF8.GetString(rawValue, 0, rawValue.Length).Trim();

                case VR.DA:
                    return Encoding.UTF8.GetString(rawValue, 0, rawValue.Length).Trim();

                case VR.DS:
                    return Encoding.UTF8.GetString(rawValue, 0, rawValue.Length).Trim();

                case VR.DT:
                    return Encoding.UTF8.GetString(rawValue, 0, rawValue.Length).Trim();

                case VR.FL: //Floating Point Single
                    return rawValue; //TODO

                case VR.FD: //Floating Point Double
                    return rawValue; //TODO
                case VR.IS:
                    return Encoding.UTF8.GetString(rawValue, 0, rawValue.Length).Trim();

                case VR.LO:
                    return Encoding.UTF8.GetString(rawValue, 0, rawValue.Length).Trim();

                case VR.LT:
                    return Encoding.UTF8.GetString(rawValue, 0, rawValue.Length).Trim();

                case VR.OB:
                    return rawValue;
                case VR.OD:
                    return rawValue; //TODO
                case VR.OF:
                    return rawValue; //TODO
                case VR.OW:
                    return rawValue; //TODO
                case VR.PN:
                    return Encoding.UTF8.GetString(rawValue, 0, rawValue.Length).Trim();

                case VR.SH:
                    return Encoding.UTF8.GetString(rawValue, 0, rawValue.Length).Trim();

                case VR.SL:
                    return rawValue; //TODO

                case VR.SQ: 
                    return rawValue; //TODO

                case VR.SS:
                    return rawValue; //TODO

                case VR.ST:
                    return Encoding.UTF8.GetString(rawValue, 0, rawValue.Length).Trim();

                case VR.TM:
                    return Encoding.UTF8.GetString(rawValue, 0, rawValue.Length).Trim();

                case VR.UI:
                    return Encoding.UTF8.GetString(rawValue, 0, rawValue.Length).Trim();

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
    }
}
