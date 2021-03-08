var dataTable;

$(document).ready(function () {
    loadList();
});

function loadList() {
    dataTable = $("#DT_load").DataTable({
        "ajax": {
            "url": "/api/category",
            "type": "GET",
            "dataType": "json"
        },
        "columnDefs": [
            { targets: [1], className: 'dt-body-center' }
        ],
        "columns": [
            { "data": "name", "width": "30%" },
            { "data": "displayOrder", "width": "20%" },
            {
                "data": "backgroundColor",
                "render": function (data) {
                    return `<div class="text-center text-white mt-1 pt-2" style="background-color: ${data}; height: 34px;">${data}</div>`;
                },
                "width": "20%"
            },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                                <a href="/Admin/Category/Upsert?id=${data}" class="btn btn-success text-white" style="cursor:pointer; width:40%">
                                    <i class="far fa-edit"></i> Edit
                                </a>
                                <a class="btn btn-danger text-white" style="cursor:pointer; width:40%" onclick="Delete('/api/category/${data}')">
                                    <i class="far fa-trash-alt"></i> Delete
                                </a>
                            </div>`;
                },
                "width": "30%"
            },
        ],
        "language": {
            "emptyTable": "No data found"
        },
        "width": "100%"
    });
}

function Delete(url) {
    swal({
        title: "Are you sure you want to delete?",
        text: "You will not be able to restore the data!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then(willDelete => {
        if (willDelete) {
            $.ajax({
                type: "delete",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}
