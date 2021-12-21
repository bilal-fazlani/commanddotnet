// begin-snippet: data_annotations_validation_create_invalid
$ dotnet table.dll create TooLongTableName --server bossman --owner abc -qv
'server' is not a valid fully-qualified http, https, or ftp URL.
'name' must be a string or array type with a maximum length of '10'.
'owner' is not a valid e-mail address.
quiet and verbose are mutually exclusive. There can be only one!
// end-snippet