using System;
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
   }
}