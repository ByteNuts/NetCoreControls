Setup and Overview
==================

You can use these controls with any web ASP.NET Core project.
All controls were built natively for .NET Core and use Tag Helpers to perform all their logic.

.. note:: NetCoreControls only targets ASP.NET Core 1.1. If you have an ASP.NET Core 1.0 project then you can follow `this guide for updating to ASP.NET Core 1.1 <https://blogs.msdn.microsoft.com/webdev/2016/11/16/announcing-asp-net-core-1-1/>`_.


Dependencies
------------

You must use the **jQuery javascript** library starting from v2.x.

The controls also use some styles from Bootstrap, but it's not a mandatory requirement since you can link your own styles classes.


Basic setup
-----------


**1. Install the NetCoreControls NuGet package**

Add to ``project.json`` the following dependency::

    "NetCoreControls" : "1.0.0-beta1"

Or you can use the Package Manager Console::

    Install-Package NetCoreControls -Pre



**2. Register NetCoreControls**

In your ``Startup.cs`` class, inside the ``ConfigureServices`` method, add the following line after Mvc registration::

	services.AddMvc();
	(...)
	services.AddNetCoreControls(Configuration);



**3. Reference the assembly to enable usage as TagHelpers**

In your ``_ViewImports.cshtml`` file inside your ``Views`` folder, add the following line::

    @addTagHelper "*, NetCoreControls"



**4. Add references to CSS and Script files**

Inside your ``<head></head>`` tag, insert the following::

    <link href="@Url.Action("GetNccCssFile", "NetCoreControls")" rel="stylesheet">

On the bottom of your page, just above the ``</body>`` tag, insert the following::

    <script type="text/javascript" src="@Url.Action("GetNccJsFile", "NetCoreControls")"></script>

.. note:: Although the tag that links to the stylesheet is optional, the script is mandatory and should be placed after the jQuery link.






**Daily builds**

To use the latest daily builds of the controls, please add the following MyGet repo to download latest binaries::

    https://www.myget.org/F/netcorecontrols/api/v3/index.json

Add to ``project.json`` the following dependency::

    "NetCoreControls" : "1.0.0-beta-*"