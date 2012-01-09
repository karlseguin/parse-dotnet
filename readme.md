
# Parse for Windows Phone
This is a Windows Phone library for accessing the <http://parse.com> web services via the REST api.

This library is in very early development.

## Installation
The library is availble on nuget, at <http://nuget.org/packages/parse>

## Configuration
On application start you should configure the driver by supplying your application id and master key (authorization with the REST API is done via the master key and not the client key):

	ParseConfiguration.Configure(APPLICATION_ID, MASTER_KEY);

Once configured, you can create an instance of the parse driver:

	var parse = new Driver();

This instance is both thread-safe and inexpensive to create..you can either keep a single one around or create them as needed.

## Objects
Parse allows developers to easily persist data from a mobile device into the cloud. The data can be retrieved at a later time. 

### ParseObject & IParseObject
Although Parse can deal with any serializable object, you'll gain some small benefits by inheriting from the `ParseObject` or, failing that, implementing `IParseObject`. 

###  Saving Objects
Parse can save any object, as long as it can be serialized to JSON (using [Json.NET](http://json.codeplex.com/))

	var user = new User{Name = "Goku", PowerLevel = 9001};
	parse.Objects.Save(user);

A second optional callback can be provided:

	var user = new User{Name = "Goku", PowerLevel = 9001};
	parse.Objects.Save(user, r =>
	{
		if (r.Success)
		{
			var id = r.Data.Id;
			var createdAt = r.Data.CreatedAt;
		}
		else
		{
			//log r.Error.Message;
		}
	});

Due to the asynchronous nature of the driver, the original `user` instance won't be updated with the parse id and created at values (there's just something weird about having this updated at some unspecific time by a separate thread).

###  Updating Objects
Updating works similar to saving. Again, the callback is optional. You can either pass in any object (along with the object's string id), or you can pass in an object of `IParseObject`:

	var user = new User{Id = "123", Name = "Goku", PowerLevel = 9001};
	parse.Objects.Update(user.Id, user);

	//or 
	var blah = new ParseObjectChild{Id = "123", BadExample = true};
	parse.Objects.Update(blah);

	//with callback
	var blah = new ParseObjectChild{Id = "123", BadExample = true};
	parse.Objects.Update(blah, r => 
	{
		if (r.Success)
		{
			var updatedAt = r.Data.UpdatedAt;
		}
	});

Alternatively you can update only specific fields:

	//execute takes an optional callback
	var blah = new ParseObjectChild{Id = "123", BadExample = true};
	parse.Objects.Update(blah).Set(b => b.BadExample, false).Execute();
	
	//or
	parse.Objects.Update<User>("1994").Set(u => u.Password, "password").Execute(r =>
	{
		if (r.Success) { .... }
	});

Like saving, updating a ParseObject won't update the local instance.

### Incrementing Values
Counter can be incremented via the `Increment` method:

	parse.Objects.Increment<Hits>("AJ7Yl6OHB4", c => c.Count, 1, r =>
	{
		if (r.Success) { var currentCount = r.Data; }
	});
	
	//or
	parse.Objects.Increment<Hits>("AJ7Yl6OHB4", "visits", -1)
	
	//or
	var blah = new ParseObjectChild{Id = "123"};
	parse.Objects.Increment(blah, c => c.Visits, 10);

Like saving and updating, the last example won't update the actual `blah` instance.

### Deleting Objects
This should be obvious by now.

	parse.Objects.Delete<User>(USERID, optionalCallback);

### Getting an Object By Id

	parse.Objects.Get<User>("9000!", r =>
	{
		if (r.Success)
		{
			var user = r.Data;
		}
	});

### Querying Objects
The library has a very basic query interface:

	parse.Objects.Query<User>().Where(c => c.Name == "Leto" && c.Home == "Arakis").Execute(r => 
	{
		if (r.Success) 
		{ 
			var found = r.Data.Results;
		}
	});

Or (`||`) are not supported. Supported operators are `==`, `!=`, `>`, `>=`, `<` and `<=` and boolean (`c.IsGhola` and `!c.IsGhola`).

`Skip` and `Limit` are supported for paging:

	parse.Objects.Query<User>().Where(c => c.Name == "Leto").Skip(10).Limit(100).Execute(r => 
	{
		if (r.Success) 
		{ 
			var found = r.Data.Results;
		}
	});


When `Count` is called, a count of records is also returned. This can be combined with `Limit(0)` to only return a count...or `Limit(X)` to return the first X documents plus a count of documents:

	parse.Objects.Query<User>().Where(c => c.Name == "Leto").Count().Limit(0).Execute(r => 
	{
		if (r.Success) 
		{ 
			var found = r.Data.Results; //will be empty
			var count = r.Data.Count;
		}
	});

Regular expressions can be used used via the Regex.IsMatch method:

	parse.Objects.Query<User>().Where(c => Regex.IsMatch(c.Name, "[a-e]").Execute(r => 
	{
		...
	});

Along with regular expressions, `StartsWith`, `EndsWith` and `Contains` are also supported on `string` members:

	parse.Objects.Query<User>().Where(c => c.Name.EndsWith("ly")).Execute(r => 
	{
		...
	});

By importing `System.Linq` you can provide a list of values for an IN operation:

	using System.Linq;
	....
	var names = new[]{"Jessica", "Paul", "Leto"};
	parse.Objects.Query<User>().Where(c => names.Contains(c.Name)).Execute(r => 
	{
		...
	});


## Users
Parse allows developers to register and authenticate users. In parse, all users must have a `username` and `password` field. Within this library this is enforced by the `IParseUser` interface. This means you'd likely have a `User` class which looks something like:

	public class User : IParseUser
	{
		public string UserName{get;set;}
		public string Password{get;set;}
		//any other properties specific to your app here
	});

### Registration
This method registers a user with parse. All communication with the parse api happens over SSL, and the parse backend only stores an encrypted version of the password (according to their documentation anyways!)

	var user = new User{Name = "Leto", Password = "Ghanima", DuncansKilled = 58};
	parse.Users.Register(user)

Registration takes an optional callback, which exposes a `ParseObject`, just like the `Objects.Save` method