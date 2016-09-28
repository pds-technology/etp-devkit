### ETP DevKit v1.1

##### ABOUT THE DEVKIT

The ETP DevKit is a .NET library providing a common foundation and the basic infrastructure needed to communicate via the Energistics Transfer Protocol (ETP).  The library is written in C# using the .NET Framework version 4.5.2.  It is designed to simplify and standardize ETP client and server development by integrating the necessary dependencies and abstracting away the lower level details of communicating via ETP.  It builds on the ETP Messages library, which provides .NET definitions for the ETP messages and associated data structures.

The ETP DevKit provides a definition and base implementation of each interface described in the ETP Specification.  Each interface implementation has been developed as a protocol handler that can be used out of the box or extended to provide additional functionality.  This abstracts away the low level details of sending and receiving messages between clients and servers.  Customized processing of messages can be achieved either by registering handlers for the various interface events or by deriving from the library’s protocol handlers and overriding the virtual message handling methods.

The aim of the ETP DevKit is to reduce the time it takes to develop and evaluate the latest version of the standards utilizing the Energistics Transfer Protocol and to provide a shared and tested framework for establishing and facilitating communication between applications needing to exchange data.

##### LICENSE

The ETP DevKit was developed by Petrotechnical Data Systems (PDS) and contributed to Energistics.  It is provided as an open source project under the Apache License, Version 2.0.  Further development will be guided by Energistics and the user community.

##### RESOURCES

The ETP Specification defines the messages and interfaces that can be used to exchange data between applications that support the EnergyML family of data standards: WITSML, PRODML and RESQML.  The messages define the content that is exchanged between applications and the interfaces define the expected behavior in exchanging messages.  More information about ETP resources can be found at [http://www.energistics.org/standards-portfolio/energistics-transfer-protocol](http://www.energistics.org/standards-portfolio/energistics-transfer-protocol).