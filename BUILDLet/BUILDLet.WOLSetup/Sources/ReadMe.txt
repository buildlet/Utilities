BUILDLet WOL
============

January 24, 2015


This application is front end to send the magic packet, built by WFP.


Microsoft .NET Framework 4.5 is required to run this application.

If "WOL.conf" exists in proper directory, 17 characters of the head of the 
file is read and it is used as MAC address.  
MAC address should be written like "FF:FF:FF:FF:FF:FF".

The file "WOL.conf" is searched in the following order.
  1. Program Folder  (C:\Program Files (x86)\BUILDLet\WOL\)
  2. My Documents    (C:\Users\<User Name>\Documents\)
  3. Windows Folder  (C:\Windows\)
  4. System32 Folder (C:\Windows\System32\)


Release History
---------------
January 24, 2015    1.0.0.0    1st Release


License
-------
This software is released under the MIT License, see License.txt.
