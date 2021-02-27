var dataTable;

$(document).ready(function () {
    var urlParams = new URLSearchParams(window.location.search);
    if (urlParams.get('status') === "cancelled") {
        loadList("cancelled");
    }
    else if (urlParams.get('status') === "completed") {
        loadList("completed");
    }
    else {
        loadList("");
    }
});

function loadList(status) {
    dataTable = $("#DT_load").DataTable({
        "ajax": {
            "url": "/api/orderpickup",
            "data": { status: status },
            "type": "GET",
            "dataType": "json",
        },
        "columnDefs": [
            { targets: [3], className: 'dt-body-right' }
        ],
        "columns": [
            { "data": "orderHeader.id", "width": "10%"},
            { "data": "orderHeader.pickupName", "width": "30%" },
            { "data": "orderHeader.applicationUser.email", "width": "20%" },
            { "data": "orderHeader.orderTotal", "width": "20%", render: $.fn.dataTable.render.number('.', ',', 2, 'R$ ') },
            {
                "data": "orderHeader.id",
                "render": function (data) {
                    return `<div class="text-center">
                                <a href="/Admin/Order/Details/${data}" class="btn btn-success text-white" style="cursor:pointer; width:50%">
                                    <i class="far fa-edit"></i> Details
                                </a>
                            </div>`;
                },
                "width": "30%"
            }
        ],
        "language": {
            "emptyTable": "No data found"
        },
        "width": "100%"
    });
}
