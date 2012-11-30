Task: SAS Catalog Explorer

This task allows you to explore the SAS Catalog entries within
the SAS servers and libraries that you have access to.
With this task in SAS Enterprise Guide, you can view
the catalog listings and view the content of certain
entry types, including SOURCE entries and SCL entries. 
(To see SCL source code, it must have been compiled to 
keep the source with the entry.)

Requirements:
- Microsoft .NET Framework 3.5
- SAS Enterprise Guide 4.2 or later
- SAS 9.2 or later (local on your Windows machine, or a remote SAS session)


How to install this task:
1. Copy the contents of the ZIP file to one of these locations:

- For use by all users on a machine (requires administrative privileges to install):
  C:\Program Files\SAS\EnterpriseGuide\4.2\Custom  (create the Custom folder if needed)
  
- For use by just the current user:
  %appdata%\SAS\EnterpriseGuide\4.2\Custom (create the Custom folder if needed)
  
  Note: The "%appdata%" environment variable is a Windows variable that 
  maps to something like "C:\Users\<yourAccount>\AppData\Roaming" or 
  "C:\Documents and Settings\<yourAccount>\Application Data"
	  
2. Start SAS Enterprise Guide 4.2.  The new task appears in the menus
   as Tools->Add-In->SAS Catalog Explorer