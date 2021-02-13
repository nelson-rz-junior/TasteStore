
function validateInputs() {
    let pickupName = document.getElementById('txtName').value;
    let pickupDate = document.getElementById('datepicker').value;
    let pickupTime = document.getElementById('timepicker').value;
    let pickupPhoneNumber = document.getElementById('txtPhone').value;

    let warningMessage = '';

    if (pickupName === '') {
        warningMessage = "Please enter a pick up name";
    }
    else if (pickupPhoneNumber === '') {
        warningMessage = "Please enter a pick up phone number";
    }
    else if (pickupDate.toString() === '') {
        warningMessage = "Please enter a pick up date";
    }
    else if (pickupTime.toString() === '') {
        warningMessage = "Please enter a pick up time";
    }

    if (warningMessage === '') {
        return true;
    }
    else {
        swal("Warning", warningMessage, "warning");
        return false;
    }
}

function createOrder(publishableKey) {
    let userId = document.getElementById('userId').value;
    let pickupName = document.getElementById('txtName').value;
    let pickupDate = document.getElementById('datepicker').value;
    let pickupTime = document.getElementById('timepicker').value;
    let pickupPhoneNumber = document.getElementById('txtPhone').value;
    let pickupComments = document.getElementById('txtComments').value;

    let saveOrderData = {
        userId: userId,
        pickupName: pickupName,
        pickupDate: moment(pickupDate, 'DDMMYYYY').format(),
        pickupTime: moment(pickupDate, 'DDMMYYYY').add(pickupTime).format(),
        pickupPhoneNumber: pickupPhoneNumber,
        pickupComments: pickupComments
    };

    $.ajax({
        url: '/api/order/save',
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        data: JSON.stringify(saveOrderData),
        success: function (response) {
            if (response.success) {
                createSession(publishableKey, userId, response.orderId);
            }
            else {
                toastr.error(saveOrderResponse.message);
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            let error = JSON.parse(xhr.responseText);
            toastr.error(`An error occurred while creating an order: ${error.errors.$[0]}`);
        }
    });
}

function createSession(publishableKey, userId, orderId) {
    $.ajax({
        url: '/api/order/stripe/createsession',
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        data: JSON.stringify({
            userId: userId,
            orderId: orderId
        }),
        success: function (response) {
            if (response.success) {
                let stripe = Stripe(publishableKey);

                stripe.redirectToCheckout({
                    sessionId: response.sessionId
                }).then(function (result) {
                    toastr.error(result.error.message);
                });
            }
            else {
                toastr.error(response.message);
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            let error = JSON.parse(xhr.responseText);
            toastr.error(`An error occurred while creating a session: ${error.errors.$[0]}`);
        }
    });
}

function placeOrder(publishableKey) {
    let checkoutButton = document.getElementById('btnPlaceOrder');

    checkoutButton.addEventListener('click', function () {
        let validate = validateInputs();

        if (validate) {
            createOrder(publishableKey);
        }
    });
}
