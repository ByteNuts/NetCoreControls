NCC Select
==========

A select HTML tag that allows linking other controls and creates dependencies among the data loaded by each control.


Context
-------

The context class for the Grid control is ``NccSelectContext``.

- **TextValue** - a string name of a property that will be rendered as the text of the option.

- **DataValue** - a string name of a property that will be rendered as the value of the option.

- **SelectedValue** - the default value to be automatic selected.

- **FirstItem** - the value that the first element must contain. The value for this item is always an empty string.


Tags
----

The Select control is composed by a single tag.

Each custom attributes set on the tag will override the settings placed on context.

<ncc-select>
``````````

**Attributes**

- Context
- TextValue
- DataValue
- SelectedValue
- FirstItem


Events
------

For subscribing to these events, the ``EventHandlerClass`` property of the context must be set with the reference for a class that derives from ``NccSelectEvents``.

The following are Select specific events:

- **OptionBound** - fires on each option before added to select



Link control
------------

To link this select with other controls on page, whether they are ncc:select controls, or any other NetCoreControls, their id's must be included in a specific attribute tag:

- **ncc-link-targets** - a comma separated and ordenated list of the controls that load their data according to the previous control selected.


.. note:: At the moment, **ncc-link-targets** can only be placed on NccSelect controls. Nevertheless, they can contain NccGrid id's for example, but any action taken on the grid will not have any efect on the NccSelect controls. Please, see samples for a working example.
