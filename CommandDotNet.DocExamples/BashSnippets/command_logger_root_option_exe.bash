// begin-snippet: command_logger_root_option_exe
$ example.exe --logcmd Add 1 1

***************************************
Original input:
  --logcmd Add 1 1

command: Add

arguments:

  x <Number>
    value: 1
    inputs: 1
    default:

  y <Number>
    value: 1
    inputs: 1
    default:

options:

  logcmd <flag>
    value: True
    inputs: true (from: --logcmd)
    default:

Tool version  = doc-examples.dll 1.1.1.1
.Net version  = .NET 5.0.13
OS version    = Microsoft Windows 10.0.12345
***************************************
2
// end-snippet