Supports all types with a string constructor or where a TypeConverter is defined that can convert from a string.  Includes, but not limited to:

- `string`
- `char`
- `bool`
- `enum`
- `short`
- `int`
- `long`
- `decimal`
- `double`
- `Guid`
- `Uri`
- `FileInfo`
- `DirectoryInfo`

Also supports `List<T>`, `IEnumerable<T>` and `Nullable<T>` where T can be converted from string.  Note: `T[]` is not currently supported.

These are applicable for both - Options and Arguments

Note for arguments: 
- There can be only one `List` argument in the method. It can be used with other non `List` type arguments or `List` type options.
- If the method has a `List` type argument, it should be defined last in the order.

### Adding support for new types

In most cases, create a [TypeConverter](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.typeconverter?view=netframework-4.8) for your type

If you need 

- to override an existing TypeConverter
- conditional logic based on argument metadata (custom attributes, etc)
- the converter only for parsing parameters and not the business logic of your application

Implement `ITypedArgumentTypeDescriptor` or `IGenericArgumentTypeDescriptor` and register with `AppSettings.ArgumentTypeDescriptors.Register(...)`.