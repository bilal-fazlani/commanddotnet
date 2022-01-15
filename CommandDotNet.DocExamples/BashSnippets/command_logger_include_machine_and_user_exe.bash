// begin-snippet: command_logger_include_machine_and_user_exe
$ example.exe [cmdlog] Add 1 1

***************************************
Original input:
  [cmdlog] Add 1 1

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

Machine   = my-machine
Username  = \my-machine\username
***************************************
2
// end-snippet