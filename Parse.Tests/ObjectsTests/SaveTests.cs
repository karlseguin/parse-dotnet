using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Parse.Tests.ObjectsTests
{
   [TestFixture]
   public class SaveTests : BaseFixture
   {
      [Test]
      public void SendsASaveRequest()
      {
         Server.Stub(new ApiExpectation { Method = "POST", Url = "/1/classes/ParseObjectClass", Request = "{\"Name\":\"Leto\",\"objectId\":null,\"createdAt\":null,\"updatedAt\":null}", Response = "{createdAt: '2011-08-20T02:06:57.931Z', objectId: 'Ed1nuqPvcm' }" });
         new Driver().Objects.Save(new ParseObjectClass{Name = "Leto"}, r =>
         {
            Assert.AreEqual("Ed1nuqPvcm", r.Data.Id);
            Assert.AreEqual(null, r.Data.UpdatedAt);
            Assert.AreEqual(new DateTime(2011, 8, 20, 2, 6, 57, 931), r.Data.CreatedAt.Value.ToUniversalTime());
            SetIfSuccess(r);
         });
         WaitOne();
      }

      [Test]
      public void SendsASaveRequestWithADate()
      {
         Server.Stub(new ApiExpectation { Method = "POST", Url = "/1/classes/ParseObjectClass", Request = "{\"Name\":null,\"objectId\":null,\"createdAt\":{\"__type\":\"Date\",\"iso\":\"2002-03-03T21:06:07.0000000Z\"},\"updatedAt\":null}", Response = "{}" });
         new Driver().Objects.Save(new ParseObjectClass { CreatedAt = new DateTime(2002, 3, 4, 5, 6, 7) }, SetIfSuccess);
         WaitOne();
      }

      [Test]
      public void SendsASaveRequestWithAByteArray()
      {
         Server.Stub(new ApiExpectation { Method = "POST", Url = "/1/classes/ByteArrayClass", Request = "{\"Nibbles\":{\"__type\":\"Bytes\",\"base64\":\"AQIDCAcG\"}}", Response = "{}" });
         new Driver().Objects.Save(new ByteArrayClass { Nibbles = new byte[]{1,2,3,8,7,6}}, SetIfSuccess);
         WaitOne();
      }

      [Test]
      public void SendsASaveRequestWithAByteEnumerable()
      {
         Server.Stub(new ApiExpectation { Method = "POST", Url = "/1/classes/ByteEnumeratorClass", Request = "{\"Nibbles\":{\"__type\":\"Bytes\",\"base64\":\"Bjon/xQAAQI=\"}}", Response = "{}" });
         new Driver().Objects.Save(new ByteEnumeratorClass { Nibbles = new List<byte> { 6, 58, 39, 255, 20, 0, 1, 2 } }, SetIfSuccess);
         WaitOne();
      }

      [Test]
      public void SendsASaveRequestWithAGeoPoint()
      {
         Server.Stub(new ApiExpectation { Method = "POST", Url = "/1/classes/GeoPointClass", Request = "{\"Location\":{\"__type\":\"GeoPoint\",\"latitude\":23.3,\"longitude\":-39.4}}", Response = "{}" });
         new Driver().Objects.Save(new GeoPointClass {Location = new GeoPoint(23.3, -39.4)}, SetIfSuccess);
         WaitOne();
      }

      [Test]
      public void SendsASaveRequestWithAFile()
      {
         Server.Stub(new ApiExpectation { Method = "POST", Url = "/1/classes/FileClass", Request = "{\"File\":{\"__type\":\"File\",\"name\":\"test.gif\"}}", Response = "{}" });
         new Driver().Objects.Save(new FileClass { File = new ParseFile("test.gif") }, SetIfSuccess);
         WaitOne();
      }
   }
}