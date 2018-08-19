using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace joselima.dicom.network {


    public class AssociateRequest {

        public UInt32 PDULength { get; set; }

        public string CallingAETitle { get; set; }

        public string CalledAETitle { get; set; }

        public string ApplicationContextName { get; set; }

        public IEnumerable<PresentationContext> PresentationContexts { get; set; }

        public UserInformation UserInformation { get; set; }
    }


    public class PresentationContext {

        public ushort ID { get; set; }

        public SyntaxItems SyntaxItems { get; set; }

    }


    public class SyntaxItems {

        public string AbstractSyntax { get; set; }

        public IEnumerable<string> TransferSyntaxes { get; set; }

    }

    public class UserInformation {

        public UInt32 MaxLength { get; set; }

        public byte[] RawData { get; set; }

    }


    public class CEchoParser {

        /// <summary>
        /// See DICOM Std. 9.3.2 A-ASSOCIATE-RQ PDU Structure (Table 9-11)
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static AssociateRequest ParseRequest(byte[] buffer, int len = -1) {

            if (len < 0)
                len = buffer.Length;

            var stream = new MemoryStream(buffer);

            //Bytes 1: PDU-TYPE: 01H
            var rawField = new byte[1];
            stream.Read(rawField, 0, rawField.Length);

            //Bytes 2 (1): Reserved
            rawField = new byte[1];
            stream.Read(rawField, 0, rawField.Length);

            //Bytes 3-6 (4): PDU-length
            rawField = new byte[4];
            stream.Read(rawField, 0, rawField.Length);
            var totalLen = ReverseBytes(BitConverter.ToUInt32(rawField, 0));

            //Bytes 7-8 (2): Protocol - version
            rawField = new byte[2];
            stream.Read(rawField, 0, rawField.Length);
            var protocolVersion = ReverseBytes(BitConverter.ToUInt16(rawField, 0));

            //Bytes 9-10 (2): Reserved
            rawField = new byte[2];
            stream.Read(rawField, 0, rawField.Length);

            //Bytes 11-26 (16): Called-AE-title
            rawField = new byte[16];
            stream.Read(rawField, 0, rawField.Length);
            var calledAETitle = Encoding.ASCII.GetString(rawField).Trim();

            //Bytes 27-42 (16): Calling-AE-title
            rawField = new byte[16];
            stream.Read(rawField, 0, rawField.Length);
            var callingAETitle = Encoding.ASCII.GetString(rawField).Trim();

            //Bytes 43-74 (32): Reserved
            rawField = new byte[32];
            stream.Read(rawField, 0, rawField.Length);

            //Bytes 75-xxx: Variable items
            string applicationContext = ParseApplicationContext(stream);
            IEnumerable<PresentationContext> presentationContexts = ParsePresentationContexts(stream, totalLen);
            UserInformation userInfo = ParseUserInfo(stream, totalLen);

            var parsedRequest = new AssociateRequest() {
                PDULength = totalLen,
                CalledAETitle = calledAETitle,
                CallingAETitle = callingAETitle,
                ApplicationContextName = applicationContext,
                PresentationContexts = presentationContexts,
                UserInformation = userInfo
            };

            return parsedRequest;
        }


        private static UserInformation ParseUserInfo(MemoryStream stream, uint totalLen) {

            //Bytes 1 (1): Item - type: 50H
            var rawField = new byte[1];
            stream.Read(rawField, 0, rawField.Length);

            //Bytes 2 (1): Reserved
            rawField = new byte[1];
            stream.Read(rawField, 0, rawField.Length);

            //Bytes 3-4 (2): Item length
            rawField = new byte[2];
            stream.Read(rawField, 0, rawField.Length);
            var itemLength = ReverseBytes(BitConverter.ToUInt16(rawField, 0));

            //Bytes 5-xxx: User-data
            UInt32 maxLength = ParseUserInfoMaxLength(stream);

            //TODO: Extended User Information Negotiation

            return new UserInformation() {
                RawData = rawField,
                MaxLength = maxLength
            };
        }

        private static uint ParseUserInfoMaxLength(MemoryStream stream) {

            //Bytes 1 (1): Item - type: 51H
            var rawField = new byte[1];
            stream.Read(rawField, 0, rawField.Length);

            //Bytes 2 (1): Reserved
            rawField = new byte[1];
            stream.Read(rawField, 0, rawField.Length);

            //Bytes 3-4 (2): Item length
            rawField = new byte[2];
            stream.Read(rawField, 0, rawField.Length);
            //var itemLength = ReverseBytes(BitConverter.ToUInt16(rawField, 0)); //Allways 4

            //Bytes 5-8 (4): Maximum-length-received
            rawField = new byte[4];
            stream.Read(rawField, 0, rawField.Length);
            var maxLength = ReverseBytes(BitConverter.ToUInt32(rawField, 0));

            return maxLength;
        }

        private static IEnumerable<PresentationContext> ParsePresentationContexts(MemoryStream stream, uint totalLength) {

            var presentationContexts = new List<PresentationContext>();

            while (stream.Position < totalLength) {

                //Bytes 1 (1): Item - type: 20H
                var rawField = new byte[1];
                stream.Read(rawField, 0, rawField.Length);
                if (rawField[0] == 0x50) {
                    stream.Seek(-1, SeekOrigin.Current);
                    break;
                }

                var newContext = new PresentationContext();

                //Bytes 2 (1): Reserved
                rawField = new byte[1];
                stream.Read(rawField, 0, rawField.Length);

                //Bytes 3-4 (2): Item length
                rawField = new byte[2];
                stream.Read(rawField, 0, rawField.Length);
                var itemLength = ReverseBytes(BitConverter.ToUInt16(rawField, 0));

                //Bytes 5: Presentation-context-ID
                rawField = new byte[1];
                stream.Read(rawField, 0, rawField.Length);
                newContext.ID = rawField[0];

                //Bytes 6-8: Reserved
                rawField = new byte[3];
                stream.Read(rawField, 0, rawField.Length);

                //Bytes 9-xxx: Abstract/Transfer Syntax Sub-Items
                newContext.SyntaxItems = ParseSyntaxes(stream, totalLength);

                presentationContexts.Add(newContext);

            }
            return presentationContexts;
        }

        private static SyntaxItems ParseSyntaxes(MemoryStream stream, uint totalLength) {

            var syntaxes = new SyntaxItems();

            //Abstract Syntax
            syntaxes.AbstractSyntax = ParsePresentationContextItem(stream);

            //Transfer Syntaxes
            var transferSyntaxes = new List<string>();
            while (stream.Position < totalLength) {
                var transferSyntax = ParsePresentationContextItem(stream);
                if (transferSyntax == null) {
                    break;
                }
                transferSyntaxes.Add(transferSyntax);
            }
            syntaxes.TransferSyntaxes = transferSyntaxes;

            return syntaxes;
        }

        private static string ParsePresentationContextItem(MemoryStream stream) {

            //Bytes 1 (1): Item - type: 30H for Abstract / 40H for Transfer Syntaxes
            var rawField = new byte[1];
            stream.Read(rawField, 0, rawField.Length);
            if(rawField[0] == 0x50) {
                stream.Seek(-1, SeekOrigin.Current);
                return null;
            }

            //Bytes 2 (1): Reserved
            rawField = new byte[1];
            stream.Read(rawField, 0, rawField.Length);

            //Bytes 3-4 (2): Item length
            rawField = new byte[2];
            stream.Read(rawField, 0, rawField.Length);
            var itemLength = ReverseBytes(BitConverter.ToUInt16(rawField, 0));

            //Bytes 5-xxx: Abstract-syntax-name
            rawField = new byte[itemLength];
            stream.Read(rawField, 0, rawField.Length);
            var syntaxName = Encoding.ASCII.GetString(rawField);

            return syntaxName;
        }


        /// <summary>
        /// Parse Application Context (Table 9-12. Application Context Item Fields).
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static string ParseApplicationContext(MemoryStream stream) {

            //Bytes 1 (1): Item - type: 10H
            var rawField = new byte[1];
            stream.Read(rawField, 0, rawField.Length);

            //Bytes 2 (1): Reserved
            rawField = new byte[1];
            stream.Read(rawField, 0, rawField.Length);

            //Bytes 3-4 (2): Item length
            rawField = new byte[2];
            stream.Read(rawField, 0, rawField.Length);
            var itemLength = ReverseBytes(BitConverter.ToUInt16(rawField, 0));

            //Bytes 5-xxx: Application-context-name
            rawField = new byte[itemLength];
            stream.Read(rawField, 0, rawField.Length);
            var appContextName = Encoding.ASCII.GetString(rawField);

            return appContextName;
        }

        // reverse byte order (16-bit)
        public static UInt16 ReverseBytes(UInt16 value) {
            return (UInt16)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }

        // reverse byte order (32-bit)
        public static UInt32 ReverseBytes(UInt32 value) {
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                   (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }
    }
}
