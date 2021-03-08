var dataTable;

$(document).ready(function () {
    loadList();
});

function loadList() {
    dataTable = $("#DT_load").DataTable({
        "ajax": {
            "url": "/api/MenuItem",
            "type": "GET",
            "dataType": "json"
        },
        "columnDefs": [
            { targets: [1], className: 'dt-body-right' },
            { targets: [2, 3], className: 'dt-body-center' }
        ],
        "columns": [
            { "data": "name", "width": "25%" },
            { "data": "price", "width": "15%", render: $.fn.dataTable.render.number('.', ',', 2, 'R$ ') },
            { "data": "category.name", "width": "15%" },
            { "data": "foodType.name", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                                <a href="/Admin/MenuItem/Upsert?id=${data}" class="btn btn-success text-white" style="cursor:pointer; width:40%">
                                    <i class="far fa-edit"></i> Edit
                                </a>
                                <a class="btn btn-danger text-white" style="cursor:pointer; width:40%" onclick="Delete('/api/MenuItem/${data}')">
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
        "width": "100%",
        "order": [[2, "asc"]]
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
