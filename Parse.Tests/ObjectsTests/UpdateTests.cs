using System;
using NUnit.Framework;

namespace Parse.Tests.ObjectsTests
{
   [TestFixture]
   public class UpdateTests : BaseFixture
   {
      [Test]
      public void SendsAnUpdateRequest()
      {
         var o = new ParseObjectClass {Id = "over9000", Name = "ouch"};
         Server.Stub(new ApiExpectation { Method = "PUT", Url = "/1/classes/ParseObjectClass/over9000", Request = "{\"Name\":\"ouch\",\"objectId\":\"over9000\",\"createdAt\":null,\"updatedAt\":null}", Response = "{updatedAt: '2011-08-21T18:02:52.248Z' }" });
         new Driver().Objects.Update(o, r =>
         {
            Assert.AreEqual(new DateTime(2011, 8, 21, 18, 2, 52, 248), r.Data.ToUniversalTime());
            SetIfSuccess(r);
         });
         WaitOne();
      }

      [Test]
      public void SendsAnUpdateRequestForAParseObject()
      {
         Server.Stub(new ApiExpectation { Method = "PUT", Url = "/1/classes/SimpleClass/9393", Request = "{\"PowerLevel\":44}", Response = "{updatedAt: '2012-09-22T19:03:53.249Z' }" });
         new Driver().Objects.Update("9393", new SimpleClass{PowerLevel =  44}, r =>
         {
            Assert.AreEqual(new DateTime(2012, 9, 22, 19, 3, 53, 249), r.Data.ToUniversalTime());
            SetIfSuccess(r);
         });
         WaitOne();
      }
   }
}