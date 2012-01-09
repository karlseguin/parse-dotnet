using NUnit.Framework;

namespace Parse.Tests.UsersTests
{
   public class LoginTests : BaseFixture
   {
      [Test]
      public void LogsAUserIn()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/1/login", Request = "username=paul&password=chani", Response = "{username: 'paul', password: null, objectid: '123'}" });
         new Driver().Users.Login<ParseUser>("paul", "chani", r =>
         {
            Assert.AreEqual(null, r.Data.Password);
            Assert.AreEqual("paul", r.Data.UserName);
            SetIfSuccess(r);
         });
         WaitOne();
      }

      [Test]
      public void DoesNotLogsAUserIn()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/1/login", Request = "username=paul&password=vladamir", Response = "null" });
         new Driver().Users.Login<ParseUser>("paul", "vladamir", r =>
         {
            Assert.AreEqual(null, r.Data);
            SetIfSuccess(r);
         });
         WaitOne();
      }
   }
}