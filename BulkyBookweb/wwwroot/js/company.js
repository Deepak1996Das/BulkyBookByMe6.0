var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').dataTable({
        "ajax": {
            "url": "/Admin/Company/GetAll"
        },
        "columns": [
            { "data": "name", "width": "14%" },
            { "data": "streetAddress", "width": "14%" },
            { "data": "city", "width": "14%" },
            { "data": "state", "width": "14%" },
            { "data": "phoneNumber", "width": "14%" },
            {
                "data": "id",
                "render": function (data) {
                    return`
                    <div class="w-75 btn btn-group" role="group">

                             <a href="/Admin/Company/Upsert?id=${data}"  class="btn btn-primary mx-2">
                                <i class="bi bi-pencil-square"></i>&nbsp Edit
                            </a>
                             <a onClick=Delete('/Admin/Company/Delete?id=${data}') class="btn btn-danger">
                                <i class="bi bi-trash3"></i>&nbsp Delete
                            </a>
                    </div>
                    `
                },
                "width": "14%"
            }


        ]
    });
}

function Delete(url) 
{
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: True,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText:'yes,delete it!'
    }).then(result)=> {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.Message);
                    }
                    else {
                        toastr.error(data.Message);
                    }
                }
            })
        }
    }
}