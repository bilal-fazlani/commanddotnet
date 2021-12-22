# Data Types

Arguments can be defined with any type that...

* is a primitive type: 
* has a [TypeConverter](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.typeconverter)
* contains a string constructor
* has a `public static Parse(string)` method or `public static Parse(string, {optional paremeters})`
  
The constructor and static Parse method may contain additional optional parameters but must contain only a single required string parameter.

<!-- snippet: argument_types_username -->
<a id='snippet-argument_types_username'></a>
```c#
public class Username
{
    public string Value { get; }

    public Username(string value) => Value = value;
    public Username(string value, DateTime? validUntil = null) => Value = value;

    public static Username Parse(string value) => new(value);
    public static Username Parse(string value, DateTime? validUntil = null) => new(value, validUntil);
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Arguments/Arguments/Argument_Types.cs#L19-L30' title='Snippet source file'>snippet source</a> | <a href='#snippet-argument_types_username' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Any of those constructors or Parse methods will allow conversion from string input, as shown in this example

<!-- snippet: argument_types -->
<a id='snippet-argument_types'></a>
```c#
public void Login(IConsole console, Username username, Password password)
{
    console.WriteLine($"u:{username.Value} p:{password}");
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/Arguments/Arguments/Argument_Types.cs#L11-L16' title='Snippet source file'>snippet source</a> | <a href='#snippet-argument_types' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

<!-- snippet: argument_types_login -->
<a id='snippet-argument_types_login'></a>
```bash
$ myapp.exe Login roy rogers
u:roy p:*****
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.DocExamples/BashSnippets/argument_types_login.bash#L1-L4' title='Snippet source file'>snippet source</a> | <a href='#snippet-argument_types_login' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Includes, but not limited to:

- `string`,`char`,`enum`,`bool`
- `short`,`int`,`long`,`decimal`,`double`
- `Guid`,`Uri`,`FileInfo`,`DirectoryInfo`

Also supports `Nullable<T>` and `IEnumerable<T>` (`T[]`, `List<T>`, etc.) where T can be converted from string.

## Adding support for other types

Options for supporting other types

* If you control the type, consider adding a constructor with a single string parameter.
* Create a [TypeConverter](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.typeconverter) for your type
* Create a [type descriptor](#type-descriptors)

### Type Descriptors

Type descriptors are your best choice when you need 

- to override an existing TypeConverter
- conditional logic based on argument metadata (custom attributes, etc)
- the converter only for parsing parameters and not the business logic of your application, ruling out a TypeConvertor

Implement [IArgumentTypeDescriptor](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/TypeDescriptors/IArgumentTypeDescriptor.cs) or instantiate a [DelegatedTypeDescriptor<T>](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/TypeDescriptors/DelegatedTypeDescriptor.cs) and register with `AppSettings.ArgumentTypeDescriptors.Register(...)`.

If the type has a limited range of acceptable values, the descriptor should also implement [IAllowedValuesTypeDescriptor](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/TypeDescriptors/IAllowedValuesTypeDescriptor.cs).  See [EnumTypeDescriptor](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/TypeDescriptors/EnumTypeDescriptor.cs) for an example.

<!-- snippet: type_descriptors_enum -->
<a id='snippet-type_descriptors_enum'></a>
```c#
public class EnumTypeDescriptor : 
    IArgumentTypeDescriptor,
    IAllowedValuesTypeDescriptor
{
    public bool CanSupport(Type type) => 
        type.IsEnum;

    public string GetDisplayName(IArgument argument) => 
        argument.TypeInfo.UnderlyingType.Name;

    public object ParseString(IArgument argument, string value) => 
        Enum.Parse(argument.TypeInfo.UnderlyingType, value);

    public IEnumerable<string> GetAllowedValues(IArgument argument) => 
        Enum.GetNames(argument.TypeInfo.UnderlyingType);
}
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/TypeDescriptors/EnumTypeDescriptor.cs#L6-L23' title='Snippet source file'>snippet source</a> | <a href='#snippet-type_descriptors_enum' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Use [DelegatedTypeDescriptor](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/TypeDescriptors/DelegatedTypeDescriptor.cs) just to override the display text or factory method for the type.

<!-- snippet: type_descriptors_string -->
<a id='snippet-type_descriptors_string'></a>
```c#
new DelegatedTypeDescriptor<string>(Resources.A.Type_Text, v => v),
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/TypeDescriptors/ArgumentTypeDescriptors.cs#L21-L23' title='Snippet source file'>snippet source</a> | <a href='#snippet-type_descriptors_string' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

See [StringCtorTypeDescriptor](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/TypeDescriptors/StringCtorTypeDescriptor.cs) and [ComponentModelTypeDescriptor](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/TypeDescriptors/ComponentModelTypeDescriptor.cs) for examples to create your own.

<!-- snippet: type_descriptors_type_convertor -->
<a id='snippet-type_descriptors_type_convertor'></a>
```c#
public class ComponentModelTypeDescriptor : IArgumentTypeDescriptor
{
    public bool CanSupport(Type type)
    {
        var typeConverter = TypeDescriptor.GetConverter(type);
        return typeConverter.CanConvertFrom(typeof(string));
    }

    public string GetDisplayName(IArgument argument)
    {
        return argument.TypeInfo.UnderlyingType.Name;
    }

    public object? ParseString(IArgument argument, string value)
    {
        var typeConverter = argument.Arity.AllowsMany()
            ? TypeDescriptor.GetConverter(argument.TypeInfo.UnderlyingType)
            : TypeDescriptor.GetConverter(argument.TypeInfo.Type);
        return typeConverter.ConvertFrom(value)!;
    }
```
<sup><a href='https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/TypeDescriptors/ComponentModelTypeDescriptor.cs#L6-L27' title='Snippet source file'>snippet source</a> | <a href='#snippet-type_descriptors_type_convertor' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->
