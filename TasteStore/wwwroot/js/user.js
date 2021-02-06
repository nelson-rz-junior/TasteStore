var dataTable;

$(document).ready(function () {
    loadList();
});

function loadList() {
    dataTable = $("#DT_load").DataTable({
        "ajax": {
            "url": "/api/user",
            "type": "GET",
            "dataType": "json"
        },
        "columns": [
            { "data": "fullName", "width": "20%" },
            { "data": "email", "width": "20%" },
            { "data": "phoneNumber", "width": "20%" },
            {
                "data": { id: "id", lockoutEnd: "lockoutEnd" },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();

                    var result = `<div class="text-center">
                                       <a href="/Identity/Account/Manage?userId=${data.id}" class="btn btn-info text-white mr-1" style="cursor:pointer; width:30%">
                                           <i class="far fa-edit"></i> Edit
                                       </a>`;

                    if (lockout > today) {
                        // Currently user is locked
                        result += `<a class="btn btn-danger text-white" style="cursor:pointer; width:30%" onclick="LockUnlock('${data.id}', 'unlock')">
                                       <i class="fas fa-lock-open"></i> Unlock
                                   </a>`;
                    }
                    else {
                        // Currently user is unlocked
                        result += `<a class="btn btn-success text-white" style="cursor:pointer; width:30%" onclick="LockUnlock('${data.id}', 'lock')">
                                       <i class="fas fa-lock"></i> Lock
                                   </a>`;
                    }

                    result += `</div>`;

                    return result;
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

function LockUnlock(id, operation) {
    swal({
        title: `Are you sure you want to ${operation} the user?`,
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then(willDelete => {
        if (willDelete) {
            $.ajax({
                type: "post",
                url: "/api/user",
                data: JSON.stringify(id),
                contentType: "application/json",
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
