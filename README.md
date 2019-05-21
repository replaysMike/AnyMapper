# AnyMapper
[![nuget](https://img.shields.io/nuget/v/AnyMapper.svg)](https://www.nuget.org/packages/AnyMapper/)
[![nuget](https://img.shields.io/nuget/dt/AnyMapper.svg)](https://www.nuget.org/packages/AnyMapper/)
[![Build status](https://ci.appveyor.com/api/projects/status/gfwjabg1pta7em94?svg=true)](https://ci.appveyor.com/project/MichaelBrown/anymapper)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/8001bb10a20c4456a98ed4dde145350a)](https://app.codacy.com/app/replaysMike/AnyMapper?utm_source=github.com&utm_medium=referral&utm_content=replaysMike/AnyMapper&utm_campaign=Badge_Grade_Dashboard)

A CSharp library mapping alternative to AutoMapper with built-in support for Entity Framework 6 and Entity Framework Core. The terse mapper!

## Description

AnyMapper was built as a standalone object mapper that runs on either .Net Framework or .Net Core. The API is backwards compatible with AutoMapper so switching between the two is nearly seamless.

## Differences from AutoMapper

AnyMapper will automatically map between objects of different types as long as they have the same names and the types are the same or are different but convertable automatically.

## Examples

Simple usage:
```csharp
var destObject = Mapper.Map<SourceObject, DestObject>(sourceObject);
```

The above example will map the type `SourceObject` to type `DestObject`. Any fields and properties with the same name will be mapped, recursively. You can also map the same type to create a new copy of the data:

```csharp
var sourceObjectCloned = Mapper.Map<SourceObject, SourceObject>(sourceObject);
```

For all of the examples below we will use the following test classes:
```csharp
// *** classes used in all the examples ***
public class SourceObject
{
  public string Name { get; set; }
  public int Id { get; set; }
  public DateTime DateCreated { get; set; }
  public ICollection<SimpleObject> Items { get; set; }
}

public class DestObject
{
  public string Name { get; set; }
  public int Id { get; set; }
  public DateTime DateCreated { get; set; }
  public string Description { get; set; }
  public bool IsEnabled { get; set; }
  public ICollection<SimpleObject> Items { get; set; }
}

// our custom mapping profile that indicates how one object maps to another
public class MyMappingProfile : Profile
{
  public MyMappingProfile()
  {
    CreateMap<SourceObject, DestObject>()
      .ForMember(x => x.Id, x => x.Id)
      .ForMember(x => x.Name, x => x.Name)
      .ForMember(x => x.DateCreated, x => x.DateCreated)
    ;
  }
}
```

Map one object to another:
```csharp

// configure our mapping profile
var profile = new MyMappingProfile();
Mapper.Configure(config =>
{
  config.AddProfile(profile);
});

// map one object to another
var sourceObject = new SourceObject { Id = 1, Name = "Source object", DateCreated = new DateTime(2018, 1, 1) };
var destObject = Mapper.Map<SourceObject, DestObject>(sourceObject);
// output
// destObject.Id = 1
// destObject.Name = "Source object"
// destObject.DateCreated = "2018-01-01 00:00:00"
// destObject.Description = null
// destObject.IsEnabled = false
// destObject.Items = null
```

Implicitly map (no specified profile) two different objects with similar properties, only matching property names will get mapped:
```csharp
using AnyMapper;

var sourceObject = new SourceObject { Id = 1, Name = "Source object", DateCreated = new DateTime(2018, 1, 1) };
var destObject = Mapper.Map<SourceObject, DestObject>(sourceObject);

// output
// destObject.Id = 1
// destObject.Name = "Source object"
// destObject.DateCreated = "2018-01-01 00:00:00"
// destObject.Description = null
// destObject.IsEnabled = false
// destObject.Items = null

```

