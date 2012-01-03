using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;

namespace Parse.Tests
{
   public abstract class BaseFixture
   {
      protected static readonly Func<IDictionary<string, object>> EmptyPayload = () => new Dictionary<string, object>();
      protected virtual bool NeedAServer 
      { 
         get { return true; } 
      }
      protected FakeServer Server;
      protected AutoResetEvent Trigger;

      [SetUp]
      public void SetUp()
      {
         Trigger = new AutoResetEvent(false);
         if (NeedAServer)
         {
            Server = new FakeServer();
            ParseConfiguration.Configure("the-app-id", "shhh", c => c.ConnectTo("http://localhost:" + FakeServer.Port + "/"));
         }
         BeforeEachTest();
      }
      [TearDown]
      public void TearDown()
      {
         if (Server != null)
         {
            Server.Dispose();
         }
         AfterEachTest();
      }
      public virtual void AfterEachTest() { }
      public virtual void BeforeEachTest() { }

      protected void AssertMogadeException(string expectedMessage, Action code)
      {
         var ex = Assert.Throws<ParseException>(() => code());
         Assert.AreEqual(expectedMessage, ex.Message);
      }

      protected void SetIfSuccess(Response response)
      {
         if (response.Success) { Set(); }

         Assert.Fail(response.Error.Message);
      }
      protected void Set()
      {
         Trigger.Set();
      }
      protected void WaitOne()
      {
         Assert.IsTrue(Trigger.WaitOne(3000), "Test terminated without properly signalling the trigger");
      }
   }

   internal static class AutoResetEventExtensions
   {
      public static void Wait(this AutoResetEvent trigger, int count)
      {
         for (var i = 0; i < count; ++i)
         {
            trigger.WaitOne();
         }
      }
   }
}