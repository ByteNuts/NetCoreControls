# NetCoreControls
.NET Core UI Controls

## Requirements
This controls relies on the **jQuery** library that must be referenced

## Installation
1 - Install the NetCoreControls NuGet Package
```
Install-Package ByteNuts.NetCoreControls
```

2 - Add the reference to the controls scripts file
```
<script type="text/javascript" src="@Url.Action("GetNccJsFile", "NetCoreControls")"></script>
```

## Available UI controls
###### GridView

###### HtmlRender


## Usage
1 - On your Controller define a control context (e.g. `GridViewContext`) object and set required parameters. Pass it to the View using Model, ViewBag, ViewData or any other way.
```
using ByteNuts.NetCoreControls.Models.GridView;
...
var controlContext = new GridViewContext
{
	Id = "exampleGrid",
	DataAccessClass = typeof(Services.Data.IData).AssemblyQualifiedName,
	UseDependencyInjection = true,
	SelectMethod = "GetUserList",
	SelectParameters = new { appId = AppId, departmentId = DepartmentId }.NccToExpando(),
	ViewPaths = new ViewsPathsModel { ViewPath = "/Views/Example/ExampleGrid/_exampleGrid.cshtml" }
};

ViewData["exampleGrid"] = controlContext;

```
2 - On your View simply use the tag helper `ncc:grid-view`, along with the available nested tags.
Set the grid context, and use the `@Model` to access all the properties available in each list item.
The list can be a strong typed or just a dynamic object.
```
@{
    var controlContext = (GridViewContext)ViewData["exampleGrid"];
}

<ncc:grid-view Context="@context" RenderForm="true" CssClass="table-striped" HeaderCssClass="exampleGridHeader">
    <ncc:columns>
        <ncc:template-field>
            <ncc:header-template>Photo</ncc:header-template>
            <ncc:item-template>
				<img src="@Model.Photo" alt="@Model.FirstName @Model.LastName" />
            </ncc:item-template>
        </ncc:template-field>
        <ncc:bound-field DataValue="@Model.FirstName" HeaderText="First Name"></ncc:bound-field>
        <ncc:bound-field DataValue="@Model.LastName" HeaderText="Last Name"></ncc:bound-field>
        <ncc:bound-field DataValue="@Model.DepartmentName" HeaderText="Department"></ncc:bound-field>
        <ncc:bound-field DataValue="@($"{Model.DepartmentName:dd-MM-yyyy}")" HeaderText="Birth Date"></ncc:bound-field>
    </ncc:columns>
</ncc:grid-view>
```

> The grid must be placed alone in the View, or be rendered from a Partial View.

## Filters
Filters can be applied to any control.
They can override or add a parameter to the `SelectParameters` property defined on the control context.
Can be placed inside or outside the control, anywhere on the page.
To use the events, you must set the `name` attribute of the HTML element to the exact name of the `SelectParameters` property, and use the attribute `ncc-filter-targets`.
```
<select name="departmentId" ncc-filter-targets="exampleGrid" asp-items="@(new SelectList(ViewBag.DepartmentsList, "DepartmentId", "DepartmentName"))" class="form-control"></select>
```

## Events
To be able to use events, the class containing these events must be referenced in the context using the property `EventHandlerClass`:
e.g. `EventHandlerClass = typeof(ExampleGridEvents).AssemblyQualifiedName`
This class must inherit from one of the following two:
- `ByteNuts.NetCoreControls.Controls.NccEvents` - this class defines the shared events;
- `ByteNuts.NetCoreControls.Controls.[ControlName].Events.[ControlName]Events` - this class defines control specific events.

If you wish to use specific control events, you must inherit from the latter one.

There are three types of events:
- Shared control events
- Control specific events
- Custom defined events

#### Shared control events
These events area available to all controls in the library.
They are available within the enum `ByteNuts.NetCoreControls.Models.Enums.NccEvents` and are:
- `Load` - occurs when the control is initialized;
- `DataBound` - occurs after the data is obtained from the data source;
- `PreRender` - occurs just before the control is rendered;
- `PostBack` - occurs when a call to the server is made.

#### Control specific events
These events are available only to a specific control.
They are available within the enum `ByteNuts.NetCoreControls.Models.Enums.[ControlName]Events`.
E.g. for the GridView, the events are:
- `RowDataBound`;
- `RowCreated`;
- `Update`;
- `DeleteRow`

#### Custom defined events
These events can be specified by the user inside the grid on an HTML element.
The event names are defined by the user and shall be placed on the element using simultaneously two attributes:
- `ncc-event` - the name of the method that the event calls,
- `ncc-event-target` - 


> Please, refer to source to get some examples and see a live demo

> Note that this package is in beta stage and may contain some bugs. Any contributions are well appreciated.

