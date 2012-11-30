# SAS custom task example: Catalog Explorer
***
This repository contains one of a series of examples that accompany
_Custom Tasks for SAS Enterprise Guide using Microsoft .NET_ 
by [Chris Hemedinger](http://support.sas.com/hemedinger).

This example is discussed in **Chapter 16: Building a SAS Catalog Explorer**.  It's also
discussed in this blog post: 
[Viewing SAS catalogs in SAS Enterprise Guide](http://blogs.sas.com/content/sasdummy/2010/05/10/viewing-sas-catalogs-from-sas-enterprise-guide/).  
This example has 2 versions: 
- one for SAS Enterprise Guide 4.1, built using C# 
with Microsoft Visual Studio 2003.  It should run in SAS Enterprise Guide 4.1 and later.
- one for SAS Enterprise Guide 4.2 and later, built using Microsoft Visual Studio 2008.  This newer example uses 
Windows Presentation Foundation (WPF) instead of Windows Forms for the user interface.

## About this example
The SAS Catalog Explorer task in this chapter provides a 
utility to examine the contents of a SAS catalog and perform 
simple operations, such as delete catalog entries. There are two versions of this task:

- One version is compatible with SAS Enterprise Guide 4.1 and uses Microsoft .NET 1.1 and Windows Forms.
- The other version is compatible with SAS Enterprise Guide 4.2 and 4.3, and uses Microsoft .NET 3.5 and Windows Presentation Foundation (WPF).

Actually, the task version that works with SAS Enterprise Guide 4.1 also works with 
version 4.2 and 4.3. This compatibility helps 
illustrate the following important aspects of SAS custom tasks:
Custom tasks are forward compatible. The task that 
you build to work with one version of SAS Enterprise Guide should continue to work with future versions of 
SAS Enterprise Guide. As long as your task implementation complies with the documented task API and 
custom task contracts, it should continue to work with future versions of the application. 

Building custom tasks using SAS.Tasks.Toolkit results in less code. If you compare the two versions of the 
SAS Catalog Explorer task (the 4.1 version and the 4.2/4.3 version), you'll notice 
that the 4.1 version contains a lot more code and is more complex. This is despite the fact that the two versions 
are almost functionally equivalent.
The 4.2/4.3 version uses WPF for the user interface. WPF is the newer user interface technology from Microsoft. It enables you to 
encode much of the user interface behavior into a special form of XML called XAML.
With the XAML file format and special WPF features such as data binding, it's easy to present a 
user interface with a lot of content and a few lines of code. The tricky part is determining 
exactly what those few lines of code should be.

