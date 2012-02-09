
# Parse for Windows Phone
This is a Windows Phone library for accessing the <http://parse.com> web services via the REST api.

This library is in very early development.

## Installation
The library is availble on nuget, at <http://nuget.org/packages/parse>

## Configuration
On application start you should configure the driver by supplying your application id and the rest api key:

	ParseConfiguration.Configure(APPLICATION_ID, REST_API_KEY);

Once configured, you can create an instance of the parse driver:

	var parse = new Driver();

This instance is both thread-safe and inexpensive to create..you can either keep a single one around or create them as needed.

Some commands, namely those which delete objects/users/file require your master key. If you plan on using these methods, you should use the `Configure` overload:

	ParseConfiguration.Configure(APPLICATION_ID, REST_API_KEY, MASTER_KEY);

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

	//could also implement IParseObject (in addition to IParseUser)
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

### Login
Returns the user (or null) based on the username and password

	parse.Users.Login("paul", "chani", r => 
	{
		if (r.Success) { var user = r.Data }
	});

### Querying, Updating and Deleting Users
The `Users` api inherits all of the `Objects` functionality. The same methods you use to `Query`, `Delete` and `Update` (including `Increment`) should be used. Simply use `parse.Users.XYZ` instead of `parse.Objects.XYZ`

## GeoPoints
You can add a `Parse.GeoPoint` property to your objects (you can only have 1 such propery per object).

You can find objects ordered by proximity to a location via the `NearSphere` query

	parse.Objects.Query<User>().Where(c => c.Location.NearSphere(11.2, 20.5)).Execute(r => 
	{
		if (r.Success) 
		{ 
			var found = r.Data.Results;
		}
	});

`NearSphere` takes a 3rd optional parameter which is the maxium distance, in miles, to search.

## Files
Files can be uploaded and, optionally, associated with an object.

### Creating A File
You can either create a file from a text value:

	parse.Files.Save("hollow.txt", "Our dried voices, when We whisper together Are quiet and meaningless As wind in dry grass", r => 
	{
		if (r.Success) 
		{ 
			var url = r.Data.Url;
			var name = r.Data.Name;
		}
	});

Or a binary file:

	parse.Files.Save("sand.png", System.IO.File.ReadAllBytes("sand.png"), "image/png", r => 
	{
		if (r.Success) 
		{ 
			var url = r.Data.Url;
			var name = r.Data.Name;
		}
	});

Do note that Parse will give each uploaded a file its own unique name. So, if we were to upload a different "sand.png", it would create a separate file and would **not** overwrite the original.

### Deleting A File
To delete a file you must supply the Parse name of the file (when you saved a file, Parse returned a unique name based on the name you supplied). Also, the Parse driver must have been configured with a MasterKey.

	parse.Files.Delete("12323-sand.png", null);

### Associating Files with Objects
Once you've created a file, you can associate it with an object by wrapping the file name in a `ParseFile` object. Note that this must be the parse-supplied file name:

	var user = new User{Name = "Goku", PowerLevel = 9001, Profile = new ParseFile("1233233-goku.png")};
	parse.Objects.Save(user);