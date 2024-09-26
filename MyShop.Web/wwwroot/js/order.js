var dtable;
$(document).ready(function () {
    loaddata();
});
function loaddata() {

    dtable = $("#tableId").DataTable({

        "lengthMenu": [5, 10, 25, 50, 100],
        "language": {
            "lengthMenu": " Show _MENU_ Rows"
        },

        "ajax": {
            "url": "/Admin/Order/GetData",
            "type": "GET",
        },
        "columns": [
            { "data": "id" },
            { "data": "name" },
            { "data": "phoneNumber" },
            { "data": "applicationUser.email" },
            { "data": "orderStatus" },
            { "data": "totalPrice" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                           <a  href="/Admin/Order/Details?orderid=${data}"  class="btn btn-warning">Details</a>
                          
                           `
                }
            }
        ]
    });

}

function DeleteItem(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "Delete",
                success: function (data) {
                    if (data.success) {
                        dtable.ajax.reload();
                        toaster.success(data.message);
                    }
                    else {
                        toaster.error(data.message);
                    }
                }
            });

            Swal.fire({
                title: "Deleted!",
                text: "Your file has been deleted.",
                icon: "success"
            });
        }
    });
}