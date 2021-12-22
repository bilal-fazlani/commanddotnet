// begin-snippet: fluent_validation_create_invalid
$ dotnet table.dll create TooLongTableName --server bossman --owner abc -qv
'Table' is invalid
  The length of 'Name' must be 10 characters or fewer. You entered 16 characters.
  'Owner' is not a valid email address.
'Host' is invalid
  sever is not a valid fully-qualified http, https, or ftp URL
'Verbosity' is invalid
  quiet and verbose are mutually exclusive. There can be only one!
// end-snippet