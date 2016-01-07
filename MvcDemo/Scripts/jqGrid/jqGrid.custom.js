
// =====  Common Settings  =====

function SetCommonGridSettings() {
    var commonGridSettings =
        {
            rowList: [5, 10, 15, 20, 50],
            viewrecords: true,
            //rownumbers: true,
            //multiselect: true,
            //altRows: true,
            //altclass: 'myAltRowClass',
            height: '100%',
            autowidth: true,
            gridview: true,
            jsonReader: { cell: "" },
            prmNames: { search: "search", nd: null },
            loadError: SetReportError(),
            loadComplete: function () {
                // remove error div if exist
                SetClearError().call(this);
            }
        };
    return commonGridSettings;
}


// =====  Handler Errors in Grid  =====

function SetReportError() {
    var decodeErrorMessage = function (jqXHR, textStatus, errorThrown) {
        var html;
        var errorInfo;
        var i;
        var errorText = textStatus + '\n' + errorThrown;
        if (jqXHR.responseText.charAt(0) === '[') {
            try {
                errorInfo = $.parseJSON(jqXHR.responseText);
                errorText = "";
                for (i = 0; i < errorInfo.length; i++) {
                    if (errorText.length !== 0) {
                        errorText += "<hr/>";
                    }
                    errorText += errorInfo[i].Source + ":" + errorInfo[i].Message;
                }
            } catch (e) {
            }
        } else {
            html = /<body.*?>([\s\S]*)<\/body>/i.exec(jqXHR.responseText);
            if (html !== null && html.length > 1) {
                errorText = html[1];
            }
        }
        return errorText;
    };
    var reportError = function (jqXHR, textStatus, errorThrown) {
        // remove error div if exist
        $('#' + $.jgrid.jqID(this.id) + '_err').remove();
        // insert div with the error description before the grid
        $(this).closest('div.ui-jqgrid').before(
            '<div id="' + this.id + '_err" style="max-width:' + this.style.width +
                ';"><div class="ui-state-error ui-corner-all" style="padding:0.7em;"><span class="ui-icon ui-icon-alert" style="margin-right: .3em;"></span><span style="clear:left">' +
                    decodeErrorMessage(jqXHR, textStatus, errorThrown) + '</span></div><div style="clear:left"/></div>');
    };
    return reportError;
}

function SetClearError() {
    var clearError = function () {
        $('#' + $.jgrid.jqID(this.id) + '_err').remove();
    };
    return clearError;
}


// =====  Set Grid Bars  =====

function SetGridFilterToolbar(grid) {
    grid.jqGrid('filterToolbar', {
        stringResult: true,
        searchOnEnter: true,
        defaultSearch: 'cn',
        searchOperators: true
    });
}

function SetGridNavGrid(grid, pagerId) {
    grid.jqGrid('navGrid', pagerId, { add: false, edit: false, del: true, refreshstate: 'current' },
            {}, // edit 
            {}, // add 
            {}, // delete
            {multipleSearch: true, showQuery: true });
}

function SetGridNavButtonAdd(grid, pagerId, dialogBox, addNewRowAction, editRowAction) {
    grid.jqGrid('navButtonAdd', pagerId, {
        caption: '',
        buttonicon: "ui-icon ui-icon-plus",
        title: "Add new row",
        onClickButton: function () {
            window.location = addNewRowAction;
        }
    });

    grid.jqGrid('navButtonAdd', pagerId, {
        caption: '',
        id: $.jgrid.jqID(grid[0].id) + '_Edit', //the id is unique
        buttonicon: "ui-icon ui-icon-pencil",
        title: "Edit selected row",
        onClickButton: function () {
            var id = grid.getGridParam('selrow');
            if (id) {
                var data = grid.getRowData(id);
                var link = editRowAction;
                link = link.replace("0", data.ID);
                window.location = link;
            } else {
                ShowDialog(dialogBox, 'Warning', 'Please, select row');
            }
        }
    });

    //move Delete button after the Edit button
    $("#del_" + $.jgrid.jqID(grid[0].id)).insertAfter("#" + $.jgrid.jqID(grid[0].id) + "_Edit");
}

// =====  Resize Grid  =====

function AutoResizeGrid(grid, gridContainer) {
    // Auto resize grid
    $(window).bind('resize', function () {
        var width = gridContainer.width();
        if (width == null || width < 1) {
            // For IE, revert to offsetWidth if necessary
            width = gridContainer.attr('offsetWidth');
        }

        width = width - 2; // Fudge factor to prevent horizontal scrollbars
        if (width > 0 &&
        // Only resize if new width exceeds a minimal threshold
        // Fixes IE issue with in-place resizing when mousing-over frame bars
                Math.abs(width - grid.width()) > 5) {
            grid.setGridWidth(width);
        }

    }).trigger('resize');
}


// =====  DropDownd Search Column  =====

function GetUniqueNames(grid, columnName) {
    var texts = grid.jqGrid('getCol', columnName), uniqueTexts = [],
        textsLength = texts.length, text, textsMap = {}, i;
    for (i = 0; i < textsLength; i++) {
        text = texts[i];
        if (text !== undefined && textsMap[text] === undefined) {
            // to test whether the texts is unique we place it in the map.
            textsMap[text] = true;
            uniqueTexts.push(text);
        }
    }
    return uniqueTexts;
};
function BuildSearchSelect(grid, uniqueNames) {
    var values = ":All";
    $.each(uniqueNames, function () {
        values += ";" + this + ":" + this;
    });
    return values;
};


