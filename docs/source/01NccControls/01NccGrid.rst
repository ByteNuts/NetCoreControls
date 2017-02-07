NCC Grid
========

A grid control that renders data as a table.


Context
-------

The context class for the Grid control is ``NccGridContext``.

- **DataKeys** - a comma separated data keys for the data. These keys are not rendered on the client, and cannot be overriden by hidden fields.

- **DataKeysValues** *(read-only)* - a read-only ``List<Dictionary<string, object>>`` that contains all the data keys for the grid.

- **PageNumber** *(default: 1)* - page number to start from.

- **TotalItems** *(read-only)* - a read-only items counter.

- **AllowPaging** *(default: false)* - a flag indicating if the grid is paginated.

- **PageSize** *(default: 10)* - if ``AllowPaging`` is set to true, defines the size of a grid page.

- **PagerNavSize** *(default: 10)* - if ``AllowPaging`` is set to true, defines how many pages are shown in the navigation pager.

- **AutoGenerateEditButton** *(default:false)* - a flag indicating if the table is editable row by row.



Tags
----

The Grid control is composed by various tags that can be coupled together.

Each one of them contain custom attributes that can be set and some may override the settings placed on context.

<ncc-grid>
``````````

**Attributes**

- Context
- DataKeys
- AllowPaging
- RenderForm
- PageSize
- AutoGenerateEditButton
- PagerNavSize
- CssClass
- BodyCssClass
- HeaderCssClass
- FooterCssClass

**Allowed child tags**
- ncc:grid-content
- ncc:grid-columns


<ncc:grid-content>
``````````````````

**Attributes**

- ContentType

**Allowed parent tags**

- ncc:grid


<ncc:grid-columns>
``````````````````

**Allowed parent tags**

- ncc:grid

**Allowed child tags**

- ncc:grid-columnbound
- ncc:grid-columntemplate


<ncc:grid-columnbound>
``````````````````````

**Attributes**

- DataValue
- DataField
- HeaderText
- ShowHeader
- Visible
- Aggregate
- CssClass

**Allowed parent tags**

- ncc:grid


<ncc:grid-columntemplate>
`````````````````````````

**Attributes**

- ShowHeader
- Visible

**Allowed parent tags**

- ncc:grid-columns

**Allowed child tags**

- ncc:grid-headertemplate
- ncc:grid-itemtemplate
- ncc:grid-edittemplate


<ncc:grid-headertemplate>
`````````````````````````

**Attributes**

- CssClass

**Allowed parent tags**

- ncc:grid-columntemplate


<ncc:grid-itemtemplate>
```````````````````````

**Attributes**

- CssClass
- Aggregate

**Allowed parent tags**

- ncc:grid-columntemplate


<ncc:grid-edittemplate>
```````````````````````

**Attributes**

- CssClass
- Aggregate

**Allowed parent tags**

- ncc:grid-columntemplate



Events
------

For subscribing to these events, the ``EventHandlerClass`` property of the context must be set with the reference for a class that derives from ``NccGridEvents``.

The following are Grid specific events:

- **RowDataBound**
- **RowCreated**
- **Update**
- **UpdateRow**
- **DeleteRow**



Actions
-------

Actions allows the user to raise any of the referred grid events, and can be associated with any HTML element.

To use an action, the following two first attributes must be set:

- **ncc-grid-action** - the name of the action to raise.

- **ncc-grid-action-target** - the id(s) of the controls that will raise the event (it can raise Update simultaneously on multiple grids).

- **ncc-grid-row** *(optional)* - if the action requires the row number which will raise the event, this attribute must be set.

.. note:: Within the Grid, you can use ``@Model.NccRowNumber`` property to insert the row number.
