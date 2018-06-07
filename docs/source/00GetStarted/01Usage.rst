Basic Usage
===========

Just use any of the available controls using the corresponding taghelper tag.

All tags are prefixed with ``ncc:``.

All taghelpers that are attributes are prefixed with ``ncc-``.


Basic control usage
-------------------

To use any control, two steps are required:

* Create a control context
* Use the tag of the control

The example of a basic Grid is as follows.

**1. Create a method that gets the data from a datasource such as a database (the example uses Dapper for data access)**::

    public List<dynamic> GetProductList()
    {
      var sqlCommand = $@" SELECT * FROM Products";
      return Task.Factory.StartNew(() =>
      {
        using (var connection = new SqlConnection(_connStrings.Value.LocalDb))
          return connection.Query<dynamic>(sqlCommand);
        }).Result.ToList();
      }
    }


**2. On your Controller define a NccContext (e.g. ``NccGridContext``) object and set required parameters.
Pass it to the View using ``Model``, ``ViewBag``, ``ViewData`` or any other method**

.. code-block:: csharp

  using ByteNuts.NetCoreControls.Models.Grid;
  ...
  var context = new NccGridContext
  {
    Id = "SimpleGrid",
    DataAccessClass = typeof(IDataAccess).AssemblyQualifiedName,
    SelectMethod = "GetProductList",
    UseDependencyInjection = true,
    ViewPaths = new ViewsPathsModel { ViewPath = "/Views/NccGrid/SimpleGrid.cshtml"}
  };
  ViewData[context.Id] = context;


**3. On your View simply use the tag helper ``ncc:grid``, along with the available nested tags.
Set the grid context, and use the @Model inside the control tags to access all the properties available in each list item**

:: 

  @using ByteNuts.NetCoreControls.Models.Grid
  @{
    var context = ViewData["SimpleGrid"] as NccGridContext;
  }
  <ncc:grid Context="@context">
    <ncc:grid-columns>
      <ncc:grid-columnbound DataValue="@Model.ProductID" HeaderText="Product Id"></ncc:grid-columnbound>
      <ncc:grid-columnbound DataValue="@Model.ProductName" HeaderText="Product Name"></ncc:grid-columnbound>
      <ncc:grid-columnbound DataValue="@Model.SupplierID" HeaderText="Supplier ID"></ncc:grid-columnbound>
      <ncc:grid-columnbound DataValue="@Model.CategoryID" HeaderText="Category ID"></ncc:grid-columnbound>
      <ncc:grid-columnbound DataValue="@Model.QuantityPerUnit" HeaderText="Quantity Per Unit"></ncc:grid-columnbound>
      <ncc:grid-columnbound DataValue="@($"{Model.UnitPrice:0.00} €")" HeaderText="Unit Price"></ncc:grid-columnbound>
      <ncc:grid-columnbound DataValue="@Model.UnitsInStock" HeaderText="Units In Stock"></ncc:grid-columnbound>
      <ncc:grid-columnbound DataValue="@Model.UnitsOnOrder" HeaderText="Units On Order"></ncc:grid-columnbound>
      <ncc:grid-columnbound DataValue="@Model.ReorderLevel" HeaderText="Reorder Level"></ncc:grid-columnbound>
      <ncc:grid-columnbound DataValue="@(Model.Discontinued ? "Discontinued" : "Active")" HeaderText="Discontinued"></ncc:grid-columnbound>
    </ncc:grid-columns>
  </ncc:grid>
  

.. warning:: The grid shall be placed alone in a partial view. When an action occurs on the control, the partial view is full rendered.


Filters
-------

Filters are an attribute taghelper and are global to all controls.

One single filter can be applied to multiple controls.

Filters can be applied to ``input``, ``select`` and ``button`` html tags, just using the ``ncc-filter-targets`` as follow::

    <select ncc-filter-targets="MultiGridWithFilter1,MultiGridWithFilter2" name="orderId" asp-items="@(new SelectList(ViewData["Orders"] as IEnumerable, "OrderID", "OrderID"))" >
      <option value="">--- Choose an order ---</option>
    </select>


Events
------

To be able to use events, the class containing these events must inherit from the control events class.

Beside inheriting, it must be referenced in the control context that is created, using the property ``EventHandlerClass``:

e.g. ``EventHandlerClass = typeof(ExampleGridEvents).AssemblyQualifiedName``

The event handler class must inherit from one of the following two:

* **ByteNuts.NetCoreControls.Controls.NccEvents** --> this class defines the shared events;
* **ByteNuts.NetCoreControls.Controls.[ControlName].Events.[ControlName]Events** --> this class defines control specific events.

.. hint:: If you inherit from the base events class NccEvents, you may only subscribe to control shared events and not to control specific events.