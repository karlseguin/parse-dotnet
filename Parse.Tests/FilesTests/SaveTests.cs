using NUnit.Framework;

namespace Parse.Tests.FilesTests
{
   [TestFixture]
   public class SaveTests : BaseFixture
   {
      [Test]
      public void SendsASaveRequestForText()
      {
         Server.Stub(new ApiExpectation { Method = "POST", Url = "/1/files/spice.txt", Request = "worms", Response = "{url: 'http://dune.gov/11-spicee.txt', name: '11-spice.txt' }" });
         new Driver().Files.Save("spice.txt", "worms", r =>
         {
            Assert.AreEqual("http://dune.gov/11-spicee.txt", r.Data.Url);
            Assert.AreEqual("11-spice.txt", r.Data.Name);
            
            SetIfSuccess(r);
         });
         WaitOne();
      }
   }
}