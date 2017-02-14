
function nccAction(event, elem, params, prefix) {
    if (event != null) {
        event.preventDefault();
    }
    var nncActionModel = JSON.parse(params);
    var postData = [];
    var posId = 0;

    postData.push({
        name: "target_ids",
        value: nncActionModel.TargetIds
    });

    $.each(nncActionModel.Parameters,
        function (key, value) {
            postData.push({
                name: "parameters[" + posId + "].key",
                value: key
            });
            postData.push({
                name: "parameters[" + posId + "].value",
                value: value
            });
            posId++;
        });

    addElementParameters(postData, elem, posId, prefix);

    $.each(nncActionModel.TargetIds, function () {
        var controlId = this;

        postData.push({
            name: "context",
            value: $("#" + controlId + "_context").val()
        });

        var div = $("#" + controlId);

        var form = div.children("form");
        if (form.length !== 0) {
            $.each(form.serializeArray(), function (_, field) {
                var fieldName = controlId + "." + field.name;
                if (field.name.startsWith("[")) {
                    fieldName = controlId + field.name;
                }
                postData.push({
                    name: fieldName,
                    value: field.value
                });
            });
        }
    });
    nccPost(nncActionModel.TargetIds, postData, elem);
}


function nccPost(controlIds, postData, elem) {
    if (postData != null && controlIds != null) {
        var options = {
            type: "POST",
            url: "/NetCoreControls/ControlAction",
            data: postData,
            success: function (data) {
                for (var i = 0; i < data.length; i++) {
                    if (data[i] != "") {
                        var update = $("#" + controlIds[i]);
                        $(update).replaceWith(data[i]);
                    }
                }
            },
            beforeSend: function() { showAjaxLoader(controlIds, elem) },
            complete: function() { hideAjaxLoader(controlIds, elem) }
        }
        $.ajax(options);
    }
}

function addElementParameters(postData, elem, posId, prefix) {
    if (elem instanceof jQuery) {
        processElementData(postData, elem, posId, prefix, "");
    } else {
        var elemCount = 0;
        $.each(elem, function () {
            var ctrl = $("#" + this);
            processElementData(postData, ctrl, posId, prefix, elemCount);
            elemCount++;
        });
    }
}
function processElementData(postData, elem, posId, prefix, elemCount) {
    appendDataToPostArray(postData, prefix + "-ElemId" + elemCount, elem.prop("id"), posId);
    posId++;

    appendDataToPostArray(postData, prefix + "-ElemName" + elemCount, elem.prop("name"), posId);
    posId++;

    if (elem.attr("value")) {
        appendDataToPostArray(postData, prefix + "-ElemValue" + elemCount, elem.attr("value"), posId);
    } else {
        appendDataToPostArray(postData, prefix + "-ElemValue" + elemCount, elem.val(), posId);
    }
    posId++;
}

function appendDataToPostArray(postData, key, value, posId) {
    postData.push({
        name: "parameters[" + posId + "].key",
        value: key
    });
    postData.push({
        name: "parameters[" + posId + "].value",
        value: value
    });
}



function showAjaxLoader(targetIds, elem) {
    var change = false;
    $.each(targetIds,
        function () {
            var controlId = this;
            var div = $("#" + controlId);
            if (elem instanceof jQuery) {
                if (div.attr("id") == elem.attr("id")) {
                    change = true; };
            } else {
                $.each(elem, function () {
                    var ctrl = $("#" + this);
                    if (div.attr("id") == ctrl.attr("id")) {
                        change = true;
                    };
                });
            }
            if (change) {
                div.find(".ajaxLoader, .overlayAjaxLoader").fadeIn(500);
                div.prop("disabled", true);
            }
        });
    if (elem instanceof jQuery) {
        elem.prop( "disabled", true );
    } else {
        $.each(elem, function () {
            var ctrl = $("#" + this);
            ctrl.prop("disabled", true);
        });
    }
}
function hideAjaxLoader(targetIds, elem) {
    $.each(targetIds,
        function () {
            var controlId = this;
            var div = $("#" + controlId);
            div.find(".ajaxLoader, .overlayAjaxLoader").fadeOut(500);
            div.prop("disabled", false);
        });
    if (elem instanceof jQuery) {
        elem.prop("disabled", false);
    } else {
        $.each(elem, function () {
            var ctrl = $("#" + this);
            ctrl.prop("disabled", false);
        });
    }
}