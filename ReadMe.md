InterLinq 2.
===

This is InterLinq build by the guys at http://interlinq.codeplex.com/, currently the is not active anymore there so I have taken a cut of the source and modified adding support for Silverlight 4, http bindinds and fixing some small issues. 

All features from the original project are still there bar few like the service type (remoting, their version of wcf service).

My plan for this project is to finish adding support to all 101 Linq tests currently half of the tests arre written.

The current pass rate is 43 / 101 running on memory and 23 / 101 running over wcf service on http. The same pass rate of the memory tests is possible with a wcf service using tcp/ip (netdatacontract)

There is a fake warning that will point you to the area of the code that you can turn on or off the memory or http tests.

Project (Original description)
---
InterLINQ is an IQueryable Provider implementation. LINQ stands for Language Integrated Query and is one of the most important and powerful features of the new .NET 3.5 Technology. A LINQ Query is represented by an Expression Tree.

Multi-Tier functionality is not provided by LINQ, but this new technology makes it possible by serializing a the Expression Tree of the LINQ Query and sending it to a WCF Service. The Remote Service deserializes this Expression Tree and starts the execution. Return value of the Remote Service's called method is the result of the executed query.

This project still under Apache2 License and some of the new part are under MS-PL license.

Original Project
---
  - [InterLinq Project In Codeplex](http://interlinq.codeplex.com/)
  