using DicomReader;
using DicomReader.DicomObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DicomReader.DicomObjects;

namespace joselima.dicom.reader._tests
{

    [TestClass]
    public class ParserTests
    {

        static readonly string testFile1 = @"C:\Users\user1\Documents\DICOM\testData\Anonymized20180815\series-000000\image-000000.dcm";


        [TestMethod]
        public void Tag_ID_group_element() {

            //Arrange
            var tag = TagsDictionary.PatientID;

            //Assert
            Assert.AreEqual("0010", tag.Group.ToString("x4"), $"Unexpected value for tag {tag}.");
            Assert.AreEqual("0020", tag.Element.ToString("x4"), $"Unexpected value for tag {tag}.");

        }


        [TestMethod]
        public void Attribute_Tag_TagDictionary() {

            //Arrange
            var dataset = new Dictionary<Tag, DicomReader.DicomObjects.Attribute>(){
                { TagsDictionary.PatientID, new DicomReader.DicomObjects.Attribute(TagsDictionary.PatientID, "123") },
                { TagsDictionary.PatientsName, new DicomReader.DicomObjects.Attribute(TagsDictionary.PatientID, "Doe^John") }
            };

            //Assert
            var tag = TagsDictionary.PatientID;
            Assert.AreEqual("123", (string)dataset[tag].Value, $"Unexpected value for tag {tag}.");

            tag = TagsDictionary.PatientsName;
            Assert.AreEqual("Doe^John", (string)dataset[tag].Value, $"Unexpected value for tag {tag}.");

        }


        [TestMethod]
        public void ReadDicomMetaData_FileMetaInformationGroupLength() {

            //Act
            var dataset = Parser.ReadFile(testFile1);

            //Assert
            var tag = TagsDictionary.FileMetaInformationGroupLength;
            Assert.AreEqual((uint)230, dataset[tag].Value, $"Unexpected value for tag {tag}.");

        }

    }
}
