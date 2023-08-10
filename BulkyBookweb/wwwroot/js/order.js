var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').dataTable({
        "ajax": {
            "url": "/Admin/Order/GetAll"
        },
        "columns": [
            { "data": "id", "width": "14%" },
            { "data": "name", "width": "14%" },
            { "data": "phonenNumber", "width": "14%" },
            { "data": "applicationUser.email", "width": "14%" },
            { "data": "orderStatus", "width": "14%" },
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

