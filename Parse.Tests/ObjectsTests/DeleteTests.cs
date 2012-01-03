using System;
using NUnit.Framework;

namespace Parse.Tests.ObjectsTests
{
   [TestFixture]
   public class DeleteTests : BaseFixture
   {
      [Test]
      public void SendsADeleteRequest()
      {
         Server.Stub(new ApiExpectation { Method = "DELETE", Url = "/1/classes/ParseObjectClass/Ed1nuqPvcm", Response = "{name: 'Goku', createdAt: '2011-08-20T02:06:57.931Z', updatedAt: '2012-09-21T03:07:58.932Z', objectId: 'Ed1nuqPvcm' }" });
         new Driver().Objects.Delete<ParseObjectClass>("Ed1nuqPvcm", r =>
         {
            Assert.AreEqual("Ed1nuqPvcm", r.Data.Id);
            Assert.AreEqual(new DateTime(2011, 8, 20, 2, 6, 57, 931), r.Data.CreatedAt.Value.ToUniversalTime());
            Assert.AreEqual(new DateTime(2012, 9, 21, 3, 7, 58, 932), r.Data.UpdatedAt.Value.ToUniversalTime());
            Assert.AreEqual("Goku", r.Data.Name);
            SetIfSuccess(r);
         });
         WaitOne();
      }

      [Test]
      public void SendsADeleteForAParseObject()
      {
         var o = new ParseObjectClass {Id = "abc123"};
         Server.Stub(new ApiExpectation { Method = "DELETE", Url = "/1/classes/ParseObjectClass/abc123", Response = "{}" });
         new Driver().Objects.Delete(o, SetIfSuccess);
         WaitOne();
      }
   }
}