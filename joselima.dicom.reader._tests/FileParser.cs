
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;


namespace joselima.dicom.reader._tests {


    [TestClass]
    public class FileParser
    {

        static readonly string testFile1 = @"C:\Users\user1\Documents\DICOM\testData\Anonymized20180815\series-000000\image-000000.dcm";


        [TestMethod]
        public void ReadDicomMetaData_FileMetaInformationGroupLength() {

            //Act
            var file = new dicom.FileParser().Parse(testFile1, TagsDictionary.FileMetaInformationGroupLength.ID);

            //Assert
            var tag = TagsDictionary.FileMetaInformationGroupLength;
            Assert.AreEqual((uint)230, file.Attributes[tag.ID].Value, $"Unexpected value for tag {tag}.");

        }

        [TestMethod]
        public void ReadDicomMetaData_until_tag_00020002() {

            //Act
            var file = new dicom.FileParser().Parse(testFile1, 0x00020002);

            //Assert
            var tag = TagsDictionary.PatientID;
            Assert.AreEqual("1.2.840.10008.5.1.4.1.1.2", file.Attributes[0x00020002].Value.ToString(), $"Unexpected value for tag {tag}.");

        }

        [TestMethod]
        public void ReadDicomMetaData_PatientsName() {

            //Act
            var file = new dicom.FileParser().Parse(testFile1, TagsDictionary.PatientsName.ID);

            //Assert
            var tag = TagsDictionary.PatientsName;
            Assert.AreEqual("Anonymized", file.Attributes[tag.ID].Value.ToString(), $"Unexpected value for tag {tag}.");

        }

        [TestMethod]
        public void ReadDicomMetaData_all_except_pixel_data() {

            //Act
            var file = new dicom.FileParser().ParseMetadata(testFile1);

            //Assert
            uint tagId = 0x00401001;
            Assert.AreEqual("A10026177757", file.Attributes[tagId].Value.ToString(), $"Unexpected value for tag {tagId}.");

        }

    }
}
