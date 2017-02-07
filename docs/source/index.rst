NetCoreControls
===============

A set of UI controls for ASP.NET Core.

`GitHub repository <https://github.com/ByteNuts/NetCoreControls>`_

`Constrols demonstration <http://netcorecontrols.bytenuts.com>`_


Features
--------

**- Independent from data source**

You can use any data source you prefer. Just set up a method that returns the data you want to display.


**- Dynamic models allowed**

There is no need to create a model to render data to a control. Just return ``dynamic`` from your data method.


**- AJAX Enabled**

All controls use AJAX to communicate with the server and perform their actions.


**- Controls are connected**

You can easily associate a submit button or a filter with more than one control, even with different controls.


**- Subscribe control events or create custom ones**

All controls share the same base events. They also offer some other events related to the control itself.
But hey!, if that isn't enough, you can create your one custom events!

.. toctree::
   :maxdepth: 2
   :hidden:
   :caption: Get started

   00GetStarted/00SetupAndOverview
   00GetStarted/01Usage
   00GetStarted/02AdvancedSetup

.. toctree::
   :maxdepth: 2
   :hidden:
   :caption: NCC Controls

   01NccControls/00SharedBase
   01NccControls/01NccGrid
   01NccControls/02NccHtmlRender