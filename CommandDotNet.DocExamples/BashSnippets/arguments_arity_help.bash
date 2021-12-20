// begin-snippet: arguments_arity_help
$ app.exe --help
Usage: app.exe <RequiredBool> <DefaultBool> <RequiredRefType> <DefaultRefType> <requiredBool> <requiredRefType> [<nullableBool> <nullableRefType> <optionalBool> <optionalRefType>]

Arguments:

  RequiredBool     <BOOLEAN>
  Allowed values: true, false

  DefaultBool      <BOOLEAN>  [True]
  Allowed values: true, false

  RequiredRefType  <URI>

  DefaultRefType   <URI>      [http://apple.com/]

  requiredBool     <BOOLEAN>
  Allowed values: true, false

  requiredRefType  <URI>

  nullableBool     <BOOLEAN>
  Allowed values: true, false

  nullableRefType  <URI>

  optionalBool     <BOOLEAN>  [False]
  Allowed values: true, false

  optionalRefType  <URI>
// end-snippet