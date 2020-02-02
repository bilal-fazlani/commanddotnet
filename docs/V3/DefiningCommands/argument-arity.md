# Argument Arity

Arity indicates the number of values an argument can contain. 

`ArgumentArity` is defined by a `min` and `max` value.  When `min` is 0, then no values are required.  When `max` is greater than 1, then multiple values can be provided. 

`Min` is set to 0 when the argument type is Nullable or when defined by an optional parameter.  

`IArgumentModel` properties for reference types cannot be inferred to have a `Min` of 0.

!!! note
    There is currently no validation or way to set the arity via attributes.
    
    It is left to developers to define middleware to modify and validate the arity.
