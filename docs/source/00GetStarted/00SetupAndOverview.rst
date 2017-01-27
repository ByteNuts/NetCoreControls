Setup and Overview
==================

You can use these controls with any web ASP.NET Core project.
All controls were built natively for .NET Core and use Tag Helpers to perform all their logic.

.. note:: NetCoreControls only targets ASP.NET Core 1.1. If you have an ASP.NET Core 1.0 project then you can follow `this guide for updating to ASP.NET Core 1.1 <https://blogs.msdn.microsoft.com/webdev/2016/11/16/announcing-asp-net-core-1-1/>`_.


Dependencies
------------

You must use the jQuery javascript library starting from v2.
The controls also use some styles from Bootstrap, but it's not a mandatory requirement since you can link your own styles classes.


Basic setup
-----------

#. Install the NetCoreControls NuGet package
Add to ``project.json`` the following dependency:

    "NetCoreControls" : "0.1.0-beta-\*"

**OR** use the Nuget package manager

    Install-Package ByteNuts.NetCoreControls