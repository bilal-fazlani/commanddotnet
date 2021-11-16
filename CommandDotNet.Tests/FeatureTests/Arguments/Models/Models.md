The purpose of these models is to enforce testing of all sample types across tests.
When models are defined per test, it is easy to miss some of the domain, especially when adding to the domain later.

Because only one argument list can be specified, the lists have to be split into different arg models or methods.  
The string list is kept with the other sample types because
1. there will be times the other lists won't need to be tested 
1. this lets us test lists along with single valued arguments

note: This is an experiment.  It may turn out it's better to nest these classes within their test cases.
