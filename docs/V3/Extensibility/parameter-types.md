# Parameter Types

Argumetns can be defined with all types with a string constructor or where a TypeConverter is defined that can convert from a string.  Includes, but not limited to:

- `string`,`char`,`enum`,`bool`
- `short`,`int`,`long`,`decimal`,`double`
- `Guid`,`Uri`,`FileInfo`,`DirectoryInfo`

Also supports `Nullable<T>` and `IEnumerable<T>` (`T[]`, `List<T>`, etc.) where T can be converted from string.

!!! Note 
    There can be only one `List` operand in the method, where `List` is any type of `IEnumerable`. 
    `List` operands must be the last operand defined for the method.

### Adding support for new types

In most cases, create a [TypeConverter](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.typeconverter?view=netframework-4.8) for your type

If you need 

- to override an existing TypeConverter
- conditional logic based on argument metadata (custom attributes, etc)
- the converter only for parsing parameters and not the business logic of your application

Implement `IArgumentTypeDescriptor` or instantiate a `DelegatedTypeDescriptor<T>` and register with `AppSettings.ArgumentTypeDescriptors.Register(...)`.