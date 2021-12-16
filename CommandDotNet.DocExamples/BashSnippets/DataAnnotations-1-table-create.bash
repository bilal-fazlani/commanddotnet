// begin-snippet: DataAnnotations-1-table-create
$ dotnet hr.dll create TooLongTableName --server bossman --owner abc -sv
silent and verbose are mutually exclusive. There can be only one!
'server' is not a valid fully-qualified http, https, or ftp URL.
'name' must be a string or array type with a maximum length of '10'.
'owner' is not a valid e-mail address.
// end-snippet