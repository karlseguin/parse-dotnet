using NUnit.Framework;

namespace Parse.Tests.ObjectsTests
{
   [TestFixture]
   public class IncrementTests : BaseFixture
   {
      [Test]
      public void SendsAnIncrementRequest()
      {
         Server.Stub(new ApiExpectation { Method = "PUT", Url = "/1/classes/string/the-id", Request = "{\"Length\":{\"__op\":\"Increment\",\"amount\":10}}", Response = "{Length: 12, updatedAt: '2011-08-21T18:02:52.248Z' }" });         
         new Driver().Objects.Increment<string>("the-id", s => s.Length, 10, r =>
         {
            Assert.AreEqual(12, r.Data);
            SetIfSuccess(r);
         });
         WaitOne();
      }

      [Test]
      public void SendsAnIncrementRequestByFieldName()
      {
         Server.Stub(new ApiExpectation { Method = "PUT", Url = "/1/classes/string/the-id2", Request = "{\"ouch\":{\"__op\":\"Increment\",\"amount\":12}}", Response = "{Length: 484, updatedAt: '2011-08-21T18:02:52.248Z' }" });
         new Driver().Objects.Increment<string>("the-id2", "ouch", 12, r =>
         {
            Assert.AreEqual(484, r.Data);
            SetIfSuccess(r);
         });
         WaitOne();
      }

      [Test]
      public void SendsAnIncrementRequestForParseObject()
      {
         var complex = new ComplexParseObjectClass {Id = "theid3"};
         Server.Stub(new ApiExpectation { Method = "PUT", Url = "/1/classes/ComplexParseObjectClass/theid3", Request = "{\"PowerLevel\":{\"__op\":\"Increment\",\"amount\":1762}}", Response = "{Length: 484, updatedAt: '2011-08-21T18:02:52.248Z' }" });
         new Driver().Objects.Increment(complex, o => o.PowerLevel, 1762, r =>
         {
            Assert.AreEqual(484, r.Data);
            SetIfSuccess(r);
         });
         WaitOne();
      }
   }
}