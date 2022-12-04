
![Icon](Graphics/icon.png)

Core Command Line 
=================


This is a light-weight dotnet standard 2.1 library, that you can add to your console applications, 
 and create your command line application easily.


 How to use
 ==========

  * Install package from [NuGet](https://www.nuget.org/packages/CoreCommandLine)
  * Create an application by extending the class ```CommandLineApplication```.
  * Add each Command you need, by creating a class which implements ```ICommand```. You can also extend ```CommandBase``` instead, so you can make use of some pre implemented and helper methods in it.
  * Introduce each command to your application by using the ```SubCommand``` attribute. ex:
```c#

  [Subcommands(typeof(FirstCommand),typeof(SecondCommand))]
  public class ExampleApplication:CommandLineApplication{
    ...
  }
```

  * In your main application entry, instantiate your application and start it.
  

___Please checkout the Example project in repository, for a quick start___

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

__NOTE__: If you use dotnet's builtin di system, you can just simply call ```application.UseDotnetResolver(serviceProvider)``` 
passing an instance of dotnet's ```IServiceProvider``` to it.

