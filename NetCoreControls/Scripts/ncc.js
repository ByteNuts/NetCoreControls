var posId = 0;
var pref = "";
var ajax = true;
var form;

function nccAction(event, elem, params, prefix) {
    if (event != null) {
        event.preventDefault();
    }
    var nncActionModel = JSON.parse(params);
    var postData = [];
    posId = 0;
    pref = prefix;

    postData.push({
        name: "target_ids",
        value: nncActionModel.TargetIds
    });

    $.each(nncActionModel.Parameters,
        function (key, value) {
            if (value.toLowerCase() == "exportexcel") {
                ajax = false;
            };
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

    addElementParameters(postData, elem);

    $.each(nncActionModel.TargetIds, function () {
        var controlId = this;

        postData.push({
            name: "context",
            value: $("#" + controlId + "_context").val()
        });

        var div = $("#" + controlId);

        form = div.children("form");
        if (form.length === 0) {
            form = div.closest("form");
        }
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
        var basePath = document.getElementsByTagName('base')[0];
        var basePathUrl = "";
        if (basePath != null) {
            basePathUrl = basePath.href;
        }

        if (ajax) {
            var options = {
                type: "POST",
                url: basePathUrl + "/NetCoreControls/ControlAction",
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
        } else {
            $.each(postData, function (key, value) {
                var input = $("<input>").attr("type", "hidden").attr("name", value.name).val(value.value);
                form.append($(input));
            });
            form.attr('action', basePathUrl + "/NetCoreControls/ControlAction").attr('method', 'post').submit();

            //var form = $("<form>").attr("action", basePathUrl + "/NetCoreControls/ControlAction");
            //$.each(postData, function (key, value) {
            //    if (value.name == "target_ids" || value.name == "context" || value.value == "ncc-ActionType" || value.value == "Event" || value.value == "ncc-EventName" || value.value == "exportxlsx") {
            //        var input = $("<input>").attr("type", "hidden").attr("name", value.name).val(value.value);
            //        form.append($(input));
            //    }
            //});
            //form.submit();

        }
    }
}

function addElementParameters(postData, elem) {
    if (elem instanceof jQuery) {
        processElementData(postData, elem, "");
    } else {
        var elemCount = 0;
        $.each(elem, function () {
            var ctrl = $("#" + this);
            processElementData(postData, ctrl, elemCount);
            elemCount++;
        });
    }
}
function processElementData(postData, elem, elemCount) {
    appendDataToPostArray(postData, pref + "-ElemId" + elemCount, elem.prop("id"));
    posId++;

    appendDataToPostArray(postData, pref + "-ElemName" + elemCount, elem.prop("name"));
    posId++;

    if (elem.attr("value")) {
        appendDataToPostArray(postData, pref + "-ElemValue" + elemCount, elem.attr("value"));
    } else {
        appendDataToPostArray(postData, pref + "-ElemValue" + elemCount, elem.val());
    }
    posId++;
}

function appendDataToPostArray(postData, key, value) {
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
    //var change = false;
    $.each(targetIds,
        function () {
            var controlId = this;
            var div = $("#" + controlId);
            //if (elem instanceof jQuery) {
            //    if (div.attr("id") == elem.attr("id")) {
            //        change = true; };
            //} else {
            //    $.each(elem, function () {
            //        var ctrl = $("#" + this);
            //        if (div.attr("id") == ctrl.attr("id")) {
            //            change = true;
            //        };
            //    });
            //}
            //if (change) {
                div.find(".ajaxLoader, .overlayAjaxLoader").fadeIn(500);
                div.prop("disabled", true);
            //}
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