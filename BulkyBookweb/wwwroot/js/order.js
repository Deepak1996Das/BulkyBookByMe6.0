var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess"))
    {
        loadDataTable("inprocess");
    }
    else
    {
        if (url.includes("complited"))
        {
            loadDataTable("complited");
        }
        else
        {
            if (url.includes("pending"))
            {
                loadDataTable("pending");
            }
            else
            {
                if (url.includes("approved")) {
                    loadDataTable("approved");
                }
                else  {
                    loadDataTable("all");
                }
            }
        }
    }
    
});

function loadDataTable(status) {
    dataTable = $('#tblData').dataTable({
        "ajax": {
            "url": "/Admin/Order/GetAll?status="+status
        },
        "columns": [
            { "data": "id", "width": "14%" },
            { "data": "name", "width": "14%" },
            { "data": "phoneNumber", "width": "14%" },
            { "data": "applicationUser.email", "width": "14%" },
            { "data": "oderStatus", "width": "14%" },
            { "data": "orderTotal", "width": "14%" },
            {
                "data": "id",
                "render": function (data) {
                    return`
                    <div class="w-75 btn btn-group" role="group">

                             <a href="/Admin/Order/Details?OrderId=${data}"  class="btn btn-primary mx-2">
                                <i class="bi bi-pencil-square"></i>&nbsp Details
                            </a>
                           
                    </div>
                    `
                },
                "width": "14%"
            }


        ]
    });
}

