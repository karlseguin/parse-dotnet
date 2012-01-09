using System;
using NUnit.Framework;

namespace Parse.Tests.UsersTests
{
   public class RegisterTests : BaseFixture
   {
      [Test]
      public void RegistersAUser()
      {
         Server.Stub(new ApiExpectation { Method = "POST", Url = "/1/users", Request = "{\"username\":\"Leto\",\"password\":\"ghanima\",\"PowerLevel\":332}", Response = "{createdAt: '2011-08-20T02:06:57.931Z', objectId: 'Ed1nuqPvcm' }" });
         new Driver().Users.Register(new ParseUser {UserName = "Leto", Password = "ghanima", PowerLevel = 332}, r =>
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