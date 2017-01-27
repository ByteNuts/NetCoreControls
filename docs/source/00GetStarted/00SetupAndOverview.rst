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

**Beta Only:** Please add the following MyGet repo to download latest binaries::

    https://www.myget.org/F/netcorecontrols/api/v3/index.json



**1. Install the NetCoreControls NuGet package**

Add to ``project.json`` the following dependency::

    "NetCoreControls" : "0.1.0-beta-\*"

Or you can use the Package Manager Console::

    Install-Package ByteNuts.NetCoreControls



**2. Add references to CSS and Script files**

Inside your ``<head></head>`` tag, insert the following::

    <link href="@Url.Action("GetNccCssFile", "NetCoreControls")" rel="stylesheet">

On the bottom of your page, just above the ``</body>`` tag, insert the following::

    <script type="text/javascript" src="@Url.Action("GetNccJsFile", "NetCoreControls")"></script>

.. note:: Although the tag that links to the stylesheet is optional, the script is mandatory and should be placed after the jQuery link.



**3. Reference the assembly to enable usage as TagHelpers**

In your ``_ViewImports.cshtml`` file inside your ``Views`` folder, add the following line::

    @addTagHelper "*, NetCoreControls"