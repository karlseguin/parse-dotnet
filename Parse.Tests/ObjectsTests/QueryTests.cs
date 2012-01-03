using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Parse.Tests.ObjectsTests
{
   [TestFixture]
   public class QueryTests : BaseFixture
   {
      private const string _blankResponse = "{results:[]}";

      [Test]
      public void SingleParameterEqualityQuery()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/1/classes/ParseObjectClass", Request = string.Concat("where=", Uri.EscapeDataString(@"{""Name"":""Jessica""}")), Response = _blankResponse});
         new Driver().Objects.Query<ParseObjectClass>().Where(c => c.Name == "Jessica").Execute(SetIfSuccess);
         WaitOne();
      }

      [Test]
      public void SingleParameterNonEqualityQuery()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/1/classes/ParseObjectClass", Request = string.Concat("where=", Uri.EscapeDataString(@"{""Id"":{""$ne"":""Harkonnen""}}")), Response = _blankResponse });
         new Driver().Objects.Query<ParseObjectClass>().Where(c => c.Id != "Harkonnen").Execute(SetIfSuccess);
         WaitOne();
      }

      [Test]
      public void SingleParameterGreaterThanQuery()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/1/classes/ComplexParseObjectClass", Request = string.Concat("where=", Uri.EscapeDataString(@"{""PowerLevel"":{""$gt"":9000}}")), Response = _blankResponse });
         new Driver().Objects.Query<ComplexParseObjectClass>().Where(c => c.PowerLevel > 9000).Execute(SetIfSuccess);
         WaitOne();
      }

      [Test]
      public void SingleParameterGreaterThanOrEqualQuery()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/1/classes/ComplexParseObjectClass", Request = string.Concat("where=", Uri.EscapeDataString(@"{""PowerLevel"":{""$gte"":2}}")), Response = _blankResponse });
         new Driver().Objects.Query<ComplexParseObjectClass>().Where(c => c.PowerLevel >= 2).Execute(SetIfSuccess);
         WaitOne();
      }

      [Test]
      public void SingleParameteLessThanQuery()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/1/classes/ComplexParseObjectClass", Request = string.Concat("where=", Uri.EscapeDataString(@"{""PowerLevel"":{""$lt"":-44}}")), Response = _blankResponse });
         new Driver().Objects.Query<ComplexParseObjectClass>().Where(c => c.PowerLevel < -44).Execute(SetIfSuccess);
         WaitOne();
      }

      [Test]
      public void SingleParameteLessThanOrEqualQuery()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/1/classes/ComplexParseObjectClass", Request = string.Concat("where=", Uri.EscapeDataString(@"{""PowerLevel"":{""$lte"":-3}}")), Response = _blankResponse });
         new Driver().Objects.Query<ComplexParseObjectClass>().Where(c => c.PowerLevel <= -3).Execute(SetIfSuccess);
         WaitOne();
      }

      [Test]
      public void TrueBooleanParameterQuery()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/1/classes/ComplexParseObjectClass", Request = string.Concat("where=", Uri.EscapeDataString(@"{""Sayan"":true}")), Response = _blankResponse });
         new Driver().Objects.Query<ComplexParseObjectClass>().Where(c => c.Sayan).Execute(SetIfSuccess);
         WaitOne();
      }

      [Test]
      public void FalseBooleanParameterQuery()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/1/classes/ComplexParseObjectClass", Request = string.Concat("where=", Uri.EscapeDataString(@"{""Sayan"":false}")), Response = _blankResponse });
         new Driver().Objects.Query<ComplexParseObjectClass>().Where(c => !c.Sayan).Execute(SetIfSuccess);
         WaitOne();
      }

      [Test]
      public void StupidBoolean()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/1/classes/ComplexParseObjectClass", Request = string.Concat("where=", Uri.EscapeDataString(@"{""Sayan"":true}")), Response = _blankResponse });
         new Driver().Objects.Query<ComplexParseObjectClass>().Where(c => !!c.Sayan).Execute(SetIfSuccess);
         WaitOne();
      }

      [Test]
      public void SendsAComplexishQuery()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/1/classes/ComplexParseObjectClass", Request = string.Concat("where=", Uri.EscapeDataString(@"{""PowerLevel"":{""$gte"":33},""Name"":{""$ne"":""test""}}")), Response = _blankResponse });
         new Driver().Objects.Query<ComplexParseObjectClass>().Where(c => c.PowerLevel >= 33 && c.Name !=  "test").Execute(SetIfSuccess);
         WaitOne();
      }

      [Test]
      public void SendsAMoreComplexishQuery()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/1/classes/ComplexParseObjectClass", Request = string.Concat("where=", Uri.EscapeDataString(@"{""PowerLevel"":{""$gt"":9000,""$lt"":10000},""Sayan"":false}")), Response = _blankResponse });
         new Driver().Objects.Query<ComplexParseObjectClass>().Where(c => c.PowerLevel > 9000 && c.PowerLevel < 10000 && !c.Sayan).Execute(SetIfSuccess);
         WaitOne();
      }

      [Test]
      public void SendsPagingInformation()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/1/classes/ComplexParseObjectClass", Request = "skip=10&limit=100", Response = _blankResponse });
         new Driver().Objects.Query<ComplexParseObjectClass>().Limit(100).Skip(10).Execute(SetIfSuccess);
         WaitOne();
      }

      [Test]
      public void SendsACountRequest()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/1/classes/ComplexParseObjectClass", Request = "count=1", Response = _blankResponse });
         new Driver().Objects.Query<ComplexParseObjectClass>().Count().Execute(SetIfSuccess);
         WaitOne();
      }

      [Test]
      public void SortsAscending()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/1/classes/ComplexParseObjectClass", Request = "order=Sayan", Response = _blankResponse });
         new Driver().Objects.Query<ComplexParseObjectClass>().SortAscending(c => c.Sayan).Execute(SetIfSuccess);
         WaitOne();
      }

      [Test]
      public void SortsDecending()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/1/classes/ComplexParseObjectClass", Request = "order=-PowerLevel", Response = _blankResponse });
         new Driver().Objects.Query<ComplexParseObjectClass>().SortDescending(c => c.PowerLevel).Execute(SetIfSuccess);
         WaitOne();
      }

      [Test]
      public void SuportsRegularExpressionQuery()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/1/classes/ComplexParseObjectClass", Request = string.Concat("where=", Uri.EscapeDataString(@"{""Name"":{""$regex"":""[a-e\\d]{1,2}""}}")), Response = _blankResponse });
         new Driver().Objects.Query<ComplexParseObjectClass>().Where(c => Regex.IsMatch(c.Name, "[a-e\\d]{1,2}")).Execute(SetIfSuccess);
         WaitOne();
      }

      [Test]
      public void SupportsStartsWithQuery()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/1/classes/ComplexParseObjectClass", Request = string.Concat("where=", Uri.EscapeDataString(@"{""Name"":{""$regex"":""^nice""}}")), Response = _blankResponse });
         new Driver().Objects.Query<ComplexParseObjectClass>().Where(c => c.Name.StartsWith("nice")).Execute(SetIfSuccess);
         WaitOne();
      }

      [Test]
      public void SupportsEndsWithQuery()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/1/classes/ComplexParseObjectClass", Request = string.Concat("where=", Uri.EscapeDataString(@"{""Name"":{""$regex"":""\\d$""}}")), Response = _blankResponse });
         new Driver().Objects.Query<ComplexParseObjectClass>().Where(c => c.Name.EndsWith("\\d")).Execute(SetIfSuccess);
         WaitOne();
      }

      [Test]
      public void SupportsContainsQuery()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/1/classes/ComplexParseObjectClass", Request = string.Concat("where=", Uri.EscapeDataString(@"{""Name"":{""$regex"":""[a,e,i,o,u]""}}")), Response = _blankResponse });
         new Driver().Objects.Query<ComplexParseObjectClass>().Where(c => c.Name.Contains("[a,e,i,o,u]")).Execute(SetIfSuccess);
         WaitOne();
      }

      [Test]
      public void SendsAlot()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/1/classes/ComplexParseObjectClass", Request = string.Concat("where=", Uri.EscapeDataString(@"{""PowerLevel"":{""$gt"":9000,""$lt"":10000},""Sayan"":false}"), "&count=1&skip=10&limit=20&order=-Id"), Response = _blankResponse });
         new Driver().Objects.Query<ComplexParseObjectClass>().Where(c => c.PowerLevel > 9000 && c.PowerLevel < 10000 && !c.Sayan).Count().Limit(20).Skip(10).Sort(c => c.Id, false).Execute(SetIfSuccess);
         WaitOne();
      }
   }
}