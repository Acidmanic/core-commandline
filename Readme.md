
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