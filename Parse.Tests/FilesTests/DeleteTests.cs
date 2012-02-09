using NUnit.Framework;

namespace Parse.Tests.FilesTests
{
   public class DeleteTests : BaseFixture
   {
      [Test]
      public void SendsADeleteRequest()
      {
         Server.Stub(new ApiExpectation { Method = "DELETE", Url = "/1/files/sand.txt", Request = "", Response = "" });
         new Driver().Files.Delete("sand.txt", SetIfSuccess);
         WaitOne();
      }
   }
}