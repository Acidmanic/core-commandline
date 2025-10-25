
![Icon](Graphics/icon.png)

Core Command Line 
=================


This is a light-weight dotnet standard 2.1 library, that you can add to your console applications, 
 and create your command line application easily.


 How to use
 ==========

  * Create a simple Console application
  * Install package from [NuGet](https://www.nuget.org/packages/CoreCommandLine)
  * Create Your commands
  * Setup and execute the application in your entry file (progra.cs)
  

Creating Commands
-----------------

  You can implement the ```ICommand``` interface. Prefferebly you can inherit one of the common base commands.


__Implementing ```ICommand```__

* Implement __Execute__ methods. It's reasonable to implement one of the Sync or Async versions regarding the nature of the task in hand, and then call the implemented method from the other version. When execute is called, it would receive all arguments available for it. but the method must return how many of these arguments was belonging to this command.

    * The __Description__ property would return a description about what this command would do. It later would be displayed in help manual pages.

    * The __SetLogger__ method would be called before command being executed and would deliver a logger object. This feature makes it easier to create simple commands that might only need a logger, without populating the command registery in di.


* Apply needed attributes:
    *  ```[RootCommand]```

    __Root commands:__ A root command is a command which is a direct child of the application itself. In other words, when application is started with some arguments for exampl ```your-app first secods third```, it will look for command named __first__ in root commands and the _second_ and _third_ phrases in this example, they might be raw arguments for __first__ or sub-commands of __first__.

    * ```[CommandName("name","-sn")]```

    This attribute is needed to provide a name and a short name for your command. both these names can be used to call your command. They also will be displayed in help manual page.

    * ```[SubCommands(typeof(SubCommandType),...)]```

    This attribute will define subcommands for a command. Commands have a tree strcture in this sencse that each command can be considered the entry of an application. For example you can have __Request__ command which might have __Post__ and __Get__ sub commands. In this case the main work would be performed in Post and Get commands and the Request command is actually only a hub. Another example which is more common, would be commands which deliver a value to main command. For example you might have a __Download__ command which needs a url and also knowing that should it accept any ssl certificate or not. You can provide a __Uri__ subcommand to read the uri 
    and a __--trust-certificates__ subcommand which if present, would indicate that certificates can be trusted. 


The Context which is present in execute methods, is shared throuout the execution of each command and all it's children. So children can set the values into the context and the parent commands can retreive them by reading the context.

__The child commands are always executed before their parents__



__Extending Command base classes__

* ```CommandBase```: this class is actualy a simple implementation of ```ICommand`` plus some helper mthods for reading and writing information.
* ```NodeCommandBase:CommandBase``` and ```NodeAsyncCommandBase:CommandBase```:
Node commands are the default base class for performing any task. This is the type to extend for commands that perform the duties of the application.

* ```FlagCommandBase``` This command is a good choice for cases that you need set a flag if a certain command is present in the command line. (Ex. --trust-certificates)

* ```ParameterCommandBase``` This is the base command for those cases that you want to specify a value for an argument. (Ex. url https://some.where.com)

* ```ArgumentRangeCommandBase``` and ```ArgumentRangeAsyncCommandBase```: 
this for those cases that you want to use a number of arguments given after the command name. If you return null 
for ```NumberOfExpectedArguments```, the command would consume all the arguments to the end.


The difference between the sync and async classes is that sync calsses provide a sync implementation, and their async method would call the sync method. And the async classes behave the opposite.


Setting up the application
--------------------------

* Create an instance of ```ConsoleApplicationBuilder```
  the builder allows you to set applications logger, set the title and description for the application,
   choose dotnet core's builtin Di system or your own 

Features
=========

* Simple to use
* Nested commands (Each command can have its own sub-commands and so on...)
* Auto generated Help command for each command
* Can be started as a regular console application OR interactively
* Supports Dependency Injection



Nested Commands
===============

Each command, can have as many sub commands as needed the same way you added your commands to your application class. For a command to have some sub-commands, you would use ```Subcommands``` attribute on parent command and introduce it's child commands.

Argument Commands - Context
========================

Arguments would be child commands that their job is to acquire a value from ```string[] args``` for it's parent command. the ```Context``` object would be the carrie of this data. the argument-command, processes the args[] object, and saves the result in context. and when parent command gets executed, it can check the context object for it's arguments.

Context
-------

* ```public void Set<T>(string key, T value)```
    * This method will store a value into the context
* ```public T Get<T>(string key, T defaultValue)```
    * This method would be used to provide previously stored data from the context.
* ```ApplicationExit```
    * Setting this property inside any command, would cause the chain of commands to be terminated.
* ```InteractiveExit```
    * Setting this property inside any command, would close the application when is executed interactively.
    * Usually there is no need to set this manually. When you start your application interactively, and __exit__ command is already added to your commands that would set this property when called. So without any extra coding, you can exit the interactive application just by typing exit.

Command Name And Descriptions
================

If you implement ```ICommand``` class, then you would have to implement ```Name```  and ```Description``` properties. On the other hand, by extending the ```CommandBase``` class, by default these properties would be set from the class name of the command. but you still are able to __override__ these properties.


Application Title - Application Description
==================

There are ```ApplicationTitle``` and ```ApplicationDescription``` properties in CommandLineApplication class that you can set on instance you create or in the driven class's constructor. These values would be printed when You start your application.


Default output
==============

There is an ```Output``` property in Application command which determines the default output method for console application to print out it's output. The ```Action<string>``` set into this property, would be delivered to all executing commands to use. 
If you extend ```CommandBase``` class, it's already handled and you can use ```Output("some-string")``` inside your driven class (your command) to write on output.
And if you implemented the ```ICommand``` interface, this output delegate would be delivered to your code when ```SetOutput(..)``` method of your command is called before execution.

Logging
==========

In a similar way to ```Output``` property, you can set ```Logger``` property of your application to make use of your logger of interest. This property is of type ```ILogger``` (microsoft's) and can accept any implementation.


__NOTE__: Using ___Logger___ or default ___Output___ for printing information towards the user, is up to the developers decision. But the recommended usage would be to use ___Output___ for printing execution results and information. And Use logger only for debugging and development logs.
    

Dependency Injection
==================

To make use of any kind of dependency injection, you just need to provide a 
facade of your di/ioc resolver by implementing ```IResolver```. Then you can 
connect your di/ioc by setting the Application's Resolver property to an instance 
of your ```IResolver```. 

by doing so, you can deliver your registered services to your commands by conventional 
constructor injection easily.


__NOTE__: In most of ioc/di implementations, you need to register any thing which at some point 
is supposed to be resolved. since commands with constructor parameters are also being 
resolved using your resolver, it's so important to __REMEMBER TO REGISTER COMMANDS ON YOUR DI REGISTRY__. 
other-wise your commands would not be resolved with resolver, also since they have constructor parameters, 
they would not be instantiated without a resolver and therefore they will not work.


__NOTE__: If you use dotnet's builtin di system, you can just simply call ```application.UseDotnetResolver(serviceProvider)``` 
passing an instance of dotnet's ```IServiceProvider``` to it.

