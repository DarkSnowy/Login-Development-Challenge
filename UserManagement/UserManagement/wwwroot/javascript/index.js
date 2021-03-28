var vueApp;

$(function () {
    // Create vue app.
    vueApp = new Vue({
        el: '#App',
        data: {
            displayLogin: true,
            displayUser: false,
            register: true,
            isStaff: false,
            isAdmin: false
        },
        components: {
            'login-form': {},
            'user-form': {}
        },
        methods: {
            onUserCancel(value) {
                this.displayUser = false;
                this.displayLogin = true;
            },
            onLogin(token) {
                SetupAjax(token);
                this.displayLogin = false;
                this.register = false;
                this.displayUser = true;
            },
            onNewRegister() {
                //this.$refs.UserComponent.register = true;
                this.displayUser = true;
                this.displayLogin = false;
            }
        },
    });

    var token = null;// = $cookies.get('token');

    if (token) {
        vueApp.displayLogin = false;
        vueApp.displayUser = true;
    }

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

// This sets up the ajax authorization header with the passed token.
function SetupAjax(token) {
    $.ajaxSetup({
        contentType: 'application/json; charset=utf-8',
        headers: {
            'Authorization': token
        }
    })
}