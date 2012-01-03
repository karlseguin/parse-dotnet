
# Parse for Windows Phone
This is a Windows Phone library for accessing the <http://parse.com> web services via the REST api.

This library is in very early development.

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

### Deleting Objects
This should be obvious by now.


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