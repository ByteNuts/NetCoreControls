Shared Base
===========

All controls share the same base.

This section highlights what belongs to the base and can be used within all the controls.


Context
-------

The base context class is ``NccContext``.

- **Id** - this is the id of the control. It must be unique in the page, and it's used to reference the control within any filter or other event.

- **RenderForm** *(default: true)* - a flag indicating if the control can render the form automatically or if the user wants to take control of it. A form is not mandatory for simple control interacting, such as filters and paging, but it's mandatory when actions such as update are used.

- **AutoBind** *(default: true)* - indicates if the control shall read and bind the data from the data access method, or if the data is passed in the ``DataSource`` property and it should always use that data to perform all actions.

- **DataSource** - if the data is only read once, it can be passed in this property. ``AutoBind`` property must be set to false, so this data don't get override.

- **Visible** *(default: true)* - indicates wether a control is visible on page or not. The control may exist on the page so filters can be applied to him, but it may be invisble unless the filter has a specific value.

- **DataAccessClass** - the class to request or instantiate where the data access methods exist.

- **DataAccessParameters** - parameters required by the constructor of the ``DataAccessClass``. Only required when the constructor has parameters AND the global setting ``UseDependencyInjection`` is set to false.

- **SelectMethod** - the method used to select the data.

- **SelectParameters** - the parameters required by the ``SelectMethod``. Can be static parameters or filters that can be override later.

- **UpdateMethod** - the method used to update the data.

- **UpdateParameters** - the parameters required by the ``UpdateMethod``. The POCO model used to pass the data form the control is not required to be indicated here.

- **DatabaseModelType** - the Type of the POCO model that maps the data and is used by the ``UpdateMethod``.

- **EventHandlerClass** - the class where the custom events are defined for the control.

- **Filters** - a ``Dictionary<string, string>`` that contain the filters applied to the ``SelectMethod``. Normally this property is set by the filter attribute later.

- **AdditionalData** - a ``Dictionary<string, object>`` that can be used to store data that is used inside the control to render data such as options for a Select tag.

- **ViewPaths** - a ``ViewsPathsModel`` object that contain the paths to the views. At the moment, there is only a ``ViewPath`` property that must be set with the control partial path.



Filters
-------

Filters are global and can be set to any control existing on page, from any HTML element such as ``input``, ``select``, ``button``.

To use the filter, an attribute must be placed in the HTML element.

- **ncc-filter-targets** - allows to indicate the id's of the controls to whom the filter applies. Multiple controls may be set, with comma separated id's.

- **ncc-filter-ids** - the id(s) of the HTML element that contain the values of the filter. This option MUST exist if the filter attribute is placed in a ``button`` element.

- **ncc-js-events** - the javascript event that submits the filter. By default, ``onchange`` is used by Select, ``onkeyup`` is used by Input and ``onclick`` on Button.


.. note:: It is possible to set multiple inputs with different filters, and allow a button to submit all filters at once, using the ``ncc-filter-ids`` attribute.


Events
------

Events are pre-defined within the base and are also specific to controls.

Events that are common to all controls are:

- **Load**

- **DataBound**

- **PreRender**

- **PostBack**

To use an event, just add the following 2 attributes to any element:

- **ncc-event** - the name of the event to raise

- **ncc-event-target** - the id(s) of the controls that this event applies.

.. note:: It is possible to use these events to raise events that are specific to a control, that are not mentioned here.