var vueLoginApp;

$(function () {
    $("#submit-login-spinner").hide();
    $("#submit-register-spinner").hide();

    // Create vue Login app.
    vueLoginApp = new Vue({
        el: '#LoginApp',
        data: {
            Error: '',
            Unauthenticated: true
        }
    });

    var token = $cookies.get('token');
    SetupAjax(token);

    // Add a click event listener to the login button
    $("#submit-login").click(function () {
        // Get email and password when button is clicked.
        var email = $("#login-email-input").val();
        var password = $("#login-password-input").val();
        // Call SubmitLogin method.
        SubmitLogin(email, password);
    });
});

function SubmitLogin(email, password) {
    if (!email) {
        vueLoginApp.Error = "Email is missing";
        return;
    }

    if (!password) {
        vueLoginApp.Error = "Password is missing";
        return;
    }

    try {
        $("#submit-login-spinner").show();
        $("#submit-login-spinner").prop('disabled', true);

        // Set error as empty on new attempt.
        vueLoginApp.Error = "";

        // Send login details to server.
        $.post("https://localhost:44363/Users/Login",
            JSON.stringify({ "email": email, "password": password })
        ).fail(function (error) {
            // If it failed then inform the user why. We set the returned message ourselves so we know it is safe for display.
            vueLoginApp.Error = error.responseJSON.message;
        }).done(function (data) {
            // Save the oauth token into a cookie for in case the page is refreshed.
            $cookies.set('token', data.token, data.expiration);
            // Add the token to the ajax header.
            SetupAjax(data.token);
            Vue.Unauthenticated = false;
        }).always(function () {
            $("#submit-login-spinner").hide();
            $("#submit-login-spinner").prop('disabled', false);
        });
    } catch {
        // If there is an error posting the request then don't leave the button loading forever.
        $("#submit-login-spinner").hide();
        $("#submit-login-spinner").prop('disabled', false);
        // Let the user know that something has gone wrong
        vueLoginApp.Error = "Request failed";
    }
}

// This sets up the ajax authorization header with the passed token.
function SetupAjax(token) {
    $.ajaxSetup({
        contentType: 'application/json; charset=utf-8',
        headers: {
            'Authorization': token
        }
    })
}