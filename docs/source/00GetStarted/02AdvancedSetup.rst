Advanced Setup
==============

There are some more additional settings that can be used to setup controls.


Settings
--------

This settings are global to the controls, and are placed on the ``appsettings.json`` file, within a section named ``NccSettings``::

    {
        (...)
        "Logging": {
            (...)
        },
        "NccSettings": {
            "UseDependencyInjection": "true",
            "DataProtectionKey": "11111111-2222-3333-4444-555555555555"
        } 
    }
	
- **UseDependencyInjection** *(default: true)* - indicates wether the data access class should be requested directly from the iOC or if it should be instantiated.

- **DataProtectionKey** - defines the key to be used when encrypting the context of each control. The example uses a Guid, but it can be any type of string.



Exception Handling
------------------

Exceptions within tag helpers are contained and isolated from the page, preventing that an error in a control blocks the rendering of a page.

Neverthless, if the error occurs on the Razor code, such as trying to read an inexisting property, these errors cannot be handled by the NccControls and prevent the page from rendering.

To prevent this from happening, you can use the ``RenderControl`` TagHelper to render the controls. So, instead of using::

    @await Html.PartialAsync("/Views/NccGrid/Partials/_GridExample.cshtml")

just use::

    <ncc:render-control Context="@(ViewData["GridExample"] as NccGridContext)"></ncc:render-control>