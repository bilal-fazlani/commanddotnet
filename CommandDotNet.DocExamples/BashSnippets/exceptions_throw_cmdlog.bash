// begin-snippet: exceptions_throw_cmdlog
$ example.exe Throw yikes
System.ArgumentException: yikes (Parameter 'message')
Properties:
  Message: yikes (Parameter 'message')
  ParamName: message
Data:
  method: Throw

***************************************
Original input:
  Throw yikes

command: Throw

arguments:

  message <Text>
    value: yikes
    inputs: yikes
    default:

Tool version  = doc-examples.dll 1.1.1.1
.Net version  = .NET 5.0.13
OS version    = Microsoft Windows 10.0.12345
Machine       = my-machine
Username      = \my-machine\username
***************************************

Usage: example.exe Throw <message>

Arguments:

  message  <TEXT>
// end-snippet