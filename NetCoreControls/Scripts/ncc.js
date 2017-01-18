
function nccAction(event, elem, params, prefix) {
    if (event != null) {
        event.preventDefault();
    }
    var nncActionModel = JSON.parse(params);

    $.each(nncActionModel.TargetIds, function () {
        var postData;
        var controlId = this;

        var div = $("#" + controlId);
        var form = div.children("form");
        var paramId = 0;
        var elemCount = 0;
        var elemIds;
        if (form.length === 0) {
            postData = {};
            postData["context"] = $("#" + controlId + "_context").val();
            $.each(nncActionModel.Parameters,
                function (key, value) {
                    postData["parameters[" + paramId + "].key"] = key;
                    postData["parameters[" + paramId + "].value"] = value;
                    paramId++;
                });

            //Add element properties
            postData = addElementToPostData(postData, elem, paramId, prefix);
        } else {
            form.append("<input type='hidden' name='context' value='" + $("#" + controlId + "_context").val() + "' />");
            $.each(nncActionModel.Parameters,
                function (key, value) {
                    form.append("<input type='hidden' name='parameters[" + paramId + "].key' value='" + key + "' />");
                    form.append("<input type='hidden' name='parameters[" + paramId + "].value' value='" + value + "' />");
                    paramId++;
                });

            //Add element properties
            form = addElementToForm(form, elem, paramId, prefix);
            postData = form.serializeArray();
        }
        nccPost(controlId, postData);
    });
}


function nccPost(controlId, postData) {
    if (postData != null && controlId != null) {
        var options = {
            type: "POST",
            url: "/NetCoreControls/ControlAction",
            data: postData,
            success: function (data) {
                var update = $("#" + controlId);
                $(update).replaceWith(data);
            }
        }
        $.ajax(options);
    }
}

function addElementToForm(form, elem, paramId, prefix) {
    if (elem instanceof jQuery) {
        form.append("<input type='hidden' name='parameters[" + paramId + "].key' value='" + prefix + "-ElemId' />");
        form.append("<input type='hidden' name='parameters[" + paramId + "].value' value='" + elem.prop('id') + "' />");
        paramId++;
        form.append("<input type='hidden' name='parameters[" + paramId + "].key' value='" + prefix + "-ElemName' />");
        form.append("<input type='hidden' name='parameters[" + paramId + "].value' value='" + elem.prop('name') + "' />");
        paramId++;
        form.append("<input type='hidden' name='parameters[" + paramId + "].key' value='" + prefix + "-ElemValue' />");
        form.append("<input type='hidden' name='parameters[" + paramId + "].value' value='" + elem.val() + "' />");
        paramId++;
    } else {
        var elemCount = 0;
        $.each(elem, function () {
            var ctrl = $("#" + this);
            form.append("<input type='hidden' name='parameters[" + paramId + "].key' value='" + prefix + "-ElemId" + elemCount + "' />");
            form.append("<input type='hidden' name='parameters[" + paramId + "].value' value='" + ctrl.prop('id') + "' />");
            paramId++;
            form.append("<input type='hidden' name='parameters[" + paramId + "].key' value='" + prefix + "-ElemName" + elemCount + "' />");
            form.append("<input type='hidden' name='parameters[" + paramId + "].value' value='" + ctrl.prop("name") + "' />");
            paramId++;
            form.append("<input type='hidden' name='parameters[" + paramId + "].key' value='" + prefix + "-ElemValue" + elemCount + "' />");
            form.append("<input type='hidden' name='parameters[" + paramId + "].value' value='" + ctrl.val() + "' />");
            paramId++;
            elemCount++;
        });
    }
    return form;
}

function addElementToPostData(postData, elem, paramId, prefix) {
    if (elem instanceof jQuery) {
        postData["parameters[" + paramId + "].key"] = prefix + "-ElemId";
        postData["parameters[" + paramId + "].value"] = elem.prop("id");
        paramId++;
        postData["parameters[" + paramId + "].key"] = prefix + "-ElemName";
        postData["parameters[" + paramId + "].value"] = elem.prop("name");
        paramId++;
        postData["parameters[" + paramId + "].key"] = prefix + "-ElemValue";
        postData["parameters[" + paramId + "].value"] = elem.val();
        paramId++;
    } else {
        var elemCount = 0;
        $.each(elem, function () {
            var ctrl = $("#" + this);
            postData["parameters[" + paramId + "].key"] = prefix + "-ElemId" + elemCount;
            postData["parameters[" + paramId + "].value"] = ctrl.prop("id");
            paramId++;
            postData["parameters[" + paramId + "].key"] = prefix + "-ElemName" + elemCount;
            postData["parameters[" + paramId + "].value"] = ctrl.prop("name");
            paramId++;
            postData["parameters[" + paramId + "].key"] = prefix + "-ElemValue" + elemCount;
            postData["parameters[" + paramId + "].value"] = ctrl.val();
            paramId++;
            elemCount++;
        });
    }
    return postData;
}