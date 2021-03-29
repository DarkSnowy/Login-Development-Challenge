var vueApp;
var apiUrl = "https://localhost:44363/";

$(function () {
    // Create vue app.
    vueApp = new Vue({
        el: '#App',
        data: {
            displayLogin: true,
            displayUser: false,
            register: true,
            isStaff: false,
            isAdmin: false,
            id: null,
            edit: false,
            staff: false,
            loggedIn: false
        },
        methods: {
            onUserCancel(register) {
                this.displayUser = false;
                this.displayLogin = true;
            },
            onLogin(data) {
                // Save the token and info into a cookie for handling page is refreshes.
                $cookies.set('token', data.token, data.expiration);
                $cookies.set('roles', data.roles, data.expiration);

                setLogin(data.token, data.roles);
            },
            onNewRegister() {
                vueApp.register = true;
                vueApp.displayLogin = false;
                vueApp.displayUser = true;
            },
            onLogout() {
                $cookies.remove('token');
                $cookies.remove('roles');
                vueApp.displayLogin = true;
                vueApp.displayUser = false;
            },
            authFailed() {
                // If session expires or gains access to an operation they shouldn't be doing then log them out.
                vueApp.onLogout();
                // Alert the user what happened.
                alert("Authorisation failed. Session may have expired.");
                // Should consider, in the future, updating of session cookie if user is remaining active.
            }
        },
    });

    var token = $cookies.get('token');
    var roles = $cookies.get('roles');

    if (token)
        setLogin(token, roles);
    else
        setAjax(null);

    // Add a click event listener to the login button
    $("#submit-login").click(function () {
        // Get email and password when button is clicked.
        var email = $("#login-email-input").val();
        var password = $("#login-password-input").val();
        // Call SubmitLogin method.
        SubmitLogin(email, password);
    });
});

function setLogin(token, roles) {
    setAjax(token);

    if (jQuery.inArray("Staff", roles)) {
        isStaff = true;
    }

    if (jQuery.inArray("Admin", roles)) {
        isStaff = true;
        isAdmin = true;
    }

    vueApp.register = false;
    vueApp.displayLogin = false;
    vueApp.displayUser = true;
    vueApp.id = "me";
    vueApp.loggedIn = true;
}

// This sets up the ajax authorization header with the passed token.
function setAjax(token) {
    $.ajaxSetup({
        contentType: 'application/json; charset=utf-8',
        headers: {
            'Authorization': "Bearer " + token
        }
    })
}